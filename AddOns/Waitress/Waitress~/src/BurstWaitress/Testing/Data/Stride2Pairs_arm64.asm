
	.def	"@feat.00";
	.scl	3;
	.type	0;
	.endef
	.globl	"@feat.00"
"@feat.00" = 0
	.file	"main"
	.def	56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	56880a8a6a92e7ece69104b6a234a0cb
	.p2align	2
56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc 56880a8a6a92e7ece69104b6a234a0cb
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	cmp	w2, #1
	b.lt	.LBB0_8
	mov	x8, xzr
	cmp	w2, #8
	mov	w9, w2
	b.lo	.LBB0_6
	mov	x10, #-1073741825
	mov	x11, #-1073741824
	add	x10, x9, x10
	cmp	x10, x11
	b.lo	.LBB0_6
	and	x8, x9, #0x7ffffff8
	mov	x10, xzr
	add	x11, x1, #16
	mov	x12, #68719476736
	mov	x13, x8
.LBB0_4:
	add	x14, x0, x10, asr #30
	subs	x13, x13, #8
	add	x10, x10, x12
	ld2	{ v0.4s, v1.4s }, [x14], #32
	ld2	{ v2.4s, v3.4s }, [x14]
	fadd	v0.4s, v0.4s, v1.4s
	fadd	v1.4s, v2.4s, v3.4s
	stp	q0, q1, [x11, #-16]
	add	x11, x11, #32
	b.ne	.LBB0_4
	cmp	x8, x9
	b.eq	.LBB0_8
.LBB0_6:
	mov	w10, #1
	lsl	x11, x8, #33
	add	x12, x1, x8, lsl #2
	orr	w10, w10, w8, lsl #1
	sub	x8, x9, x8
	mov	x9, #8589934592
.LBB0_7:
	asr	x13, x11, #30
	ldr	s0, [x0, w10, sxtw #2]
	subs	x8, x8, #1
	add	w10, w10, #2
	add	x11, x11, x9
	ldr	s1, [x0, x13]
	fadd	s0, s1, s0
	str	s0, [x12], #4
	b.ne	.LBB0_7
.LBB0_8:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
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

	.def	burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb
	.p2align	2
burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb
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

	.def	burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb
	.p2align	2
burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb:
.seh_proc burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb
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

	.section	.drectve,"yni"
	.ascii	" /EXPORT:56880a8a6a92e7ece69104b6a234a0cb"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.56880a8a6a92e7ece69104b6a234a0cb\""
	.ascii	" /EXPORT:\"burst.initialize.statics.56880a8a6a92e7ece69104b6a234a0cb\""
