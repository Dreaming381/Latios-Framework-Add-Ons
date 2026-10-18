
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

	.def	burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c
	.p2align	2
burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c
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

	.def	burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c
	.p2align	2
burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c
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

	.def	da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	da4a2341be7db4fb80108bdb1b99c15c
	.p2align	2
da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc da4a2341be7db4fb80108bdb1b99c15c
	stp	x29, x30, [sp, #-16]!
	.seh_save_fplr_x	16
	mov	x29, sp
	.seh_set_fp
	.seh_endprologue
	cmp	w2, #1
	b.lt	.LBB3_3
	mov	w8, w2
	cnth	x9
	cmp	x9, x8
	b.ls	.LBB3_4
	movi	d0, #0000000000000000
	mov	x9, xzr
	b	.LBB3_7
.LBB3_3:
	movi	d0, #0000000000000000
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
.LBB3_4:
	udiv	x10, x8, x9
	movi	d0, #0000000000000000
	ptrue	p0.s
	mov	x11, x0
	mov	x12, x1
	mul	x9, x10, x9
	sub	x10, x8, x9
	mov	x13, x9
.LBB3_5:
	ldr	z1, [x11]
	ldr	z2, [x12]
	dech	x13
	fmul	z1.s, z1.s, z2.s
	ldr	z2, [x12, #1, mul vl]
	incb	x12, all, mul #2
	fadda	s0, p0, s0, z1.s
	ldr	z1, [x11, #1, mul vl]
	incb	x11, all, mul #2
	fmul	z1.s, z1.s, z2.s
	fadda	s0, p0, s0, z1.s
	cbnz	x13, .LBB3_5
	cbz	x10, .LBB3_9
.LBB3_7:
	lsl	x10, x9, #2
	sub	x8, x8, x9
	add	x9, x1, x10
	add	x10, x0, x10
.LBB3_8:
	ldr	s1, [x10], #4
	subs	x8, x8, #1
	ldr	s2, [x9], #4
	fmul	s1, s1, s2
	fadd	s0, s0, s1
	b.ne	.LBB3_8
.LBB3_9:
	.seh_startepilogue
	ldp	x29, x30, [sp], #16
	.seh_save_fplr_x	16
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c\""
	.ascii	" /EXPORT:\"burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c\""
	.ascii	" /EXPORT:da4a2341be7db4fb80108bdb1b99c15c"
