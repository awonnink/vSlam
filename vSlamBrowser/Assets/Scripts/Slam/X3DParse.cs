using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X3DParse
{
    public class X3DParse
    {
        public Node Parse(string Xml)
        {
            char[] chars = Xml.ToCharArray();
            int counter = -1;
            Node top = ParsePart(ref chars, ref counter, null);
            return top;
        }
        private Node ParsePart(ref char[] chars, ref int counter, Node parent)
        {
            Node child = new Node();
            child.NodeType = NodeType.Element;
            bool nameStarted = false;
            bool isendTag = false;
            bool attibsStarted = false;
            StringBuilder sbName = new StringBuilder();
            StringBuilder sbAttribs = new StringBuilder();
            StringBuilder sbTextNode = new StringBuilder();
            //string name = "";
            //string attribs = "";
            while (counter < chars.Length-1)
            {
                counter++;
                string cs = chars[counter].ToString();
                 if(cs=="<")
                {
                    nameStarted = true;
                    string nextChar = chars[counter + 1].ToString();
                    if(nextChar == "/")
                    {
                        isendTag = true;
                    } 
                    else if(nextChar=="!" || nextChar == "?")//comment or xml declaration
                    {
                        while(chars[counter].ToString()!=">")
                        {
                            counter++;
                        }
                        counter++;
                    }
                    cs = "";
                }
                 else if(nameStarted && cs==" " && sbName.Length==0) //ignore spaces before tagname
                {
                    cs = "";
                }
                else if(isEndChar(cs))
                {
                    if (!isendTag)
                    {
                        if (nameStarted)
                        {
                            child.Name = sbName.ToString();
                        }
                    }
                    if(isendTag && cs==">")//Node completed for parent
                    {
                        parent.NodeCompleted = true;
                        if(parent.Children.Count==0)
                        {
                            parent.Value = sbTextNode.ToString();
                        }
                        break;
                    }
                    if (!string.IsNullOrEmpty(child.Name))
                    {
                        if (cs != ">")
                        {
                            attibsStarted = true;
                        }
                        else
                        {
                            
                            if (attibsStarted)
                            {
                                attibsStarted = false;
                                HandleAttribs(sbAttribs, child);
                               sbAttribs = new StringBuilder();
                            }
                            cs = "";
                            if (chars[counter - 1].ToString() == "/")//Node completed on current level
                            {
                                child.NodeCompleted = true;
                                //if(child.Name== "WorldInfo")
                                //{
                                //    var a = 1;
                                //}
                                ;
                            }
                            else
                            {

                                ParsePart(ref chars, ref counter, child);
                            }
                           
                        }
                    }
                    sbName = new StringBuilder();
                    nameStarted = false;

                }
                if (nameStarted)
                {
                    sbName.Append( cs);
                }
                else if (attibsStarted)
                {
                    sbAttribs.Append( cs);
                }
                else if(!isendTag)
                {
                    sbTextNode.Append(cs);
                }
                if (child.NodeCompleted)
                {
                    if (parent != null && !string.IsNullOrEmpty(child.Name))
                    {
                        parent.Children.Add(child);
                        child = new Node();
                    }
                    else
                    {
                        if(parent!=null)
                        {
                            parent.Value = sbTextNode.ToString();
                        }
                       return child;
                    }
                }

            }
            return child;
        }
        private void HandleAttribs(StringBuilder attribs, Node parent)
        {
//            return;
            bool startAttrvalue = false;
            StringBuilder sbName = new StringBuilder();
            StringBuilder sbVal = new StringBuilder();
//            string name = "";
//            string val = "";
            string quoteChar = null;
//           char[] chars = attribs.ToString().ToCharArray();
            for(int nn=0;nn< attribs.Length;nn++)
            {

                string cs = attribs[nn].ToString();
                if (quoteChar == null)
                {
                    if (cs == "'" || cs == "\"")
                    {
                        quoteChar = cs;
                    }
                }
                if(cs== quoteChar)
                {
                    if(startAttrvalue)
                    {
                        Node n = new Node();
                        n.NodeType = NodeType.Attribute;
                        n.Name = sbName.ToString().Trim();
                        n.Value = XmlDecode(sbVal.ToString());
                        parent.Children.Add(n);
                        sbName = new StringBuilder();
                        sbVal = new StringBuilder();
                        quoteChar = null;
                    }
                    startAttrvalue = !startAttrvalue;
                }
                else
                {
                    if(startAttrvalue)
                    {
                        sbVal.Append(cs);
 //                       val += cs;
                    }
                    else if(cs!="=")
                    {
                        sbName.Append(cs);
//                        name += cs;
                    }
                }
            }
        }
        public bool isEndChar(string c)
        {
            return " \r\n/>".IndexOf(c) > -1;
        }
        string XmlDecode(string txt)
        {
            if(!string.IsNullOrEmpty(txt))
            {
                txt = txt.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&pos;", "'");
            }
            return txt;
        }
    }
    
    public class Node
    {
        public Node()
        {
            Children = new List<Node>();
        }
        public bool NodeCompleted { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public NodeType NodeType { get; set; }
        public List<Node> Children { get; set; }
    }
    public enum NodeType
    {
        Element,
        Attribute,
        Text
    }
}
