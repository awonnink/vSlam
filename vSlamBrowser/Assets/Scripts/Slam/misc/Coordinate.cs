using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class Coordinate
    {
        public string Point { get; set; }
        public Coordinate Clone()
        {
            Coordinate coordinate = new Coordinate();
            coordinate.Point = Point;
            return coordinate;
        }
    }
}
