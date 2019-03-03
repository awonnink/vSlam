using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DScene:X3DControl
    {
        public X3DScene()
        {

            TagName = "Scene";

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("id", ID);
            if(BackGround!=null)
            {
                Children.Add(BackGround);
            }
            base.Render(sb);
        }
        public String ID { get; set; }
        public X3DBackGround BackGround { get; set; }
    }
}
