
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	361bfdbc31942f74f67b67d45a212d8e;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	361bfdbc31942f74f67b67d45a212d8e
	.p2align	4
361bfdbc31942f74f67b67d45a212d8e:
.seh_proc 361bfdbc31942f74f67b67d45a212d8e
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
	jmp	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
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
	jmp	burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e
	.seh_endproc

	.def	burst.initialize.externals.361bfdbc31942f74f67b67d45a212d8e;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.361bfdbc31942f74f67b67d45a212d8e
	.p2align	4
burst.initialize.externals.361bfdbc31942f74f67b67d45a212d8e:
.seh_proc burst.initialize.externals.361bfdbc31942f74f67b67d45a212d8e
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

	.def	burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e
	.p2align	4
burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e:
.seh_proc burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e
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

	.def	"Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@3f800000bf800000bf800000bf800000
	.section	.rdata,"dr",discard,__xmm@3f800000bf800000bf800000bf800000
	.p2align	4, 0x0
__xmm@3f800000bf800000bf800000bf800000:
	.long	0xbf800000
	.long	0xbf800000
	.long	0xbf800000
	.long	0x3f800000
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.section	.text,"xr",discard,"Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
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
	sub	rsp, 88
	.seh_stackalloc 88
	lea	rbp, [rsp + 80]
	.seh_setframe rbp, 80
	vmovaps	xmmword ptr [rbp - 16], xmm7
	.seh_savexmm xmm7, 64
	vmovaps	xmmword ptr [rbp - 32], xmm6
	.seh_savexmm xmm6, 48
	.seh_endprologue
	mov	ebx, r9d
	mov	r14, rdx
	mov	r15, rcx
	mov	rax, qword ptr [rdx + 8]
	mov	ecx, dword ptr [rdx]
	mov	dword ptr [rbp - 48], ecx
	mov	qword ptr [rbp - 40], rax
	lea	rcx, [rbp - 48]
	mov	rdx, r15
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rsi, rax
	mov	rax, qword ptr [r14 + 8]
	mov	ecx, dword ptr [r14]
	lea	rdx, [r15 + 64]
	mov	dword ptr [rbp - 48], ecx
	mov	qword ptr [rbp - 40], rax
	lea	rcx, [rbp - 48]
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rdi, rax
	mov	rax, qword ptr [r14 + 8]
	mov	ecx, dword ptr [r14]
	sub	r15, -128
	mov	dword ptr [rbp - 48], ecx
	mov	qword ptr [rbp - 40], rax
	lea	rcx, [rbp - 48]
	mov	rdx, r15
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	movsxd	rcx, dword ptr [r14]
	mov	rdx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rdx, qword ptr [rdx]
	shl	rcx, 4
	mov	edx, dword ptr [rdx + rcx + 8]
	test	bl, bl
	je	.LBB4_1
	mov	rcx, qword ptr [rbp + 96]
	mov	r8, qword ptr [rcx]
	mov	r9, qword ptr [rcx + 8]
	lea	r10, [r8 + r8]
	xor	r10, r8
	popcnt	r10, r10
	lea	r11, [r9 + r9]
	xor	r11, r9
	popcnt	r9, r11
	add	r9d, r10d
	cmp	r9d, 5
	ja	.LBB4_7
	vmovdqu	xmm2, xmmword ptr [rcx]
	xor	r10d, r10d
	vmovaps	xmm0, xmmword ptr [rip + __xmm@3f800000bf800000bf800000bf800000]
	vbroadcastss	xmm1, dword ptr [rip + __real@3f800000]
.LBB4_17:
	vpextrq	rdx, xmm2, 1
	vmovq	r8, xmm2
	mov	r9d, r10d
	.p2align	4
.LBB4_18:
	vpxor	xmm3, xmm3, xmm3
	cmp	r9d, 127
	vpxor	xmm4, xmm4, xmm4
	jg	.LBB4_23
	test	r9d, r9d
	vmovdqa	xmm4, xmm2
	je	.LBB4_23
	mov	r10d, r9d
	and	r10d, 63
	cmp	r9d, 63
	jg	.LBB4_22
	shrx	r11, r8, r10
	mov	ebx, r9d
	neg	bl
	shlx	rbx, rdx, rbx
	or	rbx, r11
	shrx	r10, rdx, r10
	vmovq	xmm4, rbx
	vmovq	xmm5, r10
	vpunpcklqdq	xmm4, xmm4, xmm5
	jmp	.LBB4_23
	.p2align	4
.LBB4_22:
	shrx	r10, rdx, r10
	vmovq	xmm4, r10
.LBB4_23:
	vpextrq	r10, xmm4, 1
	tzcnt	r14, r10
	add	r14d, 64
	vmovq	rbx, xmm4
	tzcnt	r11, rbx
	cmovb	r11d, r14d
	cmp	r11d, 127
	ja	.LBB4_28
	test	r11d, r11d
	vmovdqa	xmm3, xmm4
	je	.LBB4_28
	cmp	r11d, 63
	ja	.LBB4_27
	shrx	rbx, rbx, r11
	mov	r14d, r11d
	neg	r14b
	shlx	r14, r10, r14
	or	r14, rbx
	shrx	r10, r10, r11
	vmovq	xmm3, r14
	vmovq	xmm4, r10
	vpunpcklqdq	xmm3, xmm3, xmm4
	jmp	.LBB4_28
	.p2align	4
.LBB4_27:
	shrx	r10, r10, r11
	vmovq	xmm3, r10
.LBB4_28:
	vmovq	rbx, xmm3
	mov	r10, rbx
	not	r10
	vpextrq	r14, xmm3, 1
	not	r14
	tzcnt	r14, r14
	add	r14d, 64
	tzcnt	r10, r10
	xor	rbx, -1
	cmove	r10d, r14d
	test	r10d, r10d
	je	.LBB4_4
	add	r11d, r9d
	add	r10d, r11d
	cmp	r11d, r10d
	mov	r9d, r10d
	jge	.LBB4_18
	movsxd	r8, r11d
	movsxd	rdx, r10d
	sub	rdx, r8
	shl	r8, 2
	lea	r8, [r8 + 2*r8]
	.p2align	4
.LBB4_31:
	vmovsd	xmm2, qword ptr [rdi + r8]
	vinsertps	xmm2, xmm2, dword ptr [rdi + r8 + 8], 32
	vmovsd	xmm3, qword ptr [rax + 4*r8 + 16]
	vinsertps	xmm3, xmm3, dword ptr [rax + 4*r8 + 24], 32
	vsubps	xmm2, xmm2, xmm3
	vmulps	xmm3, xmm0, xmmword ptr [rax + 4*r8]
	vshufps	xmm4, xmm2, xmm2, 201
	vmulps	xmm4, xmm4, xmm3
	vshufps	xmm5, xmm3, xmm3, 201
	vmulps	xmm6, xmm2, xmm5
	vsubps	xmm4, xmm4, xmm6
	vaddps	xmm4, xmm4, xmm4
	vshufps	xmm6, xmm4, xmm4, 201
	vshufps	xmm7, xmm3, xmm3, 255
	vmulps	xmm7, xmm7, xmm6
	vaddps	xmm2, xmm2, xmm7
	vshufps	xmm4, xmm4, xmm4, 210
	vmulps	xmm3, xmm3, xmm4
	vmulps	xmm4, xmm5, xmm6
	vsubps	xmm3, xmm3, xmm4
	vshufps	xmm3, xmm3, xmm3, 201
	vaddps	xmm2, xmm2, xmm3
	vdivps	xmm3, xmm1, xmmword ptr [rax + 4*r8 + 32]
	vmulps	xmm2, xmm3, xmm2
	vshufps	xmm3, xmm3, xmm3, 255
	vmulps	xmm2, xmm3, xmm2
	vmovlps	qword ptr [rsi + r8], xmm2
	vextractps	dword ptr [rsi + r8 + 8], xmm2, 2
	add	r8, 12
	dec	rdx
	jne	.LBB4_31
	vmovdqu	xmm2, xmmword ptr [rcx]
	jmp	.LBB4_17
