
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	36ef535b9300e20e05c4578acab7face;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	36ef535b9300e20e05c4578acab7face
	.p2align	4
36ef535b9300e20e05c4578acab7face:
.seh_proc 36ef535b9300e20e05c4578acab7face
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
	jmp	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
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
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.9204b6fd72e718fc5e454594aaae91f2]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::NewStaticSafetyId_Ptr"], rax
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
	jmp	burst.initialize.statics.36ef535b9300e20e05c4578acab7face
	.seh_endproc

	.def	burst.initialize.externals.36ef535b9300e20e05c4578acab7face;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.36ef535b9300e20e05c4578acab7face
	.p2align	4
burst.initialize.externals.36ef535b9300e20e05c4578acab7face:
.seh_proc burst.initialize.externals.36ef535b9300e20e05c4578acab7face
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
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.9204b6fd72e718fc5e454594aaae91f2]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::NewStaticSafetyId_Ptr"], rax
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

	.def	burst.initialize.statics.36ef535b9300e20e05c4578acab7face;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.36ef535b9300e20e05c4578acab7face
	.p2align	4
burst.initialize.statics.36ef535b9300e20e05c4578acab7face:
.seh_proc burst.initialize.statics.36ef535b9300e20e05c4578acab7face
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
	movabs	rax, -2301534937205546864
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	mov	rsi, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"], rax
	movabs	rdi, -7082256032705860890
	mov	qword ptr [rbp - 16], rdi
	movabs	rax, -8559585149946538604
	mov	qword ptr [rbp - 8], rax
	lea	rcx, [rbp - 16]
	mov	edx, 16
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle> Unity.Entities.TypeManager/SharedSafetyHandle::Ref"], rax
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
	mov	qword ptr [rbp - 16], rdi
	movabs	rax, -7370724514926575087
	mov	qword ptr [rbp - 8], rax
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"], rax
	movabs	rax, 6336067141081051292
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 4
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Collections.NativeText/ReadOnly::s_staticSafetyId"], rax
	mov	qword ptr [rbp - 16], rdi
	movabs	rax, -8796907678697770368
	mov	qword ptr [rbp - 8], rax
	lea	rcx, [rbp - 16]
	mov	edx, 8
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeNames::Ref"], rax
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

	.def	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
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

	.def	"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
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
	sub	rsp, 216
	.seh_stackalloc 216
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	rax, qword ptr [rcx + 128]
	mov	r14, qword ptr [rax]
	test	r14, r14
	je	.LBB5_17
	mov	rdi, rdx
	mov	rbx, rcx
	lea	rdx, [rcx + 64]
	mov	rcx, rdi
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rsi, rax
	movsxd	rax, dword ptr [rdi]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	shl	rax, 4
	movsxd	rax, dword ptr [rcx + rax + 8]
	test	rax, rax
	jle	.LBB5_2
	xor	ecx, ecx
	xor	r12d, r12d
	xor	r15d, r15d
	jmp	.LBB5_19
	.p2align	4
.LBB5_5:
	bts	r12, rcx
.LBB5_6:
	inc	rcx
	cmp	rax, rcx
	je	.LBB5_7
.LBB5_19:
	mov	edx, dword ptr [rsi + 4*rcx]
	mov	r8d, edx
	sar	r8d, 6
	movsxd	r8, r8d
	mov	r8, qword ptr [r14 + 8*r8]
	bt	r8, rdx
	jae	.LBB5_6
	cmp	rcx, 64
	jb	.LBB5_5
	bts	r15, rcx
	jmp	.LBB5_6
.LBB5_2:
	xor	r15d, r15d
	xor	r12d, r12d
.LBB5_7:
	mov	rax, r12
	or	rax, r15
	je	.LBB5_17
	mov	eax, dword ptr [rbx + 288]
	test	eax, eax
	jne	.LBB5_11
	mov	ecx, dword ptr [rbx + 184]
	test	rcx, rcx
	je	.LBB5_10
	mov	rdx, qword ptr [rbx + 176]
	xor	eax, eax
	.p2align	4
.LBB5_22:
	cmp	qword ptr [rdx], 0
	jne	.LBB5_23
	inc	rax
	add	rdx, 24
	cmp	rax, rcx
	jb	.LBB5_22
.LBB5_10:
	xor	eax, eax
	jmp	.LBB5_11
.LBB5_23:
	dec	eax
	mov	dword ptr [rbx + 288], eax
.LBB5_11:
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 48], ymm0
	vmovups	ymmword ptr [rbp - 16], ymm0
	vmovups	ymmword ptr [rbp + 16], ymm0
	vmovups	ymmword ptr [rbp + 48], ymm0
	mov	qword ptr [rbp + 80], 0
	inc	eax
	mov	rcx, qword ptr [rbx + 176]
	cdqe
	lea	rax, [rax + 2*rax]
	mov	r13, qword ptr [rcx + 8*rax]
	lea	rdx, [rbx + 224]
	mov	rcx, rdi
	vzeroupper
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	r14, rax
	lea	rdx, [rbp - 96]
	mov	rcx, rdi
	mov	r8, rbx
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	movsxd	rax, dword ptr [rdi]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	shl	rax, 4
	mov	rdi, qword ptr [rbp - 96]
	vmovq	xmm0, r15
	vmovq	xmm1, r12
	vpunpcklqdq	xmm2, xmm1, xmm0
	mov	r9d, dword ptr [rcx + rax + 8]
	lea	rcx, [rbp - 48]
	mov	dl, 1
	call	"Latios.ChunkEntityBatchEnumerator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null..ctor(Latios.ChunkEntityBatchEnumerator*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkEntityCount) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	eax, dword ptr [rbp + 80]
	mov	ecx, dword ptr [rbp + 84]
	inc	eax
	mov	dword ptr [rbp + 80], eax
	cmp	eax, ecx
	jge	.LBB5_17
	add	r14, 16
	add	rdi, 16
	jmp	.LBB5_13
	.p2align	4
.LBB5_16:
	inc	eax
	mov	dword ptr [rbp + 80], eax
	cmp	eax, ecx
	jge	.LBB5_17
.LBB5_13:
	movsxd	r8, eax
	movzx	edx, byte ptr [rbp + r8 + 16]
	test	rdx, rdx
	je	.LBB5_16
	movzx	r10d, byte ptr [rbp + r8 - 48]
	shl	edx, 2
	lea	rdx, [rdx + 2*rdx]
	lea	r8, [r10 + 2*r10]
	lea	r9, [r14 + 8*r8]
	lea	r10, [rsi + 4*r10]
	shl	r8d, 4
	add	r8, rdi
	xor	r11d, r11d
	.p2align	4
.LBB5_15:
	vmovsd	xmm0, qword ptr [r8 + 4*r11]
	vinsertps	xmm0, xmm0, dword ptr [r8 + 4*r11 + 8], 32
	mov	ebx, dword ptr [r10]
	lea	rbx, [rbx + 2*rbx]
	vmovsd	xmm1, qword ptr [r13 + 8*rbx + 4]
	vmovsd	xmm2, qword ptr [r13 + 8*rbx + 16]
	vmovss	xmm3, dword ptr [r13 + 8*rbx]
	vmovlhps	xmm3, xmm3, xmm1
	vshufps	xmm1, xmm3, xmm1, 216
	vsubps	xmm1, xmm1, xmm0
	vmovss	xmm3, dword ptr [r13 + 8*rbx + 12]
	vmovlhps	xmm3, xmm3, xmm2
	vshufps	xmm2, xmm3, xmm2, 216
	vsubps	xmm0, xmm2, xmm0
	vinsertps	xmm1, xmm1, xmm0, 48
	vmovups	xmmword ptr [r9 + 2*r11 - 16], xmm1
	vshufps	xmm0, xmm0, xmm0, 233
	vmovlps	qword ptr [r9 + 2*r11], xmm0
	add	r11, 12
	add	r10, 4
	cmp	rdx, r11
	jne	.LBB5_15
	jmp	.LBB5_16
