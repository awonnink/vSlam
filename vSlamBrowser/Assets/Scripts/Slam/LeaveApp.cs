using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class LeaveApp : SlamObject
    {
        void Awake()
        {
            FaceCamera = "face";
        }
        public override void StartGaze()
        {
            Slam.Instance.HideLeaveApp(false);
        }
        public override void EndGaze()
        {
            Slam.Instance.HideLeaveApp(true);
        }
        public override void Update()
        {
            base.Update();
            CheckRayCast();
        }
        bool scaleReset = false;
        void CheckRayCast()
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, transform.position- Camera.main.transform.position);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                if(objectHit!=transform)
                {
                    var sta = GetComponent<HoloToolkit.Unity.SimpleTagalong>();
                    if(sta!=null)
                    {
                        var dist = Vector3.Distance(Camera.main.transform.position, hit.transform.position) * 0.7f;
                        if (dist > 0)
                        {
                            sta.TagalongDistance = dist;
                            if (sta.TagalongDistance < 2 && !scaleReset)
                            {
                                scaleReset = true;
                                transform.localScale *= dist / 2;
                            }
                        }
                    }
                }
            }
        }
    }
}
