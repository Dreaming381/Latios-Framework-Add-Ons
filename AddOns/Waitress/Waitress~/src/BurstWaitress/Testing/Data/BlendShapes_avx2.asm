
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	b639652c4fdbfa25bf83f596085599e0
	.p2align	4
b639652c4fdbfa25bf83f596085599e0:
.seh_proc b639652c4fdbfa25bf83f596085599e0
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	mov	rax, r9
	mov	rdx, r8
	mov	r9d, dword ptr [rbp + 48]
	mov	r8, rax
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	jmp	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.seh_endproc

	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize
	.p2align	4
burst.initialize:
.seh_proc burst.initialize
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	sub	rsp, 48
	.seh_stackalloc 48
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	mov	rdi, rcx
	lea	rcx, [rip + burst.string.808e8e7f94567b9f878889686ba14eb1]
	call	rdi
	mov	qword ptr [rip + burst_abort_Ptr], rax
	lea	rcx, [rip + burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4]
	call	rdi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"], rax
	lea	rcx, [rip + burst.string.76cb701fb3c87996598b3e8df4d3255d]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"], rax
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rdi
	mov	rsi, rax
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"], rax
	lea	rcx, [rip + burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.ffbfa8f9468181ee01437772260f8c26]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.e032451e108c081916a72836299e732a]
	call	rdi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"], rax
	movabs	rax, -2301534937205546864
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"], rax
	movabs	rax, 6266345168485942189
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"], rax
	mov	qword ptr [rip + "System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"], 1073741824
	movabs	rax, -2093332447932307698
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 264192
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"], rax
	movabs	rax, 1063811976437696451
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 196608
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"], rax
	movabs	rax, -2710612501355120639
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 1
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"], rax
	movabs	rax, 2312405331276346388
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 96
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"], rax
	movabs	rax, -6142761577920778426
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 4
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"], rax
	movabs	rax, -7082256032705860890
	mov	qword ptr [rbp - 16], rax
	movabs	rax, -7370724514926575087
	mov	qword ptr [rbp - 8], rax
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"], rax
	movabs	rax, -5372448820671685467
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 4
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"], rax
	movabs	rax, -1297938794087215229
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 524288
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"], rax
	movabs	rax, -130492041295693829
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 65536
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"], rax
	.seh_startepilogue
	add	rsp, 48
	pop	rdi
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0
	.p2align	4
burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0:
.seh_proc burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	sub	rsp, 40
	.seh_stackalloc 40
	lea	rbp, [rsp + 32]
	.seh_setframe rbp, 32
	.seh_endprologue
	mov	rsi, rcx
	lea	rcx, [rip + burst.string.808e8e7f94567b9f878889686ba14eb1]
	call	rsi
	mov	qword ptr [rip + burst_abort_Ptr], rax
	lea	rcx, [rip + burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4]
	call	rsi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"], rax
	lea	rcx, [rip + burst.string.76cb701fb3c87996598b3e8df4d3255d]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"], rax
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.9e09ceb2fe7b13395cb0ec8f4b07b610]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::SetNestedContainer_Injected_Ptr"], rax
	lea	rcx, [rip + burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.ffbfa8f9468181ee01437772260f8c26]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.e032451e108c081916a72836299e732a]
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"], rax
	.seh_startepilogue
	add	rsp, 40
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0
	.p2align	4
burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0:
.seh_proc burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	sub	rsp, 56
	.seh_stackalloc 56
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	movabs	rax, -2301534937205546864
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	mov	rsi, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	lea	rcx, [rbp - 8]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"], rax
	movabs	rax, 6266345168485942189
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.UInt64> Unity.Entities.EntityComponentStore::s_TotalChunkAddressSpaceInBytes"], rax
	mov	qword ptr [rip + "System.UInt64 Unity.Entities.EntityComponentStore::DefaultChunkAddressSpaceInBytes"], 1073741824
	movabs	rax, -2093332447932307698
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 264192
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"], rax
	movabs	rax, 1063811976437696451
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 196608
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/EntityStore> Unity.Entities.EntityComponentStore::s_entityStore"], rax
	movabs	rax, -2710612501355120639
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 1
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"], rax
	movabs	rax, 2312405331276346388
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 96
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"], rax
	movabs	rax, -6142761577920778426
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 4
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"], rax
	movabs	rax, -7082256032705860890
	mov	qword ptr [rbp - 8], rax
	movabs	rax, -7370724514926575087
	mov	qword ptr [rbp], rax
	lea	rcx, [rbp - 8]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"], rax
	movabs	rax, -5372448820671685467
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 4
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"], rax
	movabs	rax, -1297938794087215229
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 524288
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"], rax
	movabs	rax, -130492041295693829
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 65536
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"], rax
	.seh_startepilogue
	add	rsp, 56
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rcx], ymm0
	vmovups	ymmword ptr [rcx + 32], ymm0
	vmovups	ymmword ptr [rcx + 64], ymm0
	vmovups	ymmword ptr [rcx + 96], ymm0
	mov	qword ptr [rcx + 128], 0
	mov	dword ptr [rcx + 128], -1
	test	dl, dl
	je	.LBB4_9
	vmovq	rax, xmm2
	cmp	r9d, 64
	mov	r8, -1
	shlx	rdx, r8, r9
	not	rdx
	cmovge	rdx, r8
	xor	r8d, r8d
	and	rdx, rax
	je	.LBB4_4
	xor	eax, eax
	.p2align	4
.LBB4_3:
	tzcnt	r10, rdx
	shrx	rdx, rdx, r10
	mov	r11, rdx
	not	r11
	tzcnt	r11, r11
	movsxd	r8, r8d
	add	eax, r10d
	mov	byte ptr [rcx + r8], al
	movsxd	r8, dword ptr [rcx + 132]
	mov	byte ptr [rcx + r8 + 64], r11b
	add	al, r11b
	dec	r11b
	shrx	r10, rdx, r11
	mov	rdx, r10
	shr	rdx
	mov	r8d, dword ptr [rcx + 132]
	inc	r8d
	mov	dword ptr [rcx + 132], r8d
	cmp	r10, 1
	ja	.LBB4_3
.LBB4_4:
	cmp	r9d, 64
	jl	.LBB4_10
	vpextrq	rax, xmm2, 1
	cmp	r9d, 127
	mov	r10, -1
	shlx	rdx, r10, r9
	not	rdx
	cmova	rdx, r10
	and	rdx, rax
	mov	al, 64
	test	r8d, r8d
	je	.LBB4_6
	movsxd	r8, r8d
	movzx	r10d, byte ptr [r8 + rcx - 1]
	movzx	r9d, byte ptr [r8 + rcx + 63]
	add	r10d, r9d
	cmp	r10d, 64
	jne	.LBB4_6
	mov	r10d, edx
	and	r10d, 1
	je	.LBB4_6
	mov	rax, rdx
	not	rax
	tzcnt	r10, rax
	add	r9b, r10b
	mov	byte ptr [rcx + r8 + 63], r9b
	lea	eax, [r10 + 64]
	dec	r10b
	shrx	rdx, rdx, r10
	shr	rdx
.LBB4_6:
	test	rdx, rdx
	je	.LBB4_10
	mov	r8d, dword ptr [rcx + 132]
	.p2align	4
.LBB4_8:
	tzcnt	r9, rdx
	shrx	rdx, rdx, r9
	mov	r10, rdx
	not	r10
	tzcnt	r10, r10
	movsxd	r8, r8d
	add	eax, r9d
	mov	byte ptr [rcx + r8], al
	movsxd	r8, dword ptr [rcx + 132]
	mov	byte ptr [rcx + r8 + 64], r10b
	add	al, r10b
	dec	r10b
	shrx	r9, rdx, r10
	mov	rdx, r9
	shr	rdx
	mov	r8d, dword ptr [rcx + 132]
	inc	r8d
	mov	dword ptr [rcx + 132], r8d
	cmp	r9, 2
	jae	.LBB4_8
.LBB4_10:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
.LBB4_9:
	mov	byte ptr [rcx + 64], r9b
	mov	dword ptr [rcx + 132], 1
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64"
	.p2align	4
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 200
	.seh_stackalloc 200
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	r12, r9
	mov	rsi, r8
	mov	r14, rdx
	mov	rdi, rcx
	mov	rcx, qword ptr [rbp + 192]
	mov	rax, qword ptr [rbp + 200]
	mov	rax, qword ptr [rax]
	mov	rcx, qword ptr [rcx]
	test	rcx, rcx
	je	.LBB5_1
	mov	ebx, dword ptr [rcx]
	mov	r15d, dword ptr [rcx + 4]
	mov	edx, ebx
	mov	r8d, r15d
	jmp	.LBB5_2
.LBB5_1:
	xor	ebx, ebx
	xor	r15d, r15d
	xor	edx, edx
	xor	r8d, r8d
.LBB5_2:
	mov	rcx, qword ptr [rbp + 176]
	or	edx, r8d
	sete	dl
	test	rax, rax
	setne	r8b
	test	dl, r8b
	je	.LBB5_3
	mov	ebx, dword ptr [rax]
	mov	r15d, dword ptr [rax + 4]
.LBB5_3:
	mov	r13, qword ptr [rcx]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp - 24], xmm0
	mov	byte ptr [rbp + 48], 0
	lea	rax, [rbp + 48]
	mov	qword ptr [rsp + 32], rax
	lea	r9, [rbp - 24]
	mov	rcx, rdi
	mov	edx, ebx
	mov	r8d, r15d
	call	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	test	al, 1
	je	.LBB5_41
	mov	r8, qword ptr [rbp - 16]
	test	r8, r8
	je	.LBB5_41
	mov	rax, qword ptr [r14 + 8]
	cmp	rax, qword ptr [r8 - 16]
	mov	r9, qword ptr [r12]
	jne	.LBB5_6
	mov	rax, qword ptr [r14 + 16]
	test	r9, r9
	je	.LBB5_8
	mov	rcx, qword ptr [r9 - 16]
	cmp	rax, rcx
	je	.LBB5_11
.LBB5_6:
	mov	rcx, r14
	mov	rdx, rsi
	call	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
.LBB5_11:
	cmp	byte ptr [r14 + 24], 0
	je	.LBB5_41
	mov	r14, qword ptr [rbp + 184]
	add	rdi, 56
	lea	rdx, [rbp - 88]
	mov	rcx, rdi
	mov	r8d, ebx
	mov	r9d, r15d
	call	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rdi, qword ptr [rbp - 88]
	mov	rbx, qword ptr [r14]
	mov	r8, qword ptr [r14 + 32]
	mov	r12d, dword ptr [r14 + 40]
	mov	r9d, dword ptr [r14 + 44]
	movzx	r14d, byte ptr [r13]
	test	r14b, 32
	jne	.LBB5_13
	lea	rdx, [rbx + 16]
	test	r14b, 4
	mov	dword ptr [rbp + 68], r9d
	mov	qword ptr [rbp + 16], rdx
	mov	dword ptr [rbp + 8], r12d
	mov	eax, r14d
	jne	.LBB5_17
	and	eax, 3
	lea	rcx, [rip + "System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"]
	jmp	.LBB5_19
.LBB5_13:
	mov	r10, qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	jmp	.LBB5_14
.LBB5_8:
	xor	ecx, ecx
	cmp	rax, rcx
	jne	.LBB5_6
	jmp	.LBB5_11
.LBB5_17:
	and	eax, 7
	lea	rcx, [rip + "System.Int32[] Latios.Kinemation.BlendShapeState::PreviousFromMask_$__data_c8ad709e5ee7937b"]
.LBB5_19:
	mov	eax, dword ptr [rcx + 4*rax]
	mov	dword ptr [rbp + 36], eax
	mov	rax, qword ptr [rbx]
	test	rax, rax
	cmove	rax, rdx
	mov	qword ptr [rbp], rax
	mov	qword ptr [rbp + 48], r8
	mov	dword ptr [rbp + 56], r12d
	mov	dword ptr [rbp + 60], r9d
	mov	r15, qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::UseSecondaryVersion_Ptr"]
	lea	rcx, [rbp + 48]
	mov	r12, r8
	call	r15
	test	r14b, 4
	sete	r14b
	movsxd	rax, dword ptr [rbx + 8]
	mov	qword ptr [rbp - 8], rax
	movzx	eax, byte ptr [r13]
	mov	ecx, eax
	and	ecx, 7
	lea	rdx, [rip + "System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"]
	mov	ecx, dword ptr [rdx + 4*rcx]
	mov	dword ptr [rbp + 12], ecx
	or	al, 32
	mov	byte ptr [r13], al
	mov	rax, qword ptr [rbx]
	test	rax, rax
	mov	rcx, qword ptr [rbp + 16]
	cmovne	rcx, rax
	mov	qword ptr [rbp + 16], rcx
	mov	qword ptr [rbp + 24], r12
	mov	qword ptr [rbp + 48], r12
	mov	r12d, dword ptr [rbp + 8]
	mov	dword ptr [rbp + 56], r12d
	mov	eax, dword ptr [rbp + 68]
	mov	dword ptr [rbp + 60], eax
	lea	rcx, [rbp + 48]
	mov	qword ptr [rbp + 40], r15
	call	r15
	movsxd	rcx, dword ptr [rbx + 8]
	lea	eax, [rcx + 2]
	cmp	eax, 5
	setb	al
	or	al, r14b
	je	.LBB5_21
.LBB5_20:
	mov	r8, qword ptr [rbp + 24]
	mov	r9d, dword ptr [rbp + 68]
	mov	r10, qword ptr [rbp + 40]
	jmp	.LBB5_14
.LBB5_21:
	imul	rax, qword ptr [rbp - 8], 1431655766
	mov	rdx, rax
	shr	rdx, 63
	shr	rax, 32
	add	eax, edx
	imul	eax, dword ptr [rbp + 36]
	movsxd	r9, eax
	mov	r15, qword ptr [rbp]
	lea	rax, [r15 + 4*r9]
	imul	r8, rcx, 1431655766
	mov	rcx, r8
	shr	rcx, 63
	shr	r8, 32
	add	r8d, ecx
	mov	ecx, dword ptr [rbp + 12]
	imul	ecx, r8d
	movsxd	r10, ecx
	mov	r14, qword ptr [rbp + 16]
	lea	rcx, [r14 + 4*r10]
	mov	edx, r8d
	cmp	r8d, 4
	jb	.LBB5_22
	shl	r10, 2
	lea	r11, [r10 + r14]
	sub	r11, rax
	cmp	r11, 128
	jae	.LBB5_25
.LBB5_22:
	xor	r8d, r8d
	mov	r10, qword ptr [rbp + 40]
	.p2align	4
.LBB5_23:
	vmovss	xmm0, dword ptr [rax + 4*r8]
	vmovss	dword ptr [rcx + 4*r8], xmm0
	inc	r8
	cmp	rdx, r8
	jne	.LBB5_23
.LBB5_34:
	mov	r8, qword ptr [rbp + 24]
	mov	r9d, dword ptr [rbp + 68]
.LBB5_14:
	movzx	eax, byte ptr [r13]
	mov	ecx, eax
	and	ecx, 7
	lea	rdx, [rip + "System.Int32[] Latios.Kinemation.BlendShapeState::CurrentFromMask_$__data_ccfc3c34ee4f4957"]
	mov	r14d, dword ptr [rdx + 4*rcx]
	or	al, 32
	mov	byte ptr [r13], al
	mov	rax, qword ptr [rbx]
	lea	r15, [rbx + 16]
	test	rax, rax
	cmovne	r15, rax
	mov	qword ptr [rbp + 48], r8
	mov	dword ptr [rbp + 56], r12d
	mov	dword ptr [rbp + 60], r9d
	lea	rcx, [rbp + 48]
	vzeroupper
	call	r10
	movsxd	rcx, dword ptr [rbx + 8]
	mov	rax, qword ptr [rsi]
	cmp	dword ptr [rax + 8], 0
	setle	dl
	lea	r8d, [rcx - 3]
	cmp	r8d, -5
	setae	r8b
	or	r8b, dl
	jne	.LBB5_41
	imul	r8, rcx, 1431655766
	mov	rcx, r8
	shr	rcx, 63
	shr	r8, 32
	add	r8d, ecx
	imul	r14d, r8d
	movsxd	rcx, r14d
	lea	rcx, [r15 + 4*rcx]
	lea	rdx, [rdi + 16]
	mov	r8d, r8d
	mov	r9d, 1
	.p2align	4
.LBB5_37:
	mov	r10, qword ptr [rax]
	lea	r11, [rax + 16]
	test	r10, r10
	cmovne	r11, r10
	movsx	r10, word ptr [r11 + 2*r9 - 2]
	test	r10, r10
	js	.LBB5_35
	mov	rax, qword ptr [rdi]
	test	rax, rax
	cmove	rax, rdx
	vmovss	xmm0, dword ptr [rax + 4*r10]
	vmovss	dword ptr [rcx + 4*r9 - 4], xmm0
	mov	rax, qword ptr [rsi]
.LBB5_35:
	movsxd	r10, dword ptr [rax + 8]
	cmp	r9, r10
	jge	.LBB5_41
	cmp	r9, r8
	lea	r9, [r9 + 1]
	jb	.LBB5_37
.LBB5_41:
	.seh_startepilogue
	add	rsp, 200
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB5_25:
	cmp	r8d, 32
	jae	.LBB5_27
	xor	r8d, r8d
	mov	r10, qword ptr [rbp + 40]
	jmp	.LBB5_31
.LBB5_27:
	shl	r9, 2
	mov	r8d, edx
	and	r8d, 2147483616
	lea	r10, [r10 + r14 + 96]
	lea	r9, [r9 + r15 + 96]
	xor	r11d, r11d
.LBB5_28:
	vmovups	ymm0, ymmword ptr [r9 + 4*r11 - 96]
	vmovups	ymm1, ymmword ptr [r9 + 4*r11 - 64]
	vmovups	ymm2, ymmword ptr [r9 + 4*r11 - 32]
	vmovups	ymm3, ymmword ptr [r9 + 4*r11]
	vmovups	ymmword ptr [r10 + 4*r11 - 96], ymm0
	vmovups	ymmword ptr [r10 + 4*r11 - 64], ymm1
	vmovups	ymmword ptr [r10 + 4*r11 - 32], ymm2
	vmovups	ymmword ptr [r10 + 4*r11], ymm3
	add	r11, 32
	cmp	r8, r11
	jne	.LBB5_28
	cmp	r8d, edx
	je	.LBB5_20
	test	dl, 28
	mov	r10, qword ptr [rbp + 40]
	je	.LBB5_23
.LBB5_31:
	mov	r9, r8
	mov	r8d, edx
	and	r8d, 2147483644
	.p2align	4
