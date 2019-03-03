using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls.graph
{
 public class Xvalue
    {
        public Xvalue() { }
        public Xvalue(string aName) { name = aName; }
        public Xvalue(string aName, double aYvalue) { name = aName; Yvalue = aYvalue; }
        public string name { get; set; }
        public double Yvalue { get; set; }
    }

    public class Zvalue
    {
        public Zvalue()
        {
            Xvalues = new List<Xvalue>();
            Color = new Vector3(1, 1, 0);
            Transparency = 0;
        }
        public Zvalue(string aName)
        {
            name = aName;
            Xvalues = new List<Xvalue>();
        }
        public string name { get; set; }
        public List<Xvalue> Xvalues { get; set; }
        public Vector3 Color { get; set; }
        public double Transparency { get; set; }
    }

    public class GraphData
    {
        public GraphData()
        {
            GraphType = GraphType.Bar_Default;
            Zvalues = new List<Zvalue>();
        }
        public string Title { get; set; }
        public GraphType GraphType { get; set; }
        public List<Zvalue> Zvalues { get; set; }
    }
    public enum GraphType
    {
        Bar_Default
    }
}
