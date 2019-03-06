using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Mirror : MonoBehaviour
    {
        public Transform mirrorCam;
        Camera c;
        bool renderStarted = false;
        private void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            //if(c==null)
            //{
            //    var c = GetComponentInChildren<Camera>();
            //    if (!renderStarted &&c != null)
            //    {
            //        c.Render();
            //        renderStarted = true;
            //    }
            //}
            CalculateRotation();
        }
        void CalculateRotation()
        {
            if (Camera.main != null)
            {
                Vector3 dir = (Camera.main.transform.position - transform.position).normalized;
                var rot = Quaternion.LookRotation(dir);
                rot.eulerAngles = rot.eulerAngles-transform.eulerAngles ;
                mirrorCam.localRotation = rot;
            }
        }
    }
}
