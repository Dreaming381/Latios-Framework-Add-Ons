
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	50f894ef9d69c624ac3acb4c44ed832d;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	50f894ef9d69c624ac3acb4c44ed832d
	.p2align	4
50f894ef9d69c624ac3acb4c44ed832d:
.seh_proc 50f894ef9d69c624ac3acb4c44ed832d
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 128
	.seh_stackalloc 128
	lea	rbp, [rsp + 128]
	.seh_setframe rbp, 128
	movaps	xmmword ptr [rbp - 16], xmm11
	.seh_savexmm xmm11, 112
	movaps	xmmword ptr [rbp - 32], xmm10
	.seh_savexmm xmm10, 96
	movaps	xmmword ptr [rbp - 48], xmm9
	.seh_savexmm xmm9, 80
	movaps	xmmword ptr [rbp - 64], xmm8
	.seh_savexmm xmm8, 64
	movaps	xmmword ptr [rbp - 80], xmm7
	.seh_savexmm xmm7, 48
	movaps	xmmword ptr [rbp - 96], xmm6
	.seh_savexmm xmm6, 32
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_11
	mov	rsi, rdx
	mov	rdi, rcx
	mov	ebx, r8d
	cmp	r8d, 8
	jae	.LBB0_3
	xor	r14d, r14d
	xorps	xmm6, xmm6
	jmp	.LBB0_9
.LBB0_3:
	mov	r14d, ebx
	and	r14d, 2147483640
	mov	r15d, ebx
	shr	r15d, 3
	and	r15d, 268435455
	shl	r15, 5
	xor	r12d, r12d
	.p2align	4
.LBB0_4:
	movups	xmm0, xmmword ptr [rdi + r12]
	movups	xmm6, xmmword ptr [rdi + r12 + 16]
	xorps	xmm7, xmm7
	cmpnltps	xmm7, xmm0
	xorps	xmm8, xmm8
	cmpnltps	xmm8, xmm6
	movaps	xmm10, xmm0
	addps	xmm10, xmm0
	movaps	xmm11, xmm6
	addps	xmm11, xmm6
	call	burst.Sleef_sinf4_u35sse4_x64_sse4@@16
	movaps	xmm9, xmm0
	movaps	xmm0, xmm6
	call	burst.Sleef_sinf4_u35sse4_x64_sse4@@16
	movaps	xmm1, xmm0
	movaps	xmm0, xmm7
	blendvps	xmm10, xmm9, xmm0
	movaps	xmm0, xmm8
	blendvps	xmm11, xmm1, xmm0
	movups	xmmword ptr [rsi + r12], xmm10
	movups	xmmword ptr [rsi + r12 + 16], xmm11
	add	r12, 32
	cmp	r15, r12
	jne	.LBB0_4
	cmp	r14d, ebx
	je	.LBB0_11
	xorps	xmm6, xmm6
	jmp	.LBB0_9
	.p2align	4
.LBB0_7:
	addss	xmm0, xmm0
	movss	dword ptr [rsi + 4*r14], xmm0
	inc	r14
	cmp	rbx, r14
	je	.LBB0_11
.LBB0_9:
	movss	xmm0, dword ptr [rdi + 4*r14]
	ucomiss	xmm0, xmm6
	ja	.LBB0_7
	call	burst.Sleef_sinf_u35_x64_sse4@@8
	movss	dword ptr [rsi + 4*r14], xmm0
	inc	r14
	cmp	rbx, r14
	jne	.LBB0_9
.LBB0_11:
	movaps	xmm6, xmmword ptr [rbp - 96]
	movaps	xmm7, xmmword ptr [rbp - 80]
	movaps	xmm8, xmmword ptr [rbp - 64]
	movaps	xmm9, xmmword ptr [rbp - 48]
	movaps	xmm10, xmmword ptr [rbp - 32]
	movaps	xmm11, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 128
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r14
	pop	r15
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

	.def	burst.initialize.externals.50f894ef9d69c624ac3acb4c44ed832d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.50f894ef9d69c624ac3acb4c44ed832d
	.p2align	4
burst.initialize.externals.50f894ef9d69c624ac3acb4c44ed832d:
.seh_proc burst.initialize.externals.50f894ef9d69c624ac3acb4c44ed832d
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

	.def	burst.initialize.statics.50f894ef9d69c624ac3acb4c44ed832d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.50f894ef9d69c624ac3acb4c44ed832d
	.p2align	4
burst.initialize.statics.50f894ef9d69c624ac3acb4c44ed832d:
.seh_proc burst.initialize.statics.50f894ef9d69c624ac3acb4c44ed832d
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

	.def	burst.Sleef_sinf4_u35sse4_x64_sse4@@16;
	.scl	2;
	.type	32;
	.endef
	.globl	__xmm@7fffffff7fffffff7fffffff7fffffff
	.section	.rdata,"dr",discard,__xmm@7fffffff7fffffff7fffffff7fffffff
	.p2align	4, 0x0
__xmm@7fffffff7fffffff7fffffff7fffffff:
	.quad	9223372034707292159
	.quad	9223372034707292159
	.globl	__xmm@42fa000042fa000042fa000042fa0000
	.section	.rdata,"dr",discard,__xmm@42fa000042fa000042fa000042fa0000
	.p2align	4, 0x0
__xmm@42fa000042fa000042fa000042fa0000:
	.long	0x42fa0000
	.long	0x42fa0000
	.long	0x42fa0000
	.long	0x42fa0000
	.globl	__xmm@47185800471858004718580047185800
	.section	.rdata,"dr",discard,__xmm@47185800471858004718580047185800
	.p2align	4, 0x0
__xmm@47185800471858004718580047185800:
	.long	0x47185800
	.long	0x47185800
	.long	0x47185800
	.long	0x47185800
	.globl	__xmm@000000ff000000ff000000ff000000ff
	.section	.rdata,"dr",discard,__xmm@000000ff000000ff000000ff000000ff
	.p2align	4, 0x0
__xmm@000000ff000000ff000000ff000000ff:
	.byte	255
	.byte	0
	.byte	0
	.byte	0
	.byte	255
	.byte	0
	.byte	0
	.byte	0
	.byte	255
	.byte	0
	.byte	0
	.byte	0
	.byte	255
	.byte	0
	.byte	0
	.byte	0
	.globl	__xmm@ffffff68ffffff68ffffff68ffffff68
	.section	.rdata,"dr",discard,__xmm@ffffff68ffffff68ffffff68ffffff68
	.p2align	4, 0x0
__xmm@ffffff68ffffff68ffffff68ffffff68:
	.long	4294967144
	.long	4294967144
	.long	4294967144
	.long	4294967144
	.globl	__xmm@000000d9000000d9000000d9000000d9
	.section	.rdata,"dr",discard,__xmm@000000d9000000d9000000d9000000d9
	.p2align	4, 0x0
__xmm@000000d9000000d9000000d9000000d9:
	.long	217
	.long	217
	.long	217
	.long	217
	.globl	__xmm@e0000000e0000000e0000000e0000000
	.section	.rdata,"dr",discard,__xmm@e0000000e0000000e0000000e0000000
	.p2align	4, 0x0
