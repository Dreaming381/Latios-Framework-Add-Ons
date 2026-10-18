using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BurstWaitress.Asm
{
    /// <summary>
    /// Reads the text Burst's GetDisassembly returns: LLVM assembler output (Intel syntax on x86) with every function
    /// the module emits, including the entry stub, Burst's initializers, non-inlined callees and library routines.
    /// </summary>
    public static class AsmParser
    {
        static readonly Regex s_CvLoc = new Regex(@"^\s*\.cv_loc\s+\d+\s+(\d+)\s+(\d+)", RegexOptions.Compiled);
        static readonly Regex s_DwarfLoc = new Regex(@"^\s*\.loc\s+(\d+)\s+(\d+)", RegexOptions.Compiled);
        static readonly Regex s_CvFile = new Regex("^\\s*\\.cv_file\\s+(\\d+)\\s+\"((?:[^\"\\\\]|\\\\.)*)\"", RegexOptions.Compiled);
        static readonly Regex s_DwarfFile = new Regex("^\\s*\\.file\\s+(\\d+)\\s+\"((?:[^\"\\\\]|\\\\.)*)\"(?:\\s+\"((?:[^\"\\\\]|\\\\.)*)\")?", RegexOptions.Compiled);
        static readonly Regex s_Export = new Regex("/EXPORT:\\\\?\"?([^\"\\\\]+)", RegexOptions.Compiled);

        public static Isa IsaForTarget(string target)
        {
            var t = (target ?? "").ToUpperInvariant();
            if (t.StartsWith("ARMV8", StringComparison.Ordinal) || t.StartsWith("ARMV9", StringComparison.Ordinal))
                return Isa.Arm64;
            if (t.StartsWith("ARMV7", StringComparison.Ordinal) || t.StartsWith("THUMB", StringComparison.Ordinal))
                return Isa.Arm32;
            if (t.StartsWith("WASM", StringComparison.Ordinal))
                return Isa.Wasm;
            return Isa.X86;
        }

        public static AsmFile Parse(string text, Isa isa)
        {
            var file = new AsmFile { Isa = isa, Lines = text.Replace("\r", "").Split('\n') };
            var exported = new HashSet<string>(StringComparer.Ordinal);
            Function current = null;
            int srcFile = 0, srcLine = 0;
            var inEpilogue = false;
            for (var i = 0; i < file.Lines.Length; i++)
            {
                var raw = file.Lines[i];
                var line = StripComment(raw, isa);
                if (line.Trim().Length == 0)
                    continue;
                if (raw.Contains("/EXPORT:", StringComparison.Ordinal))
                {
                    foreach (Match m in s_Export.Matches(raw))
                        exported.Add(m.Groups[1].Value);
                    continue;
                }
                var trimmed = line.Trim();
                if (!char.IsWhiteSpace(line[0]) && trimmed.EndsWith(":", StringComparison.Ordinal) && !trimmed.StartsWith(".L", StringComparison.Ordinal))
                {
                    var name = Unquote(trimmed.Substring(0, trimmed.Length - 1));
                    if (name.StartsWith("__real@", StringComparison.Ordinal) || name.StartsWith("__xmm@", StringComparison.Ordinal) ||
                        name.StartsWith("__ymm@", StringComparison.Ordinal) || name.StartsWith("__zmm@", StringComparison.Ordinal) ||
                        name.StartsWith(".", StringComparison.Ordinal) || name.StartsWith("@", StringComparison.Ordinal))
                    {
                        Close(file, current, i);
                        current = null;
                        continue;
                    }
                    Close(file, current, i);
                    current = new Function { Name = name, StartLine = i + 1 };
                    srcFile = 0;
                    srcLine = 0;
                    inEpilogue = false;
                    continue;
                }
                if (trimmed.StartsWith(".", StringComparison.Ordinal) && !trimmed.EndsWith(":", StringComparison.Ordinal))
                {
                    Match m;
                    if ((m = s_CvFile.Match(line)).Success || (m = s_DwarfFile.Match(line)).Success)
                    {
                        var path = m.Groups.Count > 3 && m.Groups[3].Success && m.Groups[3].Value.Length > 0
                            ? m.Groups[2].Value + "/" + m.Groups[3].Value
                            : m.Groups[2].Value;
                        file.Files[int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)] = path.Replace("\\\\", "\\");
                    }
                    else if ((m = s_CvLoc.Match(line)).Success || (m = s_DwarfLoc.Match(line)).Success)
                    {
                        srcFile = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        srcLine = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                    }
                    else if (trimmed.StartsWith(".seh_endprologue", StringComparison.Ordinal) && current != null)
                        current.PrologueEnd = current.Instrs.Count;
                    else if (trimmed.StartsWith(".seh_startepilogue", StringComparison.Ordinal))
                    {
                        inEpilogue = true;
                        if (current != null)
                            current.SehEpilogue = true;
                    }
                    else if (trimmed.StartsWith(".seh_endepilogue", StringComparison.Ordinal))
                        inEpilogue = false;
                    else if (trimmed.StartsWith(".section", StringComparison.Ordinal) && current != null && current.Instrs.Count > 0 &&
                             !trimmed.Contains(".text", StringComparison.Ordinal))
                    {
                        Close(file, current, i);
                        current = null;
                    }
                    continue;
                }
                if (trimmed.EndsWith(":", StringComparison.Ordinal))
                {
                    // A local label inside a function: kept as an instruction-less marker for block splitting.
                    current?.Instrs.Add(new Instr { Line = i + 1, Mnemonic = ":", Operands = Array.Empty<string>(), Text = trimmed.Substring(0, trimmed.Length - 1) });
                    continue;
                }
                if (current == null || trimmed.Contains(" = ", StringComparison.Ordinal) && !char.IsWhiteSpace(line[0]))
                    continue;
                var instr = ParseInstr(trimmed, i + 1);
                instr.SrcFile = srcFile;
                instr.SrcLine = srcLine;
                instr.Epilogue = inEpilogue;
                current.Instrs.Add(instr);
            }
            Close(file, current, file.Lines.Length);
            foreach (var f in file.Functions)
            {
                f.Exported = exported.Contains(f.Name);
                f.Runtime = f.Name.StartsWith("burst.initialize", StringComparison.Ordinal);
                f.Display = Regex.IsMatch(f.Name, "^[0-9a-f]{32}$") ? "entry " + f.Name.Substring(0, 8) : Names.Pretty(f.Name);
            }
            return file;
        }

        static void Close(AsmFile file, Function f, int endIndex)
        {
            if (f == null)
                return;
            f.EndLine = endIndex;
            // Labels only: drop the trailing ones so the function ends at its last instruction.
            while (f.Instrs.Count > 0 && f.Instrs[f.Instrs.Count - 1].Mnemonic == ":")
                f.Instrs.RemoveAt(f.Instrs.Count - 1);
            if (f.Instrs.All(x => x.Mnemonic == ":"))
                return;
            if (f.Instrs.Count > 0)
                f.EndLine = f.Instrs[f.Instrs.Count - 1].Line;
            MarkFrame(f, file.Isa);
            file.Functions.Add(f);
        }

        // Prologue and epilogue: callee-saved register saves and restores, which aren't spills. SEH directives mark them
        // on Windows x64; elsewhere it is the run of saves at entry and of restores before each return.
        static void MarkFrame(Function f, Isa isa)
        {
            var list = f.Instrs;
            if (f.PrologueEnd >= 0)
            {
                for (var i = 0; i < f.PrologueEnd && i < list.Count; i++)
                    list[i].Prologue = true;
            }
            else
            {
                for (var i = 0; i < list.Count && IsFrameOp(list[i], isa, true); i++)
                    list[i].Prologue = true;
            }
            if (f.SehEpilogue)
                return;
            for (var r = 0; r < list.Count; r++)
            {
                if (list[r].Mnemonic != "ret")
                    continue;
                for (var i = r - 1; i >= 0 && IsFrameOp(list[i], isa, false); i--)
                    list[i].Epilogue = true;
            }
        }

        static bool IsFrameOp(Instr x, Isa isa, bool save)
        {
            var m = x.Mnemonic;
            var ops = x.Operands;
            if (m == ":")
                return false;
            if (isa == Isa.X86)
            {
                if (save ? m == "push" : m == "pop" || m == "vzeroupper")
                    return true;
                if ((m == "sub" || m == "add") && ops.Length == 2 && ops[0] == "rsp")
                    return true;
                if ((m == "mov" || m == "lea") && ops.Length == 2 && ops[0] == "rbp" && ops[1].Contains("rsp", StringComparison.Ordinal))
                    return true;
                if ((m == "mov" || m == "lea") && ops.Length == 2 && ops[0] == "rsp")
                    return true;
                if (m.StartsWith("vmovap", StringComparison.Ordinal) || m.StartsWith("movap", StringComparison.Ordinal) || m.StartsWith("vmovdqa", StringComparison.Ordinal) || m.StartsWith("movdqa", StringComparison.Ordinal))
                {
                    var mem = save ? ops.Length > 0 ? ops[0] : "" : ops.Length > 1 ? ops[1] : "";
                    return mem.Contains("[rsp", StringComparison.Ordinal) || mem.Contains("[rbp", StringComparison.Ordinal);
                }
                return false;
            }
            if (isa == Isa.Arm64)
            {
                var stackish = ops.Any(o => o.Contains("[sp", StringComparison.Ordinal) || o == "sp" || o.Contains("x29", StringComparison.Ordinal));
                if (save)
                    return (m == "stp" || m == "str" || m == "sub" || m == "mov") && stackish;
                return (m == "ldp" || m == "ldr" || m == "add" || m == "mov") && stackish;
            }
            if (isa == Isa.Arm32)
                return save ? m == "push" || m == "vpush" : m == "pop" || m == "vpop";
            return false;
        }

        public static Instr ParseInstr(string text, int line)
        {
            var cut = 0;
            while (cut < text.Length && !char.IsWhiteSpace(text[cut]))
                cut++;
            var mnemonic = text.Substring(0, cut).ToLowerInvariant();
            var rest = cut < text.Length ? text.Substring(cut).Trim() : "";
            return new Instr { Line = line, Mnemonic = mnemonic, Operands = SplitOperands(rest), Text = text };
        }

        public static string[] SplitOperands(string s)
        {
            if (s.Length == 0)
                return Array.Empty<string>();
            var list = new List<string>();
            var depth = 0;
            var quoted = false;
            var start = 0;
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '"')
                    quoted = !quoted;
                else if (quoted)
                    continue;
                else if (c == '[' || c == '{' || c == '(')
                    depth++;
                else if (c == ']' || c == '}' || c == ')')
                    depth--;
                else if (c == ',' && depth == 0)
                {
                    list.Add(s.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }
            list.Add(s.Substring(start).Trim());
            return list.ToArray();
        }

        static string StripComment(string line, Isa isa)
        {
            var quoted = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                    quoted = !quoted;
                else if (!quoted)
                {
                    if ((isa == Isa.X86 || isa == Isa.Wasm) && c == '#')
                        return line.Substring(0, i);
                    if ((isa == Isa.Arm64 || isa == Isa.Arm32) && c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                        return line.Substring(0, i);
                    if (isa == Isa.Arm32 && c == '@')
                        return line.Substring(0, i);
                }
            }
            return line;
        }

        public static string Unquote(string s) => s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"' ? s.Substring(1, s.Length - 2) : s;
    }
}
