
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
	.text
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
	lea	rcx, [rip + burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4]
	call	rsi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"], rax
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

	.def	burst.initialize.externals.ea84a0a47b565153ce5163e4a7898db2;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.ea84a0a47b565153ce5163e4a7898db2
	.p2align	4
burst.initialize.externals.ea84a0a47b565153ce5163e4a7898db2:
.seh_proc burst.initialize.externals.ea84a0a47b565153ce5163e4a7898db2
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
	lea	rcx, [rip + burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4]
	call	rsi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"], rax
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

	.def	burst.initialize.statics.ea84a0a47b565153ce5163e4a7898db2;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.ea84a0a47b565153ce5163e4a7898db2
	.p2align	4
burst.initialize.statics.ea84a0a47b565153ce5163e4a7898db2:
.seh_proc burst.initialize.statics.ea84a0a47b565153ce5163e4a7898db2
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	ea84a0a47b565153ce5163e4a7898db2;
	.scl	2;
	.type	32;
	.endef
	.globl	ea84a0a47b565153ce5163e4a7898db2
	.p2align	4
ea84a0a47b565153ce5163e4a7898db2:
.seh_proc ea84a0a47b565153ce5163e4a7898db2
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
	.seh_endprologue
	mov	rax, r9
	mov	rdi, rcx
	mov	edx, dword ptr [rbp + 144]
	mov	dword ptr [rbp + 36], 0
	mov	dword ptr [rbp + 32], 0
	lea	r8, [rbp + 36]
	lea	r9, [rbp + 32]
	mov	qword ptr [rbp + 8], rax
	mov	rcx, rax
	call	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	test	al, al
	je	.LBB3_14
	mov	rax, qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	mov	qword ptr [rbp + 16], rax
	jmp	.LBB3_3
	.p2align	4
.LBB3_2:
	mov	rcx, qword ptr [rbp + 8]
	mov	edx, dword ptr [rbp + 144]
	lea	r8, [rbp + 36]
	lea	r9, [rbp + 32]
	call	qword ptr [rbp + 16]
	test	al, al
	je	.LBB3_14
.LBB3_3:
	mov	r14d, dword ptr [rbp + 32]
	mov	esi, dword ptr [rbp + 36]
	cmp	esi, r14d
	jge	.LBB3_2
	mov	rax, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::RuntimeLog_Ptr"]
	mov	qword ptr [rbp], rax
	mov	dword ptr [rbp + 28], r14d
	jmp	.LBB3_5
	.p2align	4
.LBB3_12:
	mov	dword ptr [rsp + 32], 0
	xor	ecx, ecx
	xor	edx, edx
	lea	r8, [rip + "Latios.Kinemation.SparseUpload.LatiosThreadedSparseUploader.AddUpload(Latios.Kinemation.SparseUpload.LatiosThreadedSparseUploader* this, void* src, int size, int offsetInBytes, int repeatCount) -> void_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Kinemation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.string.IL_0016"+2]
	xor	r9d, r9d
	vzeroupper
	call	qword ptr [rbp]
	inc	rsi
	cmp	r14d, esi
	jle	.LBB3_2
.LBB3_5:
	mov	rax, qword ptr [rdi + 24]
	mov	rax, qword ptr [rax]
	imul	rcx, rsi, 76
	vmovups	ymm0, ymmword ptr [rax + rcx]
	vmovups	ymmword ptr [rbp - 84], ymm0
	vmovdqu	ymm0, ymmword ptr [rax + rcx + 32]
	vmovdqu	ymmword ptr [rbp - 52], ymm0
	mov	r13, qword ptr [rax + rcx + 64]
	mov	qword ptr [rbp - 20], r13
	mov	r15d, dword ptr [rax + rcx + 72]
	mov	dword ptr [rbp - 12], r15d
	mov	rdx, qword ptr [rdi + 32]
	mov	ecx, dword ptr [rdx + 12]
	cmp	ecx, dword ptr [rdx + 8]
	jge	.LBB3_12
	movsxd	rbx, dword ptr [rbp - 16]
.LBB3_7:
	mov	rdx, qword ptr [rdx]
	movsxd	rax, ecx
	shl	rax, 3
	lea	r9, [rax + 2*rax]
	.p2align	4
.LBB3_8:
	mov	r12, qword ptr [rdx + r9 + 8]
	mov	eax, r12d
	mov	r8, r12
	shr	r8, 32
	add	rax, 32
	mov	r10, r8
	sub	r10, rbx
	cmp	r10, rax
	jl	.LBB3_11
	shl	r10, 32
	mov	r11d, eax
	or	r11, r10
	mov	rax, r12
	lock		cmpxchg	qword ptr [rdx + r9 + 8], r11
	jne	.LBB3_8
	jmp	.LBB3_10
	.p2align	4
.LBB3_11:
	mov	rdx, qword ptr [rdi + 32]
	lea	r8d, [rcx + 1]
	mov	eax, ecx
	lock		cmpxchg	dword ptr [rdx + 12], r8d
	mov	rdx, qword ptr [rdi + 32]
	mov	ecx, eax
	cmp	eax, dword ptr [rdx + 8]
	jl	.LBB3_7
	jmp	.LBB3_12
	.p2align	4
.LBB3_10:
	mov	qword ptr [rbp - 8], r13
	mov	r13, qword ptr [rdx + r9]
	sub	r8d, ebx
	cmp	r15d, 2
	mov	eax, 1
	cmovl	r15d, eax
	mov	r14, rdi
	movsxd	rdi, r8d
	lea	rcx, [r13 + rdi]
	lea	rdx, [rbp - 84]
	mov	r8, rbx
	xor	r9d, r9d
	vzeroupper
	call	burst.memcpy.inline.AVX2.i64@@32
	movsxd	rax, r12d
	vmovd	xmm0, edi
	mov	rdi, r14
	mov	r14d, dword ptr [rbp + 28]
	vmovss	xmm1, dword ptr [rbp - 8]
	vshufps	xmm0, xmm0, xmm1, 17
	vmovups	xmmword ptr [r13 + rax], xmm0
	vmovd	xmm0, r15d
	vmovd	xmm1, ebx
	vpunpckldq	xmm0, xmm1, xmm0
	vpslldq	xmm0, xmm0, 8
	vmovdqu	xmmword ptr [r13 + rax + 16], xmm0
	inc	rsi
	cmp	r14d, esi
	jg	.LBB3_5
	jmp	.LBB3_2
.LBB3_14:
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
	jb	.LBB4_1
	xor	r9d, r9d
	.p2align	4
.LBB4_3:
	lea	rax, [r9 + 32]
	vmovups	ymm0, ymmword ptr [rdx + r9]
	vmovups	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB4_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB4_6
	.p2align	4
.LBB4_5:
	lea	r9, [rax + 1]
	movzx	r10d, byte ptr [rdx + rax]
	mov	byte ptr [rcx + rax], r10b
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB4_5
.LBB4_6:
	vzeroupper
	ret
.LBB4_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB4_5
	jmp	.LBB4_6

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.ea84a0a47b565153ce5163e4a7898db2\""
	.ascii	" /EXPORT:\"burst.initialize.statics.ea84a0a47b565153ce5163e4a7898db2\""
	.ascii	" /EXPORT:ea84a0a47b565153ce5163e4a7898db2"