__xmm@e0000000e0000000e0000000e0000000:
	.long	3758096384
	.long	3758096384
	.long	3758096384
	.long	3758096384
	.globl	__xmm@fffff000fffff000fffff000fffff000
	.section	.rdata,"dr",discard,__xmm@fffff000fffff000fffff000fffff000
	.p2align	4, 0x0
__xmm@fffff000fffff000fffff000fffff000:
	.quad	-17587891081216
	.quad	-17587891081216
	.globl	__xmm@80000000800000008000000080000000
	.section	.rdata,"dr",discard,__xmm@80000000800000008000000080000000
	.p2align	4, 0x0
__xmm@80000000800000008000000080000000:
	.quad	-9223372034707292160
	.quad	-9223372034707292160
	.globl	__xmm@40800000408000004080000040800000
	.section	.rdata,"dr",discard,__xmm@40800000408000004080000040800000
	.p2align	4, 0x0
__xmm@40800000408000004080000040800000:
	.long	0x40800000
	.long	0x40800000
	.long	0x40800000
	.long	0x40800000
	.globl	__xmm@3e8000003e8000003e8000003e800000
	.section	.rdata,"dr",discard,__xmm@3e8000003e8000003e8000003e800000
	.p2align	4, 0x0
__xmm@3e8000003e8000003e8000003e800000:
	.long	0x3e800000
	.long	0x3e800000
	.long	0x3e800000
	.long	0x3e800000
	.globl	__xmm@40c90fdb40c90fdb40c90fdb40c90fdb
	.section	.rdata,"dr",discard,__xmm@40c90fdb40c90fdb40c90fdb40c90fdb
	.p2align	4, 0x0
__xmm@40c90fdb40c90fdb40c90fdb40c90fdb:
	.long	0x40c90fdb
	.long	0x40c90fdb
	.long	0x40c90fdb
	.long	0x40c90fdb
	.globl	__xmm@40c9000040c9000040c9000040c90000
	.section	.rdata,"dr",discard,__xmm@40c9000040c9000040c9000040c90000
	.p2align	4, 0x0
__xmm@40c9000040c9000040c9000040c90000:
	.long	0x40c90000
	.long	0x40c90000
	.long	0x40c90000
	.long	0x40c90000
	.globl	__xmm@3afdb0003afdb0003afdb0003afdb000
	.section	.rdata,"dr",discard,__xmm@3afdb0003afdb0003afdb0003afdb000
	.p2align	4, 0x0
__xmm@3afdb0003afdb0003afdb0003afdb000:
	.long	0x3afdb000
	.long	0x3afdb000
	.long	0x3afdb000
	.long	0x3afdb000
	.globl	__xmm@b43bbd2eb43bbd2eb43bbd2eb43bbd2e
	.section	.rdata,"dr",discard,__xmm@b43bbd2eb43bbd2eb43bbd2eb43bbd2e
	.p2align	4, 0x0
__xmm@b43bbd2eb43bbd2eb43bbd2eb43bbd2e:
	.long	0xb43bbd2e
	.long	0xb43bbd2e
	.long	0xb43bbd2e
	.long	0xb43bbd2e
	.globl	__xmm@3f3333333f3333333f3333333f333333
	.section	.rdata,"dr",discard,__xmm@3f3333333f3333333f3333333f333333
	.p2align	4, 0x0
__xmm@3f3333333f3333333f3333333f333333:
	.long	0x3f333333
	.long	0x3f333333
	.long	0x3f333333
	.long	0x3f333333
	.globl	__xmm@00000006000000060000000600000006
	.section	.rdata,"dr",discard,__xmm@00000006000000060000000600000006
	.p2align	4, 0x0
__xmm@00000006000000060000000600000006:
	.long	6
	.long	6
	.long	6
	.long	6
	.globl	__xmm@00000001000000010000000100000001
	.section	.rdata,"dr",discard,__xmm@00000001000000010000000100000001
	.p2align	4, 0x0
__xmm@00000001000000010000000100000001:
	.byte	1
	.byte	0
	.byte	0
	.byte	0
	.byte	1
	.byte	0
	.byte	0
	.byte	0
	.byte	1
	.byte	0
	.byte	0
	.byte	0
	.byte	1
	.byte	0
	.byte	0
	.byte	0
	.globl	__xmm@00000002000000020000000200000002
	.section	.rdata,"dr",discard,__xmm@00000002000000020000000200000002
	.p2align	4, 0x0
__xmm@00000002000000020000000200000002:
	.byte	2
	.byte	0
	.byte	0
	.byte	0
	.byte	2
	.byte	0
	.byte	0
	.byte	0
	.byte	2
	.byte	0
	.byte	0
	.byte	0
	.byte	2
	.byte	0
	.byte	0
	.byte	0
	.globl	__xmm@bfc90fdbbfc90fdbbfc90fdbbfc90fdb
	.section	.rdata,"dr",discard,__xmm@bfc90fdbbfc90fdbbfc90fdbbfc90fdb
	.p2align	4, 0x0
__xmm@bfc90fdbbfc90fdbbfc90fdbbfc90fdb:
	.quad	-4627149705656594469
	.quad	-4627149705656594469
	.globl	__xmm@333bbd2e333bbd2e333bbd2e333bbd2e
	.section	.rdata,"dr",discard,__xmm@333bbd2e333bbd2e333bbd2e333bbd2e
	.p2align	4, 0x0
__xmm@333bbd2e333bbd2e333bbd2e333bbd2e:
	.quad	3691752325685951790
	.quad	3691752325685951790
	.globl	__xmm@7f8000007f8000007f8000007f800000
	.section	.rdata,"dr",discard,__xmm@7f8000007f8000007f8000007f800000
	.p2align	4, 0x0
__xmm@7f8000007f8000007f8000007f800000:
	.long	0x7f800000
	.long	0x7f800000
	.long	0x7f800000
	.long	0x7f800000
	.globl	__xmm@3ea2f9833ea2f9833ea2f9833ea2f983
	.section	.rdata,"dr",discard,__xmm@3ea2f9833ea2f9833ea2f9833ea2f983
	.p2align	4, 0x0
__xmm@3ea2f9833ea2f9833ea2f9833ea2f983:
	.long	0x3ea2f983
	.long	0x3ea2f983
	.long	0x3ea2f983
	.long	0x3ea2f983
	.globl	__xmm@c0490000c0490000c0490000c0490000
	.section	.rdata,"dr",discard,__xmm@c0490000c0490000c0490000c0490000
	.p2align	4, 0x0
__xmm@c0490000c0490000c0490000c0490000:
	.long	0xc0490000
	.long	0xc0490000
	.long	0xc0490000
	.long	0xc0490000
	.globl	__xmm@ba7d8000ba7d8000ba7d8000ba7d8000
	.section	.rdata,"dr",discard,__xmm@ba7d8000ba7d8000ba7d8000ba7d8000
	.p2align	4, 0x0
__xmm@ba7d8000ba7d8000ba7d8000ba7d8000:
	.long	0xba7d8000
	.long	0xba7d8000
	.long	0xba7d8000
	.long	0xba7d8000
	.globl	__xmm@b5288000b5288000b5288000b5288000
	.section	.rdata,"dr",discard,__xmm@b5288000b5288000b5288000b5288000
	.p2align	4, 0x0
