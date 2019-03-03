using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DFog : X3DControl
    {
        public X3DFog()
        {
            FogType = FogType.exponential;
        }
        public override void Render(StringBuilder sb)
        {

            TagName="Fog";
            AddProperty("color", Vector3.ToString(Color));
            AddProperty("slm:density", Density);
            AddProperty("fogType", FogType.ToString());

            base.Render(sb);
        }
        public Vector3 Color { get; set; }
        public float? Density { get; set; }
        public FogType FogType { get; set; }

    }
    public enum FogType
    {
        linear,
        exponential,
        exponentialsquared
    }
}
