
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	a4e4a501d09d5a364b5bd41ae0aafdee;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	a4e4a501d09d5a364b5bd41ae0aafdee
	.p2align	4
a4e4a501d09d5a364b5bd41ae0aafdee:
.seh_proc a4e4a501d09d5a364b5bd41ae0aafdee
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
	vmovaps	xmmword ptr [rbp - 16], xmm7
	.seh_savexmm xmm7, 32
	vmovaps	xmmword ptr [rbp - 32], xmm6
	.seh_savexmm xmm6, 16
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB0_19
	test	r8d, r8d
	jle	.LBB0_19
	vmovss	xmm0, dword ptr [rbp + 112]
	mov	eax, r8d
	lea	r10, [rax - 1]
	mov	r11d, eax
	and	r11d, 2147483616
	vbroadcastss	ymm1, xmm0
	mov	esi, eax
	and	esi, 2147483644
	mov	rdi, rsi
	neg	rdi
	mov	qword ptr [rbp - 40], rdi
	xor	ebx, ebx
	mov	r14, r10
	shr	r14, 32
	xor	r15d, r15d
	jmp	.LBB0_4
	.p2align	4
.LBB0_3:
	inc	r15d
	add	ebx, r8d
	cmp	r15d, r9d
	je	.LBB0_19
.LBB0_4:
	cmp	r8d, 4
	vcvtsi2ss	xmm2, xmm15, r15d
	jb	.LBB0_5
	mov	r12d, r15d
	imul	r12d, r8d
	test	r14, r14
	setne	r13b
	lea	edi, [r12 + r10]
	cmp	edi, r12d
	setl	dil
	or	dil, r13b
	je	.LBB0_8
.LBB0_5:
	xor	r12d, r12d
.LBB0_17:
	mov	r13, rax
	sub	r13, r12
	add	r12d, ebx
	.p2align	4
.LBB0_18:
	movsxd	r12, r12d
	vmulss	xmm3, xmm0, dword ptr [rcx + 4*r12]
	vaddss	xmm3, xmm3, xmm2
	vmovss	dword ptr [rdx + 4*r12], xmm3
	inc	r12d
	dec	r13
	jne	.LBB0_18
	jmp	.LBB0_3
	.p2align	4
.LBB0_8:
	cmp	r8d, 32
	jae	.LBB0_10
	xor	r13d, r13d
	jmp	.LBB0_14
.LBB0_10:
	vbroadcastss	ymm3, xmm2
	mov	r12d, ebx
	mov	r13, r11
	.p2align	4
.LBB0_11:
	movsxd	r12, r12d
	vmulps	ymm4, ymm1, ymmword ptr [rcx + 4*r12]
	vmulps	ymm5, ymm1, ymmword ptr [rcx + 4*r12 + 32]
	vmulps	ymm6, ymm1, ymmword ptr [rcx + 4*r12 + 64]
	vmulps	ymm7, ymm1, ymmword ptr [rcx + 4*r12 + 96]
	vaddps	ymm4, ymm4, ymm3
	vaddps	ymm5, ymm5, ymm3
	vaddps	ymm6, ymm6, ymm3
	vaddps	ymm7, ymm7, ymm3
	vmovups	ymmword ptr [rdx + 4*r12], ymm4
	vmovups	ymmword ptr [rdx + 4*r12 + 32], ymm5
	vmovups	ymmword ptr [rdx + 4*r12 + 64], ymm6
	vmovups	ymmword ptr [rdx + 4*r12 + 96], ymm7
	add	r12d, 32
	add	r13, -32
	jne	.LBB0_11
	cmp	r11d, eax
	je	.LBB0_3
	test	al, 28
	mov	r13, r11
	mov	r12, r11
	je	.LBB0_17
.LBB0_14:
	vbroadcastss	xmm3, xmm2
	mov	rdi, qword ptr [rbp - 40]
	lea	r12, [rdi + r13]
	add	r13d, ebx
	.p2align	4
.LBB0_15:
	movsxd	r13, r13d
	vmulps	xmm4, xmm1, xmmword ptr [rcx + 4*r13]
	vaddps	xmm4, xmm4, xmm3
	vmovups	xmmword ptr [rdx + 4*r13], xmm4
	add	r13d, 4
	add	r12, 4
	jne	.LBB0_15
	cmp	esi, eax
	mov	r12, rsi
	je	.LBB0_3
	jmp	.LBB0_17
.LBB0_19:
	vmovaps	xmm6, xmmword ptr [rbp - 32]
	vmovaps	xmm7, xmmword ptr [rbp - 16]
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
	vzeroupper
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
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.externals.a4e4a501d09d5a364b5bd41ae0aafdee;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.a4e4a501d09d5a364b5bd41ae0aafdee
	.p2align	4
burst.initialize.externals.a4e4a501d09d5a364b5bd41ae0aafdee:
.seh_proc burst.initialize.externals.a4e4a501d09d5a364b5bd41ae0aafdee
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

	.def	burst.initialize.statics.a4e4a501d09d5a364b5bd41ae0aafdee;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.a4e4a501d09d5a364b5bd41ae0aafdee
	.p2align	4
burst.initialize.statics.a4e4a501d09d5a364b5bd41ae0aafdee:
.seh_proc burst.initialize.statics.a4e4a501d09d5a364b5bd41ae0aafdee
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

	.section	.drectve,"yni"
	.ascii	" /EXPORT:a4e4a501d09d5a364b5bd41ae0aafdee"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.a4e4a501d09d5a364b5bd41ae0aafdee\""
	.ascii	" /EXPORT:\"burst.initialize.statics.a4e4a501d09d5a364b5bd41ae0aafdee\""
	.globl	_fltused
