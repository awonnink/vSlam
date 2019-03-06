using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    class Pair
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public Pair(int first, int second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
