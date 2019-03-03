using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DMovieTexture : X3DControl
    {
        public X3DMovieTexture()
        {
            //<ImageTexture url='"images/marble.jpg"' />
            TagName = "MovieTexture";
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("url", Url);
            base.Render(sb);
        }
        public string Url { get; set; }
    }
}
