using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WSA && !UNITY_EDITOR
using HoloToolkit.Unity.InputModule;
#endif
using UnityEngine;
using HoloToolkit.Unity;

namespace Slam
{ 
    public class Scene : Singleton<Scene>
    {
 
        public Vector3 walkTo = Vector3.zero;
        public Vector3 startPosition = Vector3.zero;
        public bool fast = false;
        public Vector3 StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;
                walkTo = value;
            }
        }
        void Start()
        {
            Init();
        }
        public void Init(bool setStartPosition = false)
        {
            if (setStartPosition)
            {
                transform.position = startPosition;
            }
            walkTo = transform.position;
        }
        // Update is called once per frame
        void Update()
        {
            Vector3 lastPosition = transform.position;
            if (Vector3.Distance(transform.position, walkTo) > 0.01F)
            {
                float fact = fast ? 10 : 1;
                transform.position = Vector3.Lerp(transform.position, walkTo, fact*Time.deltaTime);
                var dist = transform.position - lastPosition;
                if (dist.magnitude > 0)
                {
                    Slam.Instance.CorrectAvatarsPosition(dist);
                }
            }
            else
            {
                transform.position = walkTo;
            }
        }
    }
}
