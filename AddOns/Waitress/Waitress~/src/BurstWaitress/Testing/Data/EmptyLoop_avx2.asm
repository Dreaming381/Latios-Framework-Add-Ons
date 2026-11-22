
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	59c019ae9154aad244bf8a0175e78e30;
	.scl	2;
	.type	32;
	.endef
	.text
	.globl	59c019ae9154aad244bf8a0175e78e30
	.p2align	4
59c019ae9154aad244bf8a0175e78e30:
.seh_proc 59c019ae9154aad244bf8a0175e78e30
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r8d, r8d
	jle	.LBB0_14
	mov	eax, r8d
	cmp	r8d, 4
	jae	.LBB0_3
	xor	ecx, ecx
	jmp	.LBB0_12
.LBB0_3:
	cmp	r8d, 32
	jae	.LBB0_5
	xor	ecx, ecx
	jmp	.LBB0_9
.LBB0_5:
	mov	ecx, eax
	and	ecx, 2147483616
	mov	rdx, rcx
	.p2align	4
.LBB0_6:
	add	rdx, -32
	jne	.LBB0_6
	cmp	ecx, eax
	je	.LBB0_14
	test	al, 28
	je	.LBB0_12
.LBB0_9:
	mov	rdx, rcx
	mov	ecx, eax
	and	ecx, 2147483644
	sub	rdx, rcx
	.p2align	4
.LBB0_10:
	add	rdx, 4
	jne	.LBB0_10
	cmp	ecx, eax
	je	.LBB0_14
.LBB0_12:
	sub	rax, rcx
	.p2align	4
.LBB0_13:
	dec	rax
	jne	.LBB0_13
.LBB0_14:
	.seh_startepilogue
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

	.def	burst.initialize.externals.59c019ae9154aad244bf8a0175e78e30;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.59c019ae9154aad244bf8a0175e78e30
	.p2align	4
burst.initialize.externals.59c019ae9154aad244bf8a0175e78e30:
.seh_proc burst.initialize.externals.59c019ae9154aad244bf8a0175e78e30
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

	.def	burst.initialize.statics.59c019ae9154aad244bf8a0175e78e30;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.59c019ae9154aad244bf8a0175e78e30
	.p2align	4
burst.initialize.statics.59c019ae9154aad244bf8a0175e78e30:
.seh_proc burst.initialize.statics.59c019ae9154aad244bf8a0175e78e30
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
	.ascii	" /EXPORT:59c019ae9154aad244bf8a0175e78e30"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.59c019ae9154aad244bf8a0175e78e30\""
	.ascii	" /EXPORT:\"burst.initialize.statics.59c019ae9154aad244bf8a0175e78e30\""
