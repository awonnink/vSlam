using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class ShapeProperties
    {
        public ShapeProperties()
        {
            Target = Target._blank;
        }
        public string Href { get; set; }
        public Target Target { get; set; }
        public string FaceCamera { get; set; }
        public int? WalkFloor { get; set; }
        public int? SitPlane { get; set; }
        public string ToolTip { get; set; }
        public int? Favorite { get; set; }
        public int? History { get; set; }
        public float? Radius { get; set; }
        public float? BottomRadius { get; set; }
        public float? Height { get; set; }
        public bool Hidden { get; set; }
    }
}
