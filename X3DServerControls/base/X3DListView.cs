using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DListView:X3DControl
    {
        public X3DListView()
        {
            TagName = "slm:ListView3D";
        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("name", Name);
            AddProperty("pageSize", PageSize);
            AddProperty("start", Vector3.ToString(Start));
            AddProperty("middle1", Vector3.ToString(Middle1));
            AddProperty("middle2", Vector3.ToString(Middle2));
            AddProperty("end", Vector3.ToString(End));
            AddProperty("startrot", Vector3.ToString(StartRot));
            AddProperty("middle1rot", Vector3.ToString(Middle1Rot));
            AddProperty("middle2rot", Vector3.ToString(Middle2Rot));
            AddProperty("endrot", Vector3.ToString(EndRot));
            AddProperty("navigationbuttonscale", Vector3.ToString(NavigationButtonScale));
            AddProperty("navigationbuttonposition", Vector3.ToString(NavigationButtonPosition));
            AddChild(NodePrefab);
            AddChild(NextButtonPrefab);
            AddChild(ResetButtonPrefab);
            if(ListType!= ListType.standard)
            {
                AddProperty("listtype", ListType.ToString().ToLower());
            }
            foreach (var node in Nodes)
            {
                Children.Add(node);
            }

            base.Render(sb);
        }
        public string Name { get; set; }
        public int? PageSize { get; set; }
        public Vector3 Start { get; set; }
        public Vector3 Middle1 { get; set; }
        public Vector3 Middle2 { get; set; }
        public Vector3 End { get; set; }
        public Vector3 StartRot { get; set; }
        public Vector3 Middle1Rot { get; set; }
        public Vector3 Middle2Rot { get; set; }
        public Vector3 EndRot { get; set; }
        public Vector3 NavigationButtonScale { get; set; }
        public Vector3 NavigationButtonPosition { get; set; }
        public X3DNodePrefab NodePrefab { get; set; }
        public X3DNextButtonPrefab NextButtonPrefab { get; set; }
        public X3DResetButtonPrefab ResetButtonPrefab { get; set; }
        public List<X3DNode> Nodes { get; set; } = new List<X3DNode>();
        public ListType ListType { get; set; } = ListType.standard;
    }
    public enum ListType
    {
        standard,
        simple
    }
}