.LBB5_17:
	.seh_startepilogue
	add	rsp, 216
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
.LBB6_1:
	cqo
	idiv	r12
	inc	r13d
	test	rax, rax
	jne	.LBB6_1
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
.LBB6_3:
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
	jne	.LBB6_3
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
.LBB7_1:
	mov	rax, rcx
	xor	edx, edx
	div	r12
	inc	r13d
	cmp	rcx, r12
	mov	rcx, rax
	jae	.LBB7_1
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
.LBB7_3:
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
	jae	.LBB7_3
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
	jne	.LBB8_1
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
.LBB8_1:
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
	je	.LBB9_1
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB9_23
	lea	ebx, [rdi + 1]
	mov	dword ptr [rdx], ebx
	mov	byte ptr [rcx + rdi], 45
.LBB9_1:
	test	esi, esi
	jle	.LBB9_14
	inc	esi
	mov	r11d, 48
	.p2align	4
.LBB9_3:
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB9_23
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
	jg	.LBB9_3
	xor	r11d, r11d
	test	r11d, r11d
	jns	.LBB9_7
	jmp	.LBB9_8
.LBB9_14:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB9_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	test	r11d, r11d
	js	.LBB9_8
.LBB9_7:
	cmp	byte ptr [r10], 0
	jne	.LBB9_8
.LBB9_20:
	test	al, al
	jne	.LBB9_23
	movsxd	rax, dword ptr [rdx]
	cmp	eax, r8d
	jge	.LBB9_23
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
	jmp	.LBB9_23
.LBB9_8:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB9_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 46
	test	r11d, r11d
	js	.LBB9_17
.LBB9_10:
	cmp	byte ptr [r10], 0
	je	.LBB9_20
	inc	r10
	.p2align	4
.LBB9_19:
	movsxd	r11, dword ptr [rdx]
	cmp	r11d, r8d
	jge	.LBB9_23
	lea	esi, [r11 + 1]
	mov	dword ptr [rdx], esi
	movzx	ebx, byte ptr [r10 - 1]
	mov	byte ptr [rcx + r11], bl
	cmp	byte ptr [r10], 0
	lea	r10, [r10 + 1]
	jne	.LBB9_19
	jmp	.LBB9_20
	.p2align	4
.LBB9_16:
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	inc	r11d
	je	.LBB9_10
.LBB9_17:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jl	.LBB9_16
.LBB9_23:
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
	jne	.LBB10_4
	movzx	r11d, dl
	add	r11d, -4
	cmp	r11d, -4
	mov	r11d, edx
	ja	.LBB10_4
	mov	edi, dword ptr [rbp + 112]
	test	edi, edi
	jle	.LBB10_26
	mov	rax, qword ptr [r9]
	jmp	.LBB10_27
.LBB10_4:
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
	jle	.LBB10_9
	mov	edi, edx
	sub	edi, eax
	jle	.LBB10_9
	.p2align	4
.LBB10_6:
	movsxd	rbx, dword ptr [rsi]
	cmp	ebx, r8d
	jge	.LBB10_72
	lea	r14d, [rbx + 1]
	mov	dword ptr [rsi], r14d
	mov	byte ptr [rcx + rbx], 32
	dec	edi
	jne	.LBB10_6
	movzx	ebx, byte ptr [r9 + 20]
.LBB10_9:
	test	bl, bl
	je	.LBB10_11
	mov	r11d, dword ptr [rsi]
	mov	bl, 45
	cmp	r11d, r8d
	jl	.LBB10_13
	jmp	.LBB10_21
.LBB10_11:
	cmp	r11b, 2
	jne	.LBB10_14
	mov	r11d, dword ptr [rsi]
	mov	bl, 43
	cmp	r11d, r8d
	jge	.LBB10_21
.LBB10_13:
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movsxd	r11, r11d
	mov	byte ptr [rcx + r11], bl
.LBB10_14:
	test	r10d, r10d
	jle	.LBB10_17
	.p2align	4
.LBB10_15:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB10_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	mov	byte ptr [rcx + r11], 48
	dec	r10d
	jne	.LBB10_15
.LBB10_17:
	movsxd	r10, dword ptr [r9 + 12]
	test	r10, r10
	jle	.LBB10_21
	mov	r9, qword ptr [r9]
	.p2align	4
.LBB10_19:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB10_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movzx	ebx, byte ptr [r9]
	mov	byte ptr [rcx + r11], bl
	inc	r9
	dec	r10
	jne	.LBB10_19
.LBB10_21:
	test	r15b, r15b
	jns	.LBB10_72
	mov	r9d, edx
	neg	r9d
	cmp	eax, r9d
	jge	.LBB10_72
	add	eax, edx
	neg	eax
	.p2align	4
.LBB10_24:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB10_72
	lea	r9d, [rdx + 1]
	mov	dword ptr [rsi], r9d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB10_24
	jmp	.LBB10_72
.LBB10_26:
	mov	edi, dword ptr [r9 + 12]
	mov	rax, qword ptr [r9]
	test	edi, edi
	jle	.LBB10_38
.LBB10_27:
	mov	edx, edi
	xor	r11d, r11d
	.p2align	4
.LBB10_28:
	cmp	byte ptr [rax + r11], 0
	je	.LBB10_39
	inc	r11
	cmp	rdx, r11
	jne	.LBB10_28
	mov	edi, edx
	cmp	r10d, 1
	jne	.LBB10_31
	jmp	.LBB10_41
.LBB10_38:
	xor	r11d, r11d
.LBB10_39:
	cmp	r11d, edi
	jne	.LBB10_42
	mov	edx, edi
	cmp	r10d, 1
	je	.LBB10_41
.LBB10_31:
	cmp	byte ptr [rax + rdx], 53
	jb	.LBB10_41
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB10_33:
	test	edx, edx
	jle	.LBB10_36
	movzx	r11d, byte ptr [rax + rdx - 1]
	dec	rdx
	dec	r10d
	cmp	r11b, 57
	je	.LBB10_33
	inc	r11b
	mov	byte ptr [rax + rdx], r11b
	jmp	.LBB10_49
.LBB10_41:
	mov	r11d, edi
	jmp	.LBB10_43
.LBB10_42:
	mov	edx, r11d
.LBB10_43:
	mov	ebx, r11d
	sar	ebx, 31
	and	ebx, r11d
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB10_44:
	test	edx, edx
	jle	.LBB10_46
	dec	r10d
	cmp	byte ptr [rax + rdx - 1], 48
	lea	rdx, [rdx - 1]
	je	.LBB10_44
	jmp	.LBB10_49
.LBB10_46:
	test	rdx, rdx
	je	.LBB10_48
	mov	r10d, ebx
	jmp	.LBB10_49
.LBB10_36:
	inc	dword ptr [r9 + 16]
	mov	byte ptr [rax], 49
	mov	r10d, 1
	jmp	.LBB10_49
.LBB10_48:
	mov	dword ptr [r9 + 16], 0
	xor	r10d, r10d
.LBB10_49:
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
	jle	.LBB10_53
	mov	r10b, r13b
	lea	r11d, [rdx + 1]
	.p2align	4
.LBB10_51:
	cmp	byte ptr [rax], 1
	sbb	rax, -1
	dec	r11d
	cmp	r11d, 1
	ja	.LBB10_51
	add	edx, r10d
	xor	r13d, r13d
	mov	r15d, edx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	jns	.LBB10_54
	jmp	.LBB10_57
.LBB10_53:
	cmp	r11b, 1
	sbb	r15d, -1
	mov	r13d, ebx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	js	.LBB10_57
.LBB10_54:
	test	dl, dl
	jne	.LBB10_57
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	je	.LBB10_61
.LBB10_56:
	movsx	r12d, r14b
	test	r14b, r14b
	jg	.LBB10_64
	jmp	.LBB10_67
.LBB10_57:
	mov	r10d, r13d
	sar	r10d, 31
	andn	r10d, r10d, r13d
	test	dl, dl
	je	.LBB10_59
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
	jne	.LBB10_56
	jmp	.LBB10_61
