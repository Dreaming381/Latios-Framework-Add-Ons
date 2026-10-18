
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	32145804ffafd5e28b39c4a33109a040;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	32145804ffafd5e28b39c4a33109a040
	.p2align	4
32145804ffafd5e28b39c4a33109a040:
.seh_proc 32145804ffafd5e28b39c4a33109a040
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB0_3
	mov	eax, r9d
	shl	rax, 3
	xor	r9d, r9d
	.p2align	4
.LBB0_2:
	movsxd	r10, r9d
	vmovups	ymm0, ymmword ptr [rcx + 4*r10]
	vmulps	ymm1, ymm0, ymmword ptr [rdx + 4*r10]
	vaddps	ymm0, ymm0, ymm1
	vmovups	ymmword ptr [r8 + 4*r10], ymm0
	add	r9, 8
	cmp	rax, r9
	jne	.LBB0_2
.LBB0_3:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	vzeroupper
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

	.def	burst.initialize.externals.32145804ffafd5e28b39c4a33109a040;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.32145804ffafd5e28b39c4a33109a040
	.p2align	4
burst.initialize.externals.32145804ffafd5e28b39c4a33109a040:
.seh_proc burst.initialize.externals.32145804ffafd5e28b39c4a33109a040
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

	.def	burst.initialize.statics.32145804ffafd5e28b39c4a33109a040;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.32145804ffafd5e28b39c4a33109a040
	.p2align	4
burst.initialize.statics.32145804ffafd5e28b39c4a33109a040:
.seh_proc burst.initialize.statics.32145804ffafd5e28b39c4a33109a040
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
	.ascii	" /EXPORT:32145804ffafd5e28b39c4a33109a040"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.32145804ffafd5e28b39c4a33109a040\""
	.ascii	" /EXPORT:\"burst.initialize.statics.32145804ffafd5e28b39c4a33109a040\""
