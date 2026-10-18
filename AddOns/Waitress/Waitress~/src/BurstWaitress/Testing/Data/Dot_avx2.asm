
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

	.def	burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c
	.p2align	4
burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c
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

	.def	burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c
	.p2align	4
burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c
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

	.def	da4a2341be7db4fb80108bdb1b99c15c;
	.scl	2;
	.type	32;
	.endef
	.globl	da4a2341be7db4fb80108bdb1b99c15c
	.p2align	4
da4a2341be7db4fb80108bdb1b99c15c:
.seh_proc da4a2341be7db4fb80108bdb1b99c15c
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB3_1
	mov	eax, r8d
	vxorps	xmm0, xmm0, xmm0
	xor	r8d, r8d
	.p2align	4
.LBB3_3:
	vmovss	xmm1, dword ptr [rcx + 4*r8]
	vmulss	xmm1, xmm1, dword ptr [rdx + 4*r8]
	vaddss	xmm0, xmm0, xmm1
	inc	r8
	cmp	rax, r8
	jne	.LBB3_3
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
.LBB3_1:
	vxorps	xmm0, xmm0, xmm0
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.da4a2341be7db4fb80108bdb1b99c15c\""
	.ascii	" /EXPORT:\"burst.initialize.statics.da4a2341be7db4fb80108bdb1b99c15c\""
	.ascii	" /EXPORT:da4a2341be7db4fb80108bdb1b99c15c"
	.globl	_fltused
