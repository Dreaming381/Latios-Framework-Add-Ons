
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

	.def	burst.initialize.externals.f5019d44427a13e07d10912b0f68e93a;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.f5019d44427a13e07d10912b0f68e93a
	.p2align	4
burst.initialize.externals.f5019d44427a13e07d10912b0f68e93a:
.seh_proc burst.initialize.externals.f5019d44427a13e07d10912b0f68e93a
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

	.def	burst.initialize.statics.f5019d44427a13e07d10912b0f68e93a;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.f5019d44427a13e07d10912b0f68e93a
	.p2align	4
burst.initialize.statics.f5019d44427a13e07d10912b0f68e93a:
.seh_proc burst.initialize.statics.f5019d44427a13e07d10912b0f68e93a
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

	.def	f5019d44427a13e07d10912b0f68e93a;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@7f7fffff
	.section	.rdata,"dr",discard,__real@7f7fffff
	.p2align	2, 0x0
__real@7f7fffff:
	.long	0x7f7fffff
	.globl	__real@ff7fffff
	.section	.rdata,"dr",discard,__real@ff7fffff
	.p2align	2, 0x0
__real@ff7fffff:
	.long	0xff7fffff
	.text
	.globl	f5019d44427a13e07d10912b0f68e93a
	.p2align	4
f5019d44427a13e07d10912b0f68e93a:
.seh_proc f5019d44427a13e07d10912b0f68e93a
	push	rbp
	.seh_pushreg rbp
	sub	rsp, 48
	.seh_stackalloc 48
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	vmovaps	xmmword ptr [rbp - 16], xmm8
	.seh_savexmm xmm8, 32
	vmovaps	xmmword ptr [rbp - 32], xmm7
	.seh_savexmm xmm7, 16
	vmovaps	xmmword ptr [rbp - 48], xmm6
	.seh_savexmm xmm6, 0
	.seh_endprologue
	cmp	edx, 8
	jge	.LBB3_2
	vbroadcastss	xmm1, dword ptr [rip + __real@7f7fffff]
	vbroadcastss	xmm2, dword ptr [rip + __real@ff7fffff]
	xor	eax, eax
	vmovaps	xmm0, xmm2
	vmovaps	xmm3, xmm1
	jmp	.LBB3_4
.LBB3_2:
	vbroadcastss	xmm0, dword ptr [rip + __real@ff7fffff]
	vbroadcastss	xmm3, dword ptr [rip + __real@7f7fffff]
	xor	eax, eax
	vxorps	xmm4, xmm4, xmm4
	vmovaps	xmm1, xmm3
	vmovaps	xmm2, xmm0
	.p2align	4
.LBB3_3:
	movsxd	r10, eax
	vmovups	xmm5, xmmword ptr [rcx + 4*r10]
	vmovups	xmm6, xmmword ptr [rcx + 4*r10 + 16]
	vcmpunordps	xmm7, xmm5, xmm4
	vminps	xmm8, xmm3, xmm5
	vblendvps	xmm3, xmm8, xmm3, xmm7
	vmaxps	xmm5, xmm0, xmm5
	vblendvps	xmm0, xmm5, xmm0, xmm7
	vcmpunordps	xmm5, xmm6, xmm4
	vminps	xmm7, xmm1, xmm6
	vblendvps	xmm1, xmm7, xmm1, xmm5
	vmaxps	xmm6, xmm2, xmm6
	vblendvps	xmm2, xmm6, xmm2, xmm5
	lea	eax, [r10 + 8]
	add	r10d, 16
	cmp	r10d, edx
	jle	.LBB3_3
.LBB3_4:
	vxorps	xmm4, xmm4, xmm4
	vcmpunordps	xmm5, xmm1, xmm4
	vminps	xmm1, xmm3, xmm1
	vblendvps	xmm1, xmm1, xmm3, xmm5
	vshufps	xmm3, xmm1, xmm1, 255
	vshufpd	xmm5, xmm1, xmm1, 1
	vmovshdup	xmm6, xmm1
	vminss	xmm7, xmm6, xmm1
	vcmpunordss	xmm1, xmm1, xmm1
	vblendvps	xmm1, xmm7, xmm6, xmm1
	vcmpunordss	xmm6, xmm1, xmm1
	vminss	xmm1, xmm5, xmm1
	vblendvps	xmm1, xmm1, xmm5, xmm6
	vcmpunordss	xmm5, xmm1, xmm1
	vminss	xmm1, xmm3, xmm1
	vblendvps	xmm1, xmm1, xmm3, xmm5
	vcmpunordps	xmm3, xmm2, xmm4
	vmaxps	xmm2, xmm0, xmm2
	vblendvps	xmm0, xmm2, xmm0, xmm3
	vshufps	xmm2, xmm0, xmm0, 255
	vshufpd	xmm3, xmm0, xmm0, 1
	vmovshdup	xmm4, xmm0
	vmaxss	xmm5, xmm4, xmm0
	vcmpunordss	xmm0, xmm0, xmm0
	vblendvps	xmm0, xmm5, xmm4, xmm0
	vcmpunordss	xmm4, xmm0, xmm0
	vmaxss	xmm0, xmm3, xmm0
	vblendvps	xmm0, xmm0, xmm3, xmm4
	vcmpunordss	xmm3, xmm0, xmm0
	vmaxss	xmm0, xmm2, xmm0
	vblendvps	xmm0, xmm0, xmm2, xmm3
	cmp	eax, edx
	jge	.LBB3_9
	mov	r10d, eax
	mov	eax, edx
	sub	rax, r10
	lea	rcx, [rcx + 4*r10]
	jmp	.LBB3_6
	.p2align	4
.LBB3_8:
	vminss	xmm3, xmm1, xmm2
	vcmpunordss	xmm2, xmm2, xmm2
	vblendvps	xmm1, xmm3, xmm1, xmm2
	add	rcx, 4
	dec	rax
	je	.LBB3_9
.LBB3_6:
	vmovss	xmm2, dword ptr [rcx]
	vucomiss	xmm2, xmm2
	jp	.LBB3_8
	vmaxss	xmm0, xmm0, xmm2
	jmp	.LBB3_8
.LBB3_9:
	vmovss	dword ptr [r8], xmm1
	vmovss	dword ptr [r9], xmm0
	vmovaps	xmm6, xmmword ptr [rbp - 48]
	vmovaps	xmm7, xmmword ptr [rbp - 32]
	vmovaps	xmm8, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 48
	pop	rbp
	.seh_endepilogue
	ret
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.f5019d44427a13e07d10912b0f68e93a\""
	.ascii	" /EXPORT:\"burst.initialize.statics.f5019d44427a13e07d10912b0f68e93a\""
	.ascii	" /EXPORT:f5019d44427a13e07d10912b0f68e93a"
	.globl	_fltused