.LBB10_59:
	lea	r15d, [r15 + r10 + 1]
	sub	r15d, r13d
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	jne	.LBB10_56
.LBB10_61:
	dec	ebx
	movsxd	r11, ebx
	xor	ebx, ebx
	movabs	r12, 7378697629483820647
	mov	r10, r11
	.p2align	4
.LBB10_62:
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
	ja	.LBB10_62
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
	jle	.LBB10_67
.LBB10_64:
	mov	eax, r12d
	sub	eax, r15d
	jle	.LBB10_67
	.p2align	4
.LBB10_65:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB10_72
	lea	r10d, [rdx + 1]
	mov	dword ptr [rsi], r10d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB10_65
.LBB10_67:
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
	jns	.LBB10_72
	mov	eax, r12d
	neg	eax
	cmp	r15d, eax
	jge	.LBB10_72
	add	r15d, r12d
	neg	r15d
	.p2align	4
.LBB10_70:
	movsxd	rax, dword ptr [rsi]
	cmp	eax, ebx
	jge	.LBB10_72
	lea	ecx, [rax + 1]
	mov	dword ptr [rsi], ecx
	mov	byte ptr [rdi + rax], 32
	dec	r15d
	jne	.LBB10_70
.LBB10_72:
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

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetNativeArray<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> Unity.Collections.NativeArray`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	jne	.LBB11_17
	cmp	qword ptr [r8], r10
	jne	.LBB11_5
	movzx	r11d, word ptr [r8 + 14]
	cmp	r11w, -1
	jne	.LBB11_21
	jmp	.LBB11_4
.LBB11_17:
	cmp	qword ptr [r8], r10
	jne	.LBB11_25
	movzx	edi, word ptr [r8 + 14]
	cmp	di, -1
	je	.LBB11_4
.LBB11_20:
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
.LBB11_21:
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
	je	.LBB11_4
	movsxd	r9, dword ptr [rcx]
	mov	rdx, qword ptr [rdx]
	shl	r9, 4
	mov	r14d, dword ptr [rdx + r9 + 8]
	lea	ebx, [r14 - 1]
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	mov	rdx, qword ptr [rcx + 8]
	cmp	byte ptr [rdx + 494], 0
	jne	.LBB11_37
.LBB11_23:
	mov	qword ptr [rax], rdi
	mov	dword ptr [rax + 8], r14d
	mov	dword ptr [rax + 12], 0
	mov	dword ptr [rax + 16], ebx
	mov	ecx, 1
.LBB11_24:
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
.LBB11_37:
	cmp	byte ptr [r8 + 28], 1
	je	.LBB11_23
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
	jmp	.LBB11_23
.LBB11_5:
	mov	rdi, qword ptr [r10 + 112]
	mov	ebx, dword ptr [r10 + 168]
	movsx	r11d, word ptr [r8 + 14]
	test	r11d, r11d
	js	.LBB11_7
	cmp	ebx, r11d
	jle	.LBB11_7
	movsxd	r14, r11d
	cmp	dword ptr [rdi + 4*r14], esi
	je	.LBB11_15
.LBB11_7:
	test	ebx, ebx
	je	.LBB11_12
	xor	r11d, r11d
.LBB11_10:
	movsxd	r14, r11d
	cmp	esi, dword ptr [rdi + 4*r14]
	je	.LBB11_11
	inc	r11d
	cmp	ebx, r11d
	jne	.LBB11_10
.LBB11_12:
	mov	word ptr [r8 + 14], -1
.LBB11_13:
	mov	dword ptr [r8 + 8], 0
	mov	r11w, -1
	xor	esi, esi
	jmp	.LBB11_16
.LBB11_25:
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	edi, word ptr [r8 + 14]
	test	edi, edi
	js	.LBB11_27
	cmp	r14d, edi
	jle	.LBB11_27
	movsxd	r15, edi
	cmp	dword ptr [rbx + 4*r15], esi
	je	.LBB11_35
.LBB11_27:
	test	r14d, r14d
	je	.LBB11_32
	xor	edi, edi
.LBB11_30:
	movsxd	r15, edi
	cmp	esi, dword ptr [rbx + 4*r15]
	je	.LBB11_31
	inc	edi
	cmp	r14d, edi
	jne	.LBB11_30
.LBB11_32:
	mov	word ptr [r8 + 14], -1
.LBB11_33:
	mov	dword ptr [r8 + 8], 0
	mov	di, -1
	xor	esi, esi
	jmp	.LBB11_36
.LBB11_11:
	mov	word ptr [r8 + 14], r11w
	cmp	r11w, -1
	je	.LBB11_13
.LBB11_15:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rdi, r11w
	mov	esi, dword ptr [rsi + 4*rdi]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rdi]
.LBB11_16:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	r11w, -1
	jne	.LBB11_21
	jmp	.LBB11_4
.LBB11_31:
	mov	word ptr [r8 + 14], di
	cmp	di, -1
	je	.LBB11_33
.LBB11_35:
	mov	rsi, qword ptr [r10 + 224]
	movsx	rbx, di
	mov	esi, dword ptr [rsi + 4*rbx]
	mov	dword ptr [r8 + 8], esi
	mov	rsi, qword ptr [r10 + 232]
	movzx	esi, word ptr [rsi + 2*rbx]
.LBB11_36:
	mov	word ptr [r8 + 12], si
	mov	qword ptr [r8], r10
	cmp	di, -1
	jne	.LBB11_20
.LBB11_4:
	mov	rsi, qword ptr [r8 + 48]
	vmovsd	xmm6, qword ptr [r8 + 56]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rax], xmm0
	mov	dword ptr [rax + 16], -1
	xor	ecx, ecx
	jmp	.LBB11_24
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@656e6f706d6f63206465726975716552
	.section	.rdata,"dr",discard,__xmm@656e6f706d6f63206465726975716552
	.p2align	4, 0x0
__xmm@656e6f706d6f63206465726975716552:
	.byte	82
	.byte	101
	.byte	113
	.byte	117
	.byte	105
	.byte	114
	.byte	101
	.byte	100
	.byte	32
	.byte	99
	.byte	111
	.byte	109
	.byte	112
	.byte	111
	.byte	110
	.byte	101
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.ExposedSkeletonCullingIndex, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 472
	.seh_stackalloc 472
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	movsxd	rax, dword ptr [rcx]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	mov	r8, rax
	shl	r8, 4
	mov	r8, qword ptr [rcx + r8]
	mov	ecx, dword ptr [rdx + 16]
	cmp	qword ptr [rdx], r8
	jne	.LBB12_2
	mov	r10d, dword ptr [rdx + 8]
	movzx	r9d, word ptr [rdx + 14]
	cmp	r9w, -1
	je	.LBB12_15
.LBB12_20:
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	rdx, rax
	sar	rdx, 3
	and	rdx, -8
	and	eax, 63
	shl	eax, 14
	add	rax, qword ptr [rcx + rdx]
	movsxd	rcx, r10d
	lea	rax, [rcx + rax + 64]
	.seh_startepilogue
	add	rsp, 472
	pop	rbx
	pop	rdi
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
.LBB12_2:
	mov	r10, qword ptr [r8 + 112]
	mov	r11d, dword ptr [r8 + 168]
	movsx	r9d, word ptr [rdx + 14]
	test	r9d, r9d
	js	.LBB12_4
	cmp	r11d, r9d
	jle	.LBB12_4
	movsxd	rsi, r9d
	cmp	dword ptr [r10 + 4*rsi], ecx
	je	.LBB12_12
.LBB12_4:
	test	r11d, r11d
	je	.LBB12_9
	xor	r9d, r9d
.LBB12_7:
	movsxd	rsi, r9d
	cmp	ecx, dword ptr [r10 + 4*rsi]
	je	.LBB12_8
	inc	r9d
	cmp	r11d, r9d
	jne	.LBB12_7
.LBB12_9:
	mov	word ptr [rdx + 14], -1
.LBB12_10:
	mov	dword ptr [rdx + 8], 0
	xor	r10d, r10d
	mov	r9w, -1
	xor	r11d, r11d
	jmp	.LBB12_13
.LBB12_8:
	mov	word ptr [rdx + 14], r9w
	cmp	r9w, -1
	je	.LBB12_10
.LBB12_12:
	mov	r10, qword ptr [r8 + 224]
	movsx	r11, r9w
	mov	r10d, dword ptr [r10 + 4*r11]
	mov	dword ptr [rdx + 8], r10d
	mov	rsi, qword ptr [r8 + 232]
	movzx	r11d, word ptr [rsi + 2*r11]
.LBB12_13:
	mov	word ptr [rdx + 12], r11w
	mov	qword ptr [rdx], r8
	cmp	r9w, -1
	jne	.LBB12_20
.LBB12_15:
	mov	dword ptr [rbp + 340], ecx
	lea	rcx, [rbp + 340]
	lea	rdx, [rbp + 32]
	call	"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8"
	movzx	eax, word ptr [rbp + 32]
	vmovups	ymm0, ymmword ptr [rbp + 34]
	vmovups	ymmword ptr [rbp - 94], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 66]
	vmovups	ymmword ptr [rbp - 62], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 98]
	vmovups	ymmword ptr [rbp - 30], ymm0
	vmovups	xmm0, xmmword ptr [rbp + 130]
	vmovups	xmmword ptr [rbp + 2], xmm0
	mov	rcx, qword ptr [rbp + 146]
	mov	qword ptr [rbp + 18], rcx
	mov	ecx, dword ptr [rbp + 154]
	mov	dword ptr [rbp + 26], ecx
	movzx	ecx, word ptr [rbp + 158]
	mov	word ptr [rbp + 30], cx
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp + 160], ymm0
	vmovups	ymmword ptr [rbp + 192], ymm0
	vmovups	ymmword ptr [rbp + 224], ymm0
	vmovups	ymmword ptr [rbp + 256], ymm0
	vmovups	ymmword ptr [rbp + 288], ymm0
	mov	qword ptr [rbp + 320], 0
	mov	dword ptr [rbp + 328], 0
	lea	rdi, [rbp + 162]
	vmovaps	xmm0, xmmword ptr [rip + __xmm@656e6f706d6f63206465726975716552]
	vmovups	xmmword ptr [rbp + 162], xmm0
	mov	word ptr [rbp + 178], 29806
	mov	byte ptr [rbp + 180], 32
	test	ax, ax
	je	.LBB12_16
	movzx	ecx, ax
	cmp	ecx, 150
	mov	esi, 150
	cmovb	esi, eax
	lea	rcx, [rbp + 181]
	movzx	r8d, si
	lea	rdx, [rbp - 94]
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, 19
	movzx	ebx, si
	mov	esi, 169
	cmp	ebx, 169
	jne	.LBB12_18
	jmp	.LBB12_19
.LBB12_16:
	mov	ebx, 19
.LBB12_18:
	mov	eax, 169
	sub	eax, ebx
	cmp	eax, 24
	mov	esi, 24
	cmovb	esi, eax
	mov	ecx, ebx
	add	rcx, rdi
	lea	rdx, [rip + "Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex>(Unity.Entities.ArchetypeChunk* this, ref Unity.Entities.ComponentTypeHandle`1<Latios.Kinemation.ExposedSkeletonCullingIndex> typeHandle) -> void*_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0063.1"+2]
	mov	r8, rsi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, ebx
