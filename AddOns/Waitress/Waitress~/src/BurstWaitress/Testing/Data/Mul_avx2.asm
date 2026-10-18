
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
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.def	burst.initialize.externals.c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.c5b3140d340eecb22922950e7f54787c
	.p2align	4
burst.initialize.externals.c5b3140d340eecb22922950e7f54787c:
.seh_proc burst.initialize.externals.c5b3140d340eecb22922950e7f54787c
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

	.def	burst.initialize.statics.c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.c5b3140d340eecb22922950e7f54787c
	.p2align	4
burst.initialize.statics.c5b3140d340eecb22922950e7f54787c:
.seh_proc burst.initialize.statics.c5b3140d340eecb22922950e7f54787c
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

	.def	c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	c5b3140d340eecb22922950e7f54787c
	.p2align	4
c5b3140d340eecb22922950e7f54787c:
.seh_proc c5b3140d340eecb22922950e7f54787c
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB3_13
	mov	eax, r9d
	cmp	r9d, 4
	jae	.LBB3_3
	xor	r9d, r9d
	jmp	.LBB3_12
.LBB3_3:
	cmp	r9d, 32
	jae	.LBB3_5
	xor	r9d, r9d
	jmp	.LBB3_9
.LBB3_5:
	mov	r9d, eax
	and	r9d, 2147483616
	mov	r10d, eax
	shr	r10d, 5
	and	r10d, 67108863
	shl	r10, 7
	xor	r11d, r11d
	.p2align	4
.LBB3_6:
	vmovups	ymm0, ymmword ptr [rcx + r11]
	vmovups	ymm1, ymmword ptr [rcx + r11 + 32]
	vmovups	ymm2, ymmword ptr [rcx + r11 + 64]
	vmovups	ymm3, ymmword ptr [rcx + r11 + 96]
	vmulps	ymm0, ymm0, ymmword ptr [rdx + r11]
	vmulps	ymm1, ymm1, ymmword ptr [rdx + r11 + 32]
	vmulps	ymm2, ymm2, ymmword ptr [rdx + r11 + 64]
	vmulps	ymm3, ymm3, ymmword ptr [rdx + r11 + 96]
	vmovups	ymmword ptr [r8 + r11], ymm0
	vmovups	ymmword ptr [r8 + r11 + 32], ymm1
	vmovups	ymmword ptr [r8 + r11 + 64], ymm2
	vmovups	ymmword ptr [r8 + r11 + 96], ymm3
	sub	r11, -128
	cmp	r10, r11
	jne	.LBB3_6
	cmp	r9d, eax
	je	.LBB3_13
	test	al, 28
	je	.LBB3_12
.LBB3_9:
	mov	r10, r9
	mov	r9d, eax
	and	r9d, 2147483644
	.p2align	4
.LBB3_10:
	vmovups	xmm0, xmmword ptr [rcx + 4*r10]
	vmulps	xmm0, xmm0, xmmword ptr [rdx + 4*r10]
	vmovups	xmmword ptr [r8 + 4*r10], xmm0
	add	r10, 4
	cmp	r9, r10
	jne	.LBB3_10
	cmp	r9d, eax
	je	.LBB3_13
	.p2align	4
.LBB3_12:
	vmovss	xmm0, dword ptr [rcx + 4*r9]
	vmulss	xmm0, xmm0, dword ptr [rdx + 4*r9]
	vmovss	dword ptr [r8 + 4*r9], xmm0
	inc	r9
	cmp	rax, r9
	jne	.LBB3_12
.LBB3_13:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.c5b3140d340eecb22922950e7f54787c\""
	.ascii	" /EXPORT:\"burst.initialize.statics.c5b3140d340eecb22922950e7f54787c\""
	.ascii	" /EXPORT:c5b3140d340eecb22922950e7f54787c"
	.globl	_fltused
