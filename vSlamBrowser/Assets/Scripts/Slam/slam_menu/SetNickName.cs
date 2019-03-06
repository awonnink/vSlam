using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class SetNickName : SlamObject
    {
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            SlamMenu.Instance.SetInputText(Slam.Instance.NickName, true, MenuInputType.NickName);
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Set your nickname");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }

    }
}
