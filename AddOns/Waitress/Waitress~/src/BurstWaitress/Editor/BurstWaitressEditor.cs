// The Editor half of burstwaitress. The CLI embeds this file, writes it to UserSettings/Waitress/BurstWaitress/, and
// runs it through `unity command run_script`, so nothing is imported into the project.
//
// It only ever READS BurstCompiler.Options. Assigning any of them, even to the value it already holds, invalidates
// every Burst-compiled method and starts a project-wide recompile; EnableBurstCompileSynchronously can wedge the
// Editor. Per-call compile settings go into the GetDisassembly option string and nowhere else.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEditor;

public static class BurstWaitressEditor
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    // ---------------------------------------------------------------------------------------------------------------
    // Project state

    public static string State()
    {
        var sb = new StringBuilder("{\"ok\":true");
        AppendState(sb);
        return sb.Append('}').ToString();
    }

    static void AppendState(StringBuilder sb)
    {
        var options = BurstCompiler.Options;
        var forced = (bool)(typeof(BurstCompilerOptions).GetField("ForceDisableBurstCompilation", Any)?.GetValue(null) ?? false);
        sb.Append(",\"enabled\":").Append(B(BurstCompiler.IsEnabled));
        sb.Append(",\"menuEnabled\":").Append(B(options.EnableBurstCompilation));
        sb.Append(",\"forceDisabled\":").Append(B(forced));
        sb.Append(",\"safetyChecks\":").Append(B(options.EnableBurstSafetyChecks));
        sb.Append(",\"forceSafetyChecks\":").Append(B(options.ForceEnableBurstSafetyChecks));
        sb.Append(",\"synchronous\":").Append(B(options.EnableBurstCompileSynchronously));
        sb.Append(",\"nativeDebug\":").Append(B(options.EnableBurstDebug));
        sb.Append(",\"eager\":").Append(Q(EagerStatus()));
        sb.Append(",\"jitIdle\":").Append(B(JitIdle()));
        sb.Append(",\"progress\":").Append(Q(BurstProgress()));
        sb.Append(",\"host\":").Append(Q(Host()));
        sb.Append(",\"compiling\":").Append(B(EditorApplication.isCompiling));
        sb.Append(",\"playing\":").Append(B(EditorApplication.isPlaying));
        sb.Append(",\"burstVersion\":").Append(Q(BurstPackageVersion()));
        sb.Append(",\"disabledAssemblies\":[").Append(string.Join(",", DisabledAssemblies().Select(Q))).Append(']');
    }

    static string EagerStatus() =>
        Find("Unity.Burst.Editor.BurstLoader")?.GetField("EagerCompilationStatus", Any)?.GetValue(null)?.ToString() ?? "unknown";

    static bool JitIdle()
    {
        var m = typeof(BurstCompiler).GetMethod("IsCurrentCompilationDone", Any);
        return m == null || (bool)m.Invoke(null, null);
    }

    static string BurstProgress()
    {
        foreach (var item in Progress.EnumerateItems())
            if (item.name == "Burst")
                return item.description ?? "";
        return "";
    }

    static string Host()
    {
        try
        {
            return (string)typeof(BurstCompiler).GetMethod("GetTargetCpuFromHost", Any)?.Invoke(null, null) ?? "";
        }
        catch
        {
            return "";
        }
    }

    static string BurstPackageVersion()
    {
        var info = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(BurstCompiler).Assembly);
        return info?.version ?? "";
    }

    static IEnumerable<string> DisabledAssemblies()
    {
        const string path = "ProjectSettings/Burst_DisableAssembliesForEditorCompilation.json";
        if (!File.Exists(path))
            yield break;
        var text = File.ReadAllText(path);
        var at = text.IndexOf("DisabledAssemblies", StringComparison.Ordinal);
        if (at < 0)
            yield break;
        var open = text.IndexOf('[', at);
        var close = text.IndexOf(']', open + 1);
        if (open < 0 || close < 0)
            yield break;
        foreach (var part in text.Substring(open + 1, close - open - 1).Split(','))
        {
            var name = part.Trim().Trim('"');
            if (name.Length > 0)
                yield return name;
        }
    }

    // ---------------------------------------------------------------------------------------------------------------
    // The target index: every Burst entry point the Burst Inspector lists, under the names the CLI resolves.

    sealed class Target
    {
        public string Name;
        public string Kind;
        public string Interface;
        public MethodInfo Method;
        public Type JobType;
        public bool IsStatic;
        public bool HasBurst;
        public object Raw;
    }

    static List<Target> s_Targets;

    static List<Target> Targets()
    {
        if (s_Targets != null)
            return s_Targets;
        var reflection = Find("Unity.Burst.Editor.BurstReflection");
        reflection.GetMethod("EnsureInitialized", Any).Invoke(null, null);
        var assemblies = ((IList)reflection.GetField("EditorAssembliesThatCanPossiblyContainJobs", Any).GetValue(null)).Cast<Assembly>().ToList();
        var optionType = Find("Unity.Burst.Editor.BurstReflectionAssemblyOptions");
        var result = reflection.GetMethod("FindExecuteMethods", Any).Invoke(null, new object[] { assemblies, Enum.ToObject(optionType, 0) });
        var raw = (IList)result.GetType().GetField("CompileTargets").GetValue(result);
        var list = new List<Target>();
        foreach (var r in raw)
        {
            var type = r.GetType();
            var t = new Target
            {
                Method = (MethodInfo)type.GetField("Method").GetValue(r),
                JobType = (Type)type.GetField("JobType").GetValue(r),
                IsStatic = (bool)type.GetField("IsStaticMethod").GetValue(r),
                HasBurst = (bool)type.GetProperty("HasRequiredBurstCompileAttributes").GetValue(r),
                Raw = r,
            };
            var iface = (Type)type.GetField("JobInterfaceType").GetValue(r);
            t.Interface = iface == null ? "" : Pretty(iface.IsGenericType ? iface.GetGenericTypeDefinition() : iface, false);
            t.Kind = t.IsStatic ? "static" : "job";
            t.Name = t.IsStatic ? PrettyMethod(t.Method) : Pretty(t.JobType, true);
            list.Add(t);
        }
        // A job type implementing two job interfaces gets one entry per interface.
        foreach (var group in list.Where(t => !t.IsStatic).GroupBy(t => t.Name).Where(g => g.Count() > 1))
            foreach (var t in group)
                t.Name += " (" + t.Interface + ")";
        s_Targets = list;
        return list;
    }

    // Writes the index as TSV: name, kind, interface, burst, assembly, float mode, float precision, optimize for,
    // disable safety checks, compile synchronously, source file.
    public static string Index(string path)
    {
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling scripts; retry when it finishes");
        var sw = Stopwatch.StartNew();
        var targets = Targets();
        var sb = new StringBuilder();
        foreach (var t in targets)
        {
            var attr = Attribute(t);
            sb.Append(t.Name).Append('\t').Append(t.Kind).Append('\t').Append(t.Interface).Append('\t').Append(t.HasBurst ? "1" : "0").Append('\t')
              .Append((t.IsStatic ? t.Method.DeclaringType : t.JobType).Assembly.GetName().Name).Append('\t')
              .Append(attr?.FloatMode.ToString() ?? "").Append('\t').Append(attr?.FloatPrecision.ToString() ?? "").Append('\t')
              .Append(attr?.OptimizeFor.ToString() ?? "").Append('\t').Append(attr != null && attr.DisableSafetyChecks ? "1" : "0").Append('\t')
              .Append(attr != null && attr.CompileSynchronously ? "1" : "0").Append('\n');
        }
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
        WriteAtomic(path, sb.ToString());
        return "{\"ok\":true,\"count\":" + targets.Count + ",\"ms\":" + sw.ElapsedMilliseconds + "}";
    }

    static BurstCompileAttribute Attribute(Target t)
    {
        var member = t.IsStatic ? (MemberInfo)t.Method : t.JobType;
        var attr = member.GetCustomAttribute<BurstCompileAttribute>();
        if (attr == null)
            return null;
        var asm = member.Module.Assembly.GetCustomAttribute<BurstCompileAttribute>();
        if (asm != null)
        {
            var merge = typeof(BurstCompilerOptions).GetMethod("MergeAttributes", Any);
            var args = new object[] { attr, asm };
            merge.Invoke(null, args);
            attr = (BurstCompileAttribute)args[0];
        }
        return attr;
    }

    static Target Resolve(string name) => Targets().FirstOrDefault(t => t.Name == name);

    // A short name the CLI couldn't resolve (its index predates the last compile): unique dotted suffix.
    static Target ResolveLoose(string name)
    {
        var exact = Resolve(name);
        if (exact != null)
            return exact;
        var matches = Targets().Where(t => t.Name.EndsWith("." + name, StringComparison.Ordinal) || Bare(t.Name).EndsWith("." + name, StringComparison.Ordinal)).ToList();
        return matches.Count == 1 ? matches[0] : null;
    }

    static string Bare(string name)
    {
        var cut = name.IndexOf('(');
        return cut > 0 ? name.Substring(0, cut) : name;
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Dumps. The request file holds one dump per line: key, target name, then option overrides as name=value pairs
    // (target, float-mode, float-precision, safety, debug). Each dump is written as <key>.asm plus <key>.meta, the
    // meta last, so a meta on disk means its dump is complete.

    sealed class Request
    {
        public string Key;
        public string Name;
        public Dictionary<string, string> Options = new Dictionary<string, string>();
    }

    public static string Dump(string requestFile, string outDir, int budgetMs)
    {
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling scripts; retry when it finishes");
        var requests = new List<Request>();
        foreach (var line in File.ReadAllLines(requestFile))
        {
            if (line.Length == 0)
                continue;
            var parts = line.Split('\t');
            var r = new Request { Key = parts[0], Name = parts[1] };
            for (var p = 2; p < parts.Length; p++)
            {
                var eq = parts[p].IndexOf('=');
                if (eq > 0)
                    r.Options[parts[p].Substring(0, eq)] = parts[p].Substring(eq + 1);
            }
            requests.Add(r);
        }
        Directory.CreateDirectory(outDir);
        var work = new List<(Request request, MethodInfo method, string options, string error)>();
        foreach (var r in requests)
        {
            var t = Resolve(r.Name);
            if (t == null)
            {
                WriteMeta(outDir, r, null, 0, "no Burst entry point named '" + r.Name + "' in this Editor (the index is out of date; re-run)");
                continue;
            }
            string options;
            try
            {
                options = BuildOptions(t, r.Options);
            }
            catch (Exception e)
            {
                WriteMeta(outDir, r, null, 0, e.Message);
                continue;
            }
            work.Add((r, t.Method, options, null));
        }
        var done = new ManualResetEventSlim(false);
        var queued = !Monitor.TryEnter(CompileLock());
        if (!queued)
            Monitor.Exit(CompileLock());
        Task.Run(() =>
        {
            lock (CompileLock())
            {
                foreach (var w in work)
                {
                    // The tool touches its request file while it waits and deletes it when it exits: an abandoned
                    // request stops here instead of holding the queue.
                    if (!File.Exists(requestFile) || (DateTime.UtcNow - File.GetLastWriteTimeUtc(requestFile)).TotalSeconds > 12)
                        break;
                    if (File.Exists(Path.Combine(outDir, w.request.Key + ".meta")))
                        continue;
                    var sw = Stopwatch.StartNew();
                    string text;
                    string error = null;
                    try
                    {
                        text = Disassemble(w.method, w.options);
                    }
                    catch (Exception e)
                    {
                        text = "";
                        error = (e.InnerException ?? e).Message;
                    }
                    if (error == null)
                        error = CompileError(text);
                    if (error == null && text.Trim().Length == 0)
                        error = "Burst returned an empty dump (an option it doesn't accept?)";
                    WriteAtomic(Path.Combine(outDir, w.request.Key + ".asm"), text);
                    WriteMeta(outDir, w.request, w.options, sw.ElapsedMilliseconds, error);
                }
            }
            done.Set();
        });
        // Behind another request: return at once, so the tool can say it is waiting.
        var finished = done.Wait(queued ? Math.Min(1000, budgetMs) : Math.Max(0, budgetMs));
        return "{\"ok\":true,\"requested\":" + requests.Count + ",\"finished\":" + B(finished) + ",\"queued\":" + B(queued && !finished) + "}";
    }

    static void WriteMeta(string dir, Request r, string options, long ms, string error)
    {
        var sb = new StringBuilder();
        sb.Append("name\t").Append(r.Name).Append('\n');
        sb.Append("ms\t").Append(ms.ToString(Inv)).Append('\n');
        if (options != null)
            sb.Append("options\t").Append(options.Replace("\n", "|")).Append('\n');
        if (error != null)
            sb.Append("error\t").Append(error.Replace("\r", "").Replace("\n", "\\n")).Append('\n');
        WriteAtomic(Path.Combine(dir, r.Key + ".meta"), sb.ToString());
    }

    // The attribute's options (float mode, precision, optimize-for, user options) with the overrides replacing their
    // lines: Burst returns an empty dump when an option appears twice.
    static string BuildOptions(Target t, Dictionary<string, string> o)
    {
        var clone = typeof(BurstCompilerOptions).GetMethod("Clone", Any).Invoke(BurstCompiler.Options, null);
        var args = new object[] { t.IsStatic ? (MemberInfo)t.Method : t.JobType, null, false, true, false };
        if (!(bool)typeof(BurstCompilerOptions).GetMethod("TryGetOptions", Any).Invoke(clone, args))
            throw new InvalidOperationException((t.IsStatic ? "" : t.JobType.Name + " has no [BurstCompile]; ") + "Burst would not compile " + t.Name);
        var lines = ((string)args[1]).Split('\n').Where(l => l.Length > 0).ToList();
        void Set(string prefix, string value)
        {
            lines.RemoveAll(l => l.StartsWith(prefix, StringComparison.Ordinal));
            if (value != null)
                lines.Add(prefix + value);
        }
        lines.RemoveAll(l => l.StartsWith("--temp-folder=", StringComparison.Ordinal) || l.StartsWith("--log-timings", StringComparison.Ordinal));
        // Without a --target line GetDisassembly returns an empty dump; Auto is the host CPU.
        Set("--target=", o.TryGetValue("target", out var target) ? target : "Auto");
        if (o.TryGetValue("float-mode", out var fm))
            Set("--float-mode=", fm == "Default" ? null : fm);
        if (o.TryGetValue("float-precision", out var fp))
            Set("--float-precision=", fp == "Standard" ? null : fp);
        if (o.TryGetValue("safety", out var safety))
        {
            if (safety == "off")
                Set("--disable-safety-checks", null);
            Set("--global-safety-checks-setting=", safety == "on" ? "On" : safety == "force" ? "ForceOn" : "Off");
        }
        Set("--debug=", o.TryGetValue("debug", out var debug) ? debug : "0");
        Set("--disable-warnings=", "BC1370;BC1322");
        Set("--dump=", "Asm");
        return string.Join("\n", lines);
    }

    static MethodInfo s_Disassemble;

    static string Disassemble(MethodInfo method, string options)
    {
        s_Disassemble ??= Find("Unity.Burst.LowLevel.BurstCompilerService").GetMethod("GetDisassembly", Any);
        return (string)s_Disassemble.Invoke(null, new object[] { method, options }) ?? "";
    }

    static string CompileError(string text)
    {
        var lines = text.Replace("\r", "").Split('\n');
        var errors = lines.Where(l => l.Contains("error BC", StringComparison.Ordinal) || l.StartsWith("Failed to compile", StringComparison.Ordinal)).Take(6).ToList();
        if (errors.Count == 0)
            return null;
        var at = lines.FirstOrDefault(l => l.TrimStart().StartsWith("at ", StringComparison.Ordinal));
        return string.Join("\n", errors.Select(e => e.Trim())) + (at != null ? "\n" + at.Trim() : "");
    }

    // One compile at a time across script runs: each run_script is its own assembly, so the lock lives in the domain.
    static object CompileLock()
    {
        const string key = "BurstWaitress.CompileLock";
        lock (typeof(AppDomain))
        {
            var o = AppDomain.CurrentDomain.GetData(key);
            if (o == null)
            {
                o = new object();
                AppDomain.CurrentDomain.SetData(key, o);
            }
            return o;
        }
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Status: whether each target compiles with the options the Editor itself uses, and whether the Editor is
    // running Burst code for it right now.

    public static string Status(string namesFile, string outDir, int budgetMs, bool quick)
    {
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling scripts; retry when it finishes");
        var sb = new StringBuilder("{\"ok\":true");
        AppendState(sb);
        sb.Append(",\"items\":[");
        var first = true;
        var sw = Stopwatch.StartNew();
        var disabled = new HashSet<string>(DisabledAssemblies());
        Directory.CreateDirectory(outDir);
        foreach (var name in File.ReadAllLines(namesFile).Where(l => l.Length > 0))
        {
            if (!first)
                sb.Append(',');
            first = false;
            sb.Append("{\"name\":").Append(Q(name));
            var t = ResolveLoose(name);
            if (t == null)
            {
                sb.Append(",\"state\":\"not eligible\",\"reason\":").Append(Q(Explain(name))).Append('}');
                continue;
            }
            if (t.Name != name)
                sb.Append(",\"resolved\":").Append(Q(t.Name));
            var assembly = (t.IsStatic ? t.Method.DeclaringType : t.JobType).Assembly.GetName().Name;
            if (!t.HasBurst)
            {
                var reason = t.IsStatic ? "the method or its class lacks [BurstCompile]" : $"{t.JobType.Name} has no [BurstCompile]: the job runs managed";
                sb.Append(",\"state\":\"not eligible\",\"reason\":").Append(Q(reason)).Append('}');
                continue;
            }
            if (disabled.Contains(assembly))
            {
                sb.Append(",\"state\":\"burst disabled\",\"reason\":").Append(Q($"assembly {assembly} is listed in ProjectSettings/Burst_DisableAssembliesForEditorCompilation.json")).Append('}');
                continue;
            }
            if (sw.ElapsedMilliseconds > budgetMs)
            {
                sb.Append(",\"state\":\"unchecked\",\"reason\":\"time limit; check fewer at once\"}");
                continue;
            }
            // Compile with the Editor's own settings for the host CPU, as the Editor JIT would.
            string text, error;
            long ms;
            var options = BuildOptions(t, new Dictionary<string, string>());
            var c = Stopwatch.StartNew();
            lock (CompileLock())
            {
                try
                {
                    text = Disassemble(t.Method, options);
                    error = CompileError(text) ?? (text.Trim().Length == 0 ? "Burst returned an empty dump for it" : null);
                }
                catch (Exception e)
                {
                    text = "";
                    error = (e.InnerException ?? e).Message;
                }
            }
            ms = c.ElapsedMilliseconds;
            var key = "status-" + Hash(name + "\n" + options);
            WriteAtomic(Path.Combine(outDir, key + ".asm"), text);
            sb.Append(",\"key\":").Append(Q(key)).Append(",\"ms\":").Append(ms);
            if (error != null)
            {
                sb.Append(",\"state\":\"failed\",\"reason\":").Append(Q(error)).Append('}');
                continue;
            }
            var check = quick ? (new List<(string, string)>(), null, null) : AssemblyCheck(assembly, outDir);
            if (check.failures != null && check.failures.Count > 0)
            {
                var plural = check.failures.Count == 1;
                var reason = new StringBuilder($"Burst compiles each assembly as one library, and {check.failures.Count} other entry point{(plural ? "" : "s")} in {assembly} fail{(plural ? "s" : "")} to compile, so none of the Burst code in {assembly} runs in the Editor:");
                foreach (var (n, e) in check.failures.Take(3))
                    reason.Append('\n').Append(n).Append(": ").Append(e.Split('\n')[0]);
                sb.Append(",\"state\":\"failed\",\"assemblyFailure\":true,\"reason\":").Append(Q(reason.ToString())).Append('}');
                continue;
            }
            if (check.failures == null)
                sb.Append(",\"assemblyCheck\":").Append(Q(check.progress)).Append(",\"assemblyCheckFile\":").Append(Q(check.done));
            else if (!quick)
                sb.Append(",\"assemblyOk\":").Append(Q($"all {Targets().Count(x => x.HasBurst && (x.IsStatic ? x.Method.DeclaringType : x.JobType).Assembly.GetName().Name == assembly)} Burst entry points of {assembly} compile"));
            if (!BurstCompiler.IsEnabled)
            {
                sb.Append(",\"state\":\"burst disabled\",\"reason\":\"Burst compilation is off in this Editor (Jobs > Burst > Enable Compilation): it compiles, but runs managed\"}");
                continue;
            }
            if (t.IsStatic)
            {
                var pointer = DirectCallPointer(t.Method);
                sb.Append(",\"directCall\":").Append(Q(pointer));
            }
            // EagerCompilationStatus stays NotScheduled after a reload that had nothing to compile, so the JIT's own
            // pending flag and the Burst progress item decide.
            var progress = BurstProgress();
            if (!JitIdle() || progress.Length > 0)
            {
                sb.Append(",\"state\":\"pending\",\"reason\":").Append(Q("Burst is still compiling in the background" + (progress.Length > 0 ? " (" + progress + ")" : "") + "; until it finishes this runs managed")).Append('}');
                continue;
            }
            sb.Append(",\"state\":\"compiled\"}");
        }
        sb.Append("]}");
        return sb.ToString();
    }

    // Every Burst entry point of one assembly compiled with the Editor's own options, on a background thread, once per
    // build of that assembly. Results go to AssemblyChecks/<assembly>_<build>_<options>.tsv, then a .done file.
    static (List<(string name, string error)> failures, string progress, string done) AssemblyCheck(string assembly, string dir)
    {
        var path = Path.Combine("Library", "ScriptAssemblies", assembly + ".dll");
        var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assembly);
        if (!File.Exists(path))
            path = loaded?.Location ?? "";
        var build = File.Exists(path) ? File.GetLastWriteTimeUtc(path).Ticks.ToString(Inv) : "0";
        var entries = Targets().Where(t => t.HasBurst && (t.IsStatic ? t.Method.DeclaringType : t.JobType).Assembly.GetName().Name == assembly).ToList();
        var optionsKey = Hash(BurstCompiler.Options.EnableBurstSafetyChecks + "|" + Hash(File.Exists(SelfPath()) ? File.ReadAllText(SelfPath()) : ""));
        var checkDir = Path.Combine(dir, "AssemblyChecks");
        Directory.CreateDirectory(checkDir);
        var file = Path.Combine(checkDir, assembly + "_" + build + "_" + optionsKey + ".tsv");
        var done = file + ".done";
        if (File.Exists(done))
        {
            var failures = File.ReadAllLines(file).Select(l => l.Split('\t')).Where(p => p.Length >= 2 && p[1] != "ok").Select(p => (p[0], p[1].Replace("\\n", "\n"))).ToList();
            return (failures, null, done);
        }
        var key = "BurstWaitress.Check." + file;
        lock (typeof(AppDomain))
        {
            if (AppDomain.CurrentDomain.GetData(key) == null)
            {
                AppDomain.CurrentDomain.SetData(key, "running");
                var work = entries.Select(t => (t.Name, t.Method, BuildOptions(t, new Dictionary<string, string>()))).ToList();
                File.WriteAllText(file, "");
                Task.Run(() =>
                {
                    try
                    {
                        foreach (var (name, method, options) in work)
                        {
                            // Unoptimized compiles are about twice as fast and catch the front-end errors; a failure is
                            // confirmed with the real options, since optimization can remove a managed allocation.
                            var quickOptions = string.Join("\n", options.Split('\n').Where(l => !l.StartsWith("--opt-level=", StringComparison.Ordinal) && l != "--opt-for-size")) + "\n--disable-opt";
                            var error = TryCompile(method, quickOptions);
                            if (error != null)
                                error = TryCompile(method, options);
                            File.AppendAllText(file, name + "\t" + (error == null ? "ok" : error.Replace("\r", "").Replace("\n", "\\n")) + "\n");
                        }
                        File.WriteAllText(done, work.Count.ToString(Inv));
                    }
                    finally
                    {
                        AppDomain.CurrentDomain.SetData(key, null);
                    }
                });
            }
        }
        var have = File.Exists(file) ? File.ReadAllLines(file).Length : 0;
        return (null, $"checking the {entries.Count} Burst entry points of {assembly} ({have} done)", Path.GetFullPath(done));
    }

    static string TryCompile(MethodInfo method, string options)
    {
        lock (CompileLock())
        {
            try
            {
                var text = Disassemble(method, options);
                return CompileError(text) ?? (text.Trim().Length == 0 ? "Burst returned an empty dump" : null);
            }
            catch (Exception e)
            {
                return (e.InnerException ?? e).Message;
            }
        }
    }

    static string SelfPath() => Path.Combine("UserSettings", "Waitress", "BurstWaitress", "BurstWaitressEditor.cs");

    // The ILPP-generated direct-call class caches the function pointer after the first call in this domain.
    static string DirectCallPointer(MethodInfo method)
    {
        var holder = method.DeclaringType?.GetNestedTypes(Any).FirstOrDefault(n => n.Name.StartsWith(method.Name + "_", StringComparison.Ordinal) &&
                                                                                       n.Name.EndsWith("$BurstDirectCall", StringComparison.Ordinal) &&
                                                                                       n.GetMethod("Invoke", Any)?.GetParameters().Length == method.GetParameters().Length);
        var field = holder?.GetField("Pointer", Any);
        if (field == null)
            return "none";
        return (IntPtr)field.GetValue(null) == IntPtr.Zero ? "unrequested" : "set";
    }

    // Why a name the index doesn't list has no Burst entry point.
    static string Explain(string name)
    {
        var bare = name;
        var paren = bare.IndexOf('(');
        if (paren > 0)
            bare = bare.Substring(0, paren);
        var lt = bare.IndexOf('<');
        if (lt > 0)
            bare = bare.Substring(0, lt);
        var dot = bare.LastIndexOf('.');
        var typeName = bare;
        var methodName = dot > 0 ? bare.Substring(dot + 1) : null;
        var typePart = dot > 0 ? bare.Substring(0, dot) : bare;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(x => x != null).ToArray();
            }
            foreach (var type in types)
            {
                var full = (type.FullName ?? type.Name).Replace('+', '.');
                var tick = full.IndexOf('`');
                if (tick > 0)
                    full = full.Substring(0, tick);
                if (full == typeName || full.EndsWith("." + typeName, StringComparison.Ordinal))
                {
                    if (type.IsGenericTypeDefinition)
                        return $"{full} is generic: Burst compiles only the closed instances the project uses, and the index has none of this one";
                    if (type.IsValueType)
                        return $"{full} implements no job interface Burst knows";
                    return $"{full} has no Burst entry point (no [BurstCompile] static methods)";
                }
                if (methodName != null && (full == typePart || full.EndsWith("." + typePart, StringComparison.Ordinal)))
                {
                    var m = type.GetMethods(Any).FirstOrDefault(x => x.Name == methodName);
                    if (m == null)
                        return $"{full} has no method {methodName}";
                    var onMethod = m.GetCustomAttribute<BurstCompileAttribute>() != null;
                    var onType = type.GetCustomAttribute<BurstCompileAttribute>() != null;
                    if (!m.IsStatic)
                        return $"{full}.{methodName} is an instance method; Burst compiles static methods and jobs";
                    if (m.ContainsGenericParameters)
                        return $"{full}.{methodName} is generic; Burst can't compile it as an entry point";
                    if (!onMethod && !onType)
                        return $"neither {full}.{methodName} nor {full} has [BurstCompile]";
                    if (!onMethod)
                        return $"{full}.{methodName} lacks [BurstCompile] (its class has it)";
                    if (!onType)
                        return $"{full} lacks [BurstCompile], which a [BurstCompile] static method's class needs";
                    return $"{full}.{methodName} isn't listed by the Burst Inspector";
                }
            }
        }
        return "no type or method named '" + name + "' in this Editor";
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Targets the Burst Inspector offers, each tried on one small entry point.

    public static string TargetList(string probeName)
    {
        var cpu = Find("Unity.Burst.BurstTargetCpu");
        var names = Enum.GetNames(cpu);
        var t = probeName != null ? Resolve(probeName) : null;
        t ??= Targets().FirstOrDefault(x => x.IsStatic && x.HasBurst && x.Method.GetParameters().Length == 0) ?? Targets().FirstOrDefault(x => x.IsStatic && x.HasBurst);
        if (t == null)
            return Error("no static Burst entry point to try targets on");
        var sb = new StringBuilder("{\"ok\":true,\"probe\":").Append(Q(t.Name)).Append(",\"host\":").Append(Q(Host())).Append(",\"targets\":[");
        var first = true;
        foreach (var name in names.Concat(new[] { "AVX512" }))
        {
            string error;
            try
            {
                string text;
                lock (CompileLock())
                    text = Disassemble(t.Method, BuildOptions(t, new Dictionary<string, string> { ["target"] = name }));
                error = CompileError(text) ?? (text.Trim().Length == 0 ? "rejected (empty dump)" : null);
            }
            catch (Exception e)
            {
                error = (e.InnerException ?? e).Message;
            }
            if (!first)
                sb.Append(',');
            first = false;
            sb.Append("{\"name\":").Append(Q(name)).Append(",\"inEnum\":").Append(B(names.Contains(name))).Append(",\"ok\":").Append(B(error == null));
            if (error != null)
                sb.Append(",\"error\":").Append(Q(error));
            sb.Append('}');
        }
        return sb.Append("]}").ToString();
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Side-effect check for verification: options, compile state and dirty scenes, read only.

    public static string Snapshot()
    {
        var sb = new StringBuilder("{\"ok\":true");
        AppendState(sb);
        var dirty = 0;
        for (var i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i++)
            if (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i).isDirty)
                dirty++;
        sb.Append(",\"dirtyScenes\":").Append(dirty);
        sb.Append(",\"scriptAssemblies\":").Append(Q(Hash(string.Join("|", Directory.GetFiles("Library/ScriptAssemblies", "*.dll").OrderBy(f => f, StringComparer.Ordinal)
                                                                         .Select(f => f + ":" + File.GetLastWriteTimeUtc(f).Ticks)))));
        return sb.Append('}').ToString();
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Names

    static readonly Dictionary<Type, string> s_Keywords = new Dictionary<Type, string>
    {
        [typeof(bool)] = "bool", [typeof(byte)] = "byte", [typeof(sbyte)] = "sbyte", [typeof(short)] = "short", [typeof(ushort)] = "ushort",
        [typeof(int)] = "int", [typeof(uint)] = "uint", [typeof(long)] = "long", [typeof(ulong)] = "ulong", [typeof(float)] = "float",
        [typeof(double)] = "double", [typeof(char)] = "char", [typeof(void)] = "void", [typeof(IntPtr)] = "IntPtr", [typeof(UIntPtr)] = "UIntPtr",
    };

    static string Pretty(Type type, bool qualified)
    {
        if (type.IsByRef)
            return "ref " + Pretty(type.GetElementType(), qualified);
        if (type.IsPointer)
            return Pretty(type.GetElementType(), qualified) + "*";
        if (type.IsArray)
            return Pretty(type.GetElementType(), qualified) + "[]";
        if (s_Keywords.TryGetValue(type, out var kw))
            return kw;
        if (type.IsGenericParameter)
            return type.Name;
        var name = type.Name;
        var tick = name.IndexOf('`');
        if (tick > 0)
            name = name.Substring(0, tick);
        string prefix;
        if (type.IsNested)
            prefix = Pretty(type.DeclaringType.IsGenericTypeDefinition && type.IsConstructedGenericType
                                ? type.DeclaringType.MakeGenericType(type.GetGenericArguments().Take(type.DeclaringType.GetGenericArguments().Length).ToArray())
                                : type.DeclaringType, qualified) + ".";
        else
            prefix = qualified && !string.IsNullOrEmpty(type.Namespace) ? type.Namespace + "." : "";
        var args = type.IsGenericType ? type.GetGenericArguments() : Type.EmptyTypes;
        var inherited = type.IsNested && type.DeclaringType.IsGenericType ? type.DeclaringType.GetGenericArguments().Length : 0;
        var own = args.Skip(inherited).ToArray();
        var generic = own.Length > 0 ? "<" + string.Join(",", own.Select(a => Pretty(a, qualified))) + ">" : "";
        return prefix + name + generic;
    }

    static string PrettyMethod(MethodInfo m) =>
        Pretty(m.DeclaringType, true) + "." + m.Name + "(" + string.Join(", ", m.GetParameters().Select(p => Pretty(p.ParameterType, false))) + ")";

    // ---------------------------------------------------------------------------------------------------------------
    // Helpers

    static Type Find(string name)
    {
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = a.GetType(name);
            if (t != null)
                return t;
        }
        return null;
    }

    static void WriteAtomic(string path, string text)
    {
        var tmp = path + ".tmp" + Thread.CurrentThread.ManagedThreadId;
        File.WriteAllText(tmp, text, new UTF8Encoding(false));
        if (File.Exists(path))
            File.Delete(path);
        File.Move(tmp, path);
    }

    static string Hash(string s)
    {
        using var sha = System.Security.Cryptography.SHA1.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
        return string.Concat(bytes.Take(8).Select(b => b.ToString("x2")));
    }

    static string B(bool b) => b ? "true" : "false";

    static string Error(string message) => "{\"ok\":false,\"error\":" + Q(message) + "}";

    static string Q(string s)
    {
        if (s == null)
            return "null";
        var sb = new StringBuilder("\"");
        foreach (var c in s)
        {
            if (c == '"' || c == '\\')
                sb.Append('\\').Append(c);
            else if (c == '\n')
                sb.Append("\\n");
            else if (c == '\r')
                continue;
            else if (c == '\t')
                sb.Append("\\t");
            else if (c < ' ')
                sb.Append("\\u").Append(((int)c).ToString("x4"));
            else
                sb.Append(c);
        }
        return sb.Append('"').ToString();
    }
}