.LBB5_32:
	vmovups	xmm0, xmmword ptr [rax + 4*r9]
	vmovups	xmmword ptr [rcx + 4*r9], xmm0
	add	r9, 4
	cmp	r8, r9
	jne	.LBB5_32
	cmp	r8d, edx
	jne	.LBB5_23
	jmp	.LBB5_34
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.MatchChannels(Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Unity.Entities.BlobAssetReference`1[[Latios.Mecanim.MecanimAnimatedPropertiesBlob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bindings, Unity.Entities.BlobAssetReference`1[[Latios.Kinemation.MeshDeformDataBlob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 88
	.seh_stackalloc 88
	lea	rbp, [rsp + 80]
	.seh_setframe rbp, 80
	.seh_endprologue
	mov	qword ptr [rbp - 8], rdx
	test	r8, r8
	je	.LBB6_1
	mov	rax, qword ptr [r8 - 16]
	mov	qword ptr [rcx + 8], rax
	test	r9, r9
	je	.LBB6_4
.LBB6_5:
	mov	rax, qword ptr [r9 - 16]
	jmp	.LBB6_6
.LBB6_1:
	xor	eax, eax
	mov	qword ptr [rcx + 8], rax
	test	r9, r9
	jne	.LBB6_5
.LBB6_4:
	xor	eax, eax
.LBB6_6:
	mov	qword ptr [rcx + 16], rax
	mov	byte ptr [rcx + 24], 0
	movzx	eax, word ptr [r9 + 188]
	test	rax, rax
	je	.LBB6_7
	mov	edx, 5381
	mov	r15d, edx
	.p2align	4
.LBB6_9:
	movzx	r10d, byte ptr [r9 + rax + 189]
	shl	r15d, 5
	add	r15d, edx
	add	r15d, r10d
	cmp	rax, 1
	lea	rax, [rax - 1]
	mov	edx, r15d
	ja	.LBB6_9
	jmp	.LBB6_10
.LBB6_7:
	mov	r15d, 5381
.LBB6_10:
	mov	rax, qword ptr [rbp - 8]
	mov	r12, qword ptr [rax]
	mov	dword ptr [r12 + 8], 0
	mov	r13d, dword ptr [r9 + 68]
	mov	eax, dword ptr [r12 + 12]
	cmp	r13d, eax
	mov	qword ptr [rbp - 32], r8
	jle	.LBB6_14
	lea	edx, [rax + rax]
	cmp	edx, r13d
	cmovle	edx, r13d
	cmp	edx, 9
	mov	ebx, 8
	cmovge	ebx, edx
	cmp	ebx, eax
	je	.LBB6_14
	mov	qword ptr [rbp - 24], rcx
	mov	qword ptr [rbp - 16], r9
	mov	rax, qword ptr [rbp - 8]
	movzx	ecx, byte ptr [rax + 57]
	mov	byte ptr [rbp + 6], cl
	movzx	eax, byte ptr [rax + 58]
	mov	byte ptr [rbp + 7], al
	lea	r14d, [rbx + rbx]
	mov	rdi, qword ptr [r12]
	lea	rax, [r12 + 16]
	test	rdi, rdi
	mov	qword ptr [rbp - 40], rax
	cmove	rdi, rax
	mov	qword ptr [rsp + 32], 0
	mov	rcx, r14
	mov	edx, 64
	mov	r8d, 4
	mov	r9d, 1
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	rsi, rax
	cmp	rdi, rax
	je	.LBB6_13
	cmp	byte ptr [rbp + 6], 1
	mov	eax, dword ptr [r12 + 12]
	jne	.LBB6_19
	lea	ecx, [rax + rax]
	movsxd	rcx, ecx
	sub	r14, rcx
	jle	.LBB6_19
	add	rcx, rsi
	movzx	edx, byte ptr [rbp + 7]
	mov	r8, r14
	xor	r9d, r9d
	call	burst.memset.inline.AVX2.i64@@32
	mov	eax, dword ptr [r12 + 12]
.LBB6_19:
	cmp	eax, ebx
	cmovge	eax, ebx
	movsxd	r8, eax
	add	r8, r8
	mov	rcx, rsi
	mov	rdx, rdi
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [r12]
	test	rcx, rcx
	je	.LBB6_13
	mov	edx, 4
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB6_13:
	xor	eax, eax
	cmp	rsi, qword ptr [rbp - 40]
	cmovne	rax, rsi
	mov	qword ptr [r12], rax
	mov	dword ptr [r12 + 12], ebx
	mov	rax, qword ptr [rbp - 8]
	mov	r12, qword ptr [rax]
	mov	r9, qword ptr [rbp - 16]
	mov	rcx, qword ptr [rbp - 24]
.LBB6_14:
	mov	dword ptr [r12 + 8], r13d
	cmp	dword ptr [r9 + 68], 0
	jle	.LBB6_36
	lea	rax, [r9 + 64]
	mov	qword ptr [rbp - 24], rax
	mov	r10, r9
	add	r10, 65
	mov	rax, qword ptr [rbp - 32]
	add	rax, 16
	mov	qword ptr [rbp - 16], rax
	xor	esi, esi
	jmp	.LBB6_29
	.p2align	4
.LBB6_34:
	mov	ebx, -1
.LBB6_28:
	mov	rax, qword ptr [rbp - 8]
	mov	rax, qword ptr [rax]
	mov	rdx, qword ptr [rax]
	add	rax, 16
	test	rdx, rdx
	cmovne	rax, rdx
	mov	word ptr [rax + 2*rsi], bx
	test	ebx, ebx
	setns	al
	or	byte ptr [rcx + 24], al
	inc	rsi
	movsxd	rax, dword ptr [r9 + 68]
	sub	r10, -128
	cmp	rsi, rax
	jge	.LBB6_36
.LBB6_29:
	mov	rax, qword ptr [rbp - 24]
	movsxd	r11, dword ptr [rax]
	add	rax, r11
	mov	rdx, rsi
	shl	rdx, 7
	movzx	ebx, word ptr [rdx + rax]
	test	rbx, rbx
	je	.LBB6_30
	add	r11, r10
	mov	eax, 5381
	mov	edi, eax
	.p2align	4
.LBB6_32:
	movzx	edx, byte ptr [r11 + rbx]
	shl	edi, 5
	add	edi, eax
	add	edi, edx
	cmp	rbx, 1
	lea	rbx, [rbx - 1]
	mov	eax, edi
	ja	.LBB6_32
	mov	rax, qword ptr [rbp - 32]
	movsxd	r14, dword ptr [rax + 12]
	test	r14, r14
	jg	.LBB6_35
	jmp	.LBB6_34
	.p2align	4
.LBB6_30:
	mov	edi, 5381
	mov	rax, qword ptr [rbp - 32]
	movsxd	r14, dword ptr [rax + 12]
	test	r14, r14
	jle	.LBB6_34
.LBB6_35:
	movsxd	r12, dword ptr [rax + 8]
	add	r12, qword ptr [rbp - 16]
	mov	ebx, -1
	xor	r13d, r13d
	mov	r11d, -1
	jmp	.LBB6_26
.LBB6_24:
	xor	r8d, r8d
	test	edx, edx
	setne	r8b
	cmp	eax, 1
	sbb	r8d, -1
	cmp	r8d, r11d
	cmovg	r11d, r8d
	cmovg	ebx, r13d
	.p2align	4
.LBB6_25:
	inc	r13
	add	r12, 12
	cmp	r14, r13
	je	.LBB6_28
.LBB6_26:
	cmp	dword ptr [r12 - 8], edi
	jne	.LBB6_25
	mov	eax, dword ptr [r12 - 4]
	mov	edx, dword ptr [r12]
	test	eax, eax
	je	.LBB6_23
	cmp	eax, r15d
	jne	.LBB6_25
.LBB6_23:
	test	edx, edx
	je	.LBB6_24
	cmp	edx, dword ptr [rcx]
	je	.LBB6_24
	jmp	.LBB6_25
.LBB6_36:
	.seh_startepilogue
	add	rsp, 88
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 936
	.seh_stackalloc 936
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovaps	xmmword ptr [rbp + 784], xmm12
	.seh_savexmm xmm12, 912
	vmovaps	xmmword ptr [rbp + 768], xmm11
	.seh_savexmm xmm11, 896
	vmovaps	xmmword ptr [rbp + 752], xmm10
	.seh_savexmm xmm10, 880
	vmovaps	xmmword ptr [rbp + 736], xmm9
	.seh_savexmm xmm9, 864
	vmovaps	xmmword ptr [rbp + 720], xmm8
	.seh_savexmm xmm8, 848
	vmovaps	xmmword ptr [rbp + 704], xmm7
	.seh_savexmm xmm7, 832
	vmovaps	xmmword ptr [rbp + 688], xmm6
	.seh_savexmm xmm6, 816
	.seh_endprologue
	mov	byte ptr [rbp + 687], r9b
	mov	rdi, rdx
	mov	rsi, rcx
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp + 360], ymm0
	vmovups	ymmword ptr [rbp + 392], ymm0
	vxorps	xmm1, xmm1, xmm1
	vmovups	xmmword ptr [rbp + 336], xmm1
	mov	qword ptr [rbp + 352], 0
	vmovups	ymmword ptr [rbp + 224], ymm0
	vmovups	ymmword ptr [rbp + 256], ymm0
	vmovups	xmmword ptr [rbp + 312], xmm1
	mov	qword ptr [rbp + 328], 0
	vmovups	xmmword ptr [rbp + 288], xmm1
	mov	qword ptr [rbp + 304], 0
	vmovups	ymmword ptr [rbp - 8], ymm0
	vmovups	ymmword ptr [rbp + 24], ymm0
	vmovups	ymmword ptr [rbp + 56], ymm0
	vmovups	ymmword ptr [rbp + 88], ymm0
	mov	qword ptr [rbp + 120], 0
	lea	rdx, [rcx + 136]
	mov	rcx, rdi
	vzeroupper
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	qword ptr [rbp + 488], rax
	lea	r8, [rsi + 200]
	cmp	byte ptr [rsi + 224], 1
	sete	r9b
	lea	rdx, [rbp + 536]
	mov	rcx, rdi
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	qword ptr [rbp + 624], rdi
	movsxd	rax, dword ptr [rdi]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	mov	rdx, rax
	shl	rdx, 4
	mov	rcx, qword ptr [rcx + rdx]
	cmp	qword ptr [rsi + 280], rcx
	mov	qword ptr [rbp + 672], rsi
	jne	.LBB7_6
	movzx	edx, word ptr [rsi + 294]
	cmp	dx, -1
	je	.LBB7_3
.LBB7_18:
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	rdx, rax
	sar	rdx, 3
	and	rdx, -8
	and	eax, 63
	shl	eax, 14
	add	rax, qword ptr [rcx + rdx]
	movsxd	rcx, dword ptr [rsi + 288]
	lea	rax, [rcx + rax + 64]
	mov	qword ptr [rbp + 616], rax
.LBB7_19:
	mov	rax, qword ptr [rbp + 536]
	mov	qword ptr [rbp + 496], rax
	movsxd	rax, dword ptr [rbp + 548]
	mov	qword ptr [rbp + 600], rax
	mov	eax, dword ptr [rbp + 552]
	mov	dword ptr [rbp + 632], eax
	movzx	ebx, byte ptr [rbp + 556]
	mov	rax, qword ptr [rbp + 560]
	mov	qword ptr [rbp + 432], rax
	vmovsd	xmm6, qword ptr [rbp + 568]
	mov	rax, qword ptr [rbp + 576]
	mov	qword ptr [rbp + 440], rax
	vmovsd	xmm7, qword ptr [rbp + 584]
	lea	r8, [rsi + 344]
	lea	rdx, [rbp - 56]
	mov	r14, qword ptr [rbp + 624]
	mov	rcx, r14
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rax, qword ptr [rbp - 56]
	mov	qword ptr [rbp + 504], rax
	mov	rax, qword ptr [rbp - 32]
	mov	qword ptr [rbp + 448], rax
	vmovsd	xmm8, qword ptr [rbp - 24]
	lea	r8, [rsi + 408]
	cmp	byte ptr [rsi + 432], 1
	sete	r9b
	lea	rdx, [rbp + 536]
	mov	rcx, r14
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, qword ptr [rbp + 536]
	mov	qword ptr [rbp + 512], rax
	movsxd	rax, dword ptr [rbp + 548]
	mov	qword ptr [rbp + 608], rax
	mov	eax, dword ptr [rbp + 552]
	mov	dword ptr [rbp + 636], eax
	mov	rdi, rsi
	movzx	esi, byte ptr [rbp + 556]
	mov	rax, qword ptr [rbp + 560]
	mov	qword ptr [rbp + 456], rax
	vmovsd	xmm9, qword ptr [rbp + 568]
	mov	rax, qword ptr [rbp + 576]
	mov	qword ptr [rbp + 464], rax
	vmovsd	xmm10, qword ptr [rbp + 584]
	lea	r8, [rdi + 488]
	lea	rdx, [rbp + 176]
	mov	rcx, r14
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rax, qword ptr [rbp + 176]
	mov	qword ptr [rbp + 520], rax
	mov	eax, dword ptr [rbp + 184]
	mov	dword ptr [rbp + 640], eax
	mov	rax, qword ptr [rbp + 200]
	mov	qword ptr [rbp + 472], rax
	vmovsd	xmm11, qword ptr [rbp + 208]
	lea	r8, [rdi + 552]
	lea	rdx, [rbp + 128]
	mov	rcx, r14
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rax, qword ptr [rbp + 128]
	mov	qword ptr [rbp + 528], rax
	mov	eax, dword ptr [rbp + 136]
	mov	dword ptr [rbp + 644], eax
	mov	rax, qword ptr [rbp + 152]
	mov	qword ptr [rbp + 480], rax
	vmovsd	xmm12, qword ptr [rbp + 160]
	mov	rax, qword ptr [rbp + 912]
	vmovups	xmm2, xmmword ptr [rax]
	movsxd	rax, dword ptr [r14]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	shl	rax, 4
	mov	r9d, dword ptr [rcx + rax + 8]
	lea	rcx, [rbp - 8]
	movzx	edx, byte ptr [rbp + 687]
	call	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	eax, dword ptr [rbp + 120]
	mov	ecx, dword ptr [rbp + 124]
	inc	eax
	mov	dword ptr [rbp + 120], eax
	cmp	eax, ecx
	jge	.LBB7_4
	xor	edx, edx
	xor	r8d, r8d
	cmp	bl, 1
	setne	dl
	sete	r8b
	mov	dword ptr [rbp + 652], r8d
	add	r8d, r8d
	mov	dword ptr [rbp + 656], r8d
	add	edx, edx
	mov	dword ptr [rbp + 648], edx
	xor	edx, edx
	xor	r8d, r8d
	cmp	sil, 1
	setne	dl
	sete	r8b
	mov	dword ptr [rbp + 664], r8d
	add	r8d, r8d
	mov	dword ptr [rbp + 668], r8d
	add	edx, edx
	mov	dword ptr [rbp + 660], edx
	xor	r9d, r9d
	jmp	.LBB7_21
	.p2align	4
.LBB7_28:
	mov	eax, dword ptr [rbp + 120]
	mov	ecx, dword ptr [rbp + 124]
.LBB7_29:
	inc	eax
	mov	dword ptr [rbp + 120], eax
	cmp	eax, ecx
	jge	.LBB7_4
.LBB7_21:
	movsxd	rdx, eax
	movzx	r8d, byte ptr [rbp + rdx + 56]
	test	r8, r8
	je	.LBB7_29
	movzx	r13d, byte ptr [rbp + rdx - 8]
	mov	rsi, qword ptr [rbp + 608]
	imul	rsi, r13
	add	rsi, qword ptr [rbp + 512]
	mov	rax, qword ptr [rbp + 504]
	lea	rbx, [rax + r13]
	lea	r12d, [8*r13]
	mov	rax, qword ptr [rbp + 528]
	lea	r14, [rax + r12]
	add	r12, qword ptr [rbp + 616]
	mov	rdi, qword ptr [rbp + 600]
	imul	rdi, r13
	shl	r13d, 5
	mov	rax, qword ptr [rbp + 520]
	lea	r15, [rax + r13]
	add	r13, qword ptr [rbp + 488]
	add	rdi, qword ptr [rbp + 496]
	jmp	.LBB7_23
	.p2align	4
.LBB7_27:
	mov	qword ptr [rbp + 336], rbx
	mov	r9, qword ptr [rbp + 448]
	mov	qword ptr [rbp + 344], r9
	vmovlps	qword ptr [rbp + 352], xmm8
	mov	qword ptr [rbp + 224], rsi
	mov	r9d, dword ptr [rbp + 636]
	mov	dword ptr [rbp + 232], r9d
	mov	r9, qword ptr [rbp + 456]
	mov	qword ptr [rbp + 240], r9
	vmovlps	qword ptr [rbp + 248], xmm9
	mov	r9, qword ptr [rbp + 464]
	mov	qword ptr [rbp + 256], r9
	vmovlps	qword ptr [rbp + 264], xmm10
	mov	r9d, dword ptr [rbp + 668]
	mov	dword ptr [rbp + 272], r9d
	mov	r9d, dword ptr [rbp + 660]
	mov	dword ptr [rbp + 276], r9d
	mov	r9d, dword ptr [rbp + 664]
	mov	byte ptr [rbp + 280], r9b
	mov	dword ptr [rbp + 281], 0
	mov	word ptr [rbp + 285], 0
	mov	byte ptr [rbp + 287], 0
	mov	qword ptr [rbp + 312], rax
	mov	qword ptr [rbp + 320], rcx
	vmovlps	qword ptr [rbp + 328], xmm1
	mov	qword ptr [rbp + 288], rdx
	mov	qword ptr [rbp + 296], r8
	vmovlps	qword ptr [rbp + 304], xmm0
	lea	rax, [rbp + 288]
	mov	qword ptr [rsp + 56], rax
	lea	rax, [rbp + 312]
	mov	qword ptr [rsp + 48], rax
	lea	rax, [rbp + 224]
	mov	qword ptr [rsp + 40], rax
	lea	rax, [rbp + 336]
	mov	qword ptr [rsp + 32], rax
	mov	rcx, qword ptr [rbp + 672]
	mov	rdx, r13
	lea	r8, [rbp + 360]
	mov	r9, r12
	call	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Mecanim.MecanimBlendShapeListener&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null listener, Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null channels, Latios.Kinemation.MeshDeformDataBlobReference&, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null mesh, Unity.Entities.RefRW`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeState, Unity.Entities.DynamicBuffer`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null blendShapeWeights, Unity.Entities.RefRO`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null skeletonDependent, Unity.Entities.RefRO`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null source) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@64"
	add	rsi, qword ptr [rbp + 608]
	inc	rbx
	add	r14, 8
	add	r12, 8
	add	r15, 32
	add	r13, 32
	add	rdi, qword ptr [rbp + 600]
	mov	r8, qword ptr [rbp + 424]
	dec	r8
	mov	r9d, 0
	je	.LBB7_28
.LBB7_23:
	mov	qword ptr [rbp + 424], r8
	cmp	dword ptr [rbp + 640], 0
	mov	rax, r15
	cmove	rax, r9
	mov	rcx, qword ptr [rbp + 472]
	cmove	rcx, r9
	vxorps	xmm0, xmm0, xmm0
	vxorps	xmm1, xmm1, xmm1
	je	.LBB7_25
	vmovaps	xmm1, xmm11
.LBB7_25:
	cmp	dword ptr [rbp + 644], 0
	mov	qword ptr [rbp + 360], rdi
	mov	edx, dword ptr [rbp + 632]
	mov	dword ptr [rbp + 368], edx
	mov	dword ptr [rbp + 372], 0
	mov	rdx, qword ptr [rbp + 432]
	mov	qword ptr [rbp + 376], rdx
	vmovlps	qword ptr [rbp + 384], xmm6
	mov	rdx, qword ptr [rbp + 440]
	mov	qword ptr [rbp + 392], rdx
	vmovlps	qword ptr [rbp + 400], xmm7
	mov	edx, dword ptr [rbp + 656]
	mov	dword ptr [rbp + 408], edx
	mov	edx, dword ptr [rbp + 648]
	mov	dword ptr [rbp + 412], edx
	mov	edx, dword ptr [rbp + 652]
	mov	byte ptr [rbp + 416], dl
	mov	dword ptr [rbp + 417], 0
	mov	word ptr [rbp + 421], 0
	mov	byte ptr [rbp + 423], 0
	mov	rdx, r14
	cmove	rdx, r9
	mov	r8, qword ptr [rbp + 480]
	cmove	r8, r9
	je	.LBB7_27
	vmovaps	xmm0, xmm12
	jmp	.LBB7_27
.LBB7_4:
	mov	r8, qword ptr [rbp + 672]
	mov	eax, dword ptr [r8 + 624]
	test	eax, eax
	js	.LBB7_5
	cmp	byte ptr [rbp + 687], 0
	je	.LBB7_32
	mov	rcx, qword ptr [rbp + 912]
	popcnt	rdx, qword ptr [rcx]
	popcnt	rcx, qword ptr [rcx + 8]
	add	ecx, edx
	jmp	.LBB7_33
.LBB7_5:
	xor	ecx, ecx
	jmp	.LBB7_33
.LBB7_32:
	mov	rcx, qword ptr [rbp + 624]
	movsxd	rcx, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rdx, qword ptr [rdx]
	shl	rcx, 4
	mov	ecx, dword ptr [rdx + rcx + 8]
.LBB7_33:
	add	ecx, eax
	mov	dword ptr [r8 + 624], ecx
	vmovaps	xmm6, xmmword ptr [rbp + 688]
	vmovaps	xmm7, xmmword ptr [rbp + 704]
	vmovaps	xmm8, xmmword ptr [rbp + 720]
	vmovaps	xmm9, xmmword ptr [rbp + 736]
	vmovaps	xmm10, xmmword ptr [rbp + 752]
	vmovaps	xmm11, xmmword ptr [rbp + 768]
	vmovaps	xmm12, xmmword ptr [rbp + 784]
	.seh_startepilogue
	add	rsp, 936
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB7_6:
	mov	r8d, dword ptr [rsi + 296]
	mov	r9, qword ptr [rcx + 112]
	mov	r10d, dword ptr [rcx + 168]
	movsx	edx, word ptr [rsi + 294]
	test	edx, edx
	js	.LBB7_8
	cmp	r10d, edx
	jle	.LBB7_8
	movsxd	r11, edx
	cmp	dword ptr [r9 + 4*r11], r8d
	je	.LBB7_16
.LBB7_8:
	test	r10d, r10d
	je	.LBB7_13
	xor	edx, edx
.LBB7_11:
	movsxd	r11, edx
	cmp	r8d, dword ptr [r9 + 4*r11]
	je	.LBB7_12
	inc	edx
	cmp	r10d, edx
	jne	.LBB7_11
.LBB7_13:
	mov	rdx, qword ptr [rbp + 672]
	mov	word ptr [rdx + 294], -1
.LBB7_14:
	mov	rdx, qword ptr [rbp + 672]
	mov	dword ptr [rdx + 288], 0
	mov	dx, -1
	xor	r8d, r8d
	jmp	.LBB7_17
.LBB7_12:
	mov	r8, qword ptr [rbp + 672]
	mov	word ptr [r8 + 294], dx
	cmp	dx, -1
	je	.LBB7_14
.LBB7_16:
	mov	r8, qword ptr [rcx + 224]
	movsx	r9, dx
	mov	r8d, dword ptr [r8 + 4*r9]
	mov	r10, qword ptr [rbp + 672]
	mov	dword ptr [r10 + 288], r8d
	mov	r8, qword ptr [rcx + 232]
	movzx	r8d, word ptr [r8 + 2*r9]
.LBB7_17:
	mov	rsi, qword ptr [rbp + 672]
	mov	word ptr [rsi + 292], r8w
	mov	qword ptr [rsi + 280], rcx
	cmp	dx, -1
	jne	.LBB7_18
.LBB7_3:
	mov	qword ptr [rbp + 616], 0
	jmp	.LBB7_19
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 24
	.seh_stackalloc 24
	lea	rbp, [rsp + 16]
	.seh_setframe rbp, 16
	.seh_endprologue
	mov	r14, r9
	mov	esi, r8d
	mov	rdi, rdx
	mov	rbx, rcx
	cmp	byte ptr [rbp + 112], 3
	mov	eax, 16
	mov	r12d, 10
	cmove	r12, rax
	mov	r13d, -1
	mov	rax, r9
	.p2align	4
.LBB8_1:
	cqo
	idiv	r12
	inc	r13d
	test	rax, rax
	jne	.LBB8_1
	lea	r8d, [r13 + 2]
	lea	rax, [r8 + 15]
	and	rax, -16
	call	__chkstk
	sub	rsp, rax
	mov	r15, rsp
	sub	rsp, 32
	mov	rcx, r15
	xor	edx, edx
	xor	r9d, r9d
	call	burst.memset.inline.AVX2.i32@@32
	add	rsp, 32
	xor	ecx, ecx
	cmp	byte ptr [rbp + 136], 0
	setne	cl
	shl	ecx, 5
	add	ecx, 55
	lea	r8d, [r13 + 1]
	mov	rax, r14
	.p2align	4
.LBB8_3:
	cqo
	idiv	r12
	movsxd	r9, r13d
	dec	r13d
	mov	r10d, edx
	neg	r10d
	cmovs	r10d, edx
	lea	edx, [r10 + 48]
	lea	r11d, [rcx + r10]
	cmp	r10d, 10
	movzx	edx, dl
	cmovge	edx, r11d
	mov	byte ptr [r15 + r9], dl
	test	rax, rax
	jne	.LBB8_3
	mov	qword ptr [rbp], 0
	movsxd	rax, r8d
	mov	byte ptr [r15 + rax], 0
	mov	dword ptr [rbp - 8], 0
	mov	qword ptr [rbp - 16], r15
	mov	dword ptr [rbp - 4], eax
	mov	dword ptr [rbp], eax
	shr	r14, 63
	mov	byte ptr [rbp + 4], r14b
	movzx	eax, byte ptr [rbp + 128]
	sub	rsp, 80
	movzx	ecx, byte ptr [rbp + 136]
	mov	byte ptr [rsp + 64], cl
	mov	byte ptr [rsp + 56], al
	movzx	ecx, byte ptr [rbp + 120]
	mov	byte ptr [rsp + 48], cl
	movzx	ecx, byte ptr [rbp + 112]
	mov	byte ptr [rsp + 40], cl
	mov	dword ptr [rsp + 32], eax
	lea	r9, [rbp - 16]
	mov	rcx, rbx
	mov	rdx, rdi
	mov	r8d, esi
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	nop
	.seh_startepilogue
	lea	rsp, [rbp + 8]
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 24
	.seh_stackalloc 24
	lea	rbp, [rsp + 16]
	.seh_setframe rbp, 16
	.seh_endprologue
	mov	r14, r9
	mov	esi, r8d
	mov	rdi, rdx
	mov	rbx, rcx
	cmp	byte ptr [rbp + 112], 3
	mov	eax, 16
	mov	r12d, 10
	cmove	r12, rax
	mov	r13d, -1
	mov	rcx, r9
	.p2align	4
.LBB9_1:
	mov	rax, rcx
	xor	edx, edx
	div	r12
	inc	r13d
	cmp	rcx, r12
	mov	rcx, rax
	jae	.LBB9_1
	lea	r8d, [r13 + 2]
	lea	rax, [r8 + 15]
	and	rax, -16
	call	__chkstk
	sub	rsp, rax
	mov	r15, rsp
	sub	rsp, 32
	mov	rcx, r15
	xor	edx, edx
	xor	r9d, r9d
	call	burst.memset.inline.AVX2.i32@@32
	add	rsp, 32
	cmp	byte ptr [rbp + 136], 0
	setne	cl
	shl	cl, 5
	add	cl, 55
	lea	r8d, [r13 + 1]
	.p2align	4
.LBB9_3:
	mov	rax, r14
	xor	edx, edx
	div	r12
	movsxd	r9, r13d
	dec	r13d
	lea	r10d, [rcx + rdx]
	mov	r11d, edx
	or	r11b, 48
	cmp	rdx, 10
	movzx	edx, r10b
	movzx	r10d, r11b
	cmovae	r10d, edx
	mov	byte ptr [r15 + r9], r10b
	cmp	r14, r12
	mov	r14, rax
	jae	.LBB9_3
	mov	qword ptr [rbp], 0
	movsxd	rax, r8d
	mov	byte ptr [r15 + rax], 0
	mov	dword ptr [rbp - 8], 0
	mov	qword ptr [rbp - 16], r15
	mov	dword ptr [rbp - 4], eax
	mov	dword ptr [rbp], eax
	movzx	eax, byte ptr [rbp + 128]
	sub	rsp, 80
	movzx	ecx, byte ptr [rbp + 136]
	mov	byte ptr [rsp + 64], cl
	mov	byte ptr [rsp + 56], al
	movzx	ecx, byte ptr [rbp + 120]
	mov	byte ptr [rsp + 48], cl
	movzx	ecx, byte ptr [rbp + 112]
	mov	byte ptr [rsp + 40], cl
	mov	dword ptr [rsp + 32], eax
	lea	r9, [rbp - 16]
	mov	rcx, rbx
	mov	rdx, rdi
	mov	r8d, esi
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	nop
	.seh_startepilogue
	lea	rsp, [rbp + 8]
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	.seh_endprologue
	mov	eax, dword ptr [rbp + 64]
	mov	r11d, eax
	shr	r11d, 8
	mov	r10d, eax
	shr	r10d, 16
	mov	esi, eax
	shr	esi, 24
	cmp	al, 3
	jne	.LBB10_1
	mov	r9d, r9d
	mov	byte ptr [rsp + 40], r11b
	mov	byte ptr [rsp + 32], al
	mov	byte ptr [rsp + 48], r10b
	mov	byte ptr [rsp + 56], sil
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertUnsignedIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	nop
	.seh_startepilogue
	add	rsp, 72
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
.LBB10_1:
	movsxd	r9, r9d
	mov	byte ptr [rsp + 40], r11b
	mov	byte ptr [rsp + 32], al
	mov	byte ptr [rsp + 48], r10b
	mov	byte ptr [rsp + 56], sil
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	nop
	.seh_startepilogue
	add	rsp, 72
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.p2align	4
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	push	rbp
	.seh_pushreg rbp
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 64
	.seh_stackalloc 64
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	.seh_endprologue
	mov	r11d, dword ptr [r9 + 16]
	cmp	r11d, dword ptr [rbp + 80]
	setle	r10b
	cmp	r11d, -3
	setge	al
	and	al, r10b
	mov	esi, 1
	cmovne	esi, r11d
	mov	r10, qword ptr [r9]
	cmp	byte ptr [r9 + 20], 0
	je	.LBB11_1
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB11_23
	lea	ebx, [rdi + 1]
	mov	dword ptr [rdx], ebx
	mov	byte ptr [rcx + rdi], 45
.LBB11_1:
	test	esi, esi
	jle	.LBB11_14
	inc	esi
	mov	r11d, 48
	.p2align	4
.LBB11_3:
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB11_23
	lea	ebx, [rdi + 1]
	mov	dword ptr [rdx], ebx
	movzx	ebx, byte ptr [r10]
	xor	r14d, r14d
	test	bl, bl
	setne	r14b
	cmove	ebx, r11d
	add	r10, r14
	mov	byte ptr [rcx + rdi], bl
	dec	esi
	cmp	esi, 1
	jg	.LBB11_3
	xor	r11d, r11d
	test	r11d, r11d
	jns	.LBB11_7
	jmp	.LBB11_8
.LBB11_14:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB11_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	test	r11d, r11d
	js	.LBB11_8
.LBB11_7:
	cmp	byte ptr [r10], 0
	jne	.LBB11_8
.LBB11_20:
	test	al, al
	jne	.LBB11_23
	movsxd	rax, dword ptr [rdx]
	cmp	eax, r8d
	jge	.LBB11_23
	movzx	r10d, byte ptr [rbp + 88]
	lea	r11d, [rax + 1]
	mov	dword ptr [rdx], r11d
	mov	byte ptr [rcx + rax], r10b
	mov	eax, dword ptr [r9 + 16]
	dec	eax
	movsxd	r9, eax
	mov	byte ptr [rsp + 56], 0
	mov	byte ptr [rsp + 48], 2
	mov	byte ptr [rsp + 40], 0
	mov	byte ptr [rsp + 32], 2
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ConvertIntegerToString(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	jmp	.LBB11_23
.LBB11_8:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB11_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 46
	test	r11d, r11d
	js	.LBB11_17
.LBB11_10:
	cmp	byte ptr [r10], 0
	je	.LBB11_20
	inc	r10
	.p2align	4
.LBB11_19:
	movsxd	r11, dword ptr [rdx]
	cmp	r11d, r8d
	jge	.LBB11_23
	lea	esi, [r11 + 1]
	mov	dword ptr [rdx], esi
	movzx	ebx, byte ptr [r10 - 1]
	mov	byte ptr [rcx + r11], bl
	cmp	byte ptr [r10], 0
	lea	r10, [r10 + 1]
	jne	.LBB11_19
	jmp	.LBB11_20
	.p2align	4
.LBB11_16:
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	inc	r11d
	je	.LBB11_10
.LBB11_17:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jl	.LBB11_16
.LBB11_23:
	.seh_startepilogue
	add	rsp, 64
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.globl	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.p2align	4
"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48":
.seh_proc "Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatNumber(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, Unity.Burst.BurstString+FormatOptions, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null options) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	.seh_endprologue
	mov	rsi, rdx
	movzx	eax, byte ptr [rbp + 136]
	movzx	edx, byte ptr [rbp + 120]
	movzx	r15d, byte ptr [rbp + 128]
	mov	r10d, dword ptr [r9 + 8]
	test	r10d, r10d
	setne	r11b
	test	dl, dl
	setne	bl
	or	bl, r11b
	test	al, al
	setne	dil
	or	dil, bl
	mov	r11b, 1
	cmp	dil, 1
	jne	.LBB12_4
	movzx	r11d, dl
	add	r11d, -4
	cmp	r11d, -4
	mov	r11d, edx
	ja	.LBB12_4
	mov	edi, dword ptr [rbp + 112]
	test	edi, edi
	jle	.LBB12_26
	mov	rax, qword ptr [r9]
	jmp	.LBB12_27
.LBB12_4:
	mov	edx, dword ptr [r9 + 12]
	movzx	eax, al
	mov	edi, edx
	sub	edi, eax
	mov	r14d, eax
	cmovg	r14d, edx
	neg	edi
	xor	r10d, r10d
	cmp	eax, edx
	cmovg	r10d, edi
	cmp	r11b, 2
	sete	dl
	movzx	ebx, byte ptr [r9 + 20]
	xor	eax, eax
	or	dl, bl
	setne	al
	add	eax, r14d
	movsx	edx, r15b
	test	dl, dl
	jle	.LBB12_9
	mov	edi, edx
	sub	edi, eax
	jle	.LBB12_9
	.p2align	4
.LBB12_6:
	movsxd	rbx, dword ptr [rsi]
	cmp	ebx, r8d
	jge	.LBB12_72
	lea	r14d, [rbx + 1]
	mov	dword ptr [rsi], r14d
	mov	byte ptr [rcx + rbx], 32
	dec	edi
	jne	.LBB12_6
	movzx	ebx, byte ptr [r9 + 20]
.LBB12_9:
	test	bl, bl
	je	.LBB12_11
	mov	r11d, dword ptr [rsi]
	mov	bl, 45
	cmp	r11d, r8d
	jl	.LBB12_13
	jmp	.LBB12_21
.LBB12_11:
	cmp	r11b, 2
	jne	.LBB12_14
	mov	r11d, dword ptr [rsi]
	mov	bl, 43
	cmp	r11d, r8d
	jge	.LBB12_21
.LBB12_13:
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movsxd	r11, r11d
	mov	byte ptr [rcx + r11], bl
.LBB12_14:
	test	r10d, r10d
	jle	.LBB12_17
	.p2align	4
.LBB12_15:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB12_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	mov	byte ptr [rcx + r11], 48
	dec	r10d
	jne	.LBB12_15
.LBB12_17:
	movsxd	r10, dword ptr [r9 + 12]
	test	r10, r10
	jle	.LBB12_21
	mov	r9, qword ptr [r9]
	.p2align	4
.LBB12_19:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB12_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movzx	ebx, byte ptr [r9]
	mov	byte ptr [rcx + r11], bl
	inc	r9
	dec	r10
	jne	.LBB12_19
.LBB12_21:
	test	r15b, r15b
	jns	.LBB12_72
	mov	r9d, edx
	neg	r9d
	cmp	eax, r9d
	jge	.LBB12_72
	add	eax, edx
	neg	eax
	.p2align	4
.LBB12_24:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB12_72
	lea	r9d, [rdx + 1]
	mov	dword ptr [rsi], r9d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB12_24
	jmp	.LBB12_72
.LBB12_26:
	mov	edi, dword ptr [r9 + 12]
	mov	rax, qword ptr [r9]
	test	edi, edi
	jle	.LBB12_38
.LBB12_27:
	mov	edx, edi
	xor	r11d, r11d
	.p2align	4
.LBB12_28:
	cmp	byte ptr [rax + r11], 0
	je	.LBB12_39
	inc	r11
	cmp	rdx, r11
	jne	.LBB12_28
	mov	edi, edx
	cmp	r10d, 1
	jne	.LBB12_31
	jmp	.LBB12_41
.LBB12_38:
	xor	r11d, r11d
.LBB12_39:
	cmp	r11d, edi
	jne	.LBB12_42
	mov	edx, edi
	cmp	r10d, 1
	je	.LBB12_41
.LBB12_31:
	cmp	byte ptr [rax + rdx], 53
	jb	.LBB12_41
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB12_33:
	test	edx, edx
	jle	.LBB12_36
	movzx	r11d, byte ptr [rax + rdx - 1]
	dec	rdx
	dec	r10d
	cmp	r11b, 57
	je	.LBB12_33
	inc	r11b
	mov	byte ptr [rax + rdx], r11b
	jmp	.LBB12_49
.LBB12_41:
	mov	r11d, edi
	jmp	.LBB12_43
.LBB12_42:
	mov	edx, r11d
.LBB12_43:
	mov	ebx, r11d
	sar	ebx, 31
	and	ebx, r11d
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB12_44:
	test	edx, edx
	jle	.LBB12_46
	dec	r10d
	cmp	byte ptr [rax + rdx - 1], 48
	lea	rdx, [rdx - 1]
	je	.LBB12_44
	jmp	.LBB12_49
.LBB12_46:
	test	rdx, rdx
	je	.LBB12_48
	mov	r10d, ebx
	jmp	.LBB12_49
.LBB12_36:
	inc	dword ptr [r9 + 16]
	mov	byte ptr [rax], 49
	mov	r10d, 1
	jmp	.LBB12_49
.LBB12_48:
	mov	dword ptr [r9 + 16], 0
	xor	r10d, r10d
.LBB12_49:
	movsxd	rdx, r10d
	mov	byte ptr [rax + rdx], 0
	mov	dword ptr [r9 + 12], edx
	mov	ebx, dword ptr [r9 + 16]
	cmp	ebx, edi
	setle	al
	cmp	ebx, -3
	setge	r12b
	and	r12b, al
	mov	r15d, 1
	mov	edx, 1
	cmovne	edx, ebx
	mov	rax, qword ptr [r9]
	movzx	r11d, byte ptr [r9 + 20]
	xor	r10d, r10d
	test	r11b, r11b
	setne	r13b
	test	edx, edx
	jle	.LBB12_53
	mov	r10b, r13b
	lea	r11d, [rdx + 1]
	.p2align	4
.LBB12_51:
	cmp	byte ptr [rax], 1
	sbb	rax, -1
	dec	r11d
	cmp	r11d, 1
	ja	.LBB12_51
	add	edx, r10d
	xor	r13d, r13d
	mov	r15d, edx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	jns	.LBB12_54
	jmp	.LBB12_57
.LBB12_53:
	cmp	r11b, 1
	sbb	r15d, -1
	mov	r13d, ebx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	js	.LBB12_57
.LBB12_54:
	test	dl, dl
	jne	.LBB12_57
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	je	.LBB12_61
.LBB12_56:
	movsx	r12d, r14b
	test	r14b, r14b
	jg	.LBB12_64
	jmp	.LBB12_67
.LBB12_57:
	mov	r10d, r13d
	sar	r10d, 31
	andn	r10d, r10d, r13d
	test	dl, dl
	je	.LBB12_59
	add	r15d, r10d
	inc	rax
	mov	qword ptr [rbp - 8], rcx
	mov	rcx, rax
	mov	dword ptr [rbp + 4], edi
	mov	edi, r8d
	mov	r14, r9
	call	strlen
	mov	r9, r14
	mov	rcx, qword ptr [rbp - 8]
	mov	r8d, edi
	mov	edi, dword ptr [rbp + 4]
	lea	r15d, [r15 + rax + 2]
	sub	r15d, r13d
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	jne	.LBB12_56
	jmp	.LBB12_61
.LBB12_59:
	lea	r15d, [r15 + r10 + 1]
	sub	r15d, r13d
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	jne	.LBB12_56
.LBB12_61:
	dec	ebx
	movsxd	r11, ebx
	xor	ebx, ebx
	movabs	r12, 7378697629483820647
	mov	r10, r11
	.p2align	4
.LBB12_62:
	mov	rax, r10
	imul	r12
	mov	rax, rdx
	shr	rax, 63
	sar	rdx, 2
	add	rdx, rax
	inc	ebx
	add	r10, 9
	cmp	r10, 18
	mov	r10, rdx
	ja	.LBB12_62
	mov	eax, r11d
	sar	eax, 31
	shr	r11, 63
	add	r11d, eax
	cmp	ebx, 3
	mov	eax, 2
	cmovge	eax, ebx
	add	r11d, r15d
	lea	r15d, [rax + r11 + 2]
	movsx	r12d, r14b
	test	r14b, r14b
	jle	.LBB12_67
.LBB12_64:
	mov	eax, r12d
	sub	eax, r15d
	jle	.LBB12_67
	.p2align	4
.LBB12_65:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB12_72
	lea	r10d, [rdx + 1]
	mov	dword ptr [rsi], r10d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB12_65
.LBB12_67:
	cmp	byte ptr [rbp + 144], 0
	setne	al
	shl	al, 5
	or	al, 69
	mov	dword ptr [rsp + 32], edi
	mov	byte ptr [rsp + 40], al
	mov	rdi, rcx
	mov	rdx, rsi
	mov	ebx, r8d
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.FormatGeneral(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, Unity.Burst.BurstString+NumberBuffer&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null number, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 nMaxDigits, System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 expChar) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	test	r14b, r14b
	jns	.LBB12_72
	mov	eax, r12d
	neg	eax
	cmp	r15d, eax
	jge	.LBB12_72
	add	r15d, r12d
	neg	r15d
	.p2align	4
.LBB12_70:
	movsxd	rax, dword ptr [rsi]
	cmp	eax, ebx
	jge	.LBB12_72
	lea	ecx, [rax + 1]
	mov	dword ptr [rsi], ecx
	mov	byte ptr [rdi + rax], 32
	dec	r15d
	jne	.LBB12_70
.LBB12_72:
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Kinemation.BlendShapeWeight, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 48
	.seh_stackalloc 48
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	mov	rax, rdx
	test	r9b, r9b
	jne	.LBB13_2
	cmp	byte ptr [r8 + 24], 1
	je	.LBB13_27
.LBB13_2:
	movsxd	r10, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rdx]
	shl	r10, 4
	mov	r10, qword ptr [r11 + r10]
	cmp	qword ptr [r8], r10
	jne	.LBB13_7
	movzx	r11d, word ptr [r8 + 14]
	test	r9b, r9b
	je	.LBB13_19
.LBB13_5:
	cmp	r11w, -1
	je	.LBB13_28
	mov	r9d, dword ptr [rcx]
	mov	r11d, r9d
	sar	r11d, 6
	movsxd	r11, r11d
	shl	r11, 3
	jmp	.LBB13_21
.LBB13_7:
	mov	edi, dword ptr [r8 + 16]
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	mov	rsi, r8
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB13_9
	cmp	r14d, r11d
	jle	.LBB13_9
	movsxd	r8, r11d
	cmp	dword ptr [rbx + 4*r8], edi
	je	.LBB13_17
.LBB13_9:
	test	r14d, r14d
	je	.LBB13_14
	xor	r11d, r11d
.LBB13_12:
	movsxd	r8, r11d
	cmp	edi, dword ptr [rbx + 4*r8]
	je	.LBB13_13
	inc	r11d
	cmp	r14d, r11d
	jne	.LBB13_12
.LBB13_14:
	mov	word ptr [rsi + 14], -1
.LBB13_15:
	mov	dword ptr [rsi + 8], 0
	mov	r11w, -1
	xor	edi, edi
	jmp	.LBB13_18
.LBB13_27:
	lea	rcx, [rip + burst.string.1ea13c7f7ea144ea32205d7ec317f39f]
	lea	rdx, [rip + burst.string.cd55498fb82486272ef3dc32c0931ef8]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB13_13:
	mov	word ptr [rsi + 14], r11w
	cmp	r11w, -1
	je	.LBB13_15
.LBB13_17:
	mov	r8, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	r8d, dword ptr [r8 + 4*rdi]
	mov	dword ptr [rsi + 8], r8d
	mov	r8, qword ptr [r10 + 232]
	movzx	edi, word ptr [r8 + 2*rdi]
.LBB13_18:
	mov	r8, rsi
	mov	word ptr [rsi + 12], di
	mov	qword ptr [rsi], r10
	test	r9b, r9b
	jne	.LBB13_5
.LBB13_19:
	cmp	r11w, -1
	je	.LBB13_28
	mov	esi, dword ptr [r8 + 20]
	movsxd	r9, dword ptr [rcx]
	movsx	r11d, r11w
	mov	rdi, qword ptr [rdx]
	mov	rbx, r9
	shl	rbx, 4
	movsxd	rdi, dword ptr [rdi + rbx + 12]
	mov	ebx, dword ptr [r10 + 8]
	lea	r14d, [4*rbx]
	movsxd	r14, r14d
	add	r14, qword ptr [r10]
	imul	r11d, ebx
	movsxd	r11, r11d
	lea	r11, [r14 + 4*r11]
	mov	dword ptr [r11 + 4*rdi], esi
	mov	r11, r9
	sar	r11, 3
	and	r11, -8
.LBB13_21:
	add	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	and	r9d, 63
	shl	r9d, 14
	add	r9, qword ptr [r11]
	movsxd	rsi, dword ptr [r8 + 8]
	add	rsi, r9
	add	rsi, 64
	je	.LBB13_28
	movsx	r9, word ptr [r8 + 14]
	mov	r10, qword ptr [r10 + 240]
	mov	edi, dword ptr [r10 + 4*r9]
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	ebx, dword ptr [rdx + r9 + 8]
	movzx	r14d, word ptr [r8 + 12]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB13_25
.LBB13_23:
	xor	ecx, ecx
	xor	r10d, r10d
	cmp	byte ptr [r8 + 24], 1
	setne	cl
	sete	r10b
	mov	rdx, qword ptr [r8 + 40]
	vmovsd	xmm0, qword ptr [r8 + 48]
	mov	r9, qword ptr [r8 + 56]
	vmovsd	xmm1, qword ptr [r8 + 64]
	sete	byte ptr [rax + 20]
	add	r10d, r10d
	add	ecx, ecx
	mov	qword ptr [rax], rsi
	mov	dword ptr [rax + 8], ebx
	mov	dword ptr [rax + 12], r14d
	mov	dword ptr [rax + 16], edi
.LBB13_24:
	mov	word ptr [rax + 21], 0
	mov	byte ptr [rax + 23], 0
	mov	qword ptr [rax + 24], rdx
	vmovlps	qword ptr [rax + 32], xmm0
	mov	qword ptr [rax + 40], r9
	vmovlps	qword ptr [rax + 48], xmm1
	mov	dword ptr [rax + 56], r10d
	mov	dword ptr [rax + 60], ecx
	.seh_startepilogue
	add	rsp, 48
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB13_28:
	mov	rdx, qword ptr [r8 + 40]
	mov	r9, qword ptr [r8 + 56]
	vmovsd	xmm0, qword ptr [r8 + 48]
	vmovsd	xmm1, qword ptr [r8 + 64]
	vxorps	xmm2, xmm2, xmm2
	vmovups	xmmword ptr [rax], xmm2
	mov	dword ptr [rax + 16], 0
	mov	byte ptr [rax + 20], 1
	xor	ecx, ecx
	mov	r10d, 2
	jmp	.LBB13_24
.LBB13_25:
	cmp	byte ptr [r8 + 24], 1
	je	.LBB13_23
	mov	r10d, dword ptr [r8 + 16]
	mov	r9d, dword ptr [r8 + 20]
	mov	dword ptr [rsp + 40], 0
	mov	qword ptr [rsp + 32], 0
	mov	edx, 16
	mov	r15, r8
	mov	r8d, r10d
	mov	r12, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	r8, r15
	mov	rax, r12
	jmp	.LBB13_23
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetBufferAccessorInternal<Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.BufferTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null bufferTypeHandle, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 readOnly) -> Unity.Entities.BufferAccessor`1[[Latios.Mecanim.MecanimBlendShapeChannelIndex, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 48
	.seh_stackalloc 48
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	mov	rax, rdx
	test	r9b, r9b
	jne	.LBB14_2
	cmp	byte ptr [r8 + 24], 1
	je	.LBB14_27
.LBB14_2:
	movsxd	r10, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rdx]
	shl	r10, 4
	mov	r10, qword ptr [r11 + r10]
	cmp	qword ptr [r8], r10
	jne	.LBB14_7
	movzx	r11d, word ptr [r8 + 14]
	test	r9b, r9b
	je	.LBB14_19
.LBB14_5:
	cmp	r11w, -1
	je	.LBB14_28
	mov	r9d, dword ptr [rcx]
	mov	r11d, r9d
	sar	r11d, 6
	movsxd	r11, r11d
	shl	r11, 3
	jmp	.LBB14_21
.LBB14_7:
	mov	edi, dword ptr [r8 + 16]
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	mov	rsi, r8
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB14_9
	cmp	r14d, r11d
	jle	.LBB14_9
	movsxd	r8, r11d
	cmp	dword ptr [rbx + 4*r8], edi
	je	.LBB14_17
.LBB14_9:
	test	r14d, r14d
	je	.LBB14_14
	xor	r11d, r11d
.LBB14_12:
	movsxd	r8, r11d
	cmp	edi, dword ptr [rbx + 4*r8]
	je	.LBB14_13
	inc	r11d
	cmp	r14d, r11d
	jne	.LBB14_12
.LBB14_14:
	mov	word ptr [rsi + 14], -1
.LBB14_15:
	mov	dword ptr [rsi + 8], 0
	mov	r11w, -1
	xor	edi, edi
	jmp	.LBB14_18
.LBB14_27:
	lea	rcx, [rip + burst.string.1ea13c7f7ea144ea32205d7ec317f39f]
	lea	rdx, [rip + burst.string.cd55498fb82486272ef3dc32c0931ef8]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB14_13:
	mov	word ptr [rsi + 14], r11w
	cmp	r11w, -1
	je	.LBB14_15
.LBB14_17:
	mov	r8, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	r8d, dword ptr [r8 + 4*rdi]
	mov	dword ptr [rsi + 8], r8d
	mov	r8, qword ptr [r10 + 232]
	movzx	edi, word ptr [r8 + 2*rdi]
.LBB14_18:
	mov	r8, rsi
	mov	word ptr [rsi + 12], di
	mov	qword ptr [rsi], r10
	test	r9b, r9b
	jne	.LBB14_5
.LBB14_19:
	cmp	r11w, -1
	je	.LBB14_28
	mov	esi, dword ptr [r8 + 20]
	movsxd	r9, dword ptr [rcx]
	movsx	r11d, r11w
	mov	rdi, qword ptr [rdx]
	mov	rbx, r9
	shl	rbx, 4
	movsxd	rdi, dword ptr [rdi + rbx + 12]
	mov	ebx, dword ptr [r10 + 8]
	lea	r14d, [4*rbx]
	movsxd	r14, r14d
	add	r14, qword ptr [r10]
	imul	r11d, ebx
	movsxd	r11, r11d
	lea	r11, [r14 + 4*r11]
	mov	dword ptr [r11 + 4*rdi], esi
	mov	r11, r9
	sar	r11, 3
	and	r11, -8
.LBB14_21:
	add	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	and	r9d, 63
	shl	r9d, 14
	add	r9, qword ptr [r11]
	movsxd	rsi, dword ptr [r8 + 8]
	add	rsi, r9
	add	rsi, 64
	je	.LBB14_28
	movsx	r9, word ptr [r8 + 14]
	mov	r10, qword ptr [r10 + 240]
	mov	edi, dword ptr [r10 + 4*r9]
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	ebx, dword ptr [rdx + r9 + 8]
	movzx	r14d, word ptr [r8 + 12]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB14_25
.LBB14_23:
	xor	ecx, ecx
	xor	r10d, r10d
	cmp	byte ptr [r8 + 24], 1
	setne	cl
	sete	r10b
	mov	rdx, qword ptr [r8 + 40]
	vmovsd	xmm0, qword ptr [r8 + 48]
	mov	r9, qword ptr [r8 + 56]
	vmovsd	xmm1, qword ptr [r8 + 64]
	sete	byte ptr [rax + 20]
	add	r10d, r10d
	add	ecx, ecx
	mov	qword ptr [rax], rsi
	mov	dword ptr [rax + 8], ebx
	mov	dword ptr [rax + 12], r14d
	mov	dword ptr [rax + 16], edi
.LBB14_24:
	mov	word ptr [rax + 21], 0
	mov	byte ptr [rax + 23], 0
	mov	qword ptr [rax + 24], rdx
	vmovlps	qword ptr [rax + 32], xmm0
	mov	qword ptr [rax + 40], r9
	vmovlps	qword ptr [rax + 48], xmm1
	mov	dword ptr [rax + 56], r10d
	mov	dword ptr [rax + 60], ecx
	.seh_startepilogue
	add	rsp, 48
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB14_28:
	mov	rdx, qword ptr [r8 + 40]
	mov	r9, qword ptr [r8 + 56]
	vmovsd	xmm0, qword ptr [r8 + 48]
	vmovsd	xmm1, qword ptr [r8 + 64]
	vxorps	xmm2, xmm2, xmm2
	vmovups	xmmword ptr [rax], xmm2
	mov	dword ptr [rax + 16], 0
	mov	byte ptr [rax + 20], 1
	xor	ecx, ecx
	mov	r10d, 2
	jmp	.LBB14_24
.LBB14_25:
	cmp	byte ptr [r8 + 24], 1
	je	.LBB14_23
	mov	r10d, dword ptr [r8 + 16]
	mov	r9d, dword ptr [r8 + 20]
	mov	dword ptr [rsp + 40], 0
	mov	qword ptr [rsp + 32], 0
	mov	edx, 16
	mov	r15, r8
	mov	r8d, r10d
	mov	r12, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	r8, r15
	mov	rax, r12
	jmp	.LBB14_23
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataPtrRW<Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimBlendShapeListener, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Latios.Mecanim.MecanimBlendShapeListener*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 48
	.seh_stackalloc 48
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	cmp	byte ptr [rdx + 28], 1
	je	.LBB15_21
	movsxd	rax, dword ptr [rcx]
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r9, qword ptr [r8]
	mov	r10, rax
	shl	r10, 4
	mov	r9, qword ptr [r9 + r10]
	cmp	qword ptr [rdx], r9
	jne	.LBB15_5
	movzx	r11d, word ptr [rdx + 14]
	cmp	r11w, -1
	je	.LBB15_4
.LBB15_17:
	mov	esi, dword ptr [rdx + 24]
	movsx	r11d, r11w
	mov	rdi, qword ptr [r8]
	movsxd	r10, dword ptr [rdi + r10 + 12]
	mov	edi, dword ptr [r9 + 8]
	lea	ebx, [4*rdi]
	movsxd	rbx, ebx
	add	rbx, qword ptr [r9]
	imul	r11d, edi
	movsxd	r9, r11d
	lea	r9, [rbx + 4*r9]
	mov	dword ptr [r9 + 4*r10], esi
	mov	r9, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r10, rax
	sar	r10, 3
	and	r10, -8
	and	eax, 63
	shl	eax, 14
	add	rax, qword ptr [r9 + r10]
	movsxd	r9, dword ptr [rdx + 8]
	lea	rax, [r9 + rax + 64]
	mov	r9, qword ptr [rcx + 8]
	cmp	byte ptr [r9 + 494], 0
	jne	.LBB15_19
.LBB15_20:
	.seh_startepilogue
	add	rsp, 48
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	rbp
	.seh_endepilogue
	ret
.LBB15_21:
	lea	rcx, [rip + burst.string.1ea13c7f7ea144ea32205d7ec317f39f]
	lea	rdx, [rip + burst.string.989b224dc613a391593dc022965ef313]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB15_5:
	mov	esi, dword ptr [rdx + 16]
	mov	rdi, qword ptr [r9 + 112]
	mov	ebx, dword ptr [r9 + 168]
	movsx	r11d, word ptr [rdx + 14]
	test	r11d, r11d
	js	.LBB15_7
	cmp	ebx, r11d
	jle	.LBB15_7
	movsxd	r14, r11d
	cmp	dword ptr [rdi + 4*r14], esi
	je	.LBB15_15
.LBB15_7:
	test	ebx, ebx
	je	.LBB15_12
	xor	r11d, r11d
.LBB15_10:
	movsxd	r14, r11d
	cmp	esi, dword ptr [rdi + 4*r14]
	je	.LBB15_11
	inc	r11d
	cmp	ebx, r11d
	jne	.LBB15_10
.LBB15_12:
	mov	word ptr [rdx + 14], -1
.LBB15_13:
	mov	dword ptr [rdx + 8], 0
	mov	r11w, -1
	xor	esi, esi
	jmp	.LBB15_16
.LBB15_11:
	mov	word ptr [rdx + 14], r11w
	cmp	r11w, -1
	je	.LBB15_13
.LBB15_15:
	mov	rsi, qword ptr [r9 + 224]
	movsx	rdi, r11w
	mov	esi, dword ptr [rsi + 4*rdi]
	mov	dword ptr [rdx + 8], esi
	mov	rsi, qword ptr [r9 + 232]
	movzx	esi, word ptr [rsi + 2*rdi]
.LBB15_16:
	mov	word ptr [rdx + 12], si
	mov	qword ptr [rdx], r9
	cmp	r11w, -1
	jne	.LBB15_17
.LBB15_4:
	xor	eax, eax
	mov	r9, qword ptr [rcx + 8]
	cmp	byte ptr [r9 + 494], 0
	je	.LBB15_20
.LBB15_19:
	movzx	r10d, word ptr [rdx + 12]
	movsxd	r9, dword ptr [rcx]
	mov	r8, qword ptr [r8]
	shl	r9, 4
	imul	r10d, dword ptr [r8 + r9 + 8]
	mov	r8d, dword ptr [rdx + 16]
	mov	r9d, dword ptr [rdx + 24]
	mov	dword ptr [rsp + 40], r10d
	mov	qword ptr [rsp + 32], rax
	mov	edx, 14
	mov	rsi, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	rax, rsi
	jmp	.LBB15_20
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.BlendShapeState, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 48
	.seh_endprologue
	mov	rax, rdx
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r10, qword ptr [rdx]
	mov	r11, r9
	shl	r11, 4
	mov	r10, qword ptr [r10 + r11]
	cmp	byte ptr [r8 + 28], 1
	mov	esi, dword ptr [r8 + 16]
	jne	.LBB16_17
	cmp	qword ptr [r8], r10
	jne	.LBB16_5
	movzx	r11d, word ptr [r8 + 14]
	cmp	r11w, -1
	jne	.LBB16_21
	jmp	.LBB16_4
.LBB16_17:
	cmp	qword ptr [r8], r10
	jne	.LBB16_25
	movzx	edi, word ptr [r8 + 14]
	cmp	di, -1
	je	.LBB16_4
.LBB16_20:
	mov	esi, dword ptr [r8 + 24]
	movsx	edi, di
	mov	rbx, qword ptr [rdx]
	movsxd	r11, dword ptr [rbx + r11 + 12]
	mov	ebx, dword ptr [r10 + 8]
	lea	r14d, [4*rbx]
	movsxd	r14, r14d
	add	r14, qword ptr [r10]
	imul	edi, ebx
	movsxd	r10, edi
	lea	r10, [r14 + 4*r10]
	mov	dword ptr [r10 + 4*r11], esi
.LBB16_21:
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r11, r9
	sar	r11, 3
	and	r11, -8
	and	r9d, 63
	shl	r9d, 14
	add	r9, qword ptr [r10 + r11]
	movsxd	rdi, dword ptr [r8 + 8]
	add	rdi, r9
	add	rdi, 64
	je	.LBB16_4
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	r14d, dword ptr [rdx + r9 + 8]
	lea	ebx, [r14 - 1]
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB16_37
.LBB16_23:
	mov	qword ptr [rax], rdi
	mov	dword ptr [rax + 8], r14d
	mov	dword ptr [rax + 12], 0
	mov	dword ptr [rax + 16], ebx
	mov	ecx, 1
.LBB16_24:
	mov	dword ptr [rax + 20], 0
	mov	qword ptr [rax + 24], rsi
	vmovlps	qword ptr [rax + 32], xmm6
	mov	dword ptr [rax + 40], ecx
	mov	dword ptr [rax + 44], 0
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB16_37:
	cmp	byte ptr [r8 + 28], 1
	je	.LBB16_23
	mov	edx, dword ptr [r8 + 20]
	imul	edx, r14d
	mov	r10d, dword ptr [r8 + 16]
	mov	r9d, dword ptr [r8 + 24]
	mov	dword ptr [rsp + 40], edx
	mov	qword ptr [rsp + 32], rdi
	mov	edx, 14
	mov	r8d, r10d
	mov	r15, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	rax, r15
	jmp	.LBB16_23
.LBB16_5:
	mov	rdi, qword ptr [r10 + 112]
	mov	ebx, dword ptr [r10 + 168]
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB16_7
	cmp	ebx, r11d
	jle	.LBB16_7
	movsxd	r14, r11d
	cmp	dword ptr [rdi + 4*r14], esi
	je	.LBB16_15
.LBB16_7:
	test	ebx, ebx
	je	.LBB16_12
	xor	r11d, r11d
.LBB16_10:
	movsxd	r14, r11d
	cmp	esi, dword ptr [rdi + 4*r14]
	je	.LBB16_11
	inc	r11d
	cmp	ebx, r11d
	jne	.LBB16_10
.LBB16_12:
	mov	word ptr [r8 + 14], -1
.LBB16_13:
	mov	dword ptr [r8 + 8], 0
	mov	r11w, -1
	xor	esi, esi
	jmp	.LBB16_16
.LBB16_25:
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	edi, word ptr [r8 + 14]
	test	edi, edi
	js	.LBB16_27
	cmp	r14d, edi
	jle	.LBB16_27
	movsxd	r15, edi
	cmp	dword ptr [rbx + 4*r15], esi
	je	.LBB16_35
.LBB16_27:
	test	r14d, r14d
	je	.LBB16_32
	xor	edi, edi
.LBB16_30:
	movsxd	r15, edi
	cmp	esi, dword ptr [rbx + 4*r15]
	je	.LBB16_31
	inc	edi
	cmp	r14d, edi
	jne	.LBB16_30
.LBB16_32:
	mov	word ptr [r8 + 14], -1
.LBB16_33:
	mov	dword ptr [r8 + 8], 0
	mov	di, -1
	xor	esi, esi
	jmp	.LBB16_36
.LBB16_11:
	mov	word ptr [r8 + 14], r11w
	cmp	r11w, -1
	je	.LBB16_13
.LBB16_15:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	esi, dword ptr [rsi + 4*rdi]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rdi]
.LBB16_16:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	r11w, -1
	jne	.LBB16_21
	jmp	.LBB16_4
.LBB16_31:
	mov	word ptr [r8 + 14], di
	cmp	di, -1
	je	.LBB16_33
.LBB16_35:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rbx, di
	mov	esi, dword ptr [rsi + 4*rbx]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rbx]
.LBB16_36:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	di, -1
	jne	.LBB16_20
.LBB16_4:
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rax], xmm0
	mov	dword ptr [rax + 16], -1
	xor	ecx, ecx
	jmp	.LBB16_24
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Kinemation.SkeletonDependent, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 48
	.seh_endprologue
	mov	rax, rdx
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r10, qword ptr [rdx]
	mov	r11, r9
	shl	r11, 4
	mov	r10, qword ptr [r10 + r11]
	cmp	byte ptr [r8 + 28], 1
	mov	esi, dword ptr [r8 + 16]
	jne	.LBB17_17
	cmp	qword ptr [r8], r10
	jne	.LBB17_5
	movzx	r11d, word ptr [r8 + 14]
	cmp	r11w, -1
	jne	.LBB17_21
	jmp	.LBB17_4
.LBB17_17:
	cmp	qword ptr [r8], r10
	jne	.LBB17_25
	movzx	edi, word ptr [r8 + 14]
	cmp	di, -1
	je	.LBB17_4
.LBB17_20:
	mov	esi, dword ptr [r8 + 24]
	movsx	edi, di
	mov	rbx, qword ptr [rdx]
	movsxd	r11, dword ptr [rbx + r11 + 12]
	mov	ebx, dword ptr [r10 + 8]
	lea	r14d, [4*rbx]
	movsxd	r14, r14d
	add	r14, qword ptr [r10]
	imul	edi, ebx
	movsxd	r10, edi
	lea	r10, [r14 + 4*r10]
	mov	dword ptr [r10 + 4*r11], esi
.LBB17_21:
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r11, r9
	sar	r11, 3
	and	r11, -8
	and	r9d, 63
	shl	r9d, 14
	add	r9, qword ptr [r10 + r11]
	movsxd	rdi, dword ptr [r8 + 8]
	add	rdi, r9
	add	rdi, 64
	je	.LBB17_4
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	r14d, dword ptr [rdx + r9 + 8]
	lea	ebx, [r14 - 1]
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB17_37
.LBB17_23:
	mov	qword ptr [rax], rdi
	mov	dword ptr [rax + 8], r14d
	mov	dword ptr [rax + 12], 0
	mov	dword ptr [rax + 16], ebx
	mov	ecx, 1
.LBB17_24:
	mov	dword ptr [rax + 20], 0
	mov	qword ptr [rax + 24], rsi
	vmovlps	qword ptr [rax + 32], xmm6
	mov	dword ptr [rax + 40], ecx
	mov	dword ptr [rax + 44], 0
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB17_37:
	cmp	byte ptr [r8 + 28], 1
	je	.LBB17_23
	mov	edx, dword ptr [r8 + 20]
	imul	edx, r14d
	mov	r10d, dword ptr [r8 + 16]
	mov	r9d, dword ptr [r8 + 24]
	mov	dword ptr [rsp + 40], edx
	mov	qword ptr [rsp + 32], rdi
	mov	edx, 14
	mov	r8d, r10d
	mov	r15, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	rax, r15
	jmp	.LBB17_23
.LBB17_5:
	mov	rdi, qword ptr [r10 + 112]
	mov	ebx, dword ptr [r10 + 168]
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB17_7
	cmp	ebx, r11d
	jle	.LBB17_7
	movsxd	r14, r11d
	cmp	dword ptr [rdi + 4*r14], esi
	je	.LBB17_15
.LBB17_7:
	test	ebx, ebx
	je	.LBB17_12
	xor	r11d, r11d
.LBB17_10:
	movsxd	r14, r11d
	cmp	esi, dword ptr [rdi + 4*r14]
	je	.LBB17_11
	inc	r11d
	cmp	ebx, r11d
	jne	.LBB17_10
.LBB17_12:
	mov	word ptr [r8 + 14], -1
.LBB17_13:
	mov	dword ptr [r8 + 8], 0
	mov	r11w, -1
	xor	esi, esi
	jmp	.LBB17_16
.LBB17_25:
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	edi, word ptr [r8 + 14]
	test	edi, edi
	js	.LBB17_27
	cmp	r14d, edi
	jle	.LBB17_27
	movsxd	r15, edi
	cmp	dword ptr [rbx + 4*r15], esi
	je	.LBB17_35
.LBB17_27:
	test	r14d, r14d
	je	.LBB17_32
	xor	edi, edi
.LBB17_30:
	movsxd	r15, edi
	cmp	esi, dword ptr [rbx + 4*r15]
	je	.LBB17_31
	inc	edi
	cmp	r14d, edi
	jne	.LBB17_30
.LBB17_32:
	mov	word ptr [r8 + 14], -1
.LBB17_33:
	mov	dword ptr [r8 + 8], 0
	mov	di, -1
	xor	esi, esi
	jmp	.LBB17_36
.LBB17_11:
	mov	word ptr [r8 + 14], r11w
	cmp	r11w, -1
	je	.LBB17_13
.LBB17_15:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	esi, dword ptr [rsi + 4*rdi]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rdi]
.LBB17_16:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	r11w, -1
	jne	.LBB17_21
	jmp	.LBB17_4
.LBB17_31:
	mov	word ptr [r8 + 14], di
	cmp	di, -1
	je	.LBB17_33
.LBB17_35:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rbx, di
	mov	esi, dword ptr [rsi + 4*rbx]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rbx]
.LBB17_36:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	di, -1
	jne	.LBB17_20
.LBB17_4:
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rax], xmm0
	mov	dword ptr [rax + 16], -1
	xor	ecx, ecx
	jmp	.LBB17_24
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Mecanim.MecanimPropertySource, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 48
	.seh_endprologue
	mov	rax, rdx
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r10, qword ptr [rdx]
	mov	r11, r9
	shl	r11, 4
	mov	r10, qword ptr [r10 + r11]
	cmp	byte ptr [r8 + 28], 1
	mov	esi, dword ptr [r8 + 16]
	jne	.LBB18_17
	cmp	qword ptr [r8], r10
	jne	.LBB18_5
	movzx	r11d, word ptr [r8 + 14]
	cmp	r11w, -1
	jne	.LBB18_21
	jmp	.LBB18_4
.LBB18_17:
	cmp	qword ptr [r8], r10
	jne	.LBB18_25
	movzx	edi, word ptr [r8 + 14]
	cmp	di, -1
	je	.LBB18_4
.LBB18_20:
	mov	esi, dword ptr [r8 + 24]
	movsx	edi, di
	mov	rbx, qword ptr [rdx]
	movsxd	r11, dword ptr [rbx + r11 + 12]
	mov	ebx, dword ptr [r10 + 8]
	lea	r14d, [4*rbx]
	movsxd	r14, r14d
	add	r14, qword ptr [r10]
	imul	edi, ebx
	movsxd	r10, edi
	lea	r10, [r14 + 4*r10]
	mov	dword ptr [r10 + 4*r11], esi
.LBB18_21:
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r11, r9
	sar	r11, 3
	and	r11, -8
	and	r9d, 63
	shl	r9d, 14
	add	r9, qword ptr [r10 + r11]
	movsxd	rdi, dword ptr [r8 + 8]
	add	rdi, r9
	add	rdi, 64
	je	.LBB18_4
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	r14d, dword ptr [rdx + r9 + 8]
	lea	ebx, [r14 - 1]
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB18_37
.LBB18_23:
	mov	qword ptr [rax], rdi
	mov	dword ptr [rax + 8], r14d
	mov	dword ptr [rax + 12], 0
	mov	dword ptr [rax + 16], ebx
	mov	ecx, 1
.LBB18_24:
	mov	dword ptr [rax + 20], 0
	mov	qword ptr [rax + 24], rsi
	vmovlps	qword ptr [rax + 32], xmm6
	mov	dword ptr [rax + 40], ecx
	mov	dword ptr [rax + 44], 0
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB18_37:
	cmp	byte ptr [r8 + 28], 1
	je	.LBB18_23
	mov	edx, dword ptr [r8 + 20]
	imul	edx, r14d
	mov	r10d, dword ptr [r8 + 16]
	mov	r9d, dword ptr [r8 + 24]
	mov	dword ptr [rsp + 40], edx
	mov	qword ptr [rsp + 32], rdi
	mov	edx, 14
	mov	r8d, r10d
	mov	r15, rax
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	mov	rax, r15
	jmp	.LBB18_23
.LBB18_5:
	mov	rdi, qword ptr [r10 + 112]
	mov	ebx, dword ptr [r10 + 168]
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB18_7
	cmp	ebx, r11d
	jle	.LBB18_7
	movsxd	r14, r11d
	cmp	dword ptr [rdi + 4*r14], esi
	je	.LBB18_15
.LBB18_7:
	test	ebx, ebx
	je	.LBB18_12
	xor	r11d, r11d
.LBB18_10:
	movsxd	r14, r11d
	cmp	esi, dword ptr [rdi + 4*r14]
	je	.LBB18_11
	inc	r11d
	cmp	ebx, r11d
	jne	.LBB18_10
.LBB18_12:
	mov	word ptr [r8 + 14], -1
.LBB18_13:
	mov	dword ptr [r8 + 8], 0
	mov	r11w, -1
	xor	esi, esi
	jmp	.LBB18_16
.LBB18_25:
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	edi, word ptr [r8 + 14]
	test	edi, edi
	js	.LBB18_27
	cmp	r14d, edi
	jle	.LBB18_27
	movsxd	r15, edi
	cmp	dword ptr [rbx + 4*r15], esi
	je	.LBB18_35
.LBB18_27:
	test	r14d, r14d
	je	.LBB18_32
	xor	edi, edi
.LBB18_30:
	movsxd	r15, edi
	cmp	esi, dword ptr [rbx + 4*r15]
	je	.LBB18_31
	inc	edi
	cmp	r14d, edi
	jne	.LBB18_30
.LBB18_32:
	mov	word ptr [r8 + 14], -1
.LBB18_33:
	mov	dword ptr [r8 + 8], 0
	mov	di, -1
	xor	esi, esi
	jmp	.LBB18_36
.LBB18_11:
	mov	word ptr [r8 + 14], r11w
	cmp	r11w, -1
	je	.LBB18_13
.LBB18_15:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	esi, dword ptr [rsi + 4*rdi]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rdi]
.LBB18_16:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	r11w, -1
	jne	.LBB18_21
	jmp	.LBB18_4
.LBB18_31:
	mov	word ptr [r8 + 14], di
	cmp	di, -1
	je	.LBB18_33
.LBB18_35:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rbx, di
	mov	esi, dword ptr [rsi + 4*rbx]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rbx]
.LBB18_36:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	di, -1
	jne	.LBB18_20
.LBB18_4:
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rax], xmm0
	mov	dword ptr [rax + 16], -1
	xor	ecx, ecx
	jmp	.LBB18_24
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecord(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@48"
	push	rbp
	.seh_pushreg rbp
	sub	rsp, 112
	.seh_stackalloc 112
	lea	rbp, [rsp + 112]
	.seh_setframe rbp, 112
	.seh_endprologue
	mov	r10d, edx
	mov	rax, rcx
	mov	rcx, qword ptr [rbp + 48]
	mov	r11d, dword ptr [rbp + 56]
	mov	dword ptr [rbp - 4], r8d
	mov	rdx, qword ptr [rax + 8]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp - 20], xmm0
	mov	dword ptr [rsp + 72], r11d
	mov	qword ptr [rsp + 64], rcx
	lea	rcx, [rbp - 4]
	mov	qword ptr [rsp + 48], rcx
	lea	rcx, [rbp - 20]
	mov	qword ptr [rsp + 40], rcx
	mov	dword ptr [rsp + 56], 1
	mov	dword ptr [rsp + 32], 1
	mov	ecx, r10d
	mov	r8d, r9d
	mov	r9, rax
	call	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	nop
	.seh_startepilogue
	add	rsp, 112
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.get_Item(Unity.Entities.BufferLookup`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity) -> Unity.Entities.DynamicBuffer`1[[Latios.Mecanim.MecanimBlendShapeValue, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 56
	.seh_stackalloc 56
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	mov	eax, r9d
	mov	rsi, rdx
	mov	rdi, rcx
	mov	rcx, qword ptr [rcx + 40]
	cmp	byte ptr [rdi + 76], 0
	mov	r9d, dword ptr [rdi + 64]
	je	.LBB20_16
	mov	rcx, qword ptr [rcx + 8]
	movsxd	rdx, r8d
	movsxd	rax, dword ptr [rcx + 8*rdx]
	mov	ecx, dword ptr [rcx + 8*rdx + 4]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rdx, qword ptr [rdx]
	mov	r8, rax
	shl	r8, 4
	mov	rdx, qword ptr [rdx + r8]
	cmp	qword ptr [rdi + 48], rdx
	jne	.LBB20_3
	mov	r8d, dword ptr [rdi + 56]
	movzx	r9d, word ptr [rdi + 60]
.LBB20_15:
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r10, rax
	sar	r10, 3
	and	r10, -8
	and	eax, 63
	shl	eax, 14
	add	rax, qword ptr [rdx + r10]
	movzx	edx, r9w
	imul	ecx, edx
	add	ecx, r8d
	movsxd	rcx, ecx
	lea	rax, [rcx + rax + 64]
	mov	cl, 1
	jmp	.LBB20_17
.LBB20_16:
	mov	edx, dword ptr [rdi + 68]
	lea	r10, [rdi + 48]
	mov	qword ptr [rsp + 40], r10
	mov	dword ptr [rsp + 32], edx
	mov	edx, r8d
	mov	r8d, eax
	call	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	cmp	byte ptr [rdi + 76], 0
	setne	cl
.LBB20_17:
	mov	rdx, qword ptr [rdi]
	mov	r8, qword ptr [rdi + 16]
	mov	r9d, dword ptr [rdi + 72]
	movzx	ecx, cl
	lea	r10d, [rcx + rcx]
	mov	r11d, ecx
	xor	r11b, 1
	movzx	r11d, r11b
	add	r11d, r11d
	mov	qword ptr [rsi], rax
	mov	dword ptr [rsi + 8], r9d
	mov	dword ptr [rsi + 12], 0
	mov	qword ptr [rsi + 16], rdx
	mov	rax, qword ptr [rdi + 8]
	mov	qword ptr [rsi + 24], rax
	mov	qword ptr [rsi + 32], r8
	mov	rax, qword ptr [rdi + 24]
	mov	qword ptr [rsi + 40], rax
	mov	dword ptr [rsi + 48], r10d
	mov	dword ptr [rsi + 52], r11d
	mov	byte ptr [rsi + 56], cl
	mov	dword ptr [rsi + 57], 0
	mov	word ptr [rsi + 61], 0
	mov	byte ptr [rsi + 63], 0
	mov	rax, rsi
	.seh_startepilogue
	add	rsp, 56
	pop	rbx
	pop	rdi
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
.LBB20_3:
	mov	r10, qword ptr [rdx + 112]
	mov	r11d, dword ptr [rdx + 168]
	movsx	r8d, word ptr [rdi + 62]
	test	r8d, r8d
	js	.LBB20_5
	cmp	r11d, r8d
	jle	.LBB20_5
	movsxd	rbx, r8d
	cmp	dword ptr [r10 + 4*rbx], r9d
	je	.LBB20_13
.LBB20_5:
	test	r11d, r11d
	je	.LBB20_10
	xor	r8d, r8d
.LBB20_8:
	movsxd	rbx, r8d
	cmp	r9d, dword ptr [r10 + 4*rbx]
	je	.LBB20_9
	inc	r8d
	cmp	r11d, r8d
	jne	.LBB20_8
.LBB20_10:
	mov	word ptr [rdi + 62], -1
.LBB20_11:
	mov	dword ptr [rdi + 56], 0
	xor	r8d, r8d
	xor	r9d, r9d
	jmp	.LBB20_14
.LBB20_9:
	mov	word ptr [rdi + 62], r8w
	cmp	r8w, -1
	je	.LBB20_11
.LBB20_13:
	mov	r9, qword ptr [rdx + 224]
	movsx	r10, r8w
	mov	r8d, dword ptr [r9 + 4*r10]
	mov	dword ptr [rdi + 56], r8d
	mov	r9, qword ptr [rdx + 232]
	movzx	r9d, word ptr [r9 + 2*r10]
.LBB20_14:
	mov	word ptr [rdi + 60], r9w
	mov	qword ptr [rdi + 48], rdx
	jmp	.LBB20_15
	.seh_endproc

	.def	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.TryGetComponent(Unity.Entities.ComponentLookup`1[[Latios.Mecanim.MecanimAnimatedProperties, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Latios.Mecanim.MecanimAnimatedProperties&, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null componentData, System.Boolean&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityExists) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	mov	r10, qword ptr [rbp + 72]
	mov	rax, qword ptr [rcx]
	cmp	byte ptr [rcx + 48], 0
	je	.LBB21_5
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [r9], xmm0
	mov	r9d, dword ptr [rcx + 40]
	movsxd	rdx, edx
	mov	r11, qword ptr [rax]
	mov	rsi, qword ptr [rax + 8]
	cmp	dword ptr [r11 + 4*rdx], r8d
	sete	r8b
	cmp	dword ptr [rsi + 8*rdx], 0
	setne	r11b
	and	r11b, r8b
	mov	byte ptr [r10], r11b
	je	.LBB21_36
	mov	rax, qword ptr [rax + 8]
	movsxd	rax, dword ptr [rax + 8*rdx]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rdx, qword ptr [rdx]
	shl	rax, 4
	mov	rax, qword ptr [rdx + rax]
	cmp	rax, qword ptr [rcx + 8]
	jne	.LBB21_11
	movzx	edx, word ptr [rcx + 22]
.LBB21_4:
	cmp	dx, -1
	setne	al
	jmp	.LBB21_37
.LBB21_5:
	movsxd	r11, edx
	mov	rdx, qword ptr [rax]
	mov	rsi, qword ptr [rax + 8]
	cmp	dword ptr [rdx + 4*r11], r8d
	sete	dl
	cmp	dword ptr [rsi + 8*r11], 0
	setne	r8b
	and	r8b, dl
	mov	byte ptr [r10], r8b
	je	.LBB21_35
	mov	rax, qword ptr [rax + 8]
	movsxd	rdx, dword ptr [rax + 8*r11]
	movsxd	rax, dword ptr [rax + 8*r11 + 4]
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r8, qword ptr [r8]
	mov	r10, rdx
	shl	r10, 4
	mov	r8, qword ptr [r8 + r10]
	cmp	qword ptr [rcx + 8], r8
	jne	.LBB21_20
	movzx	r10d, word ptr [rcx + 22]
	cmp	r10w, -1
	je	.LBB21_35
.LBB21_8:
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	r10, rdx
	sar	r10, 3
	and	r10, -8
	and	edx, 63
	shl	edx, 14
	add	rdx, qword ptr [r8 + r10]
	movsxd	r8, dword ptr [rcx + 16]
	movzx	ecx, word ptr [rcx + 20]
	imul	rax, rcx
	add	rax, r8
	add	rax, rdx
	add	rax, 64
	je	.LBB21_35
	vmovups	xmm0, xmmword ptr [rax]
	vmovups	xmmword ptr [r9], xmm0
	mov	al, 1
	jmp	.LBB21_37
.LBB21_11:
	mov	r8, qword ptr [rax + 112]
	mov	r10d, dword ptr [rax + 168]
	movsx	edx, word ptr [rcx + 22]
	test	edx, edx
	js	.LBB21_14
	cmp	r10d, edx
	jle	.LBB21_14
	movsxd	r11, edx
	cmp	dword ptr [r8 + 4*r11], r9d
	je	.LBB21_30
.LBB21_14:
	test	r10d, r10d
	je	.LBB21_18
	xor	edx, edx
.LBB21_16:
	movsxd	r11, edx
	cmp	r9d, dword ptr [r8 + 4*r11]
	je	.LBB21_29
	inc	edx
	cmp	r10d, edx
	jne	.LBB21_16
.LBB21_18:
	mov	word ptr [rcx + 22], -1
.LBB21_19:
	mov	dword ptr [rcx + 16], 0
	mov	dx, -1
	xor	r8d, r8d
	jmp	.LBB21_31
.LBB21_20:
	mov	r11d, dword ptr [rcx + 40]
	mov	rsi, qword ptr [r8 + 112]
	mov	edi, dword ptr [r8 + 168]
	movsx	r10d, word ptr [rcx + 22]
	test	r10d, r10d
	js	.LBB21_23
	cmp	edi, r10d
	jle	.LBB21_23
	movsxd	rbx, r10d
	cmp	dword ptr [rsi + 4*rbx], r11d
	je	.LBB21_33
.LBB21_23:
	test	edi, edi
	je	.LBB21_27
	xor	r10d, r10d
.LBB21_25:
	movsxd	rbx, r10d
	cmp	r11d, dword ptr [rsi + 4*rbx]
	je	.LBB21_32
	inc	r10d
	cmp	edi, r10d
	jne	.LBB21_25
.LBB21_27:
	mov	word ptr [rcx + 22], -1
.LBB21_28:
	mov	dword ptr [rcx + 16], 0
	mov	r10w, -1
	xor	r11d, r11d
	jmp	.LBB21_34
.LBB21_29:
	mov	word ptr [rcx + 22], dx
	cmp	dx, -1
	je	.LBB21_19
.LBB21_30:
	mov	r8, qword ptr [rax + 224]
	movsx	r9, dx
	mov	r8d, dword ptr [r8 + 4*r9]
	mov	dword ptr [rcx + 16], r8d
	mov	r8, qword ptr [rax + 232]
	movzx	r8d, word ptr [r8 + 2*r9]
.LBB21_31:
	mov	word ptr [rcx + 20], r8w
	mov	qword ptr [rcx + 8], rax
	jmp	.LBB21_4
.LBB21_32:
	mov	word ptr [rcx + 22], r10w
	cmp	r10w, -1
	je	.LBB21_28
.LBB21_33:
	mov	r11, qword ptr [r8 + 224]
	movsx	rsi, r10w
	mov	r11d, dword ptr [r11 + 4*rsi]
	mov	dword ptr [rcx + 16], r11d
	mov	r11, qword ptr [r8 + 232]
	movzx	r11d, word ptr [r11 + 2*rsi]
.LBB21_34:
	mov	word ptr [rcx + 20], r11w
	mov	qword ptr [rcx + 8], r8
	cmp	r10w, -1
	jne	.LBB21_8
.LBB21_35:
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [r9], xmm0
.LBB21_36:
	xor	eax, eax
.LBB21_37:
	.seh_startepilogue
	pop	rbx
	pop	rdi
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@000000000000cccc0000000000000000
	.section	.rdata,"dr",discard,__xmm@000000000000cccc0000000000000000
	.p2align	4, 0x0
__xmm@000000000000cccc0000000000000000:
	.long	0
	.long	0
	.long	52428
	.long	0
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 48
	.seh_endprologue
	mov	rax, rcx
	test	r9d, r9d
	je	.LBB22_15
	mov	rbx, qword ptr [rdx + 72]
	test	rbx, rbx
	je	.LBB22_2
	cmp	qword ptr [rbx], r8
	je	.LBB22_13
.LBB22_2:
	mov	rbx, qword ptr [rdx + 64]
	test	rbx, rbx
	je	.LBB22_7
	.p2align	4
.LBB22_4:
	cmp	qword ptr [rbx], r8
	je	.LBB22_12
	mov	rbx, qword ptr [rbx + 16]
	test	rbx, rbx
	jne	.LBB22_4
.LBB22_7:
	mov	rbx, r8
	mov	r15, rdx
	mov	r14d, r9d
	mov	rdi, rax
	mov	r12, qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	qword ptr [rsp + 32], 0
	mov	ecx, 24
	mov	edx, 64
	mov	r8d, 4
	mov	r9d, 1
	call	r12
	test	rax, rax
	je	.LBB22_8
	mov	rsi, rax
	vxorps	xmm6, xmm6, xmm6
	vmovups	xmmword ptr [rax + 8], xmm6
	mov	qword ptr [rax], rbx
	mov	qword ptr [rsp + 32], 0
	mov	ecx, 32
	mov	edx, 64
	mov	r8d, 4
	mov	r9d, 1
	call	r12
	test	rax, rax
	je	.LBB22_11
	mov	rbx, rsi
	add	rbx, 8
	vmovups	xmmword ptr [rax + 4], xmm6
	mov	qword ptr [rax + 20], 0
	mov	dword ptr [rax + 28], 0
	mov	qword ptr [rsi + 8], rax
	mov	dword ptr [rax], 4
	mov	qword ptr [rsp + 32], 0
	mov	ecx, 1048560
	mov	edx, 64
	mov	r8d, 4
	mov	r9d, 1
	mov	r13, rax
	call	r12
	mov	qword ptr [r13 + 8], rax
	vmovaps	xmm0, xmmword ptr [rip + __xmm@000000000000cccc0000000000000000]
	vmovups	xmmword ptr [r13 + 16], xmm0
	mov	rax, qword ptr [r15 + 64]
	mov	qword ptr [rsi + 16], rax
	mov	qword ptr [r15 + 64], rsi
	mov	qword ptr [r15 + 72], rsi
	mov	rax, rdi
	mov	r9d, r14d
	mov	r10, qword ptr [rbx]
	test	r10, r10
	jne	.LBB22_17
	jmp	.LBB22_15
.LBB22_12:
	mov	qword ptr [rdx + 72], rbx
.LBB22_13:
	add	rbx, 8
	mov	r10, qword ptr [rbx]
	test	r10, r10
	je	.LBB22_15
.LBB22_17:
	mov	ecx, dword ptr [r10 + 28]
	test	ecx, ecx
	je	.LBB22_15
	mov	rdx, qword ptr [r10 + 8]
	mov	r8d, dword ptr [r10 + 16]
	mov	r10d, dword ptr [r10 + 24]
	mov	r11d, r10d
	sub	r11d, r8d
	xor	esi, esi
	test	r11d, r11d
	mov	edi, r10d
	cmovg	edi, esi
	mov	ebx, r8d
	sub	ebx, edi
	movsxd	rdi, ebx
	lea	rbx, [rdi + 4*rdi]
	mov	edi, r9d
	sub	edi, dword ptr [rdx + 4*rbx]
	js	.LBB22_20
	cmp	edi, ecx
	jge	.LBB22_20
	xor	ebx, ebx
	cmp	edi, r11d
	cmovge	ebx, r10d
	add	edi, r8d
	sub	edi, ebx
	movsxd	rdi, edi
	shl	rdi, 2
	lea	rdi, [rdi + 4*rdi]
	cmp	dword ptr [rdx + rdi], r9d
	je	.LBB22_24
.LBB22_20:
	dec	ecx
	.p2align	4
.LBB22_21:
	test	ecx, ecx
	js	.LBB22_15
	cmp	ecx, r11d
	mov	edi, r10d
	cmovl	edi, esi
	lea	ebx, [r8 + rcx]
	sub	ebx, edi
	movsxd	rdi, ebx
	shl	rdi, 2
	lea	rdi, [rdi + 4*rdi]
	dec	ecx
	cmp	dword ptr [rdx + rdi], r9d
	jne	.LBB22_21
.LBB22_24:
	mov	ecx, dword ptr [rdx + rdi + 4]
	mov	r8d, dword ptr [rdx + rdi + 8]
	movzx	r9d, word ptr [rdx + rdi + 12]
	movzx	r10d, word ptr [rdx + rdi + 14]
	mov	edx, dword ptr [rdx + rdi + 16]
	jmp	.LBB22_16
.LBB22_15:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	jmp	.LBB22_16
.LBB22_11:
	mov	qword ptr [rsi + 8], 0
	mov	rcx, rsi
	mov	edx, 4
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB22_8:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	mov	rax, rdi
.LBB22_16:
	mov	dword ptr [rax], ecx
	mov	dword ptr [rax + 4], r8d
	mov	word ptr [rax + 8], r9w
	mov	word ptr [rax + 10], r10w
	mov	dword ptr [rax + 12], edx
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	.p2align	4
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 264
	.seh_stackalloc 264
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovdqa	xmmword ptr [rbp + 112], xmm11
	.seh_savexmm xmm11, 240
	vmovdqa	xmmword ptr [rbp + 96], xmm10
	.seh_savexmm xmm10, 224
	vmovdqa	xmmword ptr [rbp + 80], xmm9
	.seh_savexmm xmm9, 208
	vmovdqa	xmmword ptr [rbp + 64], xmm8
	.seh_savexmm xmm8, 192
	vmovdqa	xmmword ptr [rbp + 48], xmm7
	.seh_savexmm xmm7, 176
	vmovdqa	xmmword ptr [rbp + 32], xmm6
	.seh_savexmm xmm6, 160
	.seh_endprologue
	mov	r14d, r9d
	mov	qword ptr [rbp + 8], r8
	mov	edi, edx
	mov	rsi, rcx
	mov	edx, dword ptr [rbp + 256]
	mov	r13, qword ptr [rbp + 248]
	test	r13, r13
	setne	al
	test	edx, edx
	setg	r15b
	and	r15b, al
	xor	r12d, r12d
	cmp	r15b, 1
	jne	.LBB23_13
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rax, qword ptr [rax]
	mov	ecx, edx
	cmp	edx, 9
	jae	.LBB23_3
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB23_11
.LBB23_3:
	cmp	edx, 33
	jae	.LBB23_5
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB23_8
.LBB23_5:
	mov	edx, ecx
	and	edx, 31
	mov	r8d, 32
	cmovne	r8, rdx
	mov	rdx, rcx
	sub	rdx, r8
	lea	r9, [r13 + 496]
	vpxor	xmm0, xmm0, xmm0
	mov	r10, rdx
	vpxor	xmm1, xmm1, xmm1
	vpxor	xmm2, xmm2, xmm2
	vpxor	xmm3, xmm3, xmm3
	.p2align	4
.LBB23_6:
	movsxd	r11, dword ptr [r9 - 432]
	shl	r11, 4
	vmovd	xmm4, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 416]
	shl	r11, 4
	vpinsrd	xmm4, xmm4, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 400]
	shl	r11, 4
	vpinsrd	xmm4, xmm4, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 384]
	shl	r11, 4
	vpinsrd	xmm4, xmm4, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 496]
	shl	r11, 4
	vmovd	xmm5, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 480]
	shl	r11, 4
	vpinsrd	xmm5, xmm5, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 464]
	shl	r11, 4
	vpinsrd	xmm5, xmm5, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 448]
	shl	r11, 4
	vpinsrd	xmm5, xmm5, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 304]
	shl	r11, 4
	vmovd	xmm6, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 288]
	shl	r11, 4
	vpinsrd	xmm6, xmm6, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 272]
	shl	r11, 4
	vpinsrd	xmm6, xmm6, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 256]
	shl	r11, 4
	vpinsrd	xmm6, xmm6, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 368]
	shl	r11, 4
	vmovd	xmm7, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 352]
	shl	r11, 4
	vpinsrd	xmm7, xmm7, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 336]
	shl	r11, 4
	vpinsrd	xmm7, xmm7, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 320]
	shl	r11, 4
	vpinsrd	xmm7, xmm7, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 176]
	shl	r11, 4
	vmovd	xmm8, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 160]
	shl	r11, 4
	vpinsrd	xmm8, xmm8, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 144]
	shl	r11, 4
	vpinsrd	xmm8, xmm8, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 128]
	shl	r11, 4
	vpinsrd	xmm8, xmm8, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 240]
	shl	r11, 4
	vmovd	xmm9, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 224]
	shl	r11, 4
	vpinsrd	xmm9, xmm9, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 208]
	shl	r11, 4
	vpinsrd	xmm9, xmm9, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 192]
	shl	r11, 4
	vpinsrd	xmm9, xmm9, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 48]
	shl	r11, 4
	vmovd	xmm10, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 32]
	shl	r11, 4
	vpinsrd	xmm10, xmm10, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 16]
	shl	r11, 4
	vpinsrd	xmm10, xmm10, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9]
	shl	r11, 4
	vpinsrd	xmm10, xmm10, dword ptr [rax + r11 + 8], 3
	movsxd	r11, dword ptr [r9 - 112]
	shl	r11, 4
	vmovd	xmm11, dword ptr [rax + r11 + 8]
	movsxd	r11, dword ptr [r9 - 96]
	shl	r11, 4
	vpinsrd	xmm11, xmm11, dword ptr [rax + r11 + 8], 1
	movsxd	r11, dword ptr [r9 - 80]
	shl	r11, 4
	vpinsrd	xmm11, xmm11, dword ptr [rax + r11 + 8], 2
	movsxd	r11, dword ptr [r9 - 64]
	shl	r11, 4
	vpinsrd	xmm11, xmm11, dword ptr [rax + r11 + 8], 3
	vinserti128	ymm4, ymm5, xmm4, 1
	vpaddd	ymm0, ymm4, ymm0
	vinserti128	ymm4, ymm7, xmm6, 1
	vpaddd	ymm1, ymm4, ymm1
	vinserti128	ymm4, ymm9, xmm8, 1
	vpaddd	ymm2, ymm4, ymm2
	vinserti128	ymm4, ymm11, xmm10, 1
	vpaddd	ymm3, ymm4, ymm3
	add	r9, 512
	add	r10, -32
	jne	.LBB23_6
	vpaddd	ymm0, ymm1, ymm0
	vpaddd	ymm0, ymm2, ymm0
	vpaddd	ymm0, ymm3, ymm0
	vextracti128	xmm1, ymm0, 1
	vpaddd	xmm0, xmm0, xmm1
	vpshufd	xmm1, xmm0, 238
	vpaddd	xmm0, xmm0, xmm1
	vpshufd	xmm1, xmm0, 85
	vpaddd	xmm0, xmm0, xmm1
	vmovd	r12d, xmm0
	cmp	r8d, 9
	jb	.LBB23_11
