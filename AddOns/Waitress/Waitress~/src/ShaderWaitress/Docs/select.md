THE SELECTOR LANGUAGE

A selector is a pipeline of steps separated by `|`. The first step filters the
whole graph; each later step filters or walks the set it is handed. Whole
pipelines combine with `+` (union), `-` (difference) and `&` (intersection).

  shaderwaitress q g.shadergraph "type:Multiply | in | type:Property"

reads as: every Multiply node, then everything feeding those, then keep only
the Property nodes among them. All in one invocation.

MATCHERS                        (glob `*` and `?`; `,` separates alternatives)

  all                           every node
  none                          empty set
  type:Multiply,Add             node type, with or without the "Node" suffix
  type:Sample*                  glob on the type
  name:Fresnel*                 node display name
  id:n42d5,nb17                 specific nodes
  group:"Soft particles"        members of a group, by title or group id
  prop:_Mask                    Property nodes reading that property
  block:*BaseColor              block nodes, by descriptor
  port:UV                       nodes that have a port called UV
  rank:0   rank:>2              distance from the output, 0 is rightmost
  is:<tag>                      see below
  <bare word>                   shorthand for name-or-type-or-id

TAGS FOR is:

  orphan       no wires at all              root      no connected inputs
  leaf         no connected outputs         block     is a block node
  property     is a Property node           keyword   is a Keyword node
  grouped      belongs to a group           ungrouped belongs to none
  reachable    feeds a block                unreachable does not
  node         is not a block

TRAVERSALS

  in            direct upstream neighbours
  out           direct downstream neighbours
  in*           everything upstream, transitively
  out*          everything downstream, transitively
  in:UV         upstream, but only through the port named UV
  out:RGBA      downstream, but only out of the port named RGBA
  both          in plus out

EXAMPLES

  "is:unreachable"                          dead branches
  "block:BaseColor | in*"                   everything Base Color depends on
  "type:SampleTexture2D | out"              what consumes each texture sample
  "prop:_Speed | out* | type:*Block*"       which outputs a property reaches
  "group:Fresnel - is:property"             group members that are not tokens
  "type:Multiply & is:ungrouped"            Multiply nodes not in any group
  "is:orphan + is:unreachable"              everything not doing any work
  "type:Add | in:B"                         what feeds the B input of each Add

OUTPUT

  --format text   (default) the matched nodes in dense form
  --format ids    one short id per line, for piping into another command
  --format count  just the number

TIP

Selectors are also accepted anywhere a command says `<selector>`, so a single
`node set` can retune every node a query finds:

  shaderwaitress node set g.shadergraph "type:Multiply | is:ungrouped" --group g1
