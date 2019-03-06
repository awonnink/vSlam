using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSlamHL
{
    public class Listbox3d : MonoBehaviour, Slam.IListView3D
    {

        public int PageSize = 7;
        int firstItem = 0;
        public DuoVector3 firstItemDuoVector=new DuoVector3(new Vector3(0, 2, 0), Vector3.zero);
        public DuoVector3 middle1 = new DuoVector3(new Vector3(0, 1.8f, 0.3f), Vector3.zero);
        public DuoVector3 middle2 = new DuoVector3(new Vector3(0, 0.2f, 0.3f), Vector3.zero);
        public DuoVector3 end = new DuoVector3(new Vector3(0, 0, 0), Vector3.zero);

        List<DuoVector3> pointers = new List<DuoVector3>();
        Node currentNode=new Node();
        List<GameObject> items=new List<GameObject>();
        public GameObject navigation;
        GameObject selectedItem;
        bool initialized = false;
        public void SelectItem(GameObject item)
        {
            selectedItem = item;
            ClearList(item);
        }
        private void ClearList(GameObject item)
        {
            item.transform.parent = null;
            Items = new List<GameObject>();
        }
        public void MoveNext(bool IsUp)
        {
            if (firstItemDuoVector != null)
            {
                if (IsUp)
                {
                    Initiate();
                }
                else
                {
                    firstItem += PageSize;
                    if (firstItem > currentNode.Children.Count - PageSize)
                    {
                        firstItem = currentNode.Children.Count - PageSize;
                    }
                }
                SetFirtsItem();
            }
        }
        void SetFirtsItem()
        {
            int nn = 0;
            foreach (var ih in currentNode.Children)
            {
                ListboxItem3d li3d = ih.GameObject.GetComponent<ListboxItem3d>();
                if (nn <= firstItem)
                {
                    li3d.startPosition = firstItemDuoVector.Position;
                    li3d.startLookAt = firstItemDuoVector.Forward;
                }
                else
                {
                    li3d.startPosition = Vector3.zero;
                }
                nn++;
            }
        }
        public List<GameObject> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                //Initiate();
            }
        }
        float controlScale = 1;
        // Use this for initialization
        void Start()
        {
             //Test();
           // Initiate();
        }
        void Test()
        {
            Items = new List<GameObject>();
            for (int nn = 0; nn < 20; nn++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = 0.01f * Vector3.one;
                items.Add(cube);
            }
        }
        void InitiateNavigation()
        {
            if (navigation != null)
            {
                navigation.SetActive(items != null && items.Count > PageSize);
            }
        }
        public float GetDistance(Vector3 pos)
        {
            float dist = Vector3.Distance(pos, pointers[0].Position);

            float fact = 0.2f;
            //if(dist<0.1f)
            //{
            fact *= Mathf.Atan(dist) * controlScale;
            //}
            return fact;
        }
        void clearItemHolders()
        {
            foreach (var ih in currentNode.Children)
            {
                if (ih.GameObject != null)
                {
                    Destroy(ih.GameObject);
                }
            }
            currentNode.Children.Clear();
        }
  
        public void Initiate()
        {
            pointers.Add(firstItemDuoVector);
            pointers.Add(middle1);
            pointers.Add(middle2);
            pointers.Add(end);
            controlScale = Vector3.Distance(pointers[1].Position, pointers[2].Position);
            if(navigation!=null &&!initialized)
            {
                navigation.transform.localScale *= controlScale;
                initialized = true;
            }

            clearItemHolders();
            firstItem = 0;
            if (items != null && Items.Count > 0)
            {
                firstItemDuoVector = getFirstItemDuoVector();
                GameObject prev = null;
                int nn = 0;
                foreach (var item in items)
                {
                    GameObject ga = new GameObject("subItem");
                    ga.transform.SetParent( transform);
                    ga.transform.localPosition = Vector3.zero;
                    ga.transform.localRotation = Quaternion.identity;
                    ListboxItem3d li = ga.AddComponent<ListboxItem3d>();
                    li.listbox3d = this;
                    item.transform.parent = ga.transform;
                    item.transform.localPosition = Vector3.zero;
                    if (pointers[0].Forward != Vector3.zero)
                    {
                        item.transform.localRotation = Quaternion.LookRotation(pointers[0].Forward);
                    }
                    if (prev == null)
                    {
                        li.startLookAt = firstItemDuoVector.Forward;
                        li.startPosition = firstItemDuoVector.Position;
                    }
                    else
                    {
                        li.previous = prev.transform;
                    }
                    if (nn == 0)
                    {
                        ga.transform.localPosition = firstItemDuoVector.Position;
                    }
                    else
                    {
                        ga.transform.localPosition = pointers[0].Position;// + nn * 0.1f * Vector3.up;
                    }
                    prev = ga;
                    Node node = new Node();
                    node.GameObject = ga;
                    currentNode.Children.Add(node);
                    //ga.transform.parent = transform;
                    nn++;

                }
            }
            InitiateNavigation();
        }
        // Update is called once per frame
        void Update()
        {

        }
        DuoVector3 getFirstItemDuoVector()
        {
            Vector3 pos = pointers[2].Position + 0.8f * (pointers[3].Position - pointers[2].Position);
            return GetClosestDuoVector(pos);
        }
        public DuoVector3 GetClosestDuoVector(Vector3 pos)
        {
            ;
            float totDist = 0;
            Vector3 resPos = Vector3.zero;
            Vector3 resForward = Vector3.zero;
            foreach (var t in pointers)
            {
                float pDist = Vector3.Distance(pos, t.Position);
                if (pDist == 0)
                {
                    pDist = 0.0001f;
                }
                float weight = Mathf.Pow((1 / pDist), 1);
                totDist += weight;
                resPos += weight * t.Position;
                resForward += weight * t.Forward;
            }
            resPos /= totDist;
            resForward /= totDist;
            return new DuoVector3(resPos, resForward);
        }
    }
    public class DuoVector3
    {
        public DuoVector3()
        {
            Position = Vector3.zero;
            Forward = -Vector3.back;
        }
        public DuoVector3(Vector3 pos, Vector3 fw)
        {
            Position = pos;
            Forward = fw;
        }
        public Vector3 Position;
        public Vector3 Forward;
    }

}
