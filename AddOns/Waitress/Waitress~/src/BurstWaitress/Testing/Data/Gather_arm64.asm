
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

	.def	burst.initialize.externals.de096e6f63574a7edd03b9ed94afb977;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.de096e6f63574a7edd03b9ed94afb977
	.p2align	2
burst.initialize.externals.de096e6f63574a7edd03b9ed94afb977:
.seh_proc burst.initialize.externals.de096e6f63574a7edd03b9ed94afb977
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

	.def	burst.initialize.statics.de096e6f63574a7edd03b9ed94afb977;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.de096e6f63574a7edd03b9ed94afb977
	.p2align	2
burst.initialize.statics.de096e6f63574a7edd03b9ed94afb977:
.seh_proc burst.initialize.statics.de096e6f63574a7edd03b9ed94afb977
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

	.def	de096e6f63574a7edd03b9ed94afb977;
	.scl	2;
	.type	32;
	.endef
	.globl	de096e6f63574a7edd03b9ed94afb977
	.p2align	2
de096e6f63574a7edd03b9ed94afb977:
.seh_proc de096e6f63574a7edd03b9ed94afb977
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	cmp	w3, #1
	b.lt	.LBB3_8
	cmp	w3, #1
	mov	w8, w3
	b.ne	.LBB3_3
	mov	x9, xzr
	b	.LBB3_6
.LBB3_3:
	and	x9, x8, #0x7ffffffe
	add	x10, x1, #4
	mov	x11, x2
	mov	x12, x9
.LBB3_4:
	ldpsw	x13, x14, [x10, #-4]
	subs	x12, x12, #2
	add	x10, x10, #8
	add	x14, x0, x14, lsl #2
	ldr	s0, [x0, x13, lsl #2]
	ld1	{ v0.s }[1], [x14]
	fadd	v0.2s, v0.2s, v0.2s
	str	d0, [x11], #8
	b.ne	.LBB3_4
	cmp	x9, x8
	b.eq	.LBB3_8
.LBB3_6:
	lsl	x10, x9, #2
	sub	x8, x8, x9
	add	x9, x1, x10
	add	x10, x2, x10
.LBB3_7:
	ldrsw	x11, [x9], #4
	subs	x8, x8, #1
	ldr	s0, [x0, x11, lsl #2]
	fadd	s0, s0, s0
	str	s0, [x10], #4
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
	.ascii	" /EXPORT:\"burst.initialize.externals.de096e6f63574a7edd03b9ed94afb977\""
	.ascii	" /EXPORT:\"burst.initialize.statics.de096e6f63574a7edd03b9ed94afb977\""
	.ascii	" /EXPORT:de096e6f63574a7edd03b9ed94afb977"
