using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class MeshContainer
    {
        public Mesh Mesh { get; set; }
        public IndexedFaceSet IndexedFaceSet { get; set; }
        public GameObject GameObject { get; set; }
        public int Counter { get; set; }
    }
}
