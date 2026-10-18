
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	56880a8a6a92e7ece69104b6a234a0cb
	.p2align	4
56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc 56880a8a6a92e7ece69104b6a234a0cb
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_8
	mov	eax, r8d
	cmp	r8d, 12
	setb	r8b
	lea	r9, [rax - 1073741825]
	cmp	r9, -1073741824
	setb	r9b
	or	r9b, r8b
	je	.LBB0_3
	xor	r8d, r8d
	jmp	.LBB0_6
.LBB0_3:
	mov	r8d, eax
	and	r8d, 2147483640
	lea	r9, [rdx + 16]
	xor	r10d, r10d
	movabs	r11, 68719476736
	mov	rsi, r8
	.p2align	4
.LBB0_4:
	mov	rdi, r10
	sar	rdi, 30
	movups	xmm0, xmmword ptr [rcx + rdi]
	movups	xmm1, xmmword ptr [rcx + rdi + 16]
	haddps	xmm0, xmm1
	movups	xmm1, xmmword ptr [rcx + rdi + 32]
	movups	xmm2, xmmword ptr [rcx + rdi + 48]
	haddps	xmm1, xmm2
	movups	xmmword ptr [r9 - 16], xmm0
	movups	xmmword ptr [r9], xmm1
	add	r10, r11
	add	r9, 32
	add	rsi, -8
	jne	.LBB0_4
	cmp	r8d, eax
	je	.LBB0_8
.LBB0_6:
	sub	rax, r8
	lea	r9d, [r8 + r8]
	shl	r8, 33
	movabs	r10, 8589934592
	.p2align	4
.LBB0_7:
	mov	r11, r8
	sar	r11, 30
	movss	xmm0, dword ptr [rcx + r11]
	lea	r11d, [r9 + 1]
	movsxd	r11, r11d
	addss	xmm0, dword ptr [rcx + 4*r11]
	movss	dword ptr [rdx + 2*r9], xmm0
	add	r9, 2
	add	r8, r10
	dec	rax
	jne	.LBB0_7
.LBB0_8:
	.seh_startepilogue
	pop	rdi
	pop	rsi
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
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb
	.p2align	4
burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb
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

	.def	burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb
	.p2align	4
burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb
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
	.ascii	" /EXPORT:56880a8a6a92e7ece69104b6a234a0cb"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb\""
	.ascii	" /EXPORT:\"burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb\""
	.globl	_fltused
