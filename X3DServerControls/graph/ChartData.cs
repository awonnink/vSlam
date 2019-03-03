using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls.chart
{
    public class ChartData
    {
        public ChartData()
        {
            ChartType = ChartType.Bar_Default;
            x_values = new List<AxisValue>();
            z_values = new List<AxisValue>();
            data = new List<Item>();
        }
        public string title { get; set; }
        public object subTitle { get; set; }
        public List<AxisValue> x_values { get; set; }
        public List<AxisValue> z_values { get; set; }
        public List<Item> data { get; set; }
        public ChartType ChartType { get; set; }
    }
    public class AxisValue
    {
        public string value { get; set; }
        public string label { get; set; }
        public string color { get; set; }
    }

    public class Item
    {
        public string key { get; set; }
        public string x { get; set; }
        public string x_label { get; set; }
        public float y { get; set; }
        public string z { get; set; }
        public string z_label { get; set; }
    }
    public enum ChartType
    {
        Bar_Default
    }

}
