// The Editor half of memorywaitress. The CLI embeds this file, writes it to UserSettings/Waitress/MemoryWaitress/,
// and runs its entry points through `unity command run_script`, so nothing is imported into the project.
// Every entry point returns one line of JSON.

using System;
using System.IO;
using System.Text;
using Unity.Profiling.Memory;
using UnityEditor;

public static class MemoryWaitressEditor
{
    // Writes <path>.status when the snapshot is finished, so the CLI waits on the file system instead of polling the Editor.
    public static string Capture(string path, int flags)
    {
        if (EditorApplication.isCompiling)
            return Error("the Editor is compiling; try again when it finishes");
        var status = path + ".status";
        if (File.Exists(status))
            File.Delete(status);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        MemoryProfiler.TakeSnapshot(path, (file, ok) =>
        {
            File.WriteAllText(status, ok ? "{\"ok\":true}" : Error("Unity reported the snapshot as failed"));
        }, (CaptureFlags)flags);
        return "{\"ok\":true}";
    }

    public static string Status()
    {
        return "{\"ok\":true,\"playing\":" + (EditorApplication.isPlaying ? "true" : "false") +
               ",\"compiling\":" + (EditorApplication.isCompiling ? "true" : "false") + "}";
    }

    public static string SetPlayMode(bool playing)
    {
        EditorApplication.isPlaying = playing;
        return "{\"ok\":true}";
    }

    static string Error(string message) => "{\"ok\":false,\"error\":\"" + Escape(message) + "\"}";

    static string Escape(string s)
    {
        var sb = new StringBuilder(s.Length + 8);
        foreach (var c in s)
        {
            if (c == '"' || c == '\\')
                sb.Append('\\').Append(c);
            else if (c < ' ')
                sb.Append("\\u").Append(((int)c).ToString("x4"));
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
}
