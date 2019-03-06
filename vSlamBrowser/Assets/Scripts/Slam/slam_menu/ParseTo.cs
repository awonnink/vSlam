using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{
    public class ParseTo : SlamObject
    {

        public override void Start()
        {
            base.Start();
            ToolTip = "Say: "+ Constants.Speech_Submit_Text;
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            SlamMenu.Instance.Parse();
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Enter");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}