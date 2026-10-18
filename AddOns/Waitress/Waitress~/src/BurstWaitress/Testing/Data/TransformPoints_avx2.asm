
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	7e34353b3a7d6acb64680f65888c80e1;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	7e34353b3a7d6acb64680f65888c80e1
	.p2align	4
7e34353b3a7d6acb64680f65888c80e1:
.seh_proc 7e34353b3a7d6acb64680f65888c80e1
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_3
	vmovsd	xmm3, qword ptr [rcx + 52]
	vmovsd	xmm2, qword ptr [rcx + 36]
	vmovsd	xmm1, qword ptr [rcx + 20]
	vmovsd	xmm0, qword ptr [rcx + 4]
	vmovss	xmm4, dword ptr [rcx]
	vmovlhps	xmm4, xmm4, xmm0
	vshufps	xmm0, xmm4, xmm0, 216
	vmovss	xmm4, dword ptr [rcx + 16]
	vmovlhps	xmm4, xmm4, xmm1
	vshufps	xmm1, xmm4, xmm1, 216
	vmovss	xmm4, dword ptr [rcx + 32]
	vmovlhps	xmm4, xmm4, xmm2
	vshufps	xmm2, xmm4, xmm2, 216
	mov	eax, r8d
	vmovss	xmm4, dword ptr [rcx + 48]
	vshufps	xmm3, xmm3, xmm3, 208
	vmovss	xmm3, xmm3, xmm4
	.p2align	4
.LBB0_2:
	vmovsd	xmm4, qword ptr [rdx]
	vbroadcastss	xmm5, xmm4
	vmulps	xmm5, xmm0, xmm5
	vshufps	xmm4, xmm4, xmm4, 85
	vmulps	xmm4, xmm1, xmm4
	vaddps	xmm4, xmm5, xmm4
	vbroadcastss	xmm5, dword ptr [rdx + 8]
	vmulps	xmm5, xmm2, xmm5
	vaddps	xmm4, xmm5, xmm4
	vaddps	xmm4, xmm3, xmm4
	vmovlps	qword ptr [rdx], xmm4
	vextractps	dword ptr [rdx + 8], xmm4, 2
	add	rdx, 12
	dec	rax
	jne	.LBB0_2
.LBB0_3:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
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

	.def	burst.initialize.externals.7e34353b3a7d6acb64680f65888c80e1;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.7e34353b3a7d6acb64680f65888c80e1
	.p2align	4
burst.initialize.externals.7e34353b3a7d6acb64680f65888c80e1:
.seh_proc burst.initialize.externals.7e34353b3a7d6acb64680f65888c80e1
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

	.def	burst.initialize.statics.7e34353b3a7d6acb64680f65888c80e1;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.7e34353b3a7d6acb64680f65888c80e1
	.p2align	4
burst.initialize.statics.7e34353b3a7d6acb64680f65888c80e1:
.seh_proc burst.initialize.statics.7e34353b3a7d6acb64680f65888c80e1
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
	.ascii	" /EXPORT:7e34353b3a7d6acb64680f65888c80e1"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.7e34353b3a7d6acb64680f65888c80e1\""
	.ascii	" /EXPORT:\"burst.initialize.statics.7e34353b3a7d6acb64680f65888c80e1\""
	.globl	_fltused
