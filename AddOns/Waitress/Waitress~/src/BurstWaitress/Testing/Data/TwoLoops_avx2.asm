
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	7036149aa5c1cd8d58446b079f359289;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	7036149aa5c1cd8d58446b079f359289
	.p2align	4
7036149aa5c1cd8d58446b079f359289:
.seh_proc 7036149aa5c1cd8d58446b079f359289
	push	rbp
	.seh_pushreg rbp
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	sub	rsp, 32
	.seh_stackalloc 32
	lea	rbp, [rsp + 32]
	.seh_setframe rbp, 32
	.seh_endprologue
	test	edx, edx
	jle	.LBB0_1
	mov	rsi, rcx
	mov	edi, edx
	cmp	edx, 4
	jae	.LBB0_4
	xor	eax, eax
	jmp	.LBB0_13
.LBB0_1:
	vxorps	xmm0, xmm0, xmm0
	jmp	.LBB0_17
.LBB0_4:
	cmp	edx, 32
	jae	.LBB0_6
	xor	eax, eax
	jmp	.LBB0_10
.LBB0_6:
	mov	eax, edi
	and	eax, 2147483616
	lea	rcx, [rsi + 96]
	vbroadcastss	ymm0, dword ptr [rip + __real@3f800000]
	mov	rdx, rax
	.p2align	4
.LBB0_7:
	vmovups	ymm1, ymmword ptr [rcx - 96]
	vmovups	ymm2, ymmword ptr [rcx - 64]
	vmovups	ymm3, ymmword ptr [rcx - 32]
	vmovups	ymm4, ymmword ptr [rcx]
	vaddps	ymm1, ymm1, ymm1
	vaddps	ymm2, ymm2, ymm2
	vaddps	ymm3, ymm3, ymm3
	vaddps	ymm4, ymm4, ymm4
	vaddps	ymm1, ymm1, ymm0
	vaddps	ymm2, ymm2, ymm0
	vaddps	ymm3, ymm3, ymm0
	vaddps	ymm4, ymm4, ymm0
	vmovups	ymmword ptr [rcx - 96], ymm1
	vmovups	ymmword ptr [rcx - 64], ymm2
	vmovups	ymmword ptr [rcx - 32], ymm3
	vmovups	ymmword ptr [rcx], ymm4
	sub	rcx, -128
	add	rdx, -32
	jne	.LBB0_7
	cmp	eax, edi
	je	.LBB0_15
	test	dil, 28
	je	.LBB0_13
.LBB0_10:
	mov	rcx, rax
	mov	eax, edi
	and	eax, 2147483644
	lea	rdx, [rsi + 4*rcx]
	sub	rcx, rax
	vbroadcastss	xmm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB0_11:
	vmovups	xmm1, xmmword ptr [rdx]
	vaddps	xmm1, xmm1, xmm1
	vaddps	xmm1, xmm1, xmm0
	vmovups	xmmword ptr [rdx], xmm1
	add	rdx, 16
	add	rcx, 4
	jne	.LBB0_11
	cmp	eax, edi
	je	.LBB0_15
.LBB0_13:
	mov	rcx, rdi
	sub	rcx, rax
	lea	rax, [rsi + 4*rax]
	vmovss	xmm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB0_14:
	vmovss	xmm1, dword ptr [rax]
	vaddss	xmm1, xmm1, xmm1
	vaddss	xmm1, xmm1, xmm0
	vmovss	dword ptr [rax], xmm1
	add	rax, 4
	dec	rcx
	jne	.LBB0_14
.LBB0_15:
	vxorps	xmm0, xmm0, xmm0
	.p2align	4
.LBB0_16:
	vmovss	xmm1, dword ptr [rsi]
	vzeroupper
	call	"Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	add	rsi, 4
	dec	rdi
	jne	.LBB0_16
.LBB0_17:
	.seh_startepilogue
	add	rsp, 32
	pop	rdi
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

	.def	burst.initialize.externals.7036149aa5c1cd8d58446b079f359289;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.7036149aa5c1cd8d58446b079f359289
	.p2align	4
burst.initialize.externals.7036149aa5c1cd8d58446b079f359289:
.seh_proc burst.initialize.externals.7036149aa5c1cd8d58446b079f359289
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

	.def	burst.initialize.statics.7036149aa5c1cd8d58446b079f359289;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.7036149aa5c1cd8d58446b079f359289
	.p2align	4
burst.initialize.statics.7036149aa5c1cd8d58446b079f359289:
.seh_proc burst.initialize.statics.7036149aa5c1cd8d58446b079f359289
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

	.def	"Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16";
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3e800000
	.section	.rdata,"dr",discard,__real@3e800000
	.p2align	2, 0x0
__real@3e800000:
	.long	0x3e800000
	.section	.text,"xr",discard,"Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.globl	"Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	.p2align	4
"Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16":
.seh_proc "Latios.Validation.BurstWaitress.BwKernels, Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Opaque(System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 acc, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 x) -> System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_5c3a771e606e48a8df8d38e49bf0c0f9 from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null@@16"
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	vmulss	xmm0, xmm0, dword ptr [rip + __real@3e800000]
	vaddss	xmm0, xmm0, xmm1
	.seh_startepilogue
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:7036149aa5c1cd8d58446b079f359289"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.7036149aa5c1cd8d58446b079f359289\""
	.ascii	" /EXPORT:\"burst.initialize.statics.7036149aa5c1cd8d58446b079f359289\""
	.globl	_fltused