.LBB12_19:
	mov	eax, esi
	mov	byte ptr [rbp + rax + 162], 0
	lea	rdx, [rbp + 162]
	mov	word ptr [rdi - 2], si
	lea	rcx, [rip + burst.string.1750a84a9e73678fca2e711b84b9a69d]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.SkeletonWorldBoundsOffsetsFromPosition, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	sub	rsp, 488
	.seh_stackalloc 488
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	movsxd	rax, dword ptr [rcx]
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rdi, qword ptr [r8]
	mov	r11, rax
	shl	r11, 4
	mov	r10, qword ptr [rdi + r11]
	mov	r9d, dword ptr [rdx + 24]
	cmp	qword ptr [rdx], r10
	jne	.LBB13_2
	movzx	esi, word ptr [rdx + 14]
.LBB13_14:
	movsxd	r11, dword ptr [rdi + r11 + 12]
	movsx	esi, si
	mov	edi, dword ptr [r10 + 8]
	lea	ebx, [4*rdi]
	movsxd	rbx, ebx
	add	rbx, qword ptr [r10]
	imul	esi, edi
	movsxd	r10, esi
	lea	r10, [rbx + 4*r10]
	mov	dword ptr [r10 + 4*r11], r9d
	cmp	byte ptr [rdx + 28], 1
	je	.LBB13_24
	cmp	word ptr [rdx + 14], -1
	je	.LBB13_18
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
	jne	.LBB13_23
.LBB13_17:
	.seh_startepilogue
	add	rsp, 488
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
.LBB13_2:
	mov	edi, dword ptr [rdx + 16]
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	esi, word ptr [rdx + 14]
	test	esi, esi
	js	.LBB13_4
	cmp	r14d, esi
	jle	.LBB13_4
	movsxd	r15, esi
	cmp	dword ptr [rbx + 4*r15], edi
	je	.LBB13_12
.LBB13_4:
	test	r14d, r14d
	je	.LBB13_9
	xor	esi, esi
.LBB13_7:
	movsxd	r15, esi
	cmp	edi, dword ptr [rbx + 4*r15]
	je	.LBB13_8
	inc	esi
	cmp	r14d, esi
	jne	.LBB13_7
.LBB13_9:
	mov	word ptr [rdx + 14], -1
.LBB13_10:
	mov	dword ptr [rdx + 8], 0
	mov	si, -1
	xor	edi, edi
	jmp	.LBB13_13
.LBB13_24:
	lea	rcx, [rip + burst.string.1ea13c7f7ea144ea32205d7ec317f39f]
	lea	rdx, [rip + burst.string.989b224dc613a391593dc022965ef313]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB13_18:
	mov	eax, dword ptr [rdx + 16]
	mov	dword ptr [rbp + 356], eax
	lea	rcx, [rbp + 356]
	lea	rdx, [rbp + 48]
	call	"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8"
	movzx	eax, word ptr [rbp + 48]
	vmovups	ymm0, ymmword ptr [rbp + 50]
	vmovups	ymmword ptr [rbp - 78], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 82]
	vmovups	ymmword ptr [rbp - 46], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 114]
	vmovups	ymmword ptr [rbp - 14], ymm0
	vmovups	xmm0, xmmword ptr [rbp + 146]
	vmovups	xmmword ptr [rbp + 18], xmm0
	mov	rcx, qword ptr [rbp + 162]
	mov	qword ptr [rbp + 34], rcx
	mov	ecx, dword ptr [rbp + 170]
	mov	dword ptr [rbp + 42], ecx
	movzx	ecx, word ptr [rbp + 174]
	mov	word ptr [rbp + 46], cx
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp + 176], ymm0
	vmovups	ymmword ptr [rbp + 208], ymm0
	vmovups	ymmword ptr [rbp + 240], ymm0
	vmovups	ymmword ptr [rbp + 272], ymm0
	vmovups	ymmword ptr [rbp + 304], ymm0
	mov	qword ptr [rbp + 336], 0
	mov	dword ptr [rbp + 344], 0
	lea	rdi, [rbp + 178]
	vmovaps	xmm0, xmmword ptr [rip + __xmm@656e6f706d6f63206465726975716552]
	vmovups	xmmword ptr [rbp + 178], xmm0
	mov	word ptr [rbp + 194], 29806
	mov	byte ptr [rbp + 196], 32
	test	ax, ax
	je	.LBB13_19
	movzx	ecx, ax
	cmp	ecx, 150
	mov	esi, 150
	cmovb	esi, eax
	lea	rcx, [rbp + 197]
	movzx	r8d, si
	lea	rdx, [rbp - 78]
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, 19
	movzx	ebx, si
	mov	esi, 169
	cmp	ebx, 169
	jne	.LBB13_21
	jmp	.LBB13_22
