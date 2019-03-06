using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace Slam
{
    public class Handdraggable : MonoBehaviour,IFocusable, IInputHandler, ISourceStateHandler, ISelectHandler
    {
        public bool IsHandDragable;
        public bool IsHandRotatable;
        public Transform HostTransform=null;
        Transform GetactiveTransform()
        {
            if(HostTransform!=null)
            {
                return HostTransform;
            }
            return transform;
        }

        public void OnInputDown(InputEventData eventData)
        {
 
        }

        public void OnInputUp(InputEventData eventData)
        {
            //  throw new NotImplementedException();
            Slam.Instance.Handdraggable = null;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
       //     Slam.Instance.Handdraggable = null;
        }

        public void OnFocusEnter()
        {
           // throw new NotImplementedException();
        }

        public void OnFocusExit()
        {
           //if(Slam.Instance.Handdraggable==this.transform)
           // {
                //Slam.Instance.Handdraggable = null;
            //}
        }

        public void OnSelectPressedAmountChanged(SelectPressedEventData eventData)
        {
            if (Slam.Instance.Handdraggable == null)
            {

                if (eventData.PressedAmount > 0.8)
                {

                    if (eventData.currentInputModule != null && eventData.currentInputModule.transform != null)
                    {

                        if (eventData.InputSource is InteractionInputSource)
                        {
                            InteractionInputSource gis = (InteractionInputSource)eventData.InputSource;
                            var sdata = gis.transform;
                            
                            Slam.Instance.SetHandDraggable(GetactiveTransform(), gis, eventData.SourceId, IsHandDragable, IsHandRotatable);
                        }
                    }
                }
                else if (eventData.PressedAmount<0.3)
                {
                    Slam.Instance.Handdraggable = null;
                }
            }
            else
            {
                Slam.Instance.Handdraggable = null;
            }
        }
    }
}
