using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Like : SlamObject
    {
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Slam.Instance.Like();
            Slam.Instance.CloseMenu(2.0f);
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Like current site");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}
