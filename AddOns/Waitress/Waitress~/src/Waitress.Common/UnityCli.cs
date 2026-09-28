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

        public static Reply Call(string projectRoot, string command, IReadOnlyDictionary<string, string> args, int timeoutSeconds = 300)
        {
            var psi = new ProcessStartInfo("unity")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
            };
            psi.ArgumentList.Add("command");
            psi.ArgumentList.Add(command);
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

            string stdout;
            try
            {
                using var process = Process.Start(psi);
                if (process == null)
                    return new Reply { Error = "could not start the unity CLI" };
                var readError = process.StandardError.ReadToEndAsync();
                stdout = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                readError.Wait();
            }
            catch (Exception e)
            {
                return new Reply { Error = "could not start the unity CLI: " + e.Message };
            }
            return Parse(stdout);
        }

        static Reply Parse(string stdout)
        {
            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(stdout);
            }
            catch (JsonException)
            {
                return new Reply { Error = "the unity CLI did not answer with JSON: " + stdout.Trim() };
            }
            using (json)
            {
                var root = json.RootElement;
                var success = root.TryGetProperty("success", out var s) && s.ValueKind == JsonValueKind.True;
                if (success)
                {
                    var result = root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object &&
                                 data.TryGetProperty("result", out var r)
                        ? (r.ValueKind == JsonValueKind.String ? r.GetString() : r.GetRawText())
                        : "";
                    return new Reply { Reached = true, Success = true, Result = result };
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
                return new Reply { Reached = reached, MissingCommand = missing, Error = text.Length > 0 ? text : stdout.Trim() };
            }
        }
    }
}
