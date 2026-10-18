using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VfxWaitress.Query;
using VfxWaitress.Serialization;

namespace VfxWaitress.Model
{
    /// <summary>
    /// The VisualEffectResource settings the asset inspector shows (m_Infos), by the names
    /// `show` prints and `resource set` takes. The enums are internal to Unity, so their stored
    /// values are mirrored here.
    /// </summary>
    public static class ResourceSettings
    {
        // VFXUpdateMode bits.
        const int kDeltaTime = 1;
        const int kIgnoreTimeScale = 2;
        const int kExactFixedTimeStep = 4;

        // VFXCullingFlags, limited to the three combinations the inspector offers.
        static readonly (string name, int value)[] s_Culling =
        {
            ("CullDefault", 3),
            ("CullSimulation", 1),
            ("CullNone", 0),
        };

        // VFXInstancingMode.
        static readonly (string name, int value)[] s_Instancing =
        {
            ("Auto", 0),
            ("Disabled", -1),
            ("Custom", 1),
        };

        // The inspector's prewarm limits.
        const float kMinPrewarmDeltaTime = 1.0f / 800.0f;
        const int kMaxPrewarmSteps = 2400;

        public static readonly string[] Keys =
        {
            "updateMode", "ignoreTimeScale", "culling", "initialEvent", "instancing", "instancingCapacity",
            "prewarmTime", "prewarmSteps", "prewarmDeltaTime",
        };

        /// <summary>The settings as `key=value` pairs. Off-by-default extras are left out.</summary>
        public static IEnumerable<string> Describe(YMap infos)
        {
            var initialEvent = infos.Scalar("m_InitialEventName");
            if (initialEvent != null)
                yield return "initialEvent=" + (initialEvent.Length == 0 ? "\"\"" : initialEvent);

            if (TryInt(infos.Scalar("m_UpdateMode"), out var update))
            {
                yield return "updateMode=" + UpdateModeName(update);
                if ((update & kIgnoreTimeScale) != 0)
                    yield return "ignoreTimeScale=true";
            }

            if (TryInt(infos.Scalar("m_CullingFlags"), out var culling))
                yield return "culling=" + NameOf(s_Culling, culling);

            if (TryInt(infos.Scalar("m_InstancingMode"), out var instancing))
            {
                yield return "instancing=" + NameOf(s_Instancing, instancing);
                if (instancing == 1)
                    yield return "instancingCapacity=" + infos.Scalar("m_InstancingCapacity");
            }

            var steps = infos.Scalar("m_PreWarmStepCount");
            if (steps != null && steps != "0")
            {
                yield return "prewarmSteps=" + steps;
                yield return "prewarmDeltaTime=" + infos.Scalar("m_PreWarmDeltaTime");
            }
        }

        /// <summary>
        /// Writes the given settings onto the resource and returns one line per stored value.
        /// Every value is checked before anything is written.
        /// </summary>
        public static List<string> Apply(VfxAsset asset, IReadOnlyDictionary<string, string> values)
        {
            if (asset.Kind != "vfx")
                throw new VfxWaitressException($"a .{asset.Kind} has no resource settings of its own; they belong to the .vfx that uses it");
            var infos = asset.ResourceDoc?.Body.Map("m_Infos")
                        ?? throw new VfxWaitressException("this asset has no VisualEffectResource settings (m_Infos) to edit");
            if (values.Count == 0)
                throw new VfxWaitressException("nothing to set; keys: " + string.Join(", ", Keys));

            var pending = new Dictionary<string, string>(StringComparer.Ordinal);
            string Current(string key) => pending.TryGetValue(key, out var v) ? v : infos.Scalar(key);
            foreach (var key in values.Keys)
            {
                if (!Keys.Any(k => Loose.Equal(k, key)))
                    throw new VfxWaitressException($"no resource setting '{key}'; one of: {string.Join(", ", Keys)}");
            }
            string Get(string key) => values.FirstOrDefault(kv => Loose.Equal(kv.Key, key)).Value;

            var updateMode = Get("updateMode");
            var ignoreTimeScale = Get("ignoreTimeScale");
            if (updateMode != null || ignoreTimeScale != null)
            {
                TryInt(infos.Scalar("m_UpdateMode"), out var mode);
                if (updateMode != null)
                {
                    mode &= kIgnoreTimeScale;
                    if (Loose.Equal(updateMode, "DeltaTime"))
                        mode |= kDeltaTime;
                    else if (Loose.Equal(updateMode, "ExactFixedTime") || Loose.Equal(updateMode, "ExactFixedTimeStep"))
                        mode |= kExactFixedTimeStep;
                    else if (!Loose.Equal(updateMode, "FixedDeltaTime"))
                        throw new VfxWaitressException($"'{updateMode}' is not an update mode; one of: FixedDeltaTime, DeltaTime, ExactFixedTime");
                }
                if (ignoreTimeScale != null)
                    mode = Bool(ignoreTimeScale, "ignoreTimeScale") ? mode | kIgnoreTimeScale : mode & ~kIgnoreTimeScale;
                pending["m_UpdateMode"] = mode.ToString(CultureInfo.InvariantCulture);
            }

            var culling = Get("culling");
            if (culling != null)
            {
                var value = ValueOf(s_Culling, culling, "culling");
                var automatic = asset.Datas.Values.FirstOrDefault(d => d.Doc.Body.Scalar("boundsMode") == "2");
                if (value != 0 && automatic != null)
                    throw new VfxWaitressException(
                        $"system {automatic.ShortId} computes its bounds automatically, so VFX Graph forces culling=CullNone when it compiles; " +
                        "set that system's boundsMode to Manual or Recorded first");
                pending["m_CullingFlags"] = value.ToString(CultureInfo.InvariantCulture);
            }

            var initialEvent = Get("initialEvent");
            if (initialEvent != null)
            {
                if (initialEvent.Trim() != initialEvent || initialEvent.IndexOfAny(":#{}[],&*!|>'\"%@`".ToCharArray()) >= 0)
                    throw new VfxWaitressException($"'{initialEvent}' would need YAML quoting; give an event name without punctuation or edge spaces");
                pending["m_InitialEventName"] = initialEvent;
            }

            var instancing = Get("instancing");
            if (instancing != null)
                pending["m_InstancingMode"] = ValueOf(s_Instancing, instancing, "instancing").ToString(CultureInfo.InvariantCulture);
            var capacity = Get("instancingCapacity");
            if (capacity != null)
            {
                if (!int.TryParse(capacity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var c) || c < 1)
                    throw new VfxWaitressException($"instancingCapacity takes a whole number of at least 1, got '{capacity}'");
                if (Current("m_InstancingMode") != "1")
                    throw new VfxWaitressException("instancingCapacity is only used with instancing=Custom; set both");
                pending["m_InstancingCapacity"] = c.ToString(CultureInfo.InvariantCulture);
            }

            ApplyPrewarm(infos, Get("prewarmTime"), Get("prewarmSteps"), Get("prewarmDeltaTime"), pending);

            var lines = new List<string>();
            foreach (var kv in pending)
            {
                infos.SetScalar(kv.Key, kv.Value);
                lines.Add($"resource: {kv.Key} = {kv.Value}");
            }
            return lines;
        }

