using System;
using System.Collections.Generic;
using System.Linq;

namespace BurstWaitress.Model
{
    /// <summary>
    /// One set of per-dump compile settings. They go only into the option string of that one GetDisassembly call; the
    /// project's Burst settings are never touched. Null float mode or precision means the method's [BurstCompile].
    /// </summary>
    public sealed class CompileOptions
    {
        public string Target = "AVX2";
        public string FloatMode;
        public string FloatPrecision;
        public string Safety = "off";
        public bool Debug;

        public CompileOptions Clone() => (CompileOptions)MemberwiseClone();

        /// <summary>The request-file form the Editor script reads, which is also the cache key's option part.</summary>
        public string Request() =>
            "target=" + Target + (FloatMode != null ? "\tfloat-mode=" + FloatMode : "") + (FloatPrecision != null ? "\tfloat-precision=" + FloatPrecision : "") +
            "\tsafety=" + Safety + "\tdebug=" + (Debug ? "2" : "0");

        public string Describe(Entry e)
        {
            var fm = FloatMode ?? (string.IsNullOrEmpty(e?.FloatMode) ? "Default" : e.FloatMode) + (FloatMode == null && e != null && e.FloatMode != "Default" && e.FloatMode != "" ? " (attribute)" : "");
            var fp = FloatPrecision ?? (string.IsNullOrEmpty(e?.FloatPrecision) ? "Standard" : e.FloatPrecision);
            return $"{Target} · float {fm}" + (fp != "Standard" ? $" {fp}" : "") + $" · safety {Safety}";
        }

        public string EffectiveFloatMode(Entry e) => FloatMode ?? (string.IsNullOrEmpty(e?.FloatMode) ? "Default" : e.FloatMode);

        static readonly Dictionary<string, string> s_Targets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["avx2"] = "AVX2", ["avx"] = "AVX", ["sse4"] = "X64_SSE4", ["sse4.2"] = "X64_SSE4", ["sse42"] = "X64_SSE4", ["x64_sse4"] = "X64_SSE4",
            ["sse2"] = "X64_SSE2", ["x64_sse2"] = "X64_SSE2", ["x86_sse2"] = "X86_SSE2", ["x86_sse4"] = "X86_SSE4", ["auto"] = "Auto", ["host"] = "Auto",
            ["neon"] = "ARMV8A_AARCH64", ["arm64"] = "ARMV8A_AARCH64", ["aarch64"] = "ARMV8A_AARCH64", ["armv8"] = "ARMV8A_AARCH64",
            ["armv8a_aarch64"] = "ARMV8A_AARCH64", ["armv8_halffp"] = "ARMV8A_AARCH64_HALFFP", ["armv8a_aarch64_halffp"] = "ARMV8A_AARCH64_HALFFP",
            ["armv9"] = "ARMV9A", ["armv9a"] = "ARMV9A", ["armv7"] = "ARMV7A_NEON32", ["neon32"] = "ARMV7A_NEON32", ["armv7a_neon32"] = "ARMV7A_NEON32",
            ["thumb2"] = "THUMB2_NEON32", ["thumb2_neon32"] = "THUMB2_NEON32", ["wasm"] = "WASM32", ["wasm32"] = "WASM32", ["wasm64"] = "WASM64",
        };

        public static string ParseTarget(string s)
        {
            if (s_Targets.TryGetValue(s.Trim(), out var t))
                return t;
            if (s.Contains("512", StringComparison.Ordinal))
                throw new WaitressException("Burst 1.8 has no AVX-512 target (the Inspector doesn't offer one and the compiler rejects --target=AVX512); `burstwaitress targets` lists what this Editor supports");
            throw new WaitressException($"unknown target '{s}'; use avx2, avx, sse4, sse2, arm64 (neon), armv7, armv9, wasm or auto (`burstwaitress targets` lists them)");
        }

        public static string ParseFloatMode(string s)
        {
            foreach (var m in new[] { "Default", "Strict", "Deterministic", "Fast" })
                if (m.Equals(s.Trim(), StringComparison.OrdinalIgnoreCase))
                    return m;
            if (s.Trim().Equals("attr", StringComparison.OrdinalIgnoreCase) || s.Trim().Equals("attribute", StringComparison.OrdinalIgnoreCase))
                return null;
            throw new WaitressException($"unknown float mode '{s}'; use Default, Strict, Deterministic, Fast or attr (the method's own)");
        }

        public static string ParseFloatPrecision(string s)
        {
            foreach (var m in new[] { "Standard", "High", "Medium", "Low" })
                if (m.Equals(s.Trim(), StringComparison.OrdinalIgnoreCase))
                    return m;
            if (s.Trim().Equals("attr", StringComparison.OrdinalIgnoreCase))
                return null;
            throw new WaitressException($"unknown float precision '{s}'; use Standard, High, Medium, Low or attr");
        }

        public static string ParseSafety(string s)
        {
            var v = s.Trim().ToLowerInvariant();
            if (v is "on" or "off" or "force")
                return v;
            throw new WaitressException($"--safety takes on, off or force (on keeps [BurstCompile(DisableSafetyChecks = true)]; force overrides it)");
        }

        /// <summary>Every combination of the comma-separated values of --target, --float-mode, --float-precision and --safety.</summary>
        public static List<CompileOptions> Sets(Args args, bool debug = false)
        {
            var targets = Split(args.Get("target", "avx2")).Select(ParseTarget).ToList();
            var modes = Split(args.Get("float-mode", "attr")).Select(ParseFloatMode).ToList();
            var precisions = Split(args.Get("float-precision", "attr")).Select(ParseFloatPrecision).ToList();
            var safeties = Split(args.Get("safety", "off")).Select(ParseSafety).ToList();
            var list = new List<CompileOptions>();
            foreach (var t in targets)
                foreach (var m in modes)
                    foreach (var p in precisions)
                        foreach (var s in safeties)
                            list.Add(new CompileOptions { Target = t, FloatMode = m, FloatPrecision = p, Safety = s, Debug = debug });
            return list;
        }

        public static List<string> Split(string s) => s.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
    }
}
