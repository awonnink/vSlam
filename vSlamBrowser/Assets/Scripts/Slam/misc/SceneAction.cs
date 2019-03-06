using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public class SceneAction
    {
        public SceneAction(string goName, float sceneTime)
        {
            GoName = goName;
            SceneTime = sceneTime;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is SceneAction)
            {
                SceneAction aObj = obj as SceneAction;
                if (aObj != null && aObj.GoName == GoName && Math.Abs(aObj.SceneTime - SceneTime) < 0.001f)
                {
                    return true;
                }
            }
            return false;
        }
        public string GoName { get; set; }
        public float SceneTime { get; set; }
    }

}
