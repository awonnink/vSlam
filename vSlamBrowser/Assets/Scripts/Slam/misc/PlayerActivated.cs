using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slam
{
    public class PlayerActivated
    {
        public PlayerActivated(Transform transform, PlayerActivatedType playerActivatedType, float distance)
        {
            Transform = transform;
            PlayerActivatedType = playerActivatedType;
            Distance = distance;
            Triggered = false;
        }
        public Transform Transform { get; set; }
        public PlayerActivatedType PlayerActivatedType { get; set; }
        public IPlayerActivatedArgs PlayerAtivatedArgs { get; set; }
        public bool Triggered { get; set; }
        public float Distance { get; set; }
    }
}
