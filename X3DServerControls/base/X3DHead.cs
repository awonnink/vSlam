using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DHead : X3DControl
    {
        public X3DHead()
        {
            MetaTags = new List<X3DMeta>();
        }
        public override void Render(StringBuilder sb)
        {

            TagName="Head";
            foreach(var m in MetaTags)
            {
                AddChild(m);
            }

            base.Render(sb);
        }
        public List<X3DMeta> MetaTags { get; set; }

    }
   
}
