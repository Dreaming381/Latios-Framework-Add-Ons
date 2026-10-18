
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@00000000000000004530000043300000
	.section	.rdata,"dr",discard,__xmm@00000000000000004530000043300000
	.p2align	4, 0x0
__xmm@00000000000000004530000043300000:
	.long	1127219200
	.long	1160773632
	.long	0
	.long	0
	.globl	__xmm@45300000000000004330000000000000
	.section	.rdata,"dr",discard,__xmm@45300000000000004330000000000000
	.p2align	4, 0x0
__xmm@45300000000000004330000000000000:
	.quad	0x4330000000000000
	.quad	0x4530000000000000
	.globl	__real@3ff0000000000000
	.section	.rdata,"dr",discard,__real@3ff0000000000000
	.p2align	3, 0x0
__real@3ff0000000000000:
	.quad	0x3ff0000000000000
	.globl	__real@bf46f0068db8bac7
	.section	.rdata,"dr",discard,__real@bf46f0068db8bac7
	.p2align	3, 0x0
__real@bf46f0068db8bac7:
	.quad	0xbf46f0068db8bac7
	.text
	.globl	3d466577b535881ddd547f1bda3cca2d
	.p2align	4
3d466577b535881ddd547f1bda3cca2d:
.seh_proc 3d466577b535881ddd547f1bda3cca2d
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
	sub	rsp, 312
	.seh_stackalloc 312
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovapd	xmmword ptr [rbp + 160], xmm13
	.seh_savexmm xmm13, 288
	vmovapd	xmmword ptr [rbp + 144], xmm12
	.seh_savexmm xmm12, 272
	vmovapd	xmmword ptr [rbp + 128], xmm11
	.seh_savexmm xmm11, 256
	vmovapd	xmmword ptr [rbp + 112], xmm10
	.seh_savexmm xmm10, 240
	vmovapd	xmmword ptr [rbp + 96], xmm9
	.seh_savexmm xmm9, 224
	vmovapd	xmmword ptr [rbp + 80], xmm8
	.seh_savexmm xmm8, 208
	vmovapd	xmmword ptr [rbp + 64], xmm7
	.seh_savexmm xmm7, 192
	vmovdqa	xmmword ptr [rbp + 48], xmm6
	.seh_savexmm xmm6, 176
	.seh_endprologue
	mov	rax, r9
	mov	rdi, rcx
	mov	edx, dword ptr [rbp + 288]
	mov	dword ptr [rbp + 40], 0
	mov	dword ptr [rbp + 36], 0
	lea	r8, [rbp + 40]
	lea	r9, [rbp + 36]
	mov	qword ptr [rbp - 8], rax
	mov	rcx, rax
	call	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	test	al, al
	je	.LBB0_40
	mov	rax, qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	mov	qword ptr [rbp], rax
	vmovq	xmm6, qword ptr [rip + __xmm@00000000000000004530000043300000]
	vmovapd	xmm7, xmmword ptr [rip + __xmm@45300000000000004330000000000000]
	vmovsd	xmm8, qword ptr [rip + __real@3ff0000000000000]
	vmovsd	xmm9, qword ptr [rip + __real@bf46f0068db8bac7]
	jmp	.LBB0_3
	.p2align	4
.LBB0_2:
	mov	rcx, qword ptr [rbp - 8]
	mov	edx, dword ptr [rbp + 288]
	lea	r8, [rbp + 40]
	lea	r9, [rbp + 36]
	call	qword ptr [rbp]
	test	al, al
	je	.LBB0_40
.LBB0_3:
	mov	eax, dword ptr [rbp + 36]
	movsxd	rcx, dword ptr [rbp + 40]
	mov	dword ptr [rbp + 32], eax
	cmp	ecx, eax
	jge	.LBB0_2
	mov	rax, qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"]
	mov	qword ptr [rbp - 16], rax
	jmp	.LBB0_5
	.p2align	4
.LBB0_38:
	mov	dword ptr [r13 + 32], r12d
	mov	rcx, qword ptr [rbp + 24]
.LBB0_39:
	inc	rcx
	cmp	dword ptr [rbp + 32], ecx
	je	.LBB0_2
.LBB0_5:
	mov	rax, qword ptr [rdi + 208]
	mov	r15, qword ptr [rax + 8*rcx]
	test	r15d, r15d
	js	.LBB0_39
	mov	qword ptr [rbp + 24], rcx
	mov	rax, qword ptr [rdi + 176]
	movsxd	rcx, dword ptr [rax]
	mov	qword ptr [rbp + 8], rcx
	movsxd	rcx, dword ptr [rax + 16]
	mov	qword ptr [rbp + 16], rcx
	mov	eax, dword ptr [rax + 20]
	mov	dword ptr [rbp + 44], eax
	mov	esi, r15d
	shl	rsi, 2
	add	rsi, qword ptr [rdi + 256]
	mov	rbx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"]
	call	qword ptr [rbp - 16]
	cdqe
	shl	rax, 6
	mov	ecx, dword ptr [rbx + rax + 56]
	mov	qword ptr [rbp - 24], rcx
	lea	r14d, [rcx + 1]
	mov	dword ptr [rbx + rax + 56], r14d
	mov	r12d, dword ptr [rbx + rax + 32]
	mov	r13, qword ptr [rbx + rax + 48]
	mov	rax, qword ptr [rdi]
	mov	rdx, qword ptr [rax + 24]
	movsxd	rax, dword ptr [rdx + 16]
	mov	rcx, qword ptr [rdx + 12]
	mov	qword ptr [rdi + 56], rcx
	mov	rcx, qword ptr [rdx]
	mov	qword ptr [rdi + 32], rcx
	movsxd	r8, dword ptr [rdx + 8]
	add	r8, rcx
	mov	qword ptr [rdi + 40], r8
	lea	r8, [rcx + 4096]
	mov	qword ptr [rdi + 48], r8
	mov	edx, dword ptr [rdx + 20]
	mov	dword ptr [rdi + 64], edx
	test	edx, edx
	jne	.LBB0_8
	add	rcx, rax
	mov	qword ptr [rdi + 48], rcx
.LBB0_8:
	mov	rax, qword ptr [rdi + 88]
	mov	rdx, qword ptr [rax + 24]
	mov	rax, qword ptr [rbp + 24]
	lea	r8, [rax + 2*rax]
	mov	ebx, dword ptr [rdx + 8*r8 + 12]
	mov	dword ptr [rdi + 144], ebx
	movsxd	rax, dword ptr [rdx + 8*r8 + 16]
	mov	dword ptr [rdi + 148], eax
	mov	rcx, qword ptr [rdx + 8*r8]
	mov	qword ptr [rdi + 120], rcx
	movsxd	r9, dword ptr [rdx + 8*r8 + 8]
	add	r9, rcx
	mov	qword ptr [rdi + 128], r9
	lea	r9, [rcx + 4096]
	mov	qword ptr [rdi + 136], r9
	mov	edx, dword ptr [rdx + 8*r8 + 20]
	mov	dword ptr [rdi + 152], edx
	test	edx, edx
	je	.LBB0_9
	test	ebx, ebx
	jg	.LBB0_11
	jmp	.LBB0_35
	.p2align	4
.LBB0_9:
	add	rcx, rax
	mov	qword ptr [rdi + 136], rcx
	test	ebx, ebx
	jle	.LBB0_35
.LBB0_11:
	vmovq	xmm0, r15
	vpextrd	r15d, xmm0, 1
	mov	rax, qword ptr [rbp + 16]
	mov	rcx, rax
	imul	rcx, qword ptr [rbp + 8]
	vcvtsi2sd	xmm10, xmm15, dword ptr [rbp + 44]
	vcvtsi2sd	xmm11, xmm15, eax
	mov	qword ptr [rbp - 32], rcx
	vmovq	xmm0, rcx
	vpunpckldq	xmm0, xmm0, xmm6
	vsubpd	xmm0, xmm0, xmm7
	vshufpd	xmm1, xmm0, xmm0, 1
	vaddsd	xmm12, xmm1, xmm0
	jmp	.LBB0_29
	.p2align	4
.LBB0_23:
	vmulsd	xmm2, xmm11, xmm3
	vcvtsi2sd	xmm4, xmm15, r9d
	vmulsd	xmm2, xmm2, xmm4
	vaddsd	xmm5, xmm2, xmm1
	mov	dword ptr [rsp + 80], r15d
	mov	qword ptr [rsp + 72], rsi
	vmovss	dword ptr [rsp + 64], xmm0
	and	r8b, 1
	mov	byte ptr [rsp + 56], r8b
	vmovsd	qword ptr [rsp + 48], xmm3
	mov	qword ptr [rsp + 32], rcx
	mov	rcx, rdi
	mov	rdx, r13
	mov	r8d, r12d
	mov	r9d, r14d
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
.LBB0_28:
	dec	ebx
	je	.LBB0_35
.LBB0_29:
	dec	dword ptr [rdi + 144]
	mov	rdx, qword ptr [rdi + 128]
	lea	rax, [rdx + 24]
	mov	qword ptr [rdi + 128], rax
	cmp	rax, qword ptr [rdi + 136]
	jbe	.LBB0_33
	mov	edx, dword ptr [rdi + 152]
	dec	edx
	mov	dword ptr [rdi + 152], edx
	mov	rax, qword ptr [rdi + 120]
	mov	rax, qword ptr [rax]
	mov	qword ptr [rdi + 120], rax
	mov	ecx, 4096
	test	edx, edx
	jg	.LBB0_32
	movsxd	rcx, dword ptr [rdi + 148]
.LBB0_32:
	lea	rdx, [rax + 8]
	add	rcx, rax
	mov	qword ptr [rdi + 136], rcx
	add	rax, 32
	mov	qword ptr [rdi + 128], rax
.LBB0_33:
	vmovss	xmm0, dword ptr [rdx + 4]
	mov	r9, qword ptr [rdx + 8]
	mov	r8d, dword ptr [rdx + 20]
	mov	rcx, qword ptr [r9]
	mov	rax, qword ptr [r9 + 8]
	test	byte ptr [rdx], 2
	vmovapd	xmm2, xmm8
	je	.LBB0_12
	vmovsd	xmm2, qword ptr [r9 + 16]
.LBB0_12:
	mov	edx, dword ptr [rcx + 140]
	vcvtsi2sd	xmm1, xmm15, edx
	vmulsd	xmm3, xmm2, xmm1
	vmulsd	xmm4, xmm9, xmm3
	mov	r9d, r8d
	and	r9d, 32767
	vcvtsi2sd	xmm1, xmm15, r9d
	mov	r9d, r8d
	shr	r9d, 15
	movzx	r9d, r9w
	lea	r10d, [r9 - 1]
	vcvtsi2sd	xmm5, xmm15, r10d
	vdivsd	xmm5, xmm1, xmm5
	vxorpd	xmm1, xmm1, xmm1
	test	r8d, r8d
	js	.LBB0_13
	vmulsd	xmm4, xmm5, xmm4
	vaddsd	xmm4, xmm4, xmm1
	cmp	r9d, 1
	jne	.LBB0_16
	jmp	.LBB0_17
	.p2align	4
.LBB0_13:
	vsubsd	xmm13, xmm1, xmm4
	vmulsd	xmm5, xmm13, xmm5
	vaddsd	xmm4, xmm4, xmm5
	cmp	r9d, 1
	je	.LBB0_17
.LBB0_16:
	vmovapd	xmm1, xmm4
.LBB0_17:
	test	r8d, r8d
	js	.LBB0_19
	xor	r8d, r8d
	vdivsd	xmm3, xmm3, xmm10
	test	rax, rax
	js	.LBB0_24
.LBB0_21:
	shr	rax, 31
	and	eax, 2147483647
	mov	r9, qword ptr [rbp + 8]
	sub	r9d, eax
	cmp	edx, dword ptr [rbp + 44]
	jne	.LBB0_23
	vucomisd	xmm2, xmm8
	jne	.LBB0_23
	jp	.LBB0_23
	imul	r9d, dword ptr [rbp + 16]
	vcvtsd2si	eax, xmm1
	add	eax, r9d
	mov	dword ptr [rsp + 72], r15d
	mov	qword ptr [rsp + 64], rsi
	vmovss	dword ptr [rsp + 56], xmm0
	and	r8b, 1
	mov	byte ptr [rsp + 48], r8b
	mov	qword ptr [rsp + 32], rcx
	mov	dword ptr [rsp + 40], eax
	mov	rcx, rdi
	mov	rdx, r13
	mov	r8d, r12d
	mov	r9d, r14d
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	jmp	.LBB0_28
	.p2align	4
.LBB0_19:
	movzx	r8d, byte ptr [rcx + 148]
	vdivsd	xmm3, xmm3, xmm10
	test	rax, rax
	jns	.LBB0_21
