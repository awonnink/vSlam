using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DMeta : X3DControl
    {
        public X3DMeta()
        {

        }
        public override void Render(StringBuilder sb)
        {
            TagName="Meta";
            AddProperty("name", MetaType.ToString());
            AddProperty("content", Content);
            base.Render(sb);
        }
        public MetaType MetaType { get; set; }
        public string Content { get; set; }

    }
   
 
   public enum MetaType
   {
        none,
        keywords,
        page,
        filter,
        singleuser,
        allowrecording,
        autoStartRecord
    }
}
