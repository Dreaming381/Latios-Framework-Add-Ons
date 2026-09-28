# Performance

## The Strategy

There are three ways to make Peekaboo faster, and they aren't equally important.

1.  **Do less.** Most of the design effort goes here. The occluder budget, the
    coverage floor, the backface cone on each plate, the plate budget at bake
    time, the chunk-level early outs, and binning plates by row all exist to
    avoid starting work in the first place.
2.  **Spread it out.** Every stage that grows with the scene is a parallel job,
    over chunks or over rows.
3.  **Make each unit cheap.** Only where measurements point. In practice that
    turned out to be the per-entity test, not the rasterizer.

Burst isn't one of the three. It's a requirement. Without it, the rasterizer is
10 to 13 times slower and costs far more than it saves, which is why Peekaboo turns
itself off when Burst is disabled.

## The Parallel Shape

| stage | runs in parallel over | grows with |
|---|---|---|
| find chunks with occluders | meta chunks | chunks |
| gather occluder candidates | chunks that have occluders | renderable entities, minus chunks too small on screen |
| select and reserve | single job | candidates |
| build plates, find their row crossings, and bin them by band | selected occluders | occluders, plus plates times their height |
| clear, rasterize, and reduce | pairs of rows in every view at once | buffer size, plus plates times their height |
| find chunks to cull | meta chunks | chunks |
| cull | batches of chunks | renderable entities, minus chunks answered in one test |

Selection is the one single-threaded stage, because picking the best candidates
per view needs to see the whole view. It isn't a full sort, though. It groups
candidates by view with a counting sort, then partitions each view around its
budget, skipping any subrange entirely on one side of the cut. That's about `2n`
work instead of `n log n`.

Rows of *every* view run as one parallel job, not one job per view. So a shadow
pass with four cascades keeps the workers just as busy as a camera pass.

### Fewer Stages Beats Faster Stages

At light workloads, like 200,000 entities instead of millions, the pass is bound by
latency, not work. The per-stage sum and the whole pass come out nearly equal, so
the stages barely overlap, and each dependent dispatch adds a wait. So the occluder
side is two jobs after selection instead of six:

-   **Build and bin.** Each selected occluder builds its plates and sets their
    bits in the same job. Plates from neighboring occluders share bitfield words,
    so bits get set with an atomic OR.
-   **Clear, rasterize, and reduce.** Each work item is a pair of rows. It clears
    them, rasterizes them, and reduces them into a row of level one of both
    pyramids while they're still in cache. The last pair to finish in a view builds
    the rest of that view's pyramids, found with an atomic counter instead of
    another job.

Nothing clears the bitfield. The second pair in each band clears the band's words
after reading them, so it's all zero when the next pass starts, and it lives on the system
between passes. Selection only clears memory it grows.

Measured against the six-job version: at 200,000 entities in the city scene, the
whole pass went from 0.42, 0.39, and 0.40 ms to 0.35, 0.32, and 0.34 ms at three
poses. With the occluder budget full, `PeekabooLoadedPassBench` went from 0.26 to
0.20 ms for a camera pass and 0.46 to 0.38 ms for four cascades, and the old
numbers didn't even include clearing the depth buffer.

Two things didn't work. Building by 64-plate column, so binning needed no atomics,
made each work item build 64 plates in a row and lost the build's parallelism.
Bands of eight rows cut binning further but left the rasterizer unbalanced. Four
rows was the best of two, four, and eight. Rasterizing a whole band per item was
also too coarse. The heavy rows around the horizon piled up on a few workers, so
items are now pairs of rows, the smallest the first reduction allows.

## Where the Time Goes

In a big scene, the per-entity work is what decides whether Peekaboo is
affordable. Kinemation scenes push hundreds of thousands of entities through
culling, and every survivor gets tested once per view. So the cull job, and the
gather job that picks occluders, matter much more than the rasterizer does.

**The rasterizer only matters when the per-entity work is small.** In a scene
with a few hundred thousand entities instead of several million, the per-entity
jobs finish quickly, and the rasterizer's single-plate latency starts to show up
as its own chunk of the frame.