__xmm@b5288000b5288000b5288000b5288000:
	.long	0xb5288000
	.long	0xb5288000
	.long	0xb5288000
	.long	0xb5288000
	.globl	__xmm@af05a309af05a309af05a309af05a309
	.section	.rdata,"dr",discard,__xmm@af05a309af05a309af05a309af05a309
	.p2align	4, 0x0
__xmm@af05a309af05a309af05a309af05a309:
	.long	0xaf05a309
	.long	0xaf05a309
	.long	0xaf05a309
	.long	0xaf05a309
	.globl	__xmm@c0490e00c0490e00c0490e00c0490e00
	.section	.rdata,"dr",discard,__xmm@c0490e00c0490e00c0490e00c0490e00
	.p2align	4, 0x0
__xmm@c0490e00c0490e00c0490e00c0490e00:
	.long	0xc0490e00
	.long	0xc0490e00
	.long	0xc0490e00
	.long	0xc0490e00
	.globl	__xmm@b8ed5000b8ed5000b8ed5000b8ed5000
	.section	.rdata,"dr",discard,__xmm@b8ed5000b8ed5000b8ed5000b8ed5000
	.p2align	4, 0x0
__xmm@b8ed5000b8ed5000b8ed5000b8ed5000:
	.long	0xb8ed5000
	.long	0xb8ed5000
	.long	0xb8ed5000
	.long	0xb8ed5000
	.globl	__xmm@b1085a31b1085a31b1085a31b1085a31
	.section	.rdata,"dr",discard,__xmm@b1085a31b1085a31b1085a31b1085a31
	.p2align	4, 0x0
__xmm@b1085a31b1085a31b1085a31b1085a31:
	.long	0xb1085a31
	.long	0xb1085a31
	.long	0xb1085a31
	.long	0xb1085a31
	.globl	__xmm@362f0a87362f0a87362f0a87362f0a87
	.section	.rdata,"dr",discard,__xmm@362f0a87362f0a87362f0a87362f0a87
	.p2align	4, 0x0
__xmm@362f0a87362f0a87362f0a87362f0a87:
	.long	0x362f0a87
	.long	0x362f0a87
	.long	0x362f0a87
	.long	0x362f0a87
	.globl	__xmm@b94fbaebb94fbaebb94fbaebb94fbaeb
	.section	.rdata,"dr",discard,__xmm@b94fbaebb94fbaebb94fbaebb94fbaeb
	.p2align	4, 0x0
__xmm@b94fbaebb94fbaebb94fbaebb94fbaeb:
	.long	0xb94fbaeb
	.long	0xb94fbaeb
	.long	0xb94fbaeb
	.long	0xb94fbaeb
	.globl	__xmm@3c0887773c0887773c0887773c088777
	.section	.rdata,"dr",discard,__xmm@3c0887773c0887773c0887773c088777
	.p2align	4, 0x0
__xmm@3c0887773c0887773c0887773c088777:
	.long	0x3c088777
	.long	0x3c088777
	.long	0x3c088777
	.long	0x3c088777
	.globl	__xmm@be2aaaa6be2aaaa6be2aaaa6be2aaaa6
	.section	.rdata,"dr",discard,__xmm@be2aaaa6be2aaaa6be2aaaa6be2aaaa6
	.p2align	4, 0x0
__xmm@be2aaaa6be2aaaa6be2aaaa6be2aaaa6:
	.long	0xbe2aaaa6
	.long	0xbe2aaaa6
	.long	0xbe2aaaa6
	.long	0xbe2aaaa6
	.globl	__real@ff68ff68ff68ff68
	.section	.rdata,"dr",discard,__real@ff68ff68ff68ff68
	.p2align	3, 0x0
__real@ff68ff68ff68ff68:
	.short	65384
	.short	65384
	.short	65384
	.short	65384
	.globl	__real@f000f000f000f000
	.section	.rdata,"dr",discard,__real@f000f000f000f000
	.p2align	3, 0x0
__real@f000f000f000f000:
	.short	61440
	.short	61440
	.short	61440
	.short	61440
	.globl	__real@01010101
	.section	.rdata,"dr",discard,__real@01010101
	.p2align	2, 0x0
__real@01010101:
	.zero	4,1
	.section	.text,"xr",discard,burst.Sleef_sinf4_u35sse4_x64_sse4@@16
	.globl	burst.Sleef_sinf4_u35sse4_x64_sse4@@16
	.p2align	4
burst.Sleef_sinf4_u35sse4_x64_sse4@@16:
	sub	rsp, 168
	movaps	xmmword ptr [rsp + 144], xmm15
	movaps	xmmword ptr [rsp + 128], xmm14
	movaps	xmmword ptr [rsp + 112], xmm13
	movaps	xmmword ptr [rsp + 96], xmm12
	movaps	xmmword ptr [rsp + 80], xmm11
	movaps	xmmword ptr [rsp + 64], xmm10
	movdqa	xmmword ptr [rsp + 48], xmm9
	movaps	xmmword ptr [rsp + 32], xmm8
	movaps	xmmword ptr [rsp + 16], xmm7
	movaps	xmmword ptr [rsp], xmm6
	movaps	xmm1, xmm0
	movaps	xmm3, xmmword ptr [rip + __xmm@7fffffff7fffffff7fffffff7fffffff]
	andps	xmm3, xmm0
	movaps	xmm0, xmm3
	cmpltps	xmm0, xmmword ptr [rip + __xmm@42fa000042fa000042fa000042fa0000]
	movmskps	eax, xmm0
	cmp	eax, 15
	jne	.LBB4_2
	movaps	xmm0, xmmword ptr [rip + __xmm@3ea2f9833ea2f9833ea2f9833ea2f983]
	mulps	xmm0, xmm1
	cvtps2dq	xmm6, xmm0
	cvtdq2ps	xmm5, xmm6
	movaps	xmm0, xmmword ptr [rip + __xmm@c0490e00c0490e00c0490e00c0490e00]
	mulps	xmm0, xmm5
	addps	xmm0, xmm1
	movaps	xmm2, xmmword ptr [rip + __xmm@b8ed5000b8ed5000b8ed5000b8ed5000]
	mulps	xmm2, xmm5
	addps	xmm2, xmm0
	mulps	xmm5, xmmword ptr [rip + __xmm@b1085a31b1085a31b1085a31b1085a31]
	addps	xmm5, xmm2
