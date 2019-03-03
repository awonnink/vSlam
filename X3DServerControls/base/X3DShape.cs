using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DShape : X3DControl
    {
        public X3DShape()
        {
            TagName = "Shape";
            ShapeType = ShapeType.Sphere;
            Target = Target.None;
            Appearance = new X3DAppearance();
            FontStyle=new X3DFontStyle();
            FaceCamera = FaceCamera.None;
        }
        public override void Render(StringBuilder sb)
        {
            X3DControl shape = new X3DControl();// <Cube  slm:href='holochatspace/holochatspace.x3d' slm:target='_blank'/>
            string prefix = "";

            var tagName = ShapeType.ToString();
            if (ShapeType == ShapeType.Prefab|| ShapeType== ShapeType.Empty)
            {
                prefix = "slm:";
            }
            shape.TagName = prefix + tagName;
            shape.AddProperty("name", Name);
            shape.AddProperty("group", Group);
            shape.AddProperty("bundle", Bundle);
            shape.AddProperty("item", Item);
            shape.AddProperty("slm:href", Url);
            shape.AddProperty("slm:formfield", FormField);
            shape.AddProperty("slm:formvalue", FormValue);
            shape.AddProperty("slm:walkfloor", WalkFloor.ToSlamString());
            shape.AddProperty("length", RectLength.ToSlamString());
            shape.AddProperty("height", RectHeight.ToSlamString());
            shape.AddProperty("slm:favorite", Favorite.ToSlamString());
            shape.AddProperty("slm:history", History.ToSlamString());
            shape.AddProperty("slm:toolTip", ToolTip);
            if (Target != Target.None)
            {
                shape.AddProperty("slm:target", Target.ToString());
            }
            shape.AddProperty("slm:hidden", Hidden.ToSlamString());
            if (ShapeType== ShapeType.Text)
            {
                shape.AddProperty("string", Text);
                if (InputType != InputType.None)
                {
                    shape.AddProperty("slm:input", InputType.ToString());
                }
            }
            switch(FaceCamera)
            {
                case FaceCamera.face:
                    shape.AddProperty("slm:facecamera", "face");
                    break;
                case FaceCamera.back:
                    shape.AddProperty("slm:facecamera", "back");
                    break;
                case FaceCamera.back_lock_y:
                    shape.AddProperty("slm:facecamera", "back/lock-y");
                    break;
                case FaceCamera.face_lock_y:
                    shape.AddProperty("slm:facecamera", "face/lock-y");
                    break;
                case FaceCamera.parent_back:
                    shape.AddProperty("slm:facecamera", "parent/back");
                    break;
                case FaceCamera.parent_face:
                    shape.AddProperty("slm:facecamera", "parent/face");
                    break;
                case FaceCamera.parent_back_lock_y:
                    shape.AddProperty("slm:facecamera", "parent/back/lock-y");
                    break;
                case FaceCamera.parent_face_lock_y:
                    shape.AddProperty("slm:facecamera", "parent/face/lock-y");
                    break;
            }
            AddChild(shape);
            AddChild(Appearance);
            AddChild(IndexedTriangleSet);
            shape.AddChild(FontStyle);
            base.Render(sb);
        }
        public X3DAppearance Appearance { get; set; }
        public X3DIndexedTriangleSet IndexedTriangleSet { get; set; }
        public ShapeType ShapeType { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Bundle { get; set; }
        public string Item { get; set; }
        public string Url { get; set; }
        string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (text != null && !text.Contains("\""))
                {
                    text = "\"" + text + "\"";
                }
            }
        }
        public InputType InputType { get; set; }
        public double? RectLength { get; set; }
        public double? RectHeight { get; set; }
        public string ToolTip { get; set; }
        public string FormField { get; set; }
        public string FormValue { get; set; }
        public int? Favorite { get; set; }
        public int? History { get; set; }
        public double? WalkFloor { get; set; }
        public FaceCamera FaceCamera { get; set; }
        public X3DFontStyle FontStyle { get; set; }
        public Target Target { get; set; }
        public bool? Hidden { get; set; }
    }
    public enum InputType
    {
        None,
        Text,
        Password,
        Date,
        DateTime,
        Number,
        Community
    }
    public enum FaceCamera
    {
        None,
        face,
        back,
        face_lock_y,
        back_lock_y,
        parent_face,
        parent_back,
        parent_face_lock_y,
        parent_back_lock_y,

    }
    public enum Justify
    {
        NONE,
        LEFT,
        MIDDLE,
        RIGHT,
        TOPLEFT,
        TOPRIGHT,
        TOPMIDDLE
    }
    public enum ShapeType
    {
        Cube,
        Sphere,
        Text,
        Capsule,
        Plane,
        Prefab,
        Empty,
        Cylinder,
        Null
    }
    public enum Target
    {
        None,
        _Blank,
        _2D,
        _Self,
        _Inline
    }
}
