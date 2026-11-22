using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ClipWaitress.Model
{
    public static class EditorScript
    {
        public static string Source()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClipWaitressEditor.cs");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static string Install(string root)
        {
            var path = Path.Combine(root, "UserSettings", "Waitress", "ClipWaitress", "ClipWaitressEditor.cs");
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
                ["entry"] = "ClipWaitressEditor." + entry,
                ["args"] = JsonSerializer.Serialize(args),
            };
            var reply = UnityCli.RunScript(root, call, 3600);
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
}

namespace ClipWaitress.Model
{
    /// <summary>No Editor has the project open, as opposed to the Editor answering with an error.</summary>
    public sealed class EditorUnavailable : WaitressException
    {
        public EditorUnavailable(string message) : base(message) { }
    }
}
