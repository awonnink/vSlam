using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DMovement : X3DControl
    {
        public X3DMovement()
        {
            //<slm:Movement rotate='0 1 0' />

            TagName = "slm:Movement";
            ApplyToParent = false;
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("rotate", Vector3.ToString( Rotate));
            AddProperty("center", Vector3.ToString(Center));
            AddProperty("clingtocamera", ClingToCamera);
            AddProperty("activetransform", ActiveTransform);
            AddProperty("presentation", Presentation);
            AddProperty("presentationangle", PresentationAngle);
            AddProperty("presentationspeed", PresentationSpeed);
            AddProperty("kinetic", Kinetic);
            AddProperty("bounciness", Bounciness);
            AddProperty("applytoparent", ApplyToParent.ToSlamString());
            AddProperty("handdraggable", HandDraggable.ToSlamString());
            AddProperty("updatable", Updatable.ToSlamString());
            base.Render(sb);
        }
        public Vector3 Rotate { get; set; }
        public Vector3 Center { get; set; }
        public string ClingToCamera { get; set; }
        public string ActiveTransform { get; set; }
        public double? Presentation { get; set; }
        public double? PresentationAngle { get; set; }
        public double? PresentationSpeed { get; set; }
        public double? Kinetic { get; set; }
        public double? Bounciness { get; set; }

        public bool? ApplyToParent { get; set; }
        public bool? HandDraggable { get; set; }
        public bool? Updatable { get; set; }
    }
}
