using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DGroup:X3DControl
    {
        public X3DGroup()
        {

            TagName = "Group";

        }
        public override void Render(StringBuilder sb)
        {
            base.Render(sb);
        }
    }
}
