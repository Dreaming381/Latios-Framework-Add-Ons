
	.def	"@feat.00";
	.scl	3;
	.type	0;
	.endef
	.globl	"@feat.00"
"@feat.00" = 0
	.file	"main"
	.def	b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	b639652c4fdbfa25bf83f596085599e0
	.p2align	2
b639652c4fdbfa25bf83f596085599e0:
.seh_proc b639652c4fdbfa25bf83f596085599e0
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	mov	x1, x2
	mov	x2, x3
	mov	w3, w4
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	b	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize
	.p2align	2
burst.initialize:
.seh_proc burst.initialize
	sub	sp, sp, #48
	.seh_stackalloc	48
	stp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	stp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	x29, sp, #32
	.seh_add_fp	32
	.seh_endprologue
	mov	x20, x0
	adrp	x0, burst.string.808e8e7f94567b9f878889686ba14eb1
	add	x0, x0, :lo12:burst.string.808e8e7f94567b9f878889686ba14eb1
	blr	x20
	adrp	x8, burst_abort_Ptr
	str	x0, [x8, :lo12:burst_abort_Ptr]
	adrp	x0, burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	add	x0, x0, :lo12:burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	blr	x20
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	adrp	x0, burst.string.76cb701fb3c87996598b3e8df4d3255d
	add	x0, x0, :lo12:burst.string.76cb701fb3c87996598b3e8df4d3255d
	blr	x20
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	adrp	x0, burst.string.57a01b00185c83d1eb43aeca89f29647
	add	x0, x0, :lo12:burst.string.57a01b00185c83d1eb43aeca89f29647
	blr	x20
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	mov	x19, x0
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	adrp	x0, burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610
	add	x0, x0, :lo12:burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610
	blr	x20
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"]
	adrp	x0, burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	add	x0, x0, :lo12:burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	blr	x20
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	adrp	x0, burst.string.ffbfa8f9468181ee01437772260f8c26
	add	x0, x0, :lo12:burst.string.ffbfa8f9468181ee01437772260f8c26
	blr	x20
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	adrp	x0, burst.string.e032451e108c081916a72836299e732a
	add	x0, x0, :lo12:burst.string.e032451e108c081916a72836299e732a
	blr	x20
	mov	x8, #40816
	mov	x9, #24720
	mov	w1, #8
	movk	x8, #53833, lsl #16
	movk	x9, #11702, lsl #16
	mov	w2, #16
	movk	x8, #45524, lsl #32
	movk	x9, #20011, lsl #32
	movk	x8, #8176, lsl #48
	movk	x9, #57359, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #64595
	mov	x9, #941
	mov	w1, #8
	movk	x8, #4200, lsl #16
	movk	x9, #61335, lsl #16
	mov	w2, #16
	movk	x8, #30789, lsl #32
	movk	x9, #34746, lsl #32
	movk	x8, #43273, lsl #48
	movk	x9, #22262, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #55538
	mov	x9, #9998
	mov	w1, #2048
	movk	x8, #15309, lsl #16
	movk	x9, #50226, lsl #16
	movk	w1, #4, lsl #16
	movk	x8, #709, lsl #32
	movk	x9, #64826, lsl #32
	mov	w2, #16
	movk	x8, #7437, lsl #48
	movk	x9, #58098, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"
	mov	w9, #1073741824
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"]
	adrp	x8, "System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"
	mov	x0, sp
	str	x9, [x8, :lo12:"System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"]
	str	q0, [sp]
	blr	x19
	mov	x8, #63549
	mov	x9, #1987
	mov	w1, #196608
	movk	x8, #52383, lsl #16
	movk	x9, #13152, lsl #16
	mov	w2, #16
	movk	x8, #38052, lsl #32
	movk	x9, #27483, lsl #32
	movk	x8, #61756, lsl #48
	movk	x9, #3779, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #18431
	mov	x9, #47105
	mov	w1, #1
	movk	x8, #25345, lsl #16
	movk	x9, #40190, lsl #16
	mov	w2, #16
	movk	x8, #1973, lsl #32
	movk	x9, #63562, lsl #32
	movk	x8, #9630, lsl #48
	movk	x9, #55905, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #52204
	mov	x9, #13332
	mov	w1, #96
	movk	x8, #21645, lsl #16
	movk	x9, #43890, lsl #16
	mov	w2, #16
	movk	x8, #44954, lsl #32
	movk	x9, #20581, lsl #32
	movk	x8, #57320, lsl #48
	movk	x9, #8215, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #53434
	mov	x9, #12102
	mov	w1, #4
	movk	x8, #65506, lsl #16
	movk	x9, #29, lsl #16
	mov	w2, #16
	movk	x8, #31003, lsl #32
	movk	x9, #34532, lsl #32
	movk	x8, #21823, lsl #48
	movk	x9, #43712, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #38699
	mov	x9, #10982
	mov	w1, #8
	movk	x8, #40022, lsl #16
	movk	x9, #25139, lsl #16
	mov	w2, #16
	movk	x8, #10097, lsl #32
	movk	x9, #50601, lsl #32
	movk	x8, #64511, lsl #48
	movk	x9, #40374, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #9051
	mov	x9, #56485
	mov	w1, #4
	movk	x8, #59930, lsl #16
	movk	x9, #5605, lsl #16
	mov	w2, #16
	movk	x8, #50620, lsl #32
	movk	x9, #14915, lsl #32
	movk	x8, #19086, lsl #48
	movk	x9, #46449, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #28797
	mov	x9, #36739
	mov	w1, #524288
	movk	x8, #55748, lsl #16
	movk	x9, #9787, lsl #16
	mov	w2, #16
	movk	x8, #13428, lsl #32
	movk	x9, #52107, lsl #32
	movk	x8, #4611, lsl #48
	movk	x9, #60924, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #2053
	mov	x9, #63483
	mov	w1, #65536
	movk	x8, #63346, lsl #16
	movk	x9, #2189, lsl #16
	mov	w2, #16
	movk	x8, #39377, lsl #32
	movk	x9, #26158, lsl #32
	movk	x8, #463, lsl #48
	movk	x9, #65072, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	ldp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0
	.p2align	2
burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0:
.seh_proc burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0
	str	x19, [sp, #-32]!
	.seh_save_reg_x	x19, 32
	stp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	add	x29, sp, #8
	.seh_add_fp	8
	.seh_endprologue
	mov	x19, x0
	adrp	x0, burst.string.808e8e7f94567b9f878889686ba14eb1
	add	x0, x0, :lo12:burst.string.808e8e7f94567b9f878889686ba14eb1
	blr	x19
	adrp	x8, burst_abort_Ptr
	str	x0, [x8, :lo12:burst_abort_Ptr]
	adrp	x0, burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	add	x0, x0, :lo12:burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	blr	x19
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	adrp	x0, burst.string.76cb701fb3c87996598b3e8df4d3255d
	add	x0, x0, :lo12:burst.string.76cb701fb3c87996598b3e8df4d3255d
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	adrp	x0, burst.string.57a01b00185c83d1eb43aeca89f29647
	add	x0, x0, :lo12:burst.string.57a01b00185c83d1eb43aeca89f29647
	blr	x19
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	adrp	x0, burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610
	add	x0, x0, :lo12:burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"]
	adrp	x0, burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	add	x0, x0, :lo12:burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	adrp	x0, burst.string.ffbfa8f9468181ee01437772260f8c26
	add	x0, x0, :lo12:burst.string.ffbfa8f9468181ee01437772260f8c26
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	adrp	x0, burst.string.e032451e108c081916a72836299e732a
	add	x0, x0, :lo12:burst.string.e032451e108c081916a72836299e732a
	blr	x19
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	ldr	x19, [sp], #32
	.seh_save_reg_x	x19, 32
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0
	.p2align	2
burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0:
.seh_proc burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0
	sub	sp, sp, #48
	.seh_stackalloc	48
	str	x19, [sp, #16]
	.seh_save_reg	x19, 16
	stp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	add	x29, sp, #24
	.seh_add_fp	24
	.seh_endprologue
	mov	x8, #40816
	mov	x9, #24720
	mov	x0, sp
	movk	x8, #53833, lsl #16
	movk	x9, #11702, lsl #16
	mov	w1, #8
	movk	x8, #45524, lsl #32
	movk	x9, #20011, lsl #32
	mov	w2, #16
	movk	x8, #8176, lsl #48
	movk	x9, #57359, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	ldr	x19, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	str	q0, [sp]
	blr	x19
	mov	x8, #64595
	mov	x9, #941
	mov	w1, #8
	movk	x8, #4200, lsl #16
	movk	x9, #61335, lsl #16
	mov	w2, #16
	movk	x8, #30789, lsl #32
	movk	x9, #34746, lsl #32
	movk	x8, #43273, lsl #48
	movk	x9, #22262, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #55538
	mov	x9, #9998
	mov	w1, #2048
	movk	x8, #15309, lsl #16
	movk	x9, #50226, lsl #16
	movk	w1, #4, lsl #16
	movk	x8, #709, lsl #32
	movk	x9, #64826, lsl #32
	mov	w2, #16
	movk	x8, #7437, lsl #48
	movk	x9, #58098, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"
	mov	w9, #1073741824
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"]
	adrp	x8, "System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"
	mov	x0, sp
	str	x9, [x8, :lo12:"System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"]
	str	q0, [sp]
	blr	x19
	mov	x8, #63549
	mov	x9, #1987
	mov	w1, #196608
	movk	x8, #52383, lsl #16
	movk	x9, #13152, lsl #16
	mov	w2, #16
	movk	x8, #38052, lsl #32
	movk	x9, #27483, lsl #32
	movk	x8, #61756, lsl #48
	movk	x9, #3779, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #18431
	mov	x9, #47105
	mov	w1, #1
	movk	x8, #25345, lsl #16
	movk	x9, #40190, lsl #16
	mov	w2, #16
	movk	x8, #1973, lsl #32
	movk	x9, #63562, lsl #32
	movk	x8, #9630, lsl #48
	movk	x9, #55905, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #52204
	mov	x9, #13332
	mov	w1, #96
	movk	x8, #21645, lsl #16
	movk	x9, #43890, lsl #16
	mov	w2, #16
	movk	x8, #44954, lsl #32
	movk	x9, #20581, lsl #32
	movk	x8, #57320, lsl #48
	movk	x9, #8215, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #53434
	mov	x9, #12102
	mov	w1, #4
	movk	x8, #65506, lsl #16
	movk	x9, #29, lsl #16
	mov	w2, #16
	movk	x8, #31003, lsl #32
	movk	x9, #34532, lsl #32
	movk	x8, #21823, lsl #48
	movk	x9, #43712, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #38699
	mov	x9, #10982
	mov	w1, #8
	movk	x8, #40022, lsl #16
	movk	x9, #25139, lsl #16
	mov	w2, #16
	movk	x8, #10097, lsl #32
	movk	x9, #50601, lsl #32
	movk	x8, #64511, lsl #48
	movk	x9, #40374, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #9051
	mov	x9, #56485
	mov	w1, #4
	movk	x8, #59930, lsl #16
	movk	x9, #5605, lsl #16
	mov	w2, #16
	movk	x8, #50620, lsl #32
	movk	x9, #14915, lsl #32
	movk	x8, #19086, lsl #48
	movk	x9, #46449, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #28797
	mov	x9, #36739
	mov	w1, #524288
	movk	x8, #55748, lsl #16
	movk	x9, #9787, lsl #16
	mov	w2, #16
	movk	x8, #13428, lsl #32
	movk	x9, #52107, lsl #32
	movk	x8, #4611, lsl #48
	movk	x9, #60924, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #2053
	mov	x9, #63483
	mov	w1, #65536
	movk	x8, #63346, lsl #16
	movk	x9, #2189, lsl #16
	mov	w2, #16
	movk	x8, #39377, lsl #32
	movk	x9, #26158, lsl #32
	movk	x8, #463, lsl #48
	movk	x9, #65072, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	ldr	x19, [sp, #16]
	.seh_save_reg	x19, 16
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	movi	v1.2d, #0000000000000000
	movi	v2.2d, #0000000000000000
	mov	w8, #-1
	stp	q1, q1, [x0]
	stp	q1, q1, [x0, #32]
	stp	q1, q1, [x0, #64]
	stp	q1, q1, [x0, #96]
	str	d2, [x0, #128]
	str	w8, [x0, #128]
	cbz	w1, .LBB4_9
	mov	x8, #-1
	fmov	x10, d0
	cmp	w2, #63
	lsl	x9, x8, x2
	csinv	x8, x8, x9, gt
	mov	w9, wzr
	ands	x10, x8, x10
	b.eq	.LBB4_4
	mov	w11, wzr
	add	x8, x0, #64
.LBB4_3:
	rbit	x12, x10
	clz	x12, x12
	lsr	x10, x10, x12
	add	w11, w11, w12
	strb	w11, [x0, w9, sxtw]
	ldrsw	x9, [x0, #132]
	mvn	x13, x10
	rbit	x13, x13
	clz	x12, x13
	strb	w12, [x8, x9]
	sub	w9, w12, #1
	add	w11, w11, w12
	lsr	x13, x10, x9
	ldr	w9, [x0, #132]
	lsr	x10, x13, #1
	add	w9, w9, #1
	cmp	x13, #1
	str	w9, [x0, #132]
	b.hi	.LBB4_3
.LBB4_4:
	cmp	w2, #64
	b.lt	.LBB4_14
	mov	x8, v0.d[1]
	mov	x10, #-1
	cmp	w2, #127
	lsl	x11, x10, x2
	csinv	x10, x10, x11, hi
	and	x8, x10, x8
	cbz	w9, .LBB4_10
	sxtw	x9, w9
	add	x11, x0, #64
	sub	x10, x9, #1
	ldrb	w9, [x0, x10]
	ldrb	w12, [x11, x10]
	add	w9, w12, w9
	cmp	w9, #64
	mov	w9, #64
	b.ne	.LBB4_11
	tbz	w8, #0, .LBB4_11
	mvn	x9, x8
	rbit	x9, x9
	clz	x9, x9
	sub	w13, w9, #1
	add	w12, w12, w9
	strb	w12, [x11, x10]
	lsr	x8, x8, x13
	add	w9, w9, #64
	lsr	x8, x8, #1
	b	.LBB4_11
.LBB4_9:
	mov	w8, #1
	strb	w2, [x0, #64]
	str	w8, [x0, #132]
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
.LBB4_10:
	mov	w9, #64
.LBB4_11:
	cbz	x8, .LBB4_14
	ldr	w11, [x0, #132]
	add	x10, x0, #64
.LBB4_13:
	rbit	x12, x8
	clz	x12, x12
	lsr	x8, x8, x12
	add	w9, w9, w12
	strb	w9, [x0, w11, sxtw]
	ldrsw	x11, [x0, #132]
	mvn	x13, x8
	rbit	x13, x13
	clz	x12, x13
	strb	w12, [x10, x11]
	sub	w11, w12, #1
	add	w9, w9, w12
	lsr	x13, x8, x11
	ldr	w11, [x0, #132]
	lsr	x8, x13, #1
	add	w11, w11, #1
	cmp	x13, #2
	str	w11, [x0, #132]
	b.hs	.LBB4_13
.LBB4_14:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #240
	.seh_stackalloc	240
	stp	x19, x20, [sp, #144]
	.seh_save_regp	x19, 144
	stp	x21, x22, [sp, #160]
	.seh_save_regp	x21, 160
	stp	x23, x24, [sp, #176]
	.seh_save_regp	x23, 176
	stp	x25, x26, [sp, #192]
	.seh_save_regp	x25, 192
	stp	x27, x28, [sp, #208]
	.seh_save_regp	x27, 208
	stp	x29, x30, [sp, #224]
	.seh_save_fplr	224
	add	x29, sp, #224
	.seh_add_fp	224
	.seh_endprologue
	ldr	x8, [x6]
	mov	x20, x5
	mov	x25, x3
	mov	x19, x2
	mov	x22, x1
	mov	x21, x0
	cbz	x8, .LBB5_2
	ldp	w23, w24, [x8]
	orr	w8, w24, w23
	cbz	w8, .LBB5_3
	b	.LBB5_5
.LBB5_2:
	mov	w23, wzr
	mov	w24, wzr
	orr	w8, w24, wzr
	cbnz	w8, .LBB5_5
.LBB5_3:
	ldr	x8, [x7]
	cbz	x8, .LBB5_5
	ldp	w23, w24, [x8]
.LBB5_5:
	movi	v0.2d, #0000000000000000
	mov	w8, w23
	ldr	x26, [x4]
	orr	x1, x8, x24, lsl #32
	add	x2, sp, #48
	sub	x3, x29, #96
	mov	x0, x21
	sturb	wzr, [x29, #-96]
	str	q0, [sp, #48]
	bl	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	tbz	w0, #0, .LBB5_37
	ldr	x2, [sp, #56]
	cbz	x2, .LBB5_37
	ldr	x8, [x22, #8]
	ldur	x9, [x2, #-16]
	ldr	x3, [x25]
	cmp	x8, x9
	b.ne	.LBB5_10
	ldr	x8, [x22, #16]
	cbz	x3, .LBB5_16
	ldur	x9, [x3, #-16]
	cmp	x8, x9
	b.eq	.LBB5_11
.LBB5_10:
	mov	x0, x22
	mov	x1, x19
	bl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB5_11:
	ldrb	w8, [x22, #24]
	cbz	w8, .LBB5_37
	add	x0, x21, #56
	add	x8, sp, #64
	mov	w1, w23
	mov	w2, w24
	bl	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	w12, w24, [x20, #40]
	ldr	x21, [sp, #64]
	ldr	x22, [x20]
	adrp	x23, "System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"
	add	x23, x23, :lo12:"System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"
	ldrb	w25, [x26]
	ldr	x27, [x20, #32]
	tbnz	w25, #5, .LBB5_15
	add	x14, x22, #16
	str	x14, [sp, #40]
	tbnz	w25, #2, .LBB5_17
	and	x8, x25, #0x3
	ldr	x9, [x22]
	stur	x27, [x29, #-96]
	ldr	w8, [x23, x8, lsl #2]
	stur	w12, [x29, #-88]
	cmp	x9, #0
	str	w8, [sp, #12]
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"
	ldr	x28, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	csel	x8, x14, x9, eq
	b	.LBB5_18
.LBB5_15:
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"
	ldr	x28, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	b	.LBB5_30
.LBB5_16:
	cmp	x8, xzr
	b.ne	.LBB5_10
	b	.LBB5_11
.LBB5_17:
	and	x8, x25, #0x7
	adrp	x9, "System.Int32[] Latios.Kinemation.BlendShapeState::PreviousFromMask_$__data_c8ad709e5ee7937b"
	add	x9, x9, :lo12:"System.Int32[] Latios.Kinemation.BlendShapeState::PreviousFromMask_$__data_c8ad709e5ee7937b"
	ldr	w8, [x9, x8, lsl #2]
	ldr	x10, [x22]
	stur	x27, [x29, #-96]
	stur	w12, [x29, #-88]
	str	w8, [sp, #12]
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"
	cmp	x10, #0
	ldr	x28, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	csel	x8, x14, x10, eq
.LBB5_18:
	sub	x0, x29, #96
	str	x8, [sp, #16]
	mov	w20, w12
	stur	w24, [x29, #-84]
	blr	x28
	ldrb	w8, [x26]
	ldrsw	x10, [x22, #8]
	sub	x0, x29, #96
	str	x27, [sp, #24]
	and	x9, x8, #0x7
	orr	w8, w8, #0x20
	str	x10, [sp]
	ldr	w23, [x23, x9, lsl #2]
	strb	w8, [x26]
	ldr	x9, [sp, #40]
	ldr	x8, [x22]
	stur	x27, [x29, #-96]
	stp	w20, w24, [sp, #36]
	cmp	x8, #0
	stp	w20, w24, [x29, #-88]
	csel	x20, x9, x8, eq
	blr	x28
	tbz	w25, #2, .LBB5_29
	ldrsw	x8, [x22, #8]
	add	w9, w8, #2
	cmp	w9, #5
	b.lo	.LBB5_29
	mov	w9, #21846
	ldr	x10, [sp]
	cnth	x14
	movk	w9, #21845, lsl #16
	cmp	x14, #12
	smull	x10, w10, w9
	smull	x8, w8, w9
	lsr	x9, x10, #63
	lsr	x10, x10, #32
	lsr	x11, x8, #63
	lsr	x8, x8, #32
	add	w9, w10, w9
	ldr	w10, [sp, #12]
	add	w8, w8, w11
	mov	w11, #12
	mul	w9, w9, w10
	csel	x11, x14, x11, hi
	mul	w10, w8, w23
	cmp	x11, x8
	sbfiz	x9, x9, #2, #32
	sbfiz	x10, x10, #2, #32
	b.ls	.LBB5_22
	ldr	x2, [sp, #16]
	mov	x11, xzr
	b	.LBB5_27
.LBB5_22:
	ldr	x2, [sp, #16]
	rdvl	x11, #1
	lsr	x15, x11, #4
	add	x11, x10, x20
	add	x12, x2, x9
	sub	x11, x11, x12
	cmp	x11, x15, lsl #5
	b.hs	.LBB5_24
	mov	x11, xzr
	b	.LBB5_27
.LBB5_24:
	udiv	x16, x8, x14
	lsl	x15, x15, #2
	ptrue	p0.s
	mov	x13, xzr
	add	x17, x20, x10
	mul	x11, x16, x14
	mneg	x14, x16, x14
	sub	x16, x8, x11
.LBB5_25:
	ld1w	{ z0.s }, p0/z, [x12, x13, lsl #2]
	ld1w	{ z1.s }, p0/z, [x12, x15, lsl #2]
	st1w	{ z0.s }, p0, [x17, x13, lsl #2]
	inch	x13
	st1w	{ z1.s }, p0, [x17, x15, lsl #2]
	inch	x15
	add	x0, x14, x15
	decw	x0
	cbnz	x0, .LBB5_25
	cbz	x16, .LBB5_29
.LBB5_27:
	lsl	x12, x11, #2
	add	x10, x20, x10
	add	x13, x2, x9
	sub	x8, x8, x11
	add	x9, x10, x12
	add	x10, x13, x12
.LBB5_28:
	ldr	s0, [x10], #4
	subs	x8, x8, #1
	str	s0, [x9], #4
	b.ne	.LBB5_28
.LBB5_29:
	ldp	w12, w24, [sp, #36]
	ldr	x27, [sp, #24]
	adrp	x23, "System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"
	add	x23, x23, :lo12:"System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"
.LBB5_30:
	ldrb	w8, [x26]
	sub	x0, x29, #96
	and	x9, x8, #0x7
	orr	w8, w8, #0x20
	ldr	w20, [x23, x9, lsl #2]
	strb	w8, [x26]
	add	x9, x22, #16
	ldr	x8, [x22]
	stur	x27, [x29, #-96]
	stp	w12, w24, [x29, #-88]
	cmp	x8, #0
	csel	x23, x9, x8, eq
	blr	x28
	ldr	x8, [x19]
	ldr	w9, [x8, #8]
	cmp	w9, #1
	b.lt	.LBB5_37
	ldrsw	x10, [x22, #8]
	sub	w9, w10, #3
	cmn	w9, #6
	b.hi	.LBB5_37
	mov	w11, #21846
	mov	x9, xzr
	add	x12, x21, #16
	movk	w11, #21845, lsl #16
	smull	x10, w10, w11
	lsr	x11, x10, #63
	lsr	x10, x10, #32
	add	w10, w10, w11
	mul	w11, w10, w20
	add	x11, x23, w11, sxtw #2
.LBB5_33:
	mov	x13, x8
	ldr	x14, [x13], #16
	cmp	x14, #0
	csel	x13, x13, x14, eq
	ldrsh	x13, [x13, x9, lsl #1]
	tbnz	x13, #63, .LBB5_35
	ldr	x8, [x21]
	cmp	x8, #0
	csel	x8, x12, x8, eq
	ldr	s0, [x8, x13, lsl #2]
	str	s0, [x11, x9, lsl #2]
	ldr	x8, [x19]
.LBB5_35:
	ldrsw	x13, [x8, #8]
	add	x9, x9, #1
	cmp	x9, x13
	b.ge	.LBB5_37
	cmp	x9, x10
	b.lo	.LBB5_33
.LBB5_37:
	.seh_startepilogue
	ldp	x29, x30, [sp, #224]
	.seh_save_fplr	224
	ldp	x27, x28, [sp, #208]
	.seh_save_regp	x27, 208
	ldp	x25, x26, [sp, #192]
	.seh_save_regp	x25, 192
	ldp	x23, x24, [sp, #176]
	.seh_save_regp	x23, 176
	ldp	x21, x22, [sp, #160]
	.seh_save_regp	x21, 160
	ldp	x19, x20, [sp, #144]
	.seh_save_regp	x19, 144
	add	sp, sp, #240
	.seh_stackalloc	240
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #128
	.seh_stackalloc	128
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	stp	x23, x24, [sp, #64]
	.seh_save_regp	x23, 64
	stp	x25, x26, [sp, #80]
	.seh_save_regp	x25, 80
	stp	x27, x28, [sp, #96]
	.seh_save_regp	x27, 96
	stp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	add	x29, sp, #112
	.seh_add_fp	112
	.seh_endprologue
	cbz	x2, .LBB6_6
	ldur	x8, [x2, #-16]
	str	x8, [x0, #8]
	cbz	x3, .LBB6_7
.LBB6_2:
	ldur	x8, [x3, #-16]
	str	x8, [x0, #16]
	strb	wzr, [x0, #24]
	ldrh	w8, [x3, #188]
	cbz	x8, .LBB6_8
.LBB6_3:
	add	x9, x8, x3
	add	x8, x8, #1
	mov	w23, #5381
	add	x9, x9, #189
.LBB6_4:
	ldrb	w10, [x9], #-1
	add	w11, w23, w23, lsl #5
	sub	x8, x8, #1
	cmp	x8, #1
	add	w23, w11, w10
	b.hi	.LBB6_4
	ldr	x24, [x1]
	str	wzr, [x24, #8]
	ldr	w8, [x24, #12]
	ldr	w25, [x3, #68]
	cmp	w25, w8
	b.gt	.LBB6_9
	b	.LBB6_17
.LBB6_6:
	str	xzr, [x0, #8]
	cbnz	x3, .LBB6_2
.LBB6_7:
	str	xzr, [x0, #16]
	strb	wzr, [x0, #24]
	ldrh	w8, [x3, #188]
	cbnz	x8, .LBB6_3
.LBB6_8:
	mov	w23, #5381
	ldr	x24, [x1]
	str	wzr, [x24, #8]
	ldr	w8, [x24, #12]
	ldr	w25, [x3, #68]
	cmp	w25, w8
	b.le	.LBB6_17
.LBB6_9:
	lsl	w9, w8, #1
	mov	w10, #8
	cmp	w9, w25
	csel	w9, w9, w25, gt
	cmp	w9, #8
	csel	w28, w9, w10, gt
	cmp	w28, w8
	b.eq	.LBB6_17
	ldrb	w9, [x1, #58]
	mov	x26, x24
	lsl	w22, w28, #1
	ldr	x8, [x26], #16
	stp	x2, x0, [sp, #16]
	str	w9, [sp, #4]
	adrp	x9, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	ldrb	w21, [x1, #57]
	ldr	x9, [x9, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	str	x3, [sp, #8]
	mov	x27, x1
	cmp	x8, #0
	mov	x0, x22
	mov	w1, #64
	mov	w2, #4
	mov	w3, #1
	mov	x4, xzr
	csel	x20, x26, x8, eq
	blr	x9
	mov	x19, x0
	cmp	x20, x0
	b.eq	.LBB6_16
	ldr	w8, [x24, #12]
	cmp	w21, #1
	b.ne	.LBB6_14
	lsl	w9, w8, #1
	sxtw	x9, w9
	sub	x2, x22, x9
	cmp	x2, #1
	b.lt	.LBB6_14
	ldr	w1, [sp, #4]
	add	x0, x19, x9
	mov	w3, wzr
	bl	burst.memset.inline.ARMV9A.i64
	ldr	w8, [x24, #12]
.LBB6_14:
	cmp	w8, w28
	mov	x0, x19
	mov	x1, x20
	csel	w8, w8, w28, lt
	mov	w3, wzr
	sbfiz	x2, x8, #1, #32
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x24]
	cbz	x0, .LBB6_16
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	mov	w1, #4
	ldr	x8, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	blr	x8
.LBB6_16:
	cmp	x19, x26
	str	w28, [x24, #12]
	ldr	x0, [sp, #24]
	csel	x8, xzr, x19, eq
	ldp	x3, x2, [sp, #8]
	str	x8, [x24]
	mov	x1, x27
	ldr	x24, [x27]
.LBB6_17:
	str	w25, [x24, #8]
	ldr	w8, [x3, #68]
	cmp	w8, #1
	b.lt	.LBB6_34
	mov	x8, xzr
	add	x9, x3, #64
	add	x10, x3, #65
	add	x11, x2, #16
	b	.LBB6_21
.LBB6_19:
	mov	w13, #-1
.LBB6_20:
	ldr	x12, [x1]
	add	x10, x10, #128
	ldr	x14, [x12], #16
	cmp	x14, #0
	csel	x12, x12, x14, eq
	strh	w13, [x12, x8, lsl #1]
	mvn	w13, w13
	add	x8, x8, #1
	ldrb	w12, [x0, #24]
	orr	w12, w12, w13, lsr #31
	strb	w12, [x0, #24]
	ldrsw	x12, [x3, #68]
	cmp	x8, x12
	b.ge	.LBB6_34
.LBB6_21:
	ldrsw	x12, [x9]
	lsl	x13, x8, #7
	add	x14, x9, x12
	ldrh	w13, [x14, x13]
	cbz	x13, .LBB6_25
	add	x12, x12, x13
	add	x13, x13, #1
	add	x14, x10, x12
	mov	w12, #5381
.LBB6_23:
	ldrb	w15, [x14], #-1
	add	w12, w12, w12, lsl #5
	sub	x13, x13, #1
	cmp	x13, #1
	add	w12, w12, w15
	b.hi	.LBB6_23
	ldr	w14, [x2, #12]
	cmp	w14, #1
	b.ge	.LBB6_26
	b	.LBB6_19
.LBB6_25:
	mov	w12, #5381
	ldr	w14, [x2, #12]
	cmp	w14, #1
	b.lt	.LBB6_19
.LBB6_26:
	ldrsw	x13, [x2, #8]
	mov	x15, xzr
	mov	w17, #-1
	add	x16, x11, x13
	mov	w13, #-1
	b	.LBB6_29
.LBB6_27:
	cmp	w4, #0
	cset	w4, ne
	cmp	w5, #0
	cinc	w4, w4, ne
	cmp	w4, w17
	csel	w17, w4, w17, gt
	csel	w13, w15, w13, gt
.LBB6_28:
	add	x15, x15, #1
	add	x16, x16, #12
	cmp	x14, x15
	b.eq	.LBB6_20
.LBB6_29:
	ldur	w4, [x16, #-8]
	cmp	w4, w12
	b.ne	.LBB6_28
	ldp	w4, w5, [x16, #-4]
	cbz	w4, .LBB6_32
	cmp	w4, w23
	b.ne	.LBB6_28
.LBB6_32:
	cbz	w5, .LBB6_27
	ldr	w6, [x0]
	cmp	w5, w6
	b.eq	.LBB6_27
	b	.LBB6_28
.LBB6_34:
	.seh_startepilogue
	ldp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	ldp	x27, x28, [sp, #96]
	.seh_save_regp	x27, 96
	ldp	x25, x26, [sp, #80]
	.seh_save_regp	x25, 80
	ldp	x23, x24, [sp, #64]
	.seh_save_regp	x23, 64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #128
	.seh_stackalloc	128
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-160]!
	.seh_save_regp_x	x19, 160
	stp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	stp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	stp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	stp	x27, x28, [sp, #64]
	.seh_save_regp	x27, 64
	stp	x29, x30, [sp, #80]
	.seh_save_fplr	80
	stp	d8, d9, [sp, #96]
	.seh_save_fregp	d8, 96
	stp	d10, d11, [sp, #112]
	.seh_save_fregp	d10, 112
	stp	d12, d13, [sp, #128]
	.seh_save_fregp	d12, 128
	stp	d14, d15, [sp, #144]
	.seh_save_fregp	d14, 144
	add	x29, sp, #80
	.seh_add_fp	80
	.seh_endprologue
	sub	sp, sp, #784
	movi	v0.2d, #0000000000000000
	add	x8, sp, #368
	movi	v1.2d, #0000000000000000
	mov	x22, x1
	mov	x21, x0
	add	x1, x0, #136
	mov	x0, x22
	mov	x23, x4
	mov	w24, w3
	stp	q0, q0, [x8]
	stp	q0, q0, [x8, #32]
	str	q0, [x8, #208]
	add	x8, sp, #345
	stur	q0, [x8, #255]
	add	x8, sp, #361
	stur	q0, [x8, #255]
	add	x8, sp, #377
	stur	q0, [x8, #255]
	add	x8, sp, #393
	stur	q0, [x8, #255]
	add	x8, sp, #600
	stur	q0, [x8, #88]
	add	x8, sp, #9
	stur	q0, [x8, #255]
	add	x8, sp, #25
	stur	q0, [x8, #255]
	add	x8, sp, #41
	stur	q0, [x8, #255]
	add	x8, sp, #57
	stur	q0, [x8, #255]
	add	x8, sp, #73
	stur	q0, [x8, #255]
	add	x8, sp, #89
	str	d1, [sp, #592]
	stur	q0, [x29, #-200]
	stur	d1, [x29, #-184]
	stur	d1, [x29, #-160]
	stur	q0, [sp, #232]
	stur	q0, [sp, #248]
	stur	q0, [x8, #255]
	str	d1, [sp, #360]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldrb	w8, [x21, #224]
	str	x0, [sp, #80]
	add	x1, x21, #200
	mov	x0, x22
	cmp	w8, #1
	sub	x8, x29, #152
	cset	w2, eq
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w8, [x22]
	str	x21, [sp, #200]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	sbfiz	x10, x8, #4, #32
	ldr	x9, [x9]
	ldr	x9, [x9, x10]
	ldr	x10, [x21, #280]
	cmp	x10, x9
	b.ne	.LBB7_16
	ldrh	w10, [x21, #294]
	mov	w9, #65535
	bics	wzr, w9, w10
	b.eq	.LBB7_28
.LBB7_2:
	lsl	x9, x8, #32
	adrp	x10, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	ubfiz	x8, x8, #14, #6
	ldr	x10, [x10, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x11, [x21, #288]
	asr	x9, x9, #35
	add	x8, x8, x11
	and	x9, x9, #0xfffffffffffffff8
	ldr	x9, [x10, x9]
	add	x8, x9, x8
	add	x8, x8, #64
	str	x8, [sp, #72]
.LBB7_3:
	ldur	x8, [x29, #-152]
	ldur	d0, [x29, #-120]
	add	x1, x21, #344
	ldp	w25, w27, [x29, #-140]
	str	x8, [sp, #64]
	ldurb	w8, [x29, #-132]
	mov	x0, x22
	ldur	x26, [x29, #-128]
	ldur	x28, [x29, #-112]
	sxtw	x25, w25
	str	w8, [sp, #224]
	ldur	d11, [x29, #-104]
	add	x8, sp, #432
	str	d0, [sp, #56]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldrb	w8, [x21, #432]
	ldr	x9, [sp, #432]
	add	x1, x21, #408
	ldr	d12, [sp, #464]
	mov	x0, x22
	str	x9, [sp, #48]
	ldr	x9, [sp, #456]
	cmp	w8, #1
	cset	w2, eq
	sub	x8, x29, #152
	str	x9, [sp, #184]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldur	x8, [x29, #-152]
	ldur	x9, [x29, #-128]
	add	x1, x21, #488
	ldurb	w20, [x29, #-132]
	ldur	d13, [x29, #-120]
	mov	x0, x22
	str	x8, [sp, #40]
	ldur	d14, [x29, #-104]
	ldp	w19, w8, [x29, #-140]
	sxtw	x19, w19
	str	w8, [sp, #180]
	ldur	x8, [x29, #-112]
	stp	x8, x9, [sp, #160]
	add	x8, sp, #480
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [sp, #480]
	ldr	d15, [sp, #512]
	add	x1, x21, #552
	mov	x0, x22
	str	x8, [sp, #32]
	ldr	w8, [sp, #488]
	str	w8, [sp, #196]
	ldr	x8, [sp, #504]
	str	x8, [sp, #152]
	add	x8, sp, #528
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldrsw	x9, [x22]
	ldr	q0, [x23]
	ldr	x8, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	stp	x23, x22, [sp]
	add	x0, sp, #232
	ldr	w21, [sp, #536]
	ldr	x22, [sp, #552]
	mov	w1, w24
	ldr	x8, [x8]
	ldr	d9, [sp, #560]
	str	w24, [sp, #20]
	add	x8, x8, x9, lsl #4
	ldr	x9, [sp, #528]
	ldr	w2, [x8, #8]
	str	x9, [sp, #24]
	bl	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	w8, [sp, #360]
	ldr	w9, [sp, #364]
	add	w8, w8, #1
	cmp	w8, w9
	str	w8, [sp, #360]
	b.ge	.LBB7_10
	ldr	w10, [sp, #224]
	index	z2.s, #0, #2
	index	z3.s, #2, #-2
	mov	x12, x25
	str	x22, [sp, #144]
	cmp	w10, #1
	add	x10, sp, #232
	str	w21, [sp, #140]
	cset	w23, eq
	cmp	w20, #1
	add	x10, x10, #64
	cset	w15, eq
	dup	v0.2s, w23
	str	x10, [sp, #88]
	dup	v1.2s, w15
	str	x28, [sp, #128]
	str	d9, [sp, #120]
	str	x26, [sp, #112]
	shl	v0.2s, v0.2s, #31
	stp	d14, d15, [sp, #96]
	shl	v1.2s, v1.2s, #31
	cmlt	v0.2s, v0.2s, #0
	cmlt	v1.2s, v1.2s, #0
	mov	v10.8b, v0.8b
	mov	v8.8b, v1.8b
	ldr	s1, [sp, #196]
	bsl	v10.8b, v3.8b, v2.8b
	bsl	v8.8b, v3.8b, v2.8b
	fmov	s3, w21
	b	.LBB7_6
.LBB7_5:
	add	w8, w8, #1
	cmp	w8, w9
	str	w8, [sp, #360]
	b.ge	.LBB7_10
.LBB7_6:
	ldr	x10, [sp, #88]
	ldrb	w16, [x10, w8, sxtw]
	cbz	x16, .LBB7_5
	add	x9, sp, #232
	ldr	x11, [sp, #64]
	fmov	d14, d13
	ldrb	w8, [x9, w8, sxtw]
	fmov	d13, d12
	fmov	d12, d11
	ldr	d11, [sp, #56]
	smaddl	x0, w12, w8, x11
	ldp	x9, x11, [sp, #40]
	lsl	x10, x8, #5
	smaddl	x17, w19, w8, x9
	lsl	x9, x8, #3
	add	x25, x11, x8
	ldr	x8, [sp, #24]
	add	x2, x8, x9
	ldr	x8, [sp, #72]
	add	x24, x8, x9
	ldr	x8, [sp, #32]
	add	x9, x8, x10
	ldr	x8, [sp, #80]
	add	x20, x8, x10
.LBB7_8:
	ldr	x8, [sp, #128]
	fmov	s0, wzr
	fmov	d15, d1
	ldp	d2, d5, [sp, #96]
	stp	x9, x2, [sp, #208]
	str	x8, [sp, #400]
	add	x8, sp, #368
	add	x4, sp, #576
	stur	wzr, [x8, #57]
	cmeq	v1.2s, v1.2s, v0.2s
	cmeq	v0.2s, v3.2s, v0.2s
	sturh	wzr, [x8, #61]
	ldr	x8, [sp, #184]
	add	x5, sp, #600
	str	x0, [sp, #224]
	sub	x6, x29, #200
	sub	x7, x29, #176
	str	x8, [sp, #584]
	ldr	w8, [sp, #180]
	mvn	v1.8b, v1.8b
	mvn	v0.8b, v0.8b
	str	x0, [sp, #368]
	ldr	x0, [sp, #200]
	str	w8, [sp, #608]
	ldp	x8, x10, [sp, #160]
	dup	v1.2s, v1.s[0]
	mov	x1, x20
	mov	x3, x24
	dup	v0.2s, v0.s[0]
	str	w27, [sp, #376]
	mov	w28, w15
	str	x10, [sp, #616]
	ldr	w10, [sp, #196]
	fmov	d9, d3
	str	x8, [sp, #632]
	add	x8, sp, #600
	mov	x22, x16
	cmp	w10, #0
	stur	wzr, [x8, #57]
	and	v1.8b, v1.8b, v5.8b
	sturh	wzr, [x8, #61]
	csel	x8, xzr, x9, eq
	ldr	x9, [sp, #152]
	ldr	d5, [sp, #120]
	str	wzr, [sp, #380]
	mov	x21, x17
	csel	x9, xzr, x9, eq
	str	x26, [sp, #384]
	mov	w26, w27
	stp	x8, x9, [x29, #-200]
	ldr	w8, [sp, #140]
	and	v0.8b, v0.8b, v5.8b
	ldr	x9, [sp, #144]
	str	d11, [sp, #392]
	mov	x27, x12
	cmp	w8, #0
	stp	d12, d10, [sp, #408]
	csel	x8, xzr, x2, eq
	csel	x9, xzr, x9, eq
	add	x2, sp, #368
	strb	w23, [sp, #424]
	strb	wzr, [sp, #431]
	str	x25, [sp, #576]
	str	d13, [sp, #592]
	str	x17, [sp, #600]
	str	d14, [sp, #624]
	str	d2, [sp, #640]
	str	d8, [sp, #648]
	strb	w15, [sp, #656]
	strb	wzr, [sp, #663]
	stur	d1, [x29, #-184]
	stp	x8, x9, [x29, #-176]
	stur	d0, [x29, #-160]
	bl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x9, x2, [sp, #208]
	fmov	d1, d15
	fmov	d3, d9
	ldr	x0, [sp, #224]
	mov	x12, x27
	mov	w27, w26
	ldr	x26, [sp, #112]
	mov	w15, w28
	subs	x16, x22, #1
	add	x17, x21, x19
	add	x25, x25, #1
	add	x2, x2, #8
	add	x24, x24, #8
	add	x9, x9, #32
	add	x20, x20, #32
	add	x0, x0, x12
	b.ne	.LBB7_8
	fmov	d11, d12
	fmov	d12, d13
	ldr	w8, [sp, #360]
	fmov	d13, d14
	ldr	w9, [sp, #364]
	b	.LBB7_5
.LBB7_10:
	ldr	x11, [sp, #200]
	ldr	w8, [x11, #624]
	tbnz	w8, #31, .LBB7_13
	ldr	w9, [sp, #20]
	cbz	w9, .LBB7_14
	ldr	x9, [sp]
	ptrue	p0.d, vl2
	ldr	q0, [x9]
	cnt	z0.d, p0/m, z0.d
	xtn	v0.2s, v0.2d
	dup	v1.2s, v0.s[1]
	add	v0.2s, v1.2s, v0.2s
	fmov	w9, s0
	b	.LBB7_15
.LBB7_13:
	mov	w9, wzr
	b	.LBB7_15
.LBB7_14:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	x10, [sp, #8]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldrsw	x10, [x10]
	ldr	x9, [x9]
	add	x9, x9, x10, lsl #4
	ldr	w9, [x9, #8]
.LBB7_15:
	add	w8, w9, w8
	str	w8, [x11, #624]
	.seh_startepilogue
	add	sp, sp, #784
	.seh_stackalloc	784
	ldp	d14, d15, [sp, #144]
	.seh_save_fregp	d14, 144
	ldp	d12, d13, [sp, #128]
	.seh_save_fregp	d12, 128
	ldp	d10, d11, [sp, #112]
	.seh_save_fregp	d10, 112
	ldp	d8, d9, [sp, #96]
	.seh_save_fregp	d8, 96
	ldp	x29, x30, [sp, #80]
	.seh_save_fplr	80
	ldp	x27, x28, [sp, #64]
	.seh_save_regp	x27, 64
	ldp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	ldp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	ldp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	ldp	x19, x20, [sp], #160
	.seh_save_regp_x	x19, 160
	.seh_endepilogue
	ret
.LBB7_16:
	ldrsh	x10, [x21, #294]
	ldr	w11, [x21, #296]
	ldr	x12, [x9, #112]
	ldr	w13, [x9, #168]
	tbnz	x10, #63, .LBB7_19
	cmp	w13, w10
	b.le	.LBB7_19
	ldr	w14, [x12, x10, lsl #2]
	cmp	w14, w11
	b.eq	.LBB7_26
.LBB7_19:
	cbz	w13, .LBB7_23
	mov	w10, wzr
.LBB7_21:
	ldr	w14, [x12, w10, sxtw #2]
	cmp	w14, w11
	b.eq	.LBB7_25
	add	w10, w10, #1
	cmp	w13, w10
	b.ne	.LBB7_21
.LBB7_23:
	ldr	x11, [sp, #200]
	mov	w10, #65535
	strh	w10, [x11, #294]
.LBB7_24:
	ldr	x10, [sp, #200]
	mov	w11, wzr
	str	wzr, [x10, #288]
	mov	w10, #65535
	b	.LBB7_27
.LBB7_25:
	mov	w11, #65535
	bics	wzr, w11, w10
	ldr	x11, [sp, #200]
	strh	w10, [x11, #294]
	b.eq	.LBB7_24
.LBB7_26:
	mov	w11, w10
	ldr	x12, [x9, #224]
	ldr	x13, [sp, #200]
	sxth	x11, w11
	ldr	w12, [x12, x11, lsl #2]
	str	w12, [x13, #288]
	ldr	x12, [x9, #232]
	ldrh	w11, [x12, x11, lsl #1]
.LBB7_27:
	ldr	x21, [sp, #200]
	strh	w11, [x21, #292]
	str	x9, [x21, #280]
	mov	w9, #65535
	bics	wzr, w9, w10
	b.ne	.LBB7_2
.LBB7_28:
	str	xzr, [sp, #72]
	b	.LBB7_3
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-96]!
	.seh_save_regp_x	x19, 96
	stp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	stp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	stp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	str	x27, [sp, #64]
	.seh_save_reg	x27, 64
	stp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	add	x29, sp, #72
	.seh_add_fp	72
	.seh_endprologue
	sub	sp, sp, #32
	and	x8, x4, #0xff
	mov	w9, #16
	mov	x23, x4
	cmp	x8, #3
	mov	w8, #10
	mov	x24, x3
	mov	w20, w2
	mov	x21, x1
	mov	x22, x0
	csel	x26, x9, x8, eq
	mov	w27, #-1
	mov	x8, x3
.LBB8_1:
	sdiv	x8, x8, x26
	add	w27, w27, #1
	cbnz	x8, .LBB8_1
	add	w2, w27, #2
	add	x8, x2, #15
	lsr	x15, x8, #4
	bl	__chkstk
	sub	x25, sp, x15, lsl #4
	mov	sp, x25
	mov	x0, x25
	mov	w1, wzr
	mov	w3, wzr
	bl	burst.memset.inline.ARMV9A.i32
	tst	x23, #0xff000000
	mov	w8, #87
	mov	w9, #55
	csel	w9, w9, w8, eq
	add	w8, w27, #1
	mov	x10, x24
.LBB8_3:
	sdiv	x11, x10, x26
	sub	w14, w27, #1
	msub	w10, w11, w26, w10
	cmp	w10, #0
	cneg	w10, w10, mi
	add	w12, w10, #48
	add	w13, w9, w10
	cmp	w10, #9
	csel	w10, w13, w12, gt
	strb	w10, [x25, w27, sxtw]
	mov	w27, w14
	mov	x10, x11
	cbnz	x11, .LBB8_3
	lsr	x9, x24, #63
	ubfx	w4, w23, #16, #8
	mov	w5, w23
	sub	x3, x29, #96
	mov	x0, x22
	mov	x1, x21
	mov	w2, w20
	stur	xzr, [x29, #-80]
	strb	wzr, [x25, w8, sxtw]
	stur	x25, [x29, #-96]
	stp	wzr, w8, [x29, #-88]
	stur	w8, [x29, #-80]
	sturb	w9, [x29, #-76]
	bl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.seh_startepilogue
	sub	sp, x29, #72
	.seh_add_fp	72
	ldp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	ldr	x27, [sp, #64]
	.seh_save_reg	x27, 64
	ldp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	ldp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	ldp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	ldp	x19, x20, [sp], #96
	.seh_save_regp_x	x19, 96
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-96]!
	.seh_save_regp_x	x19, 96
	stp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	stp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	stp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	str	x27, [sp, #64]
	.seh_save_reg	x27, 64
	stp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	add	x29, sp, #72
	.seh_add_fp	72
	.seh_endprologue
	sub	sp, sp, #32
	and	x8, x4, #0xff
	mov	w9, #16
	mov	x23, x4
	cmp	x8, #3
	mov	w8, #10
	mov	x24, x3
	mov	w20, w2
	mov	x21, x1
	mov	x22, x0
	csel	x26, x9, x8, eq
	mov	w27, #-1
	mov	x8, x3
.LBB9_1:
	udiv	x9, x8, x26
	cmp	x8, x26
	add	w27, w27, #1
	mov	x8, x9
	b.hs	.LBB9_1
	add	w2, w27, #2
	add	x8, x2, #15
	lsr	x15, x8, #4
	bl	__chkstk
	sub	x25, sp, x15, lsl #4
	mov	sp, x25
	mov	x0, x25
	mov	w1, wzr
	mov	w3, wzr
	bl	burst.memset.inline.ARMV9A.i32
	tst	x23, #0xff000000
	mov	w8, #87
	mov	w9, #55
	csel	w9, w9, w8, eq
	add	w8, w27, #1
.LBB9_3:
	udiv	x10, x24, x26
	msub	x11, x10, x26, x24
	orr	w12, w11, #0x30
	add	w13, w9, w11
	cmp	x11, #9
	sub	w11, w27, #1
	csel	w12, w13, w12, hi
	cmp	x24, x26
	strb	w12, [x25, w27, sxtw]
	mov	x24, x10
	mov	w27, w11
	b.hs	.LBB9_3
	ubfx	w4, w23, #16, #8
	mov	w5, w23
	sub	x3, x29, #96
	mov	x0, x22
	mov	x1, x21
	mov	w2, w20
	stur	xzr, [x29, #-80]
	strb	wzr, [x25, w8, sxtw]
	stur	x25, [x29, #-96]
	stp	wzr, w8, [x29, #-88]
	stur	w8, [x29, #-80]
	bl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.seh_startepilogue
	sub	sp, x29, #72
	.seh_add_fp	72
	ldp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	ldr	x27, [sp, #64]
	.seh_save_reg	x27, 64
	ldp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	ldp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	ldp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	ldp	x19, x20, [sp], #96
	.seh_save_regp_x	x19, 96
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	and	w8, w4, #0xff
	mov	w4, w4
	cmp	w8, #3
	b.ne	.LBB10_2
	mov	w3, w3
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	b	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB10_2:
	sxtw	x3, w3
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	b	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	ldr	w10, [x3, #16]
	ldrb	w12, [x3, #20]
	cmp	w10, w4
	cset	w8, le
	cmn	w10, #4
	cset	w9, gt
	ands	w8, w8, w9
	ldr	x9, [x3]
	csinc	w11, w10, wzr, ne
	cbz	w12, .LBB11_3
	ldrsw	x12, [x1]
	cmp	w12, w2
	b.ge	.LBB11_23
	add	w13, w12, #1
	str	w13, [x1]
	mov	w13, #45
	strb	w13, [x0, x12]
.LBB11_3:
	cmp	w11, #1
	b.lt	.LBB11_8
	add	w10, w11, #1
	mov	w11, #48
.LBB11_5:
	ldrsw	x12, [x1]
	cmp	w12, w2
	b.ge	.LBB11_23
	add	w13, w12, #1
	sub	w10, w10, #1
	str	w13, [x1]
	mov	x13, x9
	ldrb	w14, [x13], #1
	cmp	w14, #0
	csel	x9, x9, x13, eq
	csel	w13, w14, w11, ne
	cmp	w10, #1
	strb	w13, [x0, x12]
	b.gt	.LBB11_5
	mov	w10, wzr
	tbz	wzr, #31, .LBB11_10
	b	.LBB11_14
.LBB11_8:
	ldrsw	x11, [x1]
	cmp	w11, w2
	b.ge	.LBB11_23
	add	w12, w11, #1
	str	w12, [x1]
	mov	w12, #48
	strb	w12, [x0, x11]
	tbnz	w10, #31, .LBB11_14
.LBB11_10:
	ldrb	w11, [x9]
	cbnz	w11, .LBB11_14
.LBB11_11:
	tbnz	w8, #0, .LBB11_23
	ldrsw	x8, [x1]
	cmp	w8, w2
	b.ge	.LBB11_23
	add	w9, w8, #1
	mov	w4, #131074
	str	w9, [x1]
	strb	w5, [x0, x8]
	ldr	w8, [x3, #16]
	sub	w8, w8, #1
	sxtw	x3, w8
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	b	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB11_14:
	ldrsw	x11, [x1]
	cmp	w11, w2
	b.ge	.LBB11_23
	add	w12, w11, #1
	str	w12, [x1]
	mov	w12, #46
	strb	w12, [x0, x11]
	tbz	w10, #31, .LBB11_19
	mov	w11, #48
.LBB11_17:
	ldrsw	x12, [x1]
	cmp	w12, w2
	b.ge	.LBB11_23
	add	w13, w12, #1
	adds	w10, w10, #1
	str	w13, [x1]
	strb	w11, [x0, x12]
	b.lo	.LBB11_17
.LBB11_19:
	ldrb	w10, [x9], #1
	cbz	w10, .LBB11_11
.LBB11_21:
	ldrsw	x10, [x1]
	cmp	w10, w2
	b.ge	.LBB11_23
	add	w11, w10, #1
	str	w11, [x1]
	ldurb	w11, [x9, #-1]
	strb	w11, [x0, x10]
	ldrb	w10, [x9], #1
	cbnz	w10, .LBB11_21
	b	.LBB11_11
.LBB11_23:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #112
	.seh_stackalloc	112
	stp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	stp	x21, x22, [sp, #32]
	.seh_save_regp	x21, 32
	stp	x23, x24, [sp, #48]
	.seh_save_regp	x23, 48
	stp	x25, x26, [sp, #64]
	.seh_save_regp	x25, 64
	stp	x27, x28, [sp, #80]
	.seh_save_regp	x27, 80
	stp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	add	x29, sp, #96
	.seh_add_fp	96
	.seh_endprologue
	ldr	w10, [x3, #8]
	lsr	x24, x5, #8
	cbnz	w10, .LBB12_3
	and	x8, x5, #0xffffff
	and	x8, x8, #0xffffffffffff00ff
	cbnz	x8, .LBB12_3
	mov	w11, #1
	b	.LBB12_6
.LBB12_3:
	and	w11, w5, #0xff
	sub	w8, w11, #4
	cmn	w8, #4
	b.hi	.LBB12_6
	cmp	w4, #1
	b.lt	.LBB12_32
	ldr	x8, [x3]
	b	.LBB12_33
.LBB12_6:
	ldr	w8, [x3, #12]
	ubfx	w9, w5, #16, #8
	ldrb	w13, [x3, #20]
	sbfx	w12, w5, #8, #8
	subs	w10, w9, w8
	csel	w8, w8, w9, lt
	csel	w10, w10, wzr, gt
	cmp	w11, #2
	cset	w9, eq
	orr	w9, w13, w9
	cmp	w9, #0
	sxtb	w9, w24
	cinc	w8, w8, ne
	cmp	w12, #1
	b.lt	.LBB12_12
	subs	w12, w9, w8
	b.le	.LBB12_12
	mov	w13, #32
.LBB12_9:
	ldrsw	x14, [x1]
	cmp	w14, w2
	b.ge	.LBB12_76
	add	w15, w14, #1
	subs	w12, w12, #1
	str	w15, [x1]
	strb	w13, [x0, x14]
	b.ne	.LBB12_9
	ldrb	w13, [x3, #20]
.LBB12_12:
	cbz	w13, .LBB12_15
	ldr	w11, [x1]
	cmp	w11, w2
	b.ge	.LBB12_27
	mov	w12, #45
	b	.LBB12_18
.LBB12_15:
	cmp	w11, #2
	b.ne	.LBB12_19
	ldr	w11, [x1]
	cmp	w11, w2
	b.ge	.LBB12_27
	mov	w12, #43
.LBB12_18:
	add	w13, w11, #1
	str	w13, [x1]
	strb	w12, [x0, w11, sxtw]
.LBB12_19:
	cmp	w10, #1
	b.lt	.LBB12_23
	mov	w11, #48
.LBB12_21:
	ldrsw	x12, [x1]
	cmp	w12, w2
	b.ge	.LBB12_27
	add	w13, w12, #1
	subs	w10, w10, #1
	str	w13, [x1]
	strb	w11, [x0, x12]
	b.ne	.LBB12_21
.LBB12_23:
	ldr	w10, [x3, #12]
	cmp	w10, #1
	b.lt	.LBB12_27
	ldr	x11, [x3]
.LBB12_25:
	ldrsw	x12, [x1]
	cmp	w12, w2
	b.ge	.LBB12_27
	add	w13, w12, #1
	subs	x10, x10, #1
	str	w13, [x1]
	ldrb	w13, [x11], #1
	strb	w13, [x0, x12]
	b.ne	.LBB12_25
.LBB12_27:
	tbz	w5, #15, .LBB12_76
	cmn	w8, w9
	b.ge	.LBB12_76
	add	w8, w8, w9
	mov	w9, #32
	neg	w8, w8
.LBB12_30:
	ldrsw	x10, [x1]
	cmp	w10, w2
	b.ge	.LBB12_76
	add	w11, w10, #1
	subs	w8, w8, #1
	str	w11, [x1]
	strb	w9, [x0, x10]
	b.ne	.LBB12_30
	b	.LBB12_76
.LBB12_32:
	ldr	w4, [x3, #12]
	ldr	x8, [x3]
	cmp	w4, #1
	b.lt	.LBB12_37
.LBB12_33:
	mov	x11, xzr
	mov	w9, w4
.LBB12_34:
	ldrb	w12, [x8, x11]
	cbz	w12, .LBB12_38
	add	x11, x11, #1
	cmp	x9, x11
	b.ne	.LBB12_34
	mov	w4, w9
	cmp	w10, #1
	b.ne	.LBB12_40
	b	.LBB12_45
.LBB12_37:
	mov	w11, wzr
.LBB12_38:
	cmp	w11, w4
	b.ne	.LBB12_46
	mov	w9, w4
	cmp	w10, #1
	b.eq	.LBB12_45
.LBB12_40:
	ldrb	w10, [x8, x9]
	cmp	w10, #53
	b.lo	.LBB12_45
	add	w10, w9, #1
.LBB12_42:
	cmp	w9, #1
	b.lt	.LBB12_52
	add	x11, x8, x9
	sub	w10, w10, #1
	ldurb	w12, [x11, #-1]
	sub	x11, x9, #1
	mov	x9, x11
	cmp	w12, #57
	b.eq	.LBB12_42
	add	w9, w12, #1
	strb	w9, [x8, x11]
	b	.LBB12_54
.LBB12_45:
	mov	w11, w4
	b	.LBB12_47
.LBB12_46:
	mov	w9, w11
.LBB12_47:
	and	w11, w11, w11, asr #31
	add	w10, w9, #1
.LBB12_48:
	cmp	w9, #1
	b.lt	.LBB12_50
	add	x12, x8, x9
	sub	x9, x9, #1
	sub	w10, w10, #1
	ldurb	w12, [x12, #-1]
	cmp	w12, #48
	b.eq	.LBB12_48
	b	.LBB12_54
.LBB12_50:
	cbz	x9, .LBB12_53
	mov	w10, w11
	b	.LBB12_54
.LBB12_52:
	ldr	w9, [x3, #16]
	mov	w10, #49
	add	w9, w9, #1
	str	w9, [x3, #16]
	strb	w10, [x8]
	mov	w10, #1
	b	.LBB12_54
.LBB12_53:
	mov	w10, wzr
	str	wzr, [x3, #16]
.LBB12_54:
	strb	wzr, [x8, w10, sxtw]
	ldr	w20, [x3, #16]
	ldrb	w12, [x3, #20]
	str	w10, [x3, #12]
	cmp	w20, w4
	cset	w8, le
	cmn	w20, #4
	cset	w9, gt
	ands	w19, w8, w9
	ldr	x8, [x3]
	csinc	w9, w20, wzr, ne
	cmp	w12, #0
	cset	w11, ne
	cmp	w9, #1
	b.lt	.LBB12_58
	add	w10, w9, #1
.LBB12_56:
	mov	x12, x8
	sub	w10, w10, #1
	ldrb	w13, [x12], #1
	cmp	w13, #0
	csel	x8, x8, x12, eq
	cmp	w10, #1
	b.hi	.LBB12_56
	mov	w23, wzr
	add	w22, w9, w11
	ldrb	w9, [x8]
	tbz	wzr, #31, .LBB12_59
	b	.LBB12_62
.LBB12_58:
	cmp	w12, #0
	mov	w9, #1
	mov	w23, w20
	cinc	w22, w9, ne
	ldrb	w9, [x8]
	tbnz	w20, #31, .LBB12_62
.LBB12_59:
	cbnz	w9, .LBB12_62
	tbz	w19, #0, .LBB12_64
.LBB12_61:
	sbfx	w8, w5, #8, #8
	sxtb	w23, w24
	cmp	w8, #1
	b.ge	.LBB12_67
	b	.LBB12_71
.LBB12_62:
	bic	w10, w23, w23, asr #31
	add	w22, w22, w10
	cbz	w9, .LBB12_77
	str	x0, [sp, #8]
	add	x0, x8, #1
	mov	w25, w2
	mov	x28, x1
	mov	x26, x5
	mov	x21, x3
	mov	w27, w4
	bl	strlen
	mov	x8, x0
	ldr	x0, [sp, #8]
	mov	w4, w27
	add	w8, w22, w8
	mov	x3, x21
	mov	x5, x26
	mov	x1, x28
	mov	w2, w25
	add	w8, w8, #2
	sub	w22, w8, w23
	tbnz	w19, #0, .LBB12_61
.LBB12_64:
	sub	w9, w20, #1
	mov	x10, #7378697629483820646
	mov	w8, wzr
	sxtw	x9, w9
	movk	x10, #26215
	mov	x11, x9
.LBB12_65:
	smulh	x12, x11, x10
	add	x11, x11, #9
	add	w8, w8, #1
	cmp	x11, #18
	asr	x13, x12, #2
	add	x12, x13, x12, lsr #63
	mov	x11, x12
	b.hi	.LBB12_65
	cmp	w9, #0
	mov	w10, #1
	lsr	x9, x9, #63
	cinc	w10, w10, ge
	cmp	w8, #2
	mov	w11, #2
	add	w9, w10, w9
	csel	w8, w8, w11, gt
	add	w9, w9, w22
	add	w22, w9, w8
	sbfx	w8, w5, #8, #8
	sxtb	w23, w24
	cmp	w8, #1
	b.lt	.LBB12_71
.LBB12_67:
	subs	w8, w23, w22
	b.le	.LBB12_71
	mov	w9, #32
.LBB12_69:
	ldrsw	x10, [x1]
	cmp	w10, w2
	b.ge	.LBB12_76
	add	w11, w10, #1
	subs	w8, w8, #1
	str	w11, [x1]
	strb	w9, [x0, x10]
	b.ne	.LBB12_69
.LBB12_71:
	tst	x5, #0xff000000
	mov	w8, #101
	mov	w9, #69
	csel	w8, w9, w8, eq
	mov	x24, x5
	mov	x19, x0
	mov	w5, w8
	mov	x20, x1
	mov	w21, w2
	bl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	tbz	w24, #15, .LBB12_76
	cmn	w22, w23
	b.ge	.LBB12_76
	add	w8, w22, w23
	mov	w9, #32
	neg	w8, w8
.LBB12_74:
	ldrsw	x10, [x20]
	cmp	w10, w21
	b.ge	.LBB12_76
	add	w11, w10, #1
	subs	w8, w8, #1
	str	w11, [x20]
	strb	w9, [x19, x10]
	b.ne	.LBB12_74
.LBB12_76:
	.seh_startepilogue
	ldp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	ldp	x27, x28, [sp, #80]
	.seh_save_regp	x27, 80
	ldp	x25, x26, [sp, #64]
	.seh_save_regp	x25, 64
	ldp	x23, x24, [sp, #48]
	.seh_save_regp	x23, 48
	ldp	x21, x22, [sp, #32]
	.seh_save_regp	x21, 32
	ldp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	add	sp, sp, #112
	.seh_stackalloc	112
	.seh_endepilogue
	ret
.LBB12_77:
	add	w8, w22, #1
	sub	w22, w8, w23
	tbnz	w19, #0, .LBB12_61
	b	.LBB12_64
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	tbnz	w2, #0, .LBB13_2
	ldrb	w9, [x1, #24]
	cmp	w9, #1
	b.eq	.LBB13_15
.LBB13_2:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w11, [x0]
	ldr	x10, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	sbfiz	x11, x11, #4, #32
	ldr	x9, [x10]
	ldr	x9, [x9, x11]
	ldr	x11, [x1]
	cmp	x11, x9
	b.ne	.LBB13_6
	ldrh	w11, [x1, #14]
	tbz	w2, #0, .LBB13_19
.LBB13_4:
	mov	w12, #65535
	bics	wzr, w12, w11
	b.eq	.LBB13_25
	ldr	w12, [x0]
	lsl	x11, x12, #32
	b	.LBB13_21
.LBB13_6:
	ldrsh	x11, [x1, #14]
	ldr	w13, [x1, #16]
	mov	x12, x1
	ldr	x14, [x9, #112]
	ldr	w15, [x9, #168]
	tbnz	x11, #63, .LBB13_9
	cmp	w15, w11
	b.le	.LBB13_9
	ldr	w16, [x14, x11, lsl #2]
	cmp	w16, w13
	b.eq	.LBB13_17
.LBB13_9:
	cbz	w15, .LBB13_13
	mov	w11, wzr
.LBB13_11:
	ldr	w16, [x14, w11, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB13_16
	add	w11, w11, #1
	cmp	w15, w11
	b.ne	.LBB13_11
.LBB13_13:
	mov	w11, #65535
	strh	w11, [x12, #14]
.LBB13_14:
	mov	w13, wzr
	str	wzr, [x12, #8]
	mov	w11, #65535
	b	.LBB13_18
.LBB13_15:
	adrp	x8, burst_abort_Ptr
	adrp	x0, burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	add	x0, x0, :lo12:burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	ldr	x8, [x8, :lo12:burst_abort_Ptr]
	adrp	x1, burst.string.cd55498fb82486272ef3dc32c0931ef8
	add	x1, x1, :lo12:burst.string.cd55498fb82486272ef3dc32c0931ef8
	blr	x8
	brk	#0xf000
	brk	#0x1
.LBB13_16:
	mov	w13, #65535
	strh	w11, [x12, #14]
	bics	wzr, w13, w11
	b.eq	.LBB13_14
.LBB13_17:
	mov	w13, w11
	ldr	x14, [x9, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x12, #8]
	ldr	x14, [x9, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB13_18:
	mov	x1, x12
	strh	w13, [x12, #12]
	str	x9, [x12]
	tbnz	w2, #0, .LBB13_4
.LBB13_19:
	mov	w12, #65535
	bics	wzr, w12, w11
	b.eq	.LBB13_25
	ldr	w12, [x0]
	ldr	w13, [x9, #8]
	sxth	w14, w11
	ldr	x15, [x10]
	ldr	x16, [x9]
	lsl	x11, x12, #32
	lsl	w17, w13, #2
	mul	w13, w13, w14
	add	x14, x15, x11, asr #28
	add	x15, x16, w17, sxtw
	ldrsw	x14, [x14, #12]
	add	x13, x15, w13, sxtw #2
	ldr	w15, [x1, #20]
	str	w15, [x13, x14, lsl #2]
.LBB13_21:
	adrp	x13, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	asr	x11, x11, #35
	ubfiz	x12, x12, #14, #6
	ldr	x13, [x13, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x14, [x1, #8]
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x13, x11]
	add	x12, x12, x14
	add	x11, x11, x12
	adds	x11, x11, #64
	b.eq	.LBB13_25
	ldrsw	x12, [x0]
	ldr	x10, [x10]
	ldr	x13, [x0, #8]
	ldrsh	x14, [x1, #14]
	ldr	x9, [x9, #240]
	add	x10, x10, x12, lsl #4
	ldrb	w13, [x13, #494]
	ldr	w9, [x9, x14, lsl #2]
	ldr	w12, [x10, #8]
	ldrh	w10, [x1, #12]
	cbnz	w13, .LBB13_26
.LBB13_23:
	ldrb	w13, [x1, #24]
	index	z1.s, #0, #2
	index	z2.s, #2, #-2
	str	x11, [x8]
	ldr	x11, [x1, #40]
	cmp	w13, #1
	stp	w12, w10, [x8, #8]
	ldr	x10, [x1, #56]
	cset	w13, eq
	str	w9, [x8, #16]
	dup	v0.2s, w13
	strb	w13, [x8, #20]
	shl	v0.2s, v0.2s, #31
	cmlt	v0.2s, v0.2s, #0
	bit	v1.8b, v2.8b, v0.8b
	ldr	d0, [x1, #64]
	mov	v0.d[1], v1.d[0]
	ldr	d1, [x1, #48]
.LBB13_24:
	sturh	wzr, [x8, #21]
	strb	wzr, [x8, #23]
	str	x11, [x8, #24]
	str	d1, [x8, #32]
	str	x10, [x8, #40]
	str	q0, [x8, #48]
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
.LBB13_25:
	index	z1.s, #2, #-2
	movi	v2.2d, #0000000000000000
	ldr	d0, [x1, #64]
	ldr	x11, [x1, #40]
	ldr	x10, [x1, #56]
	mov	w9, #1
	str	wzr, [x8, #16]
	strb	w9, [x8, #20]
	mov	v0.d[1], v1.d[0]
	ldr	d1, [x1, #48]
	str	q2, [x8]
	b	.LBB13_24
.LBB13_26:
	ldrb	w13, [x1, #24]
	cmp	w13, #1
	b.eq	.LBB13_23
	ldp	w2, w3, [x1, #16]
	stp	x8, x1, [x29, #-16]
	mov	w1, #16
	mov	x4, xzr
	mov	w5, wzr
	str	x11, [sp, #16]
	stur	w9, [x29, #-20]
	str	w10, [sp, #12]
	str	w12, [sp, #24]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x8, x1, [x29, #-16]
	ldr	w12, [sp, #24]
	ldr	w10, [sp, #12]
	ldur	w9, [x29, #-20]
	ldr	x11, [sp, #16]
	b	.LBB13_23
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	tbnz	w2, #0, .LBB14_2
	ldrb	w9, [x1, #24]
	cmp	w9, #1
	b.eq	.LBB14_15
.LBB14_2:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w11, [x0]
	ldr	x10, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	sbfiz	x11, x11, #4, #32
	ldr	x9, [x10]
	ldr	x9, [x9, x11]
	ldr	x11, [x1]
	cmp	x11, x9
	b.ne	.LBB14_6
	ldrh	w11, [x1, #14]
	tbz	w2, #0, .LBB14_19
.LBB14_4:
	mov	w12, #65535
	bics	wzr, w12, w11
	b.eq	.LBB14_25
	ldr	w12, [x0]
	lsl	x11, x12, #32
	b	.LBB14_21
.LBB14_6:
	ldrsh	x11, [x1, #14]
	ldr	w13, [x1, #16]
	mov	x12, x1
	ldr	x14, [x9, #112]
	ldr	w15, [x9, #168]
	tbnz	x11, #63, .LBB14_9
	cmp	w15, w11
	b.le	.LBB14_9
	ldr	w16, [x14, x11, lsl #2]
	cmp	w16, w13
	b.eq	.LBB14_17
.LBB14_9:
	cbz	w15, .LBB14_13
	mov	w11, wzr
.LBB14_11:
	ldr	w16, [x14, w11, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB14_16
	add	w11, w11, #1
	cmp	w15, w11
	b.ne	.LBB14_11
.LBB14_13:
	mov	w11, #65535
	strh	w11, [x12, #14]
.LBB14_14:
	mov	w13, wzr
	str	wzr, [x12, #8]
	mov	w11, #65535
	b	.LBB14_18
.LBB14_15:
	adrp	x8, burst_abort_Ptr
	adrp	x0, burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	add	x0, x0, :lo12:burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	ldr	x8, [x8, :lo12:burst_abort_Ptr]
	adrp	x1, burst.string.cd55498fb82486272ef3dc32c0931ef8
	add	x1, x1, :lo12:burst.string.cd55498fb82486272ef3dc32c0931ef8
	blr	x8
	brk	#0xf000
	brk	#0x1
.LBB14_16:
	mov	w13, #65535
	strh	w11, [x12, #14]
	bics	wzr, w13, w11
	b.eq	.LBB14_14
.LBB14_17:
	mov	w13, w11
	ldr	x14, [x9, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x12, #8]
	ldr	x14, [x9, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB14_18:
	mov	x1, x12
	strh	w13, [x12, #12]
	str	x9, [x12]
	tbnz	w2, #0, .LBB14_4
.LBB14_19:
	mov	w12, #65535
	bics	wzr, w12, w11
	b.eq	.LBB14_25
	ldr	w12, [x0]
	ldr	w13, [x9, #8]
	sxth	w14, w11
	ldr	x15, [x10]
	ldr	x16, [x9]
	lsl	x11, x12, #32
	lsl	w17, w13, #2
	mul	w13, w13, w14
	add	x14, x15, x11, asr #28
	add	x15, x16, w17, sxtw
	ldrsw	x14, [x14, #12]
	add	x13, x15, w13, sxtw #2
	ldr	w15, [x1, #20]
	str	w15, [x13, x14, lsl #2]
.LBB14_21:
	adrp	x13, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	asr	x11, x11, #35
	ubfiz	x12, x12, #14, #6
	ldr	x13, [x13, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x14, [x1, #8]
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x13, x11]
	add	x12, x12, x14
	add	x11, x11, x12
	adds	x11, x11, #64
	b.eq	.LBB14_25
	ldrsw	x12, [x0]
	ldr	x10, [x10]
	ldr	x13, [x0, #8]
	ldrsh	x14, [x1, #14]
	ldr	x9, [x9, #240]
	add	x10, x10, x12, lsl #4
	ldrb	w13, [x13, #494]
	ldr	w9, [x9, x14, lsl #2]
	ldr	w12, [x10, #8]
	ldrh	w10, [x1, #12]
	cbnz	w13, .LBB14_26
.LBB14_23:
	ldrb	w13, [x1, #24]
	index	z1.s, #0, #2
	index	z2.s, #2, #-2
	str	x11, [x8]
	ldr	x11, [x1, #40]
	cmp	w13, #1
	stp	w12, w10, [x8, #8]
	ldr	x10, [x1, #56]
	cset	w13, eq
	str	w9, [x8, #16]
	dup	v0.2s, w13
	strb	w13, [x8, #20]
	shl	v0.2s, v0.2s, #31
	cmlt	v0.2s, v0.2s, #0
	bit	v1.8b, v2.8b, v0.8b
	ldr	d0, [x1, #64]
	mov	v0.d[1], v1.d[0]
	ldr	d1, [x1, #48]
.LBB14_24:
	sturh	wzr, [x8, #21]
	strb	wzr, [x8, #23]
	str	x11, [x8, #24]
	str	d1, [x8, #32]
	str	x10, [x8, #40]
	str	q0, [x8, #48]
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
.LBB14_25:
	index	z1.s, #2, #-2
	movi	v2.2d, #0000000000000000
	ldr	d0, [x1, #64]
	ldr	x11, [x1, #40]
	ldr	x10, [x1, #56]
	mov	w9, #1
	str	wzr, [x8, #16]
	strb	w9, [x8, #20]
	mov	v0.d[1], v1.d[0]
	ldr	d1, [x1, #48]
	str	q2, [x8]
	b	.LBB14_24
.LBB14_26:
	ldrb	w13, [x1, #24]
	cmp	w13, #1
	b.eq	.LBB14_23
	ldp	w2, w3, [x1, #16]
	stp	x8, x1, [x29, #-16]
	mov	w1, #16
	mov	x4, xzr
	mov	w5, wzr
	str	x11, [sp, #16]
	stur	w9, [x29, #-20]
	str	w10, [sp, #12]
	str	w12, [sp, #24]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x8, x1, [x29, #-16]
	ldr	w12, [sp, #24]
	ldr	w10, [sp, #12]
	ldur	w9, [x29, #-20]
	ldr	x11, [sp, #16]
	b	.LBB14_23
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #32
	.seh_stackalloc	32
	stp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	x29, sp, #16
	.seh_add_fp	16
	.seh_endprologue
	ldrb	w8, [x1, #28]
	cmp	w8, #1
	b.eq	.LBB15_5
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w8, [x0]
	ldr	x12, [x1]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	sbfiz	x10, x8, #4, #32
	ldr	x11, [x9]
	ldr	x11, [x11, x10]
	cmp	x12, x11
	b.ne	.LBB15_6
	ldrh	w12, [x1, #14]
	mov	w13, #65535
	bics	wzr, w13, w12
	b.eq	.LBB15_18
.LBB15_3:
	ldr	w13, [x11, #8]
	sxth	w12, w12
	ldr	x11, [x11]
	ldr	x15, [x9]
	lsl	w14, w13, #2
	mul	w12, w13, w12
	lsl	x13, x8, #32
	add	x10, x15, x10
	ubfiz	x8, x8, #14, #6
	add	x11, x11, w14, sxtw
	ldrsw	x10, [x10, #12]
	add	x11, x11, w12, sxtw #2
	ldr	w12, [x1, #24]
	str	w12, [x11, x10, lsl #2]
	adrp	x10, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	asr	x11, x13, #35
	ldr	x10, [x10, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x12, [x1, #8]
	and	x11, x11, #0xfffffffffffffff8
	ldr	x10, [x10, x11]
	add	x8, x8, x12
	add	x8, x10, x8
	add	x8, x8, #64
	ldr	x10, [x0, #8]
	ldrb	w10, [x10, #494]
	cbnz	w10, .LBB15_19
.LBB15_4:
	mov	x0, x8
	.seh_startepilogue
	ldp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	sp, sp, #32
	.seh_stackalloc	32
	.seh_endepilogue
	ret
.LBB15_5:
	adrp	x8, burst_abort_Ptr
	adrp	x0, burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	add	x0, x0, :lo12:burst.string.1ea13c7f7ea144ea32205d7ec317f39f
	ldr	x8, [x8, :lo12:burst_abort_Ptr]
	adrp	x1, burst.string.989b224dc613a391593dc022965ef313
	add	x1, x1, :lo12:burst.string.989b224dc613a391593dc022965ef313
	blr	x8
	brk	#0xf000
	brk	#0x1
.LBB15_6:
	ldrsh	x12, [x1, #14]
	ldr	w13, [x1, #16]
	ldr	x14, [x11, #112]
	ldr	w15, [x11, #168]
	tbnz	x12, #63, .LBB15_9
	cmp	w15, w12
	b.le	.LBB15_9
	ldr	w16, [x14, x12, lsl #2]
	cmp	w16, w13
	b.eq	.LBB15_16
.LBB15_9:
	cbz	w15, .LBB15_13
	mov	w12, wzr
.LBB15_11:
	ldr	w16, [x14, w12, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB15_15
	add	w12, w12, #1
	cmp	w15, w12
	b.ne	.LBB15_11
.LBB15_13:
	mov	w12, #65535
	strh	w12, [x1, #14]
.LBB15_14:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w12, #65535
	b	.LBB15_17
.LBB15_15:
	mov	w13, #65535
	strh	w12, [x1, #14]
	bics	wzr, w13, w12
	b.eq	.LBB15_14
.LBB15_16:
	mov	w13, w12
	ldr	x14, [x11, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x1, #8]
	ldr	x14, [x11, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB15_17:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w13, #65535
	bics	wzr, w13, w12
	b.ne	.LBB15_3
.LBB15_18:
	mov	x8, xzr
	ldr	x10, [x0, #8]
	ldrb	w10, [x10, #494]
	cbz	w10, .LBB15_4
.LBB15_19:
	ldrsw	x10, [x0]
	ldr	x9, [x9]
	mov	x4, x8
	ldr	w2, [x1, #16]
	ldr	w3, [x1, #24]
	str	x8, [sp, #8]
	add	x9, x9, x10, lsl #4
	ldrh	w10, [x1, #12]
	mov	w1, #14
	ldr	w9, [x9, #8]
	mul	w5, w9, w10
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [sp, #8]
	b	.LBB15_4
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w10, [x0]
	ldrb	w14, [x1, #28]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	w13, [x1, #16]
	sbfiz	x12, x10, #4, #32
	cmp	w14, #1
	ldr	x11, [x9]
	ldr	x11, [x11, x12]
	b.ne	.LBB16_3
	ldr	x12, [x1]
	cmp	x12, x11
	b.ne	.LBB16_12
	ldrh	w12, [x1, #14]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB16_6
	b	.LBB16_36
.LBB16_3:
	ldr	x14, [x1]
	cmp	x14, x11
	b.ne	.LBB16_21
	ldrh	w14, [x1, #14]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.eq	.LBB16_36
.LBB16_5:
	ldr	w13, [x11, #8]
	sxth	w14, w14
	ldr	x11, [x11]
	ldr	x16, [x9]
	lsl	w15, w13, #2
	mul	w13, w13, w14
	add	x12, x16, x12
	add	x11, x11, w15, sxtw
	ldrsw	x12, [x12, #12]
	add	x11, x11, w13, sxtw #2
	ldr	w13, [x1, #24]
	str	w13, [x11, x12, lsl #2]
.LBB16_6:
	lsl	x11, x10, #32
	adrp	x12, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	ubfiz	x10, x10, #14, #6
	ldr	x12, [x12, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x13, [x1, #8]
	asr	x11, x11, #35
	add	x10, x10, x13
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x12, x11]
	add	x10, x11, x10
	adds	x10, x10, #64
	b.eq	.LBB16_36
	ldrsw	x11, [x0]
	ldr	x9, [x9]
	ldr	d0, [x1, #56]
	add	x9, x9, x11, lsl #4
	ldr	x11, [x0, #8]
	ldr	w12, [x9, #8]
	ldrb	w13, [x11, #494]
	ldr	x9, [x1, #48]
	sub	w11, w12, #1
	cbnz	w13, .LBB16_10
.LBB16_8:
	str	x10, [x8]
	mov	w10, #1
	stp	w12, wzr, [x8, #8]
.LBB16_9:
	stp	w11, wzr, [x8, #16]
	str	x9, [x8, #24]
	str	d0, [x8, #32]
	stp	w10, wzr, [x8, #40]
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
.LBB16_10:
	ldrb	w13, [x1, #28]
	cmp	w13, #1
	b.eq	.LBB16_8
	ldp	w2, w13, [x1, #16]
	ldr	w3, [x1, #24]
	mov	w1, #14
	mov	x4, x10
	stp	x8, x10, [x29, #-16]
	mul	w5, w13, w12
	str	x9, [sp, #8]
	str	d0, [sp, #24]
	str	w11, [sp, #4]
	str	w12, [sp, #20]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x8, x10, [x29, #-16]
	ldr	w12, [sp, #20]
	ldr	w11, [sp, #4]
	ldr	d0, [sp, #24]
	ldr	x9, [sp, #8]
	b	.LBB16_8
.LBB16_12:
	ldrsh	x12, [x1, #14]
	ldr	x14, [x11, #112]
	ldr	w15, [x11, #168]
	tbnz	x12, #63, .LBB16_15
	cmp	w15, w12
	b.le	.LBB16_15
	ldr	w16, [x14, x12, lsl #2]
	cmp	w16, w13
	b.eq	.LBB16_31
.LBB16_15:
	cbz	w15, .LBB16_19
	mov	w12, wzr
.LBB16_17:
	ldr	w16, [x14, w12, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB16_30
	add	w12, w12, #1
	cmp	w15, w12
	b.ne	.LBB16_17
.LBB16_19:
	mov	w12, #65535
	strh	w12, [x1, #14]
.LBB16_20:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w12, #65535
	b	.LBB16_32
.LBB16_21:
	ldrsh	x14, [x1, #14]
	ldr	x15, [x11, #112]
	ldr	w16, [x11, #168]
	tbnz	x14, #63, .LBB16_24
	cmp	w16, w14
	b.le	.LBB16_24
	ldr	w17, [x15, x14, lsl #2]
	cmp	w17, w13
	b.eq	.LBB16_34
.LBB16_24:
	cbz	w16, .LBB16_28
	mov	w14, wzr
.LBB16_26:
	ldr	w17, [x15, w14, sxtw #2]
	cmp	w17, w13
	b.eq	.LBB16_33
	add	w14, w14, #1
	cmp	w16, w14
	b.ne	.LBB16_26
.LBB16_28:
	mov	w13, #65535
	strh	w13, [x1, #14]
.LBB16_29:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w14, #65535
	b	.LBB16_35
.LBB16_30:
	mov	w13, #65535
	strh	w12, [x1, #14]
	bics	wzr, w13, w12
	b.eq	.LBB16_20
.LBB16_31:
	mov	w13, w12
	ldr	x14, [x11, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x1, #8]
	ldr	x14, [x11, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB16_32:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB16_6
	b	.LBB16_36
.LBB16_33:
	mov	w13, #65535
	strh	w14, [x1, #14]
	bics	wzr, w13, w14
	b.eq	.LBB16_29
.LBB16_34:
	mov	w13, w14
	ldr	x15, [x11, #224]
	sxth	x13, w13
	ldr	w15, [x15, x13, lsl #2]
	str	w15, [x1, #8]
	ldr	x15, [x11, #232]
	ldrh	w13, [x15, x13, lsl #1]
.LBB16_35:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.ne	.LBB16_5
.LBB16_36:
	movi	v1.2d, #0000000000000000
	ldr	x9, [x1, #48]
	ldr	d0, [x1, #56]
	mov	w10, wzr
	mov	w11, #-1
	str	q1, [x8]
	b	.LBB16_9
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w10, [x0]
	ldrb	w14, [x1, #28]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	w13, [x1, #16]
	sbfiz	x12, x10, #4, #32
	cmp	w14, #1
	ldr	x11, [x9]
	ldr	x11, [x11, x12]
	b.ne	.LBB17_3
	ldr	x12, [x1]
	cmp	x12, x11
	b.ne	.LBB17_12
	ldrh	w12, [x1, #14]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB17_6
	b	.LBB17_36
.LBB17_3:
	ldr	x14, [x1]
	cmp	x14, x11
	b.ne	.LBB17_21
	ldrh	w14, [x1, #14]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.eq	.LBB17_36
.LBB17_5:
	ldr	w13, [x11, #8]
	sxth	w14, w14
	ldr	x11, [x11]
	ldr	x16, [x9]
	lsl	w15, w13, #2
	mul	w13, w13, w14
	add	x12, x16, x12
	add	x11, x11, w15, sxtw
	ldrsw	x12, [x12, #12]
	add	x11, x11, w13, sxtw #2
	ldr	w13, [x1, #24]
	str	w13, [x11, x12, lsl #2]
.LBB17_6:
	lsl	x11, x10, #32
	adrp	x12, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	ubfiz	x10, x10, #14, #6
	ldr	x12, [x12, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x13, [x1, #8]
	asr	x11, x11, #35
	add	x10, x10, x13
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x12, x11]
	add	x10, x11, x10
	adds	x10, x10, #64
	b.eq	.LBB17_36
	ldrsw	x11, [x0]
	ldr	x9, [x9]
	ldr	d0, [x1, #56]
	add	x9, x9, x11, lsl #4
	ldr	x11, [x0, #8]
	ldr	w12, [x9, #8]
	ldrb	w13, [x11, #494]
	ldr	x9, [x1, #48]
	sub	w11, w12, #1
	cbnz	w13, .LBB17_10
.LBB17_8:
	str	x10, [x8]
	mov	w10, #1
	stp	w12, wzr, [x8, #8]
.LBB17_9:
	stp	w11, wzr, [x8, #16]
	str	x9, [x8, #24]
	str	d0, [x8, #32]
	stp	w10, wzr, [x8, #40]
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
.LBB17_10:
	ldrb	w13, [x1, #28]
	cmp	w13, #1
	b.eq	.LBB17_8
	ldp	w2, w13, [x1, #16]
	ldr	w3, [x1, #24]
	mov	w1, #14
	mov	x4, x10
	stp	x8, x10, [x29, #-16]
	mul	w5, w13, w12
	str	x9, [sp, #8]
	str	d0, [sp, #24]
	str	w11, [sp, #4]
	str	w12, [sp, #20]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x8, x10, [x29, #-16]
	ldr	w12, [sp, #20]
	ldr	w11, [sp, #4]
	ldr	d0, [sp, #24]
	ldr	x9, [sp, #8]
	b	.LBB17_8
.LBB17_12:
	ldrsh	x12, [x1, #14]
	ldr	x14, [x11, #112]
	ldr	w15, [x11, #168]
	tbnz	x12, #63, .LBB17_15
	cmp	w15, w12
	b.le	.LBB17_15
	ldr	w16, [x14, x12, lsl #2]
	cmp	w16, w13
	b.eq	.LBB17_31
.LBB17_15:
	cbz	w15, .LBB17_19
	mov	w12, wzr
.LBB17_17:
	ldr	w16, [x14, w12, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB17_30
	add	w12, w12, #1
	cmp	w15, w12
	b.ne	.LBB17_17
.LBB17_19:
	mov	w12, #65535
	strh	w12, [x1, #14]
.LBB17_20:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w12, #65535
	b	.LBB17_32
.LBB17_21:
	ldrsh	x14, [x1, #14]
	ldr	x15, [x11, #112]
	ldr	w16, [x11, #168]
	tbnz	x14, #63, .LBB17_24
	cmp	w16, w14
	b.le	.LBB17_24
	ldr	w17, [x15, x14, lsl #2]
	cmp	w17, w13
	b.eq	.LBB17_34
.LBB17_24:
	cbz	w16, .LBB17_28
	mov	w14, wzr
.LBB17_26:
	ldr	w17, [x15, w14, sxtw #2]
	cmp	w17, w13
	b.eq	.LBB17_33
	add	w14, w14, #1
	cmp	w16, w14
	b.ne	.LBB17_26
.LBB17_28:
	mov	w13, #65535
	strh	w13, [x1, #14]
.LBB17_29:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w14, #65535
	b	.LBB17_35
.LBB17_30:
	mov	w13, #65535
	strh	w12, [x1, #14]
	bics	wzr, w13, w12
	b.eq	.LBB17_20
.LBB17_31:
	mov	w13, w12
	ldr	x14, [x11, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x1, #8]
	ldr	x14, [x11, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB17_32:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB17_6
	b	.LBB17_36
.LBB17_33:
	mov	w13, #65535
	strh	w14, [x1, #14]
	bics	wzr, w13, w14
	b.eq	.LBB17_29
.LBB17_34:
	mov	w13, w14
	ldr	x15, [x11, #224]
	sxth	x13, w13
	ldr	w15, [x15, x13, lsl #2]
	str	w15, [x1, #8]
	ldr	x15, [x11, #232]
	ldrh	w13, [x15, x13, lsl #1]
.LBB17_35:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.ne	.LBB17_5
.LBB17_36:
	movi	v1.2d, #0000000000000000
	ldr	x9, [x1, #48]
	ldr	d0, [x1, #56]
	mov	w10, wzr
	mov	w11, #-1
	str	q1, [x8]
	b	.LBB17_9
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w10, [x0]
	ldrb	w14, [x1, #28]
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	w13, [x1, #16]
	sbfiz	x12, x10, #4, #32
	cmp	w14, #1
	ldr	x11, [x9]
	ldr	x11, [x11, x12]
	b.ne	.LBB18_3
	ldr	x12, [x1]
	cmp	x12, x11
	b.ne	.LBB18_12
	ldrh	w12, [x1, #14]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB18_6
	b	.LBB18_36
.LBB18_3:
	ldr	x14, [x1]
	cmp	x14, x11
	b.ne	.LBB18_21
	ldrh	w14, [x1, #14]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.eq	.LBB18_36
.LBB18_5:
	ldr	w13, [x11, #8]
	sxth	w14, w14
	ldr	x11, [x11]
	ldr	x16, [x9]
	lsl	w15, w13, #2
	mul	w13, w13, w14
	add	x12, x16, x12
	add	x11, x11, w15, sxtw
	ldrsw	x12, [x12, #12]
	add	x11, x11, w13, sxtw #2
	ldr	w13, [x1, #24]
	str	w13, [x11, x12, lsl #2]
.LBB18_6:
	lsl	x11, x10, #32
	adrp	x12, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	ubfiz	x10, x10, #14, #6
	ldr	x12, [x12, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrsw	x13, [x1, #8]
	asr	x11, x11, #35
	add	x10, x10, x13
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x12, x11]
	add	x10, x11, x10
	adds	x10, x10, #64
	b.eq	.LBB18_36
	ldrsw	x11, [x0]
	ldr	x9, [x9]
	ldr	d0, [x1, #56]
	add	x9, x9, x11, lsl #4
	ldr	x11, [x0, #8]
	ldr	w12, [x9, #8]
	ldrb	w13, [x11, #494]
	ldr	x9, [x1, #48]
	sub	w11, w12, #1
	cbnz	w13, .LBB18_10
.LBB18_8:
	str	x10, [x8]
	mov	w10, #1
	stp	w12, wzr, [x8, #8]
.LBB18_9:
	stp	w11, wzr, [x8, #16]
	str	x9, [x8, #24]
	str	d0, [x8, #32]
	stp	w10, wzr, [x8, #40]
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
.LBB18_10:
	ldrb	w13, [x1, #28]
	cmp	w13, #1
	b.eq	.LBB18_8
	ldp	w2, w13, [x1, #16]
	ldr	w3, [x1, #24]
	mov	w1, #14
	mov	x4, x10
	stp	x8, x10, [x29, #-16]
	mul	w5, w13, w12
	str	x9, [sp, #8]
	str	d0, [sp, #24]
	str	w11, [sp, #4]
	str	w12, [sp, #20]
	bl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldp	x8, x10, [x29, #-16]
	ldr	w12, [sp, #20]
	ldr	w11, [sp, #4]
	ldr	d0, [sp, #24]
	ldr	x9, [sp, #8]
	b	.LBB18_8
.LBB18_12:
	ldrsh	x12, [x1, #14]
	ldr	x14, [x11, #112]
	ldr	w15, [x11, #168]
	tbnz	x12, #63, .LBB18_15
	cmp	w15, w12
	b.le	.LBB18_15
	ldr	w16, [x14, x12, lsl #2]
	cmp	w16, w13
	b.eq	.LBB18_31
.LBB18_15:
	cbz	w15, .LBB18_19
	mov	w12, wzr
.LBB18_17:
	ldr	w16, [x14, w12, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB18_30
	add	w12, w12, #1
	cmp	w15, w12
	b.ne	.LBB18_17
.LBB18_19:
	mov	w12, #65535
	strh	w12, [x1, #14]
.LBB18_20:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w12, #65535
	b	.LBB18_32
.LBB18_21:
	ldrsh	x14, [x1, #14]
	ldr	x15, [x11, #112]
	ldr	w16, [x11, #168]
	tbnz	x14, #63, .LBB18_24
	cmp	w16, w14
	b.le	.LBB18_24
	ldr	w17, [x15, x14, lsl #2]
	cmp	w17, w13
	b.eq	.LBB18_34
.LBB18_24:
	cbz	w16, .LBB18_28
	mov	w14, wzr
.LBB18_26:
	ldr	w17, [x15, w14, sxtw #2]
	cmp	w17, w13
	b.eq	.LBB18_33
	add	w14, w14, #1
	cmp	w16, w14
	b.ne	.LBB18_26
.LBB18_28:
	mov	w13, #65535
	strh	w13, [x1, #14]
.LBB18_29:
	mov	w13, wzr
	str	wzr, [x1, #8]
	mov	w14, #65535
	b	.LBB18_35
.LBB18_30:
	mov	w13, #65535
	strh	w12, [x1, #14]
	bics	wzr, w13, w12
	b.eq	.LBB18_20
.LBB18_31:
	mov	w13, w12
	ldr	x14, [x11, #224]
	sxth	x13, w13
	ldr	w14, [x14, x13, lsl #2]
	str	w14, [x1, #8]
	ldr	x14, [x11, #232]
	ldrh	w13, [x14, x13, lsl #1]
.LBB18_32:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w11, #65535
	bics	wzr, w11, w12
	b.ne	.LBB18_6
	b	.LBB18_36
.LBB18_33:
	mov	w13, #65535
	strh	w14, [x1, #14]
	bics	wzr, w13, w14
	b.eq	.LBB18_29
.LBB18_34:
	mov	w13, w14
	ldr	x15, [x11, #224]
	sxth	x13, w13
	ldr	w15, [x15, x13, lsl #2]
	str	w15, [x1, #8]
	ldr	x15, [x11, #232]
	ldrh	w13, [x15, x13, lsl #1]
.LBB18_35:
	strh	w13, [x1, #12]
	str	x11, [x1]
	mov	w13, #65535
	bics	wzr, w13, w14
	b.ne	.LBB18_5
.LBB18_36:
	movi	v1.2d, #0000000000000000
	ldr	x9, [x1, #48]
	ldr	d0, [x1, #56]
	mov	w10, wzr
	mov	w11, #-1
	str	q1, [x8]
	b	.LBB18_9
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	movi	v0.2d, #0000000000000000
	mov	x9, x0
	mov	w0, w1
	ldr	x1, [x9, #8]
	mov	x8, x4
	stur	w2, [x29, #-4]
	str	w5, [sp, #8]
	sub	x5, x29, #20
	sub	x6, x29, #4
	mov	w2, w3
	mov	x3, x9
	mov	w4, #1
	mov	w7, #1
	stur	q0, [x29, #-20]
	str	x8, [sp]
	bl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-32]!
	.seh_save_regp_x	x19, 32
	stp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	x29, sp, #16
	.seh_add_fp	16
	.seh_endprologue
	mov	x20, x0
	ldrb	w9, [x0, #76]
	ldr	x0, [x0, #40]
	mov	x19, x8
	cbz	w9, .LBB20_4
	ldr	x8, [x0, #8]
	sbfiz	x9, x1, #3, #32
	adrp	x10, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	x8, [x8, x9]
	ldr	x9, [x10, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x9, [x9]
	sbfiz	x10, x8, #4, #32
	ldr	x11, [x9, x10]
	ldr	x9, [x20, #48]
	lsl	x10, x8, #32
	cmp	x9, x11
	mov	w9, w8
	b.ne	.LBB20_6
	ldr	w12, [x20, #56]
	ldrh	w13, [x20, #60]
.LBB20_3:
	adrp	x11, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	asr	x10, x10, #35
	lsr	x8, x8, #32
	ldr	x11, [x11, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ubfiz	x9, x9, #14, #6
	and	x10, x10, #0xfffffffffffffff8
	madd	w8, w13, w8, w12
	ldr	x10, [x11, x10]
	add	x9, x10, x9
	add	x8, x9, w8, sxtw
	add	x0, x8, #64
	mov	w8, #1
	b	.LBB20_5
.LBB20_4:
	ldp	w8, w3, [x20, #64]
	mov	w9, w1
	add	x4, x20, #48
	orr	x1, x9, x2, lsl #32
	mov	x2, x8
	bl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldrb	w8, [x20, #76]
	tst	w8, #0xff
	cset	w8, ne
.LBB20_5:
	ldr	w9, [x20, #72]
	ldr	x10, [x20]
	cmp	w8, #0
	ldr	d0, [x20, #8]
	str	x0, [x19]
	stp	w9, wzr, [x19, #8]
	ldr	x9, [x20, #16]
	str	x10, [x19, #16]
	mov	w10, #2
	str	d0, [x19, #24]
	ldr	d0, [x20, #24]
	str	x9, [x19, #32]
	csel	w9, w10, wzr, ne
	csel	w10, wzr, w10, ne
	str	d0, [x19, #40]
	stp	w9, w10, [x19, #48]
	strb	w8, [x19, #56]
	stur	wzr, [x19, #57]
	sturh	wzr, [x19, #61]
	strb	wzr, [x19, #63]
	.seh_startepilogue
	ldp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	ldp	x19, x20, [sp], #32
	.seh_save_regp_x	x19, 32
	.seh_endepilogue
	ret
.LBB20_6:
	ldrsh	x12, [x20, #62]
	ldr	w13, [x20, #64]
	ldr	x14, [x11, #112]
	ldr	w15, [x11, #168]
	tbnz	x12, #63, .LBB20_9
	cmp	w15, w12
	b.le	.LBB20_9
	ldr	w16, [x14, x12, lsl #2]
	cmp	w16, w13
	b.eq	.LBB20_16
.LBB20_9:
	cbz	w15, .LBB20_13
	mov	w12, wzr
.LBB20_11:
	ldr	w16, [x14, w12, sxtw #2]
	cmp	w16, w13
	b.eq	.LBB20_15
	add	w12, w12, #1
	cmp	w15, w12
	b.ne	.LBB20_11
.LBB20_13:
	mov	w12, #65535
	strh	w12, [x20, #62]
.LBB20_14:
	mov	w12, wzr
	mov	w13, wzr
	str	wzr, [x20, #56]
	b	.LBB20_17
.LBB20_15:
	mov	w13, #65535
	strh	w12, [x20, #62]
	bics	wzr, w13, w12
	b.eq	.LBB20_14
.LBB20_16:
	ldr	x13, [x11, #224]
	sxth	x14, w12
	ldr	w12, [x13, x14, lsl #2]
	str	w12, [x20, #56]
	ldr	x13, [x11, #232]
	ldrh	w13, [x13, x14, lsl #1]
.LBB20_17:
	strh	w13, [x20, #60]
	str	x11, [x20, #48]
	b	.LBB20_3
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	ldrb	w9, [x0, #48]
	ldr	x8, [x0]
	cbz	w9, .LBB21_5
	movi	v0.2d, #0000000000000000
	sbfiz	x10, x1, #2, #32
	sbfiz	x9, x1, #3, #32
	lsr	x13, x1, #32
	str	q0, [x2]
	ldp	x11, x12, [x8]
	ldr	w10, [x11, x10]
	ldr	w11, [x12, x9]
	cmp	w10, w13
	ldr	w10, [x0, #40]
	ccmp	w11, #0, #4, eq
	cset	w11, ne
	strb	w11, [x3]
	b.eq	.LBB21_10
	ldr	x8, [x8, #8]
	adrp	x11, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	w8, [x8, x9]
	ldr	x9, [x11, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x9, [x9]
	sbfiz	x8, x8, #4, #32
	ldr	x8, [x9, x8]
	ldr	x9, [x0, #8]
	cmp	x8, x9
	b.ne	.LBB21_11
	ldrh	w9, [x0, #22]
.LBB21_4:
	mov	w8, #65535
	bics	wzr, w8, w9
	cset	w0, ne
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
.LBB21_5:
	ldp	x11, x12, [x8]
	sbfiz	x10, x1, #2, #32
	sbfiz	x9, x1, #3, #32
	lsr	x13, x1, #32
	ldr	w10, [x11, x10]
	ldr	w11, [x12, x9]
	cmp	w10, w13
	ccmp	w11, #0, #4, eq
	cset	w10, ne
	strb	w10, [x3]
	b.eq	.LBB21_35
	ldr	x8, [x8, #8]
	adrp	x10, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	x8, [x8, x9]
	ldr	x9, [x10, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x9, [x9]
	sbfiz	x10, x8, #4, #32
	ldr	x9, [x9, x10]
	ldr	x10, [x0, #8]
	cmp	x10, x9
	b.ne	.LBB21_20
	ldrh	w10, [x0, #22]
	mov	w9, #65535
	bics	wzr, w9, w10
	b.eq	.LBB21_35
.LBB21_8:
	lsl	x9, x8, #32
	adrp	x10, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	lsr	x11, x8, #32
	ldr	x10, [x10, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrh	w12, [x0, #20]
	mov	w8, w8
	asr	x9, x9, #35
	ubfiz	x8, x8, #14, #6
	and	x9, x9, #0xfffffffffffffff8
	ldr	x9, [x10, x9]
	ldrsw	x10, [x0, #16]
	smaddl	x10, w12, w11, x10
	add	x8, x9, x8
	add	x8, x8, x10
	adds	x8, x8, #64
	b.eq	.LBB21_35
	ldr	q0, [x8]
	mov	w0, #1
	str	q0, [x2]
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
.LBB21_10:
	mov	w0, wzr
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
.LBB21_11:
	ldrsh	x9, [x0, #22]
	ldr	x11, [x8, #112]
	ldr	w12, [x8, #168]
	tbnz	x9, #63, .LBB21_14
	cmp	w12, w9
	b.le	.LBB21_14
	ldr	w13, [x11, x9, lsl #2]
	cmp	w13, w10
	b.eq	.LBB21_30
.LBB21_14:
	cbz	w12, .LBB21_18
	mov	w9, wzr
.LBB21_16:
	ldr	w13, [x11, w9, sxtw #2]
	cmp	w13, w10
	b.eq	.LBB21_29
	add	w9, w9, #1
	cmp	w12, w9
	b.ne	.LBB21_16
.LBB21_18:
	mov	w9, #65535
	strh	w9, [x0, #22]
.LBB21_19:
	mov	w10, wzr
	str	wzr, [x0, #16]
	mov	w9, #65535
	b	.LBB21_31
.LBB21_20:
	ldrsh	x10, [x0, #22]
	ldr	w11, [x0, #40]
	ldr	x12, [x9, #112]
	ldr	w13, [x9, #168]
	tbnz	x10, #63, .LBB21_23
	cmp	w13, w10
	b.le	.LBB21_23
	ldr	w14, [x12, x10, lsl #2]
	cmp	w14, w11
	b.eq	.LBB21_33
.LBB21_23:
	cbz	w13, .LBB21_27
	mov	w10, wzr
.LBB21_25:
	ldr	w14, [x12, w10, sxtw #2]
	cmp	w14, w11
	b.eq	.LBB21_32
	add	w10, w10, #1
	cmp	w13, w10
	b.ne	.LBB21_25
.LBB21_27:
	mov	w10, #65535
	strh	w10, [x0, #22]
.LBB21_28:
	mov	w11, wzr
	str	wzr, [x0, #16]
	mov	w10, #65535
	b	.LBB21_34
.LBB21_29:
	mov	w10, #65535
	strh	w9, [x0, #22]
	bics	wzr, w10, w9
	b.eq	.LBB21_19
.LBB21_30:
	mov	w10, w9
	ldr	x11, [x8, #224]
	sxth	x10, w10
	ldr	w11, [x11, x10, lsl #2]
	str	w11, [x0, #16]
	ldr	x11, [x8, #232]
	ldrh	w10, [x11, x10, lsl #1]
.LBB21_31:
	strh	w10, [x0, #20]
	str	x8, [x0, #8]
	b	.LBB21_4
.LBB21_32:
	mov	w11, #65535
	strh	w10, [x0, #22]
	bics	wzr, w11, w10
	b.eq	.LBB21_28
.LBB21_33:
	mov	w11, w10
	ldr	x12, [x9, #224]
	sxth	x11, w11
	ldr	w12, [x12, x11, lsl #2]
	str	w12, [x0, #16]
	ldr	x12, [x9, #232]
	ldrh	w11, [x12, x11, lsl #1]
.LBB21_34:
	strh	w11, [x0, #20]
	str	x9, [x0, #8]
	mov	w9, #65535
	bics	wzr, w9, w10
	b.ne	.LBB21_8
.LBB21_35:
	movi	v0.2d, #0000000000000000
	mov	w0, wzr
	str	q0, [x2]
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.globl	"__xmm@000000000000cccc0000000000000000"
	.section	.rdata,"dr",discard,"__xmm@000000000000cccc0000000000000000"
	.p2align	4, 0x0
"__xmm@000000000000cccc0000000000000000":
	.word	0
	.word	0
	.word	52428
	.word	0
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #96
	.seh_stackalloc	96
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	str	x23, [sp, #64]
	.seh_save_reg	x23, 64
	stp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	add	x29, sp, #72
	.seh_add_fp	72
	.seh_endprologue
	ldr	x8, [x0, #72]
	mov	x20, x1
	cbz	x8, .LBB22_2
	ldr	x9, [x8]
	cmp	x9, x20
	b.eq	.LBB22_11
.LBB22_2:
	ldr	x8, [x0, #64]
	cbz	x8, .LBB22_5
.LBB22_3:
	ldr	x9, [x8]
	cmp	x9, x20
	b.eq	.LBB22_10
	ldr	x8, [x8, #16]
	cbnz	x8, .LBB22_3
.LBB22_5:
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	mov	x21, x0
	mov	w0, #24
	ldr	x22, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	w1, #64
	mov	w2, #4
	mov	w3, #1
	mov	x4, xzr
	blr	x22
	cbz	x0, .LBB22_17
	movi	v0.2d, #0000000000000000
	mov	x19, x0
	str	x20, [x0]
	mov	w1, #64
	mov	w2, #4
	mov	w3, #1
	mov	x4, xzr
	mov	w23, #4
	stur	q0, [x0, #8]
	mov	w0, #32
	blr	x22
	cbz	x0, .LBB22_12
	movi	v0.2d, #0000000000000000
	movi	v1.2d, #0000000000000000
	mov	w8, w23
	mov	w9, #52428
	str	wzr, [x0, #28]
	and	w10, w23, #0xffff
	lsr	w11, w23, #16
	mov	x20, x0
	cmp	w10, #64
	stur	q1, [x0, #4]
	stur	d0, [x0, #20]
	str	x0, [x19, #8]
	str	w23, [x0]
	stp	q1, q1, [sp]
	str	w9, [sp, #8]
	mov	w9, #20
	str	w9, [sp, #16]
	mov	w9, #6
	strb	w9, [sp, #24]
	strh	w23, [sp, #12]
	strh	w11, [sp, #14]
	b.lo	.LBB22_13
	cmp	w8, #16, lsl #12
	b.hs	.LBB22_14
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
	b	.LBB22_15
.LBB22_10:
	str	x8, [x0, #72]
.LBB22_11:
	ldr	x0, [x8, #8]
	b	.LBB22_17
.LBB22_12:
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	mov	x0, x19
	mov	w1, #4
	ldr	x8, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	str	xzr, [x19, #8]
	blr	x8
	mov	x0, xzr
	b	.LBB22_17
.LBB22_13:
	ldr	w8, [sp, #12]
	mov	w0, #1048560
	mov	w1, #64
	mov	w3, #1
	mov	x4, xzr
	and	w9, w8, #0xffff
	tst	w8, #0xffc0
	mov	w8, #4
	csel	w2, w9, w8, eq
	blr	x22
	b	.LBB22_16
.LBB22_14:
	and	x8, x8, #0xffff
.LBB22_15:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	ldr	x0, [sp]
.LBB22_16:
	adrp	x8, "__xmm@000000000000cccc0000000000000000"
	str	x0, [x20, #8]
	ldr	q0, [x8, :lo12:"__xmm@000000000000cccc0000000000000000"]
	str	q0, [x20, #16]
	ldr	x8, [x21, #64]
	str	x8, [x19, #16]
	stp	x19, x19, [x21, #64]
	ldr	x0, [x19, #8]
.LBB22_17:
	.seh_startepilogue
	ldp	x29, x30, [sp, #72]
	.seh_save_fplr	72
	ldr	x23, [sp, #64]
	.seh_save_reg	x23, 64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #96
	.seh_stackalloc	96
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #176
	.seh_stackalloc	176
	stp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	stp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	stp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	stp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	stp	x27, x28, [sp, #144]
	.seh_save_regp	x27, 144
	stp	x29, x30, [sp, #160]
	.seh_save_fplr	160
	add	x29, sp, #160
	.seh_add_fp	160
	.seh_endprologue
	ldr	w20, [x29, #32]
	ldr	w22, [x29, #16]
	mov	w23, w6
	mov	x24, x5
	mov	x25, x4
	mov	w21, w3
	mov	x27, x2
	mov	w26, w1
	mov	x19, x0
	mov	w28, wzr
	str	x7, [sp]
	cbz	x5, .LBB23_9
	cmp	w23, #1
	b.lt	.LBB23_9
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	cmp	w23, #2
	mov	w9, w23
	ldr	x8, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x8, [x8]
	add	x8, x8, #8
	b.hs	.LBB23_4
	mov	x10, xzr
	mov	w28, wzr
	b	.LBB23_7
.LBB23_4:
	and	x10, x9, #0x7ffffffe
	mov	w11, wzr
	mov	w12, wzr
	add	x13, x24, #16
	mov	x14, x10
.LBB23_5:
	ldursw	x15, [x13, #-16]
	ldrsw	x16, [x13]
	subs	x14, x14, #2
	add	x13, x13, #32
	lsl	x15, x15, #4
	lsl	x16, x16, #4
	ldr	w15, [x8, x15]
	ldr	w16, [x8, x16]
	add	w11, w15, w11
	add	w12, w16, w12
	b.ne	.LBB23_5
	cmp	x10, x9
	add	w28, w12, w11
	b.eq	.LBB23_9
.LBB23_7:
	add	x11, x24, x10, lsl #4
	sub	x9, x9, x10
.LBB23_8:
	ldrsw	x10, [x11], #16
	subs	x9, x9, #1
	lsl	x10, x10, #4
	ldr	w10, [x8, x10]
	add	w28, w10, w28
	b.ne	.LBB23_8
.LBB23_9:
	mov	w9, wzr
	add	x10, x19, #80
	mov	w8, #1
	casal	w9, w8, [x10]
	dmb	ish
	cmp	w9, #0
	b.eq	.LBB23_12
.LBB23_10:
	ldr	w9, [x19, #80]
	cmp	w9, #1
	b.eq	.LBB23_10
	mov	w9, wzr
	add	x10, x19, #80
	casal	w9, w8, [x10]
	dmb	ish
	cmp	w9, #0
	b.ne	.LBB23_10
.LBB23_12:
	mov	x0, x19
	mov	w1, w26
	mov	w2, w28
	mov	w3, w22
	mov	w4, w20
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	tbz	w0, #0, .LBB23_47
	cbz	w21, .LBB23_24
	mov	x0, x19
	mov	x1, x27
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	cbz	x0, .LBB23_24
	ldr	w12, [x0, #28]
	cbz	w12, .LBB23_24
	ldr	w13, [x0, #16]
	ldr	w8, [x0, #24]
	sub	w9, w8, w13
	cmp	w9, #0
	csel	w10, wzr, w8, gt
	sub	w11, w13, w10
	mov	w10, #20
	smull	x14, w11, w10
	ldr	x11, [x0, #8]
	ldr	w14, [x11, x14]
	subs	w14, w21, w14
	b.mi	.LBB23_19
	cmp	w14, w12
	b.ge	.LBB23_19
	cmp	w14, w9
	add	w14, w14, w13
	csel	w15, wzr, w8, lt
	sub	w14, w14, w15
	mov	w15, #20
	smaddl	x14, w14, w15, x11
	ldr	w15, [x14]
	cmp	w15, w21
	b.eq	.LBB23_23
.LBB23_19:
	sub	w12, w12, #1
	add	w13, w12, w13
.LBB23_20:
	tbnz	w12, #31, .LBB23_24
	cmp	w12, w9
	sub	w12, w12, #1
	csel	w14, wzr, w8, lt
	sub	w14, w13, w14
	sub	w13, w13, #1
	smull	x14, w14, w10
	ldr	w15, [x11, x14]
	cmp	w15, w21
	b.ne	.LBB23_20
	add	x14, x11, x14
.LBB23_23:
	ldur	d0, [x14, #4]
	ldrh	w8, [x14, #12]
	ldrh	w9, [x14, #14]
	ldr	w10, [x14, #16]
	b	.LBB23_25
.LBB23_24:
	mov	w8, wzr
	mov	w9, wzr
	movi	v0.2d, #0000000000000000
	mov	w10, wzr
.LBB23_25:
	ldr	x11, [x19, #88]
	ldr	x13, [x27, #376]
	adrp	x14, "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"
	add	x12, x11, #1
	str	x12, [x19, #88]
	ldr	x12, [x14, :lo12:"Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	ldr	w12, [x12]
	strh	w8, [sp, #40]
	strh	w9, [sp, #42]
	ldp	w8, w9, [x25, #8]
	str	d0, [sp, #32]
	ldr	d0, [x25]
	ldr	x25, [x29, #24]
	str	x11, [sp, #8]
	stp	w8, w9, [sp, #56]
	ldp	w8, w9, [x19, #56]
	str	x13, [sp, #24]
	str	w10, [sp, #44]
	add	w9, w9, #72
	stp	w26, w12, [sp, #16]
	cmp	w9, w8
	str	d0, [sp, #48]
	stp	w28, w22, [sp, #64]
	stp	w20, wzr, [sp, #72]
	b.le	.LBB23_27
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	mov	x0, x19
	mov	x1, x24
	mov	w2, w23
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x24, [sp]
	cbnz	x24, .LBB23_31
	b	.LBB23_39
.LBB23_27:
	ldp	w9, w10, [x19, #48]
	sxtw	x10, w10
	cmp	w9, w10
	csel	w8, w9, w8, gt
	sub	w26, w8, w10
	ldr	x8, [x19, #40]
	cmp	w26, #71
	add	x0, x8, x10
	b.gt	.LBB23_29
	sxtw	x21, w26
	add	x1, sp, #8
	mov	w3, wzr
	add	x27, sp, #8
	mov	x2, x21
	bl	burst.memcpy.inline.ARMV9A.i64
	mov	w8, #72
	ldr	x0, [x19, #40]
	add	x1, x27, x21
	sub	w8, w8, w26
	mov	w3, wzr
	sxtw	x2, w8
	bl	burst.memcpy.inline.ARMV9A.i64
	b	.LBB23_30
.LBB23_29:
	ldur	q0, [sp, #8]
	str	q0, [x0]
	ldur	q0, [sp, #24]
	str	q0, [x0, #16]
	ldur	q0, [sp, #40]
	str	q0, [x0, #32]
	ldur	q0, [sp, #56]
	str	q0, [x0, #48]
	ldr	d0, [sp, #72]
	str	d0, [x0, #64]
.LBB23_30:
	ldp	w8, w9, [x19, #52]
	add	w8, w8, #72
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, #72
	str	w8, [x19, #52]
	str	w9, [x19, #60]
	mov	x0, x19
	mov	x1, x24
	mov	w2, w23
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x24, [sp]
	cbz	x24, .LBB23_39
.LBB23_31:
	cmp	w22, #1
	b.lt	.LBB23_39
	lsl	w21, w22, #2
	cmp	w21, #1
	b.lt	.LBB23_39
	ldp	w9, w8, [x19, #56]
	add	w8, w8, w21
	cmp	w8, w9
	b.le	.LBB23_35
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	b	.LBB23_39
.LBB23_35:
	ldp	w10, w8, [x19, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x19, #40]
	subs	w23, w21, w10
	b.le	.LBB23_37
	sxtw	x22, w10
	add	x0, x9, x8
	mov	x1, x24
	mov	w3, wzr
	mov	x2, x22
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x19, #40]
	sxtw	x2, w23
	add	x1, x24, x22
	b	.LBB23_38
.LBB23_37:
	add	x0, x9, x8
	mov	x1, x24
	mov	x2, x21
.LBB23_38:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x19, #52]
	add	w8, w8, w21
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w21
	str	w8, [x19, #52]
	str	w9, [x19, #60]
.LBB23_39:
	cbz	x25, .LBB23_47
	cmp	w20, #1
	b.lt	.LBB23_47
	ldp	w9, w8, [x19, #56]
	add	w8, w8, w20
	cmp	w8, w9
	b.le	.LBB23_43
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	b	.LBB23_47
.LBB23_43:
	ldp	w10, w8, [x19, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x19, #40]
	subs	w22, w20, w10
	b.le	.LBB23_45
	sxtw	x21, w10
	add	x0, x9, x8
	mov	x1, x25
	mov	w3, wzr
	mov	x2, x21
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x19, #40]
	sxtw	x2, w22
	add	x1, x25, x21
	b	.LBB23_46
.LBB23_45:
	add	x0, x9, x8
	mov	x1, x25
	mov	x2, x20
.LBB23_46:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x19, #52]
	add	w8, w8, w20
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w20
	str	w8, [x19, #52]
	str	w9, [x19, #60]
.LBB23_47:
	str	wzr, [x19, #80]
	.seh_startepilogue
	ldp	x29, x30, [sp, #160]
	.seh_save_fplr	160
	ldp	x27, x28, [sp, #144]
	.seh_save_regp	x27, 144
	ldp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	ldp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	ldp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	ldp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	add	sp, sp, #176
	.seh_stackalloc	176
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #176
	.seh_stackalloc	176
	stp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	stp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	stp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	stp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	stp	x27, x28, [sp, #144]
	.seh_save_regp	x27, 144
	stp	x29, x30, [sp, #160]
	.seh_save_fplr	160
	add	x29, sp, #160
	.seh_add_fp	160
	.seh_endprologue
	mov	w9, wzr
	add	x10, x0, #80
	mov	w8, #1
	casal	w9, w8, [x10]
	ldr	w20, [x29, #32]
	ldr	w22, [x29, #16]
	mov	x21, x7
	mov	w24, w6
	mov	x23, x5
	mov	x19, x0
	mov	x25, x4
	mov	w28, w3
	mov	x27, x2
	mov	w26, w1
	cmp	w9, #0
	dmb	ish
	b.eq	.LBB24_3
.LBB24_1:
	ldr	w9, [x19, #80]
	cmp	w9, #1
	b.eq	.LBB24_1
	mov	w9, wzr
	add	x10, x19, #80
	casal	w9, w8, [x10]
	cmp	w9, #0
	dmb	ish
	b.ne	.LBB24_1
.LBB24_3:
	mov	x0, x19
	mov	w1, w26
	mov	w2, w24
	mov	w3, w22
	mov	w4, w20
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	tbz	w0, #0, .LBB24_47
	cbz	w28, .LBB24_15
	mov	x0, x19
	mov	x1, x27
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemVersionBuffer(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store) -> Unity.Entities.LowLevel.Unsafe.UnsafeCircularBuffer`1[[Unity.Entities.EntitiesJournaling+JournalingState+SystemVersionHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	cbz	x0, .LBB24_15
	ldr	w12, [x0, #28]
	cbz	w12, .LBB24_15
	ldr	w13, [x0, #16]
	ldr	w8, [x0, #24]
	sub	w9, w8, w13
	cmp	w9, #0
	csel	w10, wzr, w8, gt
	sub	w11, w13, w10
	mov	w10, #20
	smull	x14, w11, w10
	ldr	x11, [x0, #8]
	ldr	w14, [x11, x14]
	subs	w14, w28, w14
	b.mi	.LBB24_10
	cmp	w14, w12
	b.ge	.LBB24_10
	cmp	w14, w9
	add	w14, w14, w13
	csel	w15, wzr, w8, lt
	sub	w14, w14, w15
	mov	w15, #20
	smaddl	x14, w14, w15, x11
	ldr	w15, [x14]
	cmp	w15, w28
	b.eq	.LBB24_14
.LBB24_10:
	sub	w12, w12, #1
	add	w13, w12, w13
.LBB24_11:
	tbnz	w12, #31, .LBB24_15
	cmp	w12, w9
	sub	w12, w12, #1
	csel	w14, wzr, w8, lt
	sub	w14, w13, w14
	sub	w13, w13, #1
	smull	x14, w14, w10
	ldr	w15, [x11, x14]
	cmp	w15, w28
	b.ne	.LBB24_11
	add	x14, x11, x14
.LBB24_14:
	ldur	d0, [x14, #4]
	ldrh	w8, [x14, #12]
	ldrh	w9, [x14, #14]
	ldr	w10, [x14, #16]
	b	.LBB24_16
.LBB24_15:
	mov	w8, wzr
	mov	w9, wzr
	movi	v0.2d, #0000000000000000
	mov	w10, wzr
.LBB24_16:
	ldr	x11, [x19, #88]
	ldr	x13, [x27, #376]
	adrp	x14, "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"
	add	x12, x11, #1
	str	x12, [x19, #88]
	ldr	x12, [x14, :lo12:"Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	ldr	w12, [x12]
	strh	w8, [sp, #40]
	strh	w9, [sp, #42]
	ldp	w8, w9, [x25, #8]
	str	d0, [sp, #32]
	ldr	d0, [x25]
	str	x11, [sp, #8]
	stp	w8, w9, [sp, #56]
	ldp	w8, w9, [x19, #56]
	str	x13, [sp, #24]
	str	w10, [sp, #44]
	add	w9, w9, #72
	stp	w26, w12, [sp, #16]
	cmp	w9, w8
	str	d0, [sp, #48]
	stp	w24, w22, [sp, #64]
	stp	w20, wzr, [sp, #72]
	b.le	.LBB24_18
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	ldr	x25, [x29, #24]
	cbnz	x23, .LBB24_22
	b	.LBB24_30
.LBB24_18:
	ldp	w9, w10, [x19, #48]
	sxtw	x10, w10
	cmp	w9, w10
	csel	w8, w9, w8, gt
	sub	w26, w8, w10
	ldr	x8, [x19, #40]
	cmp	w26, #71
	add	x0, x8, x10
	b.gt	.LBB24_20
	sxtw	x25, w26
	add	x1, sp, #8
	mov	w3, wzr
	add	x27, sp, #8
	mov	x2, x25
	bl	burst.memcpy.inline.ARMV9A.i64
	mov	w8, #72
	ldr	x0, [x19, #40]
	add	x1, x27, x25
	sub	w8, w8, w26
	mov	w3, wzr
	sxtw	x2, w8
	bl	burst.memcpy.inline.ARMV9A.i64
	b	.LBB24_21
.LBB24_20:
	ldur	q0, [sp, #8]
	str	q0, [x0]
	ldur	q0, [sp, #24]
	str	q0, [x0, #16]
	ldur	q0, [sp, #40]
	str	q0, [x0, #32]
	ldur	q0, [sp, #56]
	str	q0, [x0, #48]
	ldr	d0, [sp, #72]
	str	d0, [x0, #64]
.LBB24_21:
	ldp	w8, w9, [x19, #52]
	add	w8, w8, #72
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, #72
	str	w8, [x19, #52]
	str	w9, [x19, #60]
	ldr	x25, [x29, #24]
	cbz	x23, .LBB24_30
.LBB24_22:
	cmp	w24, #1
	b.lt	.LBB24_30
	lsl	w24, w24, #3
	cmp	w24, #1
	b.lt	.LBB24_30
	ldp	w9, w8, [x19, #56]
	add	w8, w8, w24
	cmp	w8, w9
	b.le	.LBB24_26
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.Entity* entities, int entityCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.Entity* entities, int entityCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	b	.LBB24_30
.LBB24_26:
	ldp	w10, w8, [x19, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x19, #40]
	subs	w27, w24, w10
	b.le	.LBB24_28
	sxtw	x26, w10
	add	x0, x9, x8
	mov	x1, x23
	mov	w3, wzr
	mov	x2, x26
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x19, #40]
	sxtw	x2, w27
	add	x1, x23, x26
	b	.LBB24_29
.LBB24_28:
	add	x0, x9, x8
	mov	x1, x23
	mov	x2, x24
.LBB24_29:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x19, #52]
	add	w8, w8, w24
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w24
	str	w8, [x19, #52]
	str	w9, [x19, #60]
.LBB24_30:
	cbz	x21, .LBB24_39
	cmp	w22, #1
	b.lt	.LBB24_39
	lsl	w22, w22, #2
	cmp	w22, #1
	b.lt	.LBB24_39
	ldp	w9, w8, [x19, #56]
	add	w8, w8, w22
	cmp	w8, w9
	b.le	.LBB24_35
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	b	.LBB24_39
.LBB24_35:
	ldp	w10, w8, [x19, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x19, #40]
	subs	w24, w22, w10
	b.le	.LBB24_37
	sxtw	x23, w10
	add	x0, x9, x8
	mov	x1, x21
	mov	w3, wzr
	mov	x2, x23
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x19, #40]
	sxtw	x2, w24
	add	x1, x21, x23
	b	.LBB24_38
.LBB24_37:
	add	x0, x9, x8
	mov	x1, x21
	mov	x2, x22
.LBB24_38:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x19, #52]
	add	w8, w8, w22
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w22
	str	w8, [x19, #52]
	str	w9, [x19, #60]
.LBB24_39:
	cbz	x25, .LBB24_47
	cmp	w20, #1
	b.lt	.LBB24_47
	ldp	w9, w8, [x19, #56]
	add	w8, w8, w20
	cmp	w8, w9
	b.le	.LBB24_43
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	b	.LBB24_47
.LBB24_43:
	ldp	w10, w8, [x19, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x19, #40]
	subs	w22, w20, w10
	b.le	.LBB24_45
	sxtw	x21, w10
	add	x0, x9, x8
	mov	x1, x25
	mov	w3, wzr
	mov	x2, x21
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x19, #40]
	sxtw	x2, w22
	add	x1, x25, x21
	b	.LBB24_46
.LBB24_45:
	add	x0, x9, x8
	mov	x1, x25
	mov	x2, x20
.LBB24_46:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x19, #52]
	add	w8, w8, w20
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x19, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w20
	str	w8, [x19, #52]
	str	w9, [x19, #60]
.LBB24_47:
	str	wzr, [x19, #80]
	.seh_startepilogue
	ldp	x29, x30, [sp, #160]
	.seh_save_fplr	160
	ldp	x27, x28, [sp, #144]
	.seh_save_regp	x27, 144
	ldp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	ldp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	ldp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	ldp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	add	sp, sp, #176
	.seh_stackalloc	176
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackEntities(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #112
	.seh_stackalloc	112
	stp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	stp	x21, x22, [sp, #32]
	.seh_save_regp	x21, 32
	stp	x23, x24, [sp, #48]
	.seh_save_regp	x23, 48
	stp	x25, x26, [sp, #64]
	.seh_save_regp	x25, 64
	stp	x27, x28, [sp, #80]
	.seh_save_regp	x27, 80
	stp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	add	x29, sp, #96
	.seh_add_fp	96
	.seh_endprologue
	cbz	x1, .LBB25_12
	cmp	w2, #1
	b.lt	.LBB25_12
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x24, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	adrp	x27, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	ldr	x21, [x24, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x25, [x27, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	x19, x1
	mov	x20, x0
	mov	w28, w2
	str	x8, [sp, #8]
	b	.LBB25_5
.LBB25_3:
	ldr	x8, [sp, #8]
	mov	x0, xzr
	mov	w1, #2
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.ArchetypeChunk* chunks, int chunkCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0065"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.ArchetypeChunk* chunks, int chunkCount) -> void_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0065"+2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	ldr	x21, [x24, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x25, [x27, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
.LBB25_4:
	subs	x28, x28, #1
	b.eq	.LBB25_12
.LBB25_5:
	ldrsw	x8, [x19], #16
	ldr	x9, [x21]
	sbfiz	x10, x8, #4, #32
	asr	x11, x8, #3
	ubfiz	x12, x8, #14, #6
	ldr	x10, [x9, x10]
	and	x11, x11, #0xfffffffffffffff8
	ldr	x11, [x25, x11]
	ldr	x10, [x10, #224]
	add	x11, x11, x12
	ldrsw	x10, [x10]
	add	x10, x11, x10
	adds	x23, x10, #64
	b.eq	.LBB25_13
	add	x8, x9, x8, lsl #4
	ldr	w8, [x8, #8]
	lsl	w22, w8, #3
	cmp	w22, #1
	b.lt	.LBB25_4
	ldp	w9, w8, [x20, #56]
	add	w8, w8, w22
	cmp	w8, w9
	b.gt	.LBB25_3
	ldp	w10, w8, [x20, #48]
	sxtw	x8, w8
	cmp	w10, w8
	csel	w9, w10, w9, gt
	sub	w10, w9, w8
	ldr	x9, [x20, #40]
	subs	w26, w22, w10
	b.le	.LBB25_10
	mov	x27, x24
	sxtw	x24, w10
	add	x0, x9, x8
	mov	x1, x23
	mov	w3, wzr
	mov	x2, x24
	bl	burst.memcpy.inline.ARMV9A.i64
	ldr	x0, [x20, #40]
	sxtw	x2, w26
	add	x1, x23, x24
	mov	x24, x27
	adrp	x27, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	b	.LBB25_11
.LBB25_10:
	add	x0, x9, x8
	mov	x1, x23
	mov	x2, x22
.LBB25_11:
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	ldp	w8, w9, [x20, #52]
	add	w8, w8, w22
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x20, #60]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, w22
	str	w8, [x20, #52]
	str	w9, [x20, #60]
	b	.LBB25_4
.LBB25_12:
	.seh_startepilogue
	ldp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	ldp	x27, x28, [sp, #80]
	.seh_save_regp	x27, 80
	ldp	x25, x26, [sp, #64]
	.seh_save_regp	x25, 64
	ldp	x23, x24, [sp, #48]
	.seh_save_regp	x23, 48
	ldp	x21, x22, [sp, #32]
	.seh_save_regp	x21, 32
	ldp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	add	sp, sp, #112
	.seh_stackalloc	112
	.seh_endepilogue
	ret
.LBB25_13:
	adrp	x8, burst_abort_Ptr
	adrp	x0, burst.string.768c1a52ff82ef88cabe676b503130fa
	add	x0, x0, :lo12:burst.string.768c1a52ff82ef88cabe676b503130fa
	ldr	x8, [x8, :lo12:burst_abort_Ptr]
	adrp	x1, burst.string.1c3b0f8201e0c56284796afecfd7f5fb
	add	x1, x1, :lo12:burst.string.1c3b0f8201e0c56284796afecfd7f5fb
	blr	x8
	brk	#0xf000
	brk	#0x1
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.globl	"__xmm@696c616e72756f4a7365697469746e45"
	.section	.rdata,"dr",discard,"__xmm@696c616e72756f4a7365697469746e45"
	.p2align	4, 0x0
"__xmm@696c616e72756f4a7365697469746e45":
	.byte	69
	.byte	110
	.byte	116
	.byte	105
	.byte	116
	.byte	105
	.byte	101
	.byte	115
	.byte	74
	.byte	111
	.byte	117
	.byte	114
	.byte	110
	.byte	97
	.byte	108
	.byte	105
	.globl	"__xmm@6f70206f742064656c696146203a676e"
	.section	.rdata,"dr",discard,"__xmm@6f70206f742064656c696146203a676e"
	.p2align	4, 0x0
"__xmm@6f70206f742064656c696146203a676e":
	.byte	110
	.byte	103
	.byte	58
	.byte	32
	.byte	70
	.byte	97
	.byte	105
	.byte	108
	.byte	101
	.byte	100
	.byte	32
	.byte	116
	.byte	111
	.byte	32
	.byte	112
	.byte	111
	.globl	"__real@20746e6f72662070"
	.section	.rdata,"dr",discard,"__real@20746e6f72662070"
	.p2align	3, 0x0
"__real@20746e6f72662070":
	.byte	112
	.byte	32
	.byte	102
	.byte	114
	.byte	111
	.byte	110
	.byte	116
	.byte	32
	.globl	"__xmm@65726f747320746f6e6e6143203a676e"
	.section	.rdata,"dr",discard,"__xmm@65726f747320746f6e6e6143203a676e"
	.p2align	4, 0x0
"__xmm@65726f747320746f6e6e6143203a676e":
	.byte	110
	.byte	103
	.byte	58
	.byte	32
	.byte	67
	.byte	97
	.byte	110
	.byte	110
	.byte	111
	.byte	116
	.byte	32
	.byte	115
	.byte	116
	.byte	111
	.byte	114
	.byte	101
	.globl	"__xmm@452e7365697469746e452e7974696e55"
	.section	.rdata,"dr",discard,"__xmm@452e7365697469746e452e7974696e55"
	.p2align	4, 0x0
"__xmm@452e7365697469746e452e7974696e55":
	.byte	85
	.byte	110
	.byte	105
	.byte	116
	.byte	121
	.byte	46
	.byte	69
	.byte	110
	.byte	116
	.byte	105
	.byte	116
	.byte	105
	.byte	101
	.byte	115
	.byte	46
	.byte	69
	.globl	"__xmm@6e696c616e72756f4a7365697469746e"
	.section	.rdata,"dr",discard,"__xmm@6e696c616e72756f4a7365697469746e"
	.p2align	4, 0x0
"__xmm@6e696c616e72756f4a7365697469746e":
	.byte	110
	.byte	116
	.byte	105
	.byte	116
	.byte	105
	.byte	101
	.byte	115
	.byte	74
	.byte	111
	.byte	117
	.byte	114
	.byte	110
	.byte	97
	.byte	108
	.byte	105
	.byte	110
	.globl	"__real@64726f6365522e67"
	.section	.rdata,"dr",discard,"__real@64726f6365522e67"
	.p2align	3, 0x0
"__real@64726f6365522e67":
	.byte	103
	.byte	46
	.byte	82
	.byte	101
	.byte	99
	.byte	111
	.byte	114
	.byte	100
	.globl	"__real@0000000065707954"
	.section	.rdata,"dr",discard,"__real@0000000065707954"
	.p2align	3, 0x0
"__real@0000000065707954":
	.byte	84
	.byte	121
	.byte	112
	.byte	101
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.globl	"__xmm@6e20726566667562202c746e65766520"
	.section	.rdata,"dr",discard,"__xmm@6e20726566667562202c746e65766520"
	.p2align	4, 0x0
"__xmm@6e20726566667562202c746e65766520":
	.byte	32
	.byte	101
	.byte	118
	.byte	101
	.byte	110
	.byte	116
	.byte	44
	.byte	32
	.byte	98
	.byte	117
	.byte	102
	.byte	102
	.byte	101
	.byte	114
	.byte	32
	.byte	110
	.globl	"__xmm@2e6867756f6e6520656772616c20746f"
	.section	.rdata,"dr",discard,"__xmm@2e6867756f6e6520656772616c20746f"
	.p2align	4, 0x0
"__xmm@2e6867756f6e6520656772616c20746f":
	.byte	111
	.byte	116
	.byte	32
	.byte	108
	.byte	97
	.byte	114
	.byte	103
	.byte	101
	.byte	32
	.byte	101
	.byte	110
	.byte	111
	.byte	117
	.byte	103
	.byte	104
	.byte	46
	.globl	"__xmm@697469746e65206573616572636e4920"
	.section	.rdata,"dr",discard,"__xmm@697469746e65206573616572636e4920"
	.p2align	4, 0x0
"__xmm@697469746e65206573616572636e4920":
	.byte	32
	.byte	73
	.byte	110
	.byte	99
	.byte	114
	.byte	101
	.byte	97
	.byte	115
	.byte	101
	.byte	32
	.byte	101
	.byte	110
	.byte	116
	.byte	105
	.byte	116
	.byte	105
	.globl	"__xmm@6f7420676e696c616e72756f6a207365"
	.section	.rdata,"dr",discard,"__xmm@6f7420676e696c616e72756f6a207365"
	.p2align	4, 0x0
"__xmm@6f7420676e696c616e72756f6a207365":
	.byte	101
	.byte	115
	.byte	32
	.byte	106
	.byte	111
	.byte	117
	.byte	114
	.byte	110
	.byte	97
	.byte	108
	.byte	105
	.byte	110
	.byte	103
	.byte	32
	.byte	116
	.byte	111
	.globl	"__xmm@7270206e692079726f6d656d206c6174"
	.section	.rdata,"dr",discard,"__xmm@7270206e692079726f6d656d206c6174"
	.p2align	4, 0x0
"__xmm@7270206e692079726f6d656d206c6174":
	.byte	116
	.byte	97
	.byte	108
	.byte	32
	.byte	109
	.byte	101
	.byte	109
	.byte	111
	.byte	114
	.byte	121
	.byte	32
	.byte	105
	.byte	110
	.byte	32
	.byte	112
	.byte	114
	.globl	"__real@65636e6572656665"
	.section	.rdata,"dr",discard,"__real@65636e6572656665"
	.p2align	3, 0x0
"__real@65636e6572656665":
	.byte	101
	.byte	102
	.byte	101
	.byte	114
	.byte	101
	.byte	110
	.byte	99
	.byte	101
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #240
	.seh_stackalloc	240
	stp	x19, x20, [sp, #192]
	.seh_save_regp	x19, 192
	str	x21, [sp, #208]
	.seh_save_reg	x21, 208
	stp	x29, x30, [sp, #216]
	.seh_save_fplr	216
	add	x29, sp, #216
	.seh_add_fp	216
	.seh_endprologue
	lsl	w8, w2, #3
	ldr	w11, [x0, #56]
	add	w8, w8, w3, lsl #2
	add	w8, w8, w4
	add	w9, w8, #72
	cmp	w9, w11
	b.le	.LBB26_2
	movi	v0.2d, #0000000000000000
	movi	v1.2d, #0000000000000000
	add	x8, sp, #16
	adrp	x9, "__xmm@696c616e72756f4a7365697469746e45"
	mov	w10, #32
	orr	x2, x8, #0x2
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	strb	wzr, [sp, #186]
	stp	q0, q0, [sp, #16]
	stp	q0, q0, [sp, #48]
	stp	q0, q0, [sp, #80]
	stp	q0, q0, [sp, #112]
	stp	q0, q0, [x8, #128]
	ldr	q0, [x9, :lo12:"__xmm@696c616e72756f4a7365697469746e45"]
	adrp	x9, "__xmm@65726f747320746f6e6e6143203a676e"
	str	d1, [sp, #176]
	ldr	q1, [x9, :lo12:"__xmm@65726f747320746f6e6e6143203a676e"]
	adrp	x9, "__xmm@452e7365697469746e452e7974696e55"
	stur	q0, [sp, #18]
	ldr	q0, [x9, :lo12:"__xmm@452e7365697469746e452e7974696e55"]
	adrp	x9, "__xmm@6e696c616e72756f4a7365697469746e"
	stur	q1, [sp, #34]
	ldr	q1, [x9, :lo12:"__xmm@6e696c616e72756f4a7365697469746e"]
	adrp	x9, "__real@64726f6365522e67"
	stur	q0, [sp, #51]
	ldr	d0, [x9, :lo12:"__real@64726f6365522e67"]
	adrp	x9, "__real@0000000065707954"
	stur	q1, [sp, #67]
	ldr	d1, [x9, :lo12:"__real@0000000065707954"]
	adrp	x9, "__xmm@6e20726566667562202c746e65766520"
	stur	d0, [sp, #83]
	ldr	q0, [x9, :lo12:"__xmm@6e20726566667562202c746e65766520"]
	adrp	x9, "__xmm@2e6867756f6e6520656772616c20746f"
	stur	s1, [sp, #91]
	ldr	q1, [x9, :lo12:"__xmm@2e6867756f6e6520656772616c20746f"]
	adrp	x9, "__xmm@6f7420676e696c616e72756f6a207365"
	stur	q0, [sp, #95]
	ldr	q0, [x9, :lo12:"__xmm@6f7420676e696c616e72756f6a207365"]
	adrp	x9, "__xmm@7270206e692079726f6d656d206c6174"
	stur	q1, [sp, #111]
	ldr	q1, [x9, :lo12:"__xmm@7270206e692079726f6d656d206c6174"]
	adrp	x9, "__real@65636e6572656665"
	stur	q0, [x8, #127]
	ldr	d0, [x9, :lo12:"__real@65636e6572656665"]
	mov	w9, #46
	strh	w9, [sp, #184]
	mov	w9, #115
	strb	w9, [sp, #183]
	mov	w9, #167
	stur	q1, [x8, #143]
	strb	w10, [sp, #50]
	adrp	x10, "__xmm@697469746e65206573616572636e4920"
	stur	d0, [x8, #159]
	ldr	q0, [x10, :lo12:"__xmm@697469746e65206573616572636e4920"]
	strh	w9, [sp, #16]
	adrp	x9, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	ldr	x9, [x9, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	stur	q0, [sp, #127]
	blr	x9
	b	.LBB26_25
.LBB26_2:
	b.ne	.LBB26_4
	movi	v0.2d, #0000000000000000
	mov	w12, wzr
	mov	w10, wzr
	str	wzr, [x0, #28]
	str	wzr, [x0, #60]
	str	d0, [x0, #16]
	str	d0, [x0, #48]
	ldr	w13, [x0, #24]
	cmp	wzr, w13
	b.ne	.LBB26_5
	b	.LBB26_6
.LBB26_4:
	ldr	w10, [x0, #52]
	ldp	w13, w12, [x0, #24]
	cmp	w12, w13
	b.eq	.LBB26_6
.LBB26_5:
	ldr	w8, [x0, #60]
	sub	w8, w11, w8
	cmp	w8, w9
	b.ge	.LBB26_17
.LBB26_6:
	cmp	w12, #1
	b.lt	.LBB26_12
	ldr	w14, [x0, #16]
	ldr	w17, [x0, #60]
	mov	w8, wzr
	ldr	x15, [x0, #8]
	mov	w3, wzr
	sub	w16, w13, w14
	sub	w17, w11, w17
.LBB26_8:
	cmp	w8, w16
	add	w8, w8, #1
	csel	w1, wzr, w13, lt
	sub	w1, w14, w1
	add	x1, x15, w1, sxtw #3
	ldr	w1, [x1, #4]
	add	w3, w3, w1
	add	w1, w17, w3
	cmp	w1, w9
	b.ge	.LBB26_11
	cmp	w12, w8
	add	w14, w14, #1
	b.ne	.LBB26_8
	mov	w8, w12
.LBB26_11:
	cbnz	w12, .LBB26_13
	b	.LBB26_19
.LBB26_12:
	mov	w8, wzr
	mov	w3, wzr
	cbz	w12, .LBB26_19
.LBB26_13:
	subs	w12, w12, w8
	b.lt	.LBB26_19
	ldr	w14, [x0, #16]
	str	w12, [x0, #28]
	add	w8, w14, w8
	sdiv	w14, w8, w13
	msub	w14, w14, w13, w8
	ldr	w8, [x0, #60]
	and	w15, w13, w14, asr #31
	add	w14, w15, w14
	str	w14, [x0, #16]
	cbz	w8, .LBB26_22
	subs	w8, w8, w3
	b.lt	.LBB26_22
	ldr	w14, [x0, #48]
	str	w8, [x0, #60]
	add	w14, w14, w3
	sdiv	w15, w14, w11
	msub	w14, w15, w11, w14
	and	w11, w11, w14, asr #31
	add	w11, w11, w14
	str	w11, [x0, #48]
.LBB26_17:
	cmp	w12, w13
	b.ne	.LBB26_21
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	adrp	x2, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0153"+2
	add	x2, x2, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0153"+2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x0, xzr
	mov	w1, #2
	mov	x3, xzr
	mov	w4, wzr
	blr	x8
	mov	w0, #1
	b	.LBB26_26
.LBB26_19:
	movi	v0.2d, #0000000000000000
	adrp	x9, "__xmm@696c616e72756f4a7365697469746e45"
	adrp	x10, "__xmm@6f70206f742064656c696146203a676e"
	ldr	q1, [x9, :lo12:"__xmm@696c616e72756f4a7365697469746e45"]
	adrp	x9, "__real@20746e6f72662070"
	add	x1, sp, #8
	mov	w2, #61
	mov	w3, w8
	mov	w4, wzr
	mov	w20, #61
	stp	q0, q0, [sp, #16]
	stur	q1, [sp, #18]
	ldr	d1, [x9, :lo12:"__real@20746e6f72662070"]
	add	x9, sp, #16
	stp	q0, q0, [sp, #48]
	ldr	q0, [x10, :lo12:"__xmm@6f70206f742064656c696146203a676e"]
	orr	x19, x9, #0x2
	mov	w10, #40
	mov	x0, x19
	stur	d1, [sp, #50]
	stur	q0, [sp, #34]
	str	w10, [sp, #8]
	bl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	w21, [sp, #8]
	sub	w8, w20, w21
	cmp	w8, #1
	b.lt	.LBB26_24
	sxtw	x9, w21
	cmp	w8, #9
	mov	w10, #9
	csel	w20, w8, w10, lo
	adrp	x1, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0113.3"+2
	add	x1, x1, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0113.3"+2
	add	x0, x19, x9
	mov	x2, x20
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	add	w21, w20, w21
	str	w21, [sp, #8]
	b	.LBB26_24
.LBB26_21:
	ldr	x8, [x0, #8]
	ldrsw	x11, [x0, #20]
	add	x8, x8, x11, lsl #3
	stp	w10, w9, [x8]
	ldp	w8, w9, [x0, #20]
	add	w8, w8, #1
	sdiv	w10, w8, w9
	msub	w8, w10, w9, w8
	ldr	w10, [x0, #28]
	and	w9, w9, w8, asr #31
	add	w8, w9, w8
	add	w9, w10, #1
	str	w8, [x0, #20]
	str	w9, [x0, #28]
	mov	w0, #1
	b	.LBB26_26
.LBB26_22:
	movi	v0.2d, #0000000000000000
	movi	v1.2d, #0000000000000000
	adrp	x8, "__xmm@696c616e72756f4a7365697469746e45"
	adrp	x9, "__xmm@6f70206f742064656c696146203a676e"
	add	x1, sp, #12
	mov	w2, #69
	mov	w4, wzr
	mov	w20, #69
	stp	q0, q0, [sp, #16]
	stp	q0, q0, [sp, #48]
	ldr	q0, [x8, :lo12:"__xmm@696c616e72756f4a7365697469746e45"]
	adrp	x8, "__real@20746e6f72662070"
	str	d1, [sp, #80]
	ldr	q1, [x9, :lo12:"__xmm@6f70206f742064656c696146203a676e"]
	mov	w9, #40
	stur	q0, [sp, #18]
	ldr	d0, [x8, :lo12:"__real@20746e6f72662070"]
	add	x8, sp, #16
	orr	x19, x8, #0x2
	stur	q1, [sp, #34]
	mov	x0, x19
	stur	d0, [sp, #50]
	str	w9, [sp, #12]
	bl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	w21, [sp, #12]
	sub	w8, w20, w21
	cmp	w8, #1
	b.lt	.LBB26_24
	sxtw	x9, w21
	cmp	w8, #17
	mov	w10, #17
	csel	w20, w8, w10, lo
	adrp	x1, "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0138"+2
	add	x1, x1, :lo12:"Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0138"+2
	add	x0, x19, x9
	mov	x2, x20
	mov	w3, wzr
	bl	burst.memcpy.inline.ARMV9A.i64
	add	w21, w20, w21
	str	w21, [sp, #12]
.LBB26_24:
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"
	mov	x0, xzr
	mov	w1, #2
	ldr	x8, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	x2, x19
	mov	x3, xzr
	mov	w4, wzr
	strb	wzr, [x19, w21, sxtw]
	sturh	w21, [x19, #-2]
	blr	x8
.LBB26_25:
	mov	w0, wzr
.LBB26_26:
	.seh_startepilogue
	ldp	x29, x30, [sp, #216]
	.seh_save_fplr	216
	ldr	x21, [sp, #208]
	.seh_save_reg	x21, 208
	ldp	x19, x20, [sp, #192]
	.seh_save_regp	x19, 192
	add	sp, sp, #240
	.seh_stackalloc	240
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #48
	.seh_stackalloc	48
	stp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	x29, sp, #32
	.seh_add_fp	32
	.seh_endprologue
	adrp	x10, "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"
	ldr	x10, [x10, :lo12:"Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	ldrb	w10, [x10]
	cbz	w10, .LBB27_7
	mov	x8, x3
	cmp	w4, #1
	mov	x9, x6
	ccmp	x3, #0, #0, ge
	ldr	w12, [x29, #24]
	ldr	x10, [x29, #16]
	csel	w6, w4, wzr, ne
	cmp	w7, #1
	mov	w3, w2
	ccmp	x9, #0, #0, ge
	mov	x2, x1
	mov	w1, w0
	csel	w11, w7, wzr, ne
	cmp	w12, #1
	ccmp	x10, #0, #0, ge
	csel	w12, w12, wzr, ne
	subs	w13, w11, #1
	b.lt	.LBB27_6
	cbz	x9, .LBB27_6
	adrp	x11, "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"
	ldr	w14, [x9]
	ldr	x11, [x11, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	ldr	w11, [x11]
	cmp	w14, w11
	b.ne	.LBB27_5
	add	x9, x9, #4
	mov	w11, w13
	b	.LBB27_6
.LBB27_5:
	mov	w11, w7
.LBB27_6:
	adrp	x13, "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"
	mov	x4, x5
	mov	x5, x8
	ldr	x0, [x13, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	x7, x9
	str	w12, [sp, #16]
	str	x10, [sp, #8]
	str	w11, [sp]
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB27_7:
	.seh_startepilogue
	ldp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #48
	.seh_stackalloc	48
	stp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	x29, sp, #32
	.seh_add_fp	32
	.seh_endprologue
	adrp	x10, "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"
	ldr	x10, [x10, :lo12:"Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	ldrb	w10, [x10]
	cbz	w10, .LBB28_7
	mov	x8, x3
	cmp	w4, #1
	mov	x9, x6
	ccmp	x3, #0, #0, ge
	ldr	w12, [x29, #24]
	ldr	x10, [x29, #16]
	csel	w6, w4, wzr, ne
	cmp	w7, #1
	mov	w3, w2
	ccmp	x9, #0, #0, ge
	mov	x2, x1
	mov	w1, w0
	csel	w11, w7, wzr, ne
	cmp	w12, #1
	ccmp	x10, #0, #0, ge
	csel	w12, w12, wzr, ne
	subs	w13, w11, #1
	b.lt	.LBB28_6
	cbz	x9, .LBB28_6
	adrp	x11, "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"
	ldr	w14, [x9]
	ldr	x11, [x11, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	ldr	w11, [x11]
	cmp	w14, w11
	b.ne	.LBB28_5
	add	x9, x9, #4
	mov	w11, w13
	b	.LBB28_6
.LBB28_5:
	mov	w11, w7
.LBB28_6:
	adrp	x13, "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"
	mov	x4, x5
	mov	x5, x8
	ldr	x0, [x13, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	x7, x9
	str	w12, [sp, #16]
	str	x10, [sp, #8]
	str	w11, [sp]
	bl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB28_7:
	.seh_startepilogue
	ldp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #32
	.seh_stackalloc	32
	stp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	x29, sp, #16
	.seh_add_fp	16
	.seh_endprologue
	ldr	x9, [x0, #8]
	sbfiz	x10, x1, #3, #32
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"
	ldr	x13, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	ldr	x14, [x4]
	ldr	x9, [x9, x10]
	ldr	x15, [x13]
	sbfiz	x11, x9, #4, #32
	lsr	x8, x9, #32
	lsl	x10, x9, #32
	mov	w9, w9
	ldr	x12, [x15, x11]
	cmp	x14, x12
	b.ne	.LBB29_4
	ldrh	w14, [x4, #14]
.LBB29_2:
	ldr	w13, [x12, #8]
	sxth	w14, w14
	ldr	x12, [x12]
	add	x11, x15, x11
	asr	x10, x10, #35
	ubfiz	x9, x9, #14, #6
	lsl	w16, w13, #2
	mul	w13, w13, w14
	ldrsw	x11, [x11, #12]
	and	x10, x10, #0xfffffffffffffff8
	add	x12, x12, w16, sxtw
	add	x12, x12, w13, sxtw #2
	adrp	x13, "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"
	str	w3, [x12, x11, lsl #2]
	ldr	x11, [x13, :lo12:"Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	ldrh	w12, [x4, #12]
	ldr	x10, [x11, x10]
	ldrsw	x11, [x4, #8]
	smaddl	x8, w12, w8, x11
	ldrb	w11, [x0, #494]
	add	x9, x10, x9
	add	x8, x9, x8
	cbnz	w11, .LBB29_13
.LBB29_3:
	add	x0, x8, #64
	.seh_startepilogue
	ldp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	sp, sp, #32
	.seh_stackalloc	32
	.seh_endepilogue
	ret
.LBB29_4:
	ldrsh	x14, [x4, #14]
	ldr	x15, [x12, #112]
	ldr	w16, [x12, #168]
	tbnz	x14, #63, .LBB29_7
	cmp	w16, w14
	b.le	.LBB29_7
	ldr	w17, [x15, x14, lsl #2]
	cmp	w17, w2
	b.eq	.LBB29_15
.LBB29_7:
	cbz	w16, .LBB29_11
	mov	w14, wzr
.LBB29_9:
	ldr	w17, [x15, w14, sxtw #2]
	cmp	w17, w2
	b.eq	.LBB29_14
	add	w14, w14, #1
	cmp	w16, w14
	b.ne	.LBB29_9
.LBB29_11:
	mov	w14, #65535
	strh	w14, [x4, #14]
.LBB29_12:
	mov	w15, wzr
	str	wzr, [x4, #8]
	mov	w14, #65535
	b	.LBB29_16
.LBB29_13:
	mov	w2, w2
	add	x4, x8, #64
	str	x8, [sp, #8]
	bl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [sp, #8]
	b	.LBB29_3
.LBB29_14:
	mov	w15, #65535
	strh	w14, [x4, #14]
	bics	wzr, w15, w14
	b.eq	.LBB29_12
.LBB29_15:
	mov	w15, w14
	ldr	x16, [x12, #224]
	sxth	x15, w15
	ldr	w16, [x16, x15, lsl #2]
	str	w16, [x4, #8]
	ldr	x16, [x12, #232]
	ldrh	w15, [x16, x15, lsl #1]
.LBB29_16:
	strh	w15, [x4, #12]
	str	x12, [x4]
	ldr	x15, [x13]
	b	.LBB29_2
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #64
	.seh_stackalloc	64
	stp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	x29, sp, #48
	.seh_add_fp	48
	.seh_endprologue
	stur	w2, [x29, #-12]
	stur	x1, [x29, #-8]
	tbnz	w2, #27, .LBB30_7
	and	x11, x2, #0x30000000
	mov	w12, #268435456
	mov	w9, w3
	mov	x10, x0
	cmp	x11, x12
	b.ne	.LBB30_3
	mov	x8, xzr
	mov	w11, wzr
	mov	w0, #15
	b	.LBB30_6
.LBB30_3:
	tbnz	w2, #26, .LBB30_5
	adrp	x11, "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"
	and	x12, x2, #0x1fff
	mov	w13, #104
	ldr	x11, [x11, :lo12:"Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"]
	mov	x8, x4
	mov	w0, #14
	ldr	x11, [x11]
	umaddl	x11, w12, w13, x11
	ldr	w11, [x11, #100]
	b	.LBB30_6
.LBB30_5:
	mov	x8, xzr
	mov	w11, wzr
	mov	w0, #16
.LBB30_6:
	movi	v0.2d, #0000000000000000
	sub	x3, x29, #8
	add	x5, sp, #20
	sub	x6, x29, #12
	mov	x1, x10
	mov	w2, w9
	mov	w4, #1
	mov	w7, #1
	str	w11, [sp, #8]
	str	x8, [sp]
	stur	q0, [sp, #20]
	bl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB30_7:
	.seh_startepilogue
	ldp	x29, x30, [sp, #48]
	.seh_save_fplr	48
	add	sp, sp, #64
	.seh_stackalloc	64
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-96]!
	.seh_save_regp_x	x19, 96
	stp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	stp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	stp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	stp	x27, x28, [sp, #64]
	.seh_save_regp	x27, 64
	stp	x29, x30, [sp, #80]
	.seh_save_fplr	80
	add	x29, sp, #80
	.seh_add_fp	80
	.seh_endprologue
	sub	sp, sp, #432
	movi	v0.2d, #0000000000000000
	ldr	x8, [x0, #832]
	ldr	w11, [x0, #892]
	ldr	x14, [x0, #808]
	ldr	w9, [x0, #852]
	mov	x21, x0
	str	x8, [sp, #48]
	ldr	x8, [x0, #792]
	ldr	w10, [x0, #872]
	str	w11, [sp, #164]
	ldr	x11, [x0, #816]
	ldr	x12, [x0, #824]
	orr	w9, w9, w10
	stur	q0, [x29, #-152]
	stur	q0, [x29, #-136]
	cmp	w9, #0
	stur	q0, [x29, #-120]
	stur	q0, [x29, #-104]
	ldr	x13, [x8]
	ldr	x8, [x14]
	str	w3, [sp, #100]
	ldr	x3, [x11]
	ldr	x4, [x12]
	str	x2, [sp, #88]
	str	x8, [sp, #16]
	cset	w8, eq
	stur	xzr, [x29, #-160]
	str	xzr, [sp, #184]
	cbnz	w9, .LBB31_2
	ldr	w9, [x21, #884]
	cbz	w9, .LBB31_63
.LBB31_2:
	mov	w6, wzr
	mov	w9, #1
.LBB31_3:
	ldp	q0, q1, [x21, #848]
	strb	w9, [sp, #272]
	add	x9, sp, #25
	ldr	x11, [sp, #16]
	ldr	x10, [sp, #48]
	stp	x3, x4, [sp, #208]
	stur	q0, [sp, #232]
	ldr	d0, [x21, #880]
	stp	x11, x10, [sp, #192]
	add	x10, sp, #73
	str	d0, [sp, #264]
	movi	v0.2d, #0000000000000000
	stur	q1, [sp, #248]
	str	x13, [sp, #224]
	strh	wzr, [sp, #274]
	stur	q0, [x9, #255]
	add	x9, sp, #41
	stur	q0, [x9, #255]
	ldr	w9, [sp, #164]
	stur	q0, [x10, #255]
	mov	w10, #-1
	cmp	w9, #0
	add	x9, sp, #57
	str	w10, [sp, #276]
	stur	q0, [x9, #255]
	cset	w9, ne
	ldr	w10, [x21, #888]
	strb	w9, [sp, #273]
	ldr	w9, [x14, #8]
	str	w10, [sp, #104]
	str	d0, [sp, #344]
	str	x13, [sp]
	cbz	w8, .LBB31_5
	ldr	w8, [x21, #884]
	cmp	w8, #0
	cset	w8, ne
	b	.LBB31_6
.LBB31_5:
	mov	w8, #1
.LBB31_6:
	str	w8, [sp, #84]
	ldr	w8, [sp, #104]
	mov	w25, wzr
	mov	x28, xzr
	mov	x1, xzr
	mov	x5, xzr
	cmp	w8, #1
	ldr	w8, [sp, #164]
	mov	x30, xzr
	mov	x2, xzr
	mov	x20, xzr
	mov	x7, xzr
	mov	x19, xzr
	mov	w27, #-1
	stp	wzr, w9, [x29, #-160]
	stp	x4, x3, [sp, #136]
	str	x14, [sp, #40]
	str	w6, [sp, #12]
	str	x21, [sp, #72]
	b.eq	.LBB31_65
	movi	v0.2d, #0000000000000000
	cmp	w8, #0
	ldr	w9, [sp, #84]
	cset	w8, ne
	orr	w8, w8, w9
	tbz	w8, #0, .LBB31_67
.LBB31_8:
	ldp	w8, w23, [x29, #-160]
	stur	q0, [x29, #-120]
	cmp	w8, w23
	str	w23, [sp, #184]
	b.ge	.LBB31_62
	ldr	w11, [sp, #268]
	ldr	w10, [sp, #256]
	add	x9, sp, #184
	add	x24, x9, #64
	movi	v3.2d, #0xffffffffffffffff
	str	w11, [sp, #160]
	ldp	w26, w11, [sp, #232]
	stp	x11, x10, [sp, #168]
	and	x10, x10, #0x7ffffffe
	str	x10, [sp, #152]
	add	x10, x9, #80
	add	x9, x9, #76
	stp	x10, x24, [sp, #56]
	str	x9, [sp, #128]
.LBB31_10:
	sxtw	x22, w8
	b	.LBB31_13
.LBB31_11:
	mov	x9, v0.d[1]
	fmov	x10, d0
	orr	x10, x10, x9
	cbnz	x10, .LBB31_60
.LBB31_12:
	add	x22, x22, #1
	cmp	w23, w22
	b.eq	.LBB31_62
.LBB31_13:
	ldr	w8, [x3, x22, lsl #2]
	cmp	w27, w8
	b.ne	.LBB31_47
	ldrsw	x8, [x4, x22, lsl #2]
	tbnz	w6, #0, .LBB31_33
.LBB31_15:
	ldr	x10, [x19]
	ldr	x12, [sp, #168]
	ldr	w9, [x10, #8]
	ldr	x11, [x10]
	cmp	w12, #1
	lsl	w12, w9, #2
	b.lt	.LBB31_19
	ldr	w13, [x10, #20]
	add	x14, x11, w12, sxtw #1
	ldrsh	w10, [x10, #254]
	ldr	x15, [sp, #168]
	mov	x16, x24
	mul	w13, w13, w12
	add	x13, x14, w13, sxtw
	add	x14, x19, #28
	add	x13, x13, x8, lsl #2
.LBB31_17:
	ldursw	x17, [x16, #-8]
	ldr	w0, [x16], #4
	ldr	w17, [x14, x17, lsl #2]
	sub	w17, w17, w10
	mul	w17, w17, w9
	ldr	w17, [x13, w17, sxtw #2]
	cmp	w17, w0
	b.ne	.LBB31_12
	subs	x15, x15, #1
	b.ne	.LBB31_17
.LBB31_19:
	ldr	x10, [sp, #176]
	cbz	w10, .LBB31_24
	ldr	w10, [sp, #160]
	cbnz	w10, .LBB31_25
	ldr	x13, [sp, #176]
	cmp	w13, #1
	b.lt	.LBB31_26
.LBB31_22:
	add	x11, x11, w12, sxtw
	cmp	w13, #1
	add	x12, x19, #28
	add	x11, x11, x8, lsl #2
	b.ne	.LBB31_27
	mov	x15, xzr
	mov	w13, wzr
	b	.LBB31_30
.LBB31_24:
	ldr	w10, [sp, #160]
	cbz	w10, .LBB31_33
.LBB31_25:
	add	x10, x11, w12, sxtw
	cmp	w26, #0
	ldr	w10, [x10, x8, lsl #2]
	sub	w10, w10, w26
	ccmp	w10, #0, #0, ne
	cset	w10, gt
	ldr	x13, [sp, #176]
	cmp	w13, #1
	b.ge	.LBB31_22
.LBB31_26:
	mov	w13, wzr
	b	.LBB31_32
.LBB31_27:
	ldr	x15, [sp, #56]
	ldr	x16, [sp, #152]
	mov	x24, x2
	mov	w2, w6
	mov	w6, w25
	mov	x25, x28
	mov	x28, x1
	mov	w13, wzr
	mov	w14, wzr
.LBB31_28:
	ldpsw	x17, x0, [x15, #-4]
	cmp	w26, #0
	add	x15, x15, #8
	cset	w1, eq
	ldr	w17, [x12, x17, lsl #2]
	ldr	w0, [x12, x0, lsl #2]
	mul	w17, w17, w9
	mul	w0, w0, w9
	ldr	w17, [x11, w17, sxtw #2]
	ldr	w0, [x11, w0, sxtw #2]
	sub	w17, w17, w26
	sub	w0, w0, w26
	cmp	w17, #0
	csinc	w17, w1, wzr, le
	cmp	w0, #0
	csinc	w0, w1, wzr, le
	subs	x16, x16, #2
	orr	w13, w13, w17
	orr	w14, w14, w0
	b.ne	.LBB31_28
	ldr	x15, [sp, #176]
	ldr	x16, [sp, #152]
	mov	x1, x28
	mov	x28, x25
	mov	w25, w6
	mov	w6, w2
	mov	x2, x24
	ldr	x24, [sp, #64]
	cmp	x16, x15
	orr	w13, w14, w13
	mov	x15, x16
	b.eq	.LBB31_32
.LBB31_30:
	ldr	x14, [sp, #128]
	ldr	x16, [sp, #176]
	add	x14, x14, x15, lsl #2
	sub	x15, x16, x15
.LBB31_31:
	ldrsw	x16, [x14], #4
	cmp	w26, #0
	ldr	w16, [x12, x16, lsl #2]
	mul	w16, w16, w9
	ldr	w16, [x11, w16, sxtw #2]
	sub	w16, w16, w26
	ccmp	w16, #0, #0, ne
	cset	w16, gt
	subs	x15, x15, #1
	orr	w13, w13, w16
	b.ne	.LBB31_31
.LBB31_32:
	orr	w9, w10, w13
	tbz	w9, #0, .LBB31_12
.LBB31_33:
	ldr	w9, [sp, #164]
	cbz	w9, .LBB31_59
	mul	w10, w25, w8
	ldr	w8, [x7, x8, lsl #2]
	ldp	w13, w11, [x20, #12]
	ldp	w12, w9, [x20, #20]
	add	x10, x28, w10, sxtw #4
	stur	q3, [x29, #-120]
	cbz	w13, .LBB31_49
	movi	v0.2d, #0xffffffffffffffff
	cmp	w13, #1
	b.lt	.LBB31_38
	mov	x14, x2
.LBB31_37:
	ldrsw	x15, [x14], #4
	subs	x13, x13, #1
	ldr	q1, [x10, x15, lsl #4]
	and	v0.16b, v1.16b, v0.16b
	stur	q0, [x29, #-120]
	b.ne	.LBB31_37
.LBB31_38:
	cmp	w12, #0
	b.le	.LBB31_43
.LBB31_39:
	ldrsw	x13, [x30]
	cmp	w12, #1
	ldr	q1, [x10, x13, lsl #4]
	b.eq	.LBB31_42
	sub	x12, x12, #1
	add	x13, x30, #4
.LBB31_41:
	ldrsw	x14, [x13], #4
	subs	x12, x12, #1
	ldr	q2, [x10, x14, lsl #4]
	orr	v1.16b, v2.16b, v1.16b
	b.ne	.LBB31_41
.LBB31_42:
	and	v0.16b, v1.16b, v0.16b
	stur	q0, [x29, #-120]
.LBB31_43:
	cmp	w11, #1
	mov	x12, x5
	b.lt	.LBB31_45
.LBB31_44:
	ldrsw	x13, [x12], #4
	subs	x11, x11, #1
	ldr	q1, [x10, x13, lsl #4]
	bic	v0.16b, v0.16b, v1.16b
	stur	q0, [x29, #-120]
	b.ne	.LBB31_44
.LBB31_45:
	cmp	w9, #1
	mov	x11, x1
	b.lt	.LBB31_11
.LBB31_46:
	ldrsw	x12, [x11], #4
	subs	x9, x9, #1
	ldr	q1, [x10, x12, lsl #4]
	bic	v0.16b, v0.16b, v1.16b
	stur	q0, [x29, #-120]
	b.ne	.LBB31_46
	b	.LBB31_11
.LBB31_47:
	ldr	x9, [sp]
	mov	w27, w6
	str	w8, [sp, #276]
	ldr	x15, [x9, w8, sxtw #3]
	ldr	x13, [x15]
	mov	x19, x15
	ldr	w9, [x13, #8]
	ldr	x10, [x13]
	lsl	w11, w9, #2
	ldr	w9, [x13, #20]
	mul	w14, w11, w9
	sxtw	x12, w11
	sxtw	x11, w14
	add	x14, x10, x12
	add	x14, x11, x14
	stp	x15, x14, [sp, #280]
	mov	x7, x14
	ldr	w14, [sp, #164]
	cbz	w14, .LBB31_53
	ldpsw	x14, x15, [x19, #8]
	ldr	w13, [x13, #16]
	lsl	x16, x12, #1
	sbfx	w25, w9, #0, #28
	ldp	x4, x3, [sp, #136]
	str	wzr, [sp, #348]
	add	x14, x19, x14, lsl #2
	mul	w12, w13, w12
	add	x10, x10, x16
	add	x10, x10, x11
	str	w25, [sp, #344]
	mov	x20, x19
	add	x2, x14, #28
	mov	w6, w27
	mov	w27, w8
	ldpsw	x13, x14, [x19, #16]
	add	x5, x2, x15, lsl #2
	add	w15, w16, w11
	add	x10, x10, w12, sxtw
	add	w11, w15, w12
	stp	x19, x2, [sp, #296]
	add	x30, x5, x13, lsl #2
	and	x11, x11, #0xc
	sub	x10, x10, x11
	add	x1, x30, x14, lsl #2
	add	x28, x10, #16
	stp	x30, x5, [sp, #312]
	stp	x1, x28, [sp, #328]
	b	.LBB31_54
.LBB31_49:
	mov	w13, #128
	mov	w15, w27
	mov	w27, w6
	sub	w14, w13, w8
	mov	x6, x2
	cmp	w14, #127
	b.gt	.LBB31_57
	cmp	w8, #128
	b.eq	.LBB31_58
	mov	x13, #-1
	cmp	w14, #63
	lsr	x13, x13, x14
	b.gt	.LBB31_55
	mov	x14, #-1
	lsl	x14, x14, x8
	orr	x14, x13, x14
	fmov	d0, x14
	mov	v0.d[1], x13
	b	.LBB31_56
.LBB31_53:
	ldp	x4, x3, [sp, #136]
	mov	w9, w27
	mov	w27, w8
	mov	w6, w9
.LBB31_54:
	movi	v3.2d, #0xffffffffffffffff
	ldr	x24, [sp, #64]
	ldrsw	x8, [x4, x22, lsl #2]
	tbz	w6, #0, .LBB31_15
	b	.LBB31_33
.LBB31_55:
	movi	v0.2d, #0000000000000000
	mov	v0.d[0], x13
.LBB31_56:
	ldp	x4, x3, [sp, #136]
	mov	x2, x6
	ldr	x24, [sp, #64]
	stur	q0, [x29, #-120]
	mov	w6, w27
	mov	w27, w15
	movi	v3.2d, #0xffffffffffffffff
	cmp	w12, #0
	b.gt	.LBB31_39
	b	.LBB31_43
.LBB31_57:
	movi	v0.2d, #0000000000000000
	b	.LBB31_56
.LBB31_58:
	movi	v0.2d, #0xffffffffffffffff
	b	.LBB31_56
.LBB31_59:
	str	w25, [sp, #108]
	mov	w25, w27
	mov	x27, x5
	stp	x1, x30, [sp, #112]
	mov	w3, wzr
	stp	x7, x2, [sp, #24]
	b	.LBB31_61
.LBB31_60:
	fmov	d1, x9
	ptrue	p0.d
	str	w25, [sp, #108]
	mov	w25, w27
	stp	x1, x30, [sp, #112]
	mov	x27, x5
	cnt	z0.d, p0/m, z0.d
	stp	x7, x2, [sp, #24]
	cnt	z1.d, p0/m, z1.d
	fmov	w9, s0
	fmov	w10, s1
	add	w9, w9, w10
	cmp	w9, w8
	cset	w3, ne
.LBB31_61:
	sbfiz	x8, x22, #2, #32
	ldr	x9, [sp, #16]
	add	x0, x21, #160
	sub	x1, x29, #104
	sub	x4, x29, #120
	mov	w2, w22
	ldr	w8, [x9, x8]
	ldr	x9, [sp, #48]
	stur	x9, [x29, #-96]
	stur	w8, [x29, #-104]
	bl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	movi	v3.2d, #0xffffffffffffffff
	ldp	x4, x3, [sp, #136]
	ldp	x7, x2, [sp, #24]
	add	w8, w22, #1
	ldp	x1, x30, [sp, #112]
	mov	x5, x27
	ldr	w6, [sp, #12]
	mov	w27, w25
	ldr	w25, [sp, #108]
	cmp	w8, w23
	b.lt	.LBB31_10
.LBB31_62:
	stur	wzr, [x29, #-104]
	stur	xzr, [x29, #-96]
	b	.LBB31_71
.LBB31_63:
	mov	w6, #1
	b	.LBB31_3
.LBB31_64:
	stur	wzr, [x29, #-160]
	ldr	w8, [x14, #8]
	stur	w8, [x29, #-156]
.LBB31_65:
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	ldr	x0, [sp, #88]
	mov	x21, x28
	ldr	x8, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	mov	w28, w25
	mov	x25, x1
	ldr	w1, [sp, #100]
	mov	x23, x2
	sub	x2, x29, #160
	sub	x3, x29, #156
	str	w27, [sp, #168]
	mov	x22, x5
	str	x20, [sp, #176]
	mov	x24, x19
	mov	x19, x7
	mov	x26, x30
	mov	w20, w6
	mov	x27, x14
	blr	x8
	ldp	x4, x3, [sp, #136]
	mov	x14, x27
	mov	w6, w20
	mov	x1, x25
	mov	w25, w28
	mov	x28, x21
	ldr	x21, [sp, #72]
	ldr	w27, [sp, #168]
	ldr	x20, [sp, #176]
	tst	w0, #0xff
	b.eq	.LBB31_73
	ldr	w8, [x21, #892]
	mov	x30, x26
	mov	x7, x19
	mov	x19, x24
	mov	x2, x23
	mov	x5, x22
	movi	v0.2d, #0000000000000000
	cmp	w8, #0
	ldr	w9, [sp, #84]
	cset	w8, ne
	orr	w8, w8, w9
	tbnz	w8, #0, .LBB31_8
.LBB31_67:
	ldp	w22, w8, [x29, #-160]
	ldr	x23, [x14]
	ldr	x9, [sp, #48]
	stur	wzr, [x29, #-152]
	sxtw	x22, w22
	stur	q0, [x29, #-136]
	stur	x9, [x29, #-144]
	cmp	w22, w8
	b.ge	.LBB31_72
	str	w25, [sp, #108]
	mov	w25, w27
	mov	x26, x2
	str	x19, [sp, #176]
	mov	x19, x7
	mov	w24, w6
	mov	x27, x5
	stp	x1, x30, [sp, #112]
.LBB31_69:
	ldr	w8, [x23, x22, lsl #2]
	add	x0, x21, #160
	sub	x1, x29, #152
	sub	x4, x29, #136
	mov	w2, w22
	mov	w3, wzr
	stur	w8, [x29, #-152]
	bl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldursw	x8, [x29, #-156]
	add	x22, x22, #1
	cmp	x22, x8
	b.lt	.LBB31_69
	ldp	x1, x30, [sp, #112]
	mov	x5, x27
	mov	x7, x19
	ldr	x19, [sp, #176]
	mov	w27, w25
	ldr	w25, [sp, #108]
	mov	w6, w24
	mov	x2, x26
.LBB31_71:
	ldr	x14, [sp, #40]
.LBB31_72:
	ldr	w8, [sp, #104]
	cmp	w8, #1
	b.eq	.LBB31_64
.LBB31_73:
	.seh_startepilogue
	add	sp, sp, #432
	.seh_stackalloc	432
	ldp	x29, x30, [sp, #80]
	.seh_save_fplr	80
	ldp	x27, x28, [sp, #64]
	.seh_save_regp	x27, 64
	ldp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	ldp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	ldp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	ldp	x19, x20, [sp], #96
	.seh_save_regp_x	x19, 96
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.memcpy.inline.ARMV9A.i64;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memcpy.inline.ARMV9A.i64
	.globl	burst.memcpy.inline.ARMV9A.i64
	.p2align	2
burst.memcpy.inline.ARMV9A.i64:
	cmp	x2, #16
	b.lo	.LBB32_6
	mov	x9, xzr
.LBB32_2:
	add	x10, x9, #32
	add	x8, x9, #16
	ldr	q0, [x1, x9]
	cmp	x2, x10
	str	q0, [x0, x9]
	mov	x9, x8
	b.hs	.LBB32_2
	add	x9, x8, #1
	cmp	x2, x9
	b.lo	.LBB32_5
.LBB32_4:
	add	x9, x8, #2
	ldrb	w10, [x1, x8]
	add	x11, x8, #1
	cmp	x2, x9
	strb	w10, [x0, x8]
	mov	x8, x11
	b.hs	.LBB32_4
.LBB32_5:
	ret
.LBB32_6:
	mov	x8, xzr
	add	x9, x8, #1
	cmp	x2, x9
	b.hs	.LBB32_4
	b	.LBB32_5

	.def	burst.memset.inline.ARMV9A.i32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memset.inline.ARMV9A.i32
	.globl	burst.memset.inline.ARMV9A.i32
	.p2align	2
burst.memset.inline.ARMV9A.i32:
	cmp	w2, #16
	b.lo	.LBB33_6
	dup	v0.16b, w1
	mov	w9, wzr
.LBB33_2:
	add	w10, w9, #32
	add	w8, w9, #16
	str	q0, [x0, w9, sxtw]
	cmp	w2, w10
	mov	w9, w8
	b.hs	.LBB33_2
	add	w9, w8, #1
	cmp	w2, w9
	b.lo	.LBB33_5
.LBB33_4:
	add	w9, w8, #2
	add	w10, w8, #1
	strb	w1, [x0, w8, sxtw]
	cmp	w2, w9
	mov	w8, w10
	b.hs	.LBB33_4
.LBB33_5:
	ret
.LBB33_6:
	mov	w8, wzr
	add	w9, w8, #1
	cmp	w2, w9
	b.hs	.LBB33_4
	b	.LBB33_5

	.def	burst.memset.inline.ARMV9A.i64;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memset.inline.ARMV9A.i64
	.globl	burst.memset.inline.ARMV9A.i64
	.p2align	2
burst.memset.inline.ARMV9A.i64:
	cmp	x2, #16
	b.lo	.LBB34_6
	dup	v0.16b, w1
	mov	x9, xzr
.LBB34_2:
	add	x10, x9, #32
	add	x8, x9, #16
	str	q0, [x0, x9]
	cmp	x2, x10
	mov	x9, x8
	b.hs	.LBB34_2
	add	x9, x8, #1
	cmp	x2, x9
	b.lo	.LBB34_5
.LBB34_4:
	add	x9, x8, #2
	add	x10, x8, #1
	strb	w1, [x0, x8]
	cmp	x2, x9
	mov	x8, x10
	b.hs	.LBB34_4
.LBB34_5:
	ret
.LBB34_6:
	mov	x8, xzr
	add	x9, x8, #1
	cmp	x2, x9
	b.hs	.LBB34_4
	b	.LBB34_5

	.section	.drectve,"yni"
	.ascii	" /EXPORT:b639652c4fdbfa25bf83f596085599e0"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0\""
	.ascii	" /EXPORT:\"burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0\""
