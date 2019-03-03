using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DNode:X3DControl
    {
        public X3DNode()
        {
            TagName = "Node";
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("name", Name);
            AddProperty("text", Text);
            AddProperty("href", HRef);
            AddProperty("tooltip", ToolTip);
            if (Target != Target.None)
            {
                AddProperty("target", Target.ToString());
            }
            AddChild(NodePrefab);
            foreach (var node in Nodes)
            {
                Children.Add(node);
            }
            base.Render(sb);
        }
        public string Name { get; set; }
        public string Text { get; set; }
        public string HRef { get; set; }
        public string ToolTip { get; set; }
        public Target Target { get; set; }
        public X3DNodePrefab NodePrefab { get; set; }
        public List<X3DNode> Nodes { get; set; } = new List<X3DNode>();
    }
}