.LBB4_1:
	test	edx, edx
	jle	.LBB4_4
	shl	rdx, 2
	lea	rcx, [rdx + 2*rdx]
	xor	edx, edx
	vmovaps	xmm0, xmmword ptr [rip + __xmm@3f800000bf800000bf800000bf800000]
	vbroadcastss	xmm1, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB4_3:
	vmovsd	xmm2, qword ptr [rdi + rdx]
	vinsertps	xmm2, xmm2, dword ptr [rdi + rdx + 8], 32
	vmovsd	xmm3, qword ptr [rax + 4*rdx + 16]
	vinsertps	xmm3, xmm3, dword ptr [rax + 4*rdx + 24], 32
	vsubps	xmm2, xmm2, xmm3
	vmulps	xmm3, xmm0, xmmword ptr [rax + 4*rdx]
	vshufps	xmm4, xmm2, xmm2, 201
	vmulps	xmm4, xmm4, xmm3
	vshufps	xmm5, xmm3, xmm3, 201
	vmulps	xmm6, xmm2, xmm5
	vsubps	xmm4, xmm4, xmm6
	vaddps	xmm4, xmm4, xmm4
	vshufps	xmm6, xmm4, xmm4, 201
	vshufps	xmm7, xmm3, xmm3, 255
	vmulps	xmm7, xmm7, xmm6
	vaddps	xmm2, xmm2, xmm7
	vshufps	xmm4, xmm4, xmm4, 210
	vmulps	xmm3, xmm3, xmm4
	vmulps	xmm4, xmm5, xmm6
	vsubps	xmm3, xmm3, xmm4
	vshufps	xmm3, xmm3, xmm3, 201
	vaddps	xmm2, xmm2, xmm3
	vdivps	xmm3, xmm1, xmmword ptr [rax + 4*rdx + 32]
	vmulps	xmm2, xmm3, xmm2
	vshufps	xmm3, xmm3, xmm3, 255
	vmulps	xmm2, xmm3, xmm2
	vmovlps	qword ptr [rsi + rdx], xmm2
	vextractps	dword ptr [rsi + rdx + 8], xmm2, 2
	add	rdx, 12
	cmp	rcx, rdx
	jne	.LBB4_3
	jmp	.LBB4_4
.LBB4_7:
	test	edx, edx
	jle	.LBB4_4
	cmp	edx, 64
	mov	r9d, 64
	cmovb	r9d, edx
	shl	r9, 2
	lea	r9, [r9 + 2*r9]
	xor	r10d, r10d
	vmovaps	xmm0, xmmword ptr [rip + __xmm@3f800000bf800000bf800000bf800000]
	vbroadcastss	xmm1, dword ptr [rip + __real@3f800000]
	jmp	.LBB4_9
	.p2align	4
.LBB4_11:
	shr	r8
	add	r10, 12
	cmp	r9, r10
	je	.LBB4_12
.LBB4_9:
	test	r8b, 1
	je	.LBB4_11
	vmovsd	xmm2, qword ptr [rdi + r10]
	vinsertps	xmm2, xmm2, dword ptr [rdi + r10 + 8], 32
	vmovsd	xmm3, qword ptr [rax + 4*r10 + 16]
	vinsertps	xmm3, xmm3, dword ptr [rax + 4*r10 + 24], 32
	vsubps	xmm2, xmm2, xmm3
	vmulps	xmm3, xmm0, xmmword ptr [rax + 4*r10]
	vshufps	xmm4, xmm2, xmm2, 201
	vmulps	xmm4, xmm4, xmm3
	vshufps	xmm5, xmm3, xmm3, 201
	vmulps	xmm6, xmm2, xmm5
	vsubps	xmm4, xmm4, xmm6
	vaddps	xmm4, xmm4, xmm4
	vshufps	xmm6, xmm4, xmm4, 201
	vshufps	xmm7, xmm3, xmm3, 255
	vmulps	xmm7, xmm7, xmm6
	vaddps	xmm2, xmm2, xmm7
	vshufps	xmm4, xmm4, xmm4, 210
	vmulps	xmm3, xmm3, xmm4
	vmulps	xmm4, xmm5, xmm6
	vsubps	xmm3, xmm3, xmm4
	vshufps	xmm3, xmm3, xmm3, 201
	vaddps	xmm2, xmm2, xmm3
	vdivps	xmm3, xmm1, xmmword ptr [rax + 4*r10 + 32]
	vmulps	xmm2, xmm3, xmm2
	vshufps	xmm3, xmm3, xmm3, 255
	vmulps	xmm2, xmm3, xmm2
	vmovlps	qword ptr [rsi + r10], xmm2
	vextractps	dword ptr [rsi + r10 + 8], xmm2, 2
	jmp	.LBB4_11
.LBB4_12:
	cmp	edx, 65
	jl	.LBB4_4
	mov	rcx, qword ptr [rcx + 8]
	lea	rdx, [rdx + 2*rdx]
	lea	rdx, [4*rdx - 768]
	xor	r8d, r8d
	jmp	.LBB4_14
	.p2align	4
.LBB4_15:
	shr	rcx
	add	r8, 12
	cmp	rdx, r8
	je	.LBB4_4
.LBB4_14:
	test	cl, 1
	je	.LBB4_15
	vmovsd	xmm2, qword ptr [rdi + r8 + 768]
	vinsertps	xmm2, xmm2, dword ptr [rdi + r8 + 776], 32
	vmovsd	xmm3, qword ptr [rax + 4*r8 + 3088]
	vinsertps	xmm3, xmm3, dword ptr [rax + 4*r8 + 3096], 32
	vsubps	xmm2, xmm2, xmm3
	vmulps	xmm3, xmm0, xmmword ptr [rax + 4*r8 + 3072]
	vshufps	xmm4, xmm2, xmm2, 201
	vmulps	xmm4, xmm4, xmm3
	vshufps	xmm5, xmm3, xmm3, 201
	vmulps	xmm6, xmm2, xmm5
	vsubps	xmm4, xmm4, xmm6
	vaddps	xmm4, xmm4, xmm4
	vshufps	xmm6, xmm4, xmm4, 201
	vshufps	xmm7, xmm3, xmm3, 255
	vmulps	xmm7, xmm7, xmm6
	vaddps	xmm2, xmm2, xmm7
	vshufps	xmm4, xmm4, xmm4, 210
	vmulps	xmm3, xmm3, xmm4
	vmulps	xmm4, xmm5, xmm6
	vsubps	xmm3, xmm3, xmm4
	vshufps	xmm3, xmm3, xmm3, 201
	vaddps	xmm2, xmm2, xmm3
	vdivps	xmm3, xmm1, xmmword ptr [rax + 4*r8 + 3104]
	vmulps	xmm2, xmm3, xmm2
	vshufps	xmm3, xmm3, xmm3, 255
	vmulps	xmm2, xmm3, xmm2
	vmovlps	qword ptr [rsi + r8 + 768], xmm2
	vextractps	dword ptr [rsi + r8 + 776], xmm2, 2
	jmp	.LBB4_15
.LBB4_4:
	vmovaps	xmm6, xmmword ptr [rbp - 32]
	vmovaps	xmm7, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 88
	pop	rbx
	pop	rdi
	pop	rsi
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
.LBB5_1:
	cqo
	idiv	r12
	inc	r13d
	test	rax, rax
	jne	.LBB5_1
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
.LBB5_3:
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
	jne	.LBB5_3
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
.LBB6_1:
	mov	rax, rcx
	xor	edx, edx
	div	r12
	inc	r13d
	cmp	rcx, r12
	mov	rcx, rax
	jae	.LBB6_1
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
.LBB6_3:
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
	jae	.LBB6_3
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
	jne	.LBB7_1
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
.LBB7_1:
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
	je	.LBB8_1
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB8_23
	lea	ebx, [rdi + 1]
	mov	dword ptr [rdx], ebx
	mov	byte ptr [rcx + rdi], 45
.LBB8_1:
	test	esi, esi
	jle	.LBB8_14
	inc	esi
	mov	r11d, 48
	.p2align	4
.LBB8_3:
	movsxd	rdi, dword ptr [rdx]
	cmp	edi, r8d
	jge	.LBB8_23
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
	jg	.LBB8_3
	xor	r11d, r11d
	test	r11d, r11d
	jns	.LBB8_7
	jmp	.LBB8_8
.LBB8_14:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB8_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	test	r11d, r11d
	js	.LBB8_8
.LBB8_7:
	cmp	byte ptr [r10], 0
	jne	.LBB8_8
.LBB8_20:
	test	al, al
	jne	.LBB8_23
	movsxd	rax, dword ptr [rdx]
	cmp	eax, r8d
	jge	.LBB8_23
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
	jmp	.LBB8_23
.LBB8_8:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jge	.LBB8_23
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 46
	test	r11d, r11d
	js	.LBB8_17
.LBB8_10:
	cmp	byte ptr [r10], 0
	je	.LBB8_20
	inc	r10
	.p2align	4
.LBB8_19:
	movsxd	r11, dword ptr [rdx]
	cmp	r11d, r8d
	jge	.LBB8_23
	lea	esi, [r11 + 1]
	mov	dword ptr [rdx], esi
	movzx	ebx, byte ptr [r10 - 1]
	mov	byte ptr [rcx + r11], bl
	cmp	byte ptr [r10], 0
	lea	r10, [r10 + 1]
	jne	.LBB8_19
	jmp	.LBB8_20
	.p2align	4
.LBB8_16:
	lea	edi, [rsi + 1]
	mov	dword ptr [rdx], edi
	mov	byte ptr [rcx + rsi], 48
	inc	r11d
	je	.LBB8_10
.LBB8_17:
	movsxd	rsi, dword ptr [rdx]
	cmp	esi, r8d
	jl	.LBB8_16
