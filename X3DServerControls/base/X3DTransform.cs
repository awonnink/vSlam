using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DTransform : X3DControl
    {
        public X3DTransform():base()
        {
            //          <Transform rotation='0 1 0 3' scale='0.3 0.3 0.3' position='0 1.1 0'>
            TagName = "Transform";
            //Shape = new X3DShape();
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("rotation", Quaternion.ToString( Rotation));
            AddProperty("slm:eulerrotation", Vector3.ToString( EulerRotation));
            AddProperty("position", Vector3.ToString(Position));
            AddProperty("translation", Vector3.ToString(Translation));
            AddProperty("scale", Vector3.ToString(Scale));
            AddProperty("name", Name);
            AddChild(Shape);
            base.Render(sb);
        }
        private X3DShape shape;
        public X3DShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }
        public Quaternion Rotation { get; set; }
        public Vector3 EulerRotation { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Translation { get; set; }
        public Vector3 Scale { get; set; }
        public string Name { get; set; }
        public static X3DTransform AddTransFormWithShape(ShapeType shapeType, Vector3 position =null, Quaternion rotation=null, Vector3 scale=null)
        {
            X3DTransform transform = new X3DTransform();
            if (shapeType != ShapeType.Null)
            {
                transform.Shape = new X3DShape();
                transform.Shape.ShapeType = shapeType;
            }
            transform.Rotation = rotation;
            transform.Scale =  scale;
            transform.Position = position;
            return transform;

        }
    }
}
