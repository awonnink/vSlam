using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class ServerObjectTransform
    {
        public Vector3Json Position { get; set; }
        public QuaternionJson Rotation { get; set; }
        public Vector3Json Velocity { get; set; }
        public string Guid { get; set; }
        public Vector3Json AngularVelocity { get; set; }
    }
}
