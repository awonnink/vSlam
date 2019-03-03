using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DTextureCoordinate : X3DControl
    {
        public X3DTextureCoordinate()
        {

            TagName = "TextureCoordinate";

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("point", Point);
            base.Render(sb);
        }
        public string Point { get; set; }
 
    }
}