.LBB4_5:
	pslld	xmm6, 31
	pxor	xmm6, xmm5
	mulps	xmm5, xmm5
	movaps	xmm2, xmmword ptr [rip + __xmm@362f0a87362f0a87362f0a87362f0a87]
	mulps	xmm2, xmm5
	addps	xmm2, xmmword ptr [rip + __xmm@b94fbaebb94fbaebb94fbaebb94fbaeb]
	mulps	xmm2, xmm5
	addps	xmm2, xmmword ptr [rip + __xmm@3c0887773c0887773c0887773c088777]
	mulps	xmm2, xmm5
	addps	xmm2, xmmword ptr [rip + __xmm@be2aaaa6be2aaaa6be2aaaa6be2aaaa6]
	mulps	xmm2, xmm6
	mulps	xmm2, xmm5
	addps	xmm2, xmm6
	movdqa	xmm0, xmmword ptr [rip + __xmm@80000000800000008000000080000000]
	pcmpeqd	xmm0, xmm1
	blendvps	xmm2, xmm1, xmm0
	movaps	xmm0, xmm2
	movaps	xmm6, xmmword ptr [rsp]
	movaps	xmm7, xmmword ptr [rsp + 16]
	movaps	xmm8, xmmword ptr [rsp + 32]
	movaps	xmm9, xmmword ptr [rsp + 48]
	movaps	xmm10, xmmword ptr [rsp + 64]
	movaps	xmm11, xmmword ptr [rsp + 80]
	movaps	xmm12, xmmword ptr [rsp + 96]
	movaps	xmm13, xmmword ptr [rsp + 112]
	movaps	xmm14, xmmword ptr [rsp + 128]
	movaps	xmm15, xmmword ptr [rsp + 144]
	add	rsp, 168
	ret
.LBB4_2:
	movaps	xmm0, xmm3
	cmpltps	xmm0, xmmword ptr [rip + __xmm@47185800471858004718580047185800]
	movmskps	eax, xmm0
	cmp	eax, 15
	jne	.LBB4_4
	movaps	xmm0, xmmword ptr [rip + __xmm@3ea2f9833ea2f9833ea2f9833ea2f983]
	mulps	xmm0, xmm1
	cvtps2dq	xmm6, xmm0
	cvtdq2ps	xmm5, xmm6
	movaps	xmm0, xmmword ptr [rip + __xmm@c0490000c0490000c0490000c0490000]
	mulps	xmm0, xmm5
	addps	xmm0, xmm1
	movaps	xmm2, xmmword ptr [rip + __xmm@ba7d8000ba7d8000ba7d8000ba7d8000]
	mulps	xmm2, xmm5
	addps	xmm2, xmm0
	movaps	xmm0, xmmword ptr [rip + __xmm@b5288000b5288000b5288000b5288000]
	mulps	xmm0, xmm5
	addps	xmm0, xmm2
	mulps	xmm5, xmmword ptr [rip + __xmm@af05a309af05a309af05a309af05a309]
	addps	xmm5, xmm0
	jmp	.LBB4_5
