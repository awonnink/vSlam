using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class sceneRotator : MonoBehaviour
    {
        public float hDis = 1;
        // Use this for initialization
        void Start()
        {
            float hDis =  1.2f *Slam.Instance.DeviceManager.MainMenuDistance;
        }

        // Update is called once per frame
        void Update()
        {
                CheckPosition();
 
        }

        void CheckPosition(bool init=false)
        {
            //float x = Camera.main.transform.forward.x * hDis;
            //float z = Camera.main.transform.forward.z * hDis;
            var lerpPosition = Camera.main.transform.position + hDis * Camera.main.transform.forward;// new Vector3(x, Camera.main.transform.position.y, z);// - new Vector3(4*Camera.main.transform.forward.x, -1, 2*Camera.main.transform.forward.z);
            if (init)
            {

                transform.position = lerpPosition;
            }
            else
            {
                if ((transform.position - lerpPosition).magnitude > 0.2f)
                {
                    transform.position = Vector3.Lerp(transform.position, lerpPosition, Time.deltaTime*0.2f);
                }
            }
        }
        public void Init()
        {
           // timer = 5;
            CheckPosition(true);
        }
    }
}
