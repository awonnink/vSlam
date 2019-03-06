using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slam
{
    public class HandDraggablePrep
    {
        public HandDraggablePrep(Transform parent, GameObject gameObject, bool handDragable, bool handRotatable)
        {
            Parent = parent;
            GameObject = gameObject;
            IsHandDragable = handDragable;
            IsHandRotatable = handRotatable;
        }
        public Transform Parent { get; set; }
        public GameObject GameObject { get; set; }
        public bool IsHandDragable { get; set; }
        public bool IsHandRotatable { get; set; }
    }
}
