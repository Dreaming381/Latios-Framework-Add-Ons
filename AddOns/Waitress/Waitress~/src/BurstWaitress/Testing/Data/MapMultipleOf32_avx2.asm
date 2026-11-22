
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	10954ad89c2fa47c2ca58791017e7e0f;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	10954ad89c2fa47c2ca58791017e7e0f
	.p2align	4
10954ad89c2fa47c2ca58791017e7e0f:
.seh_proc 10954ad89c2fa47c2ca58791017e7e0f
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_3
	mov	eax, r8d
	xor	r8d, r8d
	vbroadcastss	ymm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB0_2:
	vmovups	ymm1, ymmword ptr [rcx + 4*r8]
	vmovups	ymm2, ymmword ptr [rcx + 4*r8 + 32]
	vmovups	ymm3, ymmword ptr [rcx + 4*r8 + 64]
	vmovups	ymm4, ymmword ptr [rcx + 4*r8 + 96]
	vaddps	ymm1, ymm1, ymm1
	vaddps	ymm2, ymm2, ymm2
	vaddps	ymm3, ymm3, ymm3
	vaddps	ymm4, ymm4, ymm4
	vaddps	ymm1, ymm1, ymm0
	vaddps	ymm2, ymm2, ymm0
	vaddps	ymm3, ymm3, ymm0
	vaddps	ymm4, ymm4, ymm0
	vmovups	ymmword ptr [rdx + 4*r8], ymm1
	vmovups	ymmword ptr [rdx + 4*r8 + 32], ymm2
	vmovups	ymmword ptr [rdx + 4*r8 + 64], ymm3
	vmovups	ymmword ptr [rdx + 4*r8 + 96], ymm4
	add	r8, 32
	cmp	rax, r8
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

	.def	burst.initialize.externals.10954ad89c2fa47c2ca58791017e7e0f;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.10954ad89c2fa47c2ca58791017e7e0f
	.p2align	4
burst.initialize.externals.10954ad89c2fa47c2ca58791017e7e0f:
.seh_proc burst.initialize.externals.10954ad89c2fa47c2ca58791017e7e0f
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

	.def	burst.initialize.statics.10954ad89c2fa47c2ca58791017e7e0f;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.10954ad89c2fa47c2ca58791017e7e0f
	.p2align	4
burst.initialize.statics.10954ad89c2fa47c2ca58791017e7e0f:
.seh_proc burst.initialize.statics.10954ad89c2fa47c2ca58791017e7e0f
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
	.ascii	" /EXPORT:10954ad89c2fa47c2ca58791017e7e0f"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.10954ad89c2fa47c2ca58791017e7e0f\""
	.ascii	" /EXPORT:\"burst.initialize.statics.10954ad89c2fa47c2ca58791017e7e0f\""
