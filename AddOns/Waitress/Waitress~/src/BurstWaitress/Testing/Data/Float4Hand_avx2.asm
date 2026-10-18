
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

	.def	burst.initialize.externals.e09b73e16044cfada8a318de50c44a57;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.e09b73e16044cfada8a318de50c44a57
	.p2align	4
burst.initialize.externals.e09b73e16044cfada8a318de50c44a57:
.seh_proc burst.initialize.externals.e09b73e16044cfada8a318de50c44a57
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

	.def	burst.initialize.statics.e09b73e16044cfada8a318de50c44a57;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.e09b73e16044cfada8a318de50c44a57
	.p2align	4
burst.initialize.statics.e09b73e16044cfada8a318de50c44a57:
.seh_proc burst.initialize.statics.e09b73e16044cfada8a318de50c44a57
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

	.def	e09b73e16044cfada8a318de50c44a57;
	.scl	2;
	.type	32;
	.endef
	.globl	e09b73e16044cfada8a318de50c44a57
	.p2align	4
e09b73e16044cfada8a318de50c44a57:
.seh_proc e09b73e16044cfada8a318de50c44a57
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB3_3
	mov	eax, r9d
	shl	rax, 4
	xor	r9d, r9d
	.p2align	4
.LBB3_2:
	vmovups	xmm0, xmmword ptr [rcx + r9]
	vmulps	xmm1, xmm0, xmmword ptr [rdx + r9]
	vaddps	xmm0, xmm0, xmm1
	vmovups	xmmword ptr [r8 + r9], xmm0
	add	r9, 16
	cmp	rax, r9
	jne	.LBB3_2
.LBB3_3:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.e09b73e16044cfada8a318de50c44a57\""
	.ascii	" /EXPORT:\"burst.initialize.statics.e09b73e16044cfada8a318de50c44a57\""
	.ascii	" /EXPORT:e09b73e16044cfada8a318de50c44a57"
