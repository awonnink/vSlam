using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    public class TextureContainer
    {
        public TextureContainer()
        {
            GameObjects = new List<GameObject>();
        }
        public Texture Texture { get; set; }
        public List<GameObject> GameObjects { get; set; }
    }
}
