using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;

namespace Slam
{
    public class AsyncMeshLoader: Singleton<AsyncMeshLoader>
    {
        void Start()
        {
            
           
        }
       // public IEnumerator AssignMesh(GameObject go, IndexedFaceSet indexedFaceSet)
        public void AssignMesh(MeshContainer meshContainer)
        {
            meshContainer.Mesh = MeshLoadHandler.HandleVertexShapeNode(meshContainer.IndexedFaceSet);
            Slam.Instance.AssignMesh(meshContainer.GameObject, meshContainer.Mesh);
            Slam.Instance.retrievingMesh = false;
        }



    }


}