.LBB8_23:
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
	jne	.LBB9_4
	movzx	r11d, dl
	add	r11d, -4
	cmp	r11d, -4
	mov	r11d, edx
	ja	.LBB9_4
	mov	edi, dword ptr [rbp + 112]
	test	edi, edi
	jle	.LBB9_26
	mov	rax, qword ptr [r9]
	jmp	.LBB9_27
.LBB9_4:
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
	jle	.LBB9_9
	mov	edi, edx
	sub	edi, eax
	jle	.LBB9_9
	.p2align	4
.LBB9_6:
	movsxd	rbx, dword ptr [rsi]
	cmp	ebx, r8d
	jge	.LBB9_72
	lea	r14d, [rbx + 1]
	mov	dword ptr [rsi], r14d
	mov	byte ptr [rcx + rbx], 32
	dec	edi
	jne	.LBB9_6
	movzx	ebx, byte ptr [r9 + 20]
.LBB9_9:
	test	bl, bl
	je	.LBB9_11
	mov	r11d, dword ptr [rsi]
	mov	bl, 45
	cmp	r11d, r8d
	jl	.LBB9_13
	jmp	.LBB9_21
.LBB9_11:
	cmp	r11b, 2
	jne	.LBB9_14
	mov	r11d, dword ptr [rsi]
	mov	bl, 43
	cmp	r11d, r8d
	jge	.LBB9_21
.LBB9_13:
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movsxd	r11, r11d
	mov	byte ptr [rcx + r11], bl
.LBB9_14:
	test	r10d, r10d
	jle	.LBB9_17
	.p2align	4
.LBB9_15:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB9_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	mov	byte ptr [rcx + r11], 48
	dec	r10d
	jne	.LBB9_15
.LBB9_17:
	movsxd	r10, dword ptr [r9 + 12]
	test	r10, r10
	jle	.LBB9_21
	mov	r9, qword ptr [r9]
	.p2align	4
.LBB9_19:
	movsxd	r11, dword ptr [rsi]
	cmp	r11d, r8d
	jge	.LBB9_21
	lea	edi, [r11 + 1]
	mov	dword ptr [rsi], edi
	movzx	ebx, byte ptr [r9]
	mov	byte ptr [rcx + r11], bl
	inc	r9
	dec	r10
	jne	.LBB9_19
.LBB9_21:
	test	r15b, r15b
	jns	.LBB9_72
	mov	r9d, edx
	neg	r9d
	cmp	eax, r9d
	jge	.LBB9_72
	add	eax, edx
	neg	eax
	.p2align	4
.LBB9_24:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB9_72
	lea	r9d, [rdx + 1]
	mov	dword ptr [rsi], r9d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB9_24
	jmp	.LBB9_72
.LBB9_26:
	mov	edi, dword ptr [r9 + 12]
	mov	rax, qword ptr [r9]
	test	edi, edi
	jle	.LBB9_38
.LBB9_27:
	mov	edx, edi
	xor	r11d, r11d
	.p2align	4
.LBB9_28:
	cmp	byte ptr [rax + r11], 0
	je	.LBB9_39
	inc	r11
	cmp	rdx, r11
	jne	.LBB9_28
	mov	edi, edx
	cmp	r10d, 1
	jne	.LBB9_31
	jmp	.LBB9_41
.LBB9_38:
	xor	r11d, r11d
.LBB9_39:
	cmp	r11d, edi
	jne	.LBB9_42
	mov	edx, edi
	cmp	r10d, 1
	je	.LBB9_41
.LBB9_31:
	cmp	byte ptr [rax + rdx], 53
	jb	.LBB9_41
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB9_33:
	test	edx, edx
	jle	.LBB9_36
	movzx	r11d, byte ptr [rax + rdx - 1]
	dec	rdx
	dec	r10d
	cmp	r11b, 57
	je	.LBB9_33
	inc	r11b
	mov	byte ptr [rax + rdx], r11b
	jmp	.LBB9_49
.LBB9_41:
	mov	r11d, edi
	jmp	.LBB9_43
.LBB9_42:
	mov	edx, r11d
.LBB9_43:
	mov	ebx, r11d
	sar	ebx, 31
	and	ebx, r11d
	lea	r10d, [rdx + 1]
	.p2align	4
.LBB9_44:
	test	edx, edx
	jle	.LBB9_46
	dec	r10d
	cmp	byte ptr [rax + rdx - 1], 48
	lea	rdx, [rdx - 1]
	je	.LBB9_44
	jmp	.LBB9_49
.LBB9_46:
	test	rdx, rdx
	je	.LBB9_48
	mov	r10d, ebx
	jmp	.LBB9_49
.LBB9_36:
	inc	dword ptr [r9 + 16]
	mov	byte ptr [rax], 49
	mov	r10d, 1
	jmp	.LBB9_49
.LBB9_48:
	mov	dword ptr [r9 + 16], 0
	xor	r10d, r10d
.LBB9_49:
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
	jle	.LBB9_53
	mov	r10b, r13b
	lea	r11d, [rdx + 1]
	.p2align	4
.LBB9_51:
	cmp	byte ptr [rax], 1
	sbb	rax, -1
	dec	r11d
	cmp	r11d, 1
	ja	.LBB9_51
	add	edx, r10d
	xor	r13d, r13d
	mov	r15d, edx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	jns	.LBB9_54
	jmp	.LBB9_57
.LBB9_53:
	cmp	r11b, 1
	sbb	r15d, -1
	mov	r13d, ebx
	movzx	edx, byte ptr [rax]
	test	r13d, r13d
	js	.LBB9_57
.LBB9_54:
	test	dl, dl
	jne	.LBB9_57
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	je	.LBB9_61
.LBB9_56:
	movsx	r12d, r14b
	test	r14b, r14b
	jg	.LBB9_64
	jmp	.LBB9_67
.LBB9_57:
	mov	r10d, r13d
	sar	r10d, 31
	andn	r10d, r10d, r13d
	test	dl, dl
	je	.LBB9_59
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
	jne	.LBB9_56
	jmp	.LBB9_61
.LBB9_59:
	lea	r15d, [r15 + r10 + 1]
	sub	r15d, r13d
	test	r12b, r12b
	movzx	r14d, byte ptr [rbp + 128]
	jne	.LBB9_56
.LBB9_61:
	dec	ebx
	movsxd	r11, ebx
	xor	ebx, ebx
	movabs	r12, 7378697629483820647
	mov	r10, r11
	.p2align	4
.LBB9_62:
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
	ja	.LBB9_62
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
	jle	.LBB9_67
.LBB9_64:
	mov	eax, r12d
	sub	eax, r15d
	jle	.LBB9_67
	.p2align	4
.LBB9_65:
	movsxd	rdx, dword ptr [rsi]
	cmp	edx, r8d
	jge	.LBB9_72
	lea	r10d, [rdx + 1]
	mov	dword ptr [rsi], r10d
	mov	byte ptr [rcx + rdx], 32
	dec	eax
	jne	.LBB9_65
.LBB9_67:
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
	jns	.LBB9_72
	mov	eax, r12d
	neg	eax
	cmp	r15d, eax
	jge	.LBB9_72
	add	r15d, r12d
	neg	r15d
	.p2align	4
.LBB9_70:
	movsxd	rax, dword ptr [rsi]
	cmp	eax, ebx
	jge	.LBB9_72
	lea	ecx, [rax + 1]
	mov	dword ptr [rsi], ecx
	mov	byte ptr [rdi + rax], 32
	dec	r15d
	jne	.LBB9_70
.LBB9_72:
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

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
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
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	jne	.LBB10_2
	mov	r10d, dword ptr [rdx + 8]
	movzx	r9d, word ptr [rdx + 14]
	cmp	r9w, -1
	je	.LBB10_15
.LBB10_20:
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
.LBB10_2:
	mov	r10, qword ptr [r8 + 112]
	mov	r11d, dword ptr [r8 + 168]
	movsx	r9d, word ptr [rdx + 14]
	test	r9d, r9d
	js	.LBB10_4
	cmp	r11d, r9d
	jle	.LBB10_4
	movsxd	rsi, r9d
	cmp	dword ptr [r10 + 4*rsi], ecx
	je	.LBB10_12
.LBB10_4:
	test	r11d, r11d
	je	.LBB10_9
	xor	r9d, r9d
.LBB10_7:
	movsxd	rsi, r9d
	cmp	ecx, dword ptr [r10 + 4*rsi]
	je	.LBB10_8
	inc	r9d
	cmp	r11d, r9d
	jne	.LBB10_7
.LBB10_9:
	mov	word ptr [rdx + 14], -1
.LBB10_10:
	mov	dword ptr [rdx + 8], 0
	xor	r10d, r10d
	mov	r9w, -1
	xor	r11d, r11d
	jmp	.LBB10_13
.LBB10_8:
	mov	word ptr [rdx + 14], r9w
	cmp	r9w, -1
	je	.LBB10_10