        /// <summary>
        /// Prewarm is stored as a step count and a step length. A total time keeps whichever of the
        /// two was given, or else the current step length, and derives the other.
        /// </summary>
        static void ApplyPrewarm(YMap infos, string time, string stepsText, string deltaText, Dictionary<string, string> pending)
        {
            if (time == null && stepsText == null && deltaText == null)
                return;
            if (time != null && stepsText != null && deltaText != null)
                throw new VfxWaitressException("prewarmTime is prewarmSteps × prewarmDeltaTime; give at most two of the three");

            TryInt(infos.Scalar("m_PreWarmStepCount"), out var steps);
            if (!float.TryParse(infos.Scalar("m_PreWarmDeltaTime"), NumberStyles.Float, CultureInfo.InvariantCulture, out var delta) || delta < kMinPrewarmDeltaTime)
                delta = 0.05f;

            if (stepsText != null)
            {
                if (!int.TryParse(stepsText, NumberStyles.Integer, CultureInfo.InvariantCulture, out steps) || steps < 0 || steps > kMaxPrewarmSteps)
                    throw new VfxWaitressException($"prewarmSteps takes a whole number from 0 to {kMaxPrewarmSteps}, got '{stepsText}'");
            }
            if (deltaText != null)
                delta = Seconds(deltaText, "prewarmDeltaTime");

            if (time != null)
            {
                var total = float.TryParse(time, NumberStyles.Float, CultureInfo.InvariantCulture, out var t) && t >= 0
                    ? t
                    : throw new VfxWaitressException($"prewarmTime takes seconds, 0 or more, got '{time}'");
                if (total == 0)
                {
                    steps = 0;
                }
                else if (stepsText != null)
                {
                    if (steps == 0)
                        throw new VfxWaitressException("a non-zero prewarmTime needs at least one step");
                    delta = total / steps;
                    if (delta < kMinPrewarmDeltaTime)
                        throw new VfxWaitressException($"{total}s over {steps} steps is under the shortest step VFX Graph allows ({kMinPrewarmDeltaTime}s)");
                }
                else
                {
                    steps = Math.Max(1, (int)Math.Round(total / delta));
                    if (steps > kMaxPrewarmSteps)
                        throw new VfxWaitressException($"{total}s at {delta}s per step needs {steps} steps, over VFX Graph's {kMaxPrewarmSteps}; give a longer prewarmDeltaTime");
                    if (deltaText == null)
                        delta = total / steps;
                }
            }

            pending["m_PreWarmDeltaTime"] = delta.ToString("R", CultureInfo.InvariantCulture);
            pending["m_PreWarmStepCount"] = steps.ToString(CultureInfo.InvariantCulture);
        }

        static float Seconds(string text, string key)
        {
            if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || value < kMinPrewarmDeltaTime)
                throw new VfxWaitressException($"{key} takes seconds, at least {kMinPrewarmDeltaTime} (1/800), got '{text}'");
            return value;
        }

        static string UpdateModeName(int mode) =>
            (mode & kDeltaTime) != 0 ? "DeltaTime" : (mode & kExactFixedTimeStep) != 0 ? "ExactFixedTime" : "FixedDeltaTime";

        static string NameOf((string name, int value)[] table, int value) =>
            table.FirstOrDefault(e => e.value == value).name ?? value.ToString(CultureInfo.InvariantCulture);

        static int ValueOf((string name, int value)[] table, string text, string key)
        {
            foreach (var (name, value) in table)
            {
                if (Loose.Equal(name, text))
                    return value;
            }
            throw new VfxWaitressException($"'{text}' is not a value of {key}; one of: {string.Join(", ", table.Select(e => e.name))}");
        }

        static bool Bool(string text, string key)
        {
            if (Loose.Equal(text, "true") || text == "1")
                return true;
            if (Loose.Equal(text, "false") || text == "0")
                return false;
            throw new VfxWaitressException($"{key} takes true or false, got '{text}'");
        }

        static bool TryInt(string text, out int value) =>
            int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
