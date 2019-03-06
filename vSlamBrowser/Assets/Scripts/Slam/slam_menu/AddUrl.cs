using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class AddUrl : SlamObject
    {
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            //SlamMenu.Instance.Parse();
            Slam.Instance.SuggestUrl();
            //Slam.Instance.CloseMenu(0.01f);
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Suggest new URL to v-Slam search");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }

    }
}
