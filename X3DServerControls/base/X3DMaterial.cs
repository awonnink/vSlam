using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DMaterial : X3DControl
    {
        public X3DMaterial()
        {
            //<Material DEF='MaterialFloor' diffuseColor='0.1 0.1 0.1' textureScale='6 6'/>

            TagName = "Material";

        }
        public X3DMaterial(string name, 
            Vector3 diffuseColor,
            double? transparancy=null,
            Vector3 emmisiveColor = null,
             Vector3 specularColor = null,
             double? ambientIntensity = null,
             double? shininess = null
           )
        {
            TagName = "Material";
            DEF = name;
            DiffuseColor = diffuseColor;
            Transparency = transparancy;
            EmissiveColor = emmisiveColor;
            SpecularColor = specularColor;
            AmbientIntensity = ambientIntensity;
            Shininess = shininess;

        }
        public override void Render(StringBuilder sb)
        {
            if(string.IsNullOrWhiteSpace(DEF)&& string.IsNullOrWhiteSpace(USE) && DiffuseColor!=null)
            {
                DEF = "Mat" + Guid.NewGuid().ToString();
            }
            AddProperty("DEF", DEF);
            AddProperty("USE", USE);
            AddProperty("diffuseColor", Vector3.ToString( DiffuseColor));
            AddProperty("emissiveColor", Vector3.ToString(EmissiveColor));
            AddProperty("specularColor", Vector3.ToString(SpecularColor));
            AddProperty("textureScale",Vector2.ToString( TextureScale));
            AddProperty("transparency", Transparency == null ? null : Transparency.ToString());
            AddProperty("ambientIntensity", AmbientIntensity == null ? null : AmbientIntensity.ToString());
            AddProperty("shininess", Shininess == null ? null : Shininess.ToString());
            AddProperty("occluded", Occluded.ToSlamString());
            base.Render(sb);
        }
        public string DEF { get; set; }
        public string USE { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 EmissiveColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public Vector2 TextureScale { get; set; }
        public double? Transparency { get; set; }
        public double? AmbientIntensity { get; set; }
        public double? Shininess { get; set; }
        public bool? Occluded { get; set; }
    }
}