.LBB13_23:
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
	jmp	.LBB13_17
.LBB13_19:
	mov	ebx, 19
.LBB13_21:
	mov	eax, 169
	sub	eax, ebx
	cmp	eax, 24
	mov	esi, 24
	cmovb	esi, eax
	mov	ecx, ebx
	add	rcx, rdi
	lea	rdx, [rip + "Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRO<Latios.Kinemation.ExposedSkeletonCullingIndex>(Unity.Entities.ArchetypeChunk* this, ref Unity.Entities.ComponentTypeHandle`1<Latios.Kinemation.ExposedSkeletonCullingIndex> typeHandle) -> void*_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0063.1"+2]
	mov	r8, rsi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, ebx
.LBB13_22:
	mov	eax, esi
	mov	byte ptr [rbp + rax + 178], 0
	lea	rdx, [rbp + 178]
	mov	word ptr [rdi - 2], si
	lea	rcx, [rip + burst.string.1750a84a9e73678fca2e711b84b9a69d]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB13_8:
	mov	word ptr [rdx + 14], si
	cmp	si, -1
	je	.LBB13_10
.LBB13_12:
	mov	rdi, qword ptr [r10 + 224]
	movsx	rbx, si
	mov	edi, dword ptr [rdi + 4*rbx]
	mov	dword ptr [rdx + 8], edi
	mov	rdi, qword ptr [r10 + 232]
	movzx	edi, word ptr [rdi + 2*rbx]
.LBB13_13:
	mov	word ptr [rdx + 12], di
	mov	qword ptr [rdx], r10
	mov	rdi, qword ptr [r8]
	jmp	.LBB13_14
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
	je	.LBB15_15
	mov	rbx, qword ptr [rdx + 72]
	test	rbx, rbx
	je	.LBB15_2
	cmp	qword ptr [rbx], r8
	je	.LBB15_13
.LBB15_2:
	mov	rbx, qword ptr [rdx + 64]
	test	rbx, rbx
	je	.LBB15_7
	.p2align	4
.LBB15_4:
	cmp	qword ptr [rbx], r8
	je	.LBB15_12
	mov	rbx, qword ptr [rbx + 16]
	test	rbx, rbx
	jne	.LBB15_4
.LBB15_7:
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
	je	.LBB15_8
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
	je	.LBB15_11
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
	jne	.LBB15_17
	jmp	.LBB15_15
.LBB15_12:
	mov	qword ptr [rdx + 72], rbx
.LBB15_13:
	add	rbx, 8
	mov	r10, qword ptr [rbx]
	test	r10, r10
	je	.LBB15_15
.LBB15_17:
	mov	ecx, dword ptr [r10 + 28]
	test	ecx, ecx
	je	.LBB15_15
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
	js	.LBB15_20
	cmp	edi, ecx
	jge	.LBB15_20
	xor	ebx, ebx
	cmp	edi, r11d
	cmovge	ebx, r10d
	add	edi, r8d
	sub	edi, ebx
	movsxd	rdi, edi
	shl	rdi, 2
	lea	rdi, [rdi + 4*rdi]
	cmp	dword ptr [rdx + rdi], r9d
	je	.LBB15_24
.LBB15_20:
	dec	ecx
	.p2align	4
.LBB15_21:
	test	ecx, ecx
	js	.LBB15_15
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
	jne	.LBB15_21
.LBB15_24:
	mov	ecx, dword ptr [rdx + rdi + 4]
	mov	r8d, dword ptr [rdx + rdi + 8]
	movzx	r9d, word ptr [rdx + rdi + 12]
	movzx	r10d, word ptr [rdx + rdi + 14]
	mov	edx, dword ptr [rdx + rdi + 16]
	jmp	.LBB15_16
.LBB15_15:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	jmp	.LBB15_16
.LBB15_11:
	mov	qword ptr [rsi + 8], 0
	mov	rcx, rsi
	mov	edx, 4
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB15_8:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	mov	rax, rdi
.LBB15_16:
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
	jne	.LBB16_13
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rax, qword ptr [rax]
	mov	ecx, edx
	cmp	edx, 9
	jae	.LBB16_3
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB16_11
.LBB16_3:
	cmp	edx, 33
	jae	.LBB16_5
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB16_8
.LBB16_5:
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
.LBB16_6:
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
	jne	.LBB16_6
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
	jb	.LBB16_11
.LBB16_8:
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
.LBB16_9:
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
	jne	.LBB16_9
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
.LBB16_11:
	sub	rcx, rdx
	shl	rdx, 4
	add	rdx, r13
	.p2align	4
.LBB16_12:
	movsxd	r8, dword ptr [rdx]
	shl	r8, 4
	add	r12d, dword ptr [rax + r8 + 8]
	add	rdx, 16
	dec	rcx
	jne	.LBB16_12
.LBB16_13:
	mov	edx, dword ptr [rbp + 288]
	mov	ecx, 1
	.p2align	4
.LBB16_14:
	xor	eax, eax
	lock		cmpxchg	dword ptr [rsi + 80], ecx
	je	.LBB16_15
.LBB16_17:
	mov	eax, dword ptr [rsi + 80]
	cmp	eax, 1
	je	.LBB16_17
	jmp	.LBB16_14
.LBB16_15:
	mov	dword ptr [rsp + 32], edx
	mov	rcx, rsi
	mov	edx, edi
	mov	r8d, r12d
	mov	ebx, dword ptr [rbp + 272]
	mov	r9d, ebx
	vzeroupper
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	test	al, 1
	je	.LBB16_16
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
	jle	.LBB16_19
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	ebx, dword ptr [rbp + 288]
	mov	r15d, dword ptr [rbp + 256]
	cmp	byte ptr [rbp + 24], 0
	jne	.LBB16_25
	jmp	.LBB16_34
.LBB16_19:
	mov	eax, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	eax, ecx
	cmovg	ebx, eax
	sub	ebx, ecx
	add	rcx, qword ptr [rsi + 40]
	cmp	ebx, 71
	mov	r15d, dword ptr [rbp + 256]
	jg	.LBB16_21
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
	jmp	.LBB16_22
.LBB16_21:
	vmovups	ymm0, ymmword ptr [rbp - 72]
	vmovups	ymmword ptr [rcx], ymm0
	vmovdqu	ymm0, ymmword ptr [rbp - 40]
	vmovdqu	ymmword ptr [rcx + 32], ymm0
	mov	rax, qword ptr [rbp - 8]
	mov	qword ptr [rcx + 64], rax
.LBB16_22:
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
	je	.LBB16_34
.LBB16_25:
	mov	r14, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	edi, r15d
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	qword ptr [rbp + 16], r14
	jmp	.LBB16_26
	.p2align	4
.LBB16_49:
	mov	dword ptr [rsp + 32], 0
	xor	ecx, ecx
	mov	edx, 2
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.ArchetypeChunk* chunks, int chunkCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0065"+2]
	xor	r9d, r9d
	vzeroupper
	call	r14
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
.LBB16_33:
	add	r13, 16
	dec	rdi
	je	.LBB16_34
.LBB16_26:
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
	je	.LBB16_50
	mov	r15d, dword ptr [rax + rcx + 8]
	shl	r15d, 3
	test	r15d, r15d
	jle	.LBB16_33
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r15d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jg	.LBB16_49
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
	jle	.LBB16_31
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
	jmp	.LBB16_32
.LBB16_31:
	mov	r8d, r15d
.LBB16_32:
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
	jmp	.LBB16_33
