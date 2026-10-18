using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryWaitress.Snap
{
    /// <summary>Numbers derived from a snapshot that more than one command needs.</summary>
    public sealed class Model
    {
        public readonly Snapshot S;
        public readonly Crawl C;

        /// <summary>Per native object: native allocations under its root, graphics memory under its root, and its managed shell.</summary>
        public readonly ulong[] ObjNative;
        public readonly ulong[] ObjGfx;
        public readonly long[] ObjManaged;
        public readonly int[] ObjShell;

        public readonly ulong NativeTotal;
        public readonly ulong NativeInObjects;
        public readonly ulong GfxTotal;
        public readonly ulong GfxInObjects;
        public readonly ulong HeapReserved;
        public readonly ulong VmReserved;

        public Model(Snapshot s)
        {
            S = s;
            C = s.Managed;
            var n = s.ObjName.Length;
            ObjNative = new ulong[n];
            ObjGfx = new ulong[n];
            ObjManaged = new long[n];
            ObjShell = new int[n];
            Array.Fill(ObjShell, -1);
            var gfx = s.GfxByRootId();
            for (var i = 0; i < n; i++)
            {
                var root = s.ObjRootId[i];
                ObjNative[i] = s.RootIndexById.TryGetValue(root, out var r) ? s.RootSize[r] : s.ObjSize[i];
                ObjGfx[i] = gfx.TryGetValue(root, out var g) ? g : 0;
                NativeInObjects += ObjNative[i];
                GfxInObjects += ObjGfx[i];
            }
            if (C != null)
            {
                for (var o = 0; o < C.Count; o++)
                {
                    var no = C.NativeObject[o];
                    if (no < 0)
                        continue;
                    ObjShell[no] = o;
                    ObjManaged[no] = C.Size[o];
                }
            }
            foreach (var a in s.AllocSize)
                NativeTotal += a;
            foreach (var g in s.GfxSize)
                GfxTotal += g;
            foreach (var h in s.Heap)
            {
                if (h.IsVm)
                    VmReserved += (ulong)h.Bytes.Length;
                else
                    HeapReserved += (ulong)h.Bytes.Length;
            }
        }

        Dictionary<long, ulong> m_AllocatedByRoot;

        /// <summary>Native allocation bytes per root id.</summary>
        public Dictionary<long, ulong> AllocatedByRoot()
        {
            if (m_AllocatedByRoot != null)
                return m_AllocatedByRoot;
            var map = new Dictionary<long, ulong>();
            for (var a = 0; a < S.AllocSize.Length; a++)
            {
                map.TryGetValue(S.AllocRootId[a], out var v);
                map[S.AllocRootId[a]] = v + S.AllocSize[a];
            }
            return m_AllocatedByRoot = map;
        }

        /// <summary>Native allocation bytes whose root id names no root.</summary>
        public ulong UnrootedNative
        {
            get
            {
                ulong total = 0;
                foreach (var kv in AllocatedByRoot())
                    if (!S.RootIndexById.ContainsKey(kv.Key))
                        total += kv.Value;
                return total;
            }
        }

        public string TypeName(int t) => t >= 0 && t < S.TypeName.Length ? S.TypeName[t] : "?";

        /// <summary>Managed objects deriving from UnityEngine.Object whose native object is gone: kept alive by managed references only.</summary>
        public List<int> DestroyedShells()
        {
            var result = new List<int>();
            if (C == null)
                return result;
            var unityObject = Array.IndexOf(S.TypeName, "UnityEngine.Object");
            if (unityObject < 0)
                return result;
            var cachedPtr = S.TypeFields[unityObject].FirstOrDefault(f => S.FieldName[f] == "m_CachedPtr" && !S.FieldStatic[f], -1);
            if (cachedPtr < 0)
                return result;
            var offset = (ulong)(S.FieldOffset[cachedPtr]);
            var derives = new Dictionary<int, bool>();
            for (var o = 0; o < C.Count; o++)
            {
                var t = C.Type[o];
                if (!derives.TryGetValue(t, out var d))
                {
                    d = false;
                    for (var x = t; x >= 0 && !S.IsArray(x); x = S.TypeBaseOrElement[x])
                        if (x == unityObject)
                        {
                            d = true;
                            break;
                        }
                    derives[t] = d;
                }
                if (d && C.NativeObject[o] < 0 && !C.TryPtr(C.Address[o] + offset, out _))
                    result.Add(o);
            }
            return result;
        }
    }
}
