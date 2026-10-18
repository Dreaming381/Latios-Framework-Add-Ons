
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

	.def	burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28
	.p2align	2
burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28:
.seh_proc burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28
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

	.def	burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28
	.p2align	2
burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28:
.seh_proc burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28
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

	.def	ffbfedaba7141baf72974bbff3f07f28;
	.scl	2;
	.type	32;
	.endef
	.globl	ffbfedaba7141baf72974bbff3f07f28
	.p2align	2
ffbfedaba7141baf72974bbff3f07f28:
.seh_proc ffbfedaba7141baf72974bbff3f07f28
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	cmp	w2, #2
	b.lt	.LBB3_11
	mov	w8, w2
	cnth	x11
	sub	x9, x8, #1
	cmp	x9, x11
	b.lo	.LBB3_3
	rdvl	x10, #1
	lsr	x12, x10, #4
	sub	x10, x1, x0
	cmp	x10, x12, lsl #5
	b.hs	.LBB3_8
.LBB3_3:
	mov	w9, #1
.LBB3_4:
	lsl	x11, x9, #2
	fmov	s0, #3.00000000
	sub	x8, x8, x9
	add	x10, x0, x11
	add	x11, x1, x11
	b	.LBB3_6
.LBB3_5:
	subs	x8, x8, #1
	add	x11, x11, #4
	b.eq	.LBB3_11
.LBB3_6:
	ldr	s1, [x10], #4
	fcmp	s1, #0.0
	b.le	.LBB3_5
	fmul	s1, s1, s0
	str	s1, [x11]
	b	.LBB3_5
.LBB3_8:
	udiv	x13, x9, x11
	fmov	z0.s, #3.00000000
	mov	x10, xzr
	ptrue	p0.s
	add	x15, x0, #4
	mul	x14, x13, x11
	mneg	x11, x13, x11
	lsl	x13, x12, #2
	sub	x12, x9, x14
	add	x9, x14, #1
	add	x14, x1, #4
.LBB3_9:
	ld1w	{ z1.s }, p0/z, [x15, x10, lsl #2]
	ld1w	{ z2.s }, p0/z, [x15, x13, lsl #2]
	fcmgt	p1.s, p0/z, z1.s, #0.0
	fmul	z1.s, z1.s, z0.s
	fcmgt	p2.s, p0/z, z2.s, #0.0
	fmul	z2.s, z2.s, z0.s
	st1w	{ z1.s }, p1, [x14, x10, lsl #2]
	inch	x10
	st1w	{ z2.s }, p2, [x14, x13, lsl #2]
	inch	x13
	add	x16, x11, x13
	decw	x16
	cbnz	x16, .LBB3_9
	cbnz	x12, .LBB3_4
.LBB3_11:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.ffbfedaba7141baf72974bbff3f07f28\""
	.ascii	" /EXPORT:\"burst.initialize.statics.ffbfedaba7141baf72974bbff3f07f28\""
	.ascii	" /EXPORT:ffbfedaba7141baf72974bbff3f07f28"