.LBB23_8:
	mov	dword ptr [rbp + 4], r14d
	mov	dword ptr [rbp + 16], edi
	mov	byte ptr [rbp + 24], r15b
	mov	r9, rdx
	mov	edx, ecx
	and	edx, 7
	mov	r8d, 8
	cmovne	r8, rdx
	mov	rdx, rcx
	sub	rdx, r8
	vmovd	xmm0, r12d
	add	r8, r9
	sub	r8, rcx
	shl	r9, 4
	lea	r9, [r9 + r13 + 112]
	.p2align	4
.LBB23_9:
	movsxd	r10, dword ptr [r9 - 112]
	movsxd	r11, dword ptr [r9 - 96]
	movsxd	r15, dword ptr [r9 - 80]
	movsxd	r12, dword ptr [r9 - 64]
	movsxd	rbx, dword ptr [r9 - 48]
	movsxd	rdi, dword ptr [r9 - 32]
	movsxd	r14, dword ptr [r9 - 16]
	movsxd	r13, dword ptr [r9]
	shl	r10, 4
	shl	r11, 4
	shl	r15, 4
	shl	r12, 4
	shl	rbx, 4
	shl	rdi, 4
	shl	r14, 4
	shl	r13, 4
	vmovd	xmm1, dword ptr [rax + rbx + 8]
	vpinsrd	xmm1, xmm1, dword ptr [rax + rdi + 8], 1
	vpinsrd	xmm1, xmm1, dword ptr [rax + r14 + 8], 2
	vpinsrd	xmm1, xmm1, dword ptr [rax + r13 + 8], 3
	vmovd	xmm2, dword ptr [rax + r10 + 8]
	vpinsrd	xmm2, xmm2, dword ptr [rax + r11 + 8], 1
	vpinsrd	xmm2, xmm2, dword ptr [rax + r15 + 8], 2
	vpinsrd	xmm2, xmm2, dword ptr [rax + r12 + 8], 3
	vinserti128	ymm1, ymm2, xmm1, 1
	vpaddd	ymm0, ymm1, ymm0
	sub	r9, -128
	add	r8, 8
	jne	.LBB23_9
	vextracti128	xmm1, ymm0, 1
	vpaddd	xmm0, xmm0, xmm1
	vpshufd	xmm1, xmm0, 238
	vpaddd	xmm0, xmm0, xmm1
	vpshufd	xmm1, xmm0, 85
	vpaddd	xmm0, xmm0, xmm1
	vmovd	r12d, xmm0
	mov	r13, qword ptr [rbp + 248]
	movzx	r15d, byte ptr [rbp + 24]
	mov	edi, dword ptr [rbp + 16]
	mov	r14d, dword ptr [rbp + 4]
