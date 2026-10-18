
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	1ef32d0056da20291a86af0d04256624;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@41200000
	.section	.rdata,"dr",discard,__real@41200000
	.p2align	2, 0x0
__real@41200000:
	.long	0x41200000
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	1ef32d0056da20291a86af0d04256624
	.p2align	4
1ef32d0056da20291a86af0d04256624:
.seh_proc 1ef32d0056da20291a86af0d04256624
	push	rbp
	.seh_pushreg rbp
	sub	rsp, 288
	.seh_stackalloc 288
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	vmovaps	xmmword ptr [rbp + 144], xmm15
	.seh_savexmm xmm15, 272
	vmovaps	xmmword ptr [rbp + 128], xmm14
	.seh_savexmm xmm14, 256
	vmovaps	xmmword ptr [rbp + 112], xmm13
	.seh_savexmm xmm13, 240
	vmovaps	xmmword ptr [rbp + 96], xmm12
	.seh_savexmm xmm12, 224
	vmovaps	xmmword ptr [rbp + 80], xmm11
	.seh_savexmm xmm11, 208
	vmovaps	xmmword ptr [rbp + 64], xmm10
	.seh_savexmm xmm10, 192
	vmovaps	xmmword ptr [rbp + 48], xmm9
	.seh_savexmm xmm9, 176
	vmovaps	xmmword ptr [rbp + 32], xmm8
	.seh_savexmm xmm8, 160
	vmovaps	xmmword ptr [rbp + 16], xmm7
	.seh_savexmm xmm7, 144
	vmovaps	xmmword ptr [rbp], xmm6
	.seh_savexmm xmm6, 128
	.seh_endprologue
	cmp	edx, 2
	jl	.LBB0_1
	dec	edx
	lea	rax, [rcx + 16]
	vbroadcastss	xmm4, dword ptr [rip + __real@3f800000]
	vxorps	xmm1, xmm1, xmm1
	vxorps	xmm0, xmm0, xmm0
	vmovaps	xmmword ptr [rbp - 64], xmm0
	vxorps	xmm8, xmm8, xmm8
	vxorps	xmm9, xmm9, xmm9
	vxorps	xmm10, xmm10, xmm10
	vxorps	xmm2, xmm2, xmm2
	vxorps	xmm3, xmm3, xmm3
	vxorps	xmm5, xmm5, xmm5
	vmovaps	xmmword ptr [rbp - 48], xmm5
	vmovaps	xmmword ptr [rbp - 32], xmm5
	vmovaps	xmmword ptr [rbp - 16], xmm4
	vmovaps	xmm12, xmm4
	vmovaps	xmm13, xmm4
	vmovaps	xmm14, xmm4
	vmovaps	xmm15, xmm4
	vmovaps	xmm7, xmm4
	vmovaps	xmm6, xmm4
	vmovaps	xmm11, xmm4
	vmovaps	xmmword ptr [rbp - 80], xmm4
	.p2align	4