.LBB0_24:
	cmp	edx, dword ptr [rbp + 44]
	jne	.LBB0_26
	vucomisd	xmm2, xmm8
	jne	.LBB0_26
	jp	.LBB0_26
	shr	rax, 31
	and	eax, 268435455
	add	rax, qword ptr [rbp - 32]
	movsxd	r9, dword ptr [rcx + 144]
	xor	edx, edx
	div	r9
	vcvtsd2si	eax, xmm1
	add	eax, edx
	mov	dword ptr [rsp + 72], r15d
	mov	qword ptr [rsp + 64], rsi
	vmovss	dword ptr [rsp + 56], xmm0
	and	r8b, 1
	mov	byte ptr [rsp + 48], r8b
	mov	qword ptr [rsp + 32], rcx
	mov	dword ptr [rsp + 40], eax
	mov	rcx, rdi
	mov	rdx, r13
	mov	r8d, r12d
	mov	r9d, r14d
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	jmp	.LBB0_28
.LBB0_26:
	vmulsd	xmm2, xmm12, xmm3
	shr	rax, 31
	and	eax, 268435455
	vcvtsi2sd	xmm4, xmm15, eax
	vaddsd	xmm2, xmm2, xmm4
	vaddsd	xmm5, xmm2, xmm1
	mov	dword ptr [rsp + 80], r15d
	mov	qword ptr [rsp + 72], rsi
	vmovss	dword ptr [rsp + 64], xmm0
	and	r8b, 1
	mov	byte ptr [rsp + 56], r8b
	vmovsd	qword ptr [rsp + 48], xmm3
	mov	qword ptr [rsp + 32], rcx
	mov	rcx, rdi
	mov	rdx, r13
	mov	r8d, r12d
	mov	r9d, r14d
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	jmp	.LBB0_28
	.p2align	4
.LBB0_35:
	mov	rax, qword ptr [rbp - 24]
	mov	dword ptr [r13 + 56], eax
	mov	eax, dword ptr [r13 + 36]
	cmp	r12d, eax
	jbe	.LBB0_38
	cmp	r12d, 3
	mov	ecx, 2
	cmovae	ecx, r12d
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	r8, rcx
	shr	r8, 16
	or	r8, rcx
	cmp	r8, 2147483646
	mov	ecx, 2147483646
	cmovae	r8, rcx
	inc	r8d
	cmp	eax, r8d
	je	.LBB0_38
	lea	rcx, [r13 + 24]
	lea	rdx, [r13 + 40]
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	jmp	.LBB0_38
.LBB0_40:
	vmovaps	xmm6, xmmword ptr [rbp + 48]
	vmovaps	xmm7, xmmword ptr [rbp + 64]
	vmovaps	xmm8, xmmword ptr [rbp + 80]
	vmovaps	xmm9, xmmword ptr [rbp + 96]
	vmovaps	xmm10, xmmword ptr [rbp + 112]
	vmovaps	xmm11, xmmword ptr [rbp + 128]
	vmovaps	xmm12, xmmword ptr [rbp + 144]
	vmovaps	xmm13, xmmword ptr [rbp + 160]
	.seh_startepilogue
	add	rsp, 312
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
	lea	rcx, [rip + burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4]
	call	rdi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"], rax
	lea	rcx, [rip + burst.string.3beee8481600d4c1ec3133f8ae40837f]
	call	rdi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"], rax
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rdi
	mov	rsi, rax
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.ffbfa8f9468181ee01437772260f8c26]
	call	rdi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"], rax
	movabs	rax, -2808647651279017129
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 12
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"], rax
	movabs	rax, -887470822539941812
	mov	qword ptr [rbp - 16], rax
	mov	qword ptr [rbp - 8], 0
	lea	rcx, [rbp - 16]
	mov	edx, 8192
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"], rax
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

	.def	burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d
	.p2align	4
burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d:
.seh_proc burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d
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
	lea	rcx, [rip + burst.string.3beee8481600d4c1ec3133f8ae40837f]
	call	rsi
	mov	qword ptr [rip + "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"], rax
	lea	rcx, [rip + burst.string.57a01b00185c83d1eb43aeca89f29647]
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"], rax
	lea	rcx, [rip + burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"], rax
	lea	rcx, [rip + burst.string.ffbfa8f9468181ee01437772260f8c26]
	call	rsi
	mov	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"], rax
	.seh_startepilogue
	add	rsp, 40
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d
	.p2align	4
burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d:
.seh_proc burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	sub	rsp, 56
	.seh_stackalloc 56
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	movabs	rax, -2808647651279017129
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	mov	rsi, qword ptr [rip + "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	lea	rcx, [rbp - 8]
	mov	edx, 12
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"], rax
	movabs	rax, -887470822539941812
	mov	qword ptr [rbp - 8], rax
	mov	qword ptr [rbp], 0
	lea	rcx, [rbp - 8]
	mov	edx, 8192
	mov	r8d, 16
	call	rsi
	mov	qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"], rax
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

	.def	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.globl	__real@38000000
	.section	.rdata,"dr",discard,__real@38000000
	.p2align	2, 0x0
__real@38000000:
	.long	0x38000000
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.globl	__real@bf800000
	.section	.rdata,"dr",discard,__real@bf800000
	.p2align	2, 0x0
__real@bf800000:
	.long	0xbf800000
	.section	.text,"xr",discard,"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
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
	push	rax
	.seh_stackalloc 8
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	mov	dword ptr [rbp + 4], r8d
	jle	.LBB4_1
	mov	r15d, r9d
	mov	r11d, r8d
	mov	eax, dword ptr [rbp + 112]
	mov	r8d, dword ptr [rbp + 120]
	sub	r11d, r8d
	jle	.LBB4_5
	mov	esi, r8d
	shr	esi, 31
	add	esi, r8d
	sar	esi
	mov	edi, dword ptr [rcx + 8]
	xor	ebx, ebx
	mov	r14d, 88
	jmp	.LBB4_11
	.p2align	4
.LBB4_12:
	xor	r8d, r8d
.LBB4_10:
	cdqe
	lea	r9, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"]
	mov	r10d, dword ptr [r9 + 4*rax]
	movzx	r8d, r8b
	mov	r9d, r8d
	shl	r9d, 30
	sar	r9d, 31
	mov	r12d, r10d
	sar	r12d
	and	r12d, r9d
	mov	r9d, r8d
	and	r9d, 1
	neg	r9d
	mov	r13d, r10d
	sar	r13d, 2
	and	r13d, r9d
	mov	r9d, r8d
	shl	r9d, 29
	sar	r9d, 31
	and	r9d, r10d
	sar	r10d, 3
	add	r10d, r9d
	add	r10d, r12d
	add	r10d, r13d
	mov	r9d, r10d
	neg	r9d
	cmp	r8b, 8
	cmovb	r9d, r10d
	add	r9d, r15d
	cmp	r9d, -32767
	mov	r10d, -32768
	cmovl	r9d, r10d
	cmp	r9d, 32767
	mov	r10d, 32767
	cmovge	r9d, r10d
	lea	r10, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"]
	add	eax, dword ptr [r10 + 4*r8]
	mov	r8d, eax
	sar	r8d, 31
	andn	eax, r8d, eax
	cmp	eax, 88
	cmovge	eax, r14d
	xor	bl, 1
	dec	r11d
	mov	r15d, r9d
	je	.LBB4_2
.LBB4_11:
	cmp	esi, edi
	jge	.LBB4_12
	mov	r8, qword ptr [rcx]
	movsxd	r9, esi
	movzx	r8d, byte ptr [r8 + r9]
	test	bl, 1
	je	.LBB4_9
	and	r8b, 15
	inc	esi
	jmp	.LBB4_10
	.p2align	4
.LBB4_9:
	shr	r8b, 4
	jmp	.LBB4_10
.LBB4_1:
	xor	eax, eax
	xor	r9d, r9d
.LBB4_2:
	cmp	dword ptr [rdx + 8], 0
	jg	.LBB4_3
	jmp	.LBB4_25
.LBB4_5:
	mov	r9d, r15d
	cmp	dword ptr [rdx + 8], 0
	jle	.LBB4_25
.LBB4_3:
	mov	r8d, dword ptr [rbp + 4]
	mov	r10d, r8d
	and	r10b, 1
	mov	r11d, r8d
	shr	r11d, 31
	add	r11d, r8d
	sar	r11d
	xor	r8d, r8d
	vmovss	xmm0, dword ptr [rip + __real@38000000]
	vmovss	xmm1, dword ptr [rip + __real@3f800000]
	vmovss	xmm2, dword ptr [rip + __real@bf800000]
	xor	ebx, ebx
	jmp	.LBB4_23
	.p2align	4
.LBB4_22:
	mov	rsi, qword ptr [rdx]
	movsxd	r8, r8d
	vcvtsi2ss	xmm3, xmm5, r9d
	vmulss	xmm3, xmm3, xmm0
	vminss	xmm3, xmm3, xmm1
	vmaxss	xmm3, xmm3, xmm2
	vmovss	dword ptr [rsi + r8], xmm3
	inc	rbx
	xor	r10b, 1
	movsxd	rsi, dword ptr [rdx + 8]
	add	r8d, 4
	cmp	rbx, rsi
	jge	.LBB4_25
.LBB4_23:
	cmp	r11d, dword ptr [rcx + 8]
	jge	.LBB4_24
	mov	r14, qword ptr [rcx]
	movsxd	r15, r11d
	movzx	r14d, byte ptr [r14 + r15]
	test	r10b, 1
	je	.LBB4_15
	and	r14b, 15
	inc	r11d
	jmp	.LBB4_16
	.p2align	4
.LBB4_24:
	xor	r14d, r14d
	jmp	.LBB4_16
	.p2align	4
.LBB4_15:
	shr	r14b, 4
.LBB4_16:
	movsxd	r15, eax
	lea	rsi, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"]
	mov	r15d, dword ptr [rsi + 4*r15]
	movzx	r14d, r14b
	mov	r12d, r14d
	shl	r12d, 29
	sar	r12d, 31
	and	r12d, r15d
	mov	r13d, r14d
	shl	r13d, 30
	sar	r13d, 31
	mov	esi, r15d
	sar	esi
	and	esi, r13d
	mov	r13d, r14d
	and	r13d, 1
	neg	r13d
	mov	edi, r15d
	sar	edi, 2
	and	edi, r13d
	sar	r15d, 3
	add	r15d, r12d
	add	r15d, esi
	add	r15d, edi
	mov	r12d, r15d
	neg	r12d
	cmp	r14b, 8
	cmovb	r12d, r15d
	add	r12d, r9d
	cmp	r12d, -32767
	jge	.LBB4_18
	mov	r12d, -32768
.LBB4_18:
	cmp	r12d, 32767
	mov	r9d, r12d
	jl	.LBB4_20
	mov	r9d, 32767
.LBB4_20:
	lea	rsi, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"]
	add	eax, dword ptr [rsi + 4*r14]
	mov	esi, eax
	sar	esi, 31
	andn	eax, esi, eax
	cmp	eax, 88
	jl	.LBB4_22
	mov	eax, 88
	jmp	.LBB4_22
.LBB4_25:
	.seh_startepilogue
	add	rsp, 8
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

	.def	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
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
	sub	rsp, 104
	.seh_stackalloc 104
	lea	rbp, [rsp + 96]
	.seh_setframe rbp, 96
	.seh_endprologue
	mov	esi, edx
	mov	rdi, rcx
	mov	rcx, qword ptr [rcx]
	test	rcx, rcx
	je	.LBB5_1
	movsxd	r9, esi
	mov	edx, 4
	mov	r8d, 4
	call	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	jmp	.LBB5_7
.LBB5_1:
	cmp	qword ptr [rdi + 24], 0
	jne	.LBB5_2
	lea	rdx, [rbp]
	vxorps	xmm0, xmm0, xmm0
	vmovups	xmmword ptr [rbp - 16], xmm0
	mov	eax, dword ptr [rdi + 16]
	mov	dword ptr [rbp], eax
	mov	dword ptr [rbp + 4], 0
	lea	rcx, [rbp - 16]
	mov	r8d, 8
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, qword ptr [rbp - 16]
	mov	ecx, dword ptr [rbp + 4]
	mov	qword ptr [rdi + 24], rax
	mov	rax, qword ptr [rbp - 8]
	mov	qword ptr [rdi + 32], rax
	mov	eax, dword ptr [rbp]
	mov	dword ptr [rdi + 40], eax
	mov	dword ptr [rdi + 44], ecx
.LBB5_2:
	movzx	r8d, word ptr [rdi + 16]
	cmp	r8, 64
	movzx	eax, word ptr [rdi + 18]
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 48], ymm0
	mov	dword ptr [rbp - 40], esi
	mov	dword ptr [rbp - 32], 4
	mov	byte ptr [rbp - 24], 6
	mov	word ptr [rbp - 36], r8w
	mov	word ptr [rbp - 34], ax
	jb	.LBB5_8
	test	ax, ax
	jne	.LBB5_5
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	eax, word ptr [rax + 2*r8]
	mov	word ptr [rbp - 34], ax
.LBB5_5:
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	r8d, 4
	mov	rcx, qword ptr [rax + r8 + 8]
	lea	rdx, [rbp - 48]
	vzeroupper
	call	qword ptr [rax + r8]
	mov	rax, qword ptr [rbp - 48]
	movsxd	rbx, dword ptr [rdi + 32]
	mov	ecx, dword ptr [rdi + 36]
	cmp	ebx, ecx
	jl	.LBB5_14
.LBB5_10:
	lea	r14d, [rbx + 1]
	cmp	r14d, ecx
	jle	.LBB5_13
	cmp	r14d, 3
	mov	edx, 2
	cmovae	edx, r14d
	dec	rdx
	mov	r8, rdx
	shr	r8
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 2
	or	rdx, r8
	mov	r8, rdx
	shr	r8, 4
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 8
	or	rdx, r8
	mov	r9, rdx
	shr	r9, 16
	or	r9, rdx
	cmp	r9, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, r9
	inc	r8d
	cmp	ecx, r8d
	je	.LBB5_13
	lea	rcx, [rdi + 24]
	lea	rdx, [rdi + 40]
	mov	r15, rax
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, r15
.LBB5_13:
	mov	dword ptr [rdi + 32], r14d
	mov	rcx, qword ptr [rdi + 24]
	lea	rdx, [rbx + 2*rbx]
	mov	qword ptr [rcx + 8*rdx], rax
	mov	dword ptr [rcx + 8*rdx + 8], esi
	movabs	r8, 17179869188
	mov	qword ptr [rcx + 8*rdx + 12], r8
	mov	dword ptr [rcx + 8*rdx + 20], 0
	jmp	.LBB5_7
.LBB5_8:
	movsxd	rcx, esi
	shl	rcx, 2
	mov	qword ptr [rsp + 32], 0
	mov	edx, 64
	mov	r9d, 1
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	movsxd	rbx, dword ptr [rdi + 32]
	mov	ecx, dword ptr [rdi + 36]
	cmp	ebx, ecx
	jge	.LBB5_10
.LBB5_14:
	mov	rcx, qword ptr [rdi + 24]
	lea	rdx, [rbx + 2*rbx]
	mov	qword ptr [rcx + 8*rdx], rax
	mov	dword ptr [rcx + 8*rdx + 8], esi
	movabs	r8, 17179869188
	mov	qword ptr [rcx + 8*rdx + 12], r8
	mov	dword ptr [rcx + 8*rdx + 20], 0
	inc	dword ptr [rdi + 32]
.LBB5_7:
	mov	edx, esi
	.seh_startepilogue
	add	rsp, 104
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.globl	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	.p2align	4
"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40":
.seh_proc "Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
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
	mov	ebx, r9d
	mov	esi, edx
	mov	rdi, rcx
	mov	rcx, qword ptr [rbp + 112]
	mov	edx, r8d
	call	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	dword ptr [rbp - 28], edx
	movsxd	r8, esi
	test	bl, bl
	je	.LBB6_1
	imul	rcx, r8, 1717986919
	mov	r9, rcx
	shr	r9, 63
	sar	rcx, 39
	add	ecx, r9d
	mov	r9d, ecx
	shl	r9d, 6
	lea	r9d, [r9 + 4*r9]
	add	edx, esi
	cmp	r9d, edx
	jge	.LBB6_8
	movsxd	r10, dword ptr [rdi + 8]
	lea	r10, [rdi + r10 + 8]
	mov	qword ptr [rbp - 8], r10
	movsxd	r11, edx
	movsxd	r10, dword ptr [rdi + 24]
	lea	r10, [rdi + r10 + 24]
	movsxd	rdi, ecx
	shl	rdi, 4
	vpmovsxwd	xmm0, qword ptr [rdi + r10 + 8]
	vpmovsxwd	xmm1, qword ptr [rdi + r10]
	shl	esi, 2
	neg	esi
	mov	r14d, -32768
	mov	r15d, 32767
	vmovss	xmm2, dword ptr [rip + __real@38000000]
	mov	qword ptr [rbp - 24], rsi
	mov	dword ptr [rbp + 4], edx
	jmp	.LBB6_12
	.p2align	4
.LBB6_16:
	mov	rdi, qword ptr [rbp - 16]
	inc	edi
	add	r9d, ecx
	mov	edx, dword ptr [rbp + 4]
	cmp	edx, r9d
	mov	rsi, qword ptr [rbp - 24]
	jle	.LBB6_8
.LBB6_12:
	mov	qword ptr [rbp - 16], rdi
	lea	ecx, [8*rdi]
	movsxd	rcx, ecx
	mov	rdx, qword ptr [rbp - 8]
	mov	r12, qword ptr [rdx + rcx]
	mov	rcx, r12
	shr	rcx, 57
	and	ecx, -8
	lea	rdx, [rip + "System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"]
	lea	r13, [rdx + 4*rcx]
	movsxd	rdx, r9d
	lea	ebx, [rsi + 4*rdx]
	xor	r10d, r10d
	.p2align	4
.LBB6_13:
	mov	rcx, r12
	shr	rcx, 57
	and	ecx, 7
	vpmovzxdq	ymm3, xmm0
	vpmovzxdq	ymm4, xmm1
	vpmuldq	ymm3, ymm3, ymm4
	vextracti128	xmm4, ymm3, 1
	vpaddq	xmm3, xmm3, xmm4
	vpshufd	xmm4, xmm3, 238
	vpaddq	xmm3, xmm3, xmm4
	vmovq	rsi, xmm3
	shr	rsi, 13
	mov	ecx, dword ptr [r13 + 4*rcx]
	add	esi, ecx
	cmp	esi, -32767
	cmovl	esi, r14d
	cmp	esi, 32767
	cmovge	esi, r15d
	cmp	rdx, r8
	jl	.LBB6_14
	movsxd	rdi, ebx
	vcvtsi2ss	xmm3, xmm5, esi
	vmulss	xmm3, xmm3, xmm2
	vmovss	dword ptr [rax + rdi], xmm3
.LBB6_14:
	vmovd	xmm3, ecx
	sar	ecx, 4
	vpbroadcastd	xmm3, xmm3
	vpsrad	xmm3, xmm3, 4
	neg	ecx
	vmovd	xmm4, ecx
	vpbroadcastd	xmm4, xmm4
	vblendvps	xmm3, xmm3, xmm4, xmm1
	vpaddd	xmm0, xmm3, xmm0
	vpshufd	xmm1, xmm1, 249
	vpinsrd	xmm1, xmm1, esi, 3
	lea	ecx, [r10 + 1]
	cmp	r10d, 18
	ja	.LBB6_16
	shl	r12, 3
	inc	rdx
	add	ebx, 4
	cmp	rdx, r11
	mov	r10d, ecx
	jl	.LBB6_13
	jmp	.LBB6_16
.LBB6_1:
	imul	r10, r8, 1717986919
	mov	rcx, r10
	shr	rcx, 63
	sar	r10, 39
	add	r10d, ecx
	mov	ecx, r10d
	shl	ecx, 6
	lea	r9d, [rcx + 4*rcx]
	add	edx, esi
	cmp	r9d, edx
	jge	.LBB6_8
	movsxd	rcx, dword ptr [rdi]
	add	rcx, rdi
	mov	qword ptr [rbp - 8], rcx
	movsxd	r11, edx
	movsxd	rcx, dword ptr [rdi + 16]
	lea	rcx, [rdi + rcx + 16]
	movsxd	r10, r10d
	shl	r10, 4
	vpmovsxwd	xmm0, qword ptr [r10 + rcx + 8]
	vpmovsxwd	xmm1, qword ptr [r10 + rcx]
	shl	esi, 2
	neg	esi
	mov	r14d, -32768
	mov	r15d, 32767
	vmovss	xmm2, dword ptr [rip + __real@38000000]
	mov	qword ptr [rbp - 24], rsi
	mov	dword ptr [rbp + 4], edx
	jmp	.LBB6_3
	.p2align	4
.LBB6_7:
	mov	r10, qword ptr [rbp - 16]
	inc	r10d
	add	r9d, ecx
	mov	edx, dword ptr [rbp + 4]
	cmp	edx, r9d
	mov	rsi, qword ptr [rbp - 24]
	jle	.LBB6_8
.LBB6_3:
	mov	qword ptr [rbp - 16], r10
	lea	ecx, [8*r10]
	movsxd	rcx, ecx
	mov	rdx, qword ptr [rbp - 8]
	mov	r12, qword ptr [rdx + rcx]
	mov	rcx, r12
	shr	rcx, 57
	and	ecx, -8
	lea	rdx, [rip + "System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"]
	lea	r13, [rdx + 4*rcx]
	movsxd	rbx, r9d
	lea	edx, [rsi + 4*rbx]
	xor	r10d, r10d
	.p2align	4
.LBB6_4:
	mov	rcx, r12
	shr	rcx, 57
	and	ecx, 7
	vpmovzxdq	ymm3, xmm0
	vpmovzxdq	ymm4, xmm1
	vpmuldq	ymm3, ymm3, ymm4
	vextracti128	xmm4, ymm3, 1
	vpaddq	xmm3, xmm3, xmm4
	vpshufd	xmm4, xmm3, 238
	vpaddq	xmm3, xmm3, xmm4
	vmovq	rsi, xmm3
	shr	rsi, 13
	mov	ecx, dword ptr [r13 + 4*rcx]
	add	esi, ecx
	cmp	esi, -32767
	cmovl	esi, r14d
	cmp	esi, 32767
	cmovge	esi, r15d
	cmp	rbx, r8
	jl	.LBB6_5
	movsxd	rdi, edx
	vcvtsi2ss	xmm3, xmm5, esi
	vmulss	xmm3, xmm3, xmm2
	vmovss	dword ptr [rax + rdi], xmm3
.LBB6_5:
	vmovd	xmm3, ecx
	sar	ecx, 4
	vpbroadcastd	xmm3, xmm3
	vpsrad	xmm3, xmm3, 4
	neg	ecx
	vmovd	xmm4, ecx
	vpbroadcastd	xmm4, xmm4
	vblendvps	xmm3, xmm3, xmm4, xmm1
	vpaddd	xmm0, xmm3, xmm0
	vpshufd	xmm1, xmm1, 249
	vpinsrd	xmm1, xmm1, esi, 3
	lea	ecx, [r10 + 1]
	cmp	r10d, 18
	ja	.LBB6_7
	shl	r12, 3
	inc	rbx
	add	edx, 4
	cmp	rbx, r11
	mov	r10d, ecx
	jl	.LBB6_4
	jmp	.LBB6_7
.LBB6_8:
	mov	edx, dword ptr [rbp - 28]
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
	vzeroupper
	ret
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	.p2align	4
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
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
	sub	rsp, 184
	.seh_stackalloc 184
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovaps	xmmword ptr [rbp + 32], xmm6
	.seh_savexmm xmm6, 160
	.seh_endprologue
	mov	rdi, rdx
	mov	r15d, dword ptr [rbp + 200]
	mov	eax, dword ptr [rbp + 168]
	mov	rbx, qword ptr [rbp + 160]
	test	eax, eax
	mov	r12d, dword ptr [rbx + 144]
	jns	.LBB7_2
	.p2align	4
.LBB7_1:
	add	eax, r12d
	js	.LBB7_1
.LBB7_2:
	mov	r13, qword ptr [rbp + 192]
	vmovss	xmm6, dword ptr [rbp + 184]
	movzx	r10d, byte ptr [rbp + 176]
	cdq
	idiv	r12d
	mov	esi, edx
	lea	eax, [rsi + r15]
	cmp	eax, r12d
	jge	.LBB7_5
	mov	dword ptr [rsp + 72], r15d
	mov	qword ptr [rsp + 64], r13
	vmovss	dword ptr [rsp + 56], xmm6
	mov	byte ptr [rsp + 48], r10b
	mov	dword ptr [rsp + 40], esi
	mov	qword ptr [rsp + 32], rbx
	mov	rdx, rdi
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	jmp	.LBB7_33
.LBB7_5:
	lea	eax, [r15 + r15]
	cmp	eax, r12d
	jge	.LBB7_7
	sub	r12d, esi
	mov	dword ptr [rsp + 72], r12d
	mov	qword ptr [rsp + 64], r13
	vmovss	dword ptr [rsp + 56], xmm6
	mov	byte ptr [rsp + 48], r10b
	mov	dword ptr [rsp + 40], esi
	mov	qword ptr [rsp + 32], rbx
	mov	rsi, rcx
	mov	rdx, rdi
	mov	r14d, r8d
	mov	ebx, r9d
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	sub	r15d, r12d
	shl	r12d, 2
	movsxd	rax, r12d
	add	rax, r13
	mov	dword ptr [rsp + 72], r15d
	mov	qword ptr [rsp + 64], rax
	vmovss	dword ptr [rsp + 56], xmm6
	movzx	eax, byte ptr [rbp + 176]
	mov	byte ptr [rsp + 48], al
	mov	rax, qword ptr [rbp + 160]
	mov	qword ptr [rsp + 32], rax
	mov	dword ptr [rsp + 40], 0
	mov	rcx, rsi
	mov	rdx, rdi
	mov	r8d, r14d
	mov	r9d, ebx
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	jmp	.LBB7_33
.LBB7_7:
	mov	rax, qword ptr [rcx + 176]
	mov	eax, dword ptr [rax + 20]
	mov	ecx, dword ptr [rdi + 56]
	inc	ecx
	mov	dword ptr [rdi + 56], ecx
	mov	edx, dword ptr [rdi + 32]
	mov	r8, qword ptr [rdi + 48]
	mov	qword ptr [rbp - 48], r8
	mov	dword ptr [rbp - 40], edx
	mov	dword ptr [rbp - 36], ecx
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 32], ymm0
	mov	dword ptr [rbp], eax
	mov	dword ptr [rbp + 4], 0
	movzx	eax, byte ptr [rbx + 149]
	cmp	eax, 2
	mov	r8d, dword ptr [rbx + 144]
	je	.LBB7_27
	cmp	eax, 1
	je	.LBB7_12
	test	eax, eax
	jne	.LBB7_10
	movsxd	rax, dword ptr [rbx]
	lea	rcx, [rbx + rax]
	lea	rax, [rbx + rax + 8]
	test	r10b, r10b
	cmove	rax, rcx
	movsxd	rcx, dword ptr [rax]
	add	rcx, rax
	test	r15d, r15d
	jg	.LBB7_29
	jmp	.LBB7_31
