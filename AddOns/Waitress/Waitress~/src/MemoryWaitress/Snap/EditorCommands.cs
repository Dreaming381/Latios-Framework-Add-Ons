using System.Collections.Generic;
using System.Text.Json;

namespace MemoryWaitress.Snap
{
    /// <summary>Runs the Pipeline's own commands in the Editor that has the project open.</summary>
    public static class EditorCommands
    {
        public static JsonElement Call(string root, string command, IReadOnlyDictionary<string, string> args = null, int timeoutSeconds = 60)
        {
            var reply = UnityCli.Call(root, command, args, timeoutSeconds);
            if (!reply.Success)
            {
                if (reply.MissingCommand)
                    throw new WaitressException($"the Editor has no `{command}` command; it needs com.unity.pipeline 0.10 or newer " +
                                                $"(update it with `unity pipeline install --force --project-path \"{root}\"`)");
                var why = reply.Reached ? reply.Error : "no Editor has this project open (" + reply.Error + ")";
                throw new WaitressException($"`{command}` needs the Unity Editor: {why}. Open the project with `unity open \"{root}\"` and retry; " +
                                            "reading snapshots that already exist works without it");
            }
            // Commands such as editor.playmode start succeed without printing anything.
            if (string.IsNullOrWhiteSpace(reply.Result))
                return default;
            try
            {
                return JsonDocument.Parse(reply.Result).RootElement;
            }
            catch (JsonException)
            {
                throw new WaitressException($"unexpected reply from `{command}`: " + reply.Result);
            }
        }

        public static bool Playing(string root) => Call(root, "editor status").GetProperty("playMode").GetString() != "stopped";
    }
}
