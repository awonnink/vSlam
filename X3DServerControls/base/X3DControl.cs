using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DControl : IX3DControl
    {
        public X3DControl()
        {
            Children = new List<X3DControl>();
            Attributes = new Dictionary<string, string>();
        }
        public string TagName { get; set; }
        public List<X3DControl> Children { get; set; }
        public virtual void Render(StringBuilder sb)
        {
            sb.Append("<");
            sb.Append(TagName);
            foreach(var attr in Attributes)
            {
                sb.Append(" ");
                sb.Append(attr.Key);
                sb.Append("=");
                sb.Append("'");
                sb.Append(attr.Value.Replace("'", "\""));
                sb.Append("'");
            }
            sb.Append(">");
            foreach (var child in Children)
            {
                child.Render(sb);
            }
            sb.Append("</");
            sb.Append(TagName);
            sb.Append(">\r\n");
        }
        public void AddProperty(string name, object value)
        {
            if(value!=null)
            {
                Attributes[name] = XmlEncode(XmlDecode( value.ToString()));
            }
        }
        public void AddChild(X3DControl child)
        {
            if (child != null)
            {
                Children.Add(child);
            }
        }
        string XmlDecode(string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                txt = txt.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
            }
            return txt;
        }
        string XmlEncode(string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                txt = txt.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
            }
            return txt;
        }
        public Dictionary<string, string> Attributes;
    }
}