.LBB0_3:
	vmovaps	xmmword ptr [rbp - 112], xmm4
	vmovaps	xmmword ptr [rbp - 128], xmm11
	vmovaps	xmmword ptr [rbp - 96], xmm6
	vmovups	xmm6, xmmword ptr [rax - 16]
	vmovups	xmm5, xmmword ptr [rax]
	vmovaps	xmm11, xmm7
	vmulps	xmm7, xmm4, xmm6
	vaddps	xmm1, xmm1, xmm7
	vmulps	xmm7, xmm5, xmm1
	vmovaps	xmm4, xmm3
	vmovaps	xmm3, xmm2
	vmovaps	xmm2, xmm0
	vmovaps	xmm0, xmm10
	vmovaps	xmm10, xmm9
	vmovaps	xmm9, xmm8
	vmovaps	xmm8, xmmword ptr [rbp - 64]
	vaddps	xmm8, xmm8, xmm7
	vmovaps	xmmword ptr [rbp - 64], xmm8
	vmulps	xmm7, xmm8, xmm6
	vmovaps	xmm8, xmm9
	vmovaps	xmm9, xmm10
	vmovaps	xmm10, xmm0
	vmovaps	xmm0, xmm2
	vaddps	xmm8, xmm8, xmm7
	vmulps	xmm7, xmm8, xmm5
	vaddps	xmm9, xmm9, xmm7
	vmulps	xmm7, xmm9, xmm6
	vaddps	xmm10, xmm10, xmm7
	vmulps	xmm7, xmm10, xmm5
	vaddps	xmm0, xmm0, xmm7
	vmulps	xmm7, xmm6, xmm0
	vaddps	xmm2, xmm3, xmm7
	vmulps	xmm7, xmm5, xmm2
	vaddps	xmm3, xmm4, xmm7
	vmulps	xmm7, xmm6, xmm3
	vmovaps	xmm4, xmmword ptr [rbp - 48]
	vaddps	xmm4, xmm4, xmm7
	vmovaps	xmmword ptr [rbp - 48], xmm4
	vmulps	xmm7, xmm5, xmm4
	vmovaps	xmm4, xmmword ptr [rbp - 32]
	vaddps	xmm4, xmm4, xmm7
	vmovaps	xmm7, xmmword ptr [rbp - 80]
	vmovaps	xmmword ptr [rbp - 32], xmm4
	vmulps	xmm7, xmm7, xmm4
	vmovaps	xmmword ptr [rbp - 80], xmm7
	vaddps	xmm7, xmm6, xmm7
	vmovaps	xmm4, xmmword ptr [rbp - 16]
	vmulps	xmm4, xmm4, xmm7
	vmovaps	xmmword ptr [rbp - 16], xmm4
	vaddps	xmm7, xmm5, xmm4
	vmulps	xmm12, xmm12, xmm7
	vaddps	xmm7, xmm12, xmm6
	vmulps	xmm13, xmm13, xmm7
	vaddps	xmm7, xmm13, xmm5
	vmulps	xmm14, xmm14, xmm7
	vaddps	xmm7, xmm14, xmm6
	vmulps	xmm15, xmm15, xmm7
	vaddps	xmm7, xmm15, xmm5
	vmulps	xmm7, xmm11, xmm7
	vmovaps	xmm11, xmmword ptr [rbp - 128]
	vaddps	xmm6, xmm6, xmm7
	vmovaps	xmm4, xmmword ptr [rbp - 96]
	vmulps	xmm4, xmm4, xmm6
	vmovaps	xmmword ptr [rbp - 96], xmm4
	vmovaps	xmm4, xmmword ptr [rbp - 112]
	vmovaps	xmm6, xmmword ptr [rbp - 96]
	vaddps	xmm5, xmm5, xmm6
	vmulps	xmm11, xmm11, xmm5
	vaddps	xmm5, xmm11, xmm1
	vmulps	xmm4, xmm4, xmm5
	add	rax, 16
	dec	rdx
	jne	.LBB0_3
	vaddps	xmm1, xmm1, xmmword ptr [rbp - 64]
	vaddps	xmm1, xmm8, xmm1
	vaddps	xmm1, xmm9, xmm1
	vaddps	xmm1, xmm10, xmm1
	vaddps	xmm1, xmm1, xmm0
	vaddps	xmm1, xmm1, xmm2
	vaddps	xmm1, xmm1, xmm3
	vaddps	xmm1, xmm1, xmmword ptr [rbp - 48]
	vaddps	xmm1, xmm1, xmmword ptr [rbp - 32]
	vmovaps	xmm0, xmmword ptr [rbp - 16]
	vaddps	xmm5, xmm0, xmmword ptr [rbp - 80]
	vaddps	xmm5, xmm12, xmm5
	vaddps	xmm5, xmm13, xmm5
	vaddps	xmm5, xmm14, xmm5
	vaddps	xmm5, xmm15, xmm5
	vaddps	xmm0, xmm5, xmm7
	vaddps	xmm0, xmm0, xmm6
	vaddps	xmm0, xmm11, xmm0
	vaddps	xmm0, xmm0, xmm4
	jmp	.LBB0_5
.LBB0_1:
	vbroadcastss	xmm0, dword ptr [rip + __real@41200000]
	vxorps	xmm1, xmm1, xmm1
.LBB0_5:
	vmovups	xmmword ptr [rcx], xmm1
	vmovups	xmmword ptr [rcx + 16], xmm0
	vmovaps	xmm6, xmmword ptr [rbp]
	vmovaps	xmm7, xmmword ptr [rbp + 16]
	vmovaps	xmm8, xmmword ptr [rbp + 32]
	vmovaps	xmm9, xmmword ptr [rbp + 48]
	vmovaps	xmm10, xmmword ptr [rbp + 64]
	vmovaps	xmm11, xmmword ptr [rbp + 80]
	vmovaps	xmm12, xmmword ptr [rbp + 96]
	vmovaps	xmm13, xmmword ptr [rbp + 112]
	vmovaps	xmm14, xmmword ptr [rbp + 128]
	vmovaps	xmm15, xmmword ptr [rbp + 144]
	.seh_startepilogue
	add	rsp, 288
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

	.def	burst.initialize.externals.1ef32d0056da20291a86af0d04256624;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.1ef32d0056da20291a86af0d04256624
	.p2align	4
burst.initialize.externals.1ef32d0056da20291a86af0d04256624:
.seh_proc burst.initialize.externals.1ef32d0056da20291a86af0d04256624
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

	.def	burst.initialize.statics.1ef32d0056da20291a86af0d04256624;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.1ef32d0056da20291a86af0d04256624
	.p2align	4
burst.initialize.statics.1ef32d0056da20291a86af0d04256624:
.seh_proc burst.initialize.statics.1ef32d0056da20291a86af0d04256624
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
	.ascii	" /EXPORT:1ef32d0056da20291a86af0d04256624"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.1ef32d0056da20291a86af0d04256624\""
	.ascii	" /EXPORT:\"burst.initialize.statics.1ef32d0056da20291a86af0d04256624\""
