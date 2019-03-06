using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Slam
{
    public class SmallMenu : SlamObject
    {
        public float y = 1.2f;
        public float hDis = 1.2f;
        
        public override void Start()
        {
            base.Start();
           // ToolTip = "Go Home";
        }
        public override void Update()
        {
            base.Update();
            float x = Camera.main.transform.forward.x* hDis;
            float z= Camera.main.transform.forward.z * hDis;
            Vector3 lerpPosition = Camera.main.transform.position + new Vector3(x, y, z) ;// - new Vector3(4*Camera.main.transform.forward.x, -1, 2*Camera.main.transform.forward.z);
            if((transform.position-lerpPosition).magnitude>0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, lerpPosition, Time.deltaTime);
            }
        }


    }
 
}
