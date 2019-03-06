using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SimpleJSON;
namespace VSlamHL
{
    public class Tree3D
    {

        public Tree3D(string nameField = "Name", string idField = "Id", string groupField = "")
        {
            NameField = nameField;
            IdField = idField;
            GroupField = groupField;
            Nodes = new List<Node>();
        }
        public List<Node> Nodes;
        //public List<Node> Create(string jsonString)
        //{
        //    Nodes = new List<Node>();
        //    var N = JSON.Parse(jsonString);
        //    var l = N.Count;
        //    for (int nn = 0; nn < l; nn++)
        //    {
        //        var name = N[nn][NameField];
        //        var id = N[nn][IdField];
        //        Node parent = new Node(name, id);
        //        if (!string.IsNullOrEmpty(GroupField))
        //        {
        //            var group = N[nn][GroupField];
        //            parent.Group = group;
        //        }
        //        Nodes.Add(parent);
        //        CheckChildren(N[nn], parent);

        //    }
        //    return Nodes;
        //}
        //private void CheckChildren(JSONNode parent, Node parentNode)
        //{
        //    var children = parent["Children"];
        //    var l = children.Count;

        //    for (int nn = 0; nn < l; nn++)
        //    {
        //        var name = children[nn][NameField];
        //        var id = children[nn][IdField];
        //        Node child = new Node(name, id);
        //        if (!string.IsNullOrEmpty(GroupField))
        //        {
        //            var group = children[nn][GroupField];
        //            child.Group = group;
        //        }
        //        parentNode.Children.Add(child);
        //        CheckChildren(children[nn], child);

        //    }
        //}

        public string NameField { get; set; }
        public string IdField { get; set; }
        public string GroupField { get; set; }


    }
    public class Node
    {
        public Node()
        { Children = new List<Node>(); }
        public Node(string aName, string anId)
        {
            Id = anId;
            Name = aName;
            Children = new List<Node>();
        }
        public void AddChild(Node aNode)
        {
            aNode.Parent = this;
            Children.Add(aNode);
        }
        public Node Parent { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public List<Node> Children { get; set; }
        public GameObject GameObject { get; set; }
    }
}