.LBB10_12:
	mov	r10, qword ptr [r8 + 224]
	movsx	r11, r9w
	mov	r10d, dword ptr [r10 + 4*r11]
	mov	dword ptr [rdx + 8], r10d
	mov	rsi, qword ptr [r8 + 232]
	movzx	r11d, word ptr [rsi + 2*r11]
.LBB10_13:
	mov	word ptr [rdx + 12], r11w
	mov	qword ptr [rdx], r8
	cmp	r9w, -1
	jne	.LBB10_20
.LBB10_15:
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
	je	.LBB10_16
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
	jne	.LBB10_18
	jmp	.LBB10_19
.LBB10_16:
	mov	ebx, 19
.LBB10_18:
	mov	eax, 169
	sub	eax, ebx
	cmp	eax, 24
	mov	esi, 24
	cmovb	esi, eax
	mov	ecx, ebx
	add	rcx, rdi
	lea	rdx, [rip + "Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint>(Unity.Entities.ArchetypeChunk* this, ref Unity.Entities.ComponentTypeHandle`1<Latios.Kinemation.LodGroupReferencePoint> typeHandle) -> void*_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0082.1"+2]
	mov	r8, rsi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, ebx
.LBB10_19:
	mov	eax, esi
	mov	byte ptr [rbp + rax + 162], 0
	lea	rdx, [rbp + 162]
	mov	word ptr [rdi - 2], si
	lea	rcx, [rip + burst.string.1750a84a9e73678fca2e711b84b9a69d]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	jne	.LBB11_2
	mov	r10d, dword ptr [rdx + 8]
	movzx	r9d, word ptr [rdx + 14]
	cmp	r9w, -1
	je	.LBB11_15
.LBB11_20:
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
.LBB11_2:
	mov	r10, qword ptr [r8 + 112]
	mov	r11d, dword ptr [r8 + 168]
	movsx	r9d, word ptr [rdx + 14]
	test	r9d, r9d
	js	.LBB11_4
	cmp	r11d, r9d
	jle	.LBB11_4
	movsxd	rsi, r9d
	cmp	dword ptr [r10 + 4*rsi], ecx
	je	.LBB11_12
.LBB11_4:
	test	r11d, r11d
	je	.LBB11_9
	xor	r9d, r9d
.LBB11_7:
	movsxd	rsi, r9d
	cmp	ecx, dword ptr [r10 + 4*rsi]
	je	.LBB11_8
	inc	r9d
	cmp	r11d, r9d
	jne	.LBB11_7
.LBB11_9:
	mov	word ptr [rdx + 14], -1
.LBB11_10:
	mov	dword ptr [rdx + 8], 0
	xor	r10d, r10d
	mov	r9w, -1
	xor	r11d, r11d
	jmp	.LBB11_13
.LBB11_8:
	mov	word ptr [rdx + 14], r9w
	cmp	r9w, -1
	je	.LBB11_10
.LBB11_12:
	mov	r10, qword ptr [r8 + 224]
	movsx	r11, r9w
	mov	r10d, dword ptr [r10 + 4*r11]
	mov	dword ptr [rdx + 8], r10d
	mov	rsi, qword ptr [r8 + 232]
	movzx	r11d, word ptr [rsi + 2*r11]
.LBB11_13:
	mov	word ptr [rdx + 12], r11w
	mov	qword ptr [rdx], r8
	cmp	r9w, -1
	jne	.LBB11_20
.LBB11_15:
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
	je	.LBB11_16
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
	jne	.LBB11_18
	jmp	.LBB11_19
.LBB11_16:
	mov	ebx, 19
.LBB11_18:
	mov	eax, 169
	sub	eax, ebx
	cmp	eax, 24
	mov	esi, 24
	cmovb	esi, eax
	mov	ecx, ebx
	add	rcx, rdi
	lea	rdx, [rip + "Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint>(Unity.Entities.ArchetypeChunk* this, ref Unity.Entities.ComponentTypeHandle`1<Latios.Kinemation.LodGroupReferencePoint> typeHandle) -> void*_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0082.1"+2]
	mov	r8, rsi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, ebx
.LBB11_19:
	mov	eax, esi
	mov	byte ptr [rbp + rax + 162], 0
	lea	rdx, [rbp + 162]
	mov	word ptr [rdi - 2], si
	lea	rcx, [rip + burst.string.1750a84a9e73678fca2e711b84b9a69d]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
	.seh_endproc

	.def	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	jne	.LBB12_2
	movzx	esi, word ptr [rdx + 14]
.LBB12_14:
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
	je	.LBB12_24
	cmp	word ptr [rdx + 14], -1
	je	.LBB12_18
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
	jne	.LBB12_23
.LBB12_17:
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
.LBB12_2:
	mov	edi, dword ptr [rdx + 16]
	mov	rbx, qword ptr [r10 + 112]
	mov	r14d, dword ptr [r10 + 168]
	movsx	esi, word ptr [rdx + 14]
	test	esi, esi
	js	.LBB12_4
	cmp	r14d, esi
	jle	.LBB12_4
	movsxd	r15, esi
	cmp	dword ptr [rbx + 4*r15], edi
	je	.LBB12_12
.LBB12_4:
	test	r14d, r14d
	je	.LBB12_9
	xor	esi, esi
.LBB12_7:
	movsxd	r15, esi
	cmp	edi, dword ptr [rbx + 4*r15]
	je	.LBB12_8
	inc	esi
	cmp	r14d, esi
	jne	.LBB12_7
.LBB12_9:
	mov	word ptr [rdx + 14], -1
.LBB12_10:
	mov	dword ptr [rdx + 8], 0
	mov	si, -1
	xor	edi, edi
	jmp	.LBB12_13
.LBB12_24:
	lea	rcx, [rip + burst.string.1ea13c7f7ea144ea32205d7ec317f39f]
	lea	rdx, [rip + burst.string.989b224dc613a391593dc022965ef313]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB12_18:
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
	je	.LBB12_19
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
	jne	.LBB12_21
	jmp	.LBB12_22
.LBB12_23:
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
	jmp	.LBB12_17
.LBB12_19:
	mov	ebx, 19
.LBB12_21:
	mov	eax, 169
	sub	eax, ebx
	cmp	eax, 24
	mov	esi, 24
	cmovb	esi, eax
	mov	ecx, ebx
	add	rcx, rdi
	lea	rdx, [rip + "Unity.Entities.ArchetypeChunk.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint>(Unity.Entities.ArchetypeChunk* this, ref Unity.Entities.ComponentTypeHandle`1<Latios.Kinemation.LodGroupReferencePoint> typeHandle) -> void*_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0082.1"+2]
	mov	r8, rsi
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	add	esi, ebx
.LBB12_22:
	mov	eax, esi
	mov	byte ptr [rbp + rax + 178], 0
	lea	rdx, [rbp + 178]
	mov	word ptr [rdi - 2], si
	lea	rcx, [rip + burst.string.1750a84a9e73678fca2e711b84b9a69d]
	call	qword ptr [rip + burst_abort_Ptr]
	int3
.LBB12_8:
	mov	word ptr [rdx + 14], si
	cmp	si, -1
	je	.LBB12_10
.LBB12_12:
	mov	rdi, qword ptr [r10 + 224]
	movsx	rbx, si
	mov	edi, dword ptr [rdi + 4*rbx]
	mov	dword ptr [rdx + 8], edi
	mov	rdi, qword ptr [r10 + 232]
	movzx	edi, word ptr [rdi + 2*rbx]
.LBB12_13:
	mov	word ptr [rdx + 12], di
	mov	qword ptr [rdx], r10
	mov	rdi, qword ptr [r8]
	jmp	.LBB12_14
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
	je	.LBB14_15
	mov	rbx, qword ptr [rdx + 72]
	test	rbx, rbx
	je	.LBB14_2
	cmp	qword ptr [rbx], r8
	je	.LBB14_13
.LBB14_2:
	mov	rbx, qword ptr [rdx + 64]
	test	rbx, rbx
	je	.LBB14_7
	.p2align	4
.LBB14_4:
	cmp	qword ptr [rbx], r8
	je	.LBB14_12
	mov	rbx, qword ptr [rbx + 16]
	test	rbx, rbx
	jne	.LBB14_4
.LBB14_7:
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
	je	.LBB14_8
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
	je	.LBB14_11
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
	jne	.LBB14_17
	jmp	.LBB14_15
.LBB14_12:
	mov	qword ptr [rdx + 72], rbx
.LBB14_13:
	add	rbx, 8
	mov	r10, qword ptr [rbx]
	test	r10, r10
	je	.LBB14_15
.LBB14_17:
	mov	ecx, dword ptr [r10 + 28]
	test	ecx, ecx
	je	.LBB14_15
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
	js	.LBB14_20
	cmp	edi, ecx
	jge	.LBB14_20
	xor	ebx, ebx
	cmp	edi, r11d
	cmovge	ebx, r10d
	add	edi, r8d
	sub	edi, ebx
	movsxd	rdi, edi
	shl	rdi, 2
	lea	rdi, [rdi + 4*rdi]
	cmp	dword ptr [rdx + rdi], r9d
	je	.LBB14_24
