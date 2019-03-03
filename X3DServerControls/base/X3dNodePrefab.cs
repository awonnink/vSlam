using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DNodePrefab:X3DControl
    {
        public X3DNodePrefab()
        {
            TagName = "NodePrefab";
            
        }
        public override void Render(StringBuilder sb)
        {
            AddChild(Prefab);
            base.Render(sb);
        }
        public X3DControl Prefab { get; set; }

    }
}
