using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Slam
{
    public class SmallMenuGoHome : SlamObject
    {

        public override void Start()
        {
            base.Start();
            ToolTip = "Go Home";
        }
        public override void Update()
        {
            base.Update();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            base.DoSelect(v);
            Slam.Instance.GoHome();
        }

    }

}
