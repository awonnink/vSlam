using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class CopyPaste : SlamObject
    {
        public bool Paste = false;
        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            base.DoSelect(position);
            Slam.Instance.CopyPaste(Paste);
            if (!Paste)
            {
                Slam.Instance.CloseMenu(0.5f);
            }
        }
        public override void StartGaze()
        {
            base.StartGaze();
            var msg = Paste ? "Paste from clipboard" : "Copy to clipboard";
            SlamMenu.Instance.SetHelpText(msg);
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }
    }
}