.LBB7_12:
	movsxd	r14, dword ptr [rbx]
	lea	rdi, [rbx + r14]
	lea	rcx, [rbp - 48]
	mov	edx, r8d
	mov	r12d, r10d
	vzeroupper
	call	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	rcx, rax
	lea	rax, [rbx + r14 + 8]
	test	r12b, r12b
	cmove	rax, rdi
	test	edx, edx
	jle	.LBB7_28
	movzx	r8d, r12b
	movsxd	r9, dword ptr [rax]
	add	r9, rax
	mov	qword ptr [rbp + 16], r9
	mov	eax, dword ptr [rdi + 8*r8 + 4]
	mov	dword ptr [rbp + 28], eax
	mov	eax, edx
	mov	qword ptr [rbp + 8], rax
	xor	r9d, r9d
	vmovss	xmm0, dword ptr [rip + __real@38000000]
	vmovss	xmm1, dword ptr [rip + __real@3f800000]
	vmovss	xmm2, dword ptr [rip + __real@bf800000]
	xor	edi, edi
	xor	r14d, r14d
	xor	r10d, r10d
	xor	r15d, r15d
	jmp	.LBB7_14
	.p2align	4
.LBB7_24:
	movsxd	r9, r9d
	vcvtsi2ss	xmm3, xmm5, r10d
	vmulss	xmm3, xmm3, xmm0
	vminss	xmm3, xmm3, xmm1
	vmaxss	xmm3, xmm3, xmm2
	vmovss	dword ptr [rcx + r9], xmm3
	inc	rdi
	add	r9d, 4
	cmp	qword ptr [rbp + 8], rdi
	je	.LBB7_25
.LBB7_14:
	cmp	r14d, dword ptr [rbp + 28]
	jge	.LBB7_26
	movsxd	rax, r14d
	mov	rdx, qword ptr [rbp + 16]
	movzx	r12d, byte ptr [rdx + rax]
	test	dil, 1
	je	.LBB7_17
	and	r12b, 15
	inc	r14d
	jmp	.LBB7_18
	.p2align	4
.LBB7_26:
	xor	r12d, r12d
	jmp	.LBB7_18
	.p2align	4
.LBB7_17:
	shr	r12b, 4
.LBB7_18:
	mov	eax, r15d
	lea	rdx, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"]
	mov	eax, dword ptr [rdx + 4*rax]
	movzx	r12d, r12b
	mov	r11d, r12d
	shl	r11d, 29
	sar	r11d, 31
	and	r11d, eax
	mov	r8d, r12d
	shl	r8d, 30
	sar	r8d, 31
	mov	edx, eax
	sar	edx
	and	edx, r8d
	mov	r8d, r12d
	and	r8d, 1
	neg	r8d
	mov	ebx, eax
	sar	ebx, 2
	and	ebx, r8d
	sar	eax, 3
	add	eax, r11d
	add	eax, edx
	add	eax, ebx
	mov	r11d, eax
	neg	r11d
	cmp	r12b, 8
	cmovb	r11d, eax
	add	r11d, r10d
	cmp	r11d, -32767
	jge	.LBB7_20
	mov	r11d, -32768
.LBB7_20:
	cmp	r11d, 32767
	mov	r10d, r11d
	jl	.LBB7_22
	mov	r10d, 32767
.LBB7_22:
	lea	rax, [rip + "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"]
	add	r15d, dword ptr [rax + 4*r12]
	mov	eax, r15d
	sar	eax, 31
	andn	r15d, eax, r15d
	cmp	r15d, 88
	jl	.LBB7_24
	mov	r15d, 88
	jmp	.LBB7_24
.LBB7_27:
	movsxd	rcx, dword ptr [rbx]
	add	rcx, rbx
	lea	rax, [rbp - 48]
	mov	qword ptr [rsp + 32], rax
	xor	edx, edx
	mov	r9d, r10d
	vzeroupper
	call	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	rcx, rax
	test	r15d, r15d
	jg	.LBB7_29
	jmp	.LBB7_31
.LBB7_10:
	xor	ecx, ecx
.LBB7_28:
	test	r15d, r15d
	jle	.LBB7_31
.LBB7_29:
	mov	r8d, r15d
	shl	r8, 2
	xor	r9d, r9d
	.p2align	4
.LBB7_30:
	mov	eax, esi
	cdq
	idiv	dword ptr [rbx + 144]
	movsxd	rax, r9d
	shl	edx, 2
	movsxd	rdx, edx
	vmulss	xmm0, xmm6, dword ptr [rcx + rdx]
	vaddss	xmm0, xmm0, dword ptr [r13 + rax]
	vmovss	dword ptr [r13 + rax], xmm0
	add	r9, 4
	inc	esi
	cmp	r8, r9
	jne	.LBB7_30
.LBB7_31:
	mov	rsi, qword ptr [rbp - 48]
	mov	edi, dword ptr [rbp - 40]
	mov	eax, dword ptr [rbp - 36]
	dec	eax
	mov	dword ptr [rsi + 56], eax
	mov	eax, dword ptr [rsi + 36]
	cmp	edi, eax
	jle	.LBB7_32
	cmp	edi, 3
	mov	ecx, 2
	cmovae	ecx, edi
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 16
	or	rdx, rcx
	cmp	rdx, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, rdx
	inc	r8d
	cmp	eax, r8d
	je	.LBB7_32
	lea	rcx, [rsi + 24]
	lea	rdx, [rsi + 40]
	vzeroupper
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
.LBB7_32:
	mov	dword ptr [rsi + 32], edi
.LBB7_33:
	vmovaps	xmm6, xmmword ptr [rbp + 32]
	.seh_startepilogue
	add	rsp, 184
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
.LBB7_25:
	mov	rbx, qword ptr [rbp + 160]
	mov	r15d, dword ptr [rbp + 200]
	test	r15d, r15d
	jg	.LBB7_29
	jmp	.LBB7_31
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	.p2align	4
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
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
	sub	rsp, 136
	.seh_stackalloc 136
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	.seh_endprologue
	mov	rax, qword ptr [rbp + 96]
	mov	r9d, dword ptr [rbp + 136]
	mov	r8d, dword ptr [rbp + 104]
	mov	r10d, r8d
	neg	r10d
	mov	r11d, r10d
	sar	r11d, 31
	andn	r14d, r11d, r10d
	mov	r10d, dword ptr [rax + 144]
	sub	r10d, r8d
	mov	r11d, r10d
	sar	r11d, 31
	andn	esi, r11d, r10d
	cmp	r9d, esi
	cmovl	esi, r9d
	sub	esi, r14d
	test	esi, esi
	jle	.LBB8_31
	movzx	ebx, byte ptr [rbp + 112]
	mov	r9d, r8d
	sar	r9d, 31
	andn	edi, r9d, r8d
	mov	rcx, qword ptr [rcx + 176]
	mov	ecx, dword ptr [rcx + 20]
	mov	r8d, dword ptr [rdx + 56]
	inc	r8d
	mov	dword ptr [rdx + 56], r8d
	mov	r9d, dword ptr [rdx + 32]
	mov	rdx, qword ptr [rdx + 48]
	mov	qword ptr [rbp - 72], rdx
	mov	dword ptr [rbp - 64], r9d
	mov	dword ptr [rbp - 60], r8d
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 56], ymm0
	mov	dword ptr [rbp - 24], ecx
	mov	dword ptr [rbp - 20], 0
	movzx	ecx, byte ptr [rax + 149]
	cmp	ecx, 2
	je	.LBB8_12
	cmp	ecx, 1
	je	.LBB8_8
	test	ecx, ecx
	jne	.LBB8_4
	movsxd	rcx, dword ptr [rax]
	add	rcx, rax
	test	bl, bl
	je	.LBB8_6
	movsxd	rax, dword ptr [rcx + 8]
	lea	rcx, [rcx + rax + 8]
	shl	edi, 2
	movsxd	rax, edi
	add	rax, rcx
	jmp	.LBB8_13
.LBB8_8:
	movsxd	r15, dword ptr [rax]
	add	r15, rax
	lea	rcx, [rbp - 72]
	mov	edx, esi
	vzeroupper
	call	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	qword ptr [rbp - 4], rax
	mov	dword ptr [rbp + 4], edx
	mov	eax, edi
	shr	eax, 8
	and	eax, 8388604
	test	bl, bl
	je	.LBB8_9
	movsxd	rcx, dword ptr [r15 + 24]
	lea	rcx, [r15 + rcx + 24]
	lea	r8, [r15 + 8]
	mov	edx, 12
	jmp	.LBB8_11
.LBB8_12:
	movsxd	rcx, dword ptr [rax]
	add	rcx, rax
	lea	rax, [rbp - 72]
	mov	qword ptr [rsp + 32], rax
	mov	edx, edi
	mov	r8d, esi
	mov	r9d, ebx
	vzeroupper
	call	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	jmp	.LBB8_13
.LBB8_4:
	xor	eax, eax
	jmp	.LBB8_13
.LBB8_9:
	movsxd	rcx, dword ptr [r15 + 16]
	lea	rcx, [r15 + rcx + 16]
	mov	edx, 4
	mov	r8, r15
.LBB8_11:
	movsxd	r10, dword ptr [r8]
	add	r10, r8
	movsx	r9d, word ptr [rcx + rax]
	movsx	eax, word ptr [rcx + rax + 2]
	mov	ecx, dword ptr [r15 + rdx]
	mov	qword ptr [rbp - 16], r10
	mov	dword ptr [rbp - 8], ecx
	mov	ecx, edi
	and	ecx, 2147482624
	mov	dword ptr [rsp + 40], ecx
	mov	dword ptr [rsp + 32], eax
	lea	rcx, [rbp - 16]
	lea	rdx, [rbp - 4]
	mov	r8d, edi
	call	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	rax, qword ptr [rbp - 4]
	jmp	.LBB8_13
.LBB8_6:
	movsxd	rdx, dword ptr [rcx]
	shl	edi, 2
	movsxd	rax, edi
	add	rax, rcx
	add	rax, rdx
.LBB8_13:
	vmovss	xmm0, dword ptr [rbp + 120]
	mov	rcx, qword ptr [rbp + 128]
	mov	edx, esi
	cmp	esi, 3
	jbe	.LBB8_14
	lea	r8, [rdx - 1]
	lea	r9d, [4*r14]
	lea	r10d, [r8 + r14]
	shl	r10d, 2
	cmp	r10d, r9d
	setl	r10b
	cmp	r8, 536870912
	setae	r8b
	or	r8b, r10b
	jne	.LBB8_14
	movsxd	r8, r9d
	add	r8, rcx
	lea	r10, [rax + 4*rdx]
	cmp	r8, r10
	jae	.LBB8_20
	lea	r8, [r8 + 4*rdx]
	cmp	rax, r8
	jae	.LBB8_20
.LBB8_14:
	xor	r8d, r8d
.LBB8_15:
	mov	r9, r8
	sub	r9, rdx
	shl	r8d, 2
	shl	r14d, 2
	.p2align	4
.LBB8_16:
	lea	edx, [r14 + r8]
	movsxd	rdx, edx
	movsxd	r8, r8d
	vmulss	xmm1, xmm0, dword ptr [rax + r8]
	vaddss	xmm1, xmm1, dword ptr [rcx + rdx]
	vmovss	dword ptr [rcx + rdx], xmm1
	add	r8d, 4
	inc	r9
	jne	.LBB8_16
