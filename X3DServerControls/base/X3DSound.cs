using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DSound : X3DControl
    {
        public X3DSound()
        {
            TagName = "Sound";
    
        }
        public override void Render(StringBuilder sb)
        {
            //<Sound>
            // < AudioClip url = '"sound/soft.wav"' loop = 'true' enabled = 'true' volume = '0.2' />
            //</ Sound >
            X3DControl audioclip = new X3DControl();
            audioclip.TagName = "AudioClip";
            AddChild(audioclip);
            audioclip.AddProperty("url", Url);
            audioclip.AddProperty("loop", Loop.ToSlamString());
            audioclip.AddProperty("slm:effect", Effect.ToSlamString());
            audioclip.AddProperty("enabled", Enabled.ToSlamString());
            audioclip.AddProperty("volume", Volume.ToSlamString());

            base.Render(sb);
        }
        public string Url { get; set; }
        public bool? Loop { get; set; }
        public bool? Enabled { get; set; }
        public bool? Effect { get; set; }
        public double? Volume { get; set; }
    }


}
