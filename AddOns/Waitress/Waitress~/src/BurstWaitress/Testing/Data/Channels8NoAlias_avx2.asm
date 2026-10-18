
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	47f6a2de2f05dab58046d4c3e0328922;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	47f6a2de2f05dab58046d4c3e0328922
	.p2align	4
47f6a2de2f05dab58046d4c3e0328922:
.seh_proc 47f6a2de2f05dab58046d4c3e0328922
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB0_8
	movsxd	rdi, edx
	lea	rax, [rcx + 4*rdi]
	lea	edx, [rdi + rdi]
	movsxd	rsi, edx
	lea	rdx, [rcx + 4*rsi]
	lea	r8d, [rdi + 2*rdi]
	movsxd	r8, r8d
	lea	r8, [rcx + 4*r8]
	lea	r10d, [4*rdi]
	movsxd	r10, r10d
	lea	r10, [rcx + 4*r10]
	lea	r11d, [rdi + 4*rdi]
	movsxd	r11, r11d
	lea	r11, [rcx + 4*r11]
	lea	esi, [rsi + 2*rsi]
	movsxd	rsi, esi
	lea	rsi, [rcx + 4*rsi]
	lea	ebx, [8*rdi]
	sub	ebx, edi
	movsxd	rdi, ebx
	lea	rdi, [rcx + 4*rdi]
	mov	ebx, r9d
	cmp	r9d, 8
	jae	.LBB0_3
	xor	r9d, r9d
	jmp	.LBB0_6
.LBB0_3:
	mov	r9d, ebx
	and	r9d, 2147483640
	vbroadcastss	ymm0, xmm2
	mov	r14d, ebx
	shr	r14d, 3
	and	r14d, 268435455
	shl	r14, 5
	xor	r15d, r15d
	vbroadcastss	ymm1, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB0_4:
	vmovups	ymm3, ymmword ptr [r10 + r15]
	vmulps	ymm3, ymm3, ymmword ptr [r11 + r15]
	vmulps	ymm4, ymm3, ymmword ptr [rsi + r15]
	vmovups	ymmword ptr [rcx + r15], ymm4
	vmulps	ymm4, ymm3, ymmword ptr [rdi + r15]
	vmovups	ymmword ptr [rax + r15], ymm4
	vmulps	ymm4, ymm0, ymm3
	vmovups	ymmword ptr [rdx + r15], ymm4
	vsubps	ymm3, ymm1, ymm3
	vmovups	ymmword ptr [r8 + r15], ymm3
	add	r15, 32
	cmp	r14, r15
	jne	.LBB0_4
	cmp	r9d, ebx
	je	.LBB0_8
.LBB0_6:
	vmovss	xmm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB0_7:
	vmovss	xmm1, dword ptr [r10 + 4*r9]
	vmulss	xmm1, xmm1, dword ptr [r11 + 4*r9]
	vmulss	xmm3, xmm1, dword ptr [rsi + 4*r9]
	vmovss	dword ptr [rcx + 4*r9], xmm3
	vmulss	xmm3, xmm1, dword ptr [rdi + 4*r9]
	vmovss	dword ptr [rax + 4*r9], xmm3
	vmulss	xmm3, xmm2, xmm1
	vmovss	dword ptr [rdx + 4*r9], xmm3
	vsubss	xmm1, xmm0, xmm1
	vmovss	dword ptr [r8 + 4*r9], xmm1
	inc	r9
	cmp	rbx, r9
	jne	.LBB0_7
.LBB0_8:
	.seh_startepilogue
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r14
	pop	r15
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

	.def	burst.initialize.externals.47f6a2de2f05dab58046d4c3e0328922;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.47f6a2de2f05dab58046d4c3e0328922
	.p2align	4
burst.initialize.externals.47f6a2de2f05dab58046d4c3e0328922:
.seh_proc burst.initialize.externals.47f6a2de2f05dab58046d4c3e0328922
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

	.def	burst.initialize.statics.47f6a2de2f05dab58046d4c3e0328922;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.47f6a2de2f05dab58046d4c3e0328922
	.p2align	4
burst.initialize.statics.47f6a2de2f05dab58046d4c3e0328922:
.seh_proc burst.initialize.statics.47f6a2de2f05dab58046d4c3e0328922
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
	.ascii	" /EXPORT:47f6a2de2f05dab58046d4c3e0328922"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.47f6a2de2f05dab58046d4c3e0328922\""
	.ascii	" /EXPORT:\"burst.initialize.statics.47f6a2de2f05dab58046d4c3e0328922\""
	.globl	_fltused
