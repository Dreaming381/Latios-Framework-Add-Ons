
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
	cmp	w3, #8
	mov	w8, w3
	b.hs	.LBB3_3
	mov	x9, xzr
	b	.LBB3_6
.LBB3_3:
	and	x9, x8, #0x7ffffff8
	add	x10, x1, #16
	add	x11, x0, #16
	add	x12, x2, #16
	mov	x13, x9
.LBB3_4:
	ldp	q0, q3, [x10, #-16]
	subs	x13, x13, #8
	ldp	q1, q2, [x11, #-16]
	add	x10, x10, #32
	add	x11, x11, #32
	fmul	v0.4s, v1.4s, v0.4s
	fmul	v1.4s, v2.4s, v3.4s
	stp	q0, q1, [x12, #-16]
	add	x12, x12, #32
	b.ne	.LBB3_4
	cmp	x9, x8
	b.eq	.LBB3_8
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
