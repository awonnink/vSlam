using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class TimedGameObject
    {
        public TimedGameObject(GameObject gameObject, string dissGuid)
        {
            GameObject = gameObject;
            DateTime = DateTime.Now;
            DissonanceGuid = dissGuid;
        }
        public GameObject GameObject { get; set; }
        public DateTime DateTime { get; set; }
        public string DissonanceGuid { get; set; }
        public string AvatarNo { get; set; }
    }
}
