using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public static class Extensions
    {
        public static void SetTheParent(this Transform child, Transform parent)
        {
            Vector3 pos = child.position;
            Quaternion rot = child.rotation;
            Vector3 scale = child.localScale;
            child.parent = parent;
            child.localPosition = pos;
            child.localRotation = rot;
            child.localScale = scale;
        }
    }
}