.LBB23_11:
	sub	rcx, rdx
	shl	rdx, 4
	add	rdx, r13
	.p2align	4
.LBB23_12:
	movsxd	r8, dword ptr [rdx]
	shl	r8, 4
	add	r12d, dword ptr [rax + r8 + 8]
	add	rdx, 16
	dec	rcx
	jne	.LBB23_12
.LBB23_13:
	mov	edx, dword ptr [rbp + 288]
	mov	ecx, 1
	.p2align	4
.LBB23_14:
	xor	eax, eax
	lock		cmpxchg	dword ptr [rsi + 80], ecx
	je	.LBB23_15
.LBB23_17:
	mov	eax, dword ptr [rsi + 80]
	cmp	eax, 1
	je	.LBB23_17
	jmp	.LBB23_14
.LBB23_15:
	mov	dword ptr [rsp + 32], edx
	mov	rcx, rsi
	mov	edx, edi
	mov	r8d, r12d
	mov	ebx, dword ptr [rbp + 272]
	mov	r9d, ebx
	vzeroupper
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	test	al, 1
	je	.LBB23_16
	mov	byte ptr [rbp + 24], r15b
	mov	r9d, r14d
	mov	r14, qword ptr [rbp + 240]
	lea	rcx, [rbp - 88]
	mov	rdx, rsi
	mov	r15, qword ptr [rbp + 8]
	mov	r8, r15
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	vmovdqu	xmm0, xmmword ptr [rbp - 88]
	mov	rax, qword ptr [r15 + 376]
	mov	rcx, qword ptr [rsi + 88]
	lea	rdx, [rcx + 1]
	mov	qword ptr [rsi + 88], rdx
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	mov	edx, dword ptr [rdx]
	mov	r8d, dword ptr [r14 + 12]
	mov	qword ptr [rbp - 72], rcx
	mov	dword ptr [rbp - 64], edi
	mov	dword ptr [rbp - 60], edx
	mov	qword ptr [rbp - 56], rax
	vmovdqu	xmmword ptr [rbp - 48], xmm0
	mov	rax, qword ptr [r14]
	mov	qword ptr [rbp - 32], rax
	mov	eax, dword ptr [r14 + 8]
	mov	dword ptr [rbp - 24], eax
	mov	dword ptr [rbp - 20], r8d
	mov	dword ptr [rbp - 16], r12d
	mov	dword ptr [rbp - 12], ebx
	mov	eax, dword ptr [rbp + 288]
	mov	dword ptr [rbp - 8], eax
	mov	dword ptr [rbp - 4], 0
	mov	ebx, dword ptr [rsi + 56]
	mov	eax, dword ptr [rsi + 60]
	add	eax, 72
	cmp	eax, ebx
	jle	.LBB23_19
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	ebx, dword ptr [rbp + 288]
	mov	r15d, dword ptr [rbp + 256]
	cmp	byte ptr [rbp + 24], 0
	jne	.LBB23_25
	jmp	.LBB23_34
