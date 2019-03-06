using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VSlamHL
{
    public class TreeView3DItem : MonoBehaviour
    {
        public float parentDistance = 1;
        bool selected = false;
        public Vector3 defaultScale = Vector3.zero;
        Vector3 toScale = Vector3.one;
        public bool isVisible = true;
        public TreeView3D tv3d;
        public int level = 0;
        public bool FaceCamera = true;

        // Use this for initialization
        void Start()
        {
            SetDefaultScale(transform.localScale);
        }
        public void SetDefaultScale(Vector3 scale)
        {
            //tv3d = GameObject.Find("TreeView3D").GetComponent<TreeView3D>();
            if (scale != Vector3.zero)
            {
                defaultScale = scale;
                toScale = defaultScale;
                transform.localScale = scale;
            }

        }
        public void setVisible(bool visible)
        {
            isVisible = visible;
            //for (int nn = 0; nn < transform.childCount; nn++)
            //{
            if (transform && transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(isVisible);
            }
            //}
        }
  
        public void SetSelected(bool isselected)
        {
//            CheckScaleInitialization();
            selected = isselected;
            toScale = selected ? defaultScale * tv3d.selectedSizeFactor : defaultScale;
            if (isselected)
            {
                foreach (var child in Children)
                {
                    child.setVisible(true);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(ParentItem!=null)
            {
                TreeView3D.CheckDistance(this, ParentItem, parentDistance);
                DrawLine();
            }
            transform.localScale = Vector3.Lerp(transform.localScale, toScale, Time.deltaTime);
            if (FaceCamera)
            {
                faceCamera();
            }

        }
        void UpdateDistance()
        { 
        }
        void faceCamera()
        {
            Vector3 direction = (transform.position-Camera.main.transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        void DrawLine()
        {
            if (ParentItem != null && isVisible && tv3d!=null)
            {
                tv3d.DrawLine(ParentItem.transform.position, this.transform.position);
            }
        }
 
        public TreeView3DItem ParentItem;
        public List<TreeView3DItem> Children = new List<TreeView3DItem>();
    }
}
