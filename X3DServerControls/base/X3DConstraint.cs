using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DConstraint : X3DControl
    {
        public X3DConstraint()
        {
            //<ImageTexture url='"images/marble.jpg"' />
            TagName = "Constraint";
            ConstraintType = ConstraintType.sphere;
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("target", TargetName);
            AddProperty("range", Range);
            AddProperty("speed", Speed);
            AddProperty("borders", Vector3.ToString(Borders));
            AddProperty("type", ConstraintType.ToString());
            base.Render(sb);
        }
        public string TargetName { get; set; }
        public float? Range { get; set; }
        public float? Speed { get; set; }
        public Vector3 Borders { get; set; }
        public ConstraintType ConstraintType { get; set; }
    }
    public enum ConstraintType
    {
        notset,
        sphere,
        block,
        scale,
        delete,
        snap,
        constraintgroup,
        scaledsphere,
        selectfar,
        selectclose,
        move

    }

}
