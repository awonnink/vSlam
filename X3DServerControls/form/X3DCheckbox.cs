using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DCheckBox : X3DControl
    {
        bool _tristate = true;
        public X3DCheckBox(string name, bool triState=true)
        {
            Name = name;
            _tristate = triState;
            CheckBoxState = _tristate ? CheckBoxState.Undetermined:CheckBoxState.UnChecked;
        }
        public override void Render(StringBuilder sb)
        {
            TagName = "Transform";
            Children.Clear();
            AddProperty("rotation", Quaternion.ToString( Rotation));
            AddProperty("slm:eulerrotation", Vector3.ToString( EulerRotation));
            AddProperty("position", Vector3.ToString(Position));
            AddProperty("translation", Vector3.ToString(Translation));
            AddProperty("scale", Vector3.ToString(Scale));
            AddProperty("name", Name);
            //X3DShape shapeud = null;
            //X3DShape shapeuc = null;
            //X3DShape shapec = null;
            ShapeUndetermined.Name = Name + "_" + CheckBoxState.Undetermined.ToString();
            ShapeUnChecked.Name = Name + "_" + CheckBoxState.UnChecked.ToString();
            ShapeChecked.Name = Name + "_" + CheckBoxState.Checked.ToString();
            ShapeUndetermined.Url = "switch:" + ShapeChecked.Name;
            ShapeUnChecked.Url = "switch:" + (_tristate? ShapeUndetermined.Name:ShapeChecked.Name);
            ShapeChecked.Url = "switch:" + ShapeUnChecked.Name;
            Children.Add(ShapeUndetermined);
            Children.Add(ShapeUnChecked);
            Children.Add(ShapeChecked);

            switch (CheckBoxState)
            {
                case CheckBoxState.Undetermined:
                    ShapeChecked.Hidden=true;
                    ShapeUnChecked.Hidden = true;
                    break;
                case CheckBoxState.UnChecked:
                    ShapeUndetermined.Hidden = true;
                    ShapeChecked.Hidden = true;
                    break;
                case CheckBoxState.Checked:
                    ShapeUndetermined.Hidden = true;
                    ShapeUnChecked.Hidden = true;
                    break;
            }
 
            base.Render(sb);
        }
        public string Name { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 EulerRotation { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Translation { get; set; }
        public Vector3 Scale { get; set; }
        public CheckBoxState CheckBoxState { get; set; }
        public X3DShape ShapeUndetermined { get; set; }
        public X3DShape ShapeUnChecked { get; set; }
        public X3DShape ShapeChecked { get; set; }
    }
    public enum CheckBoxState
    {
        Undetermined,
        UnChecked,
        Checked
    }
   
 
  
}
