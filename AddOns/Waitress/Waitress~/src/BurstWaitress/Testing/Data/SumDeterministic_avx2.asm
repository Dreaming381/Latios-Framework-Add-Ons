
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	79b93a385e831f46a86bfaaf7c2c4aad;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	79b93a385e831f46a86bfaaf7c2c4aad
	.p2align	4
79b93a385e831f46a86bfaaf7c2c4aad:
.seh_proc 79b93a385e831f46a86bfaaf7c2c4aad
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	sub	rsp, 104
	.seh_stackalloc 104
	lea	rbp, [rsp + 96]
	.seh_setframe rbp, 96
	.seh_endprologue
	vstmxcsr	dword ptr [rbp - 32]
	mov	esi, dword ptr [rbp - 32]
	mov	eax, esi
	or	eax, 32768
	mov	dword ptr [rbp - 32], eax
	vldmxcsr	dword ptr [rbp - 32]
	call	79b93a385e831f46a86bfaaf7c2c4aad_inner_avx2@@16
	mov	dword ptr [rbp - 64], esi
	vldmxcsr	dword ptr [rbp - 64]
	.seh_startepilogue
	add	rsp, 104
	pop	rsi
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

	.def	burst.initialize.externals.79b93a385e831f46a86bfaaf7c2c4aad;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.79b93a385e831f46a86bfaaf7c2c4aad
	.p2align	4
burst.initialize.externals.79b93a385e831f46a86bfaaf7c2c4aad:
.seh_proc burst.initialize.externals.79b93a385e831f46a86bfaaf7c2c4aad
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

	.def	burst.initialize.statics.79b93a385e831f46a86bfaaf7c2c4aad;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.79b93a385e831f46a86bfaaf7c2c4aad
	.p2align	4
burst.initialize.statics.79b93a385e831f46a86bfaaf7c2c4aad:
.seh_proc burst.initialize.statics.79b93a385e831f46a86bfaaf7c2c4aad
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

	.def	79b93a385e831f46a86bfaaf7c2c4aad_inner_avx2@@16;
	.scl	3;
	.type	32;
	.endef
	.globl	__real@80000000
	.section	.rdata,"dr",discard,__real@80000000
	.p2align	2, 0x0
__real@80000000:
	.long	0x80000000
	.globl	__ymm@8000000080000000800000008000000080000000800000008000000000000000
	.section	.rdata,"dr",discard,__ymm@8000000080000000800000008000000080000000800000008000000000000000
	.p2align	5, 0x0
__ymm@8000000080000000800000008000000080000000800000008000000000000000:
	.long	0x00000000
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.globl	__xmm@80000000800000008000000000000000
	.section	.rdata,"dr",discard,__xmm@80000000800000008000000000000000
	.p2align	4, 0x0
__xmm@80000000800000008000000000000000:
	.zero	4
	.long	0x80000000
	.long	0x80000000
	.long	0x80000000
	.text
	.p2align	4
79b93a385e831f46a86bfaaf7c2c4aad_inner_avx2@@16:
.seh_proc 79b93a385e831f46a86bfaaf7c2c4aad_inner_avx2@@16
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	edx, edx
	jle	.LBB4_1
	cmp	edx, 4
	jae	.LBB4_4
	vxorps	xmm0, xmm0, xmm0
	xor	eax, eax
	jmp	.LBB4_13
.LBB4_1:
	vxorps	xmm0, xmm0, xmm0
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
.LBB4_4:
	cmp	edx, 32
	jae	.LBB4_6
	vxorps	xmm0, xmm0, xmm0
	xor	eax, eax
	jmp	.LBB4_10
.LBB4_6:
	mov	eax, edx
	and	eax, 2147483616
	lea	r8, [rcx + 96]
	vbroadcastss	ymm0, dword ptr [rip + __real@80000000]
	vmovaps	ymm1, ymmword ptr [rip + __ymm@8000000080000000800000008000000080000000800000008000000000000000]
	mov	r9d, eax
	vmovaps	ymm2, ymm0
	vmovaps	ymm3, ymm0
	.p2align	4
.LBB4_7:
	vaddps	ymm1, ymm1, ymmword ptr [r8 - 96]
	vaddps	ymm0, ymm0, ymmword ptr [r8 - 64]
	vaddps	ymm2, ymm2, ymmword ptr [r8 - 32]
	vaddps	ymm3, ymm3, ymmword ptr [r8]
	sub	r8, -128
	add	r9d, -32
	jne	.LBB4_7
	vaddps	ymm0, ymm0, ymm1
	vaddps	ymm0, ymm2, ymm0
	vaddps	ymm0, ymm3, ymm0
	vmovshdup	xmm1, xmm0
	vaddss	xmm1, xmm0, xmm1
	vshufpd	xmm2, xmm0, xmm0, 1
	vaddss	xmm1, xmm1, xmm2
	vshufps	xmm2, xmm0, xmm0, 255
	vaddss	xmm1, xmm1, xmm2
	vextractf128	xmm0, ymm0, 1
	vaddss	xmm1, xmm1, xmm0
	vmovshdup	xmm2, xmm0
	vaddss	xmm1, xmm1, xmm2
	vshufpd	xmm2, xmm0, xmm0, 1
	vaddss	xmm1, xmm1, xmm2
	vshufps	xmm0, xmm0, xmm0, 255
	vaddss	xmm0, xmm1, xmm0
	cmp	edx, eax
	je	.LBB4_15
	test	dl, 28
	je	.LBB4_13
.LBB4_10:
	mov	r8d, eax
	mov	eax, edx
	and	eax, 2147483644
	vblendps	xmm0, xmm0, xmmword ptr [rip + __xmm@80000000800000008000000000000000], 14
	.p2align	4
.LBB4_11:
	movsxd	r8, r8d
	vaddps	xmm0, xmm0, xmmword ptr [rcx + 4*r8]
	add	r8d, 4
	cmp	eax, r8d
	jne	.LBB4_11
	vmovshdup	xmm1, xmm0
	vaddss	xmm1, xmm0, xmm1
	vshufpd	xmm2, xmm0, xmm0, 1
	vaddss	xmm1, xmm1, xmm2
	vshufps	xmm0, xmm0, xmm0, 255
	vaddss	xmm0, xmm1, xmm0
	cmp	edx, eax
	je	.LBB4_15
.LBB4_13:
	sub	edx, eax
	mov	eax, eax
	lea	rax, [rcx + 4*rax]
	.p2align	4
.LBB4_14:
	vaddss	xmm0, xmm0, dword ptr [rax]
	add	rax, 4
	dec	edx
	jne	.LBB4_14
.LBB4_15:
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:79b93a385e831f46a86bfaaf7c2c4aad"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.79b93a385e831f46a86bfaaf7c2c4aad\""
	.ascii	" /EXPORT:\"burst.initialize.statics.79b93a385e831f46a86bfaaf7c2c4aad\""
	.globl	_fltused
