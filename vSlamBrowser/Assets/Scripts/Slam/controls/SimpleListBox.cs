using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class SimpleListBox : MonoBehaviour, IListView3D
    {
        public int PageSize = 7;
        int firstItem = 0;
        public Vector3 navigationButtonScale = Vector3.one*0.3f;
        public Vector3 navigationButtonPosition = Vector3.zero;
        List<GameObject> items = new List<GameObject>();
        X3DParse.Node parentNode = new X3DParse.Node();
        public X3DParse.Node ParentNode
        {
            get
            {
                return parentNode;
            }
            set
            {
                parentNode = value;
            }
        }
        public GameObject nodePrefab = null;
        public GameObject previousPrefab = null;
        public GameObject nextPrefab = null;

        GameObject prefBtn = null;
        GameObject nextBtn = null;
        
        // Use this for initialization
        void Start()
        {

        }
        GameObject GetDefaultNode()
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            var obj = Slam.Instance.GetPrefab(go.transform, "primitives", "smoothcube");
            if (obj != null)
            {
                obj.transform.parent = go.transform;
            }

            var txt = Slam.Instance.GetTextMeshObject();
            if (txt != null)
            {
                txt.transform.parent = go.transform;
            }
            return go;
        }
        public void Initiate()
        {
            if(nodePrefab==null)
            {
                nodePrefab = GetDefaultNode();
            }
            if(parentNode!=null)
            {
                int nn = 0;
                foreach(var child in parentNode.Children)
                {
                    AddNodelIstItem(child, nn);
                    nn++;
                }
            }
            if(nodePrefab!=null)
            {
                nodePrefab.SetActive(false);
            }
            bool buttonhasPrefab = false;

            GameObject navigation = new GameObject("navigation");
            navigation.transform.parent=transform;
            if(navigationButtonPosition==Vector3.zero)
            {
                navigation.transform.localPosition = new Vector3(-0.3f, -0.3f, 0);
            }
            else
            {
                navigation.transform.localPosition = navigationButtonPosition;
            }
            navigation.transform.localScale = navigationButtonScale;
            prefBtn = AssureNavigationButtonPrefab(previousPrefab, true, out buttonhasPrefab);
            prefBtn.transform.SetTheParent(navigation.transform);
            if(!buttonhasPrefab)
            {
                prefBtn.transform.localPosition = new Vector3(0, 0, 0);
            }
            nextBtn = AssureNavigationButtonPrefab(previousPrefab, false, out buttonhasPrefab);
            nextBtn.transform.SetTheParent(navigation.transform);
            if (!buttonhasPrefab)
            {
                nextBtn.transform.localPosition = new Vector3(0, -0.3f, 0);
            }
            Move();

        }

        void AddNodelIstItem(X3DParse.Node child, int count)
        {

            var newItem = GameObject.Instantiate(nodePrefab, transform);
            ClickableTextSpec spec = ClickableTextSpec.FromX3dParseNode(child);

            var firstCollider = newItem.gameObject.GetComponentInChildren<Collider>();
            if (firstCollider != null)
            {
                var so = firstCollider.gameObject.GetComponent<SlamObject>();
                if (so == null)
                {
                    so = firstCollider.gameObject.AddComponent<SlamObject>();
                }
                so.Target = spec.Target;
                so.Href = spec.Href;
                so.ToolTip = spec.Tooltip;
                so.name = "lbItem" + count.ToString();

            }
            //text
            if (!string.IsNullOrEmpty(spec.Text))
            {
                var textMeshObj = newItem.GetComponentInChildren<TMPro.TextMeshPro>();
                if (textMeshObj == null)
                {
                    textMeshObj = Slam.Instance.GetTextMeshObject(spec.Text, 10, 1, 15);
                    textMeshObj.gameObject.transform.SetTheParent(newItem.transform);
                    textMeshObj.gameObject.transform.localPosition = new Vector3(7, 0, 0);
                    textMeshObj.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    textMeshObj.alignment = TMPro.TextAlignmentOptions.Left;
                    textMeshObj.color = Color.black;

                }
                else
                {
                    textMeshObj.text = spec.Text;
                }
            }
            items.Add(newItem);
            newItem.SetActive(false);
        }

        GameObject AssureNavigationButtonPrefab( GameObject prefab, bool isUp, out bool buttonHasPrefab)
        {
            buttonHasPrefab = true;
            if (prefab == null)
            {
                prefab = Slam.Instance.GetPrefab(null, Constants.primitivesGroup, Constants.arrowPrefabName);
                var rotation = isUp ? new Vector3(0, -90f, 90f) : new Vector3(0, -90f, 270f);
                prefab.transform.localRotation = Quaternion.Euler(rotation);
                buttonHasPrefab = false;
            }
            var coll = prefab.GetComponentInChildren<Collider>();
            if (coll != null)
            {
                var l3dnb = coll.gameObject.AddComponent<List3DNavigationButton>();
                l3dnb.reset = isUp;
                l3dnb.listbox = this;
            }
            return prefab;

        }

        public void MoveNext(bool IsUp)
        {
            if (IsUp)
            {
                firstItem -= PageSize;
                if (firstItem<0)
                {
                    firstItem = 0;
                }
            }
            else
            {
                if (firstItem < items.Count - PageSize)
                {
                    firstItem += PageSize;
                }
            }
           Move();
        }
       void Move()
        {
            int nn = 0;
            int pp = 0;
            if(PageSize==0)
            {
                PageSize = 7;
            }
            float itemDist = 1 / ((float)PageSize);
            foreach (var item in items)
            {
                if(nn<firstItem )
                {
                    item.transform.localPosition = Vector3.zero;
                    item.SetActive(false);
                }
                else if (nn >= firstItem + PageSize)
                {
                    var so=item.GetComponentInChildren<SlamObject>();
                    Vector3 worlp = transform.TransformPoint(Vector3.zero - Vector3.up);
                    so.MoveTo(worlp, item.transform.rotation);
                    item.SetActive(false);
                }
                else
                {
                    item.transform.localPosition=Vector3.zero-pp* itemDist* Vector3.up;
                    item.SetActive(true);
                    pp++;
                }
                nn++;
            }
            if( prefBtn!=null)
            {
                prefBtn.SetActive(firstItem != 0);
            }
            if (nextBtn != null && items!=null)
            {
                nextBtn.SetActive(firstItem+PageSize<items.Count);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
