IDENTIFIERS

Shader Graph identifies everything with a 32 hex character object id. Those are
unreadable and expensive to pass around, so the tool shows a short id instead:
a kind letter plus the shortest unique prefix of the object id, starting at four
characters.

  n42d5  node        b6978  block        g1734  group
  p8e6e  property    k0a1c  keyword      t22b9  target

Short ids are STABLE. They come only from the object id, so adding or removing a
node doesn't renumber anything else, and an id from one call still works in the
next. Nodes the tool creates get object ids with a prefix nothing else has, so
they never collide. A node added in the Editor can still collide with an
existing one, and then both get a longer prefix. The full object id always
works.

Anywhere a command wants an entity you may pass:

  a short id                n42d5
  a full object id          420025db64c641ceb408c4872a903120
  a node display name       "Simple Noise"        (when unambiguous)
  a block descriptor        BaseColor  or  SurfaceDescription.BaseColor
  a property name or ref    "Heat Distortion"  or  _HeatDistortion

For a port, append a dot: n42d5.RGBA. The port half may be dropped whenever the
node has exactly one port in the required direction.

A property reference used in a wiring position resolves to the Property NODE
that reads it, provided there is exactly one.
