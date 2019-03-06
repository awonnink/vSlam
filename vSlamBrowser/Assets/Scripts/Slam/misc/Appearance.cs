using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class Appearance
    {
        public Appearance()
        {
            //Material = new MaterialSettings();
            ImageTexture = new ImageTexture();
            Movement = new Movement();
        }
        public MaterialSettings Material { get; set; }
        public LineProperties LineProperties { get; set; }
        public ImageTexture ImageTexture { get; set; }
        public MovieTexture MovieTexture { get; set; }
        public Movement Movement { get; set; }

    }
}
