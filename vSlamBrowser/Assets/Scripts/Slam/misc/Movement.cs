using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class Movement
    {
        public Movement()
        {
            ColliderType = ColliderType.NotSet;
            Constraints = new List<Constraint>();
        }
        public string Rotation { get; set; }
        public string Center { get; set; }
        public string ApplyToParent { get; set; }
        public float Presentation { get; set; }
        public float PresentationAngle { get; set; }
        public float PresentationSpeed { get; set; }
        public float Kinetic { get; set; }
        public ColliderType ColliderType { get; set; }
        public float Bounciness { get; set; }
        public string ClingToCamera { get; set; }
        public string ActiveTransform { get; set; }
        public bool? HandDraggable { get; set; }
        public bool? HandRotatable { get; set; }
        public bool? Updatable { get; set; }
        public bool? ServerObject { get; set; }
        public List<Constraint> Constraints { get; set; }

    }
}
