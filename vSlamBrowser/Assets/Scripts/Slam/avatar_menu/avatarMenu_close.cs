using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class avatarMenu_close : SlamObject
    {

        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            Slam.Instance.CloseAvatarMenu();
        }

    }
}
