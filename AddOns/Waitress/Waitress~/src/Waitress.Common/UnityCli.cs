using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Waitress
{
    /// <summary>
    /// Calls a Pipeline command in the Editor that has a given project open, through the unity CLI.
    /// The Waitress samples register their Editor-side operations as Pipeline commands, so a call is
    /// a direct method invocation rather than a compiled script.
    /// </summary>
    public static class UnityCli
    {
        public struct Reply
        {
            /// <summary>The Editor answered, whether or not the command succeeded.</summary>
            public bool Reached;
            public bool Success;
            /// <summary>The command's result as text: a string result verbatim, anything else as JSON.</summary>
            public string Result;
            public string Error;
            /// <summary>The Editor answered, but no such command is registered.</summary>
            public bool MissingCommand;
        }

        /// <summary>
        /// Runs a script file through the Pipeline's script runner: `script run` since Pipeline 0.9, `run_script` before.
        /// On success, <see cref="Reply.Result"/> is the entry point's return value.
        /// </summary>
        public static Reply RunScript(string projectRoot, IReadOnlyDictionary<string, string> args, int timeoutSeconds = 300)
        {
            var reply = Call(projectRoot, "script run", args, timeoutSeconds);
            if (reply.MissingCommand)
                reply = Call(projectRoot, "run_script", args, timeoutSeconds);
            return UnwrapScriptRun(reply);
        }

        /// <summary>A grouped command is passed with its group first, separated by a space, such as "script run".</summary>
        public static Reply Call(string projectRoot, string command, IReadOnlyDictionary<string, string> args, int timeoutSeconds = 300)
        {
            var psi = new ProcessStartInfo("unity")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            psi.ArgumentList.Add("command");
            foreach (var part in command.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                psi.ArgumentList.Add(part);
            if (args != null)
            {
                foreach (var pair in args)
                {
                    psi.ArgumentList.Add("--" + pair.Key);
                    psi.ArgumentList.Add(pair.Value ?? "");
                }
            }
            if (!string.IsNullOrEmpty(projectRoot))
            {
                psi.ArgumentList.Add("--project-path");
                psi.ArgumentList.Add(projectRoot);
            }
            psi.ArgumentList.Add("--timeout");
            psi.ArgumentList.Add(timeoutSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
            psi.ArgumentList.Add("--caller");
            psi.ArgumentList.Add("waitress");
            psi.ArgumentList.Add("--format");
            psi.ArgumentList.Add("json");

            string stdout, stderr;
            int exitCode;
            try
            {
                using var process = Process.Start(psi);
                if (process == null)
                    return new Reply { Error = "could not start the unity CLI" };
                var readError = process.StandardError.ReadToEndAsync();
                stdout = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                stderr = readError.Result;
                exitCode = process.ExitCode;
            }
            catch (Exception e)
            {
                return new Reply { Error = "could not start the unity CLI: " + e.Message };
            }
            return TryParseEnvelope(stdout, out var reply) ? reply : ParseCommandOutput(stdout, stderr, exitCode);
        }

        /// <summary>
        /// The CLI's own envelope: what every reply looked like before Pipeline 1.0, and still what the CLI prints
        /// when it fails before reaching an Editor.
        /// </summary>
        static bool TryParseEnvelope(string stdout, out Reply reply)
        {
            reply = default;
            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(stdout);
            }
            catch (JsonException)
            {
                return false;
            }
            using (json)
            {
                var root = json.RootElement;
                if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("success", out var s) ||
                    (s.ValueKind != JsonValueKind.True && s.ValueKind != JsonValueKind.False) || !root.TryGetProperty("command", out _))
                    return false;
                if (s.ValueKind == JsonValueKind.True)
                {
                    var result = root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object &&
                                 data.TryGetProperty("result", out var r)
                        ? ResultText(r)
                        : "";
                    reply = new Reply { Reached = true, Success = true, Result = result };
                    return true;
                }

                var message = new StringBuilder();
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
                {
                    foreach (var error in errors.EnumerateArray())
                    {
                        if (error.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                            message.Append(message.Length > 0 ? "; " : "").Append(m.GetString());
                    }
                }
                var text = message.ToString();
                var missing = text.Contains("Command Not Found", StringComparison.Ordinal);
                // "Pipeline server returned" means the Editor answered and refused. Anything else
                // is the CLI failing to reach an Editor at all.
                var reached = missing || text.Contains("Pipeline server returned", StringComparison.Ordinal);
                if (missing)
                {
                    var cut = text.IndexOf(" Available:", StringComparison.Ordinal);
                    if (cut > 0)
                        text = text.Substring(0, cut);
                }
                reply = new Reply { Reached = reached, MissingCommand = missing, Error = text.Length > 0 ? text : stdout.Trim() };
                return true;
            }
        }

        /// <summary>
        /// Pipeline 1.0 and newer write the command's result itself to stdout and the reason for a failure to stderr,
        /// and report the outcome in the exit code.
        /// </summary>
        static Reply ParseCommandOutput(string stdout, string stderr, int exitCode)
        {
            const int k_NoEditor = 12;
            if (exitCode == 0)
            {
                string result;
                try
                {
                    using var json = JsonDocument.Parse(stdout);
                    result = ResultText(json.RootElement);
                }
                catch (JsonException)
                {
                    result = stdout.Trim();
                }
                return new Reply { Reached = true, Success = true, Result = result };
            }

            var error = ErrorText(stderr);
            if (error.Length == 0)
                error = stdout.Trim().Length > 0 ? stdout.Trim() : $"the unity CLI exited with code {exitCode}";
            // Exit code 13 also covers a script entry point that doesn't exist, so the message decides.
            var missing = error.StartsWith("No command named", StringComparison.Ordinal);
            return new Reply { Reached = exitCode != k_NoEditor, MissingCommand = missing, Error = error };
        }

        static string ErrorText(string stderr)
        {
            var sb = new StringBuilder();
            foreach (var raw in stderr.Replace("\r\n", "\n").Split('\n'))
            {
                var line = raw.TrimEnd();
                if (line.Length == 0)
                    continue;
                if (line.StartsWith("error: ", StringComparison.Ordinal))
                    line = line.Substring("error: ".Length);
                sb.Append(sb.Length > 0 ? "\n" : "").Append(line);
            }
            return sb.ToString();
        }

        static string ResultText(JsonElement result) => result.ValueKind == JsonValueKind.String ? result.GetString() : result.GetRawText();

        /// <summary>
        /// Before Pipeline 1.0 the script runner answered with an object holding success, result, errorDetails and
        /// diagnostics; since 1.0 its result is the entry point's return value and a failure is reported like any
        /// other command's.
        /// </summary>
        static Reply UnwrapScriptRun(Reply reply)
        {
            if (!reply.Success)
                return reply;
            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(reply.Result);
            }
            catch (JsonException)
            {
                return reply;
            }
            using (json)
            {
                var run = json.RootElement;
                if (run.ValueKind != JsonValueKind.Object || !run.TryGetProperty("result", out var result))
                    return reply;
                if (!run.TryGetProperty("success", out var ok))
                    return new Reply { Reached = true, Success = true, Result = ResultText(result) };
                if (ok.ValueKind == JsonValueKind.True)
                    return new Reply { Reached = true, Success = true, Result = ResultText(result) };

                var sb = new StringBuilder();
                if (run.TryGetProperty("errorDetails", out var details) && details.ValueKind == JsonValueKind.String)
                    sb.Append(details.GetString());
                else if (run.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.String)
                    sb.Append(error.GetString());
                if (run.TryGetProperty("diagnostics", out var diagnostics) && diagnostics.ValueKind == JsonValueKind.Array)
                    foreach (var d in diagnostics.EnumerateArray())
                        sb.Append("\n  line ").Append(d.GetProperty("line")).Append(": ").Append(d.GetProperty("message").GetString());
                return new Reply { Reached = true, Error = sb.ToString() };
            }
        }
    }
}
