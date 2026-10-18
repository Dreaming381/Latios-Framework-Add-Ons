using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProfilerWaitress.Data;

namespace ProfilerWaitress.Cli
{
    /// <summary>The frames and threads a command looks at, parsed from --frames and --thread.</summary>
    public sealed class Scope
    {
        public int[] Frames;
        public bool[] Threads;
        public string FramesText;
        public string ThreadsText;
        /// <summary>--internal: show the job system's bookkeeping markers, which the Profiler window hides.</summary>
        public bool ShowInternal;

        public bool HasThread(int thread) => Threads[thread];

        public static Scope Parse(CaptureFile capture, Args args, string defaultThreads)
        {
            var framesText = args.Get("frames") ?? args.Get("f");
            var threadsText = args.Get("thread") ?? args.Get("t") ?? defaultThreads;
            return new Scope
            {
                Frames = ParseFrames(capture, framesText),
                Threads = ParseThreads(capture, threadsText),
                FramesText = framesText,
                ThreadsText = threadsText,
                ShowInternal = args.Flag("internal"),
            };
        }

        /// <summary>
        /// Frame numbers as the Profiler numbers them: "all", "N", "A-B", "A-", "-B", comma-separated; "first:N",
        /// "last:N", and "skip:N" (all but the first N frames, which usually hold warm-up hitches).
        /// Returns indices into the capture's frame table.
        /// </summary>
        public static int[] ParseFrames(CaptureFile capture, string text)
        {
            int first = capture.FrameNumber(0), last = capture.FrameNumber(capture.FrameCount - 1);
            if (string.IsNullOrEmpty(text) || text == "all")
                return Enumerable.Range(0, capture.FrameCount).ToArray();
            var set = new SortedSet<int>();
            foreach (var raw in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                int a, b;
                if (TrySuffix(raw, "first:", out var n))
                {
                    a = first;
                    b = Math.Min(last, first + n - 1);
                }
                else if (TrySuffix(raw, "last:", out n))
                {
                    a = Math.Max(first, last - n + 1);
                    b = last;
                }
                else if (TrySuffix(raw, "skip:", out n))
                {
                    a = first + n;
                    b = last;
                }
                else
                {
                    var dash = raw.IndexOf('-', 1);
                    if (raw.StartsWith("-", StringComparison.Ordinal))
                    {
                        a = first;
                        b = Int(raw.Substring(1));
                    }
                    else if (dash < 0)
                        a = b = Int(raw);
                    else
                    {
                        a = Int(raw.Substring(0, dash));
                        b = dash == raw.Length - 1 ? last : Int(raw.Substring(dash + 1));
                        if (b < a)
                            throw new WaitressException($"--frames {raw} is reversed; write {b}-{a}");
                    }
                }
                a = Math.Max(a, first);
                b = Math.Min(b, last);
                for (var f = a; f <= b; f++)
                    set.Add(f - first);
            }
            if (set.Count == 0)
                throw new WaitressException($"--frames {text} selects nothing; this capture has frames {first}-{last}");
            return set.ToArray();
        }

        static bool TrySuffix(string raw, string prefix, out int n)
        {
            n = 0;
            return raw.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && int.TryParse(raw.Substring(prefix.Length), out n);
        }

        static int Int(string s)
        {
            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                throw new WaitressException($"bad frame number '{s}'");
            return v;
        }