### The Cull Job

For each entity that isn't settled at the chunk level, the cull job does two
things: projects its bounds to the screen, and reads the depth pyramid over that
rectangle. Measured on the four-million-entity city, **the pyramid read costs more
than the projection** at every camera pose. Query rectangles are small, roughly 10
by 2 texels for a chunk and 2 by 2 for an entity.

What makes this fast:

-   **Chunk tests first.** The min pyramid can say a whole chunk is hidden, and
    the max pyramid can say nothing in a chunk could be hidden. Most chunks in a
    big scene are one or the other, so one test answers for up to 128 entities.
-   **A meta-chunk pre-pass.** `FindChunksToCullJob` walks meta chunks and only
    emits chunks that frustum culling left something in. It also carries pointers
    to the chunk's bounds and masks forward, so the cull job doesn't have to look
    them up again.
-   **Batching chunks.** Scheduling the cull job with a batch size of one means one
    atomic claim per chunk, across tens of thousands of chunks. Batching them is
    one of the biggest single wins available, around 7% to 18% depending on the
    view.
-   **Splitting the loop.** Projecting every entity first, then reading the pyramid
    for every entity, is much faster than doing both per entity. When they're
    fused, the pyramid read's hard-to-predict branch sits between one projection
    and the next and stops the CPU from running ahead. Split apart, the
    projections pipeline.
-   **Eight entities at a time.** For perspective views, the projection runs on
    eight entities at once, one per SIMD lane. Projecting one entity's eight
    corners across eight lanes has to end in a cross-lane reduction, which is
    mostly shuffles. Eight entities in eight lanes never reduces at all. The
    orthographic projection is too cheap to benefit: gathering eight entities into
    lanes costs more than projecting them.
-   **Avoiding `math.cmin` and `math.cmax` on a `float4`.** Each is a
    12-instruction serial chain that handles NaN carefully. Raw `minps` and `maxps`
    with one finiteness check up front are much cheaper. (Note that `minps` drops
    a NaN instead of passing it on, so the check has to happen *before* the
    reduction.)
-   **Not copying `PeekabooView`.** It's over 300 bytes, and `NativeArray<T>`'s
    indexer returns by value. So `views[v]` passed to an `in` parameter copies the
    whole struct to the stack, once per entity per view. Indexing through
    `views.AsReadOnlySpan()` avoids the copy. Watch for this in any hot DOTS loop
    over a `NativeArray` of large structs.
-   **Reading the pyramid with SIMD.** The scan loads eight texels at a time,
    compares them all, and checks the result with a movemask. The buffer is padded
    so it's safe to read past the end of a row and mask off the extra lanes.

### Ideas That Didn't Work

These were measured, not guessed, so there's no need to try them again:

-   **Projecting an AABB with the rotate-the-extents trick** instead of projecting
    eight corners. It cuts the instruction count a lot, but the resulting screen
    rectangle is looser under perspective. A looser rectangle means more pyramid
    reads and more survivors, which costs more than the projection saves.
-   **Eight at a time for orthographic views.** Slower, as described above.
-   **`FloatMode.Fast`.** Produces identical assembly. The NaN handling in
    `math.cmin` is an explicit select, not something fast math can relax.
-   **FMA, or building corners from center and extents.** Fewer instructions, no
    time saved, and it loses bit-exact results.
-   **Settling half a chunk at a time** by bounding 64 entities at once. Pure
    overhead in the city scene, since the half-chunk bounds were almost as big as
    the whole chunk.
-   **Chunk near and far depth planes** to skip per-entity tests. Saves at most 3%
    and adds 14% more texel reads, since chunk rectangles are too loose.
-   **Deferring the mask load, or carrying chunk bounds out of the meta pass** to
    avoid a lookup. No effect. Chunks come out in meta-chunk order, so the lookup
    was already in cache.

### The Gather Job

Most of the gather job's time used to go to visiting chunks with nothing in them.
In the city scene every entity has a plate blob, so without help gather would walk
all ~43,000 chunks, and about 30,000 of them have no entities left after frustum
culling. `FindOccluderChunksJob` fixes this the same way the cull job does: a
meta-chunk pre-pass that only emits chunks with work.

