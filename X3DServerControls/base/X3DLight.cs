using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DLight : X3DControl
    {
        public X3DLight()
        {
            TagName = "PointLight";
            LightType = LightType.PointLight;
    
        }
        public override void Render(StringBuilder sb)
        {
            //<PointLight DEF='point' on='TRUE' intensity='0.9000' color='0.64 0.65 0.5' location='-1.4 0.47 0.75' radius='5.0000' >  </PointLight>

            TagName = LightType.ToString();
            AddProperty("direction", Vector3.ToString( Direction));
            AddProperty("location", Vector3.ToString(Location));
            AddProperty("intensity", Intensity.ToSlamString());
            AddProperty("shadowIntensity", ShadowIntensity.ToSlamString());
            AddProperty("color", Vector3.ToString(Color));
            AddProperty("radius", Radius.ToSlamString());
            AddProperty("range", Range.ToSlamString());

            base.Render(sb);
        }
        public LightType LightType { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Location { get; set; }
        public double? Intensity { get; set; }
        public double? ShadowIntensity { get; set; }
        public Vector3 Color { get; set; }
        public double? Radius { get; set; }
        public double? Range { get; set; }
    }

    public enum LightType
    {
        PointLight,
        DirectionalLight,
        SpotLight
    }
   
}