.LBB4_4:
	movaps	xmm4, xmm1
	psrld	xmm4, 23
	pand	xmm4, xmmword ptr [rip + __xmm@000000ff000000ff000000ff000000ff]
	pmovsxwd	xmm0, qword ptr [rip + __real@ff68ff68ff68ff68]
	paddd	xmm0, xmm4
	pcmpgtd	xmm4, xmmword ptr [rip + __xmm@000000d9000000d9000000d9000000d9]
	pand	xmm4, xmmword ptr [rip + __xmm@e0000000e0000000e0000000e0000000]
	paddd	xmm4, xmm1
	movdqa	xmm2, xmm0
	psrad	xmm2, 31
	pandn	xmm2, xmm0
	pslld	xmm2, 2
	pextrd	eax, xmm2, 3
	cdqe
	lea	rcx, [rip + rempitabsp]
	pextrd	edx, xmm2, 2
	movsxd	rdx, edx
	pextrd	r8d, xmm2, 1
	movsxd	r8, r8d
	movd	r9d, xmm2
	movsxd	r9, r9d
	movss	xmm2, dword ptr [rcx + 4*r9]
	movss	xmm8, dword ptr [rcx + 4*r9 + 4]
	insertps	xmm2, dword ptr [rcx + 4*r8], 16
	insertps	xmm2, dword ptr [rcx + 4*rdx], 32
	insertps	xmm2, dword ptr [rcx + 4*rax], 48
	pmovsxwd	xmm0, qword ptr [rip + __real@f000f000f000f000]
	movdqa	xmm5, xmm4
	pand	xmm5, xmm0
	movdqa	xmm6, xmm4
	subps	xmm6, xmm5
	movaps	xmm10, xmm2
	andps	xmm10, xmm0
	movaps	xmm9, xmm2
	subps	xmm9, xmm10
	mulps	xmm2, xmm4
	movaps	xmm13, xmmword ptr [rip + __xmm@80000000800000008000000080000000]
	movaps	xmm11, xmm2
	xorps	xmm11, xmm13
	movaps	xmm12, xmm5
	mulps	xmm12, xmm10
	addps	xmm12, xmm11
	mulps	xmm10, xmm6
	addps	xmm10, xmm12
	movaps	xmm11, xmm6
	mulps	xmm11, xmm9
	mulps	xmm9, xmm5
	addps	xmm9, xmm10
	addps	xmm9, xmm11
	movaps	xmm7, xmmword ptr [rip + __xmm@40800000408000004080000040800000]
	movaps	xmm10, xmm2
	mulps	xmm10, xmm7
	roundps	xmm10, xmm10, 8
	roundps	xmm12, xmm2, 8
	mulps	xmm12, xmm7
	movaps	xmm11, xmm10
	subps	xmm11, xmm12
	movaps	xmm12, xmmword ptr [rip + __xmm@3e8000003e8000003e8000003e800000]
	mulps	xmm10, xmm12
	subps	xmm2, xmm10
	movaps	xmm14, xmm2
	addps	xmm14, xmm9
	subps	xmm2, xmm14
	addps	xmm2, xmm9
	insertps	xmm8, dword ptr [rcx + 4*r8 + 4], 16
	insertps	xmm8, dword ptr [rcx + 4*rdx + 4], 32
	insertps	xmm8, dword ptr [rcx + 4*rax + 4], 48
	movaps	xmm9, xmm8
	andps	xmm9, xmm0
	movaps	xmm15, xmm8
	mulps	xmm8, xmm4
	movaps	xmm10, xmm8
	xorps	xmm10, xmm13
	movaps	xmm7, xmm5
	mulps	xmm7, xmm9
	addps	xmm7, xmm10
	subps	xmm15, xmm9
	mulps	xmm9, xmm6
	addps	xmm9, xmm7
	movaps	xmm7, xmm6
	mulps	xmm7, xmm15
	mulps	xmm15, xmm5
	addps	xmm15, xmm9
	addps	xmm15, xmm7
	addps	xmm15, xmm2
	movaps	xmm10, xmm8
	addps	xmm10, xmm14
	movaps	xmm2, xmm10
	subps	xmm2, xmm14
	movaps	xmm7, xmm10
	subps	xmm7, xmm2
	subps	xmm14, xmm7
	subps	xmm8, xmm2
	movss	xmm9, dword ptr [rcx + 4*r9 + 8]
	cvttps2dq	xmm7, xmm11
	addps	xmm8, xmm14
	addps	xmm8, xmm15
	movaps	xmm2, xmm10
	movaps	xmm14, xmmword ptr [rip + __xmm@40800000408000004080000040800000]
	mulps	xmm2, xmm14
	roundps	xmm11, xmm2, 8
	roundps	xmm2, xmm10, 8
	mulps	xmm2, xmm14
	movaps	xmm14, xmm11
	subps	xmm14, xmm2
	cvttps2dq	xmm2, xmm14
	paddd	xmm2, xmm7
	mulps	xmm11, xmm12
	subps	xmm10, xmm11
	movaps	xmm11, xmm10
	addps	xmm11, xmm8
	subps	xmm10, xmm11
	addps	xmm10, xmm8
	insertps	xmm9, dword ptr [rcx + 4*r8 + 8], 16
	insertps	xmm9, dword ptr [rcx + 4*rdx + 8], 32
	insertps	xmm9, dword ptr [rcx + 4*rax + 8], 48
	movaps	xmm7, xmm9
	andps	xmm7, xmm0
	movaps	xmm8, xmm9
	subps	xmm8, xmm7
	mulps	xmm9, xmm4
	movaps	xmm12, xmm9
	xorps	xmm12, xmm13
	movaps	xmm14, xmm8
	mulps	xmm14, xmm5
	mulps	xmm5, xmm7
	addps	xmm5, xmm12
	addps	xmm5, xmm14
	movss	xmm12, dword ptr [rcx + 4*r9 + 12]
	insertps	xmm12, dword ptr [rcx + 4*r8 + 12], 16
	insertps	xmm12, dword ptr [rcx + 4*rdx + 12], 32
	insertps	xmm12, dword ptr [rcx + 4*rax + 12], 48
	mulps	xmm7, xmm6
	addps	xmm7, xmm5
	mulps	xmm8, xmm6
	addps	xmm8, xmm7
	mulps	xmm12, xmm4
	addps	xmm12, xmm8
	addps	xmm12, xmm10
	movaps	xmm7, xmm9
	addps	xmm7, xmm11
	movaps	xmm5, xmm7
	subps	xmm5, xmm11
	movaps	xmm6, xmm7
	subps	xmm6, xmm5
	subps	xmm11, xmm6
	subps	xmm9, xmm5
	addps	xmm9, xmm11
	addps	xmm9, xmm12
	movaps	xmm6, xmm7
	addps	xmm6, xmm9
	subps	xmm7, xmm6
	addps	xmm7, xmm9
	andps	xmm0, xmm6
	movaps	xmm5, xmm6
	subps	xmm5, xmm0
	movaps	xmm9, xmmword ptr [rip + __xmm@40c90fdb40c90fdb40c90fdb40c90fdb]
	movaps	xmm8, xmm6
	mulps	xmm8, xmm9
	movaps	xmm10, xmm8
	xorps	xmm10, xmm13
	movaps	xmm11, xmmword ptr [rip + __xmm@40c9000040c9000040c9000040c90000]
	movaps	xmm12, xmm0
	mulps	xmm12, xmm11
	addps	xmm12, xmm10
	mulps	xmm11, xmm5
	addps	xmm11, xmm12
	movaps	xmm10, xmmword ptr [rip + __xmm@3afdb0003afdb0003afdb0003afdb000]
	mulps	xmm0, xmm10
	addps	xmm0, xmm11
	mulps	xmm5, xmm10
	addps	xmm5, xmm0
	mulps	xmm6, xmmword ptr [rip + __xmm@b43bbd2eb43bbd2eb43bbd2eb43bbd2e]
	addps	xmm6, xmm5
	mulps	xmm7, xmm9
	addps	xmm7, xmm6
	movaps	xmm0, xmmword ptr [rip + __xmm@7fffffff7fffffff7fffffff7fffffff]
	andps	xmm0, xmm4
	cmpltps	xmm0, xmmword ptr [rip + __xmm@3f3333333f3333333f3333333f333333]
	blendvps	xmm8, xmm4, xmm0
	xorps	xmm5, xmm5
	blendvps	xmm7, xmm5, xmm0
	movdqa	xmm9, xmm2
	xorps	xmm0, xmm0
	cmpltps	xmm0, xmm8
	pmovsxbd	xmm6, dword ptr [rip + __real@01010101]
	pblendvb	xmm6, xmmword ptr [rip + __xmm@00000002000000020000000200000002], xmm0
	pslld	xmm2, 31
	andps	xmm13, xmm8
	movaps	xmm4, xmmword ptr [rip + __xmm@bfc90fdbbfc90fdbbfc90fdbbfc90fdb]
	xorps	xmm4, xmm13
	movaps	xmm10, xmm8
	addps	xmm10, xmm4
	movaps	xmm11, xmm10
	subps	xmm11, xmm8
	movaps	xmm12, xmm8
	movdqa	xmm0, xmm2
	blendvps	xmm8, xmm10, xmm0
	subps	xmm10, xmm11
	subps	xmm12, xmm10
	subps	xmm4, xmm11
	addps	xmm4, xmm12
	orps	xmm13, xmmword ptr [rip + __xmm@333bbd2e333bbd2e333bbd2e333bbd2e]
	addps	xmm13, xmm7
	addps	xmm13, xmm4
	blendvps	xmm7, xmm13, xmm0
	paddd	xmm9, xmm9
	pand	xmm9, xmmword ptr [rip + __xmm@00000006000000060000000600000006]
	paddd	xmm6, xmm9
	addps	xmm7, xmm8
	cmpeqps	xmm3, xmmword ptr [rip + __xmm@7f8000007f8000007f8000007f800000]
	cmpunordps	xmm5, xmm1
	orps	xmm5, xmm3
	orps	xmm5, xmm7
	psrad	xmm6, 2
	jmp	.LBB4_5

	.def	burst.Sleef_sinf_u35_x64_sse4@@8;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@42fa0000
	.section	.rdata,"dr",discard,__real@42fa0000
	.p2align	2, 0x0
__real@42fa0000:
	.long	0x42fa0000
	.globl	__real@47185800
	.section	.rdata,"dr",discard,__real@47185800
	.p2align	2, 0x0
__real@47185800:
	.long	0x47185800
	.globl	__real@ff800000
	.section	.rdata,"dr",discard,__real@ff800000
	.p2align	2, 0x0
__real@ff800000:
	.long	0xff800000
	.globl	__real@7f800000
	.section	.rdata,"dr",discard,__real@7f800000
	.p2align	2, 0x0
__real@7f800000:
	.long	0x7f800000
	.globl	__real@7fc00000
	.section	.rdata,"dr",discard,__real@7fc00000
	.p2align	2, 0x0
__real@7fc00000:
	.long	0x7fc00000
	.globl	__real@3ea2f983
	.section	.rdata,"dr",discard,__real@3ea2f983
	.p2align	2, 0x0
__real@3ea2f983:
	.long	0x3ea2f983
	.globl	__real@bf0000003f000000
	.section	.rdata,"dr",discard,__real@bf0000003f000000
	.p2align	3, 0x0
__real@bf0000003f000000:
	.long	0x3f000000
	.long	0xbf000000
	.globl	__real@c0490000
	.section	.rdata,"dr",discard,__real@c0490000
	.p2align	2, 0x0
__real@c0490000:
	.long	0xc0490000
	.globl	__real@ba7d8000
	.section	.rdata,"dr",discard,__real@ba7d8000
	.p2align	2, 0x0
__real@ba7d8000:
	.long	0xba7d8000
	.globl	__real@b5288000
	.section	.rdata,"dr",discard,__real@b5288000
	.p2align	2, 0x0
__real@b5288000:
	.long	0xb5288000
	.globl	__real@af05a309
	.section	.rdata,"dr",discard,__real@af05a309
	.p2align	2, 0x0