The occluder query has a dozen `Without<>` clauses, and a meta query can't filter
on entity components. So the pre-pass checks them with an `EntityQueryMask` built
from the same query, which is a single archetype compare and can't drift out of
sync with the query.

Gather also has its own chunk test. A chunk's bounds contain every entity in it,
and projecting a box inside another box lands inside the outer box's screen
rectangle. Coverage is capped at one, so no entity can score higher than its
chunk's screen fraction. **A chunk too small on screen to be worth rasterizing
can't hold anything worth rasterizing**, and one projection dismisses all its
entities.

Be careful with chunks whose bounds cross the near plane. Those can't be
projected, so they're *kept* rather than dismissed, since entities inside them
might project fine.

## The Occluder Side

### Binning Is a Bitfield

Binning records which plates cross which bands of four rows, using **one bit per
band and plate**, stored band-major. The build job sets the bits, and each row in
the band reads them back with `tzcnt`. A bit per row worked the same way, but cost
four times the atomic writes.

**Crossings are found once per plate.** Right after building a plate, the build
job walks its edges once and records where they cross every row boundary the
plate covers, since each row's bottom is the next row's top. A row then reads two
crossings per plate instead of walking the plate's edges. It's the same math as
crossing one row at a time, so the results match exactly. The buffer is sized at
32 boundaries per plate, and a plate that doesn't fit falls back to finding its
own crossings per row. Moving this out of the rasterizer cut its CPU time by about
a quarter at street level.

### Rows Near the Horizon

At street level, a row near the horizon can hold 260 spans, where an average row
holds 20. Three things used to grow faster than linearly there, and all three were
replaced with versions that give bit-identical results, so the survivor counts
stayed a reliable check:

-   **Sorting** was an insertion sort of 32-byte spans. Now it sorts 64-bit keys
    and then moves each span once. Each key is the start's float bits, flipped to
    order as integers, above the plate index, which breaks ties so the order
    matches the old stable sort. A row that's already in order skips the sort.
    Rows of 40 spans or more use a radix sort on the float half, four passes of
    8 bits, skipping any pass where every key has the same digit. It's stable, so
    ties still come out in index order. Shorter rows use a comparison sort.

    In `PeekabooSortProbe`, radix sorting 48 spans took 0.49 µs against 0.65 µs
    for the comparison sort, and 256 spans took 1.4 µs against 5.0 µs. Below 32
    spans, clearing and summing the 1024 buckets costs more than it saves. 11-bit
    digits lost at every row size up to the 512-span cap, since 2048-bucket
    histograms cost more than one saved pass. In the loaded pass bench, it took
    the rasterizing stage for four full cascades from 0.190 to 0.185 ms, and made
    no measurable difference to a camera pass, where fewer rows reach 40 spans.
-   **Welding** checked every plate in a run for every empty pixel. Now each plate
    visits its own edge pixels, and a bit per pixel marks which ones got reached,
    so nothing walks the run's interior. See [Rasterizing](Rasterizing.md).
-   **Scratch memory** was zeroed on every row, about 30 KiB of it, because a
    `stackalloc` is zeroed unless the method has `[SkipLocalsInit]`. The raster
    methods have it now, and scratch is sized to the row instead of the maximum.

At 400,000 entities that took the rasterizing stage from 0.141, 0.097, and 0.147
ms to 0.117, 0.089, and 0.117 ms at three poses.

**Most of the slowest items aren't slow.** Timing every phase inside the slowest
item of a sampling window showed its phases adding up to a tenth of its wall
time, and one of them held three plates. Those are workers getting interrupted,
not rows with too much work.

Items are also visited in a scattered order, with a stride coprime to the item
count, so the neighboring heavy rows around the horizon don't all fall into one
worker's range. That measured as no change here, but it targets a pattern seen in
other profiles, where a few workers carried long rasterizing slices while the rest
sat idle.

There's no list of indices, so there's no count to take, no offsets to compute,
and no capacity to pick:

