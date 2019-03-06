using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class MsgboxButton : SlamObject
    {

        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            base.DoSelect(position);
            Slam.Instance.ShowMessage(false);
        }

    }
}
