using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class slam_key : SlamObject
    {
        public string Key;
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            base.DoSelect(v);
            SlamMenu.Instance.HandleKey(Key);
        }
    }
}
