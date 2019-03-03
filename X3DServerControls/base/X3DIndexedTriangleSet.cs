using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DIndexedTriangleSet : X3DControl
    {
        public X3DIndexedTriangleSet()
        {

            TagName = "IndexedTriangleSet";

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("index", Index);
            AddChild(Coordinate);
            AddChild(TextureCoordinate);
            AddChild(Normal);
            base.Render(sb);
        }
        public string Index { get; set; }
        public X3DCoordinate Coordinate { get; set; }
        public X3DTextureCoordinate TextureCoordinate { get; set; }
        public X3DNormal Normal { get; set; }
    }
}
