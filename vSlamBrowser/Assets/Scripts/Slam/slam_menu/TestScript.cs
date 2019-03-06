using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{
    public class TestScript : SlamObject
    {
        Renderer r = null;
        void Awake()
        {
            r = GetComponent<Renderer>();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Slam.Instance.Test();

        }
        public void CheckTexture(bool micMute)
        {
            if (r!=null)
            {
            }
        }
    }
}
