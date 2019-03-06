using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class SetAsHome : SlamObject
    {
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            SlamMenu.Instance.SetAsHome();
            Slam.Instance.CloseMenu(0.01f);
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Set current page temporarily as home");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }

    }
}
