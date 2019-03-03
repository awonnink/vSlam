using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlmControls;
using SlmControls.chart;
namespace SlmControls.graph
{
    public static class Graph
    {
        public static GraphData GetDefaultSet()
        {
            GraphData o = new GraphData();
            o.Title = "Usage of v-Slam for presentations";
            Zvalue zvalue2017 = new Zvalue("2017");
            o.Zvalues.Add(zvalue2017);
            zvalue2017.Transparency = 0.2;
            zvalue2017.Xvalues.Add(new Xvalue("January", 273));
            zvalue2017.Xvalues.Add(new Xvalue("February", 100));
            zvalue2017.Xvalues.Add(new Xvalue("March", 59));
            zvalue2017.Xvalues.Add(new Xvalue("April", 421));
            zvalue2017.Xvalues.Add(new Xvalue("May", 110));
            zvalue2017.Xvalues.Add(new Xvalue("June", 160));
            zvalue2017.Xvalues.Add(new Xvalue("July", 94));
            zvalue2017.Xvalues.Add(new Xvalue("August", 333));
            zvalue2017.Xvalues.Add(new Xvalue("September", 210));
            zvalue2017.Xvalues.Add(new Xvalue("October", 190));
            zvalue2017.Xvalues.Add(new Xvalue("November", 220));
            zvalue2017.Xvalues.Add(new Xvalue("December", 44));
            Zvalue zvalue2018 = new Zvalue("2018");
            o.Zvalues.Add(zvalue2018);
            zvalue2018.Transparency = 0.2;
            zvalue2018.Xvalues.Add(new Xvalue("January", 441));
            zvalue2018.Xvalues.Add(new Xvalue("February", 132));
            zvalue2018.Xvalues.Add(new Xvalue("March", 90));
            zvalue2018.Xvalues.Add(new Xvalue("April", 302));
            zvalue2018.Xvalues.Add(new Xvalue("May", 267));
            zvalue2018.Xvalues.Add(new Xvalue("June", 152));
            zvalue2018.Xvalues.Add(new Xvalue("July", 94));
            zvalue2018.Xvalues.Add(new Xvalue("August", 402));
            zvalue2018.Xvalues.Add(new Xvalue("September", 111));
            zvalue2018.Xvalues.Add(new Xvalue("October", 98));
            zvalue2018.Xvalues.Add(new Xvalue("November", 10));
            zvalue2018.Xvalues.Add(new Xvalue("December", 229));
            return o;
        }
        public static GraphData GetDefaultSet1()
        {
            GraphData o = new GraphData();
            o.Title = "Big city populations (milion people)";
            Zvalue zvalueBC = new Zvalue("Europe");
            o.Zvalues.Add(zvalueBC);
            zvalueBC.Transparency = 0.2;
            zvalueBC.Xvalues.Add(new Xvalue("Paris", 2.1));
            zvalueBC.Xvalues.Add(new Xvalue("London", 7.0));
            zvalueBC.Xvalues.Add(new Xvalue("Amsterdam", 0.7));
            zvalueBC.Xvalues.Add(new Xvalue("Moscow", 8.2));
            zvalueBC.Xvalues.Add(new Xvalue("St Petersburg", 4.8));
            zvalueBC.Xvalues.Add(new Xvalue("Berlin", 3.4));
 
            return o;
        }
        
