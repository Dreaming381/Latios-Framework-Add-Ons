
	.def	@feat.00;
	.scl	3;
	.type	0;
	.endef
	.globl	@feat.00
@feat.00 = 0
	.intel_syntax noprefix
	.file	"main"
	.def	80257df5f57d3c1e53b6fdb472701a02;
	.scl	2;
	.type	32;
	.endef
	.globl	__real@7fc00000
	.section	.rdata,"dr",discard,__real@7fc00000
	.p2align	2, 0x0
__real@7fc00000:
	.long	0x7fc00000
	.text
	.globl	80257df5f57d3c1e53b6fdb472701a02
	.p2align	4
80257df5f57d3c1e53b6fdb472701a02:
.seh_proc 80257df5f57d3c1e53b6fdb472701a02
	push	rbp
	.seh_pushreg rbp
	mov	rbp, rsp
	.seh_setframe rbp, 0
	.seh_endprologue
	test	r9d, r9d
	jle	.LBB0_3
	mov	eax, r9d
	vbroadcastss	xmm0, dword ptr [rip + __real@7fc00000]
	.p2align	4
.LBB0_2:
	vmovups	xmmword ptr [r8], xmm0
	add	r8, 16
	dec	rax
	jne	.LBB0_2
.LBB0_3:
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

	.def	burst.initialize.externals.80257df5f57d3c1e53b6fdb472701a02;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.externals.80257df5f57d3c1e53b6fdb472701a02
	.p2align	4
burst.initialize.externals.80257df5f57d3c1e53b6fdb472701a02:
.seh_proc burst.initialize.externals.80257df5f57d3c1e53b6fdb472701a02
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

	.def	burst.initialize.statics.80257df5f57d3c1e53b6fdb472701a02;
	.scl	2;
	.type	32;
	.endef
	.globl	burst.initialize.statics.80257df5f57d3c1e53b6fdb472701a02
	.p2align	4
burst.initialize.statics.80257df5f57d3c1e53b6fdb472701a02:
.seh_proc burst.initialize.statics.80257df5f57d3c1e53b6fdb472701a02
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
	.ascii	" /EXPORT:80257df5f57d3c1e53b6fdb472701a02"
	.ascii	" /EXPORT:\"burst.initialize\""
	.ascii	" /EXPORT:\"burst.initialize.externals.80257df5f57d3c1e53b6fdb472701a02\""
	.ascii	" /EXPORT:\"burst.initialize.statics.80257df5f57d3c1e53b6fdb472701a02\""