__real@af05a309:
	.long	0xaf05a309
	.globl	__real@c0490e00
	.section	.rdata,"dr",discard,__real@c0490e00
	.p2align	2, 0x0
__real@c0490e00:
	.long	0xc0490e00
	.globl	__real@b8ed5000
	.section	.rdata,"dr",discard,__real@b8ed5000
	.p2align	2, 0x0
__real@b8ed5000:
	.long	0xb8ed5000
	.globl	__real@b1085a31
	.section	.rdata,"dr",discard,__real@b1085a31
	.p2align	2, 0x0
__real@b1085a31:
	.long	0xb1085a31
	.globl	__real@362f0a87
	.section	.rdata,"dr",discard,__real@362f0a87
	.p2align	2, 0x0
__real@362f0a87:
	.long	0x362f0a87
	.globl	__real@b94fbaeb
	.section	.rdata,"dr",discard,__real@b94fbaeb
	.p2align	2, 0x0
__real@b94fbaeb:
	.long	0xb94fbaeb
	.globl	__real@3c088777
	.section	.rdata,"dr",discard,__real@3c088777
	.p2align	2, 0x0
__real@3c088777:
	.long	0x3c088777
	.globl	__real@be2aaaa6
	.section	.rdata,"dr",discard,__real@be2aaaa6
	.p2align	2, 0x0
__real@be2aaaa6:
	.long	0xbe2aaaa6
	.globl	__real@80000000
	.section	.rdata,"dr",discard,__real@80000000
	.p2align	2, 0x0
__real@80000000:
	.long	0x80000000
	.section	.text,"xr",discard,burst.Sleef_sinf_u35_x64_sse4@@8
	.globl	burst.Sleef_sinf_u35_x64_sse4@@8
	.p2align	4
burst.Sleef_sinf_u35_x64_sse4@@8:
	sub	rsp, 72
	movaps	xmmword ptr [rsp + 48], xmm7
	movaps	xmmword ptr [rsp + 32], xmm6
	movaps	xmm1, xmmword ptr [rip + __xmm@7fffffff7fffffff7fffffff7fffffff]
	andps	xmm1, xmm0
	movss	xmm2, dword ptr [rip + __real@42fa0000]
	ucomiss	xmm2, xmm1
	jbe	.LBB5_2
	movss	xmm1, dword ptr [rip + __real@3ea2f983]
	mulss	xmm1, xmm0
	xorps	xmm2, xmm2
	xor	eax, eax
	ucomiss	xmm2, xmm1
	seta	al
	lea	rcx, [rip + __real@bf0000003f000000]
	addss	xmm1, dword ptr [rcx + 4*rax]
	cvttps2dq	xmm1, xmm1
	cvtdq2ps	xmm1, xmm1
	cvttss2si	ecx, xmm1
	cvttps2dq	xmm1, xmm1
	cvtdq2ps	xmm2, xmm1
	movss	xmm1, dword ptr [rip + __real@c0490e00]
	mulss	xmm1, xmm2
	addss	xmm1, xmm0
	movss	xmm3, dword ptr [rip + __real@b8ed5000]
	mulss	xmm3, xmm2
	addss	xmm3, xmm1
	mulss	xmm2, dword ptr [rip + __real@b1085a31]
	addss	xmm2, xmm3
	movd	eax, xmm0
	test	cl, 1
	movaps	xmm1, xmm2
	je	.LBB5_13
.LBB5_12:
	movaps	xmm1, xmmword ptr [rip + __xmm@80000000800000008000000080000000]
	xorps	xmm1, xmm2
.LBB5_13:
	neg	eax
	jo	.LBB5_15
	mulss	xmm2, xmm2
	movss	xmm0, dword ptr [rip + __real@362f0a87]
	mulss	xmm0, xmm2
	addss	xmm0, dword ptr [rip + __real@b94fbaeb]
	mulss	xmm0, xmm2
	addss	xmm0, dword ptr [rip + __real@3c088777]
	mulss	xmm0, xmm2
	addss	xmm0, dword ptr [rip + __real@be2aaaa6]
	mulss	xmm0, xmm1
	mulss	xmm0, xmm2
	addss	xmm0, xmm1
	movaps	xmm6, xmmword ptr [rsp + 32]
	movaps	xmm7, xmmword ptr [rsp + 48]
	add	rsp, 72
	ret
.LBB5_2:
	movss	xmm2, dword ptr [rip + __real@47185800]
	ucomiss	xmm2, xmm1
	jbe	.LBB5_4
	movss	xmm1, dword ptr [rip + __real@3ea2f983]
	mulss	xmm1, xmm0
	xorps	xmm2, xmm2
	xor	eax, eax
	ucomiss	xmm2, xmm1
	seta	al
	lea	rcx, [rip + __real@bf0000003f000000]
	addss	xmm1, dword ptr [rcx + 4*rax]
	cvttps2dq	xmm1, xmm1
	cvtdq2ps	xmm1, xmm1
	cvttss2si	ecx, xmm1
	cvttps2dq	xmm1, xmm1
	cvtdq2ps	xmm2, xmm1
	movss	xmm1, dword ptr [rip + __real@c0490000]
	mulss	xmm1, xmm2
	addss	xmm1, xmm0
	movss	xmm3, dword ptr [rip + __real@ba7d8000]
	mulss	xmm3, xmm2
	addss	xmm3, xmm1
	movss	xmm1, dword ptr [rip + __real@b5288000]
	mulss	xmm1, xmm2
	addss	xmm1, xmm3
	mulss	xmm2, dword ptr [rip + __real@af05a309]
	jmp	.LBB5_10
.LBB5_15:
	movss	xmm0, dword ptr [rip + __real@80000000]
	movaps	xmm6, xmmword ptr [rsp + 32]
	movaps	xmm7, xmmword ptr [rsp + 48]
	add	rsp, 72
	ret
.LBB5_4:
	movaps	xmm6, xmm0
	call	rempif_x64_sse4@@8
	movaps	xmm1, xmm0
	mov	ecx, eax
	and	ecx, 3
	xorps	xmm0, xmm0
	xor	edx, edx
	ucomiss	xmm1, xmm0
	seta	dl
	lea	ecx, [rdx + 2*rcx + 1]
	test	al, 1
	je	.LBB5_6
	movmskps	eax, xmm1
	shl	eax, 31
	mov	edx, eax
	xor	edx, -1077342245
	movd	xmm0, edx
	or	eax, 859553070
	movd	xmm2, eax
	movaps	xmm3, xmm1
	addss	xmm3, xmm0
	movaps	xmm4, xmm3
	subss	xmm4, xmm1
	movaps	xmm5, xmm3
	subss	xmm5, xmm4
	movshdup	xmm7, xmm1
	subss	xmm1, xmm5
	subss	xmm0, xmm4
	addss	xmm0, xmm1
	addss	xmm7, xmm2
	addss	xmm7, xmm0
	insertps	xmm3, xmm7, 16
	movaps	xmm1, xmm3
