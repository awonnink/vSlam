using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.Networking;

namespace Slam
{
    public class Communicator : Singleton<Communicator>
    {
        private const string communicatorPage = "communicator.aspx";
        public bool AvatarOn = true;
        public float waitTime = 1f;
        bool checking = false;
        // Use this for initialization
        void Start()
        {
            InvokeRepeating("Check", 1f, 0.2f);
        }

        // Update is called once per frame
        void Update()
        {

        }
        private string getBaseCommunicatorUrl()
        {
            string url = Slam.Instance.UrlHandler.GetBaseUrl() + communicatorPage;
            return url;
        }

//        public IEnumerator Check()
        public void Check()
        {
            //while (true)
            //{
            //    yield return new WaitForSeconds(waitTime);
            if (!checking && AvatarOn && !string.IsNullOrEmpty(Slam.Instance.UrlHandler.CurrentPageUrl))// && !Slam.Instance.UrlHandler.SingleUser)//
            {
                checking = true;
                string url = null;
                try
                {
                    string avId = Slam.Instance.DeviceManager.UserGuid;
                    string nickN = CheckNullOrEmptyValue(Slam.Instance.NickName);
                    // Vector3 pos = Slam.Instance.SceneGO.transform.position - Camera.main.transform.position;// - 1.625F*Vector3.up;
                    Vector3 pos = Slam.Instance.ScenePosition(Camera.main.transform.position);
                    Quaternion rot = Camera.main.transform.rotation;
                    string avNo = CheckNullOrEmptyValue(Slam.Instance.AvatarNo);
                    string dissGuid = CheckNullOrEmptyValue(Slam.Instance.DissonanceGuid);

                    url = getBaseCommunicatorUrl();
                    //todo: add own properties
                    var roomUrl = Slam.Instance.UrlHandler.CurrentPageUrl;
                    if (Slam.Instance.UrlHandler.SingleUser)
                    {
                        roomUrl = Statics.AddUrlProperty(roomUrl, "su", avId);
                    }
                    else
                    {
                        roomUrl = Slam.Instance.UrlHandler.GetRoomFromUrl(roomUrl);
                    }
                    string room = WWW.EscapeURL(roomUrl);

                    string prop = room.Trim() + "|" + avId + "|" + ClearValue(pos.Vector3ToString() + "|" + ClearValue(rot.QuaternionToString()));
                    prop += "|" + nickN + "|" + avNo + "|" + dissGuid;
                    string action = "";
                    string actionTime = "";
                    if (Slam.Instance.Actions.Count > 0)
                    {
                        action = Slam.Instance.Actions.Peek().GoName;
                        actionTime = Slam.Instance.Actions.Peek().SceneTime.ToString();
                    }
                    prop += "|" + CheckNullOrEmptyValue(action) + "|" + actionTime;

                    url = Statics.AddUrlProperty(url, "u", WWW.EscapeURL(prop));
                    if(Slam.Instance.IsImmersive())
                    {
                        url = CheckImmersiveController(url, "lh", Slam.Instance.LeftController);
                        url = CheckImmersiveController(url, "rh", Slam.Instance.RightController);
                    }
                    if (Slam.Instance.UrlHandler.SingleUser)
                    {
                        url = Statics.AddUrlProperty(url, "s", "1");
                    }
                    if(true || Slam.Instance.IsHoloLens())
                    {
                        var ext = Slam.Instance.GetTypedTextForExternalKeyboard();
                        if (ext!=null)
                        {
                            url = Statics.AddUrlProperty(url, "kb", WWW.EscapeURL(ext));
                            url = Statics.AddUrlProperty(url, "g", Slam.Instance.DeviceManager.UserGuid);
                        }
                    }
                    if (Slam.Instance.communityPosts != null)
                    {
                        url = Statics.AddUrlProperty(url, "p", WWW.EscapeURL(Slam.Instance.postsPageno.ToString()));
                        url = Statics.AddUrlProperty(url, "lmg", WWW.EscapeURL(Slam.Instance.latestPostGuid));
                        if (!string.IsNullOrEmpty(Slam.Instance.CommunityInput))
                        {
                            url = Statics.AddUrlProperty(url, "ci", WWW.EscapeURL(Slam.Instance.CommunityInput));
                            Slam.Instance.CommunityInput = null;
                        }
                    }
                }
                catch (System.Exception x)
                {
                    checking = false;
                    return;
                }
                StartCoroutine(GetXML(url));
            }
           // }
        }
        string CheckImmersiveController(string url, string tagName, Transform controllerT)
        {
            if (controllerT != null && controllerT.gameObject.activeSelf)
            {
                var pl = Slam.Instance.ScenePosition(controllerT.position);
                var rl = controllerT.rotation;
                url = Statics.AddUrlProperty(url, tagName + "p", WWW.EscapeURL(ClearValue(pl.Vector3ToString())));
                url = Statics.AddUrlProperty(url, tagName + "r", WWW.EscapeURL(ClearValue(rl.QuaternionToString())));
            }
            return url;
        }
        string CheckNullOrEmptyValue(string aValue)
        {
            if(string.IsNullOrEmpty(aValue))
            {
                return "";
            }
            return aValue;
        }
        string ClearValue(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val.Replace("(", "").Replace(")", "").Replace(", ", " ");
            }
            return null;
        }
        IEnumerator GetXML(string url)
        {
            /* For using rest service*/
            if (string.IsNullOrEmpty(url))
            {
                throw new System.Exception("No Url provided");
            }
            UnityWebRequest www = null;
            bool anErrorOccured = false;
            try
            {
                string serverObjects = Slam.Instance.SerialiseServerObjects();

                var headers = Slam.Instance.GetSlamHttpHeaders();
                if (serverObjects == null)
                {
                    www = UnityWebRequest.Get(url);
                }
                else
                {
                    www = UnityWebRequest.Post(url, serverObjects);
                    www.SetRequestHeader("Content-Type", "application/json");
                }

                foreach (var h in headers)
                {
                    www.SetRequestHeader(h.Key, h.Value);
                }
            }
            catch(System.Exception x)
            {
                checking = false;
                anErrorOccured = true;
            }
            if (!anErrorOccured)
            {
                //  WWW www = new WWW(url, rawData, headers);
                yield return www.SendWebRequest();
                try
                {
                    if (string.IsNullOrEmpty(www.error))
                    {
                        HandleXml(www.downloadHandler.text);
                    }
                }
                catch (System.Exception) { }
                finally { checking = false; }
            }
             
        }
        void HandleXml(string text)
        {
            X3DParse.X3DParse parser = new X3DParse.X3DParse();
            X3DParse.Node root = parser.Parse(text);

            if (root != null)
            {
                    Slam.Instance.HandleCommunication(root);
            }
        }
    }
}
