using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class Normal
    {
        public string Vector { get; set; }
        public Normal Clone()
        {
            Normal normal = new Normal();
            normal.Vector = Vector;
            return normal;
        }
    }
}
