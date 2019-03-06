using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
namespace HoloToolkit.Unity
{
    public class MakeAxis : MonoBehaviour
    {
        private static List<string> axisNames = new List<string>();
        private static SerializedObject inputManagerAsset;

        // Use this for initialization
        void Start()
        {

        }

        [MenuItem("File/addAxis")]
        static void createAxisses()
        {
            inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object)));
            foreach (InputManagerAxis axis in newInputAxes)
            {
                if (!DoesAxisNameExist(axis.Name))
                {
                    AddAxis(axis);
                }
            }
        }
        private static void AddAxis(InputManagerAxis axis)
        {
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // Creates a new axis by incrementing the size of the m_Axes array.
            axesProperty.arraySize++;

            // Get the new axis be querying for the last array element.
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            // Iterate through all the properties of the new axis.
            while (axisProperty.Next(true))
            {
                switch (axisProperty.name)
                {
                    case "m_Name":
                        axisProperty.stringValue = axis.Name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.DescriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.DescriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.NegativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.PositiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.AltNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.AltPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.Gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.Dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.Sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.Snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.Invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.Type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.Axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.JoyNum;
                        break;
                }
            }
        }
        private static bool DoesAxisNameExist(string axisName)
        {
            if (axisNames.Count == 0 )
            {
                RefreshLocalAxesList();
            }

            return axisNames.Contains(axisName);
        }
        private static void RefreshLocalAxesList()
        {
            axisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }
        }
        private static readonly InputManagerAxis[] newInputAxes =
    {
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL,  Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL,    Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = AxisType.JoystickAxis, Axis = 2 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_SHARED_TRIGGER,               Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 3 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL, Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 4 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL,   Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = AxisType.JoystickAxis, Axis = 5 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_DPAD_HORIZONTAL,              Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 6 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_DPAD_VERTICAL,                Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 7 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_TRIGGER,           Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 9 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_TRIGGER,          Dead = 0.19f, Sensitivity = 1, Invert = false, Type = AxisType.JoystickAxis, Axis = 10 },

            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_A,                          PositiveButton = "joystick button 0", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_B,                          PositiveButton = "joystick button 1", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_X,                          PositiveButton = "joystick button 2", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.XBOX_Y,                          PositiveButton = "joystick button 3", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_BUMPER_OR_GRIP,  PositiveButton = "joystick button 4", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_BUMPER_OR_GRIP, PositiveButton = "joystick button 5", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_MENU,            PositiveButton = "joystick button 6", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_MENU,           PositiveButton = "joystick button 7", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_CLICK,     PositiveButton = "joystick button 8", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_CLICK,    PositiveButton = "joystick button 9", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
        };
   private class InputManagerAxis
    {
        public string Name = "";
        public string DescriptiveName = "";
        public string DescriptiveNegativeName = "";
        public string NegativeButton = "";
        public string PositiveButton = "";
        public string AltNegativeButton = "";
        public string AltPositiveButton = "";
        public float Gravity = 0.0f;
        public float Dead = 0.0f;
        public float Sensitivity = 0.0f;
        public bool Snap = false;
        public bool Invert = false;
        public AxisType Type = default(AxisType);
        public int Axis = 0;
        public int JoyNum = 0;
    }
    private enum AxisType
    {
        KeyOrMouseButton,
        MouseMovement,
        JoystickAxis
    };   }
 


}