.LBB23_19:
	mov	eax, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	eax, ecx
	cmovg	ebx, eax
	sub	ebx, ecx
	add	rcx, qword ptr [rsi + 40]
	cmp	ebx, 71
	mov	r15d, dword ptr [rbp + 256]
	jg	.LBB23_21
	movsxd	rdi, ebx
	lea	rdx, [rbp - 72]
	mov	r8, rdi
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	lea	rdx, [rbp + rdi - 72]
	mov	eax, 72
	sub	eax, ebx
	movsxd	r8, eax
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	jmp	.LBB23_22
.LBB23_21:
	vmovups	ymm0, ymmword ptr [rbp - 72]
	vmovups	ymmword ptr [rcx], ymm0
	vmovdqu	ymm0, ymmword ptr [rbp - 40]
	vmovdqu	ymmword ptr [rcx + 32], ymm0
	mov	rax, qword ptr [rbp - 8]
	mov	qword ptr [rcx + 64], rax
.LBB23_22:
	mov	ebx, dword ptr [rbp + 288]
	mov	eax, dword ptr [rsi + 52]
	mov	ecx, dword ptr [rsi + 56]
	add	eax, 72
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], 72
	cmp	byte ptr [rbp + 24], 0
	je	.LBB23_34
.LBB23_25:
	mov	r14, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	edi, r15d
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	qword ptr [rbp + 16], r14
	jmp	.LBB23_26
	.p2align	4
