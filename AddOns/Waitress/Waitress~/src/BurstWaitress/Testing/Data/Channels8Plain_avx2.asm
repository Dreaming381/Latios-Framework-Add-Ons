
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

	.def	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d
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

	.def	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d
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

	.def	d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	__ymm@0000000600000005000000040000000200000007000000060000000500000004
	.section	.rdata,"dr",discard,__ymm@0000000600000005000000040000000200000007000000060000000500000004
	.p2align	5, 0x0
__ymm@0000000600000005000000040000000200000007000000060000000500000004:
	.long	4
	.long	5
	.long	6
	.long	7
	.long	2
	.long	4
	.long	5
	.long	6
	.globl	__real@8000000000000000
	.section	.rdata,"dr",discard,__real@8000000000000000
	.p2align	3, 0x0
__real@8000000000000000:
	.quad	-9223372036854775808
	.globl	__real@8000000000000020
	.section	.rdata,"dr",discard,__real@8000000000000020
	.p2align	3, 0x0
__real@8000000000000020:
	.quad	-9223372036854775776
	.globl	__xmm@0000000000000000000000000c080400
	.section	.rdata,"dr",discard,__xmm@0000000000000000000000000c080400
	.p2align	4, 0x0
__xmm@0000000000000000000000000c080400:
	.byte	0
	.byte	4
	.byte	8
	.byte	12
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.zero	1
	.globl	__real@3f800000
	.section	.rdata,"dr",discard,__real@3f800000
	.p2align	2, 0x0
__real@3f800000:
	.long	0x3f800000
	.text
	.globl	d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
d2e6ffc2a276816faeb5eba8fa179d4d:
.seh_proc d2e6ffc2a276816faeb5eba8fa179d4d
	push	rbp
	.seh_pushreg rbp
	push	r15
	.seh_pushreg r15
	push	r14
	.seh_pushreg r14
	push	r13
	.seh_pushreg r13
	push	r12
	.seh_pushreg r12
	push	rsi
	.seh_pushreg rsi
	push	rdi
	.seh_pushreg rdi
	push	rbx
	.seh_pushreg rbx
	sub	rsp, 56
	.seh_stackalloc 56
	lea	rbp, [rsp + 48]
	.seh_setframe rbp, 48
	vmovdqa	xmmword ptr [rbp - 16], xmm8
	.seh_savexmm xmm8, 32
	vmovdqa	xmmword ptr [rbp - 32], xmm7
	.seh_savexmm xmm7, 16
	vmovdqa	xmmword ptr [rbp - 48], xmm6
	.seh_savexmm xmm6, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB3_11
	vmovd	xmm0, edx
	vpbroadcastd	ymm0, xmm0
	vpmulld	ymm1, ymm0, ymmword ptr [rip + __ymm@0000000600000005000000040000000200000007000000060000000500000004]
	lea	eax, [rdx + 2*rdx]
	vpinsrd	xmm0, xmm0, eax, 1
	vextracti128	xmm4, ymm1, 1
	vpunpcklqdq	xmm0, xmm0, xmm4
	vpmovsxdq	ymm0, xmm0
	vpmovsxdq	ymm3, xmm1
	vpsllq	ymm0, ymm0, 2
	vmovq	r14, xmm0
	lea	rax, [rcx + r14]
	vpsllq	ymm1, ymm3, 2
	vextracti128	xmm5, ymm0, 1
	vmovq	rdx, xmm5
	add	rdx, rcx
	vpextrq	r15, xmm0, 1
	lea	r8, [rcx + r15]
	vmovq	r10, xmm1
	add	r10, rcx
	vpextrq	r11, xmm1, 1
	add	r11, rcx
	vextracti128	xmm5, ymm1, 1
	vmovq	rsi, xmm5
	add	rsi, rcx
	vpextrq	r12, xmm5, 1
	lea	rdi, [rcx + r12]
	mov	ebx, r9d
	cmp	r9d, 40
	jae	.LBB3_3
	xor	r9d, r9d
