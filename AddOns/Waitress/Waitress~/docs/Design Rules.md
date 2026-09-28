# Waitress Design Rules

Both tools are built around five rules. When a design choice is unclear, these
settle it.

## The Rules

1.  **A graph the tool touched stays readable to a human.** Not just free of
    overlaps. Flow runs in one direction, wires meet their ports squarely instead
    of at shallow angles, and no wire crosses a node it has nothing to do with.
2.  **Edits respect the human's layout.** An edit changes what it was asked to
    change and nothing else. Positions a person chose are theirs. A full
    re-layout only happens when someone asks for one.
3.  **Agents work in as few calls and tokens as possible.** If adding a node and
    seeing the result takes five round trips, the tool has failed, even if every
    call succeeded.
4.  **Agents never think about positions.** No agent should reason about
    coordinates, node sizes, or wire angles. The only layout question an agent
    ever faces is whether the human wants the graph reorganized.
5.  **The Editor never gets stuck.** Nothing may leave Unity waiting on a modal
    dialog it can't answer, especially the unsaved-changes dialog. Recovering
    from that costs the user their session.

## What Follows From Them

Anyone using these tools has a Unity project with the Editor open. That's the
normal case, not an edge case, so the tools assume an Editor is there and use it
when it helps.

Reading is where the tokens go, and it has no side effects, so reading stays
offline and fast. That's rule 3. Writing is where rules 2 and 5 live. Letting the
Editor do the final write means memory and disk never disagree, there's no dirty
flag to clean up, and there's no dialog to trip over.

Layout belongs to the tool. Rule 4 means adding a node puts it somewhere sensible
without another call. Rule 2 means doing so moves nothing else. Rule 1 means the
result is readable. Node sizes aren't stored in either file format, so a readable
layout needs sizes measured by the Editor rather than guessed.

## How Each Rule Is Met

**1. Readable.** Both tools score their own layouts. ShaderWaitress uses a
penalty model covering overlaps, backward wires, crossings, shallow crossing
angles, and wires drawn over nodes. VfxWaitress counts steep wires and wires that
cross unrelated nodes. Each lays a graph out more than one way and writes the
arrangement that scores best. The arrangement the graph arrived in is always one
of the candidates, so running layout twice changes nothing the second time. The
score gets printed too, so a complaint about readability comes with a number.

VfxWaitress reads real node and port rectangles from the live graph view, so a
wire gets aimed at the port it actually lands on. ShaderWaitress uses the sizes
Shader Graph writes into the file, and estimates the rest from measured
constants.

**2. Edits respect the layout.** Adding a node places only that node. VfxWaitress
`repair` places only the node it's repairing. Only `layout` moves anything else,
and only when the result measures better. ShaderWaitress also refuses to write a
full layout that scores worse than the one already in the file.

**3. Minimal calls.** `show`, `query`, and `validate` never leave the process.
Both tools batch a whole editing session into one `apply` call that writes once.
When a write needs the Editor, the tool makes those round trips itself inside the
one command the agent ran, as direct Pipeline command calls.

**4. No positions in the agent's head.** No command takes a coordinate, and there
is no way to set one. New nodes are placed beside what they're wired to once the
command or script finishes, so a graph built node by node reads sensibly without
a `layout` call.

**5. No stuck Editor.** Both tools save whatever an open window hasn't saved
before they read the file, then hand the result back.

-   VfxWaitress has VFX Graph write the final asset itself, so the bytes on disk
    are the Editor's own. Opening the graph then doesn't mark it dirty, and no
    unsaved-changes dialog appears. This was verified on all five LifeFX graphs:
    the tool writes, the Editor re-serializes, and the file doesn't change.
-   ShaderWaitress has the Shader Graph window reload by dropping its graph,
    which skips the "Graph has changed on disk" dialog. That dialog can't be
    answered when Unity runs with `-automated`. Before the reload, Shader Graph
    rewrites the file in its own form if that differs, so the window has nothing
    to compare unfavorably against. Verified with a window holding unsaved
    changes: they end up in the file alongside the tool's edit, the window
    reloads without asking, and it shows no asterisk afterwards. Verified with
    an untouched window too: it reloads to the tool's edit, also clean.

Both need the matching Editor Bridge sample. Without it, or with `--offline`,
the tools write the file directly and say so.

## Known Issue: Wires Between VFX Systems

In a VFX graph with more than one system, an operator that feeds several systems
sends a wire across the columns in between, over whatever is drawn there. The
QvvsOrbit graph shows this.

The real fix is portal nodes, which VFX Graph gains in Unity 6.4. The Inline
operator isn't a substitute. It's an ordinary node with a wire in and a wire out,
so routing through one just moves the diagonal. Until then, VfxWaitress tries two
spots for such an operator, either in the margin of the first system it serves or
in a lane above the columns, and keeps whichever measures better.