.LBB23_49:
	mov	dword ptr [rsp + 32], 0
	xor	ecx, ecx
	mov	edx, 2
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.ArchetypeChunk* chunks, int chunkCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0065"+2]
	xor	r9d, r9d
	vzeroupper
	call	r14
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
.LBB23_33:
	add	r13, 16
	dec	rdi
	je	.LBB23_34
.LBB23_26:
	movsxd	r8, dword ptr [r13]
	mov	rax, qword ptr [r10]
	mov	rcx, r8
	shl	rcx, 4
	mov	rdx, qword ptr [rax + rcx]
	mov	r9, r8
	sar	r9, 3
	and	r9, -8
	and	r8d, 63
	shl	r8d, 14
	add	r8, qword ptr [r11 + r9]
	mov	rdx, qword ptr [rdx + 224]
	movsxd	rdx, dword ptr [rdx]
	add	rdx, r8
	add	rdx, 64
	je	.LBB23_50
	mov	r15d, dword ptr [rax + rcx + 8]
	shl	r15d, 3
	test	r15d, r15d
	jle	.LBB23_33
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r15d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jg	.LBB23_49
	mov	qword ptr [rbp + 24], r11
	mov	qword ptr [rbp + 8], r10
	mov	r8d, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	r8d, ecx
	cmovg	eax, r8d
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r12d, r15d
	sub	r12d, eax
	jle	.LBB23_31
	movsxd	rbx, eax
	mov	r14, rdx
	mov	r8, rbx
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	r14, rbx
	mov	ebx, dword ptr [rbp + 288]
	movsxd	r8, r12d
	mov	rdx, r14
	mov	r14, qword ptr [rbp + 16]
	jmp	.LBB23_32
.LBB23_31:
	mov	r8d, r15d
.LBB23_32:
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, r15d
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], r15d
	mov	r10, qword ptr [rbp + 8]
	mov	r11, qword ptr [rbp + 24]
	jmp	.LBB23_33
.LBB23_34:
	mov	rdi, qword ptr [rbp + 280]
	mov	r14, qword ptr [rbp + 264]
	test	r14, r14
	setne	al
	mov	r12d, dword ptr [rbp + 272]
	test	r12d, r12d
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB23_42
	shl	r12d, 2
	test	r12d, r12d
	jle	.LBB23_42
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r12d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB23_37
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB23_42
.LBB23_37:
	mov	r8d, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	r8d, ecx
	cmovg	eax, r8d
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r15d, r12d
	sub	r15d, eax
	jle	.LBB23_39
	movsxd	rbx, eax
	mov	rdx, r14
	mov	r8, rbx
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	r14, rbx
	mov	ebx, dword ptr [rbp + 288]
	movsxd	r8, r15d
	jmp	.LBB23_40
.LBB23_39:
	mov	r8d, r12d
.LBB23_40:
	mov	rdx, r14
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, r12d
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], r12d
.LBB23_42:
	test	rdi, rdi
	setne	al
	test	ebx, ebx
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB23_16
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, ebx
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB23_44
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB23_16
.LBB23_44:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r14d, ebx
	sub	r14d, eax
	jle	.LBB23_46
	movsxd	rbx, eax
	mov	rdx, rdi
	mov	r8, rbx
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	rdi, rbx
	mov	ebx, dword ptr [rbp + 288]
	movsxd	r8, r14d
	jmp	.LBB23_47
.LBB23_46:
	mov	r8d, ebx
.LBB23_47:
	mov	rdx, rdi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, ebx
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], ebx
.LBB23_16:
	mov	dword ptr [rsi + 80], 0
	vmovaps	xmm6, xmmword ptr [rbp + 32]
	vmovaps	xmm7, xmmword ptr [rbp + 48]
	vmovaps	xmm8, xmmword ptr [rbp + 64]
	vmovaps	xmm9, xmmword ptr [rbp + 80]
	vmovaps	xmm10, xmmword ptr [rbp + 96]
	vmovaps	xmm11, xmmword ptr [rbp + 112]
	.seh_startepilogue
	add	rsp, 264
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
.LBB23_50:
	lea	rcx, [rip + burst.string.768c1a52ff82ef88cabe676b503130fa]
	lea	rdx, [rip + burst.string.1c3b0f8201e0c56284796afecfd7f5fb]
	vzeroupper
	call	qword ptr [rip + burst_abort_Ptr]
	int3
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	.p2align	4
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 136
	.seh_stackalloc 136
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	r13d, r9d
	mov	r12, r8
	mov	r15d, edx
	mov	rsi, rcx
	mov	edx, dword ptr [rbp + 160]
	mov	edi, dword ptr [rbp + 144]
	mov	ebx, dword ptr [rbp + 128]
	mov	ecx, 1
	.p2align	4
.LBB24_1:
	xor	eax, eax
	lock		cmpxchg	dword ptr [rsi + 80], ecx
	je	.LBB24_2
.LBB24_4:
	mov	eax, dword ptr [rsi + 80]
	cmp	eax, 1
	je	.LBB24_4
	jmp	.LBB24_1
.LBB24_2:
	mov	dword ptr [rsp + 32], edx
	mov	rcx, rsi
	mov	edx, r15d
	mov	r8d, ebx
	mov	r9d, edi
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	test	al, 1
	je	.LBB24_3
	mov	r14, qword ptr [rbp + 112]
	lea	rcx, [rbp - 88]
	mov	rdx, rsi
	mov	r8, r12
	mov	r9d, r13d
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetSystemHandle(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion) -> Unity.Entities.SystemHandle, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	vmovups	xmm0, xmmword ptr [rbp - 88]
	mov	rax, qword ptr [r12 + 376]
	mov	rcx, qword ptr [rsi + 88]
	lea	rdx, [rcx + 1]
	mov	qword ptr [rsi + 88], rdx
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Entities.EntitiesJournaling/FrameCountSystem/SharedFrameCount::Ref"]
	mov	edx, dword ptr [rdx]
	mov	r8d, dword ptr [r14 + 12]
	mov	qword ptr [rbp - 72], rcx
	mov	dword ptr [rbp - 64], r15d
	mov	dword ptr [rbp - 60], edx
	mov	qword ptr [rbp - 56], rax
	vmovups	xmmword ptr [rbp - 48], xmm0
	mov	rax, qword ptr [r14]
	mov	qword ptr [rbp - 32], rax
	mov	eax, dword ptr [r14 + 8]
	mov	r14d, dword ptr [rbp + 160]
	mov	dword ptr [rbp - 24], eax
	mov	dword ptr [rbp - 20], r8d
	mov	dword ptr [rbp - 16], ebx
	mov	dword ptr [rbp - 12], edi
	mov	dword ptr [rbp - 8], r14d
	mov	dword ptr [rbp - 4], 0
	mov	r12d, dword ptr [rsi + 56]
	mov	eax, dword ptr [rsi + 60]
	add	eax, 72
	cmp	eax, r12d
	jle	.LBB24_6
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	r9, qword ptr [rbp + 120]
	jmp	.LBB24_11
.LBB24_6:
	mov	eax, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	eax, ecx
	cmovg	r12d, eax
	sub	r12d, ecx
	add	rcx, qword ptr [rsi + 40]
	cmp	r12d, 71
	jg	.LBB24_8
	movsxd	r15, r12d
	lea	rdx, [rbp - 72]
	mov	r8, r15
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	lea	rdx, [rbp + r15 - 72]
	mov	eax, 72
	sub	eax, r12d
	movsxd	r8, eax
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	jmp	.LBB24_9
.LBB24_8:
	vmovups	ymm0, ymmword ptr [rbp - 72]
	vmovups	ymmword ptr [rcx], ymm0
	vmovups	ymm0, ymmword ptr [rbp - 40]
	vmovups	ymmword ptr [rcx + 32], ymm0
	mov	rax, qword ptr [rbp - 8]
	mov	qword ptr [rcx + 64], rax
.LBB24_9:
	mov	r9, qword ptr [rbp + 120]
	mov	eax, dword ptr [rsi + 52]
	mov	ecx, dword ptr [rsi + 56]
	add	eax, 72
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], 72
.LBB24_11:
	mov	r15, qword ptr [rbp + 136]
	test	r9, r9
	setne	al
	test	ebx, ebx
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB24_19
	shl	ebx, 3
	test	ebx, ebx
	jle	.LBB24_19
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, ebx
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB24_14
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.Entity* entities, int entityCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB24_19
.LBB24_14:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r13d, ebx
	sub	r13d, eax
	jle	.LBB24_16
	movsxd	r8, eax
	mov	qword ptr [rbp], r8
	mov	rdx, r9
	mov	r12, r9
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	r12, qword ptr [rbp]
	movsxd	r8, r13d
	mov	rdx, r12
	jmp	.LBB24_17
.LBB24_16:
	mov	r8d, ebx
	mov	rdx, r9
.LBB24_17:
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, ebx
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], ebx
.LBB24_19:
	mov	rbx, qword ptr [rbp + 152]
	test	r15, r15
	setne	al
	test	edi, edi
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB24_27
	shl	edi, 2
	test	edi, edi
	jle	.LBB24_27
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, edi
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB24_22
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB24_27
.LBB24_22:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r12d, edi
	sub	r12d, eax
	jle	.LBB24_24
	movsxd	r14, eax
	mov	rdx, r15
	mov	r8, r14
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	r15, r14
	mov	r14d, dword ptr [rbp + 160]
	movsxd	r8, r12d
	jmp	.LBB24_25
.LBB24_24:
	mov	r8d, edi
.LBB24_25:
	mov	rdx, r15
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, edi
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], edi
.LBB24_27:
	test	rbx, rbx
	setne	al
	test	r14d, r14d
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB24_3
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r14d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB24_29
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB24_3
.LBB24_29:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r15d, r14d
	sub	r14d, eax
	jle	.LBB24_31
	movsxd	rdi, eax
	mov	rdx, rbx
	mov	r8, rdi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	rcx, qword ptr [rsi + 40]
	add	rbx, rdi
	movsxd	r8, r14d
	jmp	.LBB24_32
.LBB24_31:
	mov	r8d, r15d
.LBB24_32:
	mov	rdx, rbx
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	mov	eax, dword ptr [rsi + 52]
	add	eax, r15d
	mov	ecx, dword ptr [rsi + 56]
	cdq
	idiv	ecx
	mov	eax, edx
	sar	eax, 31
	and	eax, ecx
	add	eax, edx
	mov	dword ptr [rsi + 52], eax
	add	dword ptr [rsi + 60], r15d
.LBB24_3:
	mov	dword ptr [rsi + 80], 0
	.seh_startepilogue
	add	rsp, 136
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.globl	__ymm@6f70206f742064656c696146203a676e696c616e72756f4a7365697469746e45
	.section	.rdata,"dr",discard,__ymm@6f70206f742064656c696146203a676e696c616e72756f4a7365697469746e45
	.p2align	5, 0x0
__ymm@6f70206f742064656c696146203a676e696c616e72756f4a7365697469746e45:
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
	.globl	__ymm@65726f747320746f6e6e6143203a676e696c616e72756f4a7365697469746e45
	.section	.rdata,"dr",discard,__ymm@65726f747320746f6e6e6143203a676e696c616e72756f4a7365697469746e45
	.p2align	5, 0x0
__ymm@65726f747320746f6e6e6143203a676e696c616e72756f4a7365697469746e45:
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
	.globl	__ymm@6e696c616e72756f4a7365697469746e452e7365697469746e452e7974696e55
	.section	.rdata,"dr",discard,__ymm@6e696c616e72756f4a7365697469746e452e7365697469746e452e7974696e55
	.p2align	5, 0x0
__ymm@6e696c616e72756f4a7365697469746e452e7365697469746e452e7974696e55:
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
	.globl	__ymm@2e6867756f6e6520656772616c20746f6e20726566667562202c746e65766520
	.section	.rdata,"dr",discard,__ymm@2e6867756f6e6520656772616c20746f6e20726566667562202c746e65766520
	.p2align	5, 0x0
__ymm@2e6867756f6e6520656772616c20746f6e20726566667562202c746e65766520:
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
	.globl	__ymm@6f7420676e696c616e72756f6a207365697469746e65206573616572636e4920
	.section	.rdata,"dr",discard,__ymm@6f7420676e696c616e72756f6a207365697469746e65206573616572636e4920
	.p2align	5, 0x0
__ymm@6f7420676e696c616e72756f6a207365697469746e65206573616572636e4920:
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
	.globl	__xmm@7270206e692079726f6d656d206c6174
	.section	.rdata,"dr",discard,__xmm@7270206e692079726f6d656d206c6174
	.p2align	4, 0x0
__xmm@7270206e692079726f6d656d206c6174:
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
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 232
	.seh_stackalloc 232
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	eax, dword ptr [rbp + 208]
	shl	r9d, 2
	lea	edx, [r9 + 8*r8]
	lea	r8d, [rax + rdx + 72]
	mov	r11d, dword ptr [rcx + 56]
	cmp	r8d, r11d
	jle	.LBB25_1
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 80], ymm0
	vmovups	ymmword ptr [rbp - 48], ymm0
	vmovups	ymmword ptr [rbp - 16], ymm0
	vmovups	ymmword ptr [rbp + 16], ymm0
	vmovups	ymmword ptr [rbp + 48], ymm0
	mov	qword ptr [rbp + 80], 0
	mov	word ptr [rbp + 88], 0
	mov	byte ptr [rbp + 90], 0
	lea	r8, [rbp - 78]
	vmovaps	ymm0, ymmword ptr [rip + __ymm@65726f747320746f6e6e6143203a676e696c616e72756f4a7365697469746e45]
	vmovups	ymmword ptr [rbp - 78], ymm0
	mov	byte ptr [rbp - 46], 32
	vmovaps	ymm0, ymmword ptr [rip + __ymm@6e696c616e72756f4a7365697469746e452e7365697469746e452e7974696e55]
	vmovups	ymmword ptr [rbp - 45], ymm0
	movabs	rax, 7237970023830138471
	mov	qword ptr [rbp - 13], rax
	mov	dword ptr [rbp - 5], 1701869908
	vmovaps	ymm0, ymmword ptr [rip + __ymm@2e6867756f6e6520656772616c20746f6e20726566667562202c746e65766520]
	vmovups	ymmword ptr [rbp - 1], ymm0
	vmovaps	ymm0, ymmword ptr [rip + __ymm@6f7420676e696c616e72756f6a207365697469746e65206573616572636e4920]
	vmovups	ymmword ptr [rbp + 31], ymm0
	vmovaps	xmm0, xmmword ptr [rip + __xmm@7270206e692079726f6d656d206c6174]
	vmovups	xmmword ptr [rbp + 63], xmm0
	movabs	rax, 7305804402515076709
	mov	qword ptr [rbp + 79], rax
	mov	word ptr [rbp + 87], 11891
	mov	word ptr [rbp - 80], 167
	mov	dword ptr [rsp + 32], 0
	xor	edi, edi
	xor	ecx, ecx
	mov	edx, 2
	jmp	.LBB25_23
.LBB25_1:
	jne	.LBB25_2
	mov	dword ptr [rcx + 28], 0
	mov	qword ptr [rcx + 16], 0
	mov	dword ptr [rcx + 60], 0
	mov	qword ptr [rcx + 48], 0
	xor	esi, esi
	xor	r9d, r9d
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	jne	.LBB25_6
	jmp	.LBB25_4