.LBB3_9:
	vmovss	xmm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB3_10:
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
	jne	.LBB3_10
.LBB3_11:
	vmovaps	xmm6, xmmword ptr [rbp - 48]
	vmovaps	xmm7, xmmword ptr [rbp - 32]
	vmovaps	xmm8, xmmword ptr [rbp - 16]
	.seh_startepilogue
	add	rsp, 56
	pop	rbx
	pop	rdi
	pop	rsi
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	.seh_endepilogue
	vzeroupper
	ret
.LBB3_3:
	vpmovsxdq	ymm4, xmm4
	vpermq	ymm6, ymm0, 169
	vpbroadcastq	ymm7, xmm0
	vpsllq	ymm4, ymm4, 2
	vpsubq	ymm4, ymm6, ymm4
	vpsllq	xmm6, xmm3, 2
	vpxor	xmm8, xmm8, xmm8
	vpsubq	xmm6, xmm8, xmm6
	vextracti128	xmm3, ymm3, 1
	vmovq	r9, xmm3
	shl	r9, 2
	neg	r9
	vpermq	ymm3, ymm0, 230
	vpbroadcastq	xmm8, xmm0
	vpsubq	xmm3, xmm3, xmm8
	vpsubq	ymm7, ymm7, ymm1
	vmovq	xmm8, r9
	vpblendd	xmm5, xmm8, xmm5, 12
	vinserti128	ymm8, ymm0, xmm6, 1
	vperm2i128	ymm6, ymm0, ymm6, 33
	vpblendd	ymm6, ymm6, ymm8, 204
	vinserti128	ymm3, ymm5, xmm3, 1
	vpbroadcastq	ymm5, qword ptr [rip + __real@8000000000000000]
	vpxor	ymm6, ymm6, ymm5
	vpbroadcastq	ymm8, qword ptr [rip + __real@8000000000000020]
	vpcmpgtq	ymm6, ymm8, ymm6
	vpxor	ymm3, ymm3, ymm5
	vpcmpgtq	ymm3, ymm8, ymm3
	vpackssdw	ymm3, ymm6, ymm3
	vpermq	ymm3, ymm3, 216
	vpxor	ymm6, ymm7, ymm5
	vpcmpgtq	ymm6, ymm8, ymm6
	vpxor	ymm4, ymm4, ymm5
	vpcmpgtq	ymm4, ymm8, ymm4
	vpackssdw	ymm4, ymm6, ymm4
	vpermq	ymm4, ymm4, 216
	vpackssdw	ymm3, ymm3, ymm4
	vextracti128	xmm4, ymm3, 1
	vpacksswb	xmm3, xmm3, xmm4
	vpermq	ymm0, ymm0, 149
	vpsubq	ymm0, ymm0, ymm1
	vpxor	ymm0, ymm0, ymm5
	vpcmpgtq	ymm0, ymm8, ymm0
	vextracti128	xmm1, ymm0, 1
	vpackssdw	xmm0, xmm0, xmm1
	vpshufb	xmm0, xmm0, xmmword ptr [rip + __xmm@0000000000000000000000000c080400]
	vpor	xmm0, xmm3, xmm0
	vpshufd	xmm1, xmm3, 216
	vpblendd	xmm0, xmm1, xmm0, 1
	vpsllw	xmm0, xmm0, 7
	vpmovmskb	r13d, xmm0
	xor	r9d, r9d
	test	r13d, r13d
	jne	.LBB3_9
	cmp	r14, 32
	jb	.LBB3_9
	sub	r15, r12
	cmp	r15, 32
	jb	.LBB3_9
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
.LBB3_7:
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
	jne	.LBB3_7
	cmp	r9d, ebx
	jne	.LBB3_9
	jmp	.LBB3_11
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:d2e6ffc2a276816faeb5eba8fa179d4d"
	.globl	_fltused