.LBB5_6:
	shr	ecx, 2
	movss	xmm2, dword ptr [rip + __real@ff800000]
	movaps	xmm0, xmm6
	ucomiss	xmm2, xmm6
	jae	.LBB5_11
	ucomiss	xmm0, dword ptr [rip + __real@7f800000]
	jae	.LBB5_11
	ucomiss	xmm0, xmm0
	jp	.LBB5_11
	movshdup	xmm2, xmm1
.LBB5_10:
	addss	xmm2, xmm1
	movd	eax, xmm0
	test	cl, 1
	movaps	xmm1, xmm2
	jne	.LBB5_12
	jmp	.LBB5_13
.LBB5_11:
	movss	xmm2, dword ptr [rip + __real@7fc00000]
	movd	eax, xmm0
	test	cl, 1
	movaps	xmm1, xmm2
	je	.LBB5_13
	jmp	.LBB5_12

	.def	rempif_x64_sse4@@8;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@3a800000
	.section	.rdata,"dr",discard,__real@3a800000
	.p2align	2, 0x0
__real@3a800000:
	.long	0x3a800000
	.globl	__real@44800000
	.section	.rdata,"dr",discard,__real@44800000
	.p2align	2, 0x0
__real@44800000:
	.long	0x44800000
	.globl	__real@41000000
	.section	.rdata,"dr",discard,__real@41000000
	.p2align	2, 0x0
__real@41000000:
	.long	0x41000000
	.globl	__real@40800000
	.section	.rdata,"dr",discard,__real@40800000
	.p2align	2, 0x0
__real@40800000:
	.long	0x40800000
	.globl	__real@3e800000
	.section	.rdata,"dr",discard,__real@3e800000
	.p2align	2, 0x0
__real@3e800000:
	.long	0x3e800000
	.globl	__real@3e000000
	.section	.rdata,"dr",discard,__real@3e000000
	.p2align	2, 0x0
__real@3e000000:
	.long	0x3e000000
	.globl	__real@501502f9
	.section	.rdata,"dr",discard,__real@501502f9
	.p2align	2, 0x0
__real@501502f9:
	.long	0x501502f9
	.globl	__real@3dffffff
	.section	.rdata,"dr",discard,__real@3dffffff
	.p2align	2, 0x0
__real@3dffffff:
	.long	0x3dffffff
	.globl	__real@40c90fdb
	.section	.rdata,"dr",discard,__real@40c90fdb
	.p2align	2, 0x0
__real@40c90fdb:
	.long	0x40c90fdb
	.globl	__real@40c90000
	.section	.rdata,"dr",discard,__real@40c90000
	.p2align	2, 0x0
__real@40c90000:
	.long	0x40c90000
	.globl	__real@3afdb000
	.section	.rdata,"dr",discard,__real@3afdb000
	.p2align	2, 0x0
__real@3afdb000:
	.long	0x3afdb000
	.globl	__real@b43bbd2e
	.section	.rdata,"dr",discard,__real@b43bbd2e
	.p2align	2, 0x0
__real@b43bbd2e:
	.long	0xb43bbd2e
	.globl	__real@3f333333
	.section	.rdata,"dr",discard,__real@3f333333
	.p2align	2, 0x0
__real@3f333333:
	.long	0x3f333333
	.section	.text,"xr",discard,rempif_x64_sse4@@8
	.globl	rempif_x64_sse4@@8
	.p2align	4
rempif_x64_sse4@@8:
	push	rsi
	sub	rsp, 160
	movaps	xmmword ptr [rsp + 144], xmm15
	movaps	xmmword ptr [rsp + 128], xmm14
	movaps	xmmword ptr [rsp + 112], xmm13
	movaps	xmmword ptr [rsp + 96], xmm12
	movaps	xmmword ptr [rsp + 80], xmm11
	movaps	xmmword ptr [rsp + 64], xmm10
	movaps	xmmword ptr [rsp + 48], xmm9
	movaps	xmmword ptr [rsp + 32], xmm8
	movaps	xmmword ptr [rsp + 16], xmm7
	movaps	xmmword ptr [rsp], xmm6
	movd	eax, xmm0
	mov	ecx, eax
	shr	ecx, 23
	movzx	ecx, cl
	xor	edx, edx
	cmp	ecx, 218
	setb	dl
	shl	edx, 29
	lea	r8d, [rdx + rax - 536870912]
	movd	xmm2, r8d
	xor	eax, eax
	cmp	ecx, 152
	lea	edx, [4*rcx - 608]
	cmovb	edx, eax
	movsxd	r9, edx
	lea	rcx, [rip + rempitabsp]
	movd	xmm9, dword ptr [rcx + 4*r9]
	mov	r9d, r8d
	and	r9d, -4096
	movd	xmm4, r9d
	movdqa	xmm3, xmm2
	subss	xmm3, xmm4
	movd	r9d, xmm9
	and	r9d, -4096
	movd	xmm0, r9d
	movdqa	xmm5, xmm9
	subss	xmm5, xmm0
	mulss	xmm9, xmm2
	movaps	xmm1, xmm4
	mulss	xmm1, xmm0
	subss	xmm1, xmm9
	mulss	xmm0, xmm3
	addss	xmm0, xmm1
	movaps	xmm1, xmm3
	mulss	xmm1, xmm5
	mulss	xmm5, xmm4
	addss	xmm5, xmm0
	addss	xmm5, xmm1
	movss	xmm15, dword ptr [rip + __real@3a800000]
	movaps	xmm0, xmm9
	mulss	xmm0, xmm15
	cvttps2dq	xmm0, xmm0
	cvtdq2ps	xmm0, xmm0
	mulss	xmm0, dword ptr [rip + __real@44800000]
	movaps	xmm10, xmm9
	subss	xmm10, xmm0
	xorps	xmm0, xmm0
	xor	r9d, r9d
	ucomiss	xmm9, xmm0
	seta	r9b
	movss	xmm8, dword ptr [rip + __real@41000000]
	movaps	xmm0, xmm10
	mulss	xmm0, xmm8
	movmskps	r10d, xmm9
	shl	r10d, 31
	or	r10d, 1056964608
	movd	xmm1, r10d
	cvttss2si	r10d, xmm0
	movss	xmm14, dword ptr [rip + __real@40800000]
	movaps	xmm0, xmm10
	mulss	xmm0, xmm14
	addss	xmm0, xmm1
	cvttps2dq	xmm0, xmm0
	cvtdq2ps	xmm0, xmm0
	mulss	xmm0, dword ptr [rip + __real@3e800000]
	subss	xmm10, xmm0
	movaps	xmm13, xmmword ptr [rip + __xmm@7fffffff7fffffff7fffffff7fffffff]
	movaps	xmm6, xmm10
	andps	xmm6, xmm13
	movaps	xmm7, xmm10
	subss	xmm7, xmm1
	movss	xmm1, dword ptr [rip + __real@3e000000]
	movaps	xmm0, xmm1
	cmpltss	xmm0, xmm6
	blendvps	xmm10, xmm7, xmm0
	movaps	xmm0, xmm10
	andps	xmm0, xmm13
	movss	xmm6, dword ptr [rip + __real@501502f9]
	movaps	xmm7, xmm6
	cmpltss	xmm7, xmm0
	lea	r9d, [r9 + r10 + 3]
	and	r9d, 7
	add	r9d, -3
	sar	r9d
	andnps	xmm7, xmm10
	movaps	xmm0, xmm9
	andps	xmm0, xmm13
	ucomiss	xmm0, dword ptr [rip + __real@3dffffff]
	jne	.LBB6_3
	jp	.LBB6_3
	movaps	xmm7, xmm9
