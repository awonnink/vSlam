using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Favorite : SlamObject
    {
        public override void Start()
        {
            base.Start();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Slam.Instance.AddToFavorites();
            Slam.Instance.CloseMenu(0.01f);
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Add current site to favorites");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }

    }
}
