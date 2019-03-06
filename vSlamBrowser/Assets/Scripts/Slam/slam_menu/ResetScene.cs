using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{
    public class ResetScene : SlamObject
    {
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
#if UNITY_WSA && !UNITY_EDITOR
//            HoloToolkit.Unity.WorldAnchorManager.Instance.RemoveAllAnchors();
#endif
            Slam.Instance.CloseMenu(0.1f);
            Slam.Instance.UrlHandler.HomeUrl = null;
            //Slam.Instance.R
            Slam.Instance.GoHome();
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Reset Scene");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
 
    }
}
