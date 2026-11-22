using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace BurstWaitress.Model
{
    /// <summary>Installs the embedded Editor script into UserSettings/Waitress/BurstWaitress/ and runs its entry points.</summary>
    public static class EditorScript
    {
        public static string Source()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BurstWaitressEditor.cs");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static string Install(string root)
        {
            var path = Path.Combine(root, "UserSettings", "Waitress", "BurstWaitress", "BurstWaitressEditor.cs");
            var source = Source();
            if (!File.Exists(path) || File.ReadAllText(path) != source)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, source, new UTF8Encoding(false));
            }
            return path;
        }

        public static JsonElement Call(string root, string entry, params object[] args)
        {
            var script = Install(root);
            var call = new Dictionary<string, string>
            {
                ["file"] = script,
                ["entry"] = "BurstWaitressEditor." + entry,
                ["args"] = JsonSerializer.Serialize(args),
            };
            var reply = UnityCli.RunScript(root, call, 120);
            // Right after a script compile the Pipeline refuses commands for a moment while the domain reloads.
            // An open Editor keeps Library/EditorInstance.json; while it reloads its domain the CLI can't reach it at all.
            var open = File.Exists(Path.Combine(root, "Library", "EditorInstance.json"));
            for (var attempt = 0; attempt < 6 && !reply.Success && !reply.MissingCommand &&
                                  (reply.Reached && (reply.Error ?? "").Contains("400", System.StringComparison.Ordinal) || !reply.Reached && open); attempt++)
            {
                System.Threading.Thread.Sleep(3000);
                reply = UnityCli.RunScript(root, call, 120);
            }
            if (!reply.Success)
            {
                if (!reply.Reached)
                    throw new EditorUnavailable($"{entry} needs the Unity Editor: no Editor has this project open ({reply.Error}). Open the project with `unity open \"{root}\"` and retry");
                throw new WaitressException($"the Editor couldn't run {entry}: {reply.Error}");
            }
            JsonElement result;
            try
            {
                result = JsonDocument.Parse(reply.Result).RootElement;
            }
            catch (JsonException)
            {
                throw new WaitressException($"unexpected reply from {entry}: " + reply.Result);
            }
            if (result.TryGetProperty("ok", out var fine) && fine.ValueKind == JsonValueKind.False)
                throw new WaitressException(result.GetProperty("error").GetString());
            return result;
        }
    }

    /// <summary>No Editor has the project open, as opposed to the Editor answering with an error.</summary>
    public sealed class EditorUnavailable : WaitressException
    {
        public EditorUnavailable(string message) : base(message) { }
    }
}
