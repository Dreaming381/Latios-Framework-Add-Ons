
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	64aa575729d373e9a331fb0f94acd131;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.globl	__real@7fffffff
	.section	.rdata,"dr",discard,__real@7fffffff
	.p2align	2, 0x0
__real@7fffffff:
	.long	0x7fffffff
	.globl	__real@7f800000
	.section	.rdata,"dr",discard,__real@7f800000
	.p2align	2, 0x0
__real@7f800000:
	.long	0x7f800000
	.globl	__real@40400000
	.section	.rdata,"dr",discard,__real@40400000
	.p2align	2, 0x0
__real@40400000:
	.long	0x40400000
	.text
	.globl	64aa575729d373e9a331fb0f94acd131
	.p2align	4
64aa575729d373e9a331fb0f94acd131:
.seh_proc 64aa575729d373e9a331fb0f94acd131
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	vbroadcastss	xmm0, dword ptr [rip + __real@3f800000]
	vdivps	xmm0, xmm0, xmmword ptr [rdx]
	vbroadcastss	xmm1, dword ptr [rip + __real@7fffffff]
	vandps	xmm0, xmm0, xmm1
	vbroadcastss	xmm1, dword ptr [rip + __real@7f800000]
	vcmpneq_oqps	xmm0, xmm0, xmm1
	vmovmskps	eax, xmm0
	cmp	eax, 15
	sete	dl
	test	r8d, r8d
	setg	r9b
	and	r9b, dl
	cmp	r9b, 1
	jne	.LBB0_14
	mov	edx, r8d
	cmp	r8d, 4
	jae	.LBB0_3
	xor	r8d, r8d
	jmp	.LBB0_12
.LBB0_3:
	cmp	r8d, 32
	jae	.LBB0_5
	xor	r8d, r8d
	jmp	.LBB0_9
.LBB0_5:
	mov	r8d, edx
	and	r8d, 2147483616
	lea	r9, [rcx + 96]
	vbroadcastss	ymm0, dword ptr [rip + __real@40400000]
	mov	r10, r8
	.p2align	4
.LBB0_6:
	vmulps	ymm1, ymm0, ymmword ptr [r9 - 96]
	vmulps	ymm2, ymm0, ymmword ptr [r9 - 64]
	vmulps	ymm3, ymm0, ymmword ptr [r9 - 32]
	vmulps	ymm4, ymm0, ymmword ptr [r9]
	vmovups	ymmword ptr [r9 - 96], ymm1
	vmovups	ymmword ptr [r9 - 64], ymm2
	vmovups	ymmword ptr [r9 - 32], ymm3
	vmovups	ymmword ptr [r9], ymm4
	sub	r9, -128
	add	r10, -32
	jne	.LBB0_6
	cmp	r8d, edx
	je	.LBB0_14
	test	dl, 28
	je	.LBB0_12
.LBB0_9:
	mov	r9, r8
	mov	r8d, edx
	and	r8d, 2147483644
	lea	r10, [rcx + 4*r9]
	sub	r9, r8
	vbroadcastss	xmm0, dword ptr [rip + __real@40400000]
	.p2align	4
.LBB0_10:
	vmulps	xmm1, xmm0, xmmword ptr [r10]
	vmovups	xmmword ptr [r10], xmm1
	add	r10, 16
	add	r9, 4
	jne	.LBB0_10
	cmp	r8d, edx
	je	.LBB0_14
.LBB0_12:
	sub	rdx, r8
	lea	rcx, [rcx + 4*r8]
	vmovss	xmm0, dword ptr [rip + __real@40400000]
	.p2align	4
.LBB0_13:
	vmulss	xmm1, xmm0, dword ptr [rcx]
	vmovss	dword ptr [rcx], xmm1
	add	rcx, 4
	dec	rdx
	jne	.LBB0_13
.LBB0_14:
	movzx	eax, al
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

	.def	burst.initialize.externals.64aa575729d373e9a331fb0f94acd131;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.64aa575729d373e9a331fb0f94acd131
	.p2align	4
burst.initialize.externals.64aa575729d373e9a331fb0f94acd131:
.seh_proc burst.initialize.externals.64aa575729d373e9a331fb0f94acd131
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

	.def	burst.initialize.statics.64aa575729d373e9a331fb0f94acd131;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.64aa575729d373e9a331fb0f94acd131
	.p2align	4
burst.initialize.statics.64aa575729d373e9a331fb0f94acd131:
.seh_proc burst.initialize.statics.64aa575729d373e9a331fb0f94acd131
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
	.ascii	" /EXPORT:64aa575729d373e9a331fb0f94acd131"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.64aa575729d373e9a331fb0f94acd131\""
	.ascii	" /EXPORT:\"burst.initialize.statics.64aa575729d373e9a331fb0f94acd131\""
	.globl	_fltused
