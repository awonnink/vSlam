using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WSA
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
#endif
using System;

namespace Slam
{

    public class SlamObjectHL : MonoBehaviour,
#if UNITY_WSA
        IInputClickHandler, IFocusable, //IManipulationHandler,
#endif
        ISlamObject
    {
        public string Href { get; set; }

        public Target Target { get; set; }

        public bool WalkFloor { get; set; }
        public bool SitPlane = false;
        public bool showWorkflowIndicator = false;



        float DragSpeed = 1.5f;

        float DragScale = 1.5f;

        float MaxDragDistance = 3f;
        Vector3 lastPosition;
        bool draggingEnabled = true;
        // Use this for initialization
        void Start()
        {
        }

        int counter = 0;
        // Update is called once per frame
        public virtual void Update()
        {
#if UNITY_WSA
            //if (!Slam.Instance.DeviceManager.IsOpaque)
            //{
            //    if (showWorkflowIndicator)
            //    {
            //       Slam.Instance.ShowWorkFloorIndicator(true, GazeManager.Instance.HitPosition);
            //    }
            //    else
            //    {
            //        Slam.Instance.ShowWorkFloorIndicator(false, Vector3.zero);
            //    }
            //}
#endif
        }
        public void ShowWorkFloorIndicator(bool show)
        {
#if UNITY_WSA
            //if (!Slam.Instance.DeviceManager.IsOpaque)
            //{
            if (show)
            {
                Slam.Instance.ShowWorkFloorIndicator(true, GazeManager.Instance.HitPosition);
            }
            else
            {
                Slam.Instance.ShowWorkFloorIndicator(false, Vector3.zero);
            }
            //}
#endif
        }

#if UNITY_WSA
        public void OnInputClicked(InputClickedEventData eventData)
        {

            if (!Slam.Instance.DeviceManager.IsOpaque)
            {
                //eventData.
                DoSelect(GazeManager.Instance.HitPosition);
            }

        }
#endif
        public void OnFocusEnter()
        {
            StartGaze();
        }

        public void OnFocusExit()
        {
            EndGaze();
        }

        public virtual void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
        }

        public virtual void StartGaze()
        {
        }

        public virtual void EndGaze()
        { }
#if UNITY_WSA

        //public void OnManipulationStarted(ManipulationEventData eventData)
        //{
        //    InputManager.Instance.PushModalInputHandler(gameObject);
        //    lastPosition = transform.position;
        //}

        //public void OnManipulationUpdated(ManipulationEventData eventData)
        //{
        //    if (draggingEnabled)
        //    {
        //        Drag(eventData.CumulativeDelta);

        //        //sharing & messaging
        //        //SharingMessages.Instance.SendDragging(Id, eventData.CumulativeDelta);
        //    }
        //}

        //public void OnManipulationCompleted(ManipulationEventData eventData)
        //{
        //    InputManager.Instance.PopModalInputHandler();
        //}

        //public void OnManipulationCanceled(ManipulationEventData eventData)
        //{
        //    InputManager.Instance.PopModalInputHandler();
        //}

        //void Drag(Vector3 positon)
        //{
        //    var targetPosition = lastPosition + positon * DragScale;
        //    if (Vector3.Distance(lastPosition, targetPosition) <= MaxDragDistance)
        //    {
        //        transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
        //    }
        //}
#endif
    }
}
