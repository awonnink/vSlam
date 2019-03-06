using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class MaterialSettings
    {
        public bool Define { get; set; }
        public bool Occluded { get; set; }
        public string Name { get; set; }
        public string DiffuseColor { get; set; }
        public string SpecularColor { get; set; }
        public string EmissiveColor { get; set; }
        public string AmbientIntensity { get; set; }
        public string Shininess { get; set; }
        public string Transparency { get; set; }
        public string TextureScale { get; set; }
        public string Physics { get; set; }
    }
}