.LBB6_3:
	movaps	xmm9, xmm5
	addss	xmm9, xmm7
	lea	r10d, [rdx + 1]
	movsxd	r10, r10d
	movss	xmm11, dword ptr [rcx + 4*r10]
	movaps	xmm10, xmm11
	mulss	xmm10, xmm2
	movaps	xmm12, xmm10
	addss	xmm12, xmm9
	mulss	xmm15, xmm12
	cvttps2dq	xmm0, xmm15
	cvtdq2ps	xmm0, xmm0
	mulss	xmm0, dword ptr [rip + __real@44800000]
	movaps	xmm15, xmm12
	subss	xmm15, xmm0
	mov	r10d, 0
	cmovne	r10d, r9d
	cmovp	r10d, r9d
	movd	r9d, xmm11
	and	r9d, -4096
	xor	r11d, r11d
	xorps	xmm0, xmm0
	ucomiss	xmm12, xmm0
	seta	r11b
	mulss	xmm14, xmm15
	movmskps	esi, xmm12
	shl	esi, 31
	or	esi, 1056964608
	movd	xmm0, esi
	addss	xmm14, xmm0
	cvttps2dq	xmm14, xmm14
	cvtdq2ps	xmm14, xmm14
	mulss	xmm14, dword ptr [rip + __real@3e800000]
	mulss	xmm8, xmm15
	subss	xmm15, xmm14
	movaps	xmm14, xmm15
	subss	xmm14, xmm0
	movaps	xmm0, xmm15
	andps	xmm0, xmm13
	cmpltss	xmm1, xmm0
	movaps	xmm0, xmm1
	blendvps	xmm15, xmm14, xmm0
	cvttss2si	esi, xmm8
	lea	r11d, [r11 + rsi + 3]
	and	r11d, 7
	add	r11d, -3
	sar	r11d
	movaps	xmm0, xmm15
	andps	xmm0, xmm13
	andps	xmm13, xmm12
	ucomiss	xmm13, dword ptr [rip + __real@3dffffff]
	cmpltss	xmm6, xmm0
	andnps	xmm6, xmm15
	jne	.LBB6_6
	jp	.LBB6_6
	movaps	xmm6, xmm12
.LBB6_6:
	cmovne	eax, r11d
	cmovp	eax, r11d
	add	eax, r10d
	and	r8d, 2147483647
	movd	xmm0, r8d
	movss	xmm1, dword ptr [rip + __real@3f333333]
	ucomiss	xmm1, xmm0
	ja	.LBB6_7
	movd	xmm0, r9d
	movaps	xmm1, xmm4
	mulss	xmm1, xmm0
	subss	xmm11, xmm0
	subss	xmm1, xmm10
	mulss	xmm0, xmm3
	addss	xmm0, xmm1
	movaps	xmm1, xmm3
	mulss	xmm1, xmm11
	mulss	xmm11, xmm4
	addss	xmm11, xmm0
	movaps	xmm0, xmm12
	subss	xmm0, xmm9
	subss	xmm7, xmm9
	addss	xmm11, xmm1
	subss	xmm12, xmm0
	addss	xmm5, xmm7
	addss	xmm5, xmm11
	subss	xmm9, xmm12
	subss	xmm10, xmm0
	addss	xmm10, xmm9
	addss	xmm10, xmm5
	movaps	xmm0, xmm10
	addss	xmm0, xmm6
	subss	xmm6, xmm0
	addss	xmm6, xmm10
	lea	r8d, [rdx + 2]
	movsxd	r8, r8d
	movd	xmm5, dword ptr [rcx + 4*r8]
	or	edx, 3
	movsxd	rdx, edx
	movd	r8d, xmm5
	and	r8d, -4096
	movd	xmm1, r8d
	movdqa	xmm7, xmm5
	subss	xmm7, xmm1
	mulss	xmm5, xmm2
	movaps	xmm8, xmm7
	mulss	xmm8, xmm4
	mulss	xmm4, xmm1
	subss	xmm4, xmm5
	addss	xmm4, xmm8
	mulss	xmm1, xmm3
	addss	xmm1, xmm4
	mulss	xmm7, xmm3
	addss	xmm7, xmm1
	mulss	xmm2, dword ptr [rcx + 4*rdx]
	addss	xmm2, xmm7
	addss	xmm2, xmm6
	movaps	xmm1, xmm5
	addss	xmm1, xmm0
	movaps	xmm3, xmm1
	subss	xmm3, xmm0
	movaps	xmm4, xmm1
	subss	xmm4, xmm3
	subss	xmm0, xmm4
	subss	xmm5, xmm3
	addss	xmm5, xmm0
	addss	xmm5, xmm2
	movaps	xmm2, xmm1
	addss	xmm2, xmm5
	subss	xmm1, xmm2
	addss	xmm1, xmm5
	movd	ecx, xmm2
	and	ecx, -4096
	movd	xmm3, ecx
	movaps	xmm4, xmm2
	subss	xmm4, xmm3
	movss	xmm5, dword ptr [rip + __real@40c90fdb]
	movaps	xmm0, xmm2
	mulss	xmm0, xmm5
	movss	xmm6, dword ptr [rip + __real@40c90000]
	movaps	xmm7, xmm3
	mulss	xmm7, xmm6
	subss	xmm7, xmm0
	mulss	xmm6, xmm4
	addss	xmm6, xmm7
	movss	xmm7, dword ptr [rip + __real@3afdb000]
	mulss	xmm3, xmm7
	addss	xmm3, xmm6
	mulss	xmm4, xmm7
	addss	xmm4, xmm3
	mulss	xmm2, dword ptr [rip + __real@b43bbd2e]
	addss	xmm2, xmm4
	mulss	xmm1, xmm5
	addss	xmm1, xmm2
	insertps	xmm0, xmm1, 16
	jmp	.LBB6_9
.LBB6_7:
	xorps	xmm0, xmm0
	movss	xmm0, xmm2
.LBB6_9:
	movaps	xmm6, xmmword ptr [rsp]
	movaps	xmm7, xmmword ptr [rsp + 16]
	movaps	xmm8, xmmword ptr [rsp + 32]
	movaps	xmm9, xmmword ptr [rsp + 48]
	movaps	xmm10, xmmword ptr [rsp + 64]
	movaps	xmm11, xmmword ptr [rsp + 80]
	movaps	xmm12, xmmword ptr [rsp + 96]
	movaps	xmm13, xmmword ptr [rsp + 112]
	movaps	xmm14, xmmword ptr [rsp + 128]
	movaps	xmm15, xmmword ptr [rsp + 144]
	add	rsp, 160
	pop	rsi
	ret

	.section	.drectve,"yni"
	.ascii	" /EXPORT:50f894ef9d69c624ac3acb4c44ed832d"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.50f894ef9d69c624ac3acb4c44ed832d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.50f894ef9d69c624ac3acb4c44ed832d\""
	.globl	_fltused