-   **No per-row capacity.** A row holds as many plates as it holds.
-   **Order comes free.** Bits come out in ascending order, so a row's plates
    arrive in index order without any sorting.
-   **Setting bits is the whole job.** It happens right where each plate gets
    built, with no job of its own.

**A row only reads its own view's words.** Candidates are grouped by view and
reserve their plates in order, so each view's plates are one contiguous run. With
four shadow cascades, a row scans a quarter of the bitfield. That keeps the read
cheaper than a packed list, even though the bitfield is only about 1% full.

Here's how it compared to list-based designs, back when binning was its own job
with a bit per row, in a pass with the full occluder budget:

| | camera | light, 4 cascades | light, 4x budget |
|---|---|---|---|
| one counting sort | 0.059 ms | 0.181 ms | ~0.71 ms |
| count, scan, scatter | 0.092 ms | 0.114 ms | 0.163 ms |
| count, reserve, scatter | 0.059 ms | 0.099 ms | 0.096 ms |
| **bitfield** | **0.027 ms** | **0.055 ms** | **0.087 ms** |
| bitfield memory | 24 KiB | 384 KiB | 1.5 MiB |
| packed list memory | 256 KiB | 1 MiB | 1 MiB |

The bitfield is smaller and faster everywhere except memory at a very large
budget, where a bit per plate per row eventually outgrows a fixed slice per row.
That only happens well past where the sweep's own span limit kicks in.

Two other designs don't fit well. An `UnsafeIndexedBlockList` keyed by row and
batch ends up with nearly one element per stream, and pays a block allocation for
each. A `NativeStream` keeps write order but is keyed by foreach index, so
regrouping by row would be single-threaded again.

### Order Within a Row Matters (a Little)

The row sweep sorts spans by where they start, which should make arrival order
irrelevant. It almost does, except for ties. Ties go to the lower plate index, so
plates starting at the same column keep their binned order. The welding pass
then chains each span to the one right before it, so a different tie order breaks
chains in different places, and a chain's length decides how many seam pixels get
welded.

**Every ordering is safe**, because each link in a chain gets checked on its own,
so no ordering can weld across a real gap. They only differ in how much they
manage to weld. Keeping a fixed order is still worth it, since it makes passes
reproducible regardless of which worker took which occluder. The bitfield gives
you that order for free.

### The Span Limit

A row holds at most `kMaxSpansPerRow` (512) spans, in a stack buffer. At the
default budget, the busiest row in a loaded shadow pass holds 178. At four times
the budget, 113 of 512 rows go over, with the busiest at 734. So this limit, not
the bitfield, is what caps how high the occluder budget can usefully go.

### Plate Shapes

The rasterizer's cost follows **plate-row crossings**, not plates or pixels.
Rounded meshes produce more plates than boxes, but that barely matters:

| mesh | triangles | plates |
|---|---|---|
| Cube | 12 | 6 |
| Cylinder | 80 | 28 |
| Sphere | 768 | 17 |
| Capsule | 832 | 32 |

A loaded shadow pass with spheres carries almost three times the cubes' plates,
yet rasterizes *faster*. A sphere's plates are interior slices, which are small on
screen and often face away and get dropped by the backface cone. A box's six
plates are all big, and two or three always face the viewer.

## Latency Against the Frame

At a few hundred thousand entities, what hurts isn't a pass's CPU time. It's how
long the main thread waits on the culling jobs. The clearest place to see it is
`UploadMaterialPropertiesSystem`, on its second update of the frame, the one that
takes the Write path. The time between the start of its `ShouldUpdateSystem()`
and its `OnUpdate()` is the wait. More than 0.25 ms of it at under 500,000
entities is a problem.

What that wait is made of:

-   **Passes run one after another.** Each pass rewrites the chunk culling masks,
    so a shadow pass's jobs can't start until the camera pass, and everything
    downstream of it, is done with them. The wait covers the tail of that whole
    chain, not one pass.
-   **The main thread decides who wins.** The job chain is about the same length
    every frame. Frames with a short wait are the ones where the main thread took
    longer to get to the upload, not the ones where the jobs ran faster.