.LBB14_20:
	dec	ecx
	.p2align	4
.LBB14_21:
	test	ecx, ecx
	js	.LBB14_15
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
	jne	.LBB14_21
.LBB14_24:
	mov	ecx, dword ptr [rdx + rdi + 4]
	mov	r8d, dword ptr [rdx + rdi + 8]
	movzx	r9d, word ptr [rdx + rdi + 12]
	movzx	r10d, word ptr [rdx + rdi + 14]
	mov	edx, dword ptr [rdx + rdi + 16]
	jmp	.LBB14_16
.LBB14_15:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	jmp	.LBB14_16
.LBB14_11:
	mov	qword ptr [rsi + 8], 0
	mov	rcx, rsi
	mov	edx, 4
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB14_8:
	xor	ecx, ecx
	xor	r8d, r8d
	xor	r9d, r9d
	xor	r10d, r10d
	xor	edx, edx
	mov	rax, rdi
.LBB14_16:
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
	jne	.LBB15_13
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rax, qword ptr [rax]
	mov	ecx, edx
	cmp	edx, 9
	jae	.LBB15_3
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB15_11
.LBB15_3:
	cmp	edx, 33
	jae	.LBB15_5
	xor	edx, edx
	xor	r12d, r12d
	jmp	.LBB15_8
.LBB15_5:
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
.LBB15_6:
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
	jne	.LBB15_6
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
	jb	.LBB15_11
.LBB15_8:
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
.LBB15_9:
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
	jne	.LBB15_9
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
.LBB15_11:
	sub	rcx, rdx
	shl	rdx, 4
	add	rdx, r13
	.p2align	4
.LBB15_12:
	movsxd	r8, dword ptr [rdx]
	shl	r8, 4
	add	r12d, dword ptr [rax + r8 + 8]
	add	rdx, 16
	dec	rcx
	jne	.LBB15_12
.LBB15_13:
	mov	edx, dword ptr [rbp + 288]
	mov	ecx, 1
	.p2align	4
.LBB15_14:
	xor	eax, eax
	lock		cmpxchg	dword ptr [rsi + 80], ecx
	je	.LBB15_15
.LBB15_17:
	mov	eax, dword ptr [rsi + 80]
	cmp	eax, 1
	je	.LBB15_17
	jmp	.LBB15_14
.LBB15_15:
	mov	dword ptr [rsp + 32], edx
	mov	rcx, rsi
	mov	edx, edi
	mov	r8d, r12d
	mov	ebx, dword ptr [rbp + 272]
	mov	r9d, ebx
	vzeroupper
	call	"Unity.Entities.EntitiesJournaling+JournalingState, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.PushBackRecord(Unity.Entities.EntitiesJournaling+JournalingState*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.EntitiesJournaling+RecordType, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null recordType, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 entityCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 typeCount, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 dataLength) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	test	al, 1
	je	.LBB15_16
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
	jle	.LBB15_19
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackHeader(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, ulong worldSequenceNumber, ref Unity.Entities.SystemHandle executingSystem, ref Unity.Entities.SystemHandle originSystem, int entityCount, int typeCount, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_003E"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	ebx, dword ptr [rbp + 288]
	mov	r15d, dword ptr [rbp + 256]
	cmp	byte ptr [rbp + 24], 0
	jne	.LBB15_25
	jmp	.LBB15_34
.LBB15_19:
	mov	eax, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	eax, ecx
	cmovg	ebx, eax
	sub	ebx, ecx
	add	rcx, qword ptr [rsi + 40]
	cmp	ebx, 71
	mov	r15d, dword ptr [rbp + 256]
	jg	.LBB15_21
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
	jmp	.LBB15_22
.LBB15_21:
	vmovups	ymm0, ymmword ptr [rbp - 72]
	vmovups	ymmword ptr [rcx], ymm0
	vmovdqu	ymm0, ymmword ptr [rbp - 40]
	vmovdqu	ymmword ptr [rcx + 32], ymm0
	mov	rax, qword ptr [rbp - 8]
	mov	qword ptr [rcx + 64], rax
.LBB15_22:
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
	je	.LBB15_34
.LBB15_25:
	mov	r14, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	edi, r15d
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
	mov	qword ptr [rbp + 16], r14
	jmp	.LBB15_26
	.p2align	4
.LBB15_49:
	mov	dword ptr [rsp + 32], 0
	xor	ecx, ecx
	mov	edx, 2
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackEntities(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.ArchetypeChunk* chunks, int chunkCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0065"+2]
	xor	r9d, r9d
	vzeroupper
	call	r14
	mov	r10, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	r11, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/ChunkStore> Unity.Entities.EntityComponentStore::s_chunkStore"]
.LBB15_33:
	add	r13, 16
	dec	rdi
	je	.LBB15_34
.LBB15_26:
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
	je	.LBB15_50
	mov	r15d, dword ptr [rax + rcx + 8]
	shl	r15d, 3
	test	r15d, r15d
	jle	.LBB15_33
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r15d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jg	.LBB15_49
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
	jle	.LBB15_31
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
	jmp	.LBB15_32
.LBB15_31:
	mov	r8d, r15d
.LBB15_32:
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
	jmp	.LBB15_33
.LBB15_34:
	mov	rdi, qword ptr [rbp + 280]
	mov	r14, qword ptr [rbp + 264]
	test	r14, r14
	setne	al
	mov	r12d, dword ptr [rbp + 272]
	test	r12d, r12d
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB15_42
	shl	r12d, 2
	test	r12d, r12d
	jle	.LBB15_42
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, r12d
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB15_37
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackTypes(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.TypeIndex* types, int typeCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0020"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB15_42
.LBB15_37:
	mov	r8d, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	r8d, ecx
	cmovg	eax, r8d
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r15d, r12d
	sub	r15d, eax
	jle	.LBB15_39
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
	jmp	.LBB15_40
.LBB15_39:
	mov	r8d, r12d
.LBB15_40:
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
.LBB15_42:
	test	rdi, rdi
	setne	al
	test	ebx, ebx
	setg	cl
	and	cl, al
	cmp	cl, 1
	jne	.LBB15_16
	mov	ecx, dword ptr [rsi + 60]
	add	ecx, ebx
	mov	eax, dword ptr [rsi + 56]
	cmp	ecx, eax
	jle	.LBB15_44
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackData(Unity.Entities.EntitiesJournaling.JournalingState* this, void* data, int dataLength) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0019"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB15_16
.LBB15_44:
	mov	edx, dword ptr [rsi + 48]
	movsxd	rcx, dword ptr [rsi + 52]
	cmp	edx, ecx
	cmovg	eax, edx
	sub	eax, ecx
	add	rcx, qword ptr [rsi + 40]
	mov	r14d, ebx
	sub	r14d, eax
	jle	.LBB15_46
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
	jmp	.LBB15_47
.LBB15_46:
	mov	r8d, ebx
.LBB15_47:
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
.LBB15_16:
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
.LBB15_50:
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
	jle	.LBB16_1
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
	jmp	.LBB16_23
.LBB16_1:
	jne	.LBB16_2
	mov	dword ptr [rcx + 28], 0
	mov	qword ptr [rcx + 16], 0
	mov	dword ptr [rcx + 60], 0
	mov	qword ptr [rcx + 48], 0
	xor	esi, esi
	xor	r9d, r9d
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	jne	.LBB16_6
	jmp	.LBB16_4
.LBB16_2:
	mov	esi, dword ptr [rcx + 28]
	mov	r9d, dword ptr [rcx + 52]
	mov	edi, dword ptr [rcx + 24]
	cmp	esi, edi
	je	.LBB16_4
.LBB16_6:
	mov	eax, r11d
	sub	eax, dword ptr [rcx + 60]
	cmp	eax, r8d
	jge	.LBB16_7
.LBB16_4:
	mov	dword ptr [rbp + 92], r9d
	test	esi, esi
	jle	.LBB16_5
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
.LBB16_18:
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
	jge	.LBB16_13
	cmp	esi, eax
	jne	.LBB16_18
	mov	eax, esi
	jmp	.LBB16_13
.LBB16_5:
	xor	eax, eax
	xor	r9d, r9d
.LBB16_13:
	test	esi, esi
	setne	dl
	sub	esi, eax
	setge	bl
	test	dl, bl
	je	.LBB16_19
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
	je	.LBB16_24
	sub	ebx, r9d
	jl	.LBB16_24
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
.LBB16_7:
	cmp	esi, edi
	jne	.LBB16_8
	mov	dword ptr [rsp + 32], 0
	lea	r8, [rip + "Unity.Entities.EntitiesJournaling.JournalingState.PushBackRecord(Unity.Entities.EntitiesJournaling.JournalingState* this, Unity.Entities.EntitiesJournaling.RecordType recordType, int entityCount, int typeCount, int dataLength) -> bool_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0153"+2]
	xor	ecx, ecx
	mov	edx, 2
	xor	r9d, r9d
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	jmp	.LBB16_9
.LBB16_19:
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
	jle	.LBB16_22
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
	jmp	.LBB16_21
.LBB16_8:
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
.LBB16_9:
	mov	dil, 1
	jmp	.LBB16_10
.LBB16_24:
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
	jle	.LBB16_22
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
.LBB16_21:
	mov	ebx, edi
.LBB16_22:
	movsxd	rax, ebx
	mov	byte ptr [rbp + rax - 78], 0
	mov	word ptr [rsi - 2], ax
	mov	dword ptr [rsp + 32], 0
	xor	edi, edi
	xor	ecx, ecx
	mov	edx, 2
	mov	r8, rsi
.LBB16_23:
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
.LBB16_10:
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
	je	.LBB17_3
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
	jne	.LBB17_2
	mov	r15d, dword ptr [rsi]
	mov	r12, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.TypeIndex> Unity.Entities.EntitiesJournaling/SharedEntityTypeIndex::Ref"]
	cmp	r15d, dword ptr [r12]
	jne	.LBB17_5
	add	rsi, 4
	dec	ebx
	jmp	.LBB17_2
.LBB17_5:
	mov	ebx, ecx
.LBB17_2:
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
.LBB17_3:
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

	.def	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@ffffffffffffffffffffffffffffffff
	.section	.rdata,"dr",discard,__xmm@ffffffffffffffffffffffffffffffff
	.p2align	4, 0x0
__xmm@ffffffffffffffffffffffffffffffff:
	.zero	16,255
	.section	.text,"xr",discard,"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Unity.Entities.JobChunkExtensions+JobChunkProducer`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ExecuteInternal(Unity.Entities.JobChunkExtensions+JobChunkWrapper`1[[Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null jobWrapper, System.IntPtr, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 bufferRangePatchData, Unity.Jobs.LowLevel.Unsafe.JobRanges&, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null ranges, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 jobIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
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
	sub	rsp, 392
	.seh_stackalloc 392
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovaps	xmmword ptr [rbp + 240], xmm7
	.seh_savexmm xmm7, 368
	vmovdqa	xmmword ptr [rbp + 224], xmm6
	.seh_savexmm xmm6, 352
	.seh_endprologue
	mov	dword ptr [rbp + 156], r9d
	mov	qword ptr [rbp - 72], r8
	mov	dword ptr [rbp + 216], 0
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp + 176], xmm0
	vmovups	xmmword ptr [rbp + 104], xmm0
	mov	r8, qword ptr [rcx + 352]
	mov	r10, qword ptr [rcx + 368]
	mov	rax, qword ptr [rcx + 392]
	mov	qword ptr [rbp + 120], rax
	mov	eax, dword ptr [rcx + 452]
	mov	rdx, qword ptr [rcx + 376]
	mov	r11, qword ptr [rcx + 384]
	mov	r8, qword ptr [r8]
	mov	r9, qword ptr [r10]
	mov	qword ptr [rbp + 128], r10
	mov	edi, dword ptr [r10 + 8]
	mov	r10, qword ptr [rdx]
	mov	r11, qword ptr [r11]
	mov	edx, dword ptr [rcx + 432]
	mov	qword ptr [rbp + 200], rcx
	or	edx, dword ptr [rcx + 412]
	sete	dl
	jne	.LBB18_3
	mov	rcx, qword ptr [rbp + 200]
	cmp	dword ptr [rcx + 444], 0
	je	.LBB18_2
.LBB18_3:
	mov	sil, 1
.LBB18_4:
	test	eax, eax
	mov	dword ptr [rbp - 64], edi
	mov	dword ptr [rbp - 60], 0
	mov	qword ptr [rbp - 56], r9
	mov	rcx, qword ptr [rbp + 120]
	mov	qword ptr [rbp - 48], rcx
	mov	qword ptr [rbp - 40], r10
	mov	qword ptr [rbp - 32], r11
	mov	qword ptr [rbp - 24], r8
	mov	rcx, qword ptr [rbp + 200]
	vmovups	ymm0, ymmword ptr [rcx + 408]
	vmovups	ymmword ptr [rbp - 16], ymm0
	mov	r8, qword ptr [rcx + 440]
	mov	qword ptr [rbp + 16], r8
	mov	byte ptr [rbp + 24], sil
	setne	byte ptr [rbp + 25]
	mov	word ptr [rbp + 26], 0
	mov	dword ptr [rbp + 28], -1
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp + 32], ymm0
	vmovups	ymmword ptr [rbp + 64], ymm0
	mov	qword ptr [rbp + 96], 0
	mov	ecx, dword ptr [rcx + 448]
	mov	dword ptr [rbp + 196], ecx
	mov	byte ptr [rbp + 223], 1
	test	dl, dl
	je	.LBB18_6
	mov	rcx, qword ptr [rbp + 200]
	cmp	dword ptr [rcx + 444], 0
	setne	byte ptr [rbp + 223]
.LBB18_6:
	cmp	dword ptr [rbp + 196], 1
	mov	dword ptr [rbp + 216], 0
	mov	dword ptr [rbp + 212], edi
	jne	.LBB18_7
	jmp	.LBB18_11
.LBB18_2:
	xor	esi, esi
	jmp	.LBB18_4
.LBB18_7:
	test	eax, eax
	setne	al
	or	al, byte ptr [rbp + 223]
	test	al, 1
	je	.LBB18_13
	mov	eax, dword ptr [rbp + 212]
	mov	dword ptr [rbp - 64], eax
	mov	esi, dword ptr [rbp + 216]
	cmp	esi, eax
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp + 176], xmm0
	jge	.LBB18_29
	mov	rax, qword ptr [rbp + 200]
	lea	r15, [rax + 160]
	lea	r12, [rbp + 104]
	vpcmpeqd	xmm6, xmm6, xmm6
