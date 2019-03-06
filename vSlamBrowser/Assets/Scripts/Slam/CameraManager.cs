using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.EventSystems;

namespace Slam
{
    public class CameraManager : Singleton<CameraManager>
    {
        Quaternion startRotation;
        Vector3 startPosition;
        public float floorLevel = 0;
        public float AvatarLengthCorrection = 0;
       // PhysicsRaycaster rc;
        void Start()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
           // rc=gameObject.AddComponent<PhysicsRaycaster>();
        }
        // Update is called once per frame
        void Update()
        {
            //CheckZoom();
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startPosition.y + floorLevel+AvatarLengthCorrection, transform.position.z), Time.deltaTime);
        }
        public void ResetTransform()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}
