using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class ClickableTextSpec
    {
        public ClickableTextSpec()
        {
            Target = Target._blank;
        }
        public string Text { get; set; }
        public string Tooltip { get; set; }
        public string Href { get; set; }
        public Target Target { get; set; }

        public static ClickableTextSpec FromX3dParseNode(X3DParse.Node node)
        {
            ClickableTextSpec spec = new ClickableTextSpec();
            if (node != null)
            {
                foreach (var child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "text":
                            spec.Text = child.Value;
                            break;
                        case "slm:href":
                        case "href":
                            spec.Href = child.Value;
                            break;
                        case "slm:tooltip":
                        case "tooltip":
                            spec.Tooltip = child.Value;
                            break;
                        case "slm:target":
                        case "target":
                            Target t = Target._blank;
                            Statics.TryGetTarget(child.Value, out t);
                            spec.Target = t;
                            break;
                    }
                }
            }
            return spec;
        }
    }
}
