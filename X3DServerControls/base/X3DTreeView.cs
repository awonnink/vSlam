using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DTreeView:X3DControl
    {
        public X3DTreeView()
        {
            TagName = "slm:TreeView3D";
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("name", Name);
            AddProperty("startlevel", StartLevel);
            AddProperty("linestartcolor", LineStartColor);
            AddProperty("lineendcolor", LineEndColor);
            AddProperty("linestartwidth", LineStartWidth);
            AddProperty("lineendwidth", LineEndWidth);
            AddProperty("parentdistance", ParentDistance);
            AddProperty("childrendistance", ChildrenDistance);
            AddProperty("selectedsizefactor", SelectedSizeFactor);
            AddProperty("facecamera", FaceCamera);
  
            AddProperty("drift", Vector3.ToString( Drift));
            AddChild(NodePrefab);
            foreach(var node in Nodes)
            {
                Children.Add(node);
            }
            base.Render(sb);
        }
        public string Name { get; set; }
        public int? StartLevel { get; set; }
        public Vector3 LineStartColor { get; set; }
        public Vector3 LineEndColor { get; set; }
        public float? LineStartWidth { get; set; }
        public float? LineEndWidth { get; set; }
        public float? ParentDistance { get; set; }
        public float? ChildrenDistance { get; set; }
        public float? SelectedSizeFactor { get; set; }
        public string FaceCamera { get; set; }

        public Vector3 Drift { get; set; }
        public X3DNodePrefab NodePrefab { get; set; }
        public List<X3DNode> Nodes { get; set; } = new List<X3DNode>();
    }
   
}
