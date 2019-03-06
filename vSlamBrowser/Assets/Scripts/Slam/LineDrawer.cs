using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class LineDrawer
    {
        public Color LineStartColor = Color.blue;
        public Color LineEndColor = Color.green;
        public float LineStartWidth = 0.001f;
        public float LineEndWidth = 0.003f;
        Material lineMaterial=null;
        public Material LineMaterial
        {
            get
            {
                if(lineMaterial==null)
                {
                    var sh = Shader.Find("LineShader");
                    if (sh != null)
                    {
                        lineMaterial = new Material(sh);
                    }
                }
                return lineMaterial;
            }
        }
 
        public IEnumerator DrawLine(Transform parent,  Vector3 start, Vector3 end, Color color, float duration = 0.01f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.SetParent(parent);
            myLine.transform.position = start;
            LineRenderer lr = myLine.AddComponent<LineRenderer>();
            // LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = LineMaterial;
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
