using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class IndexedFaceSet
    {
        public IndexedFaceSet()
        {
            MultiTextureCoordinate = new List<TextureCoordinate2D>();
            //Normal = new Normal();
            //Coordinate = new Coordinate();
            //TextureCoordinate = new TextureCoordinate();
        }
        public string Index { get; set; }
        public string CoordIndex { get; set; }
        public string TextCoordIndex { get; set; }
        public Normal Normal { get; set; }
        public Coordinate Coordinate { get; set; }
        public TextureCoordinate TextureCoordinate { get; set; }
        public List<TextureCoordinate2D> MultiTextureCoordinate { get; set; }

        public IndexedFaceSet Clone()
        {
            IndexedFaceSet clone = new IndexedFaceSet();
            clone.Index = Index;
            clone.CoordIndex = CoordIndex;
            clone.TextCoordIndex = TextCoordIndex;
            clone.Normal = Normal == null ? null : Normal.Clone();
            clone.Coordinate = Coordinate == null ? null : Coordinate.Clone();
            clone.TextureCoordinate = TextureCoordinate == null ? null : TextureCoordinate.Clone();
            foreach (var text2d in MultiTextureCoordinate)
            {
                clone.MultiTextureCoordinate.Add(text2d.Clone());
            }
            return clone;
        }
    }
}
