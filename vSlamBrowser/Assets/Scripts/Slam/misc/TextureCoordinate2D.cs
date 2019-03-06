using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class TextureCoordinate2D
    {
        public string Point { get; set; }
        public TextureCoordinate2D Clone()
        {
            TextureCoordinate2D textureCoordinate2D = new TextureCoordinate2D();
            textureCoordinate2D.Point = Point;
            return textureCoordinate2D;
        }
    }
}
