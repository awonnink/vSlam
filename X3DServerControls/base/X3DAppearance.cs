using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DAppearance : X3DControl
    {
        public X3DAppearance()
        {
            //<ImageTexture url='"images/marble.jpg"' />
            TagName = "Appearance";
            Material = new X3DMaterial();
            ImageTexture = new X3DImageTexture();
            Movement = new X3DMovement();
        }
        public override void Render(StringBuilder sb)
        {
            AddChild(Material);
            AddChild(ImageTexture);
            AddChild(MovieTexture);
            if (Movement.Rotate!=null || !string.IsNullOrWhiteSpace(Movement.ClingToCamera)||Movement.Presentation!=null || Movement.Bounciness != null || Movement.Kinetic != null || Movement.HandDraggable != null)
            {
                AddChild(Movement);
            }
            base.Render(sb);
        }
        public X3DMaterial Material { get; set; }
        public X3DImageTexture ImageTexture { get; set; }
        public X3DMovieTexture MovieTexture { get; set; }
        public X3DMovement Movement { get; set; }

    }
}
