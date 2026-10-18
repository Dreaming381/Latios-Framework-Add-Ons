
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
.Lfunc_begin0:
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
.Lfunc_end0:
	.seh_endproc

	.def	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d:
.Lfunc_begin1:
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
.Lfunc_end1:
	.seh_endproc

	.def	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d:
.Lfunc_begin2:
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
.Lfunc_end2:
	.seh_endproc

	.def	d2e6ffc2a276816faeb5eba8fa179d4d;
	.scl	2;
	.type	32;
	.endef
	.globl	d2e6ffc2a276816faeb5eba8fa179d4d
	.p2align	4
d2e6ffc2a276816faeb5eba8fa179d4d:
.Lfunc_begin3:
.seh_proc d2e6ffc2a276816faeb5eba8fa179d4d
	push	rbp
	.seh_pushreg rbp
	sub	rsp, 32
	.seh_stackalloc 32
	lea	rbp, [rsp + 32]
	.seh_setframe rbp, 32
	.seh_endprologue
	call	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	nop
	.seh_startepilogue
	add	rsp, 32
	pop	rbp
	.seh_endepilogue
	ret
.Lfunc_end3:
	.seh_endproc

	.def	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	3;
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
	.p2align	4
"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.Lfunc_begin4:
	.cv_func_id 0
	.cv_file	1 "unknown\\unknown"
	.cv_loc	0 1 0 0
.seh_proc "Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
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
.Ltmp0:
	.cv_file	2 "C:\\Project\\Assets\\Validation\\BurstWaitress\\BwKernels.cs" "42B6FA50B986BB7982390AB18996455657908AE625F89C5F362A7397E6801A2A" 3
	.cv_inline_site_id 1 within 0 inlined_at 1 0 0
	.cv_inline_site_id 2 within 1 inlined_at 1 0 0
	.cv_inline_site_id 3 within 2 inlined_at 2 51 0
	.cv_loc	3 2 65 0
	test	r9d, r9d
	jle	.LBB4_11
.Ltmp1:
	.cv_loc	2 2 51 0
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
.Ltmp2:
	.cv_loc	3 2 65 0
	mov	ebx, r9d
	cmp	r9d, 40
	jae	.LBB4_3
	xor	r9d, r9d
.LBB4_9:
	vmovss	xmm0, dword ptr [rip + __real@3f800000]
	.p2align	4
.LBB4_10:
	.cv_loc	3 2 67 0
	vmovss	xmm1, dword ptr [r10 + 4*r9]
	vmulss	xmm1, xmm1, dword ptr [r11 + 4*r9]
	.cv_loc	3 2 68 0
	vmulss	xmm3, xmm1, dword ptr [rsi + 4*r9]
	vmovss	dword ptr [rcx + 4*r9], xmm3
	.cv_loc	3 2 69 0
	vmulss	xmm3, xmm1, dword ptr [rdi + 4*r9]
	vmovss	dword ptr [rax + 4*r9], xmm3
	.cv_loc	3 2 70 0
	vmulss	xmm3, xmm2, xmm1
	vmovss	dword ptr [rdx + 4*r9], xmm3
	.cv_loc	3 2 71 0
	vsubss	xmm1, xmm0, xmm1
	vmovss	dword ptr [r8 + 4*r9], xmm1
	.cv_loc	3 2 65 0
	inc	r9
	cmp	rbx, r9
	jne	.LBB4_10
.Ltmp3:
.LBB4_11:
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
.LBB4_3:
.Ltmp4:
	.cv_loc	2 2 51 0
	vpmovsxdq	ymm4, xmm4
	vpermq	ymm6, ymm0, 169
	vpbroadcastq	ymm7, xmm0
	vpsllq	ymm4, ymm4, 2
.Ltmp5:
	.cv_loc	3 2 65 0
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
	jne	.LBB4_9
	cmp	r14, 32
	jb	.LBB4_9
	sub	r15, r12
	cmp	r15, 32
	jb	.LBB4_9
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
.LBB4_7:
	.cv_loc	3 2 67 0
	vmovups	ymm3, ymmword ptr [r10 + r15]
	vmulps	ymm3, ymm3, ymmword ptr [r11 + r15]
	.cv_loc	3 2 68 0
	vmulps	ymm4, ymm3, ymmword ptr [rsi + r15]
	vmovups	ymmword ptr [rcx + r15], ymm4
	.cv_loc	3 2 69 0
	vmulps	ymm4, ymm3, ymmword ptr [rdi + r15]
	vmovups	ymmword ptr [rax + r15], ymm4
	.cv_loc	3 2 70 0
	vmulps	ymm4, ymm0, ymm3
	vmovups	ymmword ptr [rdx + r15], ymm4
	.cv_loc	3 2 71 0
	vsubps	ymm3, ymm1, ymm3
	vmovups	ymmword ptr [r8 + r15], ymm3
	.cv_loc	3 2 65 0
	add	r15, 32
	cmp	r14, r15
	jne	.LBB4_7
	cmp	r9d, ebx
	jne	.LBB4_9
	jmp	.LBB4_11
.Ltmp6:
.Lfunc_end4:
	.seh_endproc

	.section	.drectve,"yni"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.d2e6ffc2a276816faeb5eba8fa179d4d\""
	.ascii	" /EXPORT:d2e6ffc2a276816faeb5eba8fa179d4d"
	.section	.debug$S,"dr"
	.p2align	2, 0x0
	.long	4
	.long	241
	.long	.Ltmp8-.Ltmp7
