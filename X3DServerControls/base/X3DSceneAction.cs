using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DSceneAction : X3DControl
    {
        public X3DSceneAction()
        {

            TagName = "slm:SceneAction";

        }
        public override void Render(StringBuilder sb)
        {
            if(ResetActions==true)
            {
                AddProperty("actionname", "slam_reset_actions");
            }
            else
            {
            AddProperty("actionname", ActionName);

            }
            AddProperty("scenetime", SceneTime);
            base.Render(sb);
        }
        public string ActionName { get; set; }
        public float? SceneTime { get; set; }
        public bool ResetActions { get; set; }
        
    }
}
