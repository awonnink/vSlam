using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace Slam
{
    
    public class slam_notification : MonoBehaviour
    {
        TMPro.TextMeshPro tmpro;
        GameObject imgCube;
        float time = 0;
        int notificationNo=0;
        void Start()
        {
            tmpro = GetComponentInChildren<TMPro.TextMeshPro>();
        }
        public void Notify(string Text, int notificationNo, string textureName = null, float showtime=3)
        {
           
            time = showtime;
            this.notificationNo = notificationNo;
            if (tmpro==null)
            {
                tmpro = GetComponentInChildren<TMPro.TextMeshPro>();
            }
            if (tmpro != null)
            {
                tmpro.text = Text;
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * Slam.Instance.DeviceManager.PresentationDistanceCorrection;
            }
            if(textureName != null)
            {
                var imgCubeT = transform.Find("imgCube");
                if (imgCubeT != null)
                {
                    imgCube = imgCubeT.gameObject;
                
                    Renderer renderer = imgCube.GetComponent<Renderer>();
                    renderer.material.mainTexture = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
                }

            }
        }
        void Update()
        {
            if(time<0)
            {
               Slam.Instance.HideNotification(gameObject);
            }
            else
            {
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * Slam.Instance.DeviceManager.PresentationDistanceCorrection+Vector3.up*(0.4f- notificationNo*0.1f);
                transform.LookAt(Camera.main.transform);
                time -= Time.deltaTime;
            }
        }
    }
}
