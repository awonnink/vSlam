using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class soundrecord : SlamObject
    {
        Renderer therenderer;
        string tooltipOff = "Say: start recording";
        string tooltipOn = "";
        public override void Start()
        {
            base.Start();
            therenderer = GetComponent<Renderer>();
            ToolTip = tooltipOff;
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Slam.Instance.ToggleSoundRecord();
         }
        public void SetRecordState(SoundRecordState soundRecordState)
        {
            string textureName = "Slam/soundrecordoff";
            switch (soundRecordState)
            {
                case SoundRecordState.Recording:
                    textureName = "Slam/soundrecordon";
                    ToolTip = tooltipOn;
                    break;
                case SoundRecordState.RecordingStopped:
                    ToolTip = tooltipOff;
                    break;
                case SoundRecordState.NotSupported:
                    SlamMenu.Instance.SetHelpText("Recording here not possible");
                    break;
            }
            therenderer.material.mainTexture = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;

        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Toggle sound recording");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}
