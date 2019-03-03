using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DAssetbundle : X3DControl
    {
        public X3DAssetbundle()
        {
        }
        public override void Render(StringBuilder sb)
        {

            TagName="slm:assetbundle";
            AddProperty("name", Name);
            AddProperty("url", Url);
            AddProperty("url", Url);
            AddProperty("sizeinbytes", SizeInBytes);


            base.Render(sb);
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public long? SizeInBytes { get; set; }
        
    }
   
}
