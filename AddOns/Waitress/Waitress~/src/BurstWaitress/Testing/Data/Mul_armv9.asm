
	.def	"@feat.00";
	.scl	3;
	.type	0;
	.endef
	.globl	"@feat.00"
"@feat.00" = 0
	.file	"main"
	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	burst.initialize
	.p2align	2
burst.initialize:
.seh_proc burst.initialize
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.externals.c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.c5b3140d340eecb22922950e7f54787c
	.p2align	2
burst.initialize.externals.c5b3140d340eecb22922950e7f54787c:
.seh_proc burst.initialize.externals.c5b3140d340eecb22922950e7f54787c
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.statics.c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.c5b3140d340eecb22922950e7f54787c
	.p2align	2
burst.initialize.statics.c5b3140d340eecb22922950e7f54787c:
.seh_proc burst.initialize.statics.c5b3140d340eecb22922950e7f54787c
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	c5b3140d340eecb22922950e7f54787c;
	.scl	2;
	.type	32;
	.endef
	.globl	c5b3140d340eecb22922950e7f54787c
	.p2align	2
c5b3140d340eecb22922950e7f54787c:
.seh_proc c5b3140d340eecb22922950e7f54787c
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	cmp	w3, #1
	b.lt	.LBB3_8
	mov	w8, w3
	cnth	x9
	cmp	x9, x8
	b.ls	.LBB3_3
	mov	x9, xzr
	b	.LBB3_6
.LBB3_3:
	udiv	x11, x8, x9
	ptrue	p0.b
	mov	x10, xzr
	mul	x9, x11, x9
	sub	x11, x8, x9
	mov	x12, x9
.LBB3_4:
	ld1b	{ z0.b }, p0/z, [x0, x10]
	ld1b	{ z1.b }, p0/z, [x1, x10]
	add	x13, x0, x10
	add	x14, x1, x10
	ldr	z2, [x13, #1, mul vl]
	dech	x12
	ldr	z3, [x14, #1, mul vl]
	add	x13, x2, x10
	fmul	z0.s, z0.s, z1.s
	fmul	z1.s, z2.s, z3.s
	st1b	{ z0.b }, p0, [x2, x10]
	incb	x10, all, mul #2
	str	z1, [x13, #1, mul vl]
	cbnz	x12, .LBB3_4
	cbz	x11, .LBB3_8
.LBB3_6:
	lsl	x11, x9, #2
	sub	x8, x8, x9
	add	x9, x1, x11
	add	x10, x0, x11
	add	x11, x2, x11
.LBB3_7:
	ldr	s0, [x10], #4
	subs	x8, x8, #1
	ldr	s1, [x9], #4
	fmul	s0, s0, s1
	str	s0, [x11], #4
	b.ne	.LBB3_7
.LBB3_8:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.c5b3140d340eecb22922950e7f54787c\""
	.ascii	" /EXPORT:\"burst.initialize.statics.c5b3140d340eecb22922950e7f54787c\""
	.ascii	" /EXPORT:c5b3140d340eecb22922950e7f54787c"
