using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{
    public class MicOnOff : SlamObject
    {
        Renderer r = null;
        void Awake()
        {
            r = GetComponent<Renderer>();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            if(Slam.Instance.micMute)
            {
                 Slam.Instance.StartSpeak();
            }
            else
            {
                Slam.Instance.StopSpeak();
           }
        }
        public void CheckTexture(bool micMute)
        {
            if (r!=null)
            {
                string textureName = micMute ? "slam_mic_off" :"slam_mic_on" ;
                r.material.mainTexture = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
            }
        }
    }
}