.LBB18_22:
	movsxd	rsi, esi
	jmp	.LBB18_23
	.p2align	4
.LBB18_45:
	vptest	xmm0, xmm0
	jne	.LBB18_46
.LBB18_28:
	inc	rsi
	movsxd	rax, dword ptr [rbp - 64]
	cmp	rsi, rax
	jge	.LBB18_29
.LBB18_23:
	mov	rax, qword ptr [rbp - 40]
	mov	rcx, qword ptr [rbp - 32]
	movsxd	r13, dword ptr [rcx + 4*rsi]
	mov	eax, dword ptr [rax + 4*rsi]
	cmp	dword ptr [rbp + 28], eax
	jne	.LBB18_24
.LBB18_26:
	cmp	byte ptr [rbp + 24], 0
	je	.LBB18_52
	mov	rcx, qword ptr [rbp + 32]
	mov	edx, r13d
	lea	r8, [rbp - 16]
	vzeroupper
	call	"Unity.Entities.MatchingArchetype, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ChunkMatchesFilter(Unity.Entities.MatchingArchetype*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndex, Unity.Entities.EntityQueryFilter&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null filter) -> System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	test	al, 1
	je	.LBB18_28
.LBB18_52:
	cmp	byte ptr [rbp + 25], 0
	je	.LBB18_53
	mov	rax, qword ptr [rbp + 40]
	mov	rdx, qword ptr [rbp + 48]
	mov	eax, dword ptr [rax + 4*r13]
	mov	r14d, dword ptr [rdx + 12]
	mov	r9d, dword ptr [rdx + 16]
	mov	ecx, dword ptr [rbp + 96]
	imul	ecx, r13d
	movsxd	rcx, ecx
	shl	rcx, 4
	add	rcx, qword ptr [rbp + 88]
	test	r14, r14
	mov	r11d, dword ptr [rdx + 20]
	mov	edx, dword ptr [rdx + 24]
	mov	rbx, qword ptr [rbp + 56]
	mov	r10, qword ptr [rbp + 72]
	mov	r13, qword ptr [rbp + 64]
	mov	r8, qword ptr [rbp + 80]
	vmovdqu	xmmword ptr [rbp + 176], xmm6
	je	.LBB18_31
	vpcmpeqd	xmm0, xmm0, xmm0
	test	r14d, r14d
	jle	.LBB18_41
	.p2align	4
.LBB18_40:
	movsxd	rdi, dword ptr [rbx]
	shl	rdi, 4
	vpand	xmm0, xmm0, xmmword ptr [rcx + rdi]
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	add	rbx, 4
	dec	r14
	jne	.LBB18_40
.LBB18_41:
	test	r11d, r11d
	jle	.LBB18_42
	movsxd	rdi, dword ptr [r13]
	shl	rdi, 4
	vmovdqu	xmm1, xmmword ptr [rcx + rdi]
	cmp	r11d, 1
	je	.LBB18_50
	dec	r11
	add	r13, 4
	.p2align	4