.Ltmp7:
	.short	.Ltmp10-.Ltmp9
.Ltmp9:
	.short	4353
	.long	0
	.byte	0
	.p2align	2, 0x0
.Ltmp10:
	.short	.Ltmp12-.Ltmp11
.Ltmp11:
	.short	4412
	.long	0
	.short	208
	.short	0
	.short	0
	.short	106
	.short	0
	.short	21013
	.short	0
	.short	0
	.short	0
	.asciz	"Burst 0.0.106.0 (Frontend Version : 5c2a062f-e02a-4f6e-9f19-390d3b7b4649)"
	.p2align	2, 0x0
.Ltmp12:
.Ltmp8:
	.p2align	2, 0x0
	.long	246
	.long	.Ltmp14-.Ltmp13
.Ltmp13:
	.long	0


	.long	4098
	.cv_filechecksumoffset	1
	.long	0


	.long	4099
	.cv_filechecksumoffset	2
	.long	51


	.long	4100
	.cv_filechecksumoffset	2
	.long	65
.Ltmp14:
	.p2align	2, 0x0
	.long	241
	.long	.Ltmp16-.Ltmp15
.Ltmp15:
	.short	.Ltmp18-.Ltmp17
.Ltmp17:
	.short	4422
	.long	0
	.long	0
	.long	0
	.long	.Lfunc_end4-"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.long	0
	.long	0
	.long	4101
	.secrel32	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.secidx	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.byte	193
	.asciz	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2, 0x0
.Ltmp18:
	.short	.Ltmp20-.Ltmp19
.Ltmp19:
	.short	4114
	.long	64
	.long	0
	.long	0
	.long	56
	.long	0
	.short	0
	.long	1220608
	.p2align	2, 0x0
.Ltmp20:
	.short	.Ltmp22-.Ltmp21
.Ltmp21:
	.short	4456
	.long	1
	.long	4098
	.p2align	2, 0x0
.Ltmp22:
	.short	.Ltmp24-.Ltmp23
.Ltmp23:
	.short	4429
	.long	0
	.long	0
	.long	4098
	.cv_inline_linetable	1 1 0 .Lfunc_begin4 .Lfunc_end4
	.p2align	2, 0x0
.Ltmp24:
	.short	.Ltmp26-.Ltmp25
.Ltmp25:
	.short	4429
	.long	0
	.long	0
	.long	4099
	.cv_inline_linetable	2 2 51 .Lfunc_begin4 .Lfunc_end4
	.p2align	2, 0x0
.Ltmp26:
	.short	.Ltmp28-.Ltmp27
.Ltmp27:
	.short	4429
	.long	0
	.long	0
	.long	4100
	.cv_inline_linetable	3 2 65 .Lfunc_begin4 .Lfunc_end4
	.p2align	2, 0x0
.Ltmp28:
	.short	2
	.short	4430
	.short	2
	.short	4430
	.short	2
	.short	4430
	.short	2
	.short	4431
.Ltmp16:
	.p2align	2, 0x0
	.cv_linetable	0, "Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", .Lfunc_end4
	.cv_filechecksums
	.cv_stringtable
	.long	241
	.long	.Ltmp30-.Ltmp29
.Ltmp29:
	.short	.Ltmp32-.Ltmp31
.Ltmp31:
	.short	4428
	.long	4105
	.p2align	2, 0x0
.Ltmp32:
.Ltmp30:
	.p2align	2, 0x0
	.section	.debug$T,"dr"
	.p2align	2, 0x0
	.long	4
	.short	0x6
	.short	0x1201
	.long	0x0
	.short	0xe
	.short	0x1008
	.long	0x3
	.byte	0x0
	.byte	0x0
	.short	0x0
	.long	0x1000
	.short	0x86
	.short	0x1601
	.long	0x0
	.long	0x1001
	.asciz	"Latios.Validation.BurstWaitress.BwKernels.Latios.Validation.BurstWaitress.Channels8Plain_00000012$BurstDirectCall.Invoke"
	.byte	243
	.byte	242
	.byte	241
	.short	0x52
	.short	0x1601
	.long	0x0
	.long	0x1001
	.asciz	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain$BurstManaged"
	.byte	242
	.byte	241
	.short	0x46
	.short	0x1601
	.long	0x0
	.long	0x1001
	.asciz	"Latios.Validation.BurstWaitress.BwKernels.ManyChannelsPlain"
	.short	0xfa
	.short	0x1601
	.long	0x0
	.long	0x1001
	.asciz	"Latios.Validation.BurstWaitress.BwKernels.Channels8Plain(float* arena, int stride, float dt, int count) -> void_5250aef4a1bf354f590c69c4b3bed2ed from Latios.Validation.BurstWaitress, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.byte	243
	.byte	242
	.byte	241
	.short	0x3a
	.short	0x1605
	.long	0x0
	.asciz	"C:\\Project\\unknown"
	.short	0xe
	.short	0x1605
	.long	0x0
	.asciz	"unknown"
	.short	0xa
	.short	0x1605
	.long	0x0
	.byte	0
	.byte	243
	.byte	242
	.byte	241
	.short	0x1a
	.short	0x1603
	.short	0x5
	.long	0x1006
	.long	0x1008
	.long	0x1007
	.long	0x1008
	.long	0x1008
	.byte	242
	.byte	241
	.globl	_fltused
