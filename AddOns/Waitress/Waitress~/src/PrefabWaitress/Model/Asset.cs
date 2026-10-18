using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrefabWaitress.Yaml;

namespace PrefabWaitress.Model
{
    public enum AssetKind
    {
        Scene,
        Prefab,
        Model,
        Missing,
        Other,
    }

    public enum ModStatus
    {
        Applied,
        /// <summary>Applied, but sets the value the source already has.</summary>
        Redundant,
        /// <summary>The target object no longer exists in the source.</summary>
        NoTarget,
        /// <summary>The source object has no such property.</summary>
        NoProperty,
        /// <summary>The source can't be read (a model file or a missing asset), so the override wasn't checked.</summary>
        Unchecked,
    }

    /// <summary>One entry of a prefab instance's modification list.</summary>
    public sealed class Mod
    {
        public Layer Layer;
        public int Index;
        public Ref Target;
        public string Path;
        public string Value;
        public YNode ObjectReference;
        public ModStatus Status;
        public string Detail;
        public Obj TargetObj;
        /// <summary>The value before this override applied, when it applied.</summary>
        public YNode Before;

        public bool SetsReference => ObjectReference != null && !ObjectReference.AsRef().IsNull;

        public YNode NewValue(YNode previous)
        {
            if (SetsReference || (previous != null && previous.IsRef()))
                return ObjectReference?.Clone() ?? YNodeExtensions.MakeRef(default);
            return new YScalar(Value ?? "");
        }
    }

    /// <summary>A PrefabInstance in a file: the source it instantiates and the changes it makes to it.</summary>
    public sealed class Layer
    {
        public Asset Owner;
        public long Id;
        public string SourceGuid;
        public Asset Source;
        public long TransformParent;
        public readonly List<Mod> Mods = new List<Mod>();
        public readonly List<Ref> RemovedComponents = new List<Ref>();
        public readonly List<Ref> RemovedGameObjects = new List<Ref>();
        public readonly List<(Ref target, int index, long added)> AddedGameObjects = new List<(Ref, int, long)>();
        public readonly List<(Ref target, int index, long added)> AddedComponents = new List<(Ref, int, long)>();
        public Obj Root;
        public int Line;
        /// <summary>Ids that stripped objects in the file give to source objects; files written before Unity 2018.3 don't follow the XOR rule.</summary>
        public readonly Dictionary<long, long> IdMap = new Dictionary<long, long>();

        public long Map(long sourceId) => IdMap.TryGetValue(sourceId, out var id) ? id : Asset.InstancedId(Id, sourceId);
    }

    /// <summary>An object as it exists in one file: written there, or brought in by a prefab instance.</summary>
    public sealed class Obj
    {
        public Asset Owner;
        public long Id;
        public int ClassId;
        public string TypeName;
        YMap m_Body;

        /// <summary>The effective serialized values. An instanced object copies its source's values only when first read.</summary>
        public YMap Body
        {
            get
            {
                if (m_Body == null && Source != null)
                    Owner.Materialize(this);
                return m_Body;
            }
            set => m_Body = value;
        }

        public bool HasBody => m_Body != null;
        /// <summary>The object this one is an instance of, in the source asset; null for an object written in this file.</summary>
        public Obj Source;
        public Layer Layer;
        /// <summary>For each overridden property path, the last override that set it, from this file or any source below it.</summary>
        public Dictionary<string, Mod> SetBy = new Dictionary<string, Mod>(StringComparer.Ordinal);
        public bool Opaque;
        public int Line;

        public Obj GameObject;
        public readonly List<Obj> Components = new List<Obj>();
        public Obj Transform;
        public Obj Parent;
        public readonly List<Obj> Children = new List<Obj>();
        /// <summary>For a transform: its parent transform and child transforms.</summary>
        public Obj Father;
        public readonly List<Obj> ChildTransforms = new List<Obj>();

        public bool IsGameObject => ClassId == 1;
        public bool IsTransform => ClassId == 4 || ClassId == 224;
        public bool IsInstance => Layer != null;

        public string Name => GameObject != null && GameObject != this ? GameObject.Name : HasBody || Source == null ? Body?.Scalar("m_Name") ?? "" : Source.Name;

        /// <summary>The object this one ultimately comes from: the end of the source chain.</summary>
        public Obj Origin
        {
            get
            {
                var o = this;
                while (o.Source != null)
                    o = o.Source;
                return o;
            }
        }

        public override string ToString() => $"{TypeName} &{Id}";
    }

    public sealed class Asset
    {
        public string Path;
        public string Guid;
        public AssetKind Kind;
        public string Problem;
        public List<YamlDoc> Docs = new List<YamlDoc>();
        public readonly Dictionary<long, Obj> Objects = new Dictionary<long, Obj>();
        public readonly List<Layer> Layers = new List<Layer>();
        /// <summary>Root GameObjects in hierarchy order.</summary>
        public readonly List<Obj> Roots = new List<Obj>();
        public bool HasModelCache;
        internal bool HierarchyBuilt;
        /// <summary>Some objects come from a source that can't be read (a model or a missing asset), here or in a source below.</summary>
        public bool HasOpaque;

        internal void Materialize(Obj obj) => Project.Materialize(obj);

        public bool Readable => Kind is AssetKind.Scene or AssetKind.Prefab || HasModelCache;

        public string FileName => System.IO.Path.GetFileName(Path);

        public Obj Get(long id) => Objects.TryGetValue(id, out var o) ? o : null;

        public IEnumerable<Obj> GameObjects => Objects.Values.Where(o => o.IsGameObject);

        /// <summary>The instance id an object of a source file takes when that file is instantiated by a PrefabInstance.</summary>
        public static long InstancedId(long instance, long source) => (instance ^ source) & 0x7FFFFFFFFFFFFFFF;

        public static bool IsModelPath(string path)
        {
            var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
            return ext is ".fbx" or ".obj" or ".blend" or ".dae" or ".3ds" or ".max" or ".ma" or ".mb" or ".dxf" or ".skp" or ".c4d" or ".lxo" or ".jas" or ".gltf" or ".glb";
        }
    }

    public static class Num
    {
        public static string Str(long v) => v.ToString(CultureInfo.InvariantCulture);
    }
}
