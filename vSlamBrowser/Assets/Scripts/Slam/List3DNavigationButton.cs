using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class List3DNavigationButton : SlamObject
    {
        public IListView3D listbox;
        public bool reset;
        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            //base.DoSelect(position);
            if(listbox!=null)
            {
                listbox.MoveNext(reset);
                base.DoSelect(Vector3.zero, true);
            }
        }
    }
}
