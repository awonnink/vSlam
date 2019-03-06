using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Turn : SlamObject
    {
        CameraMovement camMovement;
        public float TurnSpeed=0;
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            if(camMovement==null && Slam.Instance.CameraTransForm!=null)
            {
                camMovement = Slam.Instance.CameraTransForm.GetComponent<CameraMovement>();//parent for MR camera
                if(camMovement!=null)
                {
                    camMovement.evaluateMouse = Slam.Instance.DeviceManager.Name == CallingDevices.PC.ToString();
                }
            }
            if (camMovement!=null)
            {
                //camMovement.deltaY += angle;
                var dis= Slam.Instance.SetSceneRotator(TurnSpeed!=0);
                camMovement.Rotate(TurnSpeed, dis);
            }
     
        }
    }
}
