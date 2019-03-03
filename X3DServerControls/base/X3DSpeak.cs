using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DSpeak : X3DControl
    {
        public X3DSpeak()
        {
            Voice = Voice.Default;
        }
        public override void Render(StringBuilder sb)
        {
            TagName= "slm:speak";
            AddProperty("voice", Voice.ToString());
            AddProperty("text", Text);
            AddProperty("distance", Distance);
            base.Render(sb);
        }
        public Voice Voice { get; set; }
        public string Text { get; set; }
        public float? Distance { get; set; }

    }


    public enum Voice
   {
        Default,
        David,
        Mark,
        Zira
    }
}
