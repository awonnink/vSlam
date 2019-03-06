using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class GoHomeMenu : SlamObject
    {

        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            base.DoSelect(v);
            Slam.Instance.GoHome();
        }
        // Update is called once per frame
    }
}