       public static X3DTransform GetChart(ChartData o)
        {
            if (o != null)
            {
                switch (o.ChartType)
                {
                    case ChartType.Bar_Default:
                        return BarGraph(o);
                }
            }
            return null;
        }
        public static X3DTransform GetGraph(GraphData o)
        {
            if (o != null)
            {
                switch (o.GraphType)
                {
                    case GraphType.Bar_Default:
                        return BarGraph(o);
                }
            }
            return null;
        }
        public static X3DTransform BarGraph(ChartData o)
        {
            double yscale = GetYScale(o) * 0.4;
            double graphScale = 0.2;
            double dataHeight = 3;
            X3DTransform p = new X3DTransform();
            p.Translation = new Vector3(0, -graphScale * dataHeight / 2.0, 0);
            p.Scale = new Vector3(graphScale);
            X3DTransform legenda = new X3DTransform();
            p.AddChild(legenda);
            legenda.Name = "legenda";
            int xlength = o.x_values.Count;
            int zlength = o.z_values.Count;
            var offset = new Vector3(-((double)xlength) / 2, 0, 0);
            legenda.Translation = offset + new Vector3(xlength + 1, 3, 0);
            //title
            if (!string.IsNullOrEmpty(o.title))
            {
                var titletxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
                p.AddChild(titletxt);
                titletxt.Translation = offset + new Vector3(((float)xlength) / 2.0f, -2, 0);
                titletxt.Scale = new Vector3(0.2);
                titletxt.Shape.Text = o.title;
            }

            if (o.z_values.Count > 1)
            {
                foreach (var val in o.z_values)
                {
                    var z_idx = o.z_values.IndexOf(val);
                    var li = GetLegendaLabel(val.value, z_idx, getColor(val.color), val.label);
                    legenda.AddChild(li);
                    li.Translation = new Vector3(0, -z_idx * 0.3, zlength);
                }
            }
            foreach (var m in o.data)
            {
                var value = m.y;
                var z_val = o.z_values.Find(i => i.value == m.z);
                var z_idx = 0;
                if (z_val == null && o.z_values.Count > 0)
                {
                    z_val = o.z_values[0];
                }
                if (z_val == null)
                {
                    z_val = new AxisValue();
                    z_val.color = "1, 0, 0";
                }
                else
                {
                    z_idx = o.z_values.IndexOf(z_val);
                }
                z_idx = z_idx == -1 ? 0 : z_idx;
                var x_val = o.x_values.Find(i => i.value == m.x);
                var x_idx = o.x_values.IndexOf(x_val);
                var label = "";
                if (o.x_values.Count > 0)
                {
                    label = z_idx == 0 ? o.x_values[x_idx].label : null;
                }
                z_idx = o.z_values.Count - z_idx;
                //var z_val = o.z_values[m.z];
                Vector3 color = new Vector3(1);
                p.AddChild(GetBar(z_idx, x_idx, value, dataHeight * value / yscale, getColor(z_val.color), label, 0, zlength, offset));
            }
            return p;
        }
        static Vector3 getColor(string c)
        {
            Vector3 color = new Vector3(1);
            if (c != null)
            {
                color = new Vector3(c.Replace(",", " "));
            }
            return color;

        }
        public static X3DTransform BarGraph(GraphData o)
        {
            double yscale = GetYScale(o) * 1.1;
            double graphScale = 0.2;
            double dataHeight = 3;
            X3DTransform p = new X3DTransform();
            p.Translation = new Vector3(0, -graphScale * dataHeight /2.0, 0);
            p.Scale = new Vector3(graphScale);
            X3DTransform legenda = new X3DTransform();
            p.AddChild(legenda);
            legenda.Name = "legenda";
            int xlength = 1;
            int ylength = 1;
            if (o.Zvalues.Count>0)
            {
                ylength = o.Zvalues.Count;
                xlength = o.Zvalues[0].Xvalues.Count;
            }
            var offset = new Vector3(-((double)xlength) / 2, 0, 0);
            legenda.Translation = offset + new Vector3(xlength+1, 3, 0);
            //title
            if (!string.IsNullOrEmpty(o.Title))
            {
                var titletxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
                p.AddChild(titletxt);
                titletxt.Translation = offset + new Vector3(((float)xlength)/2.0f, -2, 0);
                titletxt.Scale = new Vector3(0.2);
                titletxt.Shape.Text = o.Title;
            }


            int z_idx = 0;
            foreach (var zvalue in o.Zvalues)
            {
                var li = GetLegendaLabel(zvalue.name, z_idx, zvalue.Color, zvalue.name);
                legenda.AddChild(li);
                li.Translation = new Vector3(0, -z_idx * 0.3, ylength);
                int monthno = 0;
                foreach (var m in zvalue.Xvalues)
                {
                    var value = m.Yvalue;
                    var label = z_idx == 0 ? m.name : null;
                    p.AddChild(GetBar(z_idx, monthno, value, dataHeight * value / yscale, zvalue.Color, label, zvalue.Transparency, ylength, offset));
                    monthno++;
                }
                z_idx++;
            }
            return p;
        }
        static X3DTransform GetLegendaLabel(string axisValue, int axisIndex, Vector3 color, string label)
        {
            X3DTransform p = new X3DTransform();
            X3DTransform t = X3DTransform.AddTransFormWithShape(ShapeType.Sphere);
            p.AddChild(t);
            t.Scale = new Vector3(0.2);
            t.Shape.Appearance.Material = new X3DMaterial();
            t.Shape.Appearance.Material.DiffuseColor = color == null ? GetColor(axisIndex) : color;
            if (!string.IsNullOrEmpty(label))
            {
                var labeltxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
                p.AddChild(labeltxt);
                labeltxt.Translation = new Vector3(0.75, 0, 0);
                labeltxt.Scale = new Vector3(0.2);
                labeltxt.Shape.Text = label;
            }
            return p;
        }
        static X3DTransform GetBar(int z, int x, double value, double drawvalue, Vector3 color,  string label, double transparency, int ylength, Vector3 offset)
        {
            X3DTransform p = new X3DTransform();
            p.Translation = offset+ new Vector3(x, 0, z);
            X3DTransform t = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            t.Shape.Group = "primitives";
            t.Shape.Item = "smoothcube";
            p.AddChild(t);
            t.Translation = new Vector3(0, 0.5 * drawvalue, 0);
            t.Scale = 0.5* new Vector3(0.7, drawvalue, 0.7);
            t.Shape.Appearance.Material = new X3DMaterial();
            t.Shape.Appearance.Material.DiffuseColor = color==null?GetColor(z): color;
            if(transparency>0)
            {
                t.Shape.Appearance.Material.Transparency = transparency;
            }
            if (!string.IsNullOrEmpty(label))
            {
                var labeltxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
                p.AddChild(labeltxt);
                labeltxt.Translation = new Vector3(1.2, -1, ylength);
                labeltxt.Scale = new Vector3(0.2);
                labeltxt.EulerRotation = new Vector3(0, 0, -30);
                labeltxt.Shape.Text = label;
                labeltxt.Shape.FontStyle.Justify = Justify.LEFT;
                labeltxt.Shape.RectHeight = 1;
                labeltxt.Shape.RectLength = 14;
            }
            var valuetxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
            p.AddChild(valuetxt);
            valuetxt.Translation = new Vector3(0, drawvalue+0.2, 0);
            valuetxt.Scale = new Vector3(0.2);
            valuetxt.Shape.Text = value.ToString("0.");

            return p;

        }
        static double GetYScale(GraphData o)
        {
            int cnt = 0;
            double tot = 0;
            foreach (var year in o.Zvalues)
            {
                foreach (var m in year.Xvalues)
                {
                    var value = m.Yvalue;
                    tot += value;
                    cnt++;
                }
            }
            if (cnt > 0)
            {
                return tot / cnt;
            }
            return 0;
        }
        static double GetYScale(ChartData o)
        {
            double max_y = 0;

            foreach (var z in o.data)
            {
                    var value = z.y;
                if(max_y < value)
                {
                    max_y = value;
                }
            }
            if(max_y==0)
            {
                max_y = 1;
            }
            return max_y;
        }
        static Vector3 GetColor(int val)
        {
            switch (val)
            {
                case 0:
                    return new Vector3(1, 0, 0);
                case 1:
                    return new Vector3(0, 1, 0);
                case 2:
                    return new Vector3(0, 0, 1);
            }
            return new Vector3(1, 1, 0);

        }


    }

}
