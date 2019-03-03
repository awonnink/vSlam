using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DFontStyle : X3DControl
    {
        public X3DFontStyle()
        {

        }
        public override void Render(StringBuilder sb)
        {
            TagName="FontStyle";
            if (Justify != Justify.NONE)
            {
                AddProperty("justify", Justify.ToString());
                if(Size!=null)
                {
                    AddProperty("size", Size.ToSlamString());

                }
                base.Render(sb);
            }

        }

        public Justify Justify { get; set; }
        public double? Size { get; set; }
    }



}
