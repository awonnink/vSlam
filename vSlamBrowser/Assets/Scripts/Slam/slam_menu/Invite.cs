using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{
    public class Invite : SlamObject
    {
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Slam.Instance.InviteAFriend();
        }
         public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Invite a friend to this site");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}
