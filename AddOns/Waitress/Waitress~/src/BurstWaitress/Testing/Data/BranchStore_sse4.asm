
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

	.def	burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28
	.p2align	4
burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28:
.seh_proc burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28
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

	.def	burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28
	.p2align	4
burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28:
.seh_proc burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28
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

	.def	ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@40400000
	.section	.rdata,"dr",discard,__real@40400000
	.p2align	2, 0x0
__real@40400000:
	.long	0x40400000
	.text
	.globl	ffbfedaba7141baf72974bbff3f07f28
	.p2align	4
ffbfedaba7141baf72974bbff3f07f28:
.seh_proc ffbfedaba7141baf72974bbff3f07f28
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	cmp	r8d, 2
	jl	.LBB3_24
	mov	eax, r8d
	cmp	r8d, 9
	setb	r8b
	mov	r9, rdx
	sub	r9, rcx
	cmp	r9, 32
	setb	r9b
	or	r9b, r8b
	mov	r8d, 1
	jne	.LBB3_21
	lea	r9, [rax - 1]
	mov	r10, r9
	and	r10, -8
	lea	r8, [r10 + 1]
	xor	r11d, r11d
	xorps	xmm0, xmm0
	movss	xmm1, dword ptr [rip + __real@40400000]
	jmp	.LBB3_3
	.p2align	4
.LBB3_19:
	add	r11, 8
	cmp	r10, r11
	je	.LBB3_20
.LBB3_3:
	movups	xmm3, xmmword ptr [rcx + 4*r11 + 4]
	movups	xmm2, xmmword ptr [rcx + 4*r11 + 20]
	ucomiss	xmm3, xmm0
	jbe	.LBB3_5
	movaps	xmm4, xmm3
	mulss	xmm4, xmm1
	movss	dword ptr [rdx + 4*r11 + 4], xmm4
.LBB3_5:
	xorps	xmm4, xmm4
	cmpltps	xmm4, xmm3
	pextrb	esi, xmm4, 4
	test	sil, 1
	jne	.LBB3_6
	pextrb	esi, xmm4, 8
	test	sil, 1
	jne	.LBB3_8
.LBB3_9:
	pextrb	esi, xmm4, 12
	test	sil, 1
	jne	.LBB3_10
.LBB3_11:
	ucomiss	xmm2, xmm0
	jbe	.LBB3_13
.LBB3_12:
	movaps	xmm3, xmm2
	mulss	xmm3, xmm1
	movss	dword ptr [rdx + 4*r11 + 20], xmm3
.LBB3_13:
	xorps	xmm3, xmm3
	cmpltps	xmm3, xmm2
	pextrb	esi, xmm3, 4
	test	sil, 1
	jne	.LBB3_14
	pextrb	esi, xmm3, 8
	test	sil, 1
	jne	.LBB3_16
.LBB3_17:
	pextrb	esi, xmm3, 12
	test	sil, 1
	je	.LBB3_19
	jmp	.LBB3_18
	.p2align	4
.LBB3_6:
	movshdup	xmm5, xmm3
	mulss	xmm5, xmm1
	movss	dword ptr [rdx + 4*r11 + 8], xmm5
	pextrb	esi, xmm4, 8
	test	sil, 1
	je	.LBB3_9
.LBB3_8:
	movaps	xmm5, xmm3
	unpckhpd	xmm5, xmm3
	mulss	xmm5, xmm1
	movss	dword ptr [rdx + 4*r11 + 12], xmm5
	pextrb	esi, xmm4, 12
	test	sil, 1
	je	.LBB3_11
.LBB3_10:
	shufps	xmm3, xmm3, 255
	mulss	xmm3, xmm1
	movss	dword ptr [rdx + 4*r11 + 16], xmm3
	ucomiss	xmm2, xmm0
	ja	.LBB3_12
	jmp	.LBB3_13
	.p2align	4
.LBB3_14:
	movshdup	xmm4, xmm2
	mulss	xmm4, xmm1
	movss	dword ptr [rdx + 4*r11 + 24], xmm4
	pextrb	esi, xmm3, 8
	test	sil, 1
	je	.LBB3_17
.LBB3_16:
	movaps	xmm4, xmm2
	unpckhpd	xmm4, xmm2
	mulss	xmm4, xmm1
	movss	dword ptr [rdx + 4*r11 + 28], xmm4
	pextrb	esi, xmm3, 12
	test	sil, 1
	je	.LBB3_19
.LBB3_18:
	shufps	xmm2, xmm2, 255
	mulss	xmm2, xmm1
	movss	dword ptr [rdx + 4*r11 + 32], xmm2
	jmp	.LBB3_19
.LBB3_20:
	cmp	r9, r10
	jne	.LBB3_21
.LBB3_24:
	.seh_startepilogue
	pop	rsi
	pop	rbp
	.seh_endepilogue
	ret
.LBB3_21:
	xorps	xmm0, xmm0
	movss	xmm1, dword ptr [rip + __real@40400000]
	jmp	.LBB3_22
	.p2align	4
.LBB3_23:
	inc	r8
	cmp	rax, r8
	je	.LBB3_24
.LBB3_22:
	movss	xmm2, dword ptr [rcx + 4*r8]
	ucomiss	xmm2, xmm0
	jbe	.LBB3_23
	mulss	xmm2, xmm1
	movss	dword ptr [rdx + 4*r8], xmm2
	jmp	.LBB3_23
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28\""
	.ascii	" /EXPORT:\"burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28\""
	.ascii	" /EXPORT:ffbfedaba7141baf72974bbff3f07f28"
	.globl	_fltused
