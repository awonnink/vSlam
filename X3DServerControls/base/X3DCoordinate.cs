using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DCoordinate : X3DControl
    {
        public X3DCoordinate()
        {

            TagName = "Coordinate";

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("point", Point);
            base.Render(sb);
        }
        public string Point { get; set; }
    }
}
