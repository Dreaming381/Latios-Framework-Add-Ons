
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

	.def	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	2
burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d
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

	.def	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	2
burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d
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

	.def	d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	2
d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc d2e6ffc2a276816faeb5eba8fa179d4d
	stp	x19, x20, [sp, #-32]!
	.seh_save_regp_x	x19, 32
	stp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	add	x29, sp, #16
	.seh_add_fp	16
	.seh_endprologue
	cmp	w2, #1
	b.lt	.LBB3_5
	lsl	w9, w1, #1
	lsl	w7, w1, #2
	lsl	w10, w1, #3
	sbfiz	x17, x1, #2, #32
	cmp	w2, #24
	mov	w15, w2
	sbfiz	x4, x9, #2, #32
	add	w9, w9, w1
	add	w20, w7, w1
	lsl	w19, w9, #1
	sub	w11, w10, w1
	sbfiz	x16, x9, #2, #32
	sbfiz	x6, x7, #2, #32
	sbfiz	x5, x20, #2, #32
	sbfiz	x1, x11, #2, #32
	sbfiz	x3, x19, #2, #32
	add	x8, x0, x17
	add	x9, x0, x4
	add	x10, x0, x16
	add	x11, x0, x6
	add	x12, x0, x5
	add	x13, x0, x3
	add	x14, x0, x1
	b.hs	.LBB3_6
.LBB3_2:
	mov	x16, xzr
.LBB3_3:
	fmov	s1, #1.00000000
.LBB3_4:
	ldr	s2, [x11, x16, lsl #2]
	ldr	s3, [x12, x16, lsl #2]
	fmul	s2, s2, s3
	ldr	s3, [x13, x16, lsl #2]
	fmul	s3, s3, s2
	fmul	s4, s0, s2
	str	s3, [x0, x16, lsl #2]
	ldr	s3, [x14, x16, lsl #2]
	fmul	s3, s2, s3
	fsub	s2, s1, s2
	str	s3, [x8, x16, lsl #2]
	str	s4, [x9, x16, lsl #2]
	str	s2, [x10, x16, lsl #2]
	add	x16, x16, #1
	cmp	x15, x16
	b.ne	.LBB3_4
.LBB3_5:
	.seh_startepilogue
	ldp	x29, x30, [sp, #16]
	.seh_save_fplr	16
	ldp	x19, x20, [sp], #32
	.seh_save_regp_x	x19, 32
	.seh_endepilogue
	ret
.LBB3_6:
	cmp	x17, #16
	b.lo	.LBB3_2
	cmp	x4, #16
	b.lo	.LBB3_2
	cmp	x16, #16
	b.lo	.LBB3_2
	cbz	w7, .LBB3_2
	sxtw	x2, w20
	neg	x2, x2, lsl #2
	cmp	x2, #16
	b.lo	.LBB3_2
	sxtw	x2, w19
	neg	x2, x2, lsl #2
	cmp	x2, #16
	b.lo	.LBB3_2
	cmp	x1, #16
	b.lo	.LBB3_2
	sub	x2, x4, x17
	cmp	x2, #16
	b.lo	.LBB3_2
	sub	x2, x16, x17
	cmp	x2, #16
	b.lo	.LBB3_2
	sub	x2, x17, x6
	cmp	x2, #16
	b.lo	.LBB3_2
	sub	x2, x17, x5
	cmp	x2, #16
	b.lo	.LBB3_2
	sub	x2, x17, x3
	cmp	x2, #16
	b.lo	.LBB3_2
	sub	x17, x17, x1
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x16, x4
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x4, x6
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x4, x5
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x4, x3
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x4, x1
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x16, x6
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x16, x5
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x17, x16, x3
	cmp	x17, #16
	b.lo	.LBB3_2
	sub	x16, x16, x1
	cmp	x16, #16
	mov	x16, xzr
	b.lo	.LBB3_3
	fmov	v1.4s, #1.00000000
	lsl	x1, x15, #2
	mov	x17, xzr
	and	x16, x15, #0x7ffffffc
	and	x1, x1, #0x1fffffff0
.LBB3_29:
	ldr	q2, [x11, x17]
	ldr	q3, [x12, x17]
	fmul	v2.4s, v2.4s, v3.4s
	ldr	q3, [x13, x17]
	fmul	v3.4s, v3.4s, v2.4s
	fmul	v4.4s, v2.4s, v0.s[0]
	str	q3, [x0, x17]
	ldr	q3, [x14, x17]
	fmul	v3.4s, v2.4s, v3.4s
	fsub	v2.4s, v1.4s, v2.4s
	str	q3, [x8, x17]
	str	q4, [x9, x17]
	str	q2, [x10, x17]
	add	x17, x17, #16
	cmp	x1, x17
	b.ne	.LBB3_29
	cmp	x16, x15
	b.ne	.LBB3_3
	b	.LBB3_5
	.seh_endfunclet
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:d2e6ffc2a276816faeb5eba8fa179d4d"