.LBB25_2:
	mov	esi, dword ptr [rcx + 28]
	mov	r9d, dword ptr [rcx + 52]
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	je	.LBB25_4
.LBB25_6:
	mov	eax, r11d
	sub	eax, dword ptr [rcx + 60]
	cmp	eax, r8d
	jge	.LBB25_7
.LBB25_4:
	mov	dword ptr [rbp + 92], r9d
	test	esi, esi
	jle	.LBB25_5
	mov	rdx, qword ptr [rcx + 8]
	mov	ebx, dword ptr [rcx + 16]
	mov	r14d, edi
	sub	r14d, ebx
	mov	r15d, r11d
	sub	r15d, dword ptr [rcx + 60]
	xor	r12d, r12d
	xor	eax, eax
	xor	r9d, r9d
	.p2align	4
.LBB25_18:
	cmp	eax, r14d
	mov	r13d, edi
	cmovl	r13d, r12d
	lea	r10d, [rbx + rax]
	sub	r10d, r13d
	movsxd	r10, r10d
	add	r9d, dword ptr [rdx + 8*r10 + 4]
	lea	r10d, [r15 + r9]
	inc	eax
	cmp	r10d, r8d
	jge	.LBB25_13
	cmp	esi, eax
	jne	.LBB25_18
	mov	eax, esi
	jmp	.LBB25_13
.LBB25_5:
	xor	eax, eax
	xor	r9d, r9d
.LBB25_13:
	test	esi, esi
	setne	dl
	sub	esi, eax
	setge	bl
	test	dl, bl
	je	.LBB25_19
	add	eax, dword ptr [rcx + 16]
	cdq
	idiv	edi
	mov	eax, edx
	sar	eax, 31
	and	eax, edi
	add	eax, edx
	mov	dword ptr [rcx + 16], eax
	mov	dword ptr [rcx + 28], esi
	mov	ebx, dword ptr [rcx + 60]
	test	ebx, ebx
	je	.LBB25_24
	sub	ebx, r9d
	jl	.LBB25_24
	add	r9d, dword ptr [rcx + 48]
	mov	eax, r9d
	cdq
	idiv	r11d
	mov	eax, edx
	sar	eax, 31
	and	eax, r11d
	add	eax, edx
	mov	dword ptr [rcx + 48], eax
	mov	dword ptr [rcx + 60], ebx
	mov	r9d, dword ptr [rbp + 92]
.LBB25_7:
	cmp	esi, edi
	jne	.LBB25_8
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0153"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB25_9
.LBB25_19:
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 80], ymm0
	vmovups	ymmword ptr [rbp - 48], ymm0
	lea	rsi, [rbp - 78]
	vmovaps	ymm0, ymmword ptr [rip + __ymm@6f70206f742064656c696146203a676e696c616e72756f4a7365697469746e45]
	vmovups	ymmword ptr [rbp - 78], ymm0
	movabs	rcx, 2338615531451850864
	mov	qword ptr [rbp - 46], rcx
	mov	dword ptr [rbp + 100], 40
	mov	dword ptr [rsp + 32], 0
	lea	rdx, [rbp + 100]
	mov	r14d, 61
	mov	rcx, rsi
	mov	r8d, 61
	mov	r9d, eax
	vzeroupper
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	ebx, dword ptr [rbp + 100]
	sub	r14d, ebx
	test	r14d, r14d
	jle	.LBB25_22
	movsxd	rcx, ebx
	cmp	r14d, 9
	mov	edi, 9
	cmovb	edi, r14d
	add	rcx, rsi
	lea	rdx, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0113.3"+2]
	mov	r8, rdi
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	add	edi, ebx
	mov	dword ptr [rbp + 100], edi
	jmp	.LBB25_21
.LBB25_8:
	mov	rax, qword ptr [rcx + 8]
	movsxd	rdx, dword ptr [rcx + 20]
	mov	dword ptr [rax + 8*rdx], r9d
	mov	dword ptr [rax + 8*rdx + 4], r8d
	mov	eax, dword ptr [rcx + 20]
	mov	r8d, dword ptr [rcx + 24]
	inc	eax
	cdq
	idiv	r8d
	mov	eax, edx
	sar	eax, 31
	and	eax, r8d
	add	eax, edx
	mov	dword ptr [rcx + 20], eax
	inc	dword ptr [rcx + 28]
.LBB25_9:
	mov	dil, 1
	jmp	.LBB25_10
.LBB25_24:
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 80], ymm0
	vmovups	ymmword ptr [rbp - 48], ymm0
	mov	qword ptr [rbp - 16], 0
	lea	rsi, [rbp - 78]
	vmovaps	ymm0, ymmword ptr [rip + __ymm@6f70206f742064656c696146203a676e696c616e72756f4a7365697469746e45]
	vmovups	ymmword ptr [rbp - 78], ymm0
	movabs	rax, 2338615531451850864
	mov	qword ptr [rbp - 46], rax
	mov	dword ptr [rbp + 96], 40
	mov	dword ptr [rsp + 32], 0
	lea	rdx, [rbp + 96]
	mov	r14d, 69
	mov	rcx, rsi
	mov	r8d, 69
	vzeroupper
	call	"Unity.Burst.BurstString, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Format(System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dest, System.Int32&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destIndex, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 destLength, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 value, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 formatOptionsRaw) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	ebx, dword ptr [rbp + 96]
	sub	r14d, ebx
	test	r14d, r14d
	jle	.LBB25_22
	movsxd	rcx, ebx
	cmp	r14d, 17
	mov	edi, 17
	cmovb	edi, r14d
	add	rcx, rsi
	lea	rdx, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0138"+2]
	mov	r8, rdi
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	add	edi, ebx
	mov	dword ptr [rbp + 96], edi
.LBB25_21:
	mov	ebx, edi
.LBB25_22:
	movsxd	rax, ebx
	mov	byte ptr [rbp + rax - 78], 0
	mov	word ptr [rsi - 2], ax
	mov	dword ptr [rsp + 32], 0
	xor	edi, edi
	xor	ecx, ecx
	mov	edx, 2
	mov	r8, rsi
.LBB25_23:
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
.LBB25_10:
	mov	eax, edi
	.seh_startepilogue
	add	rsp, 232
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.globl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.p2align	4
"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80":
.seh_proc "Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 96
	.seh_stackalloc 96
	lea	rbp, [rsp + 96]
	.seh_setframe rbp, 96
	.seh_endprologue
	mov	eax, r8d
	mov	r8, rdx
	mov	edx, ecx
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	cmp	byte ptr [rcx], 0
	je	.LBB26_3
	mov	r15d, dword ptr [rbp + 136]
	mov	r11, qword ptr [rbp + 128]
	mov	ecx, dword ptr [rbp + 120]
	mov	rsi, qword ptr [rbp + 112]
	mov	r10, qword ptr [rbp + 104]
	mov	ebx, dword ptr [rbp + 96]
	mov	edi, ebx
	sar	edi, 31
	and	edi, ebx
	test	r9, r9
	cmovne	edi, ebx
	mov	ebx, ecx
	sar	ebx, 31
	and	ebx, ecx
	mov	r14d, r15d
	sar	r14d, 31
	and	r14d, r15d
	test	r11, r11
	cmovne	r14d, r15d
	test	rsi, rsi
	cmovne	ebx, ecx
	sete	r15b
	test	ebx, ebx
	setle	r12b
	or	r12b, r15b
	jne	.LBB26_2
	mov	r15d, dword ptr [rsi]
	mov	r12, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	cmp	r15d, dword ptr [r12]
	jne	.LBB26_5
	add	rsi, 4
	dec	ebx
	jmp	.LBB26_2
.LBB26_5:
	mov	ebx, ecx
.LBB26_2:
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	dword ptr [rsp + 80], r14d
	mov	qword ptr [rsp + 72], r11
	mov	dword ptr [rsp + 64], ebx
	mov	qword ptr [rsp + 56], rsi
	mov	dword ptr [rsp + 48], edi
	mov	qword ptr [rsp + 40], r9
	mov	qword ptr [rsp + 32], r10
	mov	r9d, eax
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunks, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
.LBB26_3:
	nop
	.seh_startepilogue
	add	rsp, 96
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.globl	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.p2align	4
"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80":
.seh_proc "Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 96
	.seh_stackalloc 96
	lea	rbp, [rsp + 96]
	.seh_setframe rbp, 96
	.seh_endprologue
	mov	eax, r8d
	mov	r8, rdx
	mov	edx, ecx
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Boolean> Unity.Entities.EntitiesJournaling/SharedInit::Ref"]
	cmp	byte ptr [rcx], 0
	je	.LBB27_3
	mov	r15d, dword ptr [rbp + 136]
	mov	r11, qword ptr [rbp + 128]
	mov	ecx, dword ptr [rbp + 120]
	mov	rsi, qword ptr [rbp + 112]
	mov	r10, qword ptr [rbp + 104]
	mov	ebx, dword ptr [rbp + 96]
	mov	edi, ebx
	sar	edi, 31
	and	edi, ebx
	test	r9, r9
	cmovne	edi, ebx
	mov	ebx, ecx
	sar	ebx, 31
	and	ebx, ecx
	mov	r14d, r15d
	sar	r14d, 31
	and	r14d, r15d
	test	r11, r11
	cmovne	r14d, r15d
	test	rsi, rsi
	cmovne	ebx, ecx
	sete	r15b
	test	ebx, ebx
	setle	r12b
	or	r12b, r15b
	jne	.LBB27_2
	mov	r15d, dword ptr [rsi]
	mov	r12, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	cmp	r15d, dword ptr [r12]
	jne	.LBB27_5
	add	rsi, 4
	dec	ebx
	jmp	.LBB27_2
.LBB27_5:
	mov	ebx, ecx
.LBB27_2:
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntitiesJournaling/JournalingState> Unity.Entities.EntitiesJournaling/SharedState::Ref"]
	mov	dword ptr [rsp + 80], r14d
	mov	qword ptr [rsp + 72], r11
	mov	dword ptr [rsp + 64], ebx
	mov	qword ptr [rsp + 56], rsi
	mov	dword ptr [rsp + 48], edi
	mov	qword ptr [rsp + 40], r9
	mov	qword ptr [rsp + 32], r10
	mov	r9d, eax
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBack(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null store, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@88"
.LBB27_3:
	nop
	.seh_startepilogue
	add	rsp, 96
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetComponentDataWithTypeRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalVersion, Unity.Entities.LookupCache&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null cache) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 72
	.seh_stackalloc 72
	lea	rbp, [rsp + 64]
	.seh_setframe rbp, 64
	.seh_endprologue
	mov	rsi, qword ptr [rbp + 120]
	mov	r10d, dword ptr [rbp + 112]
	mov	r11, qword ptr [rcx + 8]
	movsxd	rdi, edx
	movsxd	rax, dword ptr [r11 + 8*rdi]
	movsxd	r11, dword ptr [r11 + 8*rdi + 4]
	mov	r14, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r12, qword ptr [r14]
	mov	rbx, rax
	shl	rbx, 4
	mov	rdi, qword ptr [r12 + rbx]
	cmp	qword ptr [rsi], rdi
	jne	.LBB28_2
	movzx	r15d, word ptr [rsi + 14]
.LBB28_14:
	movsxd	rbx, dword ptr [r12 + rbx + 12]
	movsx	r14d, r15w
	mov	r15d, dword ptr [rdi + 8]
	lea	r12d, [4*r15]
	movsxd	r12, r12d
	add	r12, qword ptr [rdi]
	imul	r14d, r15d
	movsxd	rdi, r14d
	lea	rdi, [r12 + 4*rdi]
	mov	dword ptr [rdi + 4*rbx], r10d
	mov	rdi, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	rbx, rax
	sar	rbx, 3
	and	rbx, -8
	and	eax, 63
	shl	eax, 14
	add	rax, qword ptr [rdi + rbx]
	movsxd	rdi, dword ptr [rsi + 8]
	movzx	esi, word ptr [rsi + 12]
	imul	r11, rsi
	add	r11, rdi
	lea	rax, [rax + r11 + 64]
	cmp	byte ptr [rcx + 494], 0
	jne	.LBB28_15
.LBB28_16:
	.seh_startepilogue
	add	rsp, 72
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB28_2:
	mov	r15, qword ptr [rdi + 112]
	mov	qword ptr [rbp - 8], r15
	mov	r15d, dword ptr [rdi + 168]
	mov	dword ptr [rbp + 4], r15d
	movsx	r15d, word ptr [rsi + 14]
	test	r15d, r15d
	js	.LBB28_4
	cmp	dword ptr [rbp + 4], r15d
	jle	.LBB28_4
	movsxd	r12, r15d
	mov	r13, qword ptr [rbp - 8]
	cmp	dword ptr [r13 + 4*r12], r9d
	je	.LBB28_12
.LBB28_4:
	cmp	dword ptr [rbp + 4], 0
	je	.LBB28_9
	xor	r15d, r15d
	mov	r13, qword ptr [rbp - 8]
.LBB28_7:
	movsxd	r12, r15d
	cmp	r9d, dword ptr [r13 + 4*r12]
	je	.LBB28_8
	inc	r15d
	cmp	dword ptr [rbp + 4], r15d
	jne	.LBB28_7
.LBB28_9:
	mov	word ptr [rsi + 14], -1
.LBB28_10:
	mov	dword ptr [rsi + 8], 0
	mov	r15w, -1
	xor	r12d, r12d
	jmp	.LBB28_13
.LBB28_15:
	mov	qword ptr [rsp + 40], rax
	mov	dword ptr [rsp + 32], r10d
	mov	rsi, rax
	call	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	rax, rsi
	jmp	.LBB28_16
.LBB28_8:
	mov	word ptr [rsi + 14], r15w
	cmp	r15w, -1
	je	.LBB28_10
.LBB28_12:
	mov	r12, qword ptr [rdi + 224]
	movsx	r13, r15w
	mov	r12d, dword ptr [r12 + 4*r13]
	mov	dword ptr [rsi + 8], r12d
	mov	r12, qword ptr [rdi + 232]
	movzx	r12d, word ptr [r12 + 2*r13]
.LBB28_13:
	mov	word ptr [rsi + 12], r12w
	mov	qword ptr [rsi], rdi
	mov	r12, qword ptr [r14]
	jmp	.LBB28_14
	.seh_endproc

	.def	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Unity.Entities.EntityComponentStore, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.JournalAddRecordGetRW(Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.Entity, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entity, Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeIndex, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 version, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	push	rbp
	.seh_pushreg rbp
	sub	rsp, 112
	.seh_stackalloc 112
	lea	rbp, [rsp + 112]
	.seh_setframe rbp, 112
	.seh_endprologue
	mov	dword ptr [rbp - 12], edx
	mov	dword ptr [rbp - 8], r8d
	mov	dword ptr [rbp - 4], r9d
	test	r9d, 134217728
	jne	.LBB29_7
	mov	rax, rcx
	mov	r8d, dword ptr [rbp + 48]
	mov	ecx, r9d
	and	ecx, 805306368
	cmp	ecx, 268435456
	jne	.LBB29_3
	xor	edx, edx
	mov	ecx, 15
	jmp	.LBB29_5
.LBB29_3:
	test	r9d, 67108864
	jne	.LBB29_4
	mov	rdx, qword ptr [rbp + 56]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"]
	mov	rcx, qword ptr [rcx]
	and	r9d, 8191
	imul	r9, r9, 104
	mov	r9d, dword ptr [rcx + r9 + 100]
	mov	ecx, 14
	jmp	.LBB29_6
.LBB29_4:
	xor	edx, edx
	mov	ecx, 16
.LBB29_5:
	xor	r9d, r9d
.LBB29_6:
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp - 28], xmm0
	mov	dword ptr [rsp + 72], r9d
	mov	qword ptr [rsp + 64], rdx
	lea	rdx, [rbp - 4]
	mov	qword ptr [rsp + 48], rdx
	lea	rdx, [rbp - 28]
	mov	qword ptr [rsp + 40], rdx
	mov	dword ptr [rsp + 56], 1
	mov	dword ptr [rsp + 32], 1
	lea	r9, [rbp - 12]
	mov	rdx, rax
	call	"Unity.Entities.EntitiesJournaling, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AddRecord(Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, Unity.Entities.EntityComponentStore*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entityComponentStore, System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 globalSystemVersion, Unity.Entities.Entity*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null entities, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, Unity.Entities.SystemHandle&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null originSystem, Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null types, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 data, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
.LBB29_7:
	nop
	.seh_startepilogue
	add	rsp, 112
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@ffffffffffffffffffffffffffffffff
	.section	.rdata,"dr",discard,__xmm@ffffffffffffffffffffffffffffffff
	.p2align	4, 0x0
__xmm@ffffffffffffffffffffffffffffffff:
	.zero	16,255
	.section	.text,"xr",discard,"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 376
	.seh_stackalloc 376
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovdqa	xmmword ptr [rbp + 224], xmm6
	.seh_savexmm xmm6, 352
	.seh_endprologue
	mov	dword ptr [rbp + 188], r9d
	mov	qword ptr [rbp - 64], r8
	mov	rbx, rcx
	mov	dword ptr [rbp + 212], 0
	mov	dword ptr [rbp + 216], 0
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp + 136], xmm0
	vmovups	xmmword ptr [rbp - 80], xmm0
	vmovups	xmmword ptr [rbp + 192], xmm0
	vmovups	xmmword ptr [rbp + 152], xmm0
	mov	rcx, qword ptr [rcx + 792]
	mov	r10, qword ptr [rbx + 808]
	mov	rax, qword ptr [rbx + 832]
	mov	qword ptr [rbp + 176], rax
	mov	eax, dword ptr [rbx + 892]
	mov	rdx, qword ptr [rbx + 816]
	mov	r11, qword ptr [rbx + 824]
	mov	r8, qword ptr [rcx]
	mov	r9, qword ptr [r10]
	mov	qword ptr [rbp + 168], r10
	mov	ecx, dword ptr [r10 + 8]
	mov	r10, qword ptr [rdx]
	mov	r11, qword ptr [r11]
	mov	edx, dword ptr [rbx + 872]
	or	edx, dword ptr [rbx + 852]
	sete	dl
	jne	.LBB30_3
	cmp	dword ptr [rbx + 884], 0
	je	.LBB30_2
.LBB30_3:
	mov	sil, 1
.LBB30_4:
	test	eax, eax
	mov	dword ptr [rbp - 32], ecx
	mov	dword ptr [rbp - 28], 0
	mov	qword ptr [rbp - 24], r9
	mov	r9, qword ptr [rbp + 176]
	mov	qword ptr [rbp - 16], r9
	mov	qword ptr [rbp - 8], r10
	mov	qword ptr [rbp], r11
	mov	qword ptr [rbp + 8], r8
	vmovdqu	ymm0, ymmword ptr [rbx + 848]
	vmovdqu	ymmword ptr [rbp + 16], ymm0
	mov	r8, qword ptr [rbx + 880]
	mov	qword ptr [rbp + 48], r8
	mov	byte ptr [rbp + 56], sil
	setne	byte ptr [rbp + 57]
	mov	word ptr [rbp + 58], 0
	mov	dword ptr [rbp + 60], -1
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	ymmword ptr [rbp + 64], ymm0
	vmovdqu	ymmword ptr [rbp + 96], ymm0
	mov	qword ptr [rbp + 128], 0
	mov	r8d, dword ptr [rbx + 888]
	mov	dword ptr [rbp + 208], r8d
	mov	byte ptr [rbp + 223], 1
	test	dl, dl
	je	.LBB30_6
	cmp	dword ptr [rbx + 884], 0
	setne	byte ptr [rbp + 223]
.LBB30_6:
	lea	r14, [rbp + 16]
	cmp	dword ptr [rbp + 208], 1
	mov	dword ptr [rbp + 212], 0
	mov	dword ptr [rbp + 216], ecx
	jne	.LBB30_7
	jmp	.LBB30_11
.LBB30_2:
	xor	esi, esi
	jmp	.LBB30_4
.LBB30_7:
	test	eax, eax
	setne	al
	or	al, byte ptr [rbp + 223]
	test	al, 1
	je	.LBB30_13
	mov	eax, dword ptr [rbp + 216]
	mov	dword ptr [rbp - 32], eax
	mov	r13d, dword ptr [rbp + 212]
	cmp	r13d, eax
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jge	.LBB30_26
	lea	rax, [rbx + 160]
	mov	qword ptr [rbp - 56], rax
	vpcmpeqd	xmm6, xmm6, xmm6
.LBB30_19:
	movsxd	r12, r13d
	jmp	.LBB30_20
	.p2align	4
.LBB30_42:
	vptest	xmm0, xmm0
	jne	.LBB30_43
.LBB30_25:
	inc	r12
	movsxd	rax, dword ptr [rbp - 32]
	cmp	r12, rax
	jge	.LBB30_26
.LBB30_20:
	mov	rax, qword ptr [rbp - 8]
	mov	rcx, qword ptr [rbp]
	movsxd	r13, dword ptr [rcx + 4*r12]
	mov	eax, dword ptr [rax + 4*r12]
	cmp	dword ptr [rbp + 60], eax
	jne	.LBB30_21
.LBB30_23:
	cmp	byte ptr [rbp + 56], 0
	je	.LBB30_49
	mov	rcx, qword ptr [rbp + 64]
	mov	edx, r13d
	mov	r8, r14
	vzeroupper
	call	"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	test	al, 1
	je	.LBB30_25
.LBB30_49:
	cmp	byte ptr [rbp + 57], 0
	je	.LBB30_50
	mov	rax, qword ptr [rbp + 72]
	mov	rdx, qword ptr [rbp + 80]
	mov	eax, dword ptr [rax + 4*r13]
	mov	r15d, dword ptr [rdx + 12]
	mov	r9d, dword ptr [rdx + 16]
	mov	ecx, dword ptr [rbp + 128]
	imul	ecx, r13d
	movsxd	rcx, ecx
	shl	rcx, 4
	add	rcx, qword ptr [rbp + 120]
	test	r15, r15
	mov	r11d, dword ptr [rdx + 20]
	mov	edx, dword ptr [rdx + 24]
	mov	rdi, qword ptr [rbp + 88]
	mov	r10, qword ptr [rbp + 104]
	mov	r13, qword ptr [rbp + 96]
	mov	r8, qword ptr [rbp + 112]
	vmovdqu	xmmword ptr [rbp + 192], xmm6
	je	.LBB30_28
	vpcmpeqd	xmm0, xmm0, xmm0
	test	r15d, r15d
	jle	.LBB30_38
	.p2align	4
.LBB30_37:
	movsxd	rsi, dword ptr [rdi]
	shl	rsi, 4
	vpand	xmm0, xmm0, xmmword ptr [rcx + rsi]
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	rdi, 4
	dec	r15
	jne	.LBB30_37
.LBB30_38:
	test	r11d, r11d
	jle	.LBB30_39
	movsxd	rsi, dword ptr [r13]
	shl	rsi, 4
	vmovdqu	xmm1, xmmword ptr [rcx + rsi]
	cmp	r11d, 1
	je	.LBB30_47
	dec	r11
	add	r13, 4
	.p2align	4
.LBB30_46:
	movsxd	rsi, dword ptr [r13]
	shl	rsi, 4
	vpor	xmm1, xmm1, xmmword ptr [rcx + rsi]
	add	r13, 4
	dec	r11
	jne	.LBB30_46
.LBB30_47:
	vpand	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
