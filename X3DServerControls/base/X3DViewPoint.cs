using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DViewPoint : X3DControl
    {
        public X3DViewPoint()
        {
            //      <Viewpoint DEF='ViewUpClose' centerOfRotation='0 -1 0' position='0 1 3'/>


            TagName = "ViewPoint";
            Position = new Vector3();

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("DEF", DEF);
            AddProperty("position", Vector3.ToString( Position));
            base.Render(sb);
        }
        public string DEF { get; set; }
        public Vector3 Position { get; set; }
    }
}