.LBB8_29:
	mov	rsi, qword ptr [rbp - 72]
	mov	edi, dword ptr [rbp - 64]
	mov	eax, dword ptr [rbp - 60]
	dec	eax
	mov	dword ptr [rsi + 56], eax
	mov	eax, dword ptr [rsi + 36]
	cmp	edi, eax
	jle	.LBB8_30
	cmp	edi, 3
	mov	ecx, 2
	cmovae	ecx, edi
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 16
	or	rdx, rcx
	cmp	rdx, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, rdx
	inc	r8d
	cmp	eax, r8d
	je	.LBB8_30
	lea	rcx, [rsi + 24]
	lea	rdx, [rsi + 40]
	vzeroupper
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
.LBB8_30:
	mov	dword ptr [rsi + 32], edi
.LBB8_31:
	.seh_startepilogue
	add	rsp, 136
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
.LBB8_20:
	xor	r10d, r10d
	cmp	esi, 32
	jae	.LBB8_22
	xor	r8d, r8d
	jmp	.LBB8_26
.LBB8_22:
	mov	r8d, edx
	and	r8d, 1073741792
	vbroadcastss	ymm1, xmm0
	mov	r11, r8
	.p2align	4
.LBB8_23:
	lea	esi, [r9 + r10]
	movsxd	rsi, esi
	movsxd	r10, r10d
	vmulps	ymm2, ymm1, ymmword ptr [rax + r10]
	vmulps	ymm3, ymm1, ymmword ptr [rax + r10 + 32]
	vmulps	ymm4, ymm1, ymmword ptr [rax + r10 + 64]
	vmulps	ymm5, ymm1, ymmword ptr [rax + r10 + 96]
	vaddps	ymm2, ymm2, ymmword ptr [rcx + rsi]
	vaddps	ymm3, ymm3, ymmword ptr [rcx + rsi + 32]
	vaddps	ymm4, ymm4, ymmword ptr [rcx + rsi + 64]
	vaddps	ymm5, ymm5, ymmword ptr [rcx + rsi + 96]
	vmovups	ymmword ptr [rcx + rsi], ymm2
	vmovups	ymmword ptr [rcx + rsi + 32], ymm3
	vmovups	ymmword ptr [rcx + rsi + 64], ymm4
	vmovups	ymmword ptr [rcx + rsi + 96], ymm5
	sub	r10d, -128
	add	r11, -32
	jne	.LBB8_23
	cmp	r8d, edx
	je	.LBB8_29
	test	dl, 28
	je	.LBB8_15
.LBB8_26:
	mov	r10, r8
	mov	r8d, edx
	and	r8d, 1073741820
	vbroadcastss	xmm1, xmm0
	mov	r11, r10
	sub	r11, r8
	shl	r10d, 2
	.p2align	4
.LBB8_27:
	lea	esi, [r9 + r10]
	movsxd	rsi, esi
	movsxd	r10, r10d
	vmulps	xmm2, xmm1, xmmword ptr [rax + r10]
	vaddps	xmm2, xmm2, xmmword ptr [rcx + rsi]
	vmovups	xmmword ptr [rcx + rsi], xmm2
	add	r10d, 16
	add	r11, 4
	jne	.LBB8_27
	cmp	r8d, edx
	jne	.LBB8_15
	jmp	.LBB8_29
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.p2align	4
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
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
	sub	rsp, 184
	.seh_stackalloc 184
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovaps	xmmword ptr [rbp + 32], xmm9
	.seh_savexmm xmm9, 160
	vmovaps	xmmword ptr [rbp + 16], xmm8
	.seh_savexmm xmm8, 144
	vmovaps	xmmword ptr [rbp], xmm7
	.seh_savexmm xmm7, 128
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 112
	.seh_endprologue
	vmovapd	xmm0, xmm5
	mov	esi, r9d
	mov	r14d, r8d
	mov	rdi, rdx
	mov	r15, rcx
	mov	ebx, dword ptr [rbp + 208]
	vmovsd	xmm7, qword ptr [rbp + 176]
	mov	r12, qword ptr [rbp + 160]
	vcvtsi2sd	xmm1, xmm4, dword ptr [r12 + 144]
	vmulsd	xmm9, xmm7, xmm1
	vxorpd	xmm2, xmm2, xmm2
	vucomisd	xmm2, xmm5
	jbe	.LBB9_2
	.p2align	4
.LBB9_1:
	vaddsd	xmm0, xmm0, xmm1
	vucomisd	xmm2, xmm0
	ja	.LBB9_1
.LBB9_2:
	mov	r13, qword ptr [rbp + 200]
	vmovss	xmm8, dword ptr [rbp + 192]
	call	burst.Sleef_fmod_avx2@@16
	vmovapd	xmm6, xmm0
	vcvtsi2sd	xmm0, xmm4, ebx
	vmulsd	xmm0, xmm7, xmm0
	vaddsd	xmm0, xmm0, xmm6
	vaddsd	xmm1, xmm0, qword ptr [rip + __real@3ff0000000000000]
	vucomisd	xmm9, xmm1
	ja	.LBB9_11
	vcvttsd2si	ebx, xmm6
	vroundsd	xmm0, xmm0, xmm0, 10
	vcvttsd2si	esi, xmm0
	sub	esi, ebx
	lea	eax, [rsi + 2]
	mov	ecx, dword ptr [rdi + 56]
	mov	qword ptr [rbp - 32], r15
	mov	qword ptr [rbp - 40], rcx
	lea	r15d, [rcx + 1]
	mov	dword ptr [rdi + 56], r15d
	mov	ecx, dword ptr [rdi + 32]
	mov	dword ptr [rbp - 20], ecx
	mov	rdi, qword ptr [rdi + 48]
	movsxd	r12, eax
	mov	edx, 4
	mov	r8d, 4
	mov	rcx, rdi
	mov	r9, r12
	call	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	mov	r14, rax
	lea	r8d, [4*rsi + 8]
	xor	esi, esi
	mov	rcx, rax
	xor	edx, edx
	xor	r9d, r9d
	call	burst.memset.inline.AVX2.i32@@32
	mov	dword ptr [rsp + 72], r12d
	mov	qword ptr [rsp + 64], r14
	vmovss	dword ptr [rsp + 56], xmm8
	movzx	eax, byte ptr [rbp + 184]
	mov	byte ptr [rsp + 48], al
	mov	dword ptr [rsp + 40], ebx
	mov	ebx, dword ptr [rbp - 20]
	mov	rax, qword ptr [rbp + 160]
	mov	qword ptr [rsp + 32], rax
	mov	rcx, qword ptr [rbp - 32]
	mov	rdx, rdi
	mov	r8d, ebx
	mov	r9d, r15d
	mov	r15d, dword ptr [rbp + 208]
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@72"
	test	r15d, r15d
	jle	.LBB9_6
	vcvttpd2dq	xmm0, xmm6
	vcvtdq2pd	xmm0, xmm0
	vsubsd	xmm0, xmm6, xmm0
	mov	eax, r15d
	xor	ecx, ecx
	.p2align	4
.LBB9_5:
	vcvtsi2sd	xmm1, xmm15, ecx
	vmulsd	xmm1, xmm7, xmm1
	vaddsd	xmm1, xmm0, xmm1
	vcvttsd2si	edx, xmm1
	lea	r8d, [4*rdx]
	movsxd	r8, r8d
	vmovss	xmm2, dword ptr [r14 + r8]
	lea	edx, [4*rdx + 4]
	movsxd	rdx, edx
	vmovss	xmm3, dword ptr [r14 + rdx]
	movsxd	rsi, esi
	vroundsd	xmm4, xmm1, xmm1, 9
	vsubsd	xmm1, xmm1, xmm4
	vcvtsd2ss	xmm1, xmm1, xmm1
	vsubss	xmm3, xmm3, xmm2
	vmulss	xmm1, xmm3, xmm1
	vaddss	xmm1, xmm2, xmm1
	vaddss	xmm1, xmm1, dword ptr [r13 + rsi]
	vmovss	dword ptr [r13 + rsi], xmm1
	inc	rcx
	add	esi, 4
	cmp	rax, rcx
	jne	.LBB9_5
.LBB9_6:
	mov	rax, qword ptr [rbp - 40]
	mov	dword ptr [rdi + 56], eax
	mov	eax, dword ptr [rdi + 36]
	cmp	ebx, eax
	jbe	.LBB9_7
	cmp	ebx, 3
	mov	ecx, 2
	cmovae	ecx, ebx
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 16
	or	rdx, rcx
	cmp	rdx, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, rdx
	inc	r8d
	cmp	eax, r8d
	je	.LBB9_7
	lea	rcx, [rdi + 24]
	lea	rdx, [rdi + 40]
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
.LBB9_7:
	mov	dword ptr [rdi + 32], ebx
	jmp	.LBB9_8
.LBB9_11:
	mov	dword ptr [rsp + 80], ebx
	mov	qword ptr [rsp + 72], r13
	vmovss	dword ptr [rsp + 64], xmm8
	movzx	eax, byte ptr [rbp + 184]
	mov	byte ptr [rsp + 56], al
	vmovsd	qword ptr [rsp + 48], xmm7
	mov	qword ptr [rsp + 32], r12
	mov	rcx, r15
	mov	rdx, rdi
	mov	r8d, r14d
	mov	r9d, esi
	vmovapd	xmm5, xmm6
	call	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
.LBB9_8:
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	vmovaps	xmm7, xmmword ptr [rbp]
	vmovaps	xmm8, xmmword ptr [rbp + 16]
	vmovaps	xmm9, xmmword ptr [rbp + 32]
	.seh_startepilogue
	add	rsp, 184
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

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
	.p2align	4
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@80"
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
	sub	rsp, 168
	.seh_stackalloc 168
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovapd	xmmword ptr [rbp + 16], xmm7
	.seh_savexmm xmm7, 144
	vmovaps	xmmword ptr [rbp], xmm6
	.seh_savexmm xmm6, 128
	.seh_endprologue
	vmovsd	xmm6, qword ptr [rbp + 144]
	mov	r14d, dword ptr [rbp + 176]
	vxorpd	xmm0, xmm0, xmm0
	vucomisd	xmm0, xmm5
	seta	r8b
	test	r14d, r14d
	setg	al
	and	r8b, al
	xor	edi, edi
	cmp	r8b, 1
	jne	.LBB10_4
	xor	edi, edi
	.p2align	4
.LBB10_2:
	vaddsd	xmm5, xmm6, xmm5
	inc	edi
	cmp	edi, r14d
	setl	al
	vucomisd	xmm0, xmm5
	jbe	.LBB10_4
	cmp	edi, r14d
	jl	.LBB10_2
.LBB10_4:
	test	al, al
	je	.LBB10_11
	mov	rax, qword ptr [rbp + 128]
	mov	r9d, dword ptr [rax + 144]
	vcvtsi2sd	xmm0, xmm4, r9d
	vucomisd	xmm5, xmm0
	jae	.LBB10_11
	movzx	ebx, byte ptr [rbp + 152]
	lea	r8d, [r9 - 1]
	vcvtsi2sd	xmm0, xmm4, r8d
	vsubsd	xmm0, xmm0, xmm5
	vdivsd	xmm0, xmm0, xmm6
	sub	r14d, edi
	vcvttsd2si	r8d, xmm0
	cmp	r14d, r8d
	cmovge	r14d, r8d
	vcvttsd2si	esi, xmm5
	lea	r10d, [r14 - 1]
	vcvtsi2sd	xmm0, xmm4, r10d
	vmulsd	xmm0, xmm6, xmm0
	vaddsd	xmm0, xmm5, xmm0
	vroundsd	xmm0, xmm0, xmm0, 10
	vcvttsd2si	r8d, xmm0
	xor	r11d, r11d
	cmp	r9d, r8d
	setle	r11b
	cmovle	r14d, r10d
	sub	r8d, esi
	sub	r8d, r11d
	inc	r8d
	mov	rcx, qword ptr [rcx + 176]
	mov	ecx, dword ptr [rcx + 20]
	mov	r9d, dword ptr [rdx + 56]
	inc	r9d
	mov	dword ptr [rdx + 56], r9d
	mov	r10d, dword ptr [rdx + 32]
	mov	rdx, qword ptr [rdx + 48]
	mov	qword ptr [rbp - 80], rdx
	mov	dword ptr [rbp - 72], r10d
	mov	dword ptr [rbp - 68], r9d
	vxorpd	xmm0, xmm0, xmm0
	vmovupd	ymmword ptr [rbp - 64], ymm0
	mov	dword ptr [rbp - 32], ecx
	mov	dword ptr [rbp - 28], 0
	movzx	ecx, byte ptr [rax + 149]
	cmp	ecx, 2
	je	.LBB10_19
	cmp	ecx, 1
	je	.LBB10_15
	test	ecx, ecx
	jne	.LBB10_9
	movsxd	rcx, dword ptr [rax]
	add	rcx, rax
	test	bl, bl
	je	.LBB10_13
	movsxd	rax, dword ptr [rcx + 8]
	lea	rcx, [rcx + rax + 8]
	lea	eax, [4*rsi]
	cdqe
	add	rax, rcx
	test	r14d, r14d
	jg	.LBB10_22
	jmp	.LBB10_24