        /// <summary>
        /// Comma-separated terms: main, render, workers (Job/*), background (Background Job/*), jobs (workers +
        /// background), all, or a glob over "Group/Name" (or just the name), e.g. "Job/Worker 1*", "Loading/*".
        /// A leading '!' excludes.
        /// </summary>
        public static bool[] ParseThreads(CaptureFile capture, string text)
        {
            var selected = new bool[capture.Threads.Length];
            var terms = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (terms.Length > 0 && terms[0].StartsWith("!", StringComparison.Ordinal))
                Array.Fill(selected, true);
            foreach (var raw in terms)
            {
                var exclude = raw.StartsWith("!", StringComparison.Ordinal);
                var term = (exclude ? raw.Substring(1) : raw).Trim();
                if (term.Length == 0)
                    throw new WaitressException($"--thread '{text}' has an empty term");
                Func<ThreadInfo, bool> match = term.ToLowerInvariant() switch
                {
                    "main" => t => t.Kind == ThreadKind.Main,
                    "render" => t => t.Kind == ThreadKind.Render,
                    "workers" or "worker" => t => t.Kind == ThreadKind.Worker,
                    "background" => t => t.Kind == ThreadKind.Background,
                    "jobs" => t => t.Kind == ThreadKind.Worker || t.Kind == ThreadKind.Background,
                    "all" or "*" => _ => true,
                    _ => term.Contains('*') || term.Contains('?') ? GlobFor(term) : null,
                };
                // A plain name that is some thread's exact name selects only those threads, so "Job/Worker 1" is not also Worker 10-19.
                if (match == null)
                {
                    var exact = capture.Threads.Any(t => string.Equals(t.Display, term, StringComparison.OrdinalIgnoreCase) || string.Equals(t.Name, term, StringComparison.OrdinalIgnoreCase));
                    match = exact
                        ? t => string.Equals(t.Display, term, StringComparison.OrdinalIgnoreCase) || string.Equals(t.Name, term, StringComparison.OrdinalIgnoreCase)
                        : GlobFor(term);
                }
                var hit = false;
                for (var i = 0; i < selected.Length; i++)
                {
                    if (!match(capture.Threads[i]))
                        continue;
                    selected[i] = !exclude;
                    hit = true;
                }
                if (!hit && !exclude)
                    throw new WaitressException($"--thread '{term}' matches no thread; see `profilerwaitress threads <capture>`");
            }
            if (!selected.Any(x => x))
                throw new WaitressException($"--thread '{text}' selects no thread");
            return selected;
        }

        /// <summary>--top N: rows to print, 0 for all.</summary>
        public static int Top(Args args, int fallback)
        {
            var top = args.GetInt("top", fallback);
            if (top < 0)
                throw new WaitressException("--top takes a row count, or 0 for all");
            return top;
        }

        static Func<ThreadInfo, bool> GlobFor(string term)
        {
            var glob = Glob.Compile(term.Contains('*') || term.Contains('?') ? term : "*" + term + "*");
            return t => glob(t.Display) || glob(t.Name);
        }

        public string Describe(CaptureFile capture)
        {
            var contiguous = Frames[^1] - Frames[0] + 1 == Frames.Length;
            var frames = Frames.Length == capture.FrameCount
                ? $"all {Frames.Length} frames"
                : contiguous
                    ? $"{Frames.Length} frames ({capture.FrameNumber(Frames[0])}-{capture.FrameNumber(Frames[^1])})"
                    : $"{Frames.Length} frames (--frames {FramesText})";
            return $"{frames}, thread {ThreadsText}";
        }
    }

    /// <summary>
    /// Resolves a marker pattern. A glob ('*', '?') must match the whole name. A plain word matches that exact name
    /// if one exists, otherwise every name containing it.
    /// </summary>
    public static class MarkerMatch
    {
        public static bool[] Resolve(CaptureFile capture, string pattern, bool required = true)
        {
            var hits = new bool[capture.Markers.Length];
            var any = false;
            foreach (var raw in pattern.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (raw.Contains('*') || raw.Contains('?'))
                {
                    var glob = Glob.Compile(raw);
                    for (var i = 0; i < hits.Length; i++)
                        if (glob(capture.Markers[i].Name))
                            any = hits[i] = true;
                    continue;
                }
                var exact = false;
                for (var i = 0; i < hits.Length; i++)
                    if (string.Equals(capture.Markers[i].Name, raw, StringComparison.OrdinalIgnoreCase))
                        exact = any = hits[i] = true;
                if (exact)
                    continue;
                for (var i = 0; i < hits.Length; i++)
                    if (capture.Markers[i].Name.Contains(raw, StringComparison.OrdinalIgnoreCase))
                        any = hits[i] = true;
            }
            if (!any && required)
                throw new WaitressException($"no marker matches '{pattern}'; list candidates with `profilerwaitress top <capture> --thread all --marker '{(pattern.Contains('*') ? pattern : "*" + pattern + "*")}'`");
            return hits;
        }

        public static List<int> List(bool[] hits)
        {
            var list = new List<int>();
            for (var i = 0; i < hits.Length; i++)
                if (hits[i])
                    list.Add(i);
            return list;
        }
    }
}
