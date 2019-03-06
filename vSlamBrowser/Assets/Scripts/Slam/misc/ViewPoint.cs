using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class ViewPoint
    {
        public ViewPoint()
        {
            Position = Vector3.zero;
        }
        public Vector3 Position { get; set; }
    }
}
