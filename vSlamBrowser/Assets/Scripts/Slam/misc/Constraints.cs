using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class Constraint
    {
        public Constraint()
        {
            Speed = 1;
            Enabled = true;
            ConstraintType = ConstraintType.notset;
        }
        public ConstraintType ConstraintType { get; set; }
        public string TargetName { get; set; }
        public GameObject Target { get; set; }
        public SlamObject TargetSO { get; set; }
        public Vector3 Borders { get; set; }
        public float Range { get; set; }
        public float Speed { get; set; }
        public bool Enabled { get; set; }
    }

}
