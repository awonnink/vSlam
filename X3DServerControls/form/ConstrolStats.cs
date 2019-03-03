using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public static class ConstrolStats
    {
        public static bool TryGetTarget(string targetString, out Target target)
        {
            target = Target._Blank;
            if(Enum.TryParse<Target>(targetString, out target))
            {
                return true;
            }
            return false;
        }
        public static X3DCheckBox CreateCheckBox(CheckBoxState cbState, string fieldName, bool triState = true, string groupName = "form")
        {
            X3DCheckBox checkbox = new X3DCheckBox(fieldName, triState);
            checkbox.CheckBoxState = cbState;
            checkbox.ShapeChecked = CreateCheckBoxShape(CheckBoxState.Checked, fieldName);
            checkbox.ShapeUnChecked = CreateCheckBoxShape(CheckBoxState.UnChecked, fieldName);
            checkbox.ShapeUndetermined = CreateCheckBoxShape(CheckBoxState.Undetermined, fieldName);
            checkbox.Scale = new Vector3(0.04, 0.04, 0.04);
            return checkbox;
        }
        public static X3DShape CreateCheckBoxShape(CheckBoxState defaultState, string fieldName, string groupName="form")
        {
            var shape = new X3DShape();
            int answ = (int)defaultState;
            shape.ShapeType = ShapeType.Prefab;
            shape.Group = groupName;
            switch (defaultState)
            {
                case CheckBoxState.Undetermined:
                    shape.Item = "checkbox";
                    shape.Appearance.Material.DiffuseColor = new Vector3(0.3, 0.3, 0.3);
                    shape.Appearance.Material.DEF = "ud";
                    break;
                case CheckBoxState.UnChecked:
                    shape.Item = "unchecked";
                    shape.Appearance.Material.DiffuseColor = new Vector3(0.9, 0.1, 0.1);
                    shape.Appearance.Material.DEF = "uc";
                    break;
                case CheckBoxState.Checked:
                    shape.Item = "checked";
                    shape.Appearance.Material.DiffuseColor = new Vector3(0.1, 0.9, 0.1);
                    shape.Appearance.Material.DEF = "ch";
                    break;
            }
            shape.FormField = fieldName;
            shape.FormValue = answ.ToString();
            return shape;
        }

    }
}
