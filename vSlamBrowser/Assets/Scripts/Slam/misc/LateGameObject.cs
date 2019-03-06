using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Slam
{
    class LateGameObject
    {
        public LateGameObject(X3DParse.Node shapeNode, Transform parent, LateGameObjectType lateGameObjectType = LateGameObjectType.ShapeNode)
        {
            ShapeNode = shapeNode;
            Parent = parent;
            LateGameObjectType = lateGameObjectType;
        }
        public X3DParse.Node ShapeNode { get; set; }
        public Transform Parent { get; set; }
        public LateGameObjectType LateGameObjectType { get; set; }
    }
}
