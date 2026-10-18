using System.Collections.Generic;

namespace BurstWaitress.Asm
{
    public enum Isa
    {
        X86,
        Arm64,
        Arm32,
        Wasm,
    }

    /// <summary>What an instruction does, for the census. Loads and stores are counted separately, so a packed multiply
    /// with a memory operand is both packed float and a load.</summary>
    public enum Cls
    {
        PackedFloat,
        ScalarFloat,
        PackedInt,
        ScalarInt,
        Move,
        Branch,
        Call,
        Ret,
        Other,
    }

    public sealed class Instr
    {
        /// <summary>1-based line in the raw dump.</summary>
        public int Line;
        public string Mnemonic;
        public string[] Operands;
        public string Text;
        public int SrcFile;
        public int SrcLine;
        public int Block = -1;
        public Cls Class;
        public bool Load;
        public bool Store;
        /// <summary>Vector register width in bits for packed instructions, 0 otherwise.</summary>
        public int Width;
        /// <summary>Lane size in bits for packed instructions, when the mnemonic says.</summary>
        public int Lane;
        public bool Fma;
        /// <summary>SVE z register: Width is the 128-bit minimum vector length.</summary>
        public bool Scalable;
        public bool Prologue;
        public bool Epilogue;
        /// <summary>The stack slot a memory operand addresses ("[rsp + 40]"), when it is one.</summary>
        public string StackSlot;
        public string Target;
        public bool Conditional;
        public bool TailCall;
        public bool ZeroIdiom;
    }

    public sealed class Block
    {
        public int Index;
        public string Label;
        public int First;
        public int Last;
        public readonly List<int> Succ = new List<int>();
        public readonly List<int> Pred = new List<int>();
    }

    public enum Shape
    {
        Scalar,
        /// <summary>Packed math across source iterations.</summary>
        Vector,
        /// <summary>Packed math within one source iteration (a float4, a pair of accumulators).</summary>
        Slp,
        /// <summary>Packed math, but the step is a pointer's bytes, so whether it spans iterations isn't known.</summary>
        Packed,
    }

    public sealed class Loop
    {
        public int Id;
        public int Header;
        public readonly HashSet<int> Blocks = new HashSet<int>();
        public int Depth;
        public bool Innermost;
        public Loop Parent;
        public LoopStats Stats;
        public bool AliasGuard;
        public int AliasGuardInstrs;
        /// <summary>The instruction that shows the alias check, with why ("1787 cmp r8, r10: pointer against pointer").</summary>
        public string AliasEvidence;
        public int AliasFirstLine;
        public int AliasLastLine;
        /// <summary>The smallest value of the guarded count (usually the C# loop bound) for which this loop runs, from the
        /// compare guards dominating it; 0 when no guard says. For a loop starting at i = 1 it is one more than the
        /// iterations.</summary>
        public int MinCount;
        /// <summary>MinCount is at a 128-bit SVE vector length and scales with it.</summary>
        public bool MinCountScalable;
        /// <summary>A constant added to a scalable MinCount (VL/16 + 1 for a loop from i = 1).</summary>
        public int MinCountExtra;
        /// <summary>A floor on a scalable MinCount: max(VL/16, 16).</summary>
        public int MinCountFloor;
        /// <summary>Vector-shaped but with none of the loop vectorizer's marks: packed by hand.</summary>
        public bool HandPacked;
        public int Unroll = 1;
        /// <summary>What one pass advances: source iterations for a counter or a scaled index, bytes for a pointer
        /// (PerIterBytes); 0 when unknown. At a 128-bit vector length when PerIterScalable.</summary>
        public int ElemsPerIter;
        public bool PerIterBytes;
        public bool PerIterScalable;
        /// <summary>A stream the counter addresses touches less than the stride each pass: a struct walked one element
        /// at a time.</summary>
        public bool Strided;
        /// <summary>The most full-width vector accesses at distinct offsets in one stream per pass; 0 when none.</summary>
        public int StreamVectors;
        /// <summary>Packed instructions in this loop outside its inner loops.</summary>
        public int OwnPacked;
        public Shape Shape;
        /// <summary>A scalar loop after a vector loop in the same function (or before one with an alias check, whose
        /// fallback it also is): the leftover iterations.</summary>
        public bool Remainder;
        public bool Main;
        public Function Owner;
    }