.LBB16_34:
	mov	rdi, qword ptr [rbp + 280]
	mov	r14, qword ptr [rbp + 264]
	test	r14, r14
	setne	al
	mov	r12d, dword ptr [rbp + 272]
	test	r12d, r12d
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB16_42
	shl	r12d, 2
	test	r12d, r12d
	jle	.LBB16_42
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r12d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB16_37
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB16_42
.LBB16_37:
	mov	r8d, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	r8d, ecx
	cmovg	eax, r8d
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r15d, r12d
	sub	r15d, eax
	jle	.LBB16_39
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
	jmp	.LBB16_40
.LBB16_39:
	mov	r8d, r12d
.LBB16_40:
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
.LBB16_42:
	test	rdi, rdi
	setne	al
	test	ebx, ebx
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB16_16
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, ebx
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB16_44
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB16_16
.LBB16_44:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r14d, ebx
	sub	r14d, eax
	jle	.LBB16_46
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
	jmp	.LBB16_47
.LBB16_46:
	mov	r8d, ebx
.LBB16_47:
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
.LBB16_16:
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
.LBB16_50:
	lea	rcx, [rip + burst.string.768c1a52ff82ef88cabe676b503130fa]
	lea	rdx, [rip + burst.string.1c3b0f8201e0c56284796afecfd7f5fb]
	vzeroupper
	call	qword ptr [rip + burst_abort_Ptr]
	int3
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
	jle	.LBB17_1
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
	jmp	.LBB17_23
.LBB17_1:
	jne	.LBB17_2
	mov	dword ptr [rcx + 28], 0
	mov	qword ptr [rcx + 16], 0
	mov	dword ptr [rcx + 60], 0
	mov	qword ptr [rcx + 48], 0
	xor	esi, esi
	xor	r9d, r9d
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	jne	.LBB17_6
	jmp	.LBB17_4
.LBB17_2:
	mov	esi, dword ptr [rcx + 28]
	mov	r9d, dword ptr [rcx + 52]
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	je	.LBB17_4
.LBB17_6:
	mov	eax, r11d
	sub	eax, dword ptr [rcx + 60]
	cmp	eax, r8d
	jge	.LBB17_7
.LBB17_4:
	mov	dword ptr [rbp + 92], r9d
	test	esi, esi
	jle	.LBB17_5
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
.LBB17_18:
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
	jge	.LBB17_13
	cmp	esi, eax
	jne	.LBB17_18
	mov	eax, esi
	jmp	.LBB17_13
.LBB17_5:
	xor	eax, eax
	xor	r9d, r9d
.LBB17_13:
	test	esi, esi
	setne	dl
	sub	esi, eax
	setge	bl
	test	dl, bl
	je	.LBB17_19
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
	je	.LBB17_24
	sub	ebx, r9d
	jl	.LBB17_24
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
.LBB17_7:
	cmp	esi, edi
	jne	.LBB17_8
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0153"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB17_9
.LBB17_19:
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
	jle	.LBB17_22
	movsxd	rcx, ebx
	cmp	r14d, 9
	mov	edi, 9
	cmovb	edi, r14d
	add	rcx, rsi
	lea	rdx, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0113.4"+2]
	mov	r8, rdi
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
	add	edi, ebx
	mov	dword ptr [rbp + 100], edi
	jmp	.LBB17_21
.LBB17_8:
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
.LBB17_9:
	mov	dil, 1
	jmp	.LBB17_10
.LBB17_24:
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
	jle	.LBB17_22
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
.LBB17_21:
	mov	ebx, edi
.LBB17_22:
	movsxd	rax, ebx
	mov	byte ptr [rbp + rax - 78], 0
	mov	word ptr [rsi - 2], ax
	mov	dword ptr [rsp + 32], 0
	xor	edi, edi
	xor	ecx, ecx
	mov	edx, 2
	mov	r8, rsi
.LBB17_23:
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
.LBB17_10:
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
	je	.LBB18_3
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
	jne	.LBB18_2
	mov	r15d, dword ptr [rsi]
	mov	r12, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	cmp	r15d, dword ptr [r12]
	jne	.LBB18_5
	add	rsi, 4
	dec	ebx
	jmp	.LBB18_2
.LBB18_5:
	mov	ebx, ecx
.LBB18_2:
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
.LBB18_3:
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

	.def	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@ffffffffffffffffffffffffffffffff
	.section	.rdata,"dr",discard,__xmm@ffffffffffffffffffffffffffffffff
	.p2align	4, 0x0
__xmm@ffffffffffffffffffffffffffffffff:
	.zero	16,255
	.section	.text,"xr",discard,"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
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
	mov	rcx, qword ptr [rcx + 456]
	mov	r10, qword ptr [rbx + 472]
	mov	rax, qword ptr [rbx + 496]
	mov	qword ptr [rbp + 176], rax
	mov	eax, dword ptr [rbx + 556]
	mov	rdx, qword ptr [rbx + 480]
	mov	r11, qword ptr [rbx + 488]
	mov	r8, qword ptr [rcx]
	mov	r9, qword ptr [r10]
	mov	qword ptr [rbp + 168], r10
	mov	ecx, dword ptr [r10 + 8]
	mov	r10, qword ptr [rdx]
	mov	r11, qword ptr [r11]
	mov	edx, dword ptr [rbx + 536]
	or	edx, dword ptr [rbx + 516]
	sete	dl
	jne	.LBB19_3
	cmp	dword ptr [rbx + 548], 0
	je	.LBB19_2
.LBB19_3:
	mov	sil, 1
.LBB19_4:
	test	eax, eax
	mov	dword ptr [rbp - 32], ecx
	mov	dword ptr [rbp - 28], 0
	mov	qword ptr [rbp - 24], r9
	mov	r9, qword ptr [rbp + 176]
	mov	qword ptr [rbp - 16], r9
	mov	qword ptr [rbp - 8], r10
	mov	qword ptr [rbp], r11
	mov	qword ptr [rbp + 8], r8
	vmovdqu	ymm0, ymmword ptr [rbx + 512]
	vmovdqu	ymmword ptr [rbp + 16], ymm0
	mov	r8, qword ptr [rbx + 544]
	mov	qword ptr [rbp + 48], r8
	mov	byte ptr [rbp + 56], sil
	setne	byte ptr [rbp + 57]
	mov	word ptr [rbp + 58], 0
	mov	dword ptr [rbp + 60], -1
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	ymmword ptr [rbp + 64], ymm0
	vmovdqu	ymmword ptr [rbp + 96], ymm0
	mov	qword ptr [rbp + 128], 0
	mov	r8d, dword ptr [rbx + 552]
	mov	dword ptr [rbp + 208], r8d
	mov	byte ptr [rbp + 223], 1
	test	dl, dl
	je	.LBB19_6
	cmp	dword ptr [rbx + 548], 0
	setne	byte ptr [rbp + 223]
.LBB19_6:
	lea	r14, [rbp + 16]
	cmp	dword ptr [rbp + 208], 1
	mov	dword ptr [rbp + 212], 0
	mov	dword ptr [rbp + 216], ecx
	jne	.LBB19_7
	jmp	.LBB19_11
.LBB19_2:
	xor	esi, esi
	jmp	.LBB19_4
.LBB19_7:
	test	eax, eax
	setne	al
	or	al, byte ptr [rbp + 223]
	test	al, 1
	je	.LBB19_13
	mov	eax, dword ptr [rbp + 216]
	mov	dword ptr [rbp - 32], eax
	mov	r13d, dword ptr [rbp + 212]
	cmp	r13d, eax
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jge	.LBB19_26
	lea	rax, [rbx + 160]
	mov	qword ptr [rbp - 56], rax
	vpcmpeqd	xmm6, xmm6, xmm6
.LBB19_19:
	movsxd	r12, r13d
	jmp	.LBB19_20
	.p2align	4
.LBB19_42:
	vptest	xmm0, xmm0
	jne	.LBB19_43
.LBB19_25:
	inc	r12
	movsxd	rax, dword ptr [rbp - 32]
	cmp	r12, rax
	jge	.LBB19_26
