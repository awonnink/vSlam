using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Record : SlamObject
    {
        public Material recordMat = null;
        public Material recordingMat = null;
        public override void Start()
        {
            base.Start();
            recordMat = Resources.Load<Material>("Slam/Material/record");
            recordingMat = Resources.Load<Material>("Slam/Material/recording");
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            var rec = Slam.Instance.Record();
            switch (rec)
            {
                case RecordingState.Recording:
                case RecordingState.RecordingStopped:
                    Renderer renderer = GetComponent<Renderer>();
                    string textureName = Slam.Instance.RecordActions ? "Slam/recording":"Slam/record"  ;
                    renderer.material.mainTexture = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
                    Slam.Instance.CloseMenu(0.5f);
                    break;
                case RecordingState.NotSupported:
                    SlamMenu.Instance.SetHelpText("Recording here not possible");
                    break;
                case RecordingState.Listening:
                    SlamMenu.Instance.SetHelpText("Someone else already started recording");
                    break;
            }
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Toggle action recording");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}