-   **Peekaboo is most of the chain.** At 400,000 entities at street level, its
    camera pass ran about 380 µs from its first job to the end of its cull, and its
    shadow pass about 270 µs, out of roughly 1 ms from the camera pass being
    scheduled to the wait ending.
-   **Every dependent job costs a wake-up.** When a stage ends raggedly, most
    workers go to sleep, and the next stage's first item starts 20 to 30 µs after
    the last one ended. That happens between select and build, and between the
    rasterizer and the cull.

Shortening the rasterizer (crossings from the build job, pairs of rows, packed
sort keys, the weld bitmask, and a four-wide first reduction) took the median
wait at four parked poses from 0.33, 0.23 to 0.26, 0.22 to 0.24, and 0.17 ms to
0.26 to 0.29, 0.21, 0.15 to 0.16, and 0.12 to 0.13 ms. That's not under the line
at street level yet.

**Don't write a scheduler.** Running build, rasterize, and cull as one job whose
workers spin between stages removed the wake-ups and cut the street median to
0.20 ms. But a spinning worker can't yield to the OS. When the OS takes a core
from a worker in the middle of an item, the spinners keep it from getting one
back for a whole time slice, and some frames stalled for 25 ms. Giving up after
a timeout bounded the stalls but gave back most of the gain. It was also tuned to
one 32-thread machine. User machines vary too much for it to hold, so stages stay
separate jobs.

## SIMD

The hand-written SIMD is all in the per-entity path, since that's where the
per-entity work is:

-   The AVX2 perspective projection, eight entities at a time
-   The four-wide orthographic projection
-   The pyramid scan, eight texels at a time with a movemask

The rasterizer has none. Its per-pixel ramp write is an unconditional
multiply-add-max over a contiguous float range with no branches, which Burst
vectorizes on its own. Most of the rasterizer's cost is finding where each plate's
edges cross each row, which is branchy and reads from a small polygon, and that
isn't a good fit for SIMD. [Deferred Work](Deferred%20Work.md) describes
precomputed edge chains, which would attack that cost directly.

## How to Measure

Numbers go stale fast, so this page mostly explains *why* things are built the way
they are. To get current numbers, use the tools in the workspace:

-   `PeekabooStressBenchmark` parks the camera at fixed poses in either stress scene
    and reports median frame time and counts over 120 frames. Compare survivors
    against what made it past frustum culling.
-   `PeekabooScaleBench` runs the real gather and cull jobs over synthetic scenes
    of up to half a million entities, without needing to render them.
-   `PeekabooLoadedPassBench` runs the occluder jobs with every view's budget full.
-   `PeekabooRasterBenchmark` times the rasterizer and the occludee sweep on their
    own.

A few things to know when measuring:

-   **Measure by adding work, not removing it.** To find what a piece of work costs,
    do it twice and take the difference. Removing it instead can change which
    chunks get settled early, and then you're measuring a different scene. In the
    city scene, stubbing out a test made the pass *five times slower* for exactly
    this reason. Check that survivor counts match to make sure nothing else changed.
    Feed the duplicate an input Burst can't prove is constant, or it'll get folded
    away.
-   **Only instructions that run matter, not code size.** Hoisting a branch out of a
    loop so less code gets inlined made no measurable difference.
-   **Changing Burst options invalidates every compiled job.** Right after a change,
    jobs run managed while Burst recompiles in the background, and look ten times
    slower. Re-run until the numbers settle.
-   **Safety checks don't matter much here.** Turning them off moves nothing past
    run-to-run noise, because the hot loops read through raw pointers.
-   **Make sure the scene actually loaded.** A compile error can silently drop the
    stress scene to a couple thousand renderables, and the numbers still look
    plausible.

## Allocation

Everything allocates from `state.WorldUpdateAllocator`. Inside the culling loop
that's the rate group's allocator, which gets rewound at the end of each pass. So
nothing lives between passes, and nothing needs to be disposed by hand.

The one fixed-size allocation is the occluder candidate array, sized from
`maxOccludersPerView`. Extra candidates get dropped rather than growing the array.
Dropping occluders is always safe, and resizing inside a parallel job isn't.