.LBB19_20:
	mov	rax, qword ptr [rbp - 8]
	mov	rcx, qword ptr [rbp]
	movsxd	r13, dword ptr [rcx + 4*r12]
	mov	eax, dword ptr [rax + 4*r12]
	cmp	dword ptr [rbp + 60], eax
	jne	.LBB19_21
.LBB19_23:
	cmp	byte ptr [rbp + 56], 0
	je	.LBB19_49
	mov	rcx, qword ptr [rbp + 64]
	mov	edx, r13d
	mov	r8, r14
	vzeroupper
	call	"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	test	al, 1
	je	.LBB19_25
.LBB19_49:
	cmp	byte ptr [rbp + 57], 0
	je	.LBB19_50
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
	je	.LBB19_28
	vpcmpeqd	xmm0, xmm0, xmm0
	test	r15d, r15d
	jle	.LBB19_38
	.p2align	4
.LBB19_37:
	movsxd	rsi, dword ptr [rdi]
	shl	rsi, 4
	vpand	xmm0, xmm0, xmmword ptr [rcx + rsi]
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	rdi, 4
	dec	r15
	jne	.LBB19_37
.LBB19_38:
	test	r11d, r11d
	jle	.LBB19_39
	movsxd	rsi, dword ptr [r13]
	shl	rsi, 4
	vmovdqu	xmm1, xmmword ptr [rcx + rsi]
	cmp	r11d, 1
	je	.LBB19_47
	dec	r11
	add	r13, 4
	.p2align	4
.LBB19_46:
	movsxd	rsi, dword ptr [r13]
	shl	rsi, 4
	vpor	xmm1, xmm1, xmmword ptr [rcx + rsi]
	add	r13, 4
	dec	r11
	jne	.LBB19_46
.LBB19_47:
	vpand	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
.LBB19_39:
	test	r9d, r9d
	jle	.LBB19_40
	.p2align	4
.LBB19_48:
	movsxd	r11, dword ptr [r10]
	shl	r11, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r11]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	r10, 4
	dec	r9
	jne	.LBB19_48
.LBB19_40:
	test	edx, edx
	jle	.LBB19_42
	.p2align	4
.LBB19_41:
	movsxd	r9, dword ptr [r8]
	shl	r9, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r9]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	add	r8, 4
	dec	rdx
	jne	.LBB19_41
	jmp	.LBB19_42
.LBB19_21:
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
	je	.LBB19_23
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
	jmp	.LBB19_23
.LBB19_28:
	mov	esi, 128
	sub	esi, eax
	cmp	esi, 127
	jg	.LBB19_29
	cmp	eax, 128
	je	.LBB19_31
	cmp	esi, 63
	mov	rdi, -1
	shrx	rsi, rdi, rsi
	jg	.LBB19_34
	shlx	rdi, rdi, rax
	or	rdi, rsi
	vmovq	xmm0, rdi
	vmovq	xmm1, rsi
	vpunpcklqdq	xmm0, xmm0, xmm1
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB19_38
.LBB19_34:
	vmovq	xmm0, rsi
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB19_38
.LBB19_29:
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB19_38
.LBB19_31:
	vpcmpeqd	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 192], xmm0
	jmp	.LBB19_38
	.p2align	4
.LBB19_50:
	xor	r9d, r9d
	jmp	.LBB19_18
	.p2align	4
.LBB19_43:
	vmovq	rcx, xmm0
	vpextrq	rdx, xmm0, 1
	popcnt	rcx, rcx
	popcnt	rdx, rdx
	add	edx, ecx
	cmp	edx, eax
	setne	r9b
.LBB19_18:
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
	call	"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	inc	r13d
	cmp	r13d, dword ptr [rbp - 32]
	jl	.LBB19_19
.LBB19_26:
	mov	dword ptr [rbp + 152], 0
	mov	qword ptr [rbp + 160], 0
	jmp	.LBB19_16
.LBB19_13:
	mov	rax, qword ptr [rbp + 168]
	mov	rdi, qword ptr [rax]
	mov	dword ptr [rbp + 136], 0
	mov	rax, qword ptr [rbp + 176]
	mov	qword ptr [rbp + 144], rax
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp - 80], xmm0
	movsxd	r15, dword ptr [rbp + 212]
	cmp	r15d, dword ptr [rbp + 216]
	jge	.LBB19_16
	lea	r12, [rbx + 160]
	lea	rsi, [rbp - 80]
	lea	r13, [rbp + 136]
	.p2align	4
.LBB19_15:
	mov	eax, dword ptr [rdi + 4*r15]
	mov	dword ptr [rbp + 136], eax
	mov	qword ptr [rsp + 32], rsi
	mov	rcx, r12
	mov	rdx, r13
	mov	r8d, r15d
	xor	r9d, r9d
	vzeroupper
	call	"Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Systems.UpdateSkeletonBoundsSystem+GatherExposedSkeletonBoundsJob*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 unfilteredChunkIndex, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	inc	r15
	movsxd	rax, dword ptr [rbp + 216]
	cmp	r15, rax
	jl	.LBB19_15
.LBB19_16:
	cmp	dword ptr [rbp + 208], 1
	jne	.LBB19_17
	mov	dword ptr [rbp + 212], 0
	mov	rax, qword ptr [rbp + 168]
	mov	eax, dword ptr [rax + 8]
	mov	dword ptr [rbp + 216], eax
.LBB19_11:
	lea	r8, [rbp + 212]
	lea	r9, [rbp + 216]
	mov	rcx, qword ptr [rbp - 64]
	mov	edx, dword ptr [rbp + 188]
	vzeroupper
	call	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	test	al, al
	je	.LBB19_17
	mov	eax, dword ptr [rbx + 556]
	jmp	.LBB19_7
.LBB19_17:
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
	jle	.LBB20_5
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
.LBB20_2:
	movsxd	rbx, dword ptr [r8 + 4*rdi + 8]
	mov	ebx, dword ptr [rcx + 4*rbx + 28]
	sub	ebx, edx
	imul	ebx, r9d
	movsxd	rbx, ebx
	mov	ebx, dword ptr [rsi + 4*rbx]
	cmp	ebx, dword ptr [r8 + 4*rdi + 16]
	jne	.LBB20_3
	inc	rdi
	cmp	edi, eax
	jl	.LBB20_2
.LBB20_5:
	mov	edx, dword ptr [r8 + 24]
	mov	eax, dword ptr [r8 + 36]
	test	rdx, rdx
	je	.LBB20_8
	test	eax, eax
	jne	.LBB20_10
	xor	eax, eax
	test	edx, edx
	jg	.LBB20_13
.LBB20_12:
	xor	ebx, ebx
	jmp	.LBB20_31
.LBB20_3:
	xor	eax, eax
	jmp	.LBB20_32
.LBB20_8:
	test	eax, eax
	je	.LBB20_9
.LBB20_10:
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
	jle	.LBB20_12
.LBB20_13:
	lea	esi, [4*r9]
	movsxd	rsi, esi
	add	rsi, r10
	mov	qword ptr [rbp - 104], r8
	mov	r15d, dword ptr [r8]
	lea	r11, [rsi + 4*r11]
	cmp	edx, 8
	jae	.LBB20_15
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB20_29
.LBB20_15:
	cmp	edx, 31
	ja	.LBB20_17
	vmovd	xmm0, r15d
	vpbroadcastd	ymm0, xmm0
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB20_24
.LBB20_9:
	mov	al, 1
	jmp	.LBB20_32
.LBB20_17:
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
	jmp	.LBB20_18
	.p2align	4
.LBB20_20:
	vpor	xmm5, xmm9, xmm5
	vpor	xmm4, xmm8, xmm4
	vpor	xmm3, xmm3, xmm7
	vpor	xmm1, xmm1, xmm6
	sub	rdi, -128
	add	rbx, -32
	je	.LBB20_21
.LBB20_18:
	test	r15d, r15d
	vpcmpeqd	xmm6, xmm6, xmm6
	vpcmpeqd	xmm7, xmm7, xmm7
	vpcmpeqd	xmm8, xmm8, xmm8
	vpcmpeqd	xmm9, xmm9, xmm9
	je	.LBB20_20
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
	jmp	.LBB20_20