.LBB18_49:
	movsxd	rdi, dword ptr [r13]
	shl	rdi, 4
	vpor	xmm1, xmm1, xmmword ptr [rcx + rdi]
	add	r13, 4
	dec	r11
	jne	.LBB18_49
.LBB18_50:
	vpand	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 176], xmm0
.LBB18_42:
	test	r9d, r9d
	jle	.LBB18_43
	.p2align	4
.LBB18_51:
	movsxd	r11, dword ptr [r10]
	shl	r11, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r11]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	add	r10, 4
	dec	r9
	jne	.LBB18_51
.LBB18_43:
	test	edx, edx
	jle	.LBB18_45
	.p2align	4
.LBB18_44:
	movsxd	r9, dword ptr [r8]
	shl	r9, 4
	vmovdqu	xmm1, xmmword ptr [rcx + r9]
	vpandn	xmm0, xmm1, xmm0
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	add	r8, 4
	dec	rdx
	jne	.LBB18_44
	jmp	.LBB18_45
.LBB18_24:
	movsxd	rcx, eax
	mov	dword ptr [rbp + 28], eax
	mov	rax, qword ptr [rbp - 24]
	mov	r11, qword ptr [rax + 8*rcx]
	mov	qword ptr [rbp + 32], r11
	mov	rcx, qword ptr [r11]
	mov	edx, dword ptr [rcx + 8]
	shl	edx, 2
	movsxd	rdx, edx
	mov	r8, qword ptr [rcx]
	add	r8, rdx
	imul	edx, dword ptr [rcx + 20]
	movsxd	rcx, edx
	add	rcx, r8
	mov	qword ptr [rbp + 40], rcx
	cmp	byte ptr [rbp + 25], 0
	je	.LBB18_26
	mov	rdi, qword ptr [r11]
	mov	r14d, dword ptr [rdi + 8]
	mov	r8d, dword ptr [rdi + 20]
	movsxd	rdx, dword ptr [r11 + 8]
	lea	rcx, [r11 + 4*rdx]
	lea	rax, [r11 + 4*rdx + 28]
	mov	qword ptr [rbp + 160], rax
	movsxd	r9, dword ptr [r11 + 12]
	lea	rax, [rcx + 4*r9 + 28]
	mov	qword ptr [rbp + 168], rax
	movsxd	r9, dword ptr [r11 + 16]
	lea	r9, [rax + 4*r9]
	movsxd	r10, dword ptr [r11 + 20]
	lea	r10, [r9 + 4*r10]
	lea	ebx, [4*r14]
	mov	rdx, r14
	movsxd	r14, ebx
	mov	ecx, r8d
	imul	ecx, r14d
	movsxd	rcx, ecx
	mov	eax, dword ptr [rdi + 16]
	imul	eax, r14d
	add	r14, r14
	movsxd	rbx, eax
	lea	eax, [rbx + 8*rdx]
	add	eax, ecx
	and	eax, 12
	add	r14, qword ptr [rdi]
	add	rbx, rcx
	add	rbx, r14
	sub	rbx, rax
	add	rbx, 16
	shl	r8d, 4
	sar	r8d, 4
	mov	qword ptr [rbp + 48], r11
	mov	rax, qword ptr [rbp + 160]
	mov	qword ptr [rbp + 56], rax
	mov	qword ptr [rbp + 64], r9
	mov	rax, qword ptr [rbp + 168]
	mov	qword ptr [rbp + 72], rax
	mov	qword ptr [rbp + 80], r10
	mov	qword ptr [rbp + 88], rbx
	mov	dword ptr [rbp + 96], r8d
	mov	dword ptr [rbp + 100], 0
	jmp	.LBB18_26
.LBB18_31:
	mov	ebx, 128
	sub	ebx, eax
	cmp	ebx, 127
	jg	.LBB18_32
	cmp	eax, 128
	je	.LBB18_34
	cmp	ebx, 63
	mov	rdi, -1
	shrx	rbx, rdi, rbx
	jg	.LBB18_37
	shlx	r14, rdi, rax
	or	r14, rbx
	vmovq	xmm0, r14
	vmovq	xmm1, rbx
	vpunpcklqdq	xmm0, xmm0, xmm1
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	jmp	.LBB18_41
.LBB18_37:
	vmovq	xmm0, rbx
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	jmp	.LBB18_41
.LBB18_32:
	vpxor	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	jmp	.LBB18_41
.LBB18_34:
	vpcmpeqd	xmm0, xmm0, xmm0
	vmovdqu	xmmword ptr [rbp + 176], xmm0
	jmp	.LBB18_41
	.p2align	4
.LBB18_53:
	xor	r9d, r9d
	jmp	.LBB18_21
	.p2align	4
.LBB18_46:
	vmovq	rcx, xmm0
	vpextrq	rdx, xmm0, 1
	popcnt	rcx, rcx
	popcnt	rdx, rdx
	add	edx, ecx
	cmp	edx, eax
	setne	r9b
.LBB18_21:
	movsxd	rsi, esi
	mov	rax, qword ptr [rbp - 56]
	mov	rcx, qword ptr [rbp - 48]
	mov	eax, dword ptr [rax + 4*rsi]
	mov	dword ptr [rbp + 104], eax
	mov	qword ptr [rbp + 112], rcx
	lea	rax, [rbp + 176]
	mov	qword ptr [rsp + 32], rax
	mov	rcx, r15
	mov	rdx, r12
	mov	r8d, esi
	vzeroupper
	call	"Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Execute(Latios.Kinemation.Authoring.Systems.LodGroupBakingSystem+Job*, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ArchetypeChunk&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunk, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 chunkIndexInQuery, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 useEnabledMask, Unity.Burst.Intrinsics.v128&, Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null chunkEnabledMask) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	inc	esi
	cmp	esi, dword ptr [rbp - 64]
	jl	.LBB18_22
.LBB18_29:
	mov	dword ptr [rbp + 104], 0
	mov	qword ptr [rbp + 112], 0
	jmp	.LBB18_19
.LBB18_13:
	movsxd	r14, dword ptr [rbp + 216]
	cmp	r14d, dword ptr [rbp + 212]
	jge	.LBB18_19
	mov	rax, qword ptr [rbp + 128]
	mov	rax, qword ptr [rax]
	mov	qword ptr [rbp + 160], rax
	mov	rax, qword ptr [rbp + 200]
	lea	rcx, [rax + 160]
	mov	qword ptr [rbp + 168], rcx
	lea	rcx, [rax + 224]
	mov	qword ptr [rbp - 88], rcx
	add	rax, 288
	mov	qword ptr [rbp - 80], rax
	lea	rsi, [rbp + 136]
	vmovaps	xmm6, xmmword ptr [rip + __xmm@3f800000bf800000bf800000bf800000]
	vbroadcastss	xmm7, dword ptr [rip + __real@3f800000]
	mov	r13, qword ptr [rbp + 120]
	jmp	.LBB18_18
	.p2align	4
.LBB18_17:
	inc	r14
	movsxd	rax, dword ptr [rbp + 212]
	cmp	r14, rax
	jge	.LBB18_19
.LBB18_18:
	mov	rax, qword ptr [rbp + 160]
	movsxd	r15, dword ptr [rax + 4*r14]
	mov	r12, r15
	shl	r12, 4
	mov	dword ptr [rbp + 136], r15d
	mov	qword ptr [rbp + 144], r13
	mov	rcx, rsi
	mov	rdx, qword ptr [rbp + 168]
	vzeroupper
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRW<Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.LodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rdi, rax
	mov	dword ptr [rbp + 136], r15d
	mov	qword ptr [rbp + 144], r13
	mov	rcx, rsi
	mov	rdx, qword ptr [rbp - 88]
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Kinemation.Authoring.BakingLodGroupReferencePoint, Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rbx, rax
	mov	dword ptr [rbp + 136], r15d
	mov	qword ptr [rbp + 144], r13
	mov	rcx, rsi
	mov	rdx, qword ptr [rbp - 80]
	call	"Unity.Entities.ArchetypeChunk, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.GetRequiredComponentDataPtrRO<Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Entities.ArchetypeChunk*, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Entities.ComponentTypeHandle`1[[Latios.Transforms.WorldTransform, Latios.Transforms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null typeHandle) -> System.Void*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Entities.EntityComponentStore/PerChunkArray> Unity.Entities.EntityComponentStore/PerChunkArray/StaticIdentifier::Ref"]
	mov	rcx, qword ptr [rcx]
	movsxd	rcx, dword ptr [rcx + r12 + 8]
	test	rcx, rcx
	jle	.LBB18_17
	shl	rcx, 2
	lea	rcx, [rcx + 2*rcx]
	xor	edx, edx
	.p2align	4
