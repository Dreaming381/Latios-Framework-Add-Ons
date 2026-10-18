
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	78d5abce2faf8ad0b5ceb3b966024804;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	78d5abce2faf8ad0b5ceb3b966024804
	.p2align	4
78d5abce2faf8ad0b5ceb3b966024804:
.seh_proc 78d5abce2faf8ad0b5ceb3b966024804
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
	sub	rsp, 16
	.seh_stackalloc 16
	lea	rbp, [rsp + 16]
	.seh_setframe rbp, 16
	vmovaps	xmmword ptr [rbp - 16], xmm6
	.seh_savexmm xmm6, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_18
	mov	eax, dword ptr [rdx]
	mov	r8d, r8d
	lea	r9, [rcx + 96]
	xor	r10d, r10d
	vmovss	xmm0, dword ptr [rip + __real@3f800000]
	vbroadcastss	xmm1, dword ptr [rip + __real@3f800000]
	vbroadcastss	ymm2, dword ptr [rip + __real@3f800000]
	jmp	.LBB0_4
	.p2align	4
.LBB0_3:
	inc	r10
	cmp	r10, r8
	je	.LBB0_18
.LBB0_4:
	mov	r11, r10
	shl	r11, 33
	sar	r11, 30
	movsxd	rdi, dword ptr [rdx + r11]
	lea	r11d, [r10 + r10 + 1]
	movsxd	r11, r11d
	mov	r11d, dword ptr [rdx + 4*r11]
	cmp	r11d, eax
	cmovge	r11d, eax
	cmp	edi, r11d
	jge	.LBB0_3
	movsxd	r11, r11d
	mov	rsi, r11
	sub	rsi, rdi
	cmp	rsi, 3
	ja	.LBB0_8
	mov	r14, rdi
	jmp	.LBB0_7
	.p2align	4
.LBB0_8:
	cmp	rsi, 32
	jae	.LBB0_13
	xor	ebx, ebx
	jmp	.LBB0_10
.LBB0_13:
	mov	rbx, rsi
	and	rbx, -32
	lea	r14, [r9 + 4*rdi]
	mov	r15, rbx
	.p2align	4
.LBB0_14:
	vmovups	ymm3, ymmword ptr [r14 - 96]
	vmovups	ymm4, ymmword ptr [r14 - 64]
	vmovups	ymm5, ymmword ptr [r14 - 32]
	vmovups	ymm6, ymmword ptr [r14]
	vaddps	ymm3, ymm3, ymm3
	vaddps	ymm4, ymm4, ymm4
	vaddps	ymm5, ymm5, ymm5
	vaddps	ymm6, ymm6, ymm6
	vaddps	ymm3, ymm3, ymm2
	vaddps	ymm4, ymm4, ymm2
	vaddps	ymm5, ymm5, ymm2
	vaddps	ymm6, ymm6, ymm2
	vmovups	ymmword ptr [r14 - 96], ymm3
	vmovups	ymmword ptr [r14 - 64], ymm4
	vmovups	ymmword ptr [r14 - 32], ymm5
	vmovups	ymmword ptr [r14], ymm6
	sub	r14, -128
	add	r15, -32
	jne	.LBB0_14
	cmp	rsi, rbx
	je	.LBB0_3
	test	sil, 28
	je	.LBB0_17
.LBB0_10:
	mov	r15, rsi
	and	r15, -4
	lea	r14, [r15 + rdi]
	mov	r12, rbx
	sub	r12, r15
	add	rbx, rdi
	lea	rdi, [rcx + 4*rbx]
	.p2align	4
.LBB0_11:
	vmovups	xmm3, xmmword ptr [rdi]
	vaddps	xmm3, xmm3, xmm3
	vaddps	xmm3, xmm3, xmm1
	vmovups	xmmword ptr [rdi], xmm3
	add	rdi, 16
	add	r12, 4
	jne	.LBB0_11
	cmp	rsi, r15
	je	.LBB0_3
	jmp	.LBB0_7
.LBB0_17:
	add	rbx, rdi
	mov	r14, rbx
	.p2align	4
.LBB0_7:
	sub	r11, r14
	lea	rsi, [rcx + 4*r14]
	.p2align	4
.LBB0_2:
	vmovss	xmm3, dword ptr [rsi]
	vaddss	xmm3, xmm3, xmm3
	vaddss	xmm3, xmm3, xmm0
	vmovss	dword ptr [rsi], xmm3
	add	rsi, 4
	dec	r11
	jne	.LBB0_2
	jmp	.LBB0_3
.LBB0_18:
	vmovaps	xmm6, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 16
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
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

	.def	burst.initialize.externals.78d5abce2faf8ad0b5ceb3b966024804;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.78d5abce2faf8ad0b5ceb3b966024804
	.p2align	4
burst.initialize.externals.78d5abce2faf8ad0b5ceb3b966024804:
.seh_proc burst.initialize.externals.78d5abce2faf8ad0b5ceb3b966024804
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

	.def	burst.initialize.statics.78d5abce2faf8ad0b5ceb3b966024804;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.78d5abce2faf8ad0b5ceb3b966024804
	.p2align	4
burst.initialize.statics.78d5abce2faf8ad0b5ceb3b966024804:
.seh_proc burst.initialize.statics.78d5abce2faf8ad0b5ceb3b966024804
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
	.ascii	" /EXPORT:78d5abce2faf8ad0b5ceb3b966024804"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.78d5abce2faf8ad0b5ceb3b966024804\""
	.ascii	" /EXPORT:\"burst.initialize.statics.78d5abce2faf8ad0b5ceb3b966024804\""
	.globl	_fltused