.LBB10_15:
	vmovapd	xmm7, xmm5
	movsxd	r15, dword ptr [rax]
	add	r15, rax
	lea	rcx, [rbp - 80]
	mov	edx, r8d
	vzeroupper
	call	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	mov	qword ptr [rbp - 12], rax
	mov	dword ptr [rbp - 4], edx
	lea	eax, [rsi + 1023]
	test	esi, esi
	cmovns	eax, esi
	sar	eax, 10
	cdqe
	test	bl, bl
	je	.LBB10_16
	movsxd	rcx, dword ptr [r15 + 24]
	lea	rcx, [r15 + rcx + 24]
	lea	r8, [r15 + 8]
	mov	edx, 12
	jmp	.LBB10_18
.LBB10_19:
	movsxd	rcx, dword ptr [rax]
	add	rcx, rax
	lea	rax, [rbp - 80]
	mov	qword ptr [rsp + 32], rax
	mov	edx, esi
	mov	r9d, ebx
	vmovapd	xmm7, xmm5
	vzeroupper
	call	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	vmovapd	xmm5, xmm7
	test	r14d, r14d
	jg	.LBB10_22
	jmp	.LBB10_24
.LBB10_9:
	xor	eax, eax
	test	r14d, r14d
	jg	.LBB10_22
	jmp	.LBB10_24
.LBB10_16:
	movsxd	rcx, dword ptr [r15 + 16]
	lea	rcx, [r15 + rcx + 16]
	mov	edx, 4
	mov	r8, r15
.LBB10_18:
	movsxd	r10, dword ptr [r8]
	add	r10, r8
	movsx	r9d, word ptr [rcx + 4*rax]
	movsx	ecx, word ptr [rcx + 4*rax + 2]
	mov	edx, dword ptr [r15 + rdx]
	mov	qword ptr [rbp - 24], r10
	mov	dword ptr [rbp - 16], edx
	shl	eax, 10
	mov	dword ptr [rsp + 40], eax
	mov	dword ptr [rsp + 32], ecx
	lea	rcx, [rbp - 24]
	lea	rdx, [rbp - 12]
	mov	r8d, esi
	call	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@40"
	mov	rax, qword ptr [rbp - 12]
	vmovapd	xmm5, xmm7
	test	r14d, r14d
	jg	.LBB10_22
	jmp	.LBB10_24
.LBB10_13:
	movsxd	rdx, dword ptr [rcx]
	lea	eax, [4*rsi]
	cdqe
	add	rax, rcx
	add	rax, rdx
	test	r14d, r14d
	jle	.LBB10_24
.LBB10_22:
	vmovss	xmm0, dword ptr [rbp + 160]
	mov	rcx, qword ptr [rbp + 168]
	vcvtsi2sd	xmm1, xmm4, esi
	vsubsd	xmm1, xmm5, xmm1
	mov	edx, r14d
	shl	edi, 2
	xor	r8d, r8d
	.p2align	4
.LBB10_23:
	vcvtsi2sd	xmm2, xmm15, r8d
	vmulsd	xmm2, xmm6, xmm2
	vaddsd	xmm2, xmm1, xmm2
	vcvttsd2si	r9d, xmm2
	lea	r10d, [4*r9]
	movsxd	r10, r10d
	vmovss	xmm3, dword ptr [rax + r10]
	lea	r9d, [4*r9 + 4]
	movsxd	r9, r9d
	vmovss	xmm4, dword ptr [rax + r9]
	movsxd	rdi, edi
	vroundsd	xmm5, xmm2, xmm2, 9
	vsubsd	xmm2, xmm2, xmm5
	vcvtsd2ss	xmm2, xmm2, xmm2
	vsubss	xmm4, xmm4, xmm3
	vmulss	xmm2, xmm4, xmm2
	vaddss	xmm2, xmm3, xmm2
	vmulss	xmm2, xmm0, xmm2
	vaddss	xmm2, xmm2, dword ptr [rcx + rdi]
	vmovss	dword ptr [rcx + rdi], xmm2
	inc	r8
	add	edi, 4
	cmp	rdx, r8
	jne	.LBB10_23
.LBB10_24:
	mov	rsi, qword ptr [rbp - 80]
	mov	edi, dword ptr [rbp - 72]
	mov	eax, dword ptr [rbp - 68]
	dec	eax
	mov	dword ptr [rsi + 56], eax
	mov	eax, dword ptr [rsi + 36]
	cmp	edi, eax
	jle	.LBB10_10
	cmp	edi, 3
	mov	ecx, 2
	cmovae	ecx, edi
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 16
	or	rdx, rcx
	cmp	rdx, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, rdx
	inc	r8d
	cmp	eax, r8d
	je	.LBB10_10
	lea	rcx, [rsi + 24]
	lea	rdx, [rsi + 40]
	vzeroupper
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
.LBB10_10:
	mov	dword ptr [rsi + 32], edi
.LBB10_11:
	vmovaps	xmm6, xmmword ptr [rbp]
	vmovaps	xmm7, xmmword ptr [rbp + 16]
	.seh_startepilogue
	add	rsp, 168
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.def	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.globl	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
	.p2align	4
"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32":
.seh_proc "Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@32"
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
	sub	rsp, 56
	.seh_stackalloc 56
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	.seh_endprologue
	xor	r15d, r15d
	test	r9, r9
	je	.LBB11_1
	mov	rsi, r9
	mov	rdi, rcx
	imul	rsi, rdx
	lea	rbx, [rcx + 24]
	mov	rcx, qword ptr [rcx + 24]
	test	rcx, rcx
	je	.LBB11_4
	mov	r14d, dword ptr [rdi + 32]
	test	r14, r14
	je	.LBB11_4
	lea	eax, [r14 - 1]
	lea	rax, [rax + 2*rax]
	mov	rdx, qword ptr [rcx + 8*rax + 8]
	add	rdx, qword ptr [rcx + 8*rax]
	lea	r9, [r8 - 1]
	add	r9, rdx
	mov	r15, r8
	neg	r15
	and	r15, r9
	cmp	r8, 1
	mov	r12d, dword ptr [rcx + 8*rax + 16]
	mov	rax, qword ptr [rdi]
	cmovb	r15, rdx
	mov	rdx, r12
	shl	rdx, 4
	mov	r9, qword ptr [rax + rdx + 8]
	sub	r9, r15
	cmp	rsi, r9
	jbe	.LBB11_19
	lea	r15d, [r12 + 1]
.LBB11_4:
	cmp	dword ptr [rdi + 8], r15d
	jle	.LBB11_12
	mov	rax, qword ptr [rdi]
	mov	r12d, r15d
	mov	r13, r12
	shl	r13, 4
	cmp	qword ptr [rax + r13 + 8], rsi
	jae	.LBB11_31
	mov	r14, rbx
	mov	qword ptr [rbp], r8
	mov	rbx, qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	.p2align	4
.LBB11_7:
	mov	rax, qword ptr [rdi]
	mov	rcx, qword ptr [rax + r13]
	mov	edx, 4
	call	rbx
	inc	r12
	mov	eax, dword ptr [rdi + 8]
	add	r13, 16
	cmp	r12, rax
	jb	.LBB11_7
	mov	eax, dword ptr [rdi + 12]
	cmp	r15d, eax
	mov	rbx, r14
	jbe	.LBB11_11
	cmp	r15d, 5
	mov	ecx, 4
	cmovae	ecx, r15d
	dec	rcx
	mov	rdx, rcx
	shr	rdx
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 2
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 4
	or	rdx, rcx
	mov	rcx, rdx
	shr	rcx, 8
	or	rcx, rdx
	mov	rdx, rcx
	shr	rdx, 16
	or	rdx, rcx
	cmp	rdx, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, rdx
	inc	r8d
	cmp	eax, r8d
	je	.LBB11_11
	lea	rdx, [rdi + 16]
	mov	rcx, rdi
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
.LBB11_11:
	mov	dword ptr [rdi + 8], r15d
	mov	r8, qword ptr [rbp]
.LBB11_12:
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"]
	movsxd	r14, dword ptr [rax + 8]
	cmp	rsi, r14
	cmova	r14, rsi
	add	r14, 63
	and	r14, -64
	mov	qword ptr [rsp + 32], 0
	mov	rcx, r14
	mov	edx, r8d
	mov	r8d, 4
	mov	r9d, 1
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	movsxd	r12, dword ptr [rdi + 8]
	mov	ecx, dword ptr [rdi + 12]
	cmp	r12d, ecx
	jge	.LBB11_13
	mov	rcx, qword ptr [rdi]
	shl	r12, 4
	mov	qword ptr [rcx + r12], rax
	mov	qword ptr [rcx + r12 + 8], r14
	inc	dword ptr [rdi + 8]
	movsxd	r14, dword ptr [rdi + 32]
	mov	ecx, dword ptr [rdi + 36]
	cmp	r14d, ecx
	jl	.LBB11_37
.LBB11_27:
	lea	r12d, [r14 + 1]
	cmp	r12d, ecx
	jle	.LBB11_30
	cmp	r12d, 3
	mov	edx, 2
	cmovae	edx, r12d
	dec	rdx
	mov	r8, rdx
	shr	r8
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 2
	or	rdx, r8
	mov	r8, rdx
	shr	r8, 4
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 8
	or	rdx, r8
	mov	r9, rdx
	shr	r9, 16
	or	r9, rdx
	cmp	r9, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, r9
	inc	r8d
	cmp	ecx, r8d
	je	.LBB11_30
	lea	rdx, [rdi + 40]
	mov	rcx, rbx
	mov	rbx, rax
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, rbx
.LBB11_30:
	mov	dword ptr [rdi + 32], r12d
	mov	rcx, qword ptr [rdi + 24]
	jmp	.LBB11_36
.LBB11_1:
	xor	eax, eax
	jmp	.LBB11_39
.LBB11_13:
	lea	r13d, [r12 + 1]
	cmp	r13d, ecx
	jle	.LBB11_16
	cmp	r13d, 5
	mov	edx, 4
	cmovae	edx, r13d
	dec	rdx
	mov	r8, rdx
	shr	r8
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 2
	or	rdx, r8
	mov	r8, rdx
	shr	r8, 4
	or	r8, rdx
	mov	rdx, r8
	shr	rdx, 8
	or	rdx, r8
	mov	r9, rdx
	shr	r9, 16
	or	r9, rdx
	cmp	r9, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, r9
	inc	r8d
	cmp	ecx, r8d
	je	.LBB11_16
	lea	rdx, [rdi + 16]
	mov	rcx, rdi
	mov	qword ptr [rbp], rbx
	mov	rbx, rax
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, rbx
	mov	rbx, qword ptr [rbp]
.LBB11_16:
	mov	dword ptr [rdi + 8], r13d
	mov	rcx, qword ptr [rdi]
	shl	r12, 4
	mov	qword ptr [rcx + r12], rax
	mov	qword ptr [rcx + r12 + 8], r14
	movsxd	r14, dword ptr [rdi + 32]
	mov	ecx, dword ptr [rdi + 36]
	cmp	r14d, ecx
	jge	.LBB11_27
.LBB11_37:
	mov	rcx, qword ptr [rdi + 24]
	jmp	.LBB11_38
.LBB11_31:
	mov	rax, qword ptr [rax + r13]
	movsxd	r14, dword ptr [rdi + 32]
	mov	edx, dword ptr [rdi + 36]
	cmp	r14d, edx
	jge	.LBB11_32
.LBB11_38:
	lea	rdx, [r14 + 2*r14]
	mov	qword ptr [rcx + 8*rdx], 0
	mov	qword ptr [rcx + 8*rdx + 8], rsi
	mov	dword ptr [rcx + 8*rdx + 16], r15d
	mov	dword ptr [rcx + 8*rdx + 20], 0
	inc	dword ptr [rdi + 32]
	jmp	.LBB11_39
.LBB11_19:
	mov	rax, qword ptr [rax + rdx]
	mov	edx, dword ptr [rdi + 36]
	cmp	r14d, edx
	jge	.LBB11_20
	lea	rdx, [r14 + 2*r14]
	mov	qword ptr [rcx + 8*rdx], r15
	mov	qword ptr [rcx + 8*rdx + 8], rsi
	mov	dword ptr [rcx + 8*rdx + 16], r12d
	mov	dword ptr [rcx + 8*rdx + 20], 0
	inc	dword ptr [rdi + 32]
	add	rax, r15
	jmp	.LBB11_39
.LBB11_32:
	lea	r12d, [r14 + 1]
	cmp	r12d, edx
	jle	.LBB11_35
	cmp	r12d, 3
	mov	r8d, 2
	cmovae	r8d, r12d
	dec	r8
	mov	r9, r8
	shr	r9
	or	r9, r8
	mov	r8, r9
	shr	r8, 2
	or	r8, r9
	mov	r9, r8
	shr	r9, 4
	or	r9, r8
	mov	r8, r9
	shr	r8, 8
	or	r8, r9
	mov	r9, r8
	shr	r9, 16
	or	r9, r8
	cmp	r9, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, r9
	inc	r8d
	cmp	edx, r8d
	je	.LBB11_35
	lea	rdx, [rdi + 40]
	mov	rcx, rbx
	mov	rbx, rax
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, rbx
	mov	rcx, qword ptr [rdi + 24]
