
	.def	"@feat.00";
	.scl	3;
	.type	0;
	.endef
	.globl	"@feat.00"
"@feat.00" = 0
	.file	"main"
	.def	3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	"__real@bf46f0068db8bac7"
	.section	.rdata,"dr",discard,"__real@bf46f0068db8bac7"
	.p2align	3, 0x0
"__real@bf46f0068db8bac7":
	.xword	0xbf46f0068db8bac7
	.text
	.globl	3d466577b535881ddd547f1bda3cca2d
	.p2align	2
3d466577b535881ddd547f1bda3cca2d:
.seh_proc 3d466577b535881ddd547f1bda3cca2d
	sub	sp, sp, #240
	.seh_stackalloc	240
	stp	x19, x20, [sp, #96]
	.seh_save_regp	x19, 96
	stp	x21, x22, [sp, #112]
	.seh_save_regp	x21, 112
	stp	x23, x24, [sp, #128]
	.seh_save_regp	x23, 128
	stp	x25, x26, [sp, #144]
	.seh_save_regp	x25, 144
	stp	x27, x28, [sp, #160]
	.seh_save_regp	x27, 160
	stp	x29, x30, [sp, #176]
	.seh_save_fplr	176
	stp	d8, d9, [sp, #192]
	.seh_save_fregp	d8, 192
	stp	d10, d11, [sp, #208]
	.seh_save_fregp	d10, 208
	stp	d12, d13, [sp, #224]
	.seh_save_fregp	d12, 224
	add	x29, sp, #176
	.seh_add_fp	176
	.seh_endprologue
	adrp	x19, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	mov	x9, x3
	mov	w1, w4
	ldr	x8, [x19, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	mov	x21, x0
	add	x2, sp, #72
	add	x3, sp, #76
	mov	x0, x9
	str	xzr, [sp, #72]
	str	x9, [sp, #16]
	str	w4, [sp, #28]
	blr	x8
	tst	w0, #0xff
	b.eq	.LBB0_36
	ldr	x8, [x19, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	movi	d8, #0000000000000000
	fmov	d9, #1.00000000
	str	x8, [sp, #8]
	adrp	x8, "__real@bf46f0068db8bac7"
	ldr	d10, [x8, :lo12:"__real@bf46f0068db8bac7"]
	b	.LBB0_3
.LBB0_2:
	ldp	x8, x0, [sp, #8]
	add	x2, sp, #72
	ldr	w1, [sp, #28]
	add	x3, sp, #76
	blr	x8
	tst	w0, #0xff
	b.eq	.LBB0_36
.LBB0_3:
	ldp	w20, w8, [sp, #72]
	sxtw	x20, w20
	str	w8, [sp, #40]
	cmp	w20, w8
	b.ge	.LBB0_2
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"
	ldr	x8, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"]
	str	x8, [sp, #32]
	b	.LBB0_7
.LBB0_5:
	str	w27, [x24, #32]
.LBB0_6:
	ldr	w8, [sp, #40]
	add	x20, x20, #1
	cmp	w8, w20
	b.eq	.LBB0_2
.LBB0_7:
	ldr	x8, [x21, #208]
	movi	v0.2d, #0000000000000000
	ldr	d1, [x8, x20, lsl #3]
	fmov	w23, s1
	str	q0, [sp, #80]
	tbnz	w23, #31, .LBB0_6
	ldr	x8, [x21, #176]
	ldr	x22, [x21, #256]
	str	q1, [sp, #48]
	ldp	w9, w25, [x8, #16]
	ldrsw	x28, [x8]
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"
	sxtw	x9, w9
	ldr	x19, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"]
	ldr	x8, [sp, #32]
	str	x9, [sp, #64]
	blr	x8
	sxtw	x9, w0
	ldr	x8, [x21]
	add	x10, x19, x9, lsl #6
	ldr	w9, [x10, #56]
	ldr	w27, [x10, #32]
	ldr	x24, [x10, #48]
	add	w19, w9, #1
	str	w9, [sp, #44]
	str	w19, [x10, #56]
	ldr	x11, [x8, #24]
	ldr	x8, [x11]
	ldrsw	x9, [x11, #8]
	ldur	d0, [x11, #12]
	add	x12, x8, x9
	add	x13, x8, #1, lsl #12
	str	d0, [x21, #56]
	ldp	w9, w11, [x11, #16]
	stp	x8, x12, [x21, #32]
	sxtw	x9, w9
	str	x13, [x21, #48]
	str	w11, [x21, #64]
	cbnz	w11, .LBB0_10
	add	x8, x8, x9
	str	x8, [x21, #48]
.LBB0_10:
	ldr	x8, [x21, #88]
	mov	w9, #24
	ldr	x8, [x8, #24]
	madd	x10, x20, x9, x8
	ldp	w26, w8, [x10, #12]
	ldr	x9, [x10]
	ldrsw	x11, [x10, #8]
	ldr	w10, [x10, #20]
	sxtw	x8, w8
	add	x11, x9, x11
	str	w10, [x21, #152]
	stp	x9, x11, [x21, #120]
	add	x11, x9, #1, lsl #12
	stp	w26, w8, [x21, #144]
	str	x11, [x21, #136]
	cbz	w10, .LBB0_12
	ldr	q0, [sp, #48]
	cmp	w26, #1
	b.ge	.LBB0_13
	b	.LBB0_33
.LBB0_12:
	add	x8, x9, x8
	str	x8, [x21, #136]
	ldr	q0, [sp, #48]
	cmp	w26, #1
	b.lt	.LBB0_33
.LBB0_13:
	ldr	x8, [sp, #64]
	mov	x10, x22
	mov	w22, v0.s[1]
	scvtf	d11, w25
	add	x23, x10, w23, uxtw #2
	smull	x9, w8, w28
	scvtf	d12, w8
	str	x9, [sp, #48]
	ucvtf	d13, x9
	b	.LBB0_16
.LBB0_14:
	fmul	d0, d1, d12
	scvtf	d4, w9
	add	x1, sp, #80
	and	w3, w8, #0x1
	mov	x0, x21
	mov	x4, x23
	mov	x5, x22
	str	x24, [sp, #80]
	stp	w27, w19, [sp, #88]
	fmul	d0, d0, d4
	fadd	d0, d0, d3
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB0_15:
	subs	w26, w26, #1
	b.eq	.LBB0_33
.LBB0_16:
	ldp	x8, x10, [x21, #128]
	ldr	w9, [x21, #144]
	sub	w9, w9, #1
	add	x11, x8, #24
	str	w9, [x21, #144]
	cmp	x11, x10
	str	x11, [x21, #128]
	b.ls	.LBB0_21
	ldr	w8, [x21, #152]
	sub	w9, w8, #1
	ldr	x8, [x21, #120]
	str	w9, [x21, #152]
	cmp	w9, #0
	ldr	x8, [x8]
	str	x8, [x21, #120]
	b.le	.LBB0_19
	mov	w9, #4096
	b	.LBB0_20
.LBB0_19:
	ldrsw	x9, [x21, #148]
.LBB0_20:
	add	x9, x8, x9
	add	x10, x8, #32
	add	x8, x8, #8
	stp	x10, x9, [x21, #128]
.LBB0_21:
	ldr	x10, [x8, #8]
	fmov	d0, #1.00000000
	ldr	s2, [x8, #4]
	ldrb	w11, [x8]
	ldr	w8, [x8, #20]
	ldp	x2, x9, [x10]
	tbz	w11, #1, .LBB0_23
	ldr	d0, [x10, #16]
.LBB0_23:
	ubfx	w11, w8, #15, #16
	and	w10, w8, #0x7fff
	ucvtf	d1, w10
	ldr	w10, [x2, #140]
	subs	w12, w11, #1
	scvtf	d3, w12
	cmp	w8, #0
	fdiv	d3, d1, d3
	scvtf	d1, w10
	fmul	d1, d0, d1
	fmul	d4, d1, d10
	fsub	d5, d8, d4
	fmul	d6, d3, d4
	fmul	d3, d3, d5
	fadd	d5, d6, d8
	fadd	d3, d4, d3
	fcsel	d3, d3, d5, lt
	cmp	w11, #1
	fcsel	d3, d8, d3, eq
	tbnz	w8, #31, .LBB0_28
	mov	w8, wzr
	fdiv	d1, d1, d11
	tbnz	x9, #63, .LBB0_29
.LBB0_25:
	ubfx	x9, x9, #31, #31
	cmp	w10, w25
	sub	w9, w28, w9
	b.ne	.LBB0_14
	fcmp	d0, d9
	b.ne	.LBB0_14
	frintx	d0, d3
	ldr	x11, [sp, #64]
	add	x1, sp, #80
	and	w4, w8, #0x1
	mov	x0, x21
	mov	x5, x23
	mov	x6, x22
	str	x24, [sp, #80]
	stp	w27, w19, [sp, #88]
	fcvtzs	w10, d0
	fmov	s0, s2
	madd	w3, w9, w11, w10
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB0_15
.LBB0_28:
	ldrb	w8, [x2, #148]
	fdiv	d1, d1, d11
	tbz	x9, #63, .LBB0_25
.LBB0_29:
	ubfx	x9, x9, #31, #28
	cmp	w10, w25
	b.ne	.LBB0_32
	fcmp	d0, d9
	b.ne	.LBB0_32
	ldr	x11, [sp, #48]
	ldrsw	x10, [x2, #144]
	frintx	d0, d3
	add	x1, sp, #80
	and	w4, w8, #0x1
	mov	x0, x21
	add	x9, x9, x11
	mov	x5, x23
	mov	x6, x22
	udiv	x11, x9, x10
	str	x24, [sp, #80]
	fcvtzs	w12, d0
	fmov	s0, s2
	stp	w27, w19, [sp, #88]
	msub	w9, w11, w10, w9
	add	w3, w9, w12
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB0_15
.LBB0_32:
	fmul	d0, d1, d13
	ucvtf	d4, w9
	add	x1, sp, #80
	and	w3, w8, #0x1
	mov	x0, x21
	mov	x4, x23
	mov	x5, x22
	str	x24, [sp, #80]
	stp	w27, w19, [sp, #88]
	fadd	d0, d0, d4
	fadd	d0, d0, d3
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB0_15
.LBB0_33:
	ldr	w8, [x24, #36]
	ldr	w9, [sp, #44]
	cmp	w27, w8
	str	w9, [x24, #56]
	b.ls	.LBB0_5
	cmp	w27, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w27, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB0_5
	add	x0, x24, #24
	add	x1, x24, #40
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB0_5
.LBB0_36:
	.seh_startepilogue
	ldp	d12, d13, [sp, #224]
	.seh_save_fregp	d12, 224
	ldp	d10, d11, [sp, #208]
	.seh_save_fregp	d10, 208
	ldp	d8, d9, [sp, #192]
	.seh_save_fregp	d8, 192
	ldp	x29, x30, [sp, #176]
	.seh_save_fplr	176
	ldp	x27, x28, [sp, #160]
	.seh_save_regp	x27, 160
	ldp	x25, x26, [sp, #144]
	.seh_save_regp	x25, 144
	ldp	x23, x24, [sp, #128]
	.seh_save_regp	x23, 128
	ldp	x21, x22, [sp, #112]
	.seh_save_regp	x21, 112
	ldp	x19, x20, [sp, #96]
	.seh_save_regp	x19, 96
	add	sp, sp, #240
	.seh_stackalloc	240
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize
	.p2align	2
burst.initialize:
.seh_proc burst.initialize
	sub	sp, sp, #48
	.seh_stackalloc	48
	stp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	stp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	x29, sp, #32
	.seh_add_fp	32
	.seh_endprologue
	mov	x20, x0
	adrp	x0, burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	add	x0, x0, :lo12:burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	blr	x20
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	adrp	x0, burst.string.3beee8481600d4c1ec3133f8ae40837f
	add	x0, x0, :lo12:burst.string.3beee8481600d4c1ec3133f8ae40837f
	blr	x20
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"]
	adrp	x0, burst.string.57a01b00185c83d1eb43aeca89f29647
	add	x0, x0, :lo12:burst.string.57a01b00185c83d1eb43aeca89f29647
	blr	x20
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	mov	x19, x0
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	adrp	x0, burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	add	x0, x0, :lo12:burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	blr	x20
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	adrp	x0, burst.string.ffbfa8f9468181ee01437772260f8c26
	add	x0, x0, :lo12:burst.string.ffbfa8f9468181ee01437772260f8c26
	blr	x20
	mov	x8, #38057
	mov	x9, #27479
	mov	w1, #12
	movk	x8, #29112, lsl #16
	movk	x9, #36423, lsl #16
	mov	w2, #16
	movk	x8, #21032, lsl #32
	movk	x9, #44503, lsl #32
	movk	x8, #9978, lsl #48
	movk	x9, #55557, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #22452
	mov	x9, #43084
	mov	w1, #8192
	movk	x8, #54788, lsl #16
	movk	x9, #10747, lsl #16
	mov	w2, #16
	movk	x8, #60930, lsl #32
	movk	x9, #4605, lsl #32
	movk	x8, #3152, lsl #48
	movk	x9, #62383, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #28797
	mov	x9, #36739
	mov	w1, #524288
	movk	x8, #55748, lsl #16
	movk	x9, #9787, lsl #16
	mov	w2, #16
	movk	x8, #13428, lsl #32
	movk	x9, #52107, lsl #32
	movk	x8, #4611, lsl #48
	movk	x9, #60924, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #2053
	mov	x9, #63483
	mov	w1, #65536
	movk	x8, #63346, lsl #16
	movk	x9, #2189, lsl #16
	mov	w2, #16
	movk	x8, #39377, lsl #32
	movk	x9, #26158, lsl #32
	movk	x8, #463, lsl #48
	movk	x9, #65072, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	ldp	x19, x20, [sp, #16]
	.seh_save_regp	x19, 16
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d
	.p2align	2
burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d:
.seh_proc burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d
	str	x19, [sp, #-32]!
	.seh_save_reg_x	x19, 32
	stp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	add	x29, sp, #8
	.seh_add_fp	8
	.seh_endprologue
	mov	x19, x0
	adrp	x0, burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	add	x0, x0, :lo12:burst.string.bc77e56f72475bcb1ba0b4d16fff9cb4
	blr	x19
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::GetWorkStealingRange_Ptr"]
	adrp	x0, burst.string.3beee8481600d4c1ec3133f8ae40837f
	add	x0, x0, :lo12:burst.string.3beee8481600d4c1ec3133f8ae40837f
	blr	x19
	adrp	x8, "Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"
	str	x0, [x8, :lo12:"Unity.Jobs.LowLevel.Unsafe.JobsUtility::get_ThreadIndex_Ptr"]
	adrp	x0, burst.string.57a01b00185c83d1eb43aeca89f29647
	add	x0, x0, :lo12:burst.string.57a01b00185c83d1eb43aeca89f29647
	blr	x19
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	str	x0, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	adrp	x0, burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	add	x0, x0, :lo12:burst.string.22ec91e5bc43e7e512fc53c9ce97a8e0
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	adrp	x0, burst.string.ffbfa8f9468181ee01437772260f8c26
	add	x0, x0, :lo12:burst.string.ffbfa8f9468181ee01437772260f8c26
	blr	x19
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	str	x0, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	ldr	x19, [sp], #32
	.seh_save_reg_x	x19, 32
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d
	.p2align	2
burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d:
.seh_proc burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d
	sub	sp, sp, #48
	.seh_stackalloc	48
	str	x19, [sp, #16]
	.seh_save_reg	x19, 16
	stp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	add	x29, sp, #24
	.seh_add_fp	24
	.seh_endprologue
	mov	x8, #38057
	mov	x9, #27479
	mov	x0, sp
	movk	x8, #29112, lsl #16
	movk	x9, #36423, lsl #16
	mov	w1, #12
	movk	x8, #21032, lsl #32
	movk	x9, #44503, lsl #32
	mov	w2, #16
	movk	x8, #9978, lsl #48
	movk	x9, #55557, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"
	ldr	x19, [x8, :lo12:"Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory_Ptr"]
	str	q0, [sp]
	blr	x19
	mov	x8, #22452
	mov	x9, #43084
	mov	w1, #8192
	movk	x8, #54788, lsl #16
	movk	x9, #10747, lsl #16
	mov	w2, #16
	movk	x8, #60930, lsl #32
	movk	x9, #4605, lsl #32
	movk	x8, #3152, lsl #48
	movk	x9, #62383, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #28797
	mov	x9, #36739
	mov	w1, #524288
	movk	x8, #55748, lsl #16
	movk	x9, #9787, lsl #16
	mov	w2, #16
	movk	x8, #13428, lsl #32
	movk	x9, #52107, lsl #32
	movk	x8, #4611, lsl #48
	movk	x9, #60924, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/StateArray> Latios.Unsafe.ThreadStackAllocator::s_states"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	mov	x8, #2053
	mov	x9, #63483
	mov	w1, #65536
	movk	x8, #63346, lsl #16
	movk	x9, #2189, lsl #16
	mov	w2, #16
	movk	x8, #39377, lsl #32
	movk	x9, #26158, lsl #32
	movk	x8, #463, lsl #48
	movk	x9, #65072, lsl #48
	index	z0.d, x9, x8
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	mov	x0, sp
	str	q0, [sp]
	blr	x19
	adrp	x8, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	str	x0, [x8, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	.seh_startepilogue
	ldp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	ldr	x19, [sp, #16]
	.seh_save_reg	x19, 16
	add	sp, sp, #48
	.seh_stackalloc	48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	str	x19, [sp, #-32]!
	.seh_save_reg_x	x19, 32
	stp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	add	x29, sp, #8
	.seh_add_fp	8
	.seh_endprologue
	cmp	w2, #0
	b.le	.LBB4_9
	lsr	x8, x3, #32
	subs	w9, w2, w4
	b.le	.LBB4_10
	add	w12, w4, w4, lsr #31
	ldr	w13, [x0, #8]
	mov	w10, wzr
	adrp	x11, "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	add	x11, x11, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	mov	w14, #-32768
	asr	w12, w12, #1
	mov	w15, #32767
	adrp	x16, "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	add	x16, x16, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	mov	w17, #88
	b	.LBB4_5
.LBB4_3:
	mov	w4, wzr
.LBB4_4:
	ldr	w5, [x11, w8, sxtw #2]
	lsl	w6, w4, #29
	sbfx	w7, w4, #1, #1
	sbfx	w19, w4, #0, #1
	tst	w4, #0xf8
	and	x4, x4, #0xff
	and	w6, w5, w6, asr #31
	and	w7, w7, w5, asr #1
	ldr	w4, [x16, x4, lsl #2]
	and	w19, w19, w5, asr #2
	eor	w10, w10, #0x1
	add	w5, w6, w5, asr #3
	add	w8, w4, w8
	add	w6, w7, w19
	bic	w8, w8, w8, asr #31
	add	w5, w5, w6
	cneg	w5, w5, ne
	add	w3, w5, w3
	cmn	w3, #8, lsl #12
	csel	w3, w3, w14, gt
	cmp	w3, w15
	csel	w3, w3, w15, lt
	cmp	w8, #88
	csel	w8, w8, w17, lt
	subs	w9, w9, #1
	b.eq	.LBB4_10
.LBB4_5:
	cmp	w12, w13
	b.ge	.LBB4_3
	ldr	x4, [x0]
	ldrb	w4, [x4, w12, sxtw]
	tbz	w10, #0, .LBB4_8
	and	w4, w4, #0xf
	add	w12, w12, #1
	b	.LBB4_4
.LBB4_8:
	lsr	w4, w4, #4
	b	.LBB4_4
.LBB4_9:
	mov	w8, wzr
	mov	w3, wzr
.LBB4_10:
	ldr	w9, [x1, #8]
	cmp	w9, #1
	b.lt	.LBB4_18
	add	w13, w2, w2, lsr #31
	mov	w9, wzr
	mov	x10, xzr
	adrp	x11, "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	add	x11, x11, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	and	w12, w2, #0x1
	fmov	s0, #1.00000000
	fmov	s1, #-1.00000000
	asr	w13, w13, #1
	mov	w14, #-32768
	mov	w15, #32767
	adrp	x16, "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	add	x16, x16, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	mov	w17, #88
	b	.LBB4_14
.LBB4_12:
	mov	w2, wzr
.LBB4_13:
	ldr	w4, [x11, w8, sxtw #2]
	lsl	w5, w2, #29
	sbfx	w6, w2, #1, #1
	sbfx	w7, w2, #0, #1
	tst	w2, #0xf8
	and	x2, x2, #0xff
	and	w5, w4, w5, asr #31
	and	w6, w6, w4, asr #1
	ldr	w2, [x16, x2, lsl #2]
	and	w7, w7, w4, asr #2
	add	x10, x10, #1
	eor	w12, w12, #0x1
	add	w4, w5, w4, asr #3
	add	w8, w2, w8
	add	w5, w6, w7
	bic	w8, w8, w8, asr #31
	add	w4, w4, w5
	cneg	w4, w4, ne
	add	w3, w4, w3
	ldr	x4, [x1]
	cmn	w3, #8, lsl #12
	csel	w3, w3, w14, gt
	cmp	w3, w15
	csel	w3, w3, w15, lt
	cmp	w8, #88
	scvtf	s2, w3, #15
	csel	w8, w8, w17, lt
	fminnm	s2, s2, s0
	fmaxnm	s2, s2, s1
	str	s2, [x4, w9, sxtw]
	add	w9, w9, #4
	ldrsw	x2, [x1, #8]
	cmp	x10, x2
	b.ge	.LBB4_18
.LBB4_14:
	ldr	w2, [x0, #8]
	cmp	w13, w2
	b.ge	.LBB4_12
	ldr	x2, [x0]
	ldrb	w2, [x2, w13, sxtw]
	tbz	w12, #0, .LBB4_17
	and	w2, w2, #0xf
	add	w13, w13, #1
	b	.LBB4_13
.LBB4_17:
	lsr	w2, w2, #4
	b	.LBB4_13
.LBB4_18:
	.seh_startepilogue
	ldp	x29, x30, [sp, #8]
	.seh_save_fplr	8
	ldr	x19, [sp], #32
	.seh_save_reg_x	x19, 32
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #128
	.seh_stackalloc	128
	stp	x19, x20, [sp, #64]
	.seh_save_regp	x19, 64
	stp	x21, x22, [sp, #80]
	.seh_save_regp	x21, 80
	stp	x23, x24, [sp, #96]
	.seh_save_regp	x23, 96
	stp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	add	x29, sp, #112
	.seh_add_fp	112
	.seh_endprologue
	mov	x20, x0
	ldr	x0, [x0]
	mov	w19, w1
	cbz	x0, .LBB5_2
	sxtw	x3, w19
	mov	w1, #4
	mov	w2, #4
	bl	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB5_14
.LBB5_2:
	mov	x21, x20
	ldr	x8, [x21, #24]!
	cbnz	x8, .LBB5_4
	movi	v0.2d, #0000000000000000
	ldr	w8, [x20, #16]
	add	x0, sp, #8
	mov	w2, #8
	stp	w8, wzr, [sp, #24]
	add	x8, sp, #8
	add	x1, x8, #16
	stur	q0, [sp, #8]
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [sp, #8]
	ldr	d0, [sp, #16]
	str	x8, [x20, #24]
	ldp	w8, w9, [sp, #24]
	str	d0, [x20, #32]
	stp	w8, w9, [x20, #40]
.LBB5_4:
	movi	v0.2d, #0000000000000000
	ldrh	w8, [x20, #16]
	ldrh	w10, [x20, #18]
	mov	w9, #4
	mov	w11, #6
	cmp	x8, #64
	stp	q0, q0, [sp, #32]
	str	w19, [sp, #40]
	str	w9, [sp, #48]
	strb	w11, [sp, #56]
	strh	w8, [sp, #44]
	strh	w10, [sp, #46]
	b.lo	.LBB5_12
	cbnz	w10, .LBB5_7
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	orr	x8, x8, x10, lsl #16
	strh	w9, [sp, #46]
.LBB5_7:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	add	x1, sp, #32
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	ldr	x0, [sp, #32]
	ldp	w22, w8, [x20, #32]
	sxtw	x22, w22
	cmp	w22, w8
	b.lt	.LBB5_13
.LBB5_8:
	add	w23, w22, #1
	cmp	w23, w8
	b.le	.LBB5_11
	cmp	w23, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w23, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB5_11
	add	x1, x20, #40
	mov	x24, x0
	mov	x0, x21
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	mov	x0, x24
.LBB5_11:
	mov	w8, #24
	movi	v0.2s, #4
	ldr	x9, [x20, #24]
	smaddl	x8, w22, w8, x9
	str	w23, [x20, #32]
	str	x0, [x8]
	str	w19, [x8, #8]
	stur	d0, [x8, #12]
	str	wzr, [x8, #20]
	b	.LBB5_14
.LBB5_12:
	ldr	w8, [sp, #44]
	mov	w10, w19
	adrp	x11, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	sbfiz	x0, x10, #2, #32
	mov	w1, #64
	mov	w3, #1
	and	w12, w8, #0xffff
	tst	w8, #0xffc0
	ldr	x8, [x11, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	csel	w2, w12, w9, eq
	mov	x4, xzr
	blr	x8
	ldp	w22, w8, [x20, #32]
	sxtw	x22, w22
	cmp	w22, w8
	b.ge	.LBB5_8
.LBB5_13:
	mov	w8, #24
	movi	v0.2s, #4
	ldr	x9, [x20, #24]
	smaddl	x8, w22, w8, x9
	str	x0, [x8]
	str	w19, [x8, #8]
	stur	d0, [x8, #12]
	str	wzr, [x8, #20]
	ldr	w8, [x20, #32]
	add	w8, w8, #1
	str	w8, [x20, #32]
.LBB5_14:
	mov	w1, w19
	.seh_startepilogue
	ldp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	ldp	x23, x24, [sp, #96]
	.seh_save_regp	x23, 96
	ldp	x21, x22, [sp, #80]
	.seh_save_regp	x21, 80
	ldp	x19, x20, [sp, #64]
	.seh_save_regp	x19, 64
	add	sp, sp, #128
	.seh_stackalloc	128
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #80
	.seh_stackalloc	80
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	str	x21, [sp, #48]
	.seh_save_reg	x21, 48
	stp	x29, x30, [sp, #56]
	.seh_save_fplr	56
	add	x29, sp, #56
	.seh_add_fp	56
	.seh_endprologue
	mov	w19, w3
	mov	w20, w2
	ands	w8, w0, #0xff
	b.eq	.LBB6_6
	cmp	w8, #1
	b.eq	.LBB6_4
	cmp	w8, #2
	b.ne	.LBB6_7
	ldrsw	x8, [x1]
	mov	w2, w4
	mov	w3, w20
	mov	x4, x5
	add	x0, x1, x8
	mov	w1, w19
	bl	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	mov	w1, w1
	b	.LBB6_10
.LBB6_4:
	ldrsw	x8, [x1]
	mov	x0, x5
	add	x21, x1, x8
	mov	w1, w4
	bl	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	add	w8, w19, #1023
	cmp	w19, #0
	str	x0, [sp, #8]
	csel	w8, w8, w19, lt
	str	w1, [sp, #16]
	asr	w8, w8, #10
	sbfiz	x9, x8, #2, #32
	tbz	w20, #0, .LBB6_8
	mov	x11, x21
	ldrsw	x12, [x11, #24]!
	sub	x10, x11, #16
	add	x12, x11, x12
	mov	w11, #12
	b	.LBB6_9
.LBB6_6:
	ldrsw	x8, [x1]
	cmp	w20, #0
	lsl	w10, w19, #2
	add	x8, x1, x8
	mov	w1, w4
	add	x9, x8, #8
	csel	x8, x9, x8, ne
	ldrsw	x9, [x8]
	add	x8, x8, x9
	add	x0, x8, w10, sxtw
	b	.LBB6_10
.LBB6_7:
	mov	x0, xzr
	mov	x1, xzr
	b	.LBB6_10
.LBB6_8:
	mov	x10, x21
	ldrsw	x11, [x10, #16]!
	add	x12, x10, x11
	mov	w11, #4
	mov	x10, x21
.LBB6_9:
	add	x9, x12, x9
	ldrsw	x12, [x10]
	ldr	w11, [x21, x11]
	ldrsh	x3, [x9]
	ldrsh	x9, [x9, #2]
	lsl	w4, w8, #10
	add	x10, x10, x12
	add	x0, sp, #20
	add	x1, sp, #8
	bfi	x3, x9, #32, #32
	mov	w2, w19
	stur	x10, [sp, #20]
	str	w11, [sp, #28]
	bl	"Latios.Myri.ADPCMCodec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(System.ReadOnlySpan`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 input, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, Latios.Myri.ADPCMCodec+IMAState, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null seekIMAState, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 seekIMAStateIndex) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x0, [sp, #8]
	ldr	w1, [sp, #16]
.LBB6_10:
	.seh_startepilogue
	ldp	x29, x30, [sp, #56]
	.seh_save_fplr	56
	ldr	x21, [sp, #48]
	.seh_save_reg	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #80
	.seh_stackalloc	80
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-48]!
	.seh_save_regp_x	x19, 48
	str	x21, [sp, #16]
	.seh_save_reg	x21, 16
	stp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	add	x29, sp, #24
	.seh_add_fp	24
	.seh_endprologue
	mov	w19, w1
	mov	x20, x0
	mov	x0, x4
	mov	w1, w2
	mov	w21, w3
	bl	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	mov	w8, #26215
	movk	w8, #26214, lsl #16
	smull	x8, w19, w8
	asr	x8, x8, #39
	tbz	w21, #0, .LBB7_9
	add	w9, w8, w8, lsr #31
	add	w11, w19, w1
	add	w8, w9, w9, lsl #2
	lsl	w8, w8, #6
	cmp	w8, w11
	b.ge	.LBB7_17
	mov	x14, x20
	ldrsw	x10, [x20, #24]!
	sbfiz	x9, x9, #4, #32
	mov	w12, w19
	sxtw	x11, w11
	add	x10, x20, x10
	ldrsw	x15, [x14, #8]!
	add	x10, x10, x9
	adrp	x13, "System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"
	add	x13, x13, :lo12:"System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"
	ldp	d1, d0, [x10]
	sxtw	x10, w12
	neg	w12, w19, lsl #2
	add	x14, x14, x15
	mov	w15, #-32768
	mov	w16, #32767
	sshll	v0.4s, v0.4h, #0
	sshll	v1.4s, v1.4h, #0
	b	.LBB7_4
.LBB7_3:
	cmp	w11, w8
	add	w9, w9, #1
	b.le	.LBB7_17
.LBB7_4:
	lsl	w2, w9, #3
	mov	w3, w8
	mov	w17, wzr
	sxtw	x5, w3
	add	w3, w12, w8, lsl #2
	ldr	x2, [x14, w2, sxtw]
	mov	x8, x5
	lsr	x4, x2, #60
	add	x4, x13, x4, lsl #5
.LBB7_5:
	smull	v2.2d, v0.2s, v1.2s
	ubfx	x5, x2, #57, #3
	smlal2	v2.2d, v0.4s, v1.4s
	addp	d2, v2.2d
	fmov	x6, d2
	lsr	x7, x6, #13
	ldr	w6, [x4, x5, lsl #2]
	add	w5, w6, w7
	cmn	w5, #8, lsl #12
	csel	w5, w5, w15, gt
	cmp	w5, w16
	csel	w5, w5, w16, lt
	cmp	x8, x10
	b.lt	.LBB7_7
	scvtf	s2, w5, #15
	str	s2, [x0, w3, sxtw]
.LBB7_7:
	asr	w6, w6, #4
	cmlt	v4.4s, v1.4s, #0
	ext	v1.16b, v1.16b, v0.16b, #4
	cmp	w17, #18
	add	x8, x8, #1
	neg	w7, w6
	dup	v2.4s, w6
	dup	v3.4s, w7
	mov	v1.s[3], w5
	bit	v2.16b, v3.16b, v4.16b
	add	v0.4s, v2.4s, v0.4s
	b.hi	.LBB7_3
	lsl	x2, x2, #3
	cmp	x8, x11
	add	w17, w17, #1
	add	w3, w3, #4
	b.lt	.LBB7_5
	b	.LBB7_3
.LBB7_9:
	add	w10, w8, w8, lsr #31
	add	w12, w19, w1
	add	w8, w10, w10, lsl #2
	lsl	w8, w8, #6
	cmp	w8, w12
	b.ge	.LBB7_17
	ldrsw	x9, [x20]
	sbfiz	x10, x10, #4, #32
	sxtw	x12, w12
	neg	w13, w19, lsl #2
	adrp	x14, "System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"
	add	x14, x14, :lo12:"System.Int32[] Latios.Myri.QOACodec::DequantTable_$__data_6c66103fb991a550"
	add	x9, x20, x9
	ldrsw	x11, [x20, #16]!
	mov	w15, #-32768
	mov	w16, #32767
	add	x11, x20, x11
	add	x11, x11, x10
	ldp	d1, d0, [x11]
	sxtw	x11, w19
	sshll	v0.4s, v0.4h, #0
	sshll	v1.4s, v1.4h, #0
	b	.LBB7_12
.LBB7_11:
	cmp	w12, w8
	add	w10, w10, #1
	b.le	.LBB7_17
.LBB7_12:
	lsl	w2, w10, #3
	mov	w3, w8
	mov	w17, wzr
	sxtw	x5, w3
	add	w3, w13, w8, lsl #2
	ldr	x2, [x9, w2, sxtw]
	mov	x8, x5
	lsr	x4, x2, #60
	add	x4, x14, x4, lsl #5
.LBB7_13:
	smull	v2.2d, v0.2s, v1.2s
	ubfx	x5, x2, #57, #3
	smlal2	v2.2d, v0.4s, v1.4s
	addp	d2, v2.2d
	fmov	x6, d2
	lsr	x7, x6, #13
	ldr	w6, [x4, x5, lsl #2]
	add	w5, w6, w7
	cmn	w5, #8, lsl #12
	csel	w5, w5, w15, gt
	cmp	w5, w16
	csel	w5, w5, w16, lt
	cmp	x8, x11
	b.lt	.LBB7_15
	scvtf	s2, w5, #15
	str	s2, [x0, w3, sxtw]
.LBB7_15:
	asr	w6, w6, #4
	cmlt	v4.4s, v1.4s, #0
	ext	v1.16b, v1.16b, v0.16b, #4
	cmp	w17, #18
	add	x8, x8, #1
	neg	w7, w6
	dup	v2.4s, w6
	dup	v3.4s, w7
	mov	v1.s[3], w5
	bit	v2.16b, v3.16b, v4.16b
	add	v0.4s, v2.4s, v0.4s
	b.hi	.LBB7_11
	lsl	x2, x2, #3
	cmp	x8, x12
	add	w17, w17, #1
	add	w3, w3, #4
	b.lt	.LBB7_13
	b	.LBB7_11
.LBB7_17:
	mov	w1, w1
	.seh_startepilogue
	ldp	x29, x30, [sp, #24]
	.seh_save_fplr	24
	ldr	x21, [sp, #16]
	.seh_save_reg	x21, 16
	ldp	x19, x20, [sp], #48
	.seh_save_regp_x	x19, 48
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #176
	.seh_stackalloc	176
	stp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	stp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	stp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	stp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	str	x27, [sp, #144]
	.seh_save_reg	x27, 144
	stp	x29, x30, [sp, #152]
	.seh_save_fplr	152
	str	d8, [sp, #168]
	.seh_save_freg	d8, 168
	add	x29, sp, #152
	.seh_add_fp	152
	.seh_endprologue
	movi	v1.2d, #0000000000000000
	ldr	x25, [x1]
	ldr	w8, [x2, #144]
	mov	x19, x6
	str	q1, [sp, #64]
	tbz	w3, #31, .LBB8_2
.LBB8_1:
	adds	w3, w3, w8
	b.mi	.LBB8_1
.LBB8_2:
	sdiv	w9, w3, w8
	ldp	w27, w26, [x1, #8]
	msub	w3, w9, w8, w3
	add	w9, w3, w19
	cmp	w9, w8
	b.ge	.LBB8_4
	mov	w6, w19
	add	x1, sp, #64
	str	x25, [sp, #64]
	stp	w27, w26, [sp, #72]
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB8_25
.LBB8_4:
	cmp	w8, w19, lsl #1
	b.le	.LBB8_6
	sub	w20, w8, w3
	add	x1, sp, #64
	str	x25, [sp, #64]
	mov	x6, x20
	stp	w27, w26, [sp, #72]
	mov	x21, x0
	mov	x22, x2
	mov	w23, w4
	fmov	s8, s0
	mov	x24, x5
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	lsl	w8, w20, #2
	fmov	s0, s8
	sub	w6, w19, w20
	add	x1, sp, #64
	mov	x0, x21
	mov	x2, x22
	add	x5, x24, w8, sxtw
	mov	w3, wzr
	mov	w4, w23
	str	x25, [sp, #64]
	stp	w27, w26, [sp, #72]
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	b	.LBB8_25
.LBB8_6:
	ldr	x8, [x0, #176]
	ldr	w9, [x25, #56]
	movi	v1.2d, #0000000000000000
	ldr	x10, [x25, #48]
	ldr	w11, [x8, #20]
	add	w8, w9, #1
	ldr	w9, [x25, #32]
	str	w8, [x25, #56]
	stp	w9, w8, [sp, #16]
	ldrb	w9, [x2, #149]
	ldr	w8, [x2, #144]
	str	x10, [sp, #8]
	cmp	w9, #2
	stur	q1, [sp, #24]
	stur	q1, [sp, #40]
	stp	w11, wzr, [sp, #56]
	b.eq	.LBB8_17
	cmp	w9, #1
	b.eq	.LBB8_10
	cbnz	w9, .LBB8_18
	ldrsw	x8, [x2]
	cmp	w4, #0
	add	x8, x2, x8
	add	x9, x8, #8
	csel	x8, x9, x8, ne
	ldrsw	x9, [x8]
	add	x0, x8, x9
	cmp	w19, #1
	b.ge	.LBB8_19
	b	.LBB8_21
.LBB8_10:
	ldrsw	x9, [x2]
	add	x0, sp, #8
	mov	w1, w8
	mov	x20, x2
	fmov	s8, s0
	mov	x23, x5
	add	x21, x2, x9
	mov	w22, w3
	mov	w24, w4
	bl	"Latios.Myri.CodecContext, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.AllocateSpan<System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>(Latios.Myri.CodecContext*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 length) -> System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	fmov	s0, s8
	add	x8, x21, #8
	cmp	w24, #0
	mov	w9, #4
	mov	w10, #12
	mov	w3, w22
	mov	x5, x23
	csel	x14, x8, x21, ne
	csel	x12, x10, x9, ne
	cmp	w1, #1
	b.lt	.LBB8_16
	ldrsw	x15, [x14]
	ldr	w12, [x21, x12]
	and	x16, x1, #0x7fffffff
	tst	x1, #0x7ffffffe
	mov	w8, wzr
	mov	x9, xzr
	mov	w10, wzr
	mov	w11, wzr
	mov	w13, wzr
	add	x14, x14, x15
	adrp	x15, "System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	add	x15, x15, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::StepTable_$__data_6efd973d159d16a5"
	fmov	s1, #1.00000000
	fmov	s2, #-1.00000000
	csinc	x16, x16, xzr, ne
	mov	w17, #-32768
	mov	w1, #32767
	adrp	x2, "System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	add	x2, x2, :lo12:"System.Int32[] Latios.Myri.ADPCMCodec::IndexTable_$__data_4638b7c0e713745d"
	mov	w4, #88
	b	.LBB8_14
.LBB8_12:
	ldrb	w6, [x14, w10, sxtw]
	tst	w9, #0x1
	cinc	w10, w10, ne
	lsr	w7, w6, #4
	and	w6, w6, #0xf
	csel	w6, w7, w6, eq
.LBB8_13:
	ldr	w7, [x15, w13, uxtw #2]
	lsl	w21, w6, #29
	sbfx	w22, w6, #1, #1
	sbfx	w23, w6, #0, #1
	tst	w6, #0xf8
	and	x6, x6, #0xff
	and	w21, w7, w21, asr #31
	and	w22, w22, w7, asr #1
	ldr	w6, [x2, x6, lsl #2]
	and	w23, w23, w7, asr #2
	add	x9, x9, #1
	add	w7, w21, w7, asr #3
	add	w13, w6, w13
	add	w21, w22, w23
	bic	w13, w13, w13, asr #31
	add	w7, w7, w21
	cneg	w7, w7, ne
	add	w11, w7, w11
	cmn	w11, #8, lsl #12
	csel	w11, w11, w17, gt
	cmp	w11, w1
	csel	w11, w11, w1, lt
	cmp	w13, #88
	scvtf	s3, w11, #15
	csel	w13, w13, w4, lt
	cmp	x16, x9
	fminnm	s3, s3, s1
	fmaxnm	s3, s3, s2
	str	s3, [x0, w8, sxtw]
	add	w8, w8, #4
	b.eq	.LBB8_16
.LBB8_14:
	cmp	w10, w12
	b.lt	.LBB8_12
	mov	w6, wzr
	b	.LBB8_13
.LBB8_16:
	mov	x2, x20
	cmp	w19, #1
	b.ge	.LBB8_19
	b	.LBB8_21
.LBB8_17:
	ldrsw	x9, [x2]
	mov	w1, wzr
	mov	x20, x2
	mov	w21, w3
	fmov	s8, s0
	mov	x22, x5
	add	x0, x2, x9
	mov	w9, w4
	add	x4, sp, #8
	mov	w2, w8
	mov	w3, w9
	bl	"Latios.Myri.QOACodecData, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeSingleChannel(Latios.Myri.QOACodecData*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 start, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 count, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	fmov	s0, s8
	mov	w3, w21
	mov	x5, x22
	mov	x2, x20
	cmp	w19, #1
	b.ge	.LBB8_19
	b	.LBB8_21
.LBB8_18:
	mov	x0, xzr
	cmp	w19, #1
	b.lt	.LBB8_21
.LBB8_19:
	mov	w8, wzr
	and	x9, x19, #0x7fffffff
.LBB8_20:
	ldr	w10, [x2, #144]
	subs	x9, x9, #1
	sdiv	w11, w3, w10
	msub	w10, w11, w10, w3
	add	w3, w3, #1
	lsl	w10, w10, #2
	ldr	s1, [x0, w10, sxtw]
	sxtw	x10, w8
	add	w8, w8, #4
	fmul	s1, s0, s1
	ldr	s2, [x5, x10]
	fadd	s1, s2, s1
	str	s1, [x5, x10]
	b.ne	.LBB8_20
.LBB8_21:
	ldr	x19, [sp, #8]
	ldp	w20, w9, [sp, #16]
	ldr	w8, [x19, #36]
	sub	w9, w9, #1
	cmp	w20, w8
	str	w9, [x19, #56]
	b.le	.LBB8_24
	cmp	w20, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w20, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB8_24
	add	x0, x19, #24
	add	x1, x19, #40
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB8_24:
	str	w20, [x19, #32]
.LBB8_25:
	.seh_startepilogue
	ldr	d8, [sp, #168]
	.seh_save_freg	d8, 168
	ldp	x29, x30, [sp, #152]
	.seh_save_fplr	152
	ldr	x27, [sp, #144]
	.seh_save_reg	x27, 144
	ldp	x25, x26, [sp, #128]
	.seh_save_regp	x25, 128
	ldp	x23, x24, [sp, #112]
	.seh_save_regp	x23, 112
	ldp	x21, x22, [sp, #96]
	.seh_save_regp	x21, 96
	ldp	x19, x20, [sp, #80]
	.seh_save_regp	x19, 80
	add	sp, sp, #176
	.seh_stackalloc	176
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-48]!
	.seh_save_regp_x	x19, 48
	str	x21, [sp, #16]
	.seh_save_reg	x21, 16
	str	x28, [sp, #24]
	.seh_save_reg	x28, 24
	stp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	add	x29, sp, #32
	.seh_add_fp	32
	.seh_endprologue
	sub	sp, sp, #64
	addvl	sp, sp, #-1
	ldr	w8, [x2, #144]
	neg	w9, w3
	bic	w21, w9, w9, asr #31
	sub	w8, w8, w3
	bic	w8, w8, w8, asr #31
	cmp	w6, w8
	csel	w8, w6, w8, lt
	sub	w20, w8, w21
	cmp	w20, #1
	b.lt	.LBB9_10
	sub	x8, x29, #32
	ldr	x10, [x0, #176]
	bic	w3, w3, w3, asr #31
	str	z0, [x8, #-1, mul vl]
	ldr	x8, [x1]
	movi	v0.2d, #0000000000000000
	ldr	w10, [x10, #20]
	mov	x19, x5
	add	x5, sp, #8
	ldr	w9, [x8, #56]
	ldr	x11, [x8, #48]
	mov	x1, x2
	add	w9, w9, #1
	str	w9, [x8, #56]
	ldr	w8, [x8, #32]
	ldrb	w0, [x2, #149]
	mov	w2, w4
	mov	w4, w20
	str	x11, [sp, #8]
	stp	w8, w9, [sp, #16]
	stur	q0, [sp, #24]
	stur	q0, [sp, #40]
	stp	w10, wzr, [sp, #56]
	bl	"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	cnth	x11
	mov	w8, #16
	cmp	x11, #16
	csel	x8, x11, x8, hi
	cmp	x8, x20
	lsl	w8, w21, #2
	b.ls	.LBB9_11
	mov	x9, xzr
.LBB9_3:
	sub	x10, x29, #32
	ldr	z5, [x10, #-1, mul vl]
.LBB9_4:
	lsl	w10, w9, #2
	sub	x9, x9, x20
.LBB9_5:
	ldr	s0, [x0, w10, sxtw]
	add	w11, w8, w10
	adds	x9, x9, #1
	sxtw	x11, w11
	add	w10, w10, #4
	fmul	s0, s5, s0
	ldr	s1, [x19, x11]
	fadd	s0, s1, s0
	str	s0, [x19, x11]
	b.lo	.LBB9_5
.LBB9_6:
	ldr	x19, [sp, #8]
	ldp	w20, w9, [sp, #16]
	ldr	w8, [x19, #36]
	sub	w9, w9, #1
	cmp	w20, w8
	str	w9, [x19, #56]
	b.le	.LBB9_9
	cmp	w20, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w20, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB9_9
	add	x0, x19, #24
	add	x1, x19, #40
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB9_9:
	str	w20, [x19, #32]
.LBB9_10:
	.seh_startepilogue
	addvl	sp, sp, #1
	.seh_allocz	1
	add	sp, sp, #64
	.seh_stackalloc	64
	ldp	x29, x30, [sp, #32]
	.seh_save_fplr	32
	ldr	x28, [sp, #24]
	.seh_save_reg	x28, 24
	ldr	x21, [sp, #16]
	.seh_save_reg	x21, 16
	ldp	x19, x20, [sp], #48
	.seh_save_regp_x	x19, 48
	.seh_endepilogue
	ret
.LBB9_11:
	sub	x10, x20, #1
	add	w9, w21, w10
	cmp	w8, w9, lsl #2
	mov	x9, xzr
	b.gt	.LBB9_3
	sub	x12, x29, #32
	lsr	x10, x10, #29
	ldr	z5, [x12, #-1, mul vl]
	cbnz	x10, .LBB9_4
	lsl	x9, x20, #2
	add	x10, x19, w8, sxtw
	add	x12, x0, x9
	cmp	x10, x12
	b.hs	.LBB9_16
	add	x9, x10, x9
	cmp	x9, x0
	b.ls	.LBB9_16
	mov	x9, xzr
	b	.LBB9_4
.LBB9_16:
	udiv	x9, x20, x11
	mov	z0.s, s5
	ptrue	p0.b
	mov	w10, wzr
	mul	x9, x9, x11
	rdvl	x11, #1
	lsr	x11, x11, #4
	lsl	w11, w11, #5
	sub	x12, x20, x9
	mov	x13, x9
.LBB9_17:
	sxtw	x14, w10
	dech	x13
	add	x15, x0, x14
	ld1b	{ z1.b }, p0/z, [x0, x14]
	add	w14, w8, w10
	ldr	z2, [x15, #1, mul vl]
	sxtw	x14, w14
	add	w10, w10, w11
	fmul	z1.s, z0.s, z1.s
	add	x15, x19, x14
	ld1b	{ z3.b }, p0/z, [x19, x14]
	fmul	z2.s, z0.s, z2.s
	ldr	z4, [x15, #1, mul vl]
	fadd	z1.s, z3.s, z1.s
	fadd	z2.s, z4.s, z2.s
	st1b	{ z1.b }, p0, [x19, x14]
	str	z2, [x15, #1, mul vl]
	cbnz	x13, .LBB9_17
	cbnz	x12, .LBB9_4
	b	.LBB9_6
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #160
	.seh_stackalloc	160
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	stp	x23, x24, [sp, #64]
	.seh_save_regp	x23, 64
	stp	x25, x26, [sp, #80]
	.seh_save_regp	x25, 80
	stp	x27, x28, [sp, #96]
	.seh_save_regp	x27, 96
	stp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	stp	d8, d9, [sp, #128]
	.seh_save_fregp	d8, 128
	stp	d10, d11, [sp, #144]
	.seh_save_fregp	d10, 144
	add	x29, sp, #112
	.seh_add_fp	112
	.seh_endprologue
	ldr	w8, [x2, #144]
	fmov	d8, d1
	fmov	s9, s2
	movi	v2.2d, #0000000000000000
	fcmp	d0, #0.0
	ldr	x25, [x1]
	scvtf	d1, w8
	mov	x21, x5
	mov	x19, x4
	mov	x23, x2
	mov	w28, w3
	mov	x22, x0
	str	q2, [sp, #16]
	fmul	d10, d8, d1
	b.pl	.LBB10_2
.LBB10_1:
	fadd	d0, d0, d1
	fcmp	d0, #0.0
	b.mi	.LBB10_1
.LBB10_2:
	ldr	d11, [x1, #8]
	bl	burst.Sleef_fmod_armv9a
	scvtf	d1, w21
	fmov	d2, #1.00000000
	fmul	d1, d8, d1
	fadd	d1, d1, d0
	fadd	d2, d1, d2
	fcmp	d2, d10
	b.mi	.LBB10_10
	fcvtzs	w26, d0
	fcvtps	w8, d1
	ldr	w9, [x25, #56]
	ldr	x20, [x25, #48]
	str	x22, [sp]
	ldr	w24, [x25, #32]
	add	w22, w9, #1
	mov	w1, #4
	mov	w2, #4
	mov	x0, x20
	fmov	d10, d0
	str	w9, [sp, #8]
	sub	w8, w8, w26
	str	w22, [x25, #56]
	add	w27, w8, #2
	sxtw	x3, w27
	bl	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	lsl	w2, w27, #2
	mov	w1, wzr
	mov	w3, wzr
	mov	x25, x0
	bl	burst.memset.inline.ARMV9A.i32
	fmov	s0, s9
	ldr	x0, [sp]
	add	x1, sp, #16
	mov	x2, x23
	mov	w3, w26
	mov	w4, w28
	mov	x5, x25
	mov	x6, x27
	str	x20, [sp, #16]
	str	w24, [sp, #12]
	stp	w24, w22, [sp, #24]
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMatchedRateLooped(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	cmp	w21, #1
	b.lt	.LBB10_6
	scvtf	d0, w26
	mov	w8, wzr
	mov	x9, xzr
	and	x10, x21, #0x7fffffff
	fsub	d0, d10, d0
.LBB10_5:
	ucvtf	d1, w9
	add	x9, x9, #1
	cmp	x10, x9
	fmul	d1, d8, d1
	fadd	d1, d0, d1
	fcvtzs	w11, d1
	frintm	d2, d1
	lsl	w11, w11, #2
	fsub	d1, d1, d2
	add	w12, w11, #4
	ldr	s2, [x25, w11, sxtw]
	sxtw	x11, w8
	ldr	s3, [x25, w12, sxtw]
	add	w8, w8, #4
	fcvt	s1, d1
	fsub	s3, s3, s2
	fmul	s1, s3, s1
	fadd	s1, s2, s1
	ldr	s2, [x19, x11]
	fadd	s1, s2, s1
	str	s1, [x19, x11]
	b.ne	.LBB10_5
.LBB10_6:
	ldp	w9, w19, [sp, #8]
	ldr	w8, [x20, #36]
	cmp	w19, w8
	str	w9, [x20, #56]
	b.ls	.LBB10_9
	cmp	w19, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w19, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB10_9
	add	x0, x20, #24
	add	x1, x20, #40
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB10_9:
	str	w19, [x20, #32]
	b	.LBB10_11
.LBB10_10:
	fmov	d1, d8
	fmov	s2, s9
	mov	w5, w21
	add	x1, sp, #16
	mov	x0, x22
	mov	x2, x23
	mov	w3, w28
	mov	x4, x19
	str	x25, [sp, #16]
	str	d11, [sp, #24]
	bl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB10_11:
	.seh_startepilogue
	ldp	d10, d11, [sp, #144]
	.seh_save_fregp	d10, 144
	ldp	d8, d9, [sp, #128]
	.seh_save_fregp	d8, 128
	ldp	x29, x30, [sp, #112]
	.seh_save_fplr	112
	ldp	x27, x28, [sp, #96]
	.seh_save_regp	x27, 96
	ldp	x25, x26, [sp, #80]
	.seh_save_regp	x25, 80
	ldp	x23, x24, [sp, #64]
	.seh_save_regp	x23, 64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #160
	.seh_stackalloc	160
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Myri.Sampling+SampleJob, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.SampleMismatchedRateOneshot(Latios.Myri.Sampling+SampleJob*, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Latios.Unsafe.ThreadStackAllocator, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null tsa, Latios.Myri.AudioClipBlob&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null clip, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipStart, System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 clipSampleStride, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 isRightChannel, System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 weight, System.Span`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 output) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #144
	.seh_stackalloc	144
	stp	x19, x20, [sp, #64]
	.seh_save_regp	x19, 64
	stp	x21, x22, [sp, #80]
	.seh_save_regp	x21, 80
	stp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	stp	d8, d9, [sp, #112]
	.seh_save_fregp	d8, 112
	str	d10, [sp, #128]
	.seh_save_freg	d10, 128
	add	x29, sp, #96
	.seh_add_fp	96
	.seh_endprologue
	cmp	w5, #0
	mov	w20, wzr
	cset	w8, gt
	fcmp	d0, #0.0
	b.pl	.LBB11_4
	cmp	w5, #1
	b.lt	.LBB11_4
	mov	w20, wzr
.LBB11_3:
	fadd	d0, d1, d0
	add	w20, w20, #1
	cmp	w20, w5
	cset	w8, lt
	fcmp	d0, #0.0
	ccmp	w20, w5, #0, mi
	b.lt	.LBB11_3
.LBB11_4:
	tbz	w8, #0, .LBB11_13
	ldr	w8, [x2, #144]
	scvtf	d3, w8
	fcmp	d0, d3
	b.ge	.LBB11_13
	sub	w9, w8, #1
	sub	w10, w5, w20
	ldr	x11, [x1]
	scvtf	d3, w9
	ldr	x13, [x0, #176]
	fcvtzs	w19, d0
	ldr	w12, [x11, #56]
	ldr	x15, [x11, #48]
	mov	x22, x4
	ldr	w13, [x13, #20]
	add	x5, sp, #8
	mov	x1, x2
	add	w12, w12, #1
	fmov	d8, d0
	fmov	s9, s2
	fsub	d3, d3, d0
	str	w12, [x11, #56]
	ldr	w11, [x11, #32]
	ldrb	w0, [x2, #149]
	mov	w2, w3
	mov	w3, w19
	stp	w11, w12, [sp, #16]
	fmov	d10, d1
	str	x15, [sp, #8]
	fdiv	d3, d3, d1
	stp	w13, wzr, [sp, #56]
	fcvtzs	w9, d3
	cmp	w10, w9
	csel	w9, w10, w9, lt
	sub	w10, w9, #1
	scvtf	d3, w10
	fmul	d3, d1, d3
	fadd	d3, d0, d3
	fcvtps	w14, d3
	movi	v3.2d, #0000000000000000
	cmp	w8, w14
	sub	w11, w14, w19
	stur	q3, [sp, #24]
	cset	w8, le
	stur	q3, [sp, #40]
	csel	w21, w10, w9, le
	sub	w8, w11, w8
	add	w4, w8, #1
	bl	"Latios.Myri.CodecDispatch, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.DecodeChannel(Latios.Myri.Codec, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codec, Unity.Entities.BlobPtr`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]&, Unity.Entities, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null codecStruct, System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 rightChannel, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 startSample, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sampleCount, Latios.Myri.CodecContext&, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null context) -> System.ReadOnlySpan`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	cmp	w21, #1
	b.lt	.LBB11_9
	scvtf	d2, w19
	fmov	d1, d10
	lsl	w9, w20, #2
	fmov	s0, s9
	mov	x8, x22
	mov	x10, xzr
	fsub	d2, d8, d2
.LBB11_8:
	ucvtf	d3, w10
	add	x10, x10, #1
	cmp	x21, x10
	fmul	d3, d1, d3
	fadd	d3, d2, d3
	fcvtzs	w11, d3
	frintm	d4, d3
	lsl	w11, w11, #2
	fsub	d3, d3, d4
	add	w12, w11, #4
	ldr	s4, [x0, w11, sxtw]
	sxtw	x11, w9
	ldr	s5, [x0, w12, sxtw]
	add	w9, w9, #4
	fcvt	s3, d3
	fsub	s5, s5, s4
	fmul	s3, s5, s3
	fadd	s3, s4, s3
	ldr	s4, [x8, x11]
	fmul	s3, s0, s3
	fadd	s3, s4, s3
	str	s3, [x8, x11]
	b.ne	.LBB11_8
.LBB11_9:
	ldr	x19, [sp, #8]
	ldp	w20, w9, [sp, #16]
	ldr	w8, [x19, #36]
	sub	w9, w9, #1
	cmp	w20, w8
	str	w9, [x19, #56]
	b.le	.LBB11_12
	cmp	w20, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w20, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB11_12
	add	x0, x19, #24
	add	x1, x19, #40
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB11_12:
	str	w20, [x19, #32]
.LBB11_13:
	.seh_startepilogue
	ldr	d10, [sp, #128]
	.seh_save_freg	d10, 128
	ldp	d8, d9, [sp, #112]
	.seh_save_fregp	d8, 112
	ldp	x29, x30, [sp, #96]
	.seh_save_fplr	96
	ldp	x21, x22, [sp, #80]
	.seh_save_regp	x21, 80
	ldp	x19, x20, [sp, #64]
	.seh_save_regp	x19, 64
	add	sp, sp, #144
	.seh_stackalloc	144
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Latios.Unsafe.ThreadStackAllocator+State, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.Allocate(Latios.Unsafe.ThreadStackAllocator+State*, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 sizeOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 alignOfElement, System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 numElements) -> System.Byte*, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	stp	x19, x20, [sp, #-80]!
	.seh_save_regp_x	x19, 80
	stp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	stp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	stp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	stp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	add	x29, sp, #64
	.seh_add_fp	64
	.seh_endprologue
	cbz	x3, .LBB12_19
	mul	x22, x3, x1
	mov	x20, x0
	mov	x19, x0
	ldr	x8, [x20, #24]!
	cbz	x8, .LBB12_5
	ldr	w21, [x19, #32]
	cbz	w21, .LBB12_5
	sub	w9, w21, #1
	mov	w10, #24
	neg	x12, x2
	umaddl	x9, w9, w10, x8
	cmp	x2, #0
	ldp	x10, x11, [x9]
	ldr	w23, [x9, #16]
	ldr	x9, [x19]
	add	x11, x11, x10
	add	x10, x9, x23, lsl #4
	add	x9, x2, x11
	sub	x9, x9, #1
	ldr	x13, [x10, #8]
	and	x9, x9, x12
	csel	x24, x11, x9, eq
	sub	x9, x13, x24
	cmp	x22, x9
	b.ls	.LBB12_28
	add	w23, w23, #1
	ldr	w9, [x19, #8]
	cmp	w9, w23
	b.gt	.LBB12_6
	b	.LBB12_13
.LBB12_5:
	mov	w23, wzr
	ldr	w9, [x19, #8]
	cmp	w9, wzr
	b.le	.LBB12_13
.LBB12_6:
	ldr	x9, [x19]
	add	x10, x9, w23, uxtw #4
	ldr	x9, [x10, #8]
	cmp	x9, x22
	b.hs	.LBB12_25
	mov	w24, w23
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	mov	x21, x2
	ldr	x25, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	lsl	x26, x24, #4
.LBB12_8:
	ldr	x8, [x19]
	mov	w1, #4
	ldr	x0, [x8, x26]
	blr	x25
	ldr	w8, [x19, #8]
	add	x24, x24, #1
	add	x26, x26, #16
	cmp	x24, x8
	b.lo	.LBB12_8
	ldr	w8, [x19, #12]
	cmp	w23, w8
	b.ls	.LBB12_12
	cmp	w23, #4
	mov	w9, #4
	mov	w10, #2147483646
	csel	w9, w23, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB12_12
	add	x1, x19, #16
	mov	x0, x19
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
.LBB12_12:
	mov	x2, x21
	str	w23, [x19, #8]
.LBB12_13:
	adrp	x8, "Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"
	adrp	x9, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	mov	w1, w2
	ldr	x8, [x8, :lo12:"Unity.Burst.SharedStatic`1<Latios.Unsafe.ThreadStackAllocator/Settings> Latios.Unsafe.ThreadStackAllocator::s_settings"]
	ldr	x9, [x9, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	w2, #4
	mov	w3, #1
	mov	x4, xzr
	ldrsw	x8, [x8, #8]
	cmp	x22, x8
	csel	x8, x22, x8, hi
	add	x8, x8, #63
	and	x21, x8, #0xffffffffffffffc0
	mov	x0, x21
	blr	x9
	ldp	w24, w8, [x19, #8]
	sxtw	x24, w24
	cmp	w24, w8
	b.ge	.LBB12_20
	ldr	x8, [x19]
	add	x8, x8, x24, lsl #4
	stp	x0, x21, [x8]
	ldr	w8, [x19, #8]
	add	w8, w8, #1
	str	w8, [x19, #8]
	ldp	w21, w8, [x19, #32]
	sxtw	x21, w21
	cmp	w21, w8
	b.lt	.LBB12_24
.LBB12_15:
	add	w24, w21, #1
	cmp	w24, w8
	b.le	.LBB12_18
	cmp	w24, #2
	mov	w9, #2
	mov	w10, #2147483646
	csel	w9, w24, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB12_18
	add	x1, x19, #40
	mov	x25, x0
	mov	x0, x20
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	mov	x0, x25
.LBB12_18:
	mov	w8, #24
	ldr	x9, [x19, #24]
	smaddl	x8, w21, w8, x9
	b	.LBB12_34
.LBB12_19:
	mov	x0, xzr
	b	.LBB12_40
.LBB12_20:
	add	w25, w24, #1
	cmp	w25, w8
	b.le	.LBB12_23
	cmp	w25, #4
	mov	w9, #4
	mov	w10, #2147483646
	csel	w9, w25, w9, hi
	sub	x9, x9, #1
	orr	x9, x9, x9, lsr #1
	orr	x9, x9, x9, lsr #2
	orr	x9, x9, x9, lsr #4
	orr	x9, x9, x9, lsr #8
	orr	x9, x9, x9, lsr #16
	cmp	x9, x10
	csel	x9, x9, x10, lo
	add	w2, w9, #1
	cmp	w8, w2
	b.eq	.LBB12_23
	add	x1, x19, #16
	mov	x26, x0
	mov	x0, x19
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	mov	x0, x26
.LBB12_23:
	ldr	x8, [x19]
	str	w25, [x19, #8]
	add	x8, x8, x24, lsl #4
	stp	x0, x21, [x8]
	ldp	w21, w8, [x19, #32]
	sxtw	x21, w21
	cmp	w21, w8
	b.ge	.LBB12_15
.LBB12_24:
	mov	w8, #24
	ldr	x9, [x19, #24]
	smaddl	x8, w21, w8, x9
	b	.LBB12_27
.LBB12_25:
	ldp	w21, w9, [x19, #32]
	ldr	x0, [x10]
	sxtw	x21, w21
	cmp	w21, w9
	b.ge	.LBB12_30
	mov	w9, #24
	smaddl	x8, w21, w9, x8
.LBB12_27:
	stp	xzr, x22, [x8]
	stp	w23, wzr, [x8, #16]
	ldr	w8, [x19, #32]
	add	w8, w8, #1
	str	w8, [x19, #32]
	b	.LBB12_40
.LBB12_28:
	ldr	w9, [x19, #36]
	ldr	x25, [x10]
	cmp	w21, w9
	b.ge	.LBB12_35
	mov	w9, #24
	umaddl	x8, w21, w9, x8
	stp	x24, x22, [x8]
	stp	w23, wzr, [x8, #16]
	ldr	w8, [x19, #32]
	add	w8, w8, #1
	str	w8, [x19, #32]
	b	.LBB12_39
.LBB12_30:
	add	w24, w21, #1
	cmp	w24, w9
	b.le	.LBB12_33
	cmp	w24, #2
	mov	w10, #2
	mov	w11, #2147483646
	csel	w10, w24, w10, hi
	sub	x10, x10, #1
	orr	x10, x10, x10, lsr #1
	orr	x10, x10, x10, lsr #2
	orr	x10, x10, x10, lsr #4
	orr	x10, x10, x10, lsr #8
	orr	x10, x10, x10, lsr #16
	cmp	x10, x11
	csel	x10, x10, x11, lo
	add	w2, w10, #1
	cmp	w9, w2
	b.eq	.LBB12_33
	add	x1, x19, #40
	mov	x25, x0
	mov	x0, x20
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [x19, #24]
	mov	x0, x25
.LBB12_33:
	mov	w9, #24
	smaddl	x8, w21, w9, x8
.LBB12_34:
	str	w24, [x19, #32]
	stp	xzr, x22, [x8]
	stp	w23, wzr, [x8, #16]
	b	.LBB12_40
.LBB12_35:
	add	w26, w21, #1
	cmp	w26, w9
	b.le	.LBB12_38
	orr	x10, x21, x21, lsr #1
	mov	w11, #2147483646
	orr	x10, x10, x10, lsr #2
	orr	x10, x10, x10, lsr #4
	orr	x10, x10, x10, lsr #8
	orr	x10, x10, x10, lsr #16
	cmp	x10, x11
	csel	x10, x10, x11, lo
	add	w2, w10, #1
	cmp	w9, w2
	b.eq	.LBB12_38
	add	x1, x19, #40
	mov	x0, x20
	bl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	ldr	x8, [x19, #24]
.LBB12_38:
	mov	w9, #24
	str	w26, [x19, #32]
	umaddl	x8, w21, w9, x8
	stp	x24, x22, [x8]
	stp	w23, wzr, [x8, #16]
.LBB12_39:
	add	x0, x25, x24
.LBB12_40:
	.seh_startepilogue
	ldp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	ldp	x25, x26, [sp, #48]
	.seh_save_regp	x25, 48
	ldp	x23, x24, [sp, #32]
	.seh_save_regp	x23, 32
	ldp	x21, x22, [sp, #16]
	.seh_save_regp	x21, 16
	ldp	x19, x20, [sp], #80
	.seh_save_regp_x	x19, 80
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Myri.CodecContext+Allocation, Latios.Myri, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #80
	.seh_stackalloc	80
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	stp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	add	x29, sp, #64
	.seh_add_fp	64
	.seh_endprologue
	bic	w22, w2, w2, asr #31
	mov	x20, x1
	cmp	w2, #0
	mov	x19, x0
	b.le	.LBB13_4
	movi	v0.2d, #0000000000000000
	ldr	w8, [x20]
	mov	w9, #24
	mov	w10, #6
	and	w11, w8, #0xffff
	lsr	w12, w8, #16
	cmp	w11, #64
	stp	q0, q0, [sp]
	str	w22, [sp, #8]
	str	w9, [sp, #16]
	strb	w10, [sp, #24]
	strh	w8, [sp, #12]
	strh	w12, [sp, #14]
	b.lo	.LBB13_5
	cmp	w8, #16, lsl #12
	b.hs	.LBB13_6
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
	b	.LBB13_7
.LBB13_4:
	mov	x21, xzr
	b	.LBB13_10
.LBB13_5:
	ldr	w8, [sp, #12]
	umull	x0, w22, w9
	adrp	x9, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	ldr	x9, [x9, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	w1, #64
	mov	w3, #1
	and	w10, w8, #0xffff
	tst	w8, #0xffc0
	mov	w8, #4
	csel	w2, w10, w8, eq
	mov	x4, xzr
	blr	x9
	mov	x21, x0
	ldr	x1, [x19]
	cbnz	x1, .LBB13_8
	b	.LBB13_17
.LBB13_6:
	and	x8, x8, #0xffff
.LBB13_7:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	ldr	x21, [sp]
	ldr	x1, [x19]
	cbz	x1, .LBB13_17
.LBB13_8:
	ldr	w8, [x19, #12]
	cmp	w8, #1
	b.lt	.LBB13_10
	cmp	w22, w8
	mov	w9, #24
	mov	x0, x21
	csel	w8, w22, w8, lo
	mov	w3, wzr
	umull	x2, w8, w9
	bl	burst.memcpy.inline.ARMV9A.i64
.LBB13_10:
	ldr	x0, [x19]
	cbz	x0, .LBB13_17
	ldr	w8, [x19, #12]
	mov	w9, #3
	str	x0, [sp]
	str	x9, [sp, #24]
	mov	w9, #24
	stp	w9, w8, [sp, #16]
	ldrh	w8, [x20]
	ldrh	w9, [x20, #2]
	str	wzr, [sp, #8]
	cmp	x8, #64
	strh	w8, [sp, #12]
	strh	w9, [sp, #14]
	b.lo	.LBB13_15
	cbnz	w9, .LBB13_14
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
.LBB13_14:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	b	.LBB13_17
.LBB13_15:
	ldrh	w8, [sp, #12]
	mov	w9, #4
	cmp	w8, #64
	csel	w1, w8, w9, lo
	cmp	w1, #1
	b.eq	.LBB13_17
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	ldr	x8, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	blr	x8
.LBB13_17:
	ldr	w8, [x19, #8]
	str	x21, [x19]
	cmp	w8, w22
	csel	w8, w8, w22, lt
	stp	w8, w22, [x19, #8]
	.seh_startepilogue
	ldp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #80
	.seh_stackalloc	80
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Allocation, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #80
	.seh_stackalloc	80
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	stp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	add	x29, sp, #64
	.seh_add_fp	64
	.seh_endprologue
	bic	w22, w2, w2, asr #31
	mov	x20, x1
	cmp	w2, #0
	mov	x19, x0
	b.le	.LBB14_4
	movi	v0.2d, #0000000000000000
	ldr	w8, [x20]
	mov	w9, #24
	mov	w10, #6
	and	w11, w8, #0xffff
	lsr	w12, w8, #16
	cmp	w11, #64
	stp	q0, q0, [sp]
	str	w22, [sp, #8]
	str	w9, [sp, #16]
	strb	w10, [sp, #24]
	strh	w8, [sp, #12]
	strh	w12, [sp, #14]
	b.lo	.LBB14_5
	cmp	w8, #16, lsl #12
	b.hs	.LBB14_6
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
	b	.LBB14_7
.LBB14_4:
	mov	x21, xzr
	b	.LBB14_10
.LBB14_5:
	ldr	w8, [sp, #12]
	umull	x0, w22, w9
	adrp	x9, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	ldr	x9, [x9, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	w1, #64
	mov	w3, #1
	and	w10, w8, #0xffff
	tst	w8, #0xffc0
	mov	w8, #4
	csel	w2, w10, w8, eq
	mov	x4, xzr
	blr	x9
	mov	x21, x0
	ldr	x1, [x19]
	cbnz	x1, .LBB14_8
	b	.LBB14_17
.LBB14_6:
	and	x8, x8, #0xffff
.LBB14_7:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	ldr	x21, [sp]
	ldr	x1, [x19]
	cbz	x1, .LBB14_17
.LBB14_8:
	ldr	w8, [x19, #12]
	cmp	w8, #1
	b.lt	.LBB14_10
	cmp	w22, w8
	mov	w9, #24
	mov	x0, x21
	csel	w8, w22, w8, lo
	mov	w3, wzr
	umull	x2, w8, w9
	bl	burst.memcpy.inline.ARMV9A.i64
.LBB14_10:
	ldr	x0, [x19]
	cbz	x0, .LBB14_17
	ldr	w8, [x19, #12]
	mov	w9, #3
	str	x0, [sp]
	str	x9, [sp, #24]
	mov	w9, #24
	stp	w9, w8, [sp, #16]
	ldrh	w8, [x20]
	ldrh	w9, [x20, #2]
	str	wzr, [sp, #8]
	cmp	x8, #64
	strh	w8, [sp, #12]
	strh	w9, [sp, #14]
	b.lo	.LBB14_15
	cbnz	w9, .LBB14_14
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
.LBB14_14:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	b	.LBB14_17
.LBB14_15:
	ldrh	w8, [sp, #12]
	mov	w9, #4
	cmp	w8, #64
	csel	w1, w8, w9, lo
	cmp	w1, #1
	b.eq	.LBB14_17
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	ldr	x8, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	blr	x8
.LBB14_17:
	ldr	w8, [x19, #8]
	str	x21, [x19]
	cmp	w8, w22
	csel	w8, w8, w22, lt
	stp	w8, w22, [x19, #8]
	.seh_startepilogue
	ldp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #80
	.seh_stackalloc	80
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.globl	"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	.p2align	2
"Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null":
.seh_proc "Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null.ResizeExact<Unity.Collections.AllocatorManager+AllocatorHandle, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null>(Unity.Collections.LowLevel.Unsafe.UnsafeList`1[[Latios.Unsafe.ThreadStackAllocator+State+Block, Latios.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]*, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null this, Unity.Collections.AllocatorManager+AllocatorHandle&, Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null allocator, System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 newCapacity) -> System.Void, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_80aae145bfa60be68c48a85692faa03d from Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
	sub	sp, sp, #80
	.seh_stackalloc	80
	stp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	stp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	stp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	add	x29, sp, #64
	.seh_add_fp	64
	.seh_endprologue
	bic	w22, w2, w2, asr #31
	mov	x20, x1
	cmp	w2, #0
	mov	x19, x0
	b.le	.LBB15_4
	movi	v0.2d, #0000000000000000
	ldr	w8, [x20]
	mov	w9, #16
	mov	w10, #6
	lsr	w11, w8, #16
	stp	q0, q0, [sp]
	str	w9, [sp, #16]
	and	w9, w8, #0xffff
	cmp	w9, #64
	str	w22, [sp, #8]
	strb	w10, [sp, #24]
	strh	w8, [sp, #12]
	strh	w11, [sp, #14]
	b.lo	.LBB15_5
	cmp	w8, #16, lsl #12
	b.hs	.LBB15_6
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
	b	.LBB15_7
.LBB15_4:
	mov	x21, xzr
	b	.LBB15_10
.LBB15_5:
	ldr	w8, [sp, #12]
	adrp	x9, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"
	ubfiz	x0, x22, #4, #32
	ldr	x9, [x9, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MallocTracked_Ptr"]
	mov	w1, #64
	mov	w3, #1
	and	w10, w8, #0xffff
	tst	w8, #0xffc0
	mov	w8, #4
	csel	w2, w10, w8, eq
	mov	x4, xzr
	blr	x9
	mov	x21, x0
	ldr	x1, [x19]
	cbnz	x1, .LBB15_8
	b	.LBB15_17
.LBB15_6:
	and	x8, x8, #0xffff
.LBB15_7:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	ldr	x21, [sp]
	ldr	x1, [x19]
	cbz	x1, .LBB15_17
.LBB15_8:
	ldr	w8, [x19, #12]
	cmp	w8, #1
	b.lt	.LBB15_10
	cmp	w22, w8
	mov	x0, x21
	mov	w3, wzr
	csel	w8, w22, w8, lo
	ubfiz	x2, x8, #4, #32
	bl	burst.memcpy.inline.ARMV9A.i64
.LBB15_10:
	ldr	x0, [x19]
	cbz	x0, .LBB15_17
	ldr	w8, [x19, #12]
	mov	w9, #3
	str	x0, [sp]
	str	x9, [sp, #24]
	mov	w9, #16
	stp	w9, w8, [sp, #16]
	ldrh	w8, [x20]
	ldrh	w9, [x20, #2]
	str	wzr, [sp, #8]
	cmp	x8, #64
	strh	w8, [sp, #12]
	strh	w9, [sp, #14]
	b.lo	.LBB15_15
	cbnz	w9, .LBB15_14
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<System.UInt16>> Unity.Collections.AllocatorManager/SharedStatics/Version::Ref"]
	ldrh	w9, [x9, x8, lsl #1]
	strh	w9, [sp, #14]
.LBB15_14:
	adrp	x9, "Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"
	mov	x1, sp
	ldr	x9, [x9, :lo12:"Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/Array32768`1<Unity.Collections.AllocatorManager/TableEntry>> Unity.Collections.AllocatorManager/SharedStatics/TableEntry::Ref"]
	add	x8, x9, x8, lsl #4
	ldp	x9, x0, [x8]
	blr	x9
	b	.LBB15_17
.LBB15_15:
	ldrh	w8, [sp, #12]
	mov	w9, #4
	cmp	w8, #64
	csel	w1, w8, w9, lo
	cmp	w1, #1
	b.eq	.LBB15_17
	adrp	x8, "Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"
	ldr	x8, [x8, :lo12:"Unity.Collections.LowLevel.Unsafe.UnsafeUtility::FreeTracked_Ptr"]
	blr	x8
.LBB15_17:
	ldr	w8, [x19, #8]
	str	x21, [x19]
	cmp	w8, w22
	csel	w8, w8, w22, lt
	stp	w8, w22, [x19, #8]
	.seh_startepilogue
	ldp	x29, x30, [sp, #64]
	.seh_save_fplr	64
	ldp	x21, x22, [sp, #48]
	.seh_save_regp	x21, 48
	ldp	x19, x20, [sp, #32]
	.seh_save_regp	x19, 32
	add	sp, sp, #80
	.seh_stackalloc	80
	.seh_endepilogue
	ret
	.seh_endfunclet
	.seh_endproc

	.def	burst.Sleef_fmod_armv9a;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.Sleef_fmod_armv9a
	.globl	burst.Sleef_fmod_armv9a
	.p2align	2
burst.Sleef_fmod_armv9a:
	fabs	d1, d1
	mov	x8, #4850376798678024192
	mov	x9, #4503599627370496
	fmov	d3, x8
	fmov	d2, x9
	fabs	d5, d0
	mov	x8, #4363988038922010624
	fmov	d18, #3.00000000
	movi	d7, #0000000000000000
	fmov	d6, x8
	fmov	x8, d0
	fmov	d21, #2.00000000
	fmul	d4, d1, d3
	fcmp	d1, d2
	fmov	d2, #1.00000000
	fmul	d17, d5, d3
	fcsel	d6, d6, d2, mi
	fcsel	d1, d4, d1, mi
	movi	d4, #0000000000000000
	fcsel	d5, d17, d5, mi
	fdiv	d16, d2, d1
	fneg	d3, d1
	fmov	d25, d5
	fmov	x9, d3
	and	x9, x9, #0xfffffffff8000000
	fmov	d17, x9
	mov	x9, #4467570830351532032
	fmov	d22, x9
	mov	w9, #1
	fsub	d19, d3, d17
	fmov	x10, d16
	fcmp	d16, #0.0
	fmul	d16, d1, d18
	fadd	d18, d1, d1
	sub	x10, x10, #1
	fmov	d20, x10
	mov	x10, #-4476578029606273024
	fmov	d23, x10
	mov	x10, #4841369599423283200
	fmov	d24, x10
	fcsel	d20, d4, d20, eq
	b	.LBB16_2
.LBB16_1:
	fcmp	d16, d25
	fmov	x10, d26
	cset	w11, gt
	fcmp	d25, d1
	and	x10, x10, #0xfffffffffffffffe
	fmov	d26, x10
	cset	w12, gt
	tst	w11, w12
	fcsel	d26, d21, d26, ne
	fcmp	d18, d25
	cset	w10, gt
	tst	w10, w12
	fcsel	d26, d2, d26, ne
	fcmp	d7, #0.0
	fcsel	d27, d2, d4, ge
	fcmp	d25, d1
	fcsel	d26, d27, d26, eq
	fmov	x10, d26
	and	x10, x10, #0xfffffffff8000000
	fmov	d27, x10
	fsub	d28, d26, d27
	fmul	d26, d26, d3
	fmul	d29, d17, d27
	fmul	d27, d19, d27
	fsub	d29, d29, d26
	fmul	d30, d28, d17
	fadd	d31, d25, d26
	fmul	d28, d19, d28
	fadd	d29, d30, d29
	fsub	d30, d31, d25
	fadd	d27, d27, d29
	fsub	d29, d31, d30
	fsub	d26, d26, d30
	fadd	d27, d28, d27
	fsub	d25, d25, d29
	fadd	d25, d26, d25
	fadd	d7, d7, d27
	fadd	d7, d25, d7
	fadd	d25, d31, d7
	fsub	d26, d31, d25
	fcmp	d25, d1
	ccmp	w9, #21, #2, pl
	add	w9, w9, #1
	fadd	d7, d7, d26
	b.hs	.LBB16_4
.LBB16_2:
	fmov	x10, d25
	fcmp	d25, #0.0
	sub	x10, x10, #1
	fmov	d26, x10
	fcsel	d26, d4, d26, eq
	fmul	d26, d20, d26
	fabs	d27, d26
	fcmp	d27, d24
	b.ge	.LBB16_1
	fmul	d27, d26, d22
	fcvtzs	w10, d27
	scvtf	d27, w10
	fmul	d27, d27, d23
	fadd	d27, d26, d27
	fcvtzs	w10, d27
	scvtf	d28, w10
	fsub	d27, d27, d28
	fsub	d26, d26, d27
	b	.LBB16_1
.LBB16_4:
	fadd	d2, d25, d7
	fmul	d3, d6, d25
	and	x8, x8, #0x8000000000000000
	fcmp	d2, d1
	fmov	x9, d3
	csel	x9, xzr, x9, eq
	fcmp	d5, d1
	eor	x8, x9, x8
	fmov	d2, x8
	mov	x8, #9221120237041090560
	fcsel	d0, d0, d2, mi
	fcmp	d1, #0.0
	fmov	d1, x8
	fcsel	d0, d1, d0, eq
	ret

	.def	burst.memcpy.inline.ARMV9A.i64;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memcpy.inline.ARMV9A.i64
	.globl	burst.memcpy.inline.ARMV9A.i64
	.p2align	2
burst.memcpy.inline.ARMV9A.i64:
	cmp	x2, #16
	b.lo	.LBB17_6
	mov	x9, xzr
.LBB17_2:
	add	x10, x9, #32
	add	x8, x9, #16
	ldr	q0, [x1, x9]
	cmp	x2, x10
	str	q0, [x0, x9]
	mov	x9, x8
	b.hs	.LBB17_2
	add	x9, x8, #1
	cmp	x2, x9
	b.lo	.LBB17_5
.LBB17_4:
	add	x9, x8, #2
	ldrb	w10, [x1, x8]
	add	x11, x8, #1
	cmp	x2, x9
	strb	w10, [x0, x8]
	mov	x8, x11
	b.hs	.LBB17_4
.LBB17_5:
	ret
.LBB17_6:
	mov	x8, xzr
	add	x9, x8, #1
	cmp	x2, x9
	b.hs	.LBB17_4
	b	.LBB17_5

	.def	burst.memset.inline.ARMV9A.i32;
	.scl	2;
	.type	32;
	.endef
	.section	.text,"xr",discard,burst.memset.inline.ARMV9A.i32
	.globl	burst.memset.inline.ARMV9A.i32
	.p2align	2
burst.memset.inline.ARMV9A.i32:
	cmp	w2, #16
	b.lo	.LBB18_6
	dup	v0.16b, w1
	mov	w9, wzr
.LBB18_2:
	add	w10, w9, #32
	add	w8, w9, #16
	str	q0, [x0, w9, sxtw]
	cmp	w2, w10
	mov	w9, w8
	b.hs	.LBB18_2
	add	w9, w8, #1
	cmp	w2, w9
	b.lo	.LBB18_5
.LBB18_4:
	add	w9, w8, #2
	add	w10, w8, #1
	strb	w1, [x0, w8, sxtw]
	cmp	w2, w9
	mov	w8, w10
	b.hs	.LBB18_4
.LBB18_5:
	ret
.LBB18_6:
	mov	w8, wzr
	add	w9, w8, #1
	cmp	w2, w9
	b.hs	.LBB18_4
	b	.LBB18_5

	.section	.drectve,"yni"
	.ascii	" /EXPORT:3d466577b535881ddd547f1bda3cca2d"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.3d466577b535881ddd547f1bda3cca2d\""
	.ascii	" /EXPORT:\"burst.initialize.statics.3d466577b535881ddd547f1bda3cca2d\""
