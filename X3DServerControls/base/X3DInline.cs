using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DInline : X3DControl
    {
        public X3DInline()
        {

        }
        public override void Render(StringBuilder sb)
        {
            TagName="Inline";
            AddProperty("url", Url);
            AddProperty("clearparent", ClearParent);
            AddProperty("updatable", Updatable);
            base.Render(sb);
        }
       
        public string Url { get; set; }
        public bool? ClearParent { get; set; }
        public bool? Updatable { get; set; }

    }



}