.LBB30_39:
	test	r9d, r9d
	jle	.LBB30_40
	.p2align	4
.LBB30_48:
	movsxd	r11, dword ptr [r10]
	shl	r11, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r11]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	r10, 4
	dec	r9
	jne	.LBB30_48
.LBB30_40:
	test	edx, edx
	jle	.LBB30_42
	.p2align	4
.LBB30_41:
	movsxd	r9, dword ptr [r8]
	shl	r9, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r9]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	r8, 4
	dec	rdx
	jne	.LBB30_41
	jmp	.LBB30_42
.LBB30_21:
	movsxd	rcx, eax
	mov	dword ptr [rbp + 60], eax
	mov	rax, qword ptr [rbp + 8]
	mov	r11, qword ptr [rax + 8*rcx]
	mov	qword ptr [rbp + 64], r11
	mov	rcx, qword ptr [r11]
	mov	edx, dword ptr [rcx + 8]
	shl	edx, 2
	movsxd	rdx, edx
	mov	r8, qword ptr [rcx]
	add	r8, rdx
	imul	edx, dword ptr [rcx + 20]
	movsxd	rcx, edx
	add	rcx, r8
	mov	qword ptr [rbp + 72], rcx
	cmp	byte ptr [rbp + 57], 0
	je	.LBB30_23
	mov	r8, qword ptr [r11]
	mov	esi, dword ptr [r8 + 8]
	mov	ecx, dword ptr [r8 + 20]
	movsxd	rdx, dword ptr [r11 + 8]
	lea	rax, [r11 + 4*rdx]
	lea	rdx, [r11 + 4*rdx + 28]
	mov	qword ptr [rbp - 48], rdx
	movsxd	r9, dword ptr [r11 + 12]
	lea	rax, [rax + 4*r9 + 28]
	mov	qword ptr [rbp - 40], rax
	movsxd	r9, dword ptr [r11 + 16]
	lea	r9, [rax + 4*r9]
	movsxd	r10, dword ptr [r11 + 20]
	lea	r10, [r9 + 4*r10]
	lea	edi, [4*rsi]
	mov	rdx, rsi
	movsxd	rsi, edi
	mov	r15d, ecx
	imul	r15d, esi
	movsxd	r15, r15d
	mov	eax, dword ptr [r8 + 16]
	imul	eax, esi
	add	rsi, rsi
	movsxd	rdi, eax
	lea	eax, [rdi + 8*rdx]
	add	eax, r15d
	and	eax, 12
	add	rsi, qword ptr [r8]
	add	rdi, r15
	add	rdi, rsi
	sub	rdi, rax
	add	rdi, 16
	shl	ecx, 4
	sar	ecx, 4
	mov	qword ptr [rbp + 80], r11
	mov	rax, qword ptr [rbp - 48]
	mov	qword ptr [rbp + 88], rax
	mov	qword ptr [rbp + 96], r9
	mov	rax, qword ptr [rbp - 40]
	mov	qword ptr [rbp + 104], rax
	mov	qword ptr [rbp + 112], r10
	mov	qword ptr [rbp + 120], rdi
	mov	dword ptr [rbp + 128], ecx
	mov	dword ptr [rbp + 132], 0
	jmp	.LBB30_23
.LBB30_28:
	mov	esi, 128
	sub	esi, eax
	cmp	esi, 127
	jg	.LBB30_29
	cmp	eax, 128
	je	.LBB30_31
	cmp	esi, 63
	mov	rdi, -1
	shrx	rsi, rdi, rsi
	jg	.LBB30_34
	shlx	rdi, rdi, rax
	or	rdi, rsi
	vmovq	xmm0, rdi
	vmovq	xmm1, rsi
	vpunpcklqdq	xmm0, xmm0, xmm1
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB30_38
.LBB30_34:
	vmovq	xmm0, rsi
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB30_38
.LBB30_29:
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB30_38
.LBB30_31:
	vpcmpeqd	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB30_38
	.p2align	4
.LBB30_50:
	xor	r9d, r9d
	jmp	.LBB30_18
	.p2align	4
.LBB30_43:
	vmovq	rcx, xmm0
	vpextrq	rdx, xmm0, 1
	popcnt	rcx, rcx
	popcnt	rdx, rdx
	add	edx, ecx
	cmp	edx, eax
	setne	r9b
.LBB30_18:
	movsxd	r13, r12d
	mov	rax, qword ptr [rbp - 24]
	mov	rcx, qword ptr [rbp - 16]
	mov	eax, dword ptr [rax + 4*r13]
	mov	dword ptr [rbp + 152], eax
	mov	qword ptr [rbp + 160], rcx
	lea	rax, [rbp + 192]
	mov	qword ptr [rsp + 32], rax
	mov	rcx, qword ptr [rbp - 56]
	lea	rdx, [rbp + 152]
	mov	r8d, r13d
	vzeroupper
	call	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	inc	r13d
	cmp	r13d, dword ptr [rbp - 32]
	jl	.LBB30_19
.LBB30_26:
	mov	dword ptr [rbp + 152], 0
	mov	qword ptr [rbp + 160], 0
	jmp	.LBB30_16
.LBB30_13:
	mov	rax, qword ptr [rbp + 168]
	mov	rdi, qword ptr [rax]
	mov	dword ptr [rbp + 136], 0
	mov	rax, qword ptr [rbp + 176]
	mov	qword ptr [rbp + 144], rax
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp - 80], xmm0
	movsxd	r15, dword ptr [rbp + 212]
	cmp	r15d, dword ptr [rbp + 216]
	jge	.LBB30_16
	lea	r12, [rbx + 160]
	lea	rsi, [rbp - 80]
	lea	r13, [rbp + 136]
	.p2align	4
.LBB30_15:
	mov	eax, dword ptr [rdi + 4*r15]
	mov	dword ptr [rbp + 136], eax
	mov	qword ptr [rsp + 32], rsi
	mov	rcx, r12
	mov	rdx, r13
	mov	r8d, r15d
	xor	r9d, r9d
	vzeroupper
	call	"Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.__ExecuteChunk(Latios.Mecanim.ApplyMecanimPropertiesSystem+ApplyBlendShapesJob*, Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Mecanim, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	inc	r15
	movsxd	rax, dword ptr [rbp + 216]
	cmp	r15, rax
	jl	.LBB30_15
.LBB30_16:
	cmp	dword ptr [rbp + 208], 1
	jne	.LBB30_17
	mov	dword ptr [rbp + 212], 0
	mov	rax, qword ptr [rbp + 168]
	mov	eax, dword ptr [rax + 8]
	mov	dword ptr [rbp + 216], eax
.LBB30_11:
	lea	r8, [rbp + 212]
	lea	r9, [rbp + 216]
	mov	rcx, qword ptr [rbp - 64]
	mov	edx, dword ptr [rbp + 188]
	vzeroupper
	call	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	test	al, al
	je	.LBB30_17
	mov	eax, dword ptr [rbx + 892]
	jmp	.LBB30_7
.LBB30_17:
	vmovaps	xmm6, xmmword ptr [rbp + 224]
	.seh_startepilogue
	add	rsp, 376
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.def	"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@00010001000100010001000100010001
	.section	.rdata,"dr",discard,__xmm@00010001000100010001000100010001
	.p2align	4, 0x0
__xmm@00010001000100010001000100010001:
	.quad	281479271743489
	.quad	281479271743489
	.section	.text,"xr",discard,"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 168
	.seh_stackalloc 168
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovdqa	xmmword ptr [rbp + 16], xmm13
	.seh_savexmm xmm13, 144
	vmovdqa	xmmword ptr [rbp], xmm12
	.seh_savexmm xmm12, 128
	vmovdqa	xmmword ptr [rbp - 16], xmm11
	.seh_savexmm xmm11, 112
	vmovdqa	xmmword ptr [rbp - 32], xmm10
	.seh_savexmm xmm10, 96
	vmovdqa	xmmword ptr [rbp - 48], xmm9
	.seh_savexmm xmm9, 80
	vmovdqa	xmmword ptr [rbp - 64], xmm8
	.seh_savexmm xmm8, 64
	vmovdqa	xmmword ptr [rbp - 80], xmm7
	.seh_savexmm xmm7, 48
	vmovdqa	xmmword ptr [rbp - 96], xmm6
	.seh_savexmm xmm6, 32
	.seh_endprologue
	mov	rsi, qword ptr [rcx]
	mov	r10, qword ptr [rsi]
	mov	r9d, dword ptr [rsi + 8]
	mov	eax, dword ptr [r8 + 4]
	test	eax, eax
	movsxd	r11, edx
	jle	.LBB31_5
	movsx	edx, word ptr [rsi + 254]
	lea	edi, [4*r9]
	movsxd	rdi, edi
	mov	esi, dword ptr [rsi + 20]
	imul	esi, edi
	movsxd	rsi, esi
	add	rsi, r10
	lea	rsi, [rsi + 2*rdi]
	lea	rsi, [rsi + 4*r11]
	xor	edi, edi
	.p2align	4
.LBB31_2:
	movsxd	rbx, dword ptr [r8 + 4*rdi + 8]
	mov	ebx, dword ptr [rcx + 4*rbx + 28]
	sub	ebx, edx
	imul	ebx, r9d
	movsxd	rbx, ebx
	mov	ebx, dword ptr [rsi + 4*rbx]
	cmp	ebx, dword ptr [r8 + 4*rdi + 16]
	jne	.LBB31_3
	inc	rdi
	cmp	edi, eax
	jl	.LBB31_2
.LBB31_5:
	mov	edx, dword ptr [r8 + 24]
	mov	eax, dword ptr [r8 + 36]
	test	rdx, rdx
	je	.LBB31_8
	test	eax, eax
	jne	.LBB31_10
	xor	eax, eax
	test	edx, edx
	jg	.LBB31_13
.LBB31_12:
	xor	ebx, ebx
	jmp	.LBB31_31
.LBB31_3:
	xor	eax, eax
	jmp	.LBB31_32
.LBB31_8:
	test	eax, eax
	je	.LBB31_9
.LBB31_10:
	lea	eax, [4*r9]
	cdqe
	add	rax, r10
	mov	eax, dword ptr [rax + 4*r11]
	mov	esi, dword ptr [r8]
	test	esi, esi
	sete	bl
	sub	eax, esi
	test	eax, eax
	setg	al
	or	al, bl
	test	edx, edx
	jle	.LBB31_12
.LBB31_13:
	lea	esi, [4*r9]
	movsxd	rsi, esi
	add	rsi, r10
	mov	qword ptr [rbp - 104], r8
	mov	r15d, dword ptr [r8]
	lea	r11, [rsi + 4*r11]
	cmp	edx, 8
	jae	.LBB31_15
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB31_29
.LBB31_15:
	cmp	edx, 31
	ja	.LBB31_17
	vmovd	xmm0, r15d
	vpbroadcastd	ymm0, xmm0
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB31_24
.LBB31_9:
	mov	al, 1
	jmp	.LBB31_32
.LBB31_17:
	mov	r10d, eax
	mov	rax, rdx
	mov	ebx, edx
	and	ebx, 2147483616
	vmovd	xmm0, r15d
	vpbroadcastd	ymm0, xmm0
	mov	rdx, qword ptr [rbp - 104]
	lea	rdi, [rdx + 152]
	vpxor	xmm1, xmm1, xmm1
	vpxor	xmm2, xmm2, xmm2
	mov	r8, rbx
	vpxor	xmm3, xmm3, xmm3
	vpxor	xmm4, xmm4, xmm4
	vpxor	xmm5, xmm5, xmm5
	jmp	.LBB31_18
	.p2align	4
.LBB31_20:
	vpor	xmm5, xmm9, xmm5
	vpor	xmm4, xmm8, xmm4
	vpor	xmm3, xmm3, xmm7
	vpor	xmm1, xmm1, xmm6
	sub	rdi, -128
	add	rbx, -32
	je	.LBB31_21
.LBB31_18:
	test	r15d, r15d
	vpcmpeqd	xmm6, xmm6, xmm6
	vpcmpeqd	xmm7, xmm7, xmm7
	vpcmpeqd	xmm8, xmm8, xmm8
	vpcmpeqd	xmm9, xmm9, xmm9
	je	.LBB31_20
	movsxd	r14, dword ptr [rdi - 108]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm6, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 104]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm6, xmm6, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 100]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm6, xmm6, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 96]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm6, xmm6, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 124]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm7, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 120]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm7, xmm7, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 116]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm7, xmm7, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 112]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm7, xmm7, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 76]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm8, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 72]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm8, xmm8, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 68]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm8, xmm8, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 64]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm8, xmm8, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 92]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm9, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 88]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm9, xmm9, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 84]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm9, xmm9, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 80]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm9, xmm9, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 44]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm10, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 40]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm10, xmm10, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 36]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm10, xmm10, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 32]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm10, xmm10, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 60]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm11, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 56]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm11, xmm11, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 52]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm11, xmm11, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 48]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm11, xmm11, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 12]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm12, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 8]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm12, xmm12, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 4]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm12, xmm12, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm12, xmm12, dword ptr [r11 + 4*r14], 3
	movsxd	r14, dword ptr [rdi - 28]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vmovd	xmm13, dword ptr [r11 + 4*r14]
	movsxd	r14, dword ptr [rdi - 24]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm13, xmm13, dword ptr [r11 + 4*r14], 1
	movsxd	r14, dword ptr [rdi - 20]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm13, xmm13, dword ptr [r11 + 4*r14], 2
	movsxd	r14, dword ptr [rdi - 16]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	movsxd	r14, r14d
	vpinsrd	xmm13, xmm13, dword ptr [r11 + 4*r14], 3
	vinserti128	ymm6, ymm7, xmm6, 1
	vinserti128	ymm7, ymm9, xmm8, 1
	vinserti128	ymm8, ymm11, xmm10, 1
	vinserti128	ymm9, ymm13, xmm12, 1
	vpsubd	ymm6, ymm6, ymm0
	vpsubd	ymm7, ymm7, ymm0
	vpsubd	ymm8, ymm8, ymm0
	vpsubd	ymm9, ymm9, ymm0
	vpcmpgtd	ymm6, ymm6, ymm2
	vextracti128	xmm10, ymm6, 1
	vpackssdw	xmm6, xmm6, xmm10
	vpcmpgtd	ymm7, ymm7, ymm2
	vextracti128	xmm10, ymm7, 1
	vpackssdw	xmm7, xmm7, xmm10
	vpcmpgtd	ymm8, ymm8, ymm2
	vextracti128	xmm10, ymm8, 1
	vpackssdw	xmm8, xmm8, xmm10
	vpcmpgtd	ymm9, ymm9, ymm2
	vextracti128	xmm10, ymm9, 1
	vpackssdw	xmm9, xmm9, xmm10
	jmp	.LBB31_20
.LBB31_21:
	vpor	xmm1, xmm3, xmm1
	vpor	xmm1, xmm4, xmm1
	vpor	xmm1, xmm5, xmm1
	vpsllw	xmm1, xmm1, 15
	vpmovmskb	edi, xmm1
	test	edi, 43690
	setne	bl
	mov	rdx, rax
	mov	rdi, r8
	cmp	edi, edx
	jne	.LBB31_23
	mov	eax, r10d
	jmp	.LBB31_31
.LBB31_23:
	test	dl, 24
	mov	eax, r10d
	je	.LBB31_29
.LBB31_24:
	mov	dword ptr [rbp - 108], eax
	mov	qword ptr [rbp - 128], rdx
	and	edx, 2147483640
	movzx	ebx, bl
	vmovd	xmm1, ebx
	mov	rax, qword ptr [rbp - 104]
	lea	rbx, [rax + 4*rdi + 56]
	mov	qword ptr [rbp - 120], rdx
	sub	rdi, rdx
	vpxor	xmm2, xmm2, xmm2
	jmp	.LBB31_25
	.p2align	4
.LBB31_27:
	vpor	xmm1, xmm1, xmm3
	add	rbx, 32
	add	rdi, 8
	je	.LBB31_28
.LBB31_25:
	test	r15d, r15d
	vpcmpeqd	xmm3, xmm3, xmm3
	je	.LBB31_27
	movsxd	r14, dword ptr [rbx - 28]
	mov	r8d, r15d
	movsxd	r15, dword ptr [rbx - 24]
	movsxd	r12, dword ptr [rbx - 20]
	movsxd	r13, dword ptr [rbx - 16]
	movsxd	rsi, dword ptr [rbx - 12]
	movsxd	r10, dword ptr [rbx - 8]
	movsxd	rax, dword ptr [rbx - 4]
	movsxd	rdx, dword ptr [rbx]
	mov	r14d, dword ptr [rcx + 4*r14 + 28]
	imul	r14d, r9d
	mov	r15d, dword ptr [rcx + 4*r15 + 28]
	imul	r15d, r9d
	mov	r12d, dword ptr [rcx + 4*r12 + 28]
	imul	r12d, r9d
	mov	r13d, dword ptr [rcx + 4*r13 + 28]
	imul	r13d, r9d
	mov	esi, dword ptr [rcx + 4*rsi + 28]
	imul	esi, r9d
	mov	r10d, dword ptr [rcx + 4*r10 + 28]
	imul	r10d, r9d
	mov	eax, dword ptr [rcx + 4*rax + 28]
	imul	eax, r9d
	mov	edx, dword ptr [rcx + 4*rdx + 28]
	imul	edx, r9d
	movsxd	r14, r14d
	movsxd	r15, r15d
	movsxd	r12, r12d
	movsxd	r13, r13d
	movsxd	rsi, esi
	movsxd	r10, r10d
	cdqe
	movsxd	rdx, edx
	vmovd	xmm3, dword ptr [r11 + 4*rsi]
	vpinsrd	xmm3, xmm3, dword ptr [r11 + 4*r10], 1
	vpinsrd	xmm3, xmm3, dword ptr [r11 + 4*rax], 2
	vpinsrd	xmm3, xmm3, dword ptr [r11 + 4*rdx], 3
	vmovd	xmm4, dword ptr [r11 + 4*r14]
	vpinsrd	xmm4, xmm4, dword ptr [r11 + 4*r15], 1
	mov	r15d, r8d
	vpinsrd	xmm4, xmm4, dword ptr [r11 + 4*r12], 2
	vpinsrd	xmm4, xmm4, dword ptr [r11 + 4*r13], 3
	vinserti128	ymm3, ymm4, xmm3, 1
	vpsubd	ymm3, ymm3, ymm0
	vpcmpgtd	ymm3, ymm3, ymm2
	vextracti128	xmm4, ymm3, 1
	vpackssdw	xmm3, xmm3, xmm4
	jmp	.LBB31_27
.LBB31_28:
	vptest	xmm1, xmmword ptr [rip + __xmm@00010001000100010001000100010001]
	setne	bl
	mov	rdx, qword ptr [rbp - 128]
	mov	rdi, qword ptr [rbp - 120]
	cmp	edi, edx
	mov	eax, dword ptr [rbp - 108]
	je	.LBB31_31
.LBB31_29:
	sub	rdx, rdi
	mov	r8, qword ptr [rbp - 104]
	lea	r8, [r8 + 4*rdi + 28]
	mov	esi, ebx
	.p2align	4
.LBB31_30:
	test	r15d, r15d
	sete	bl
	movsxd	r10, dword ptr [r8]
	mov	r10d, dword ptr [rcx + 4*r10 + 28]
	imul	r10d, r9d
	movsxd	r10, r10d
	mov	r10d, dword ptr [r11 + 4*r10]
	sub	r10d, r15d
	test	r10d, r10d
	setg	r10b
	or	bl, sil
	or	bl, r10b
	add	r8, 4
	dec	rdx
	mov	esi, ebx
	jne	.LBB31_30
.LBB31_31:
	or	al, bl
.LBB31_32:
	vmovaps	xmm6, xmmword ptr [rbp - 96]
	vmovaps	xmm7, xmmword ptr [rbp - 80]
	vmovaps	xmm8, xmmword ptr [rbp - 64]
	vmovaps	xmm9, xmmword ptr [rbp - 48]
	vmovaps	xmm10, xmmword ptr [rbp - 32]
	vmovaps	xmm11, xmmword ptr [rbp - 16]
	vmovaps	xmm12, xmmword ptr [rbp]
	vmovaps	xmm13, xmmword ptr [rbp + 16]
	.seh_startepilogue
	add	rsp, 168
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.def	burst.memcpy.inline.AVX2.i64@@32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memcpy.inline.AVX2.i64@@32
	.globl	burst.memcpy.inline.AVX2.i64@@32
	.p2align	4
burst.memcpy.inline.AVX2.i64@@32:
	cmp	r8, 32
	jb	.LBB32_1
	xor	r9d, r9d
	.p2align	4
.LBB32_3:
	lea	rax, [r9 + 32]
	vmovups	ymm0, ymmword ptr [rdx + r9]
	vmovups	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB32_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB32_6
	.p2align	4
.LBB32_5:
	lea	r9, [rax + 1]
	movzx	r10d, byte ptr [rdx + rax]
	mov	byte ptr [rcx + rax], r10b
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB32_5
.LBB32_6:
	vzeroupper
	ret
.LBB32_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB32_5
	jmp	.LBB32_6

	.def	burst.memset.inline.AVX2.i32@@32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memset.inline.AVX2.i32@@32
	.globl	burst.memset.inline.AVX2.i32@@32
	.p2align	4
burst.memset.inline.AVX2.i32@@32:
	xor	eax, eax
	cmp	r8d, 32
	jb	.LBB33_3
	xor	eax, eax
	vmovd	xmm0, edx
	vpbroadcastb	ymm0, xmm0
	.p2align	4
.LBB33_2:
	movsxd	r9, eax
	add	eax, 32
	vmovdqu	ymmword ptr [rcx + r9], ymm0
	add	r9d, 64
	cmp	r8d, r9d
	jae	.LBB33_2
.LBB33_3:
	lea	r9d, [rax + 1]
	cmp	r8d, r9d
	jb	.LBB33_5
	.p2align	4
.LBB33_4:
	movsxd	r9, eax
	inc	eax
	mov	byte ptr [rcx + r9], dl
	add	r9d, 2
	cmp	r8d, r9d
	jae	.LBB33_4
.LBB33_5:
	vzeroupper
	ret

	.def	burst.memset.inline.AVX2.i64@@32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memset.inline.AVX2.i64@@32
	.globl	burst.memset.inline.AVX2.i64@@32
	.p2align	4
burst.memset.inline.AVX2.i64@@32:
	cmp	r8, 32
	jb	.LBB34_1
	xor	r9d, r9d
	vmovd	xmm0, edx
	vpbroadcastb	ymm0, xmm0
	.p2align	4
.LBB34_3:
	lea	rax, [r9 + 32]
	vmovdqu	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB34_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB34_6
	.p2align	4
.LBB34_5:
	lea	r9, [rax + 1]
	mov	byte ptr [rcx + rax], dl
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB34_5
.LBB34_6:
	vzeroupper
	ret
.LBB34_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB34_5
	jmp	.LBB34_6

	.section	.drectve,"yni"
	.ascii	" /EXPORT:b639652c4fdbfa25bf83f596085599e0"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.b639652c4fdbfa25bf83f596085599e0\""
	.ascii	" /EXPORT:\"burst.initialize.statics.b639652c4fdbfa25bf83f596085599e0\""
	.globl	_fltused
