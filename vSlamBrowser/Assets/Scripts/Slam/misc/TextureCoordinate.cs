using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class TextureCoordinate
    {
        public string Point { get; set; }
        public TextureCoordinate Clone()
        {
            TextureCoordinate textureCoordinate = new TextureCoordinate();
            textureCoordinate.Point = Point;
            return textureCoordinate;
        }
    }
}
