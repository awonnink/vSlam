using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DNormal : X3DControl
    {
        public X3DNormal()
        {

            TagName = "Normal";

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("vector", Vector);
            base.Render(sb);
        }
        public string Vector { get; set; }
    }
}