.LBB18_16:
	vmovsd	xmm0, qword ptr [rbx + rdx]
	vinsertps	xmm0, xmm0, dword ptr [rbx + rdx + 8], 32
	vmovsd	xmm1, qword ptr [rax + 4*rdx + 16]
	vinsertps	xmm1, xmm1, dword ptr [rax + 4*rdx + 24], 32
	vsubps	xmm0, xmm0, xmm1
	vmulps	xmm1, xmm6, xmmword ptr [rax + 4*rdx]
	vshufps	xmm2, xmm0, xmm0, 201
	vmulps	xmm2, xmm2, xmm1
	vshufps	xmm3, xmm1, xmm1, 201
	vmulps	xmm4, xmm0, xmm3
	vsubps	xmm2, xmm2, xmm4
	vaddps	xmm2, xmm2, xmm2
	vshufps	xmm4, xmm2, xmm2, 201
	vshufps	xmm5, xmm1, xmm1, 255
	vmulps	xmm5, xmm5, xmm4
	vaddps	xmm0, xmm0, xmm5
	vshufps	xmm2, xmm2, xmm2, 210
	vmulps	xmm1, xmm1, xmm2
	vmulps	xmm2, xmm3, xmm4
	vsubps	xmm1, xmm1, xmm2
	vshufps	xmm1, xmm1, xmm1, 201
	vaddps	xmm0, xmm0, xmm1
	vdivps	xmm1, xmm7, xmmword ptr [rax + 4*rdx + 32]
	vmulps	xmm0, xmm1, xmm0
	vshufps	xmm1, xmm1, xmm1, 255
	vmulps	xmm0, xmm1, xmm0
	vmovlps	qword ptr [rdi + rdx], xmm0
	vextractps	dword ptr [rdi + rdx + 8], xmm0, 2
	add	rdx, 12
	cmp	rcx, rdx
	jne	.LBB18_16
	jmp	.LBB18_17
.LBB18_19:
	cmp	dword ptr [rbp + 196], 1
	jne	.LBB18_20
	mov	dword ptr [rbp + 216], 0
	mov	rax, qword ptr [rbp + 128]
	mov	eax, dword ptr [rax + 8]
	mov	dword ptr [rbp + 212], eax
.LBB18_11:
	lea	r8, [rbp + 216]
	lea	r9, [rbp + 212]
	mov	rcx, qword ptr [rbp - 72]
	mov	edx, dword ptr [rbp + 156]
	vzeroupper
	call	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	test	al, al
	je	.LBB18_20
	mov	rax, qword ptr [rbp + 200]
	mov	eax, dword ptr [rax + 452]
	jmp	.LBB18_7
.LBB18_20:
	vmovaps	xmm6, xmmword ptr [rbp + 224]
	vmovaps	xmm7, xmmword ptr [rbp + 240]
	.seh_startepilogue
	add	rsp, 392
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
	jle	.LBB19_5
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
.LBB19_2:
	movsxd	rbx, dword ptr [r8 + 4*rdi + 8]
	mov	ebx, dword ptr [rcx + 4*rbx + 28]
	sub	ebx, edx
	imul	ebx, r9d
	movsxd	rbx, ebx
	mov	ebx, dword ptr [rsi + 4*rbx]
	cmp	ebx, dword ptr [r8 + 4*rdi + 16]
	jne	.LBB19_3
	inc	rdi
	cmp	edi, eax
	jl	.LBB19_2
.LBB19_5:
	mov	edx, dword ptr [r8 + 24]
	mov	eax, dword ptr [r8 + 36]
	test	rdx, rdx
	je	.LBB19_8
	test	eax, eax
	jne	.LBB19_10
	xor	eax, eax
	test	edx, edx
	jg	.LBB19_13
.LBB19_12:
	xor	ebx, ebx
	jmp	.LBB19_31
.LBB19_3:
	xor	eax, eax
	jmp	.LBB19_32
.LBB19_8:
	test	eax, eax
	je	.LBB19_9
.LBB19_10:
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
	jle	.LBB19_12
.LBB19_13:
	lea	esi, [4*r9]
	movsxd	rsi, esi
	add	rsi, r10
	mov	qword ptr [rbp - 104], r8
	mov	r15d, dword ptr [r8]
	lea	r11, [rsi + 4*r11]
	cmp	edx, 8
	jae	.LBB19_15
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB19_29
.LBB19_15:
	cmp	edx, 31
	ja	.LBB19_17
	vmovd	xmm0, r15d
	vpbroadcastd	ymm0, xmm0
	xor	edi, edi
	xor	ebx, ebx
	jmp	.LBB19_24
.LBB19_9:
	mov	al, 1
	jmp	.LBB19_32
.LBB19_17:
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
	jmp	.LBB19_18
	.p2align	4
.LBB19_20:
	vpor	xmm5, xmm9, xmm5
	vpor	xmm4, xmm8, xmm4
	vpor	xmm3, xmm3, xmm7
	vpor	xmm1, xmm1, xmm6
	sub	rdi, -128
	add	rbx, -32
	je	.LBB19_21
.LBB19_18:
	test	r15d, r15d
	vpcmpeqd	xmm6, xmm6, xmm6
	vpcmpeqd	xmm7, xmm7, xmm7
	vpcmpeqd	xmm8, xmm8, xmm8
	vpcmpeqd	xmm9, xmm9, xmm9
	je	.LBB19_20
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
	jmp	.LBB19_20
.LBB19_21:
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
	jne	.LBB19_23
	mov	eax, r10d
	jmp	.LBB19_31
.LBB19_23:
	test	dl, 24
	mov	eax, r10d
	je	.LBB19_29
.LBB19_24:
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
	jmp	.LBB19_25
	.p2align	4
.LBB19_27:
	vpor	xmm1, xmm1, xmm3
	add	rbx, 32
	add	rdi, 8
	je	.LBB19_28
.LBB19_25:
	test	r15d, r15d
	vpcmpeqd	xmm3, xmm3, xmm3
	je	.LBB19_27
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
	jmp	.LBB19_27
.LBB19_28:
	vptest	xmm1, xmmword ptr [rip + __xmm@00010001000100010001000100010001]
	setne	bl
	mov	rdx, qword ptr [rbp - 128]
	mov	rdi, qword ptr [rbp - 120]
	cmp	edi, edx
	mov	eax, dword ptr [rbp - 108]
	je	.LBB19_31
.LBB19_29:
	sub	rdx, rdi
	mov	r8, qword ptr [rbp - 104]
	lea	r8, [r8 + 4*rdi + 28]
	mov	esi, ebx
	.p2align	4
.LBB19_30:
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
	jne	.LBB19_30
.LBB19_31:
	or	al, bl
.LBB19_32:
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
	jne	.LBB20_2
	lea	rcx, [rbp - 94]
	movzx	edx, word ptr [rbp - 96]
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle::NewStaticSafetyId_Ptr"]
	mov	dword ptr [r14], eax
.LBB20_2:
	and	ebx, 8191
	lea	rdx, [rbx + 2*rbx]
	mov	eax, dword ptr [rdi + 8*rdx + 8]
	dec	eax
	cmp	eax, 125
	ja	.LBB20_6
	lea	rcx, [rbp + 930]
	mov	rdx, qword ptr [rdi + 8*rdx]
	test	eax, eax
	mov	r8d, eax
	je	.LBB20_5
	mov	word ptr [rbp + 928], ax
	mov	byte ptr [rbp + r8 + 930], 0
.LBB20_5:
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
.LBB20_6:
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
	jb	.LBB21_1
	xor	r9d, r9d
	.p2align	4
.LBB21_3:
	lea	rax, [r9 + 32]
	vmovups	ymm0, ymmword ptr [rdx + r9]
	vmovups	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB21_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB21_6
	.p2align	4
.LBB21_5:
	lea	r9, [rax + 1]
	movzx	r10d, byte ptr [rdx + rax]
	mov	byte ptr [rcx + rax], r10b
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB21_5
.LBB21_6:
	vzeroupper
	ret
.LBB21_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB21_5
	jmp	.LBB21_6

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
	jb	.LBB22_3
	xor	eax, eax
	vmovd	xmm0, edx
	vpbroadcastb	ymm0, xmm0
	.p2align	4
.LBB22_2:
	movsxd	r9, eax
	add	eax, 32
	vmovdqu	ymmword ptr [rcx + r9], ymm0
	add	r9d, 64
	cmp	r8d, r9d
	jae	.LBB22_2
.LBB22_3:
	lea	r9d, [rax + 1]
	cmp	r8d, r9d
	jb	.LBB22_5
	.p2align	4
.LBB22_4:
	movsxd	r9, eax
	inc	eax
	mov	byte ptr [rcx + r9], dl
	add	r9d, 2
	cmp	r8d, r9d
	jae	.LBB22_4
.LBB22_5:
	vzeroupper
	ret

	.section	.drectve,"yni"
	.ascii	" /EXPORT:361bfdbc31942f74f67b67d45a212d8e"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.361bfdbc31942f74f67b67d45a212d8e\""
	.ascii	" /EXPORT:\"burst.initialize.statics.361bfdbc31942f74f67b67d45a212d8e\""
	.globl	_fltused