    public sealed class LoopStats
    {
        public int Instrs;
        public int PackedFloat;
        public int ScalarFloat;
        public int PackedInt;
        public int ScalarInt;
        public int Moves;
        public int Loads;
        public int Stores;
        public int Branches;
        public int Calls;
        public int Fma;
        public int Width;
        public int Lane;
        public int PackedWidth;
        public int MoveWidth;
        public int Spills;
        public int Reloads;
        public int FirstLine;
        public int LastLine;
        /// <summary>Calls to vector math routines (Sleef_sinf4), which do packed work out of line.</summary>
        public int VectorMathCalls;
        public int ScalarMathCalls;
        public int CallWidth;
        public int CallLane;
        /// <summary>SVE fadda: a reduction over the lanes in order, one add after another.</summary>
        public int OrderedReductions;
        /// <summary>Loads of one element into one vector lane: an emulated gather.</summary>
        public int LaneLoads;
        public int VectorLoads;
        public int VectorStores;
        /// <summary>Only vector moves (a vectorized copy): PackedWidth is the move width, with no math lanes.</summary>
        public bool Copy;
        /// <summary>Atomic read-modify-write instructions (lock cmpxchg, ldaxr/stlxr): a retry loop, not work.</summary>
        public int Atomics;
        public int Gathers;
        /// <summary>SVE: PackedWidth is the 128-bit minimum of a vector length known only at run time.</summary>
        public bool Scalable;

        /// <summary>Packed float work over all float work, with math library calls counted on their side.</summary>
        public double PackedRatio
        {
            get
            {
                var packed = PackedFloat + VectorMathCalls;
                var all = packed + ScalarFloat + ScalarMathCalls;
                return all == 0 ? -1 : (double)packed / all;
            }
        }
    }

    public sealed class Function
    {
        public string Name;
        public string Display;
        public int StartLine;
        public int EndLine;
        public readonly List<Instr> Instrs = new List<Instr>();
        public readonly List<Block> Blocks = new List<Block>();
        public readonly List<Loop> Loops = new List<Loop>();
        public LoopStats Stats;
        public bool Exported;
        public bool Runtime;
        /// <summary>Main-loop ranking group, see Analyzer.Tier.</summary>
        public int Tier;
        internal System.Collections.BitArray[] Dom;
        internal int PrologueEnd = -1;
        internal bool SehEpilogue;
    }

    public sealed class Call
    {
        public string Name;
        public string Display;
        public string Kind;
        public int Count;
        /// <summary>Sites not in a remainder loop: a scalar math call there is a vectorization miss, not leftovers.</summary>
        public int OutsideRemainder;
        public bool InLoop;
        public readonly SortedSet<int> Loops = new SortedSet<int>();
        /// <summary>Short names of the functions the calls are made from.</summary>
        public readonly SortedSet<string> From = new SortedSet<string>(System.StringComparer.Ordinal);
    }

    public sealed class AsmFile
    {
        public Isa Isa;
        public string[] Lines;
        public readonly List<Function> Functions = new List<Function>();
        public readonly Dictionary<int, string> Files = new Dictionary<int, string>();
        /// <summary>The function the entry point's code is in: the exported entry, or what a thin entry stub calls.</summary>
        public Function Body;
        public Function Entry;
        public readonly List<Call> Calls = new List<Call>();
        /// <summary>The body and the C# functions it reaches that Burst didn't inline: where the method's own code is.
        /// Library routines (Sleef) are reached but not part of it.</summary>
        public readonly List<Function> Focus = new List<Function>();
        /// <summary>The innermost loop with the widest packed math across the focus functions.</summary>
        public Loop Main;
    }
}
