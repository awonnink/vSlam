using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Control3DNodeObject : SlamObject
    {
        public VSlamHL.TreeView3D TreeView3D = null;

        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            if(TreeView3D!=null)
            {
                TreeView3D.SelectItem(this.gameObject);
            }
            base.DoSelect(position, true);
        }
        public override void StartGaze()
        {
            base.StartGaze();
        }
        public override void EndGaze()
        {
            base.EndGaze();
        }
    }
}
