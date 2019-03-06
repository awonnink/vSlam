using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySceneManager : MonoBehaviour {

    bool anchored = false;
    bool refresh = true;
    string openUrl = null;
    // Use this for initialization
    void Start () {

        Init();

    }
   
    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {
            //Init();
        }
    }
    public void Init(string[] args=null)
    {
#if !UNITY_WSA
        if(args==null)
        {
            args = System.Environment.GetCommandLineArgs();
        }
#else
       // var targsS=UnityEngine.WSA.Application.arguments;
       // var args = argsS.Split(System.Convert.ToChar(" "));
#endif
        if (args != null)
        {
            foreach (var arg in args)
            {
                Debug.Log(string.Format("Arg: {0}", arg));
                if (arg.ToLower().Contains(".x3d") || arg.ToLower().Contains(".x3dx") || arg.ToLower().Contains(".aspx"))
                {
                    if (openUrl != arg)
                    {
                        refresh = true;
                    }
                    openUrl = arg;
                    break;
                }
            }
        }
        if (refresh)
        {
           // refresh = false;
            if (openUrl != null)
            {
                Slam.Slam.Instance.UrlHandler.HomeUrl = Slam.Slam.Instance.UrlHandler.GetHomeUrl();
                if (!openUrl.ToLower().StartsWith("http"))
                {
 #if !UNITY_WSA
                   openUrl = "file:///" + openUrl.Replace(@"\", "/");
#endif
                }
                Debug.Log("Open url: " + openUrl);
                Slam.Slam.Instance.Parse(openUrl);
            }
            else
            {
                Slam.Slam.Instance.GoHome();
            }
        }
    }
        // Update is called once per frame
    void Update () {

	if(!anchored && Slam.Slam.Instance.RootGO!=null)
    {
#if UNITY_WSA && !UNITY_EDITOR
        //if (Slam.Slam.Instance != null && Slam.Slam.Instance.IsHoloLens())
        //{
        //    HoloToolkit.Unity.WorldAnchorManager.Instance.AttachAnchor(Slam.Slam.Instance.RootGO, "SlamRootObject");
        //    anchored = true;
        //}
            //https://forums.hololens.com/discussion/2667/how-to-use-unity-world-anchors
//#else
#endif
            anchored = true;
    }
    if(Input.GetKey(KeyCode.Escape))
    {
        Application.Quit();
    }
}

    private void OnApplicationQuit()
    {
        Slam.Slam.Instance.CleanUpScene();
    }
}
