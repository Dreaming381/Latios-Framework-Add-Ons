using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VfxWaitress.Model
{
    public sealed class Finding
    {
        public string Severity;
        public string Where;
        public string Message;
        public override string ToString() => $"{Severity}: {Where}: {Message}";
    }

    /// <summary>
    /// Structural checks that only need the file. The real compile goes through
    /// <see cref="Edit.EditorLink"/>.
    ///
    /// VFX Graph's per-node error reporters stay quiet on graphs that can't compile, and a failed
    /// compile still leaves an asset that loads, just with every exposed property missing. So an
    /// offline "ok" is never proof. Only --editor is.
    /// </summary>
    public static class Validator
    {
        public static List<Finding> Check(VfxAsset asset)
        {
            var findings = new List<Finding>();

            foreach (var node in asset.AllNodes())
            {
                if (node.Model == null && !asset.Catalog.IsEmpty)
                {
                    findings.Add(new Finding
                    {
                        Severity = "warning",
                        Where = node.ShortId,
                        Message = $"script {node.ScriptGuid} is not in the catalog; it belongs to a package that was not installed when the catalog was exported",
                    });
                }

                // More than four inputs throws while the node's slots resolve, which aborts the
                // whole graph's compile instead of flagging the node.
                if (node.TypeName != null && node.TypeName.EndsWith(".CustomHLSL", StringComparison.Ordinal))
                {
                    var inputs = node.Inputs.Count;
                    if (inputs > 4)
                    {
                        findings.Add(new Finding
                        {
                            Severity = "error",
                            Where = node.ShortId,
                            Message = $"Custom HLSL function takes {inputs} inputs; VFX Graph allows at most 4, because VFXExpression refuses more than 4 parents. " +
                                      "The whole graph will fail to compile, not just this node. Pack inputs into a float4x4 to get under the limit.",
                        });
                    }
                }

                foreach (var slot in node.AllInputSlots.Concat(node.AllOutputSlots))
                {
                    foreach (var linked in slot.LinkedSlotIds)
                    {
                        var other = asset.SlotById(linked);
                        if (other == null)
                        {
                            findings.Add(new Finding
                            {
                                Severity = "error",
                                Where = $"{node.ShortId}.{slot.Path}",
                                Message = $"links to slot {linked}, which is not in this file",
                            });
                            continue;
                        }
                        // VFX keeps no edge list; a link recorded on one end only is a broken graph.
                        if (!other.LinkedSlotIds.Contains(slot.FileId))
                        {
                            findings.Add(new Finding
                            {
                                Severity = "error",
                                Where = $"{node.ShortId}.{slot.Path}",
                                Message = $"links to {other.Owner?.ShortId}.{other.Path}, which does not link back",
                            });
                        }
                        if (slot.IsInput == other.IsInput)
                        {
                            findings.Add(new Finding
                            {
                                Severity = "error",
                                Where = $"{node.ShortId}.{slot.Path}",
                                Message = $"links to {other.Owner?.ShortId}.{other.Path}, which faces the same direction",
                            });
                        }
                    }
                }
            }

            foreach (var context in asset.TopLevel.Where(n => n.Kind == "context"))
            {
                var hasIncoming = asset.AllNodes().Any(n => n.FlowOut.Any(f => f.toContext == context.FileId));
                if (!hasIncoming && context.FlowOut.Count == 0 && asset.TopLevel.Count(n => n.Kind == "context") > 1)
                {
                    findings.Add(new Finding
                    {
                        Severity = "warning",
                        Where = context.ShortId,
                        Message = $"{context.DisplayType} has no flow link in or out, so nothing drives it",
                    });
                }
            }

            // Two particle contexts joined by a flow link are one system and must share their
            // VFXData. Flow edges crossing between systems is a shape the Editor never writes.
            foreach (var node in asset.AllNodes())
            {
                foreach (var flow in node.FlowOut)
                {
                    if (!asset.Nodes.TryGetValue(flow.toContext, out var target))
                        continue;
                    if (node.Data == null || target.Data == null || node.Data == target.Data)
                        continue;
                    if (node.Data.Doc.Body.Find("capacity") == null || target.Data.Doc.Body.Find("capacity") == null)
                        continue;
                    findings.Add(new Finding
                    {
                        Severity = "error",
                        Where = node.ShortId,
                        Message = $"flows into {target.ShortId} but they are in different systems ({node.Data.ShortId} and {target.Data.ShortId}); " +
                                  "a flow link between particle contexts also makes them share one VFXData",
                    });
                }
            }

            // Everything below is a shape VFX Graph silently repairs the moment the asset is
            // opened, which marks it dirty and produces a diff nobody asked for.
            if (string.IsNullOrWhiteSpace(asset.GraphDoc?.Body.Scalar("m_Name")))
            {
                findings.Add(new Finding
                {
                    Severity = "warning",
                    Where = "graph",
                    Message = "has no m_Name; the Editor fills it in from the file name on open and marks the asset dirty",
                });
            }

            var parameters = asset.TopLevel.Where(n => n.ExposedName != null).ToList();
            foreach (var parameter in parameters)
            {
                var wired = parameter.AllOutputSlots.Any(s => s.LinkedSlotIds.Count > 0);
                if (wired && parameter.ParameterNodes.Count == 0)
                {
                    findings.Add(new Finding
                    {
                        Severity = "warning",
                        Where = parameter.ShortId,
                        Message = $"\"{parameter.ExposedName}\" is wired but has no placement in m_Nodes, so it is drawn nowhere; " +
                                  "the Editor builds one from the links on open. `layout` writes it.",
                    });
                }
            }

            foreach (var parameter in parameters.Where(p => !p.IsOutput))
            {
                if (parameter.AllOutputSlots.Any(s => s.LinkedSlotIds.Count > 0))
                    continue;
                findings.Add(new Finding
                {
                    Severity = "warning",
                    Where = parameter.ShortId,
                    Message = $"\"{parameter.ExposedName}\" is exposed but nothing in the graph reads it, " +
                              "so it appears in the inspector and on every VisualEffect and does nothing",
                });
            }

            var orders = parameters
                .Select(p => (p, order: p.Doc.Body.Scalar("m_Order")))
                .Where(t => t.order != null)
                .GroupBy(t => t.order)
                .Where(g => g.Count() > 1)
                .ToList();
            foreach (var clash in orders)
            {
                findings.Add(new Finding
                {
                    Severity = "warning",
                    Where = string.Join(" ", clash.Select(t => t.p.ShortId)),
                    Message = $"share m_Order {clash.Key}; the Editor renumbers them on open, which marks the asset dirty",
                });
            }

            foreach (var warning in asset.Warnings.Where(w => w.StartsWith("dangling", StringComparison.Ordinal)).Distinct())
                findings.Add(new Finding { Severity = "error", Where = "graph", Message = warning });

            return findings;
        }
    }
}
