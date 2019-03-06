using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class TextDynamics : MonoBehaviour
    {
        public List<TextDynamic> TextDynamicList = new List<TextDynamic>();
        public string Text;
        TMPro.TextMeshPro tm = null;
        // Use this for initialization
        void Start()
        {
            tm = GetComponent<TMPro.TextMeshPro>();
        }

        // Update is called once per frame
        void Update()
        {
            if(tm !=null && TextDynamicList.Count>0)
            {
                List<string> paramList = new List<string>();
                foreach(TextDynamic td in TextDynamicList)
                {
                    switch(td)
                    {
                        case TextDynamic.CurrentUser:
                            paramList.Add(Slam.Instance.DeviceManager.NickName);
                            break;
                        case TextDynamic.TimeWithSeconds:
                            paramList.Add(System.DateTime.Now.ToString("hh:mm:ss"));
                            break;
                    }
                }
                for(int nn=0;nn<10;nn++)
                {
                    paramList.Add("");
                }
                try
                {
                    tm.text = string.Format(Text, paramList.ToArray());
                }
                catch (System.Exception) { }
            }
        }
    }
    public enum TextDynamic
    {
        TimeWithSeconds,
        CurrentUser,
    }
}