.LBB11_35:
	mov	dword ptr [rdi + 32], r12d
.LBB11_36:
	lea	rdx, [r14 + 2*r14]
	mov	qword ptr [rcx + 8*rdx], 0
	mov	qword ptr [rcx + 8*rdx + 8], rsi
	mov	dword ptr [rcx + 8*rdx + 16], r15d
	mov	dword ptr [rcx + 8*rdx + 20], 0
.LBB11_39:
	.seh_startepilogue
	add	rsp, 56
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
.LBB11_20:
	lea	r13d, [r14 + 1]
	cmp	r13d, edx
	jle	.LBB11_23
	mov	r8d, r14d
	shr	r8d
	or	r8, r14
	mov	r9d, r8d
	shr	r9d, 2
	or	r9, r8
	mov	r8d, r9d
	shr	r8d, 4
	or	r8, r9
	mov	r9d, r8d
	shr	r9d, 8
	or	r9, r8
	mov	r10d, r9d
	shr	r10d, 16
	or	r10, r9
	cmp	r10, 2147483646
	mov	r8d, 2147483646
	cmovb	r8, r10
	inc	r8d
	cmp	edx, r8d
	je	.LBB11_23
	lea	rdx, [rdi + 40]
	mov	rcx, rbx
	mov	rbx, rax
	call	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	mov	rax, rbx
	mov	rcx, qword ptr [rdi + 24]
.LBB11_23:
	mov	dword ptr [rdi + 32], r13d
	lea	rdx, [r14 + 2*r14]
	mov	qword ptr [rcx + 8*rdx], r15
	mov	qword ptr [rcx + 8*rdx + 8], rsi
	mov	dword ptr [rcx + 8*rdx + 16], r12d
	mov	dword ptr [rcx + 8*rdx + 20], 0
	add	rax, r15
	jmp	.LBB11_39
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
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
	sub	rsp, 80
	.seh_stackalloc 80
	lea	rbp, [rsp + 80]
	.seh_setframe rbp, 80
	.seh_endprologue
	mov	esi, r8d
	mov	rbx, rdx
	mov	rdi, rcx
	xor	r14d, r14d
	test	r8d, r8d
	cmovle	esi, r14d
	jle	.LBB12_1
	movzx	r8d, word ptr [rbx]
	cmp	r8, 64
	movzx	eax, word ptr [rbx + 2]
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 32], ymm0
	mov	dword ptr [rbp - 24], esi
	mov	dword ptr [rbp - 16], 24
	mov	byte ptr [rbp - 8], 6
	mov	word ptr [rbp - 20], r8w
	mov	word ptr [rbp - 18], ax
	jb	.LBB12_16
	test	ax, ax
	jne	.LBB12_12
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	eax, word ptr [rax + 2*r8]
	mov	word ptr [rbp - 18], ax
.LBB12_12:
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	r8d, 4
	mov	rcx, qword ptr [rax + r8 + 8]
	lea	rdx, [rbp - 32]
	vzeroupper
	call	qword ptr [rax + r8]
	mov	r14, qword ptr [rbp - 32]
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	jne	.LBB12_14
	jmp	.LBB12_8
.LBB12_16:
	mov	eax, esi
	shl	rax, 3
	lea	rcx, [rax + 2*rax]
	mov	qword ptr [rsp + 32], 0
	mov	edx, 64
	mov	r9d, 1
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	r14, rax
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	je	.LBB12_8
.LBB12_14:
	mov	eax, dword ptr [rdi + 12]
	test	eax, eax
	jle	.LBB12_1
	cmp	esi, eax
	cmovb	eax, esi
	shl	rax, 3
	lea	r8, [rax + 2*rax]
	mov	rcx, r14
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
.LBB12_1:
	mov	rcx, qword ptr [rdi]
	test	rcx, rcx
	je	.LBB12_8
	mov	eax, dword ptr [rdi + 12]
	mov	qword ptr [rbp - 8], 3
	mov	dword ptr [rbp - 12], eax
	mov	qword ptr [rbp - 32], rcx
	mov	dword ptr [rbp - 16], 24
	mov	dword ptr [rbp - 24], 0
	movzx	eax, word ptr [rbx]
	cmp	rax, 64
	movzx	edx, word ptr [rbx + 2]
	mov	word ptr [rbp - 20], ax
	mov	word ptr [rbp - 18], dx
	jb	.LBB12_6
	test	dx, dx
	jne	.LBB12_5
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	ecx, word ptr [rcx + 2*rax]
	mov	word ptr [rbp - 18], cx
.LBB12_5:
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	eax, 4
	mov	rcx, qword ptr [r8 + rax + 8]
	lea	rdx, [rbp - 32]
	call	qword ptr [r8 + rax]
	jmp	.LBB12_8
.LBB12_6:
	cmp	eax, 1
	je	.LBB12_8
	mov	edx, eax
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB12_8:
	mov	qword ptr [rdi], r14
	mov	dword ptr [rdi + 12], esi
	mov	eax, dword ptr [rdi + 8]
	cmp	eax, esi
	cmovl	esi, eax
	mov	dword ptr [rdi + 8], esi
	.seh_startepilogue
	add	rsp, 80
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
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
	sub	rsp, 80
	.seh_stackalloc 80
	lea	rbp, [rsp + 80]
	.seh_setframe rbp, 80
	.seh_endprologue
	mov	esi, r8d
	mov	rbx, rdx
	mov	rdi, rcx
	xor	r14d, r14d
	test	r8d, r8d
	cmovle	esi, r14d
	jle	.LBB13_1
	movzx	r8d, word ptr [rbx]
	cmp	r8, 64
	movzx	eax, word ptr [rbx + 2]
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 32], ymm0
	mov	dword ptr [rbp - 24], esi
	mov	dword ptr [rbp - 16], 24
	mov	byte ptr [rbp - 8], 6
	mov	word ptr [rbp - 20], r8w
	mov	word ptr [rbp - 18], ax
	jb	.LBB13_16
	test	ax, ax
	jne	.LBB13_12
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	eax, word ptr [rax + 2*r8]
	mov	word ptr [rbp - 18], ax
.LBB13_12:
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	r8d, 4
	mov	rcx, qword ptr [rax + r8 + 8]
	lea	rdx, [rbp - 32]
	vzeroupper
	call	qword ptr [rax + r8]
	mov	r14, qword ptr [rbp - 32]
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	jne	.LBB13_14
	jmp	.LBB13_8
.LBB13_16:
	mov	eax, esi
	shl	rax, 3
	lea	rcx, [rax + 2*rax]
	mov	qword ptr [rsp + 32], 0
	mov	edx, 64
	mov	r9d, 1
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	r14, rax
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	je	.LBB13_8
.LBB13_14:
	mov	eax, dword ptr [rdi + 12]
	test	eax, eax
	jle	.LBB13_1
	cmp	esi, eax
	cmovb	eax, esi
	shl	rax, 3
	lea	r8, [rax + 2*rax]
	mov	rcx, r14
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
.LBB13_1:
	mov	rcx, qword ptr [rdi]
	test	rcx, rcx
	je	.LBB13_8
	mov	eax, dword ptr [rdi + 12]
	mov	qword ptr [rbp - 8], 3
	mov	dword ptr [rbp - 12], eax
	mov	qword ptr [rbp - 32], rcx
	mov	dword ptr [rbp - 16], 24
	mov	dword ptr [rbp - 24], 0
	movzx	eax, word ptr [rbx]
	cmp	rax, 64
	movzx	edx, word ptr [rbx + 2]
	mov	word ptr [rbp - 20], ax
	mov	word ptr [rbp - 18], dx
	jb	.LBB13_6
	test	dx, dx
	jne	.LBB13_5
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	ecx, word ptr [rcx + 2*rax]
	mov	word ptr [rbp - 18], cx
.LBB13_5:
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	eax, 4
	mov	rcx, qword ptr [r8 + rax + 8]
	lea	rdx, [rbp - 32]
	call	qword ptr [r8 + rax]
	jmp	.LBB13_8
.LBB13_6:
	cmp	eax, 1
	je	.LBB13_8
	mov	edx, eax
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB13_8:
	mov	qword ptr [rdi], r14
	mov	dword ptr [rdi + 12], esi
	mov	eax, dword ptr [rdi + 8]
	cmp	eax, esi
	cmovl	esi, eax
	mov	dword ptr [rdi + 8], esi
	.seh_startepilogue
	add	rsp, 80
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
	.p2align	4
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@24"
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
	sub	rsp, 80
	.seh_stackalloc 80
	lea	rbp, [rsp + 80]
	.seh_setframe rbp, 80
	.seh_endprologue
	mov	esi, r8d
	mov	rbx, rdx
	mov	rdi, rcx
	xor	r14d, r14d
	test	r8d, r8d
	cmovle	esi, r14d
	jle	.LBB14_1
	movzx	r8d, word ptr [rbx]
	cmp	r8, 64
	movzx	eax, word ptr [rbx + 2]
	vxorps	xmm0, xmm0, xmm0
	vmovups	ymmword ptr [rbp - 32], ymm0
	mov	dword ptr [rbp - 24], esi
	mov	dword ptr [rbp - 16], 16
	mov	byte ptr [rbp - 8], 6
	mov	word ptr [rbp - 20], r8w
	mov	word ptr [rbp - 18], ax
	jb	.LBB14_16
	test	ax, ax
	jne	.LBB14_12
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	eax, word ptr [rax + 2*r8]
	mov	word ptr [rbp - 18], ax
.LBB14_12:
	mov	rax, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	r8d, 4
	mov	rcx, qword ptr [rax + r8 + 8]
	lea	rdx, [rbp - 32]
	vzeroupper
	call	qword ptr [rax + r8]
	mov	r14, qword ptr [rbp - 32]
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	jne	.LBB14_14
	jmp	.LBB14_8
.LBB14_16:
	mov	ecx, esi
	shl	rcx, 4
	mov	qword ptr [rsp + 32], 0
	mov	edx, 64
	mov	r9d, 1
	vzeroupper
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	r14, rax
	mov	rdx, qword ptr [rdi]
	test	rdx, rdx
	je	.LBB14_8
.LBB14_14:
	mov	r8d, dword ptr [rdi + 12]
	test	r8d, r8d
	jle	.LBB14_1
	cmp	esi, r8d
	cmovb	r8d, esi
	shl	r8, 4
	mov	rcx, r14
	xor	r9d, r9d
	call	burst.memcpy.inline.AVX2.i64@@32
.LBB14_1:
	mov	rcx, qword ptr [rdi]
	test	rcx, rcx
	je	.LBB14_8
	mov	eax, dword ptr [rdi + 12]
	mov	qword ptr [rbp - 8], 3
	mov	dword ptr [rbp - 12], eax
	mov	qword ptr [rbp - 32], rcx
	mov	dword ptr [rbp - 16], 16
	mov	dword ptr [rbp - 24], 0
	movzx	eax, word ptr [rbx]
	cmp	rax, 64
	movzx	edx, word ptr [rbx + 2]
	mov	word ptr [rbp - 20], ax
	mov	word ptr [rbp - 18], dx
	jb	.LBB14_6
	test	dx, dx
	jne	.LBB14_5
	mov	rcx, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	movzx	ecx, word ptr [rcx + 2*rax]
	mov	word ptr [rbp - 18], cx
