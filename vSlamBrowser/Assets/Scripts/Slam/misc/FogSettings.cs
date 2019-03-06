using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Slam
{
    public class FogSettings
    {
        public FogSettings()
        {
            Active = false;
            Color = Color.gray;
            FogType = FogTypeEnum.exponential;
            Density = 0.02f;
            VisibilityRange = null;
        }
        public bool Active { get; set; }
        public Color Color { get; set; }
        public FogTypeEnum FogType { get; set; }
        public float Density { get; set; }
        public float? VisibilityRange { get; set; }
    }
}
