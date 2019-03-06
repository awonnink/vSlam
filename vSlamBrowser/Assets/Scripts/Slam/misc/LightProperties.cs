using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class LightProperties
    {
        public LightProperties()
        {
            Color = Color.white;
        }
        public Vector3? Location { get; set; }
        public Vector3? Direction { get; set; }
        public Quaternion? Rotation { get; set; }
        public float? ShadowStrength { get; set; }
        public float? Intensity { get; set; }
        public float? CutOffAngle { get; set; }
        public float? Range { get; set; }
        public Color Color { get; set; }
    }
}
