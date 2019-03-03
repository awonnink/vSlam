using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DBackGround : X3DControl
    {
        public X3DBackGround()
        {
            TagName = "Background";
    
        }
        public override void Render(StringBuilder sb)
        {

            AddProperty("DEF", DEF);
            AddProperty("Name", Name);
            AddProperty("skyBox", SkyBox);
            AddProperty("groundColor", Vector3.ToString( GroundColor));
            AddProperty("skyColor", Vector3.ToString(SkyColor));
            AddProperty("slm:lightingColor", LightingColor);
            if (LightingMode != LightingMode.NotSet)
            {
                AddProperty("slm:lightingSource", LightingMode.ToString());
            }
            if (ReflectionMode != ReflectionMode.NotSet)
            {
                AddProperty("slm:reflectionMode", ReflectionMode.ToString());
            }
            base.Render(sb);
        }
        public string DEF { get; set; }
        public string Name { get; set; }
        public string SkyBox { get; set; }
        public Vector3 GroundColor { get; set; }
        public Vector3 SkyColor { get; set; }
        public LightingMode LightingMode { get; set; }
        public ReflectionMode ReflectionMode { get; set; }
        public string LightingColor { get; set; }
    }
    public enum LightingMode
    {
        NotSet,
        SkyBox,
        Gradient,
        Color
    }
    public enum ReflectionMode
    {
        NotSet,
        SkyBox,
        Custom,
    }
    }
