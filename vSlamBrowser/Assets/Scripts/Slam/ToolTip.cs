using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Slam
{
    public class ToolTip : Singleton<ToolTip>
    {
        TextMesh tm;
        Vector3 CameraOffSet;
        bool initialized = false;
        // Use this for initialization
        void Start()
        {
            tm = GetComponent<TextMesh>();
            CameraOffSet = new Vector3(0.5f, 0.37f, 2);
        }
        public void SetTip(string toolTip)//=
        {
            SetTip(toolTip, CameraOffSet);
        }
        void Initialize()
        {
            if(Slam.Instance.IsHoloLens())
            {
                transform.localScale = 0.4f * transform.localScale;
                initialized = true;
            }
        }
        public void SetTip(string toolTip, Vector3 cameraOffset)//=
        {
            if(!initialized)
            {
                Initialize();
            }
            CameraOffSet = cameraOffset;
            if(tm!=null)
            {
                tm.text = toolTip;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (tm != null && !string.IsNullOrEmpty(tm.text))
            {
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * CameraOffSet.z + Camera.main.transform.up*CameraOffSet.y+ Camera.main.transform.right*CameraOffSet.x;
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