.LBB14_5:
	mov	r8, qword ptr [rip + "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	shl	eax, 4
	mov	rcx, qword ptr [r8 + rax + 8]
	lea	rdx, [rbp - 32]
	call	qword ptr [r8 + rax]
	jmp	.LBB14_8
.LBB14_6:
	cmp	eax, 1
	je	.LBB14_8
	mov	edx, eax
	call	qword ptr [rip + "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
.LBB14_8:
	mov	qword ptr [rdi], r14
	mov	dword ptr [rdi + 12], esi
	mov	eax, dword ptr [rdi + 8]
	cmp	eax, esi
	cmovl	esi, eax
	mov	dword ptr [rdi + 8], esi
	.seh_startepilogue
	add	rsp, 80
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.Sleef_fmod_avx2@@16;
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@7fffffffffffffff7fffffffffffffff
	.section	.rdata,"dr",discard,__xmm@7fffffffffffffff7fffffffffffffff
	.p2align	4, 0x0
__xmm@7fffffffffffffff7fffffffffffffff:
	.quad	0x7fffffffffffffff
	.quad	0x7fffffffffffffff
	.globl	__real@0010000000000000
	.section	.rdata,"dr",discard,__real@0010000000000000
	.p2align	3, 0x0
__real@0010000000000000:
	.quad	0x0010000000000000
	.globl	__real@4350000000000000
	.section	.rdata,"dr",discard,__real@4350000000000000
	.p2align	3, 0x0
__real@4350000000000000:
	.quad	0x4350000000000000
	.globl	__xmm@3ff00000000000003ff0000000000000
	.section	.rdata,"dr",discard,__xmm@3ff00000000000003ff0000000000000
	.p2align	4, 0x0
__xmm@3ff00000000000003ff0000000000000:
	.quad	0x3ff0000000000000
	.quad	0x3ff0000000000000
	.globl	__xmm@3c900000000000003c90000000000000
	.section	.rdata,"dr",discard,__xmm@3c900000000000003c90000000000000
	.p2align	4, 0x0
__xmm@3c900000000000003c90000000000000:
	.quad	0x3c90000000000000
	.quad	0x3c90000000000000
	.globl	__real@4008000000000000
	.section	.rdata,"dr",discard,__real@4008000000000000
	.p2align	3, 0x0
__real@4008000000000000:
	.quad	0x4008000000000000
	.globl	__xmm@80000000000000008000000000000000
	.section	.rdata,"dr",discard,__xmm@80000000000000008000000000000000
	.p2align	4, 0x0
__xmm@80000000000000008000000000000000:
	.quad	0x8000000000000000
	.quad	0x8000000000000000
	.globl	__real@3e00000000000000
	.section	.rdata,"dr",discard,__real@3e00000000000000
	.p2align	3, 0x0
__real@3e00000000000000:
	.quad	0x3e00000000000000
	.globl	__real@c1e0000000000000
	.section	.rdata,"dr",discard,__real@c1e0000000000000
	.p2align	3, 0x0
__real@c1e0000000000000:
	.quad	0xc1e0000000000000
	.globl	__real@4330000000000000
	.section	.rdata,"dr",discard,__real@4330000000000000
	.p2align	3, 0x0
__real@4330000000000000:
	.quad	0x4330000000000000
	.globl	__xmm@40000000000000004000000000000000
	.section	.rdata,"dr",discard,__xmm@40000000000000004000000000000000
	.p2align	4, 0x0
__xmm@40000000000000004000000000000000:
	.quad	0x4000000000000000
	.quad	0x4000000000000000
	.globl	__xmm@7ff80000000000007ff8000000000000
	.section	.rdata,"dr",discard,__xmm@7ff80000000000007ff8000000000000
	.p2align	4, 0x0
__xmm@7ff80000000000007ff8000000000000:
	.quad	0x7ff8000000000000
	.quad	0x7ff8000000000000
	.globl	__real@7fffffffffffffff
	.section	.rdata,"dr",discard,__real@7fffffffffffffff
	.p2align	3, 0x0
__real@7fffffffffffffff:
	.quad	0x7fffffffffffffff
	.globl	__real@4000000000000000
	.section	.rdata,"dr",discard,__real@4000000000000000
	.p2align	3, 0x0
__real@4000000000000000:
	.quad	0x4000000000000000
	.section	.text,"xr",discard,burst.Sleef_fmod_avx2@@16
	.globl	burst.Sleef_fmod_avx2@@16
	.p2align	4
burst.Sleef_fmod_avx2@@16:
	sub	rsp, 216
	vmovapd	xmmword ptr [rsp + 192], xmm15
	vmovapd	xmmword ptr [rsp + 176], xmm14
	vmovapd	xmmword ptr [rsp + 160], xmm13
	vmovapd	xmmword ptr [rsp + 144], xmm12
	vmovaps	xmmword ptr [rsp + 128], xmm11
	vmovaps	xmmword ptr [rsp + 112], xmm10
	vmovaps	xmmword ptr [rsp + 96], xmm9
	vmovaps	xmmword ptr [rsp + 80], xmm8
	vmovaps	xmmword ptr [rsp + 64], xmm7
	vmovdqa	xmmword ptr [rsp + 48], xmm6
	vmovddup	xmm2, qword ptr [rip + __real@7fffffffffffffff]
	vandpd	xmm5, xmm0, xmm2
	vandpd	xmm1, xmm1, xmm2
	vmovsd	xmm2, qword ptr [rip + __real@0010000000000000]
	vucomisd	xmm2, xmm1
	vcmpltsd	xmm4, xmm1, xmm2
	vmovddup	xmm2, qword ptr [rip + __real@3ff0000000000000]
	jbe	.LBB15_2
	vmovsd	xmm3, qword ptr [rip + __real@4350000000000000]
	vmulsd	xmm5, xmm5, xmm3
	vmulsd	xmm1, xmm1, xmm3
.LBB15_2:
	vblendvpd	xmm2, xmm2, xmmword ptr [rip + __xmm@3c900000000000003c90000000000000], xmm4
	vmovapd	xmmword ptr [rsp], xmm2
	vmovsd	xmm4, qword ptr [rip + __real@3ff0000000000000]
	vdivsd	xmm2, xmm4, xmm1
	vxorpd	xmm3, xmm3, xmm3
	vucomisd	xmm2, xmm3
	vpxor	xmm6, xmm6, xmm6
	jne	.LBB15_3
	jnp	.LBB15_4
.LBB15_3:
	vmovq	rax, xmm2
	dec	rax
	vmovq	xmm6, rax
.LBB15_4:
	vmovapd	xmmword ptr [rsp + 32], xmm0
	vmovq	rax, xmm0
	vmulsd	xmm7, xmm1, qword ptr [rip + __real@4008000000000000]
	vaddsd	xmm8, xmm1, xmm1
	vxorpd	xmm9, xmm1, xmmword ptr [rip + __xmm@80000000000000008000000000000000]
	vmovq	rcx, xmm9
	and	rcx, -134217728
	vmovq	xmm10, rcx
	vsubsd	xmm11, xmm9, xmm10
	mov	ecx, 1
	vmovddup	xmm3, qword ptr [rip + __real@4000000000000000]
	vxorpd	xmm2, xmm2, xmm2
	vmovapd	xmmword ptr [rsp + 16], xmm5
	vmovapd	xmm0, xmm5
	.p2align	4
.LBB15_5:
	vxorpd	xmm5, xmm5, xmm5
	vucomisd	xmm0, xmm5
	jne	.LBB15_6
	jnp	.LBB15_7
.LBB15_6:
	vmovq	rdx, xmm0
	dec	rdx
	vmovq	xmm5, rdx
.LBB15_7:
	vmulsd	xmm5, xmm6, xmm5
	vmulsd	xmm12, xmm5, qword ptr [rip + __real@3e00000000000000]
	vcvttpd2dq	xmm12, xmm12
	vcvtdq2pd	xmm12, xmm12
	vmulsd	xmm12, xmm12, qword ptr [rip + __real@c1e0000000000000]
	vandpd	xmm13, xmm5, xmmword ptr [rip + __xmm@7fffffffffffffff7fffffffffffffff]
	vucomisd	xmm13, qword ptr [rip + __real@4330000000000000]
	jae	.LBB15_9
	vaddsd	xmm12, xmm12, xmm5
	vcvttpd2dq	xmm13, xmm12
	vcvtdq2pd	xmm13, xmm13
	vsubsd	xmm12, xmm12, xmm13
	vsubsd	xmm5, xmm5, xmm12
.LBB15_9:
	vmovq	rdx, xmm5
	and	rdx, -2
	vmovq	xmm5, rdx
	vucomisd	xmm0, xmm1
	vcmpltsd	xmm12, xmm1, xmm0
	vblendvpd	xmm12, xmm5, xmm3, xmm12
	vcmpltsd	xmm13, xmm0, xmm7
	vblendvpd	xmm5, xmm5, xmm12, xmm13
	vmovapd	xmm12, xmm4
	ja	.LBB15_11
	vmovapd	xmm12, xmm5
.LBB15_11:
	vcmpltsd	xmm13, xmm0, xmm8
	vblendvpd	xmm5, xmm5, xmm12, xmm13
	vxorpd	xmm12, xmm12, xmm12
	vcmplesd	xmm12, xmm12, xmm2
	vandpd	xmm12, xmm12, xmm4
	vcmpeqsd	xmm13, xmm0, xmm1
	vblendvpd	xmm5, xmm5, xmm12, xmm13
	vmovq	rdx, xmm5
	and	rdx, -134217728
	vmovq	xmm12, rdx
	vsubsd	xmm13, xmm5, xmm12
	vmulsd	xmm5, xmm9, xmm5
	vmulsd	xmm14, xmm10, xmm12
	vsubsd	xmm14, xmm14, xmm5
	vmulsd	xmm15, xmm13, xmm10
	vaddsd	xmm14, xmm15, xmm14
	vmulsd	xmm12, xmm11, xmm12
	vaddsd	xmm12, xmm12, xmm14
	vmulsd	xmm13, xmm11, xmm13
	vaddsd	xmm12, xmm13, xmm12
	vaddsd	xmm13, xmm0, xmm5
	vsubsd	xmm14, xmm13, xmm0
	vsubsd	xmm15, xmm13, xmm14
	vsubsd	xmm0, xmm0, xmm15
	vsubsd	xmm5, xmm5, xmm14
	vaddsd	xmm0, xmm5, xmm0
	vaddsd	xmm2, xmm12, xmm2
	vaddsd	xmm2, xmm0, xmm2
	vaddsd	xmm0, xmm13, xmm2
	vsubsd	xmm5, xmm13, xmm0
	vaddsd	xmm2, xmm2, xmm5
	vucomisd	xmm1, xmm0
	ja	.LBB15_13
	cmp	ecx, 21
	lea	ecx, [rcx + 1]
	jb	.LBB15_5
.LBB15_13:
	vmulsd	xmm3, xmm0, qword ptr [rsp]
	vaddsd	xmm0, xmm0, xmm2
	xor	ecx, ecx
	vucomisd	xmm0, xmm1
	vmovq	rdx, xmm3
	cmovne	rcx, rdx
	cmovp	rcx, rdx
	movabs	rdx, -9223372036854775808
	and	rax, rdx
	xor	rax, rcx
	vmovq	xmm0, rax
	vmovapd	xmm2, xmmword ptr [rsp + 16]
	vcmpltsd	xmm2, xmm2, xmm1
	vblendvpd	xmm0, xmm0, xmmword ptr [rsp + 32], xmm2
	vxorpd	xmm2, xmm2, xmm2
	vcmpeqsd	xmm1, xmm1, xmm2
	vblendvpd	xmm0, xmm0, xmmword ptr [rip + __xmm@7ff80000000000007ff8000000000000], xmm1
	vmovaps	xmm6, xmmword ptr [rsp + 48]
	vmovaps	xmm7, xmmword ptr [rsp + 64]
	vmovaps	xmm8, xmmword ptr [rsp + 80]
	vmovaps	xmm9, xmmword ptr [rsp + 96]
	vmovaps	xmm10, xmmword ptr [rsp + 112]
	vmovaps	xmm11, xmmword ptr [rsp + 128]
	vmovaps	xmm12, xmmword ptr [rsp + 144]
	vmovaps	xmm13, xmmword ptr [rsp + 160]
	vmovaps	xmm14, xmmword ptr [rsp + 176]
	vmovaps	xmm15, xmmword ptr [rsp + 192]
	add	rsp, 216
	ret

	.def	burst.memcpy.inline.AVX2.i64@@32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memcpy.inline.AVX2.i64@@32
	.globl	burst.memcpy.inline.AVX2.i64@@32
	.p2align	4
burst.memcpy.inline.AVX2.i64@@32:
	cmp	r8, 32
	jb	.LBB16_1
	xor	r9d, r9d
	.p2align	4
.LBB16_3:
	lea	rax, [r9 + 32]
	vmovups	ymm0, ymmword ptr [rdx + r9]
	vmovups	ymmword ptr [rcx + r9], ymm0
	add	r9, 64
	cmp	r8, r9
	mov	r9, rax
	jae	.LBB16_3
	lea	r9, [rax + 1]
	cmp	r8, r9
	jb	.LBB16_6
	.p2align	4
.LBB16_5:
	lea	r9, [rax + 1]
	movzx	r10d, byte ptr [rdx + rax]
	mov	byte ptr [rcx + rax], r10b
	add	rax, 2
	cmp	r8, rax
	mov	rax, r9
	jae	.LBB16_5
.LBB16_6:
	vzeroupper
	ret
.LBB16_1:
	xor	eax, eax
	lea	r9, [rax + 1]
	cmp	r8, r9
	jae	.LBB16_5
	jmp	.LBB16_6

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
	jb	.LBB17_3
	xor	eax, eax
	vmovd	xmm0, edx
	vpbroadcastb	ymm0, xmm0
	.p2align	4
.LBB17_2:
	movsxd	r9, eax
	add	eax, 32
	vmovdqu	ymmword ptr [rcx + r9], ymm0
	add	r9d, 64
	cmp	r8d, r9d
	jae	.LBB17_2
.LBB17_3:
	lea	r9d, [rax + 1]
	cmp	r8d, r9d
	jb	.LBB17_5
	.p2align	4
.LBB17_4:
	movsxd	r9, eax
	inc	eax
	mov	byte ptr [rcx + r9], dl
	add	r9d, 2
	cmp	r8d, r9d
	jae	.LBB17_4
.LBB17_5:
	vzeroupper
	ret

	.section	.drectve,"yni"
	.ascii	" /EXPORT:3d466577b535881ddd547f1bda3cca2d"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d\""
	.globl	_fltused
