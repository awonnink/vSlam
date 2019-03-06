using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class avatarMenuMute : SlamObject
    {
        public avatarMenu AvatarMenu;
        
        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            //base.DoSelect(position);
            if(AvatarMenu!=null)
            {
                if(AvatarMenu.ToggleMute())
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
 
    }
}