.LBB20_21:
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
	jne	.LBB20_23
	mov	eax, r10d
	jmp	.LBB20_31
.LBB20_23:
	test	dl, 24
	mov	eax, r10d
	je	.LBB20_29
.LBB20_24:
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
	jmp	.LBB20_25
	.p2align	4
.LBB20_27:
	vpor	xmm1, xmm1, xmm3
	add	rbx, 32
	add	rdi, 8
	je	.LBB20_28
.LBB20_25:
	test	r15d, r15d
	vpcmpeqd	xmm3, xmm3, xmm3
	je	.LBB20_27
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
	jmp	.LBB20_27
.LBB20_28:
	vptest	xmm1, xmmword ptr [rip + __xmm@00010001000100010001000100010001]
	setne	bl
	mov	rdx, qword ptr [rbp - 128]
	mov	rdi, qword ptr [rbp - 120]
	cmp	edi, edx
	mov	eax, dword ptr [rbp - 108]
	je	.LBB20_31
.LBB20_29:
	sub	rdx, rdi
	mov	r8, qword ptr [rbp - 104]
	lea	r8, [r8 + 4*rdi + 28]
	mov	esi, ebx
	.p2align	4
.LBB20_30:
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
	jne	.LBB20_30
.LBB20_31:
	or	al, bl
.LBB20_32:
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

	.def	"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8"
	.globl	"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8"
	.p2align	4
"Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8":
.seh_proc "Unity.Entities.TypeIndex, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ToFixedString(Unity.Entities.TypeIndex*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this) -> Unity.Collections.FixedString128Bytes, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.5@@8"
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
	sub	rsp, 1184
	.seh_stackalloc 1184
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	rsi, rdx
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp + 928], ymm0
	vmovups	ymmword ptr [rbp + 960], ymm0
	vmovups	ymmword ptr [rbp + 992], ymm0
	vmovups	ymmword ptr [rbp + 1024], ymm0
	mov	eax, dword ptr [rcx]
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeInfos::Ref"]
	mov	rcx, qword ptr [rcx]
	and	eax, 8191
	imul	rax, rax, 104
	mov	ebx, dword ptr [rcx + rax]
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.IntPtr> Unity.Entities.TypeManager/SharedTypeNames::Ref"]
	mov	rdi, qword ptr [rax]
	mov	r14, qword ptr [rip + "Unity.Burst.SharedStatic`1<System.Int32> Unity.Collections.NativeText/ReadOnly::s_staticSafetyId"]
	lea	rcx, [rbp + 418]
	lea	rdx, [rip + "Unity.Collections.NativeText.ReadOnly..ctor(Unity.Collections.NativeText.ReadOnly* this, Unity.Collections.LowLevel.Unsafe.UnsafeText* text, Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle safety) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_001E"+2]
	movzx	eax, word ptr [rip + "Unity.Collections.NativeText.ReadOnly..ctor(Unity.Collections.NativeText.ReadOnly* this, Unity.Collections.LowLevel.Unsafe.UnsafeText* text, Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle safety) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_001E"]
	cmp	eax, 509
	mov	r8d, 509
	cmovb	r8d, eax
	mov	word ptr [rbp + 416], r8w
	mov	byte ptr [rbp + r8 + 418], 0
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	vmovups	ymm0, ymmword ptr [rbp + 416]
	vmovups	ymm1, ymmword ptr [rbp + 448]
	vmovups	ymm2, ymmword ptr [rbp + 480]
	vmovups	ymm3, ymmword ptr [rbp + 512]
	vmovups	ymmword ptr [rbp - 96], ymm0
	vmovups	ymmword ptr [rbp - 64], ymm1
	vmovups	ymmword ptr [rbp - 32], ymm2
	vmovups	ymmword ptr [rbp], ymm3
	vmovups	ymm0, ymmword ptr [rbp + 544]
	vmovups	ymmword ptr [rbp + 32], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 576]
	vmovups	ymmword ptr [rbp + 64], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 608]
	vmovups	ymmword ptr [rbp + 96], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 640]
	vmovups	ymmword ptr [rbp + 128], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 672]
	vmovups	ymmword ptr [rbp + 160], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 704]
	vmovups	ymm1, ymmword ptr [rbp + 736]
	vmovups	ymm2, ymmword ptr [rbp + 768]
	vmovups	ymm3, ymmword ptr [rbp + 800]
	vmovups	ymmword ptr [rbp + 192], ymm0
	vmovups	ymmword ptr [rbp + 224], ymm1
	vmovups	ymmword ptr [rbp + 256], ymm2
	vmovups	ymmword ptr [rbp + 288], ymm3
	vmovups	ymm0, ymmword ptr [rbp + 832]
	vmovups	ymmword ptr [rbp + 320], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 864]
	vmovups	ymmword ptr [rbp + 352], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 896]
	vmovups	ymmword ptr [rbp + 384], ymm0
	cmp	dword ptr [r14], 0
	jne	.LBB21_2
	lea	rcx, [rbp - 94]
	movzx	edx, word ptr [rbp - 96]
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::NewStaticSafetyId_Ptr"]
	mov	dword ptr [r14], eax
.LBB21_2:
	and	ebx, 8191
	lea	rdx, [rbx + 2*rbx]
	mov	eax, dword ptr [rdi + 8*rdx + 8]
	dec	eax
	cmp	eax, 125
	ja	.LBB21_6
	lea	rcx, [rbp + 930]
	mov	rdx, qword ptr [rdi + 8*rdx]
	test	eax, eax
	mov	r8d, eax
	je	.LBB21_5
	mov	word ptr [rbp + 928], ax
	mov	byte ptr [rbp + r8 + 930], 0
.LBB21_5:
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
.LBB21_6:
	vmovups	ymm0, ymmword ptr [rbp + 928]
	vmovups	ymmword ptr [rsi], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 960]
	vmovups	ymmword ptr [rsi + 32], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 992]
	vmovups	ymmword ptr [rsi + 64], ymm0
	vmovups	ymm0, ymmword ptr [rbp + 1024]
	vmovups	ymmword ptr [rsi + 96], ymm0
	mov	rax, rsi
	.seh_startepilogue
	add	rsp, 1184
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
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
	jb	.LBB22_1
	xor	r9d, r9d
	.p2align	4
.LBB22_3:
	lea	rax, [r9 + 32]
	vmovups	ymm0, ymmword ptr [rdx + r9]
	vmovups	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB22_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB22_6
	.p2align	4
.LBB22_5:
	lea	r9, [rax + 1]
	movzx	r10d, byte ptr [rdx + rax]
	mov	byte ptr [rcx + rax], r10b
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB22_5
.LBB22_6:
	vzeroupper
	ret
.LBB22_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB22_5
	jmp	.LBB22_6

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
	jb	.LBB23_3
	xor	eax, eax
	vmovd	xmm0, edx
	vpbroadcastb	ymm0, xmm0
	.p2align	4
.LBB23_2:
	movsxd	r9, eax
	add	eax, 32
	vmovdqu	ymmword ptr [rcx + r9], ymm0
	add	r9d, 64
	cmp	r8d, r9d
	jae	.LBB23_2
.LBB23_3:
	lea	r9d, [rax + 1]
	cmp	r8d, r9d
	jb	.LBB23_5
	.p2align	4
.LBB23_4:
	movsxd	r9, eax
	inc	eax
	mov	byte ptr [rcx + r9], dl
	add	r9d, 2
	cmp	r8d, r9d
	jae	.LBB23_4
.LBB23_5:
	vzeroupper
	ret

	.section	.drectve,"yni"
	.ascii	" /EXPORT:36ef535b9300e20e05c4578acab7face"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.36ef535b9300e20e05c4578acab7face\""
	.ascii	" /EXPORT:\"burst.initialize.statics.36ef535b9300e20e05c4578acab7face\""
	.globl	_fltused
