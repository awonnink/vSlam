using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSlamHL
{
    public class TreeView3D : MonoBehaviour
    {
        public float childrenDistance = 0.4f;
        public float parentDistance = 1;
        public float childObjectScale = 1;
        public float selectedSizeFactor = 1.4f;
        public Vector3 drift = new Vector3(-10.0F, 10.0F, 0.0F);
        public static Vector3 sDrift = Vector3.zero;
        public Color LineStartColor = Color.blue;
        public Color LineEndColor = Color.green;
        public float LineStartWidth = 0.001f;
        public float LineEndWidth = 0.003f;
        List<TreeView3DItem> items = new List<TreeView3DItem>();
        GameObject selectedItem;
        Material lineMaterial;
        // Use this for initialization
        void Start()
        {
            sDrift = drift;
            lineMaterial= new Material(Shader.Find("LineShader")); 
        }

        // Update is called once per frame
        void Update()
        {
            CheckObjectDistances();
        }
        public void CreateChildren(TreeView3DItem parent, List<GameObject> children = null)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    TreeView3DItem c = CreateItem(parent, child);
                }
            }
        }
        public TreeView3DItem CreateItem(TreeView3DItem parent, GameObject visibleItem, bool hasParentTransform=false)
        {
            GameObject ga = hasParentTransform?visibleItem: new GameObject("subItem");
            TreeView3DItem li = ga.AddComponent<TreeView3DItem>();
            li.tv3d = this;
            if (parent != null)
            {
                li.ParentItem = parent;
                li.transform.position = parent.transform.position;
                li.transform.parent = parent.transform;
                parent.Children.Add(li);
            }
            else
            {
                li.defaultScale = childObjectScale* transform.localScale;
                li.transform.parent = this.transform;
            }
            if (!hasParentTransform)
            {
                visibleItem.transform.parent = li.transform;
                visibleItem.transform.localPosition = Vector3.zero;
            }
            items.Add(li);
            li.parentDistance = this.parentDistance;
            return li;
        }
        void CheckObjectDistances()
        {
            foreach (var item in items)
            {
                foreach (var toItem in items)
                {
                    if (item != toItem && item.ParentItem!=null)
                    {
                        CheckDistance(item, toItem, childrenDistance, true);
                    }
                }
            }
        }
        public static void CheckDistance(TreeView3DItem fromObject, TreeView3DItem toObject, float distance, bool minDistance = false)
        {
            Vector3 direction = fromObject.transform.position - toObject.transform.position;
            if (direction.magnitude == 0)
            {
                direction = Vector3.one;
            }
            if (!minDistance || direction.magnitude < distance)
            {
                direction.Normalize();
                fromObject.transform.position = Vector3.Lerp(fromObject.transform.position, toObject.transform.position + direction * distance, Time.deltaTime);
            }
            //
            fromObject.transform.position = Vector3.Lerp(fromObject.transform.position, sDrift, 0.001f*Time.deltaTime);

        }
        public void SelectItem(GameObject item)
        {
            selectedItem = item;
            SetSelection(item);
        }
        void SetSelection(GameObject selectedItem)
        { 

            GameObject parent = selectedItem.transform.parent.gameObject;
            if(parent!=null)
                {
                    TreeView3DItem tvItem = parent.GetComponentInChildren<TreeView3DItem>();
                    foreach(var child in items)
                    {
                         child.SetSelected(child == tvItem);
                    }
                }
        }
        public void DrawLine(Vector3 pos1, Vector3 pos2)
        {
            StartCoroutine(drawLine(pos1, pos2, LineEndColor));
        }
        IEnumerator drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.SetParent(this.transform);
            myLine.transform.position = start;
            LineRenderer lr = myLine.AddComponent<LineRenderer>();
            // LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startColor = LineStartColor;
            lr.endColor = color;
            lr.startWidth = LineStartWidth;
            lr.endWidth = LineEndWidth;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            yield return new WaitForSeconds(duration);
            GameObject.Destroy(myLine);
        }

    }
}