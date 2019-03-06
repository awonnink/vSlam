// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;
#endif
#else
using UnityEngine.VR;
#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script teleports the user to the location being gazed at when Y was pressed on a Gamepad.
    /// </summary>
    [RequireComponent(typeof(SetGlobalListener))]
    public class MRTeleport : Singleton<MRTeleport>, IControllerInputHandler, ISelectHandler, ISourcePositionHandler, ISourceRotationHandler
    {
        [Tooltip("Name of the thumbstick axis to check for teleport and strafe.")]
        public XboxControllerMappingTypes HorizontalStrafe = XboxControllerMappingTypes.XboxLeftStickHorizontal;

        [Tooltip("Name of the thumbstick axis to check for movement forwards and backwards.")]
        public XboxControllerMappingTypes ForwardMovement = XboxControllerMappingTypes.XboxLeftStickVertical;

        [Tooltip("Name of the thumbstick axis to check for rotation.")]
        public XboxControllerMappingTypes HorizontalRotation = XboxControllerMappingTypes.XboxRightStickHorizontal;

        [Tooltip("Name of the thumbstick axis to check for rotation.")]
        public XboxControllerMappingTypes VerticalRotation = XboxControllerMappingTypes.XboxRightStickVertical;

        [Tooltip("Custom Input Mapping for horizontal teleport and strafe")]
        public string LeftThumbstickX = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL;

        [Tooltip("Name of the thumbstick axis to check for movement forwards and backwards.")]
        public string LeftThumbstickY = InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL;

        [Tooltip("Custom Input Mapping for horizontal rotation")]
        public string RightThumbstickX = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL;

        [Tooltip("Custom Input Mapping for vertical rotation")]
        public string RightThumbstickY = InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL;

        public bool EnableTeleport = true;
        public bool EnableRotation = true;
        public bool EnableStrafe = true;

        public float RotationSize = 45.0f;
        public float StrafeAmount = 0.5f;
        GameObject cursor;
        Slam.LineDrawer linedrawer = new Slam.LineDrawer();

        [SerializeField]
        private GameObject teleportMarker;
        private Animator animationController;

        [SerializeField]
        private bool useCustomMapping;

        /// <summary>
        /// The fade control allows us to fade out and fade in the scene.
        /// This is done to improve comfort when using an immersive display.
        /// </summary>
        private FadeManager fadeControl;

        private bool isTeleportValid;
        private IPointingSource currentPointingSource;
        private uint currentSourceId;
        float waitTime = 0;
        private void Start()
        {
            cursor = GameObject.Find("Cursor");
            FadeManager.AssertIsInitialized();

            fadeControl = FadeManager.Instance;

            // If our FadeManager is missing, or if we're on the HoloLens
            // Remove this component.
#if UNITY_2017_2_OR_NEWER
            if (!XRDevice.isPresent ||
#if UNITY_WSA
                !HolographicSettings.IsDisplayOpaque ||
#endif
                fadeControl == null)
#else
            if (VRDevice.isPresent || fadeControl == null)
#endif
            {
//                Destroy(this);
//                return;
            }

            if (teleportMarker != null)
            {
                teleportMarker = Instantiate(teleportMarker);
                teleportMarker.SetActive(false);

                animationController = teleportMarker.GetComponentInChildren<Animator>();
                if (animationController != null)
                {
                    animationController.StopPlayback();
                }
            }
        }

        private void Update()
        {
#if UNITY_WSA
            if (InteractionManager.numSourceStates == 0)
            {
                HandleGamepad();
            }
#endif

            if (currentPointingSource != null)
            {
                PositionMarker();
            }
            if(waitTime>0)
            {
                waitTime -= Time.deltaTime;
            }
        }

        private void HandleGamepad()
        {
            if (EnableTeleport && !fadeControl.Busy)
            {
                float leftX = Input.GetAxis(useCustomMapping ? LeftThumbstickX : XboxControllerMapping.GetMapping(HorizontalStrafe));
                float leftY = Input.GetAxis(useCustomMapping ? LeftThumbstickY : XboxControllerMapping.GetMapping(ForwardMovement));

                if (currentPointingSource == null && leftY > 0.8 && Math.Abs(leftX) < 0.3)
                {
                    if (FocusManager.Instance.TryGetSinglePointer(out currentPointingSource))
                    {
                        StartTeleport();
                    }
                }
                else if (currentPointingSource != null && new Vector2(leftX, leftY).magnitude < 0.2)
                {
                    FinishTeleport();
                }
            }

            if (EnableStrafe && currentPointingSource == null && !fadeControl.Busy)
            {
                float leftX = Input.GetAxis(useCustomMapping ? LeftThumbstickX : XboxControllerMapping.GetMapping(HorizontalStrafe));
                float leftY = Input.GetAxis(useCustomMapping ? LeftThumbstickY : XboxControllerMapping.GetMapping(ForwardMovement));

                if (leftX < -0.8 && Math.Abs(leftY) < 0.3)
                {
                    DoStrafe(Vector3.left * StrafeAmount);
                }
                else if (leftX > 0.8 && Math.Abs(leftY) < 0.3)
                {
                    DoStrafe(Vector3.right * StrafeAmount);
                }
                else if (leftY < -0.8 && Math.Abs(leftX) < 0.3)
                {
                    DoStrafe(Vector3.back * StrafeAmount);
                }
            }

            if (EnableRotation && currentPointingSource == null && !fadeControl.Busy)
            {
                float rightX = Input.GetAxis(useCustomMapping ? RightThumbstickX : XboxControllerMapping.GetMapping(HorizontalRotation));
                float rightY = Input.GetAxis(useCustomMapping ? RightThumbstickY : XboxControllerMapping.GetMapping(VerticalRotation));

                if (rightX < -0.8 && Math.Abs(rightY) < 0.3)
                {
                    DoRotation(-RotationSize);
                }
                else if (rightX > 0.8 && Math.Abs(rightY) < 0.3)
                {
                    DoRotation(RotationSize);
                }
            }
        }
       public void OnSelectPressedAmountChanged(SelectPressedEventData eventData)
        {
            if (eventData.PressedAmount >0.9f && waitTime<=0)
            {
               var target = eventData.selectedObject;
           
                    Slam.SlamObject so = null;
                    if (target != null)
                    {
                        so = target.GetComponent<Slam.SlamObject>();
                        if(so != null)
                        {
                            so.DoSelect(Vector3.zero);
                            waitTime=0.5f;
                        }
                    }
                    
            }
        }
        GameObject lastTarget = null;
        Slam.SlamObject so = null;
        void IControllerInputHandler.OnInputPositionChanged(InputPositionEventData eventData)
        {
           // Debug.Log(eventData.PressType.ToString());
            var target = eventData.selectedObject;
            if (target != lastTarget)
            {
                lastTarget = target;
                if (lastTarget != null)
                {
                    so = lastTarget.GetComponent<Slam.SlamObject>();
                }
            }
            if (eventData.PressType == InteractionSourcePressInfo.Thumbstick)
            {
                if (EnableTeleport)
                {

                    if (currentPointingSource == null && eventData.Position.y > 0.8 && Math.Abs(eventData.Position.x) < 0.3)
                    {

                        if (so != null && (so.WalkFloor || so.SitPlane))
                        {
                            if (FocusManager.Instance.TryGetPointingSource(eventData, out currentPointingSource))
                            {
                                currentSourceId = eventData.SourceId;
                                StartTeleport();
                            }
                        }
                    }
                    else if (currentPointingSource != null && currentSourceId == eventData.SourceId && eventData.Position.magnitude < 0.2)
                    {
                        FinishTeleport();
                    }
                }

                if (EnableStrafe && currentPointingSource == null)
                {
                    if (eventData.Position.y < -0.8 && Math.Abs(eventData.Position.x) < 0.3)
                    {
                        DoStrafe(Vector3.back * StrafeAmount);
                    }
                }

                if (EnableRotation && currentPointingSource == null)
                {
                    if (eventData.Position.x < -0.8 && Math.Abs(eventData.Position.y) < 0.3)
                    {
                        DoRotation(-RotationSize);
                    }
                    else if (eventData.Position.x > 0.8 && Math.Abs(eventData.Position.y) < 0.3)
                    {
                        DoRotation(RotationSize);
                    }
                }
            }
            
        }

        public void StartTeleport()
        {
            if (currentPointingSource != null)// && fadeControl != null&& !fadeControl.Busy)
            {
                EnableMarker();
                PositionMarker();
            }
        }

        private void FinishTeleport()
        {
            if (currentPointingSource != null)
            {
                currentPointingSource = null;

                if (isTeleportValid)
                {
                    RaycastHit hitInfo;
                    Vector3 hitPos = teleportMarker.transform.position + Vector3.up * (Physics.Raycast(CameraCache.Main.transform.position, Vector3.down, out hitInfo, 5.0f) ? hitInfo.distance : 2.6f);

                    if(waitTime<=0)
                    {
                        waitTime = 0.5f;
                        if (fadeControl!=null && Slam.Slam.Instance.DeviceManager.IsUWP && Slam.Slam.Instance.DeviceManager.IsOpaque)
                        {
                            if (!fadeControl.Busy)
                            {
                                fadeControl.DoFade(
                                    0.25f, // Fade out time
                                    0.25f, // Fade in time
                                    () => // Action after fade out
                                    {
                                Slam.Slam.Instance.WalkTo(hitPos, 0, true);
                            }, null); // Action after fade in
                            }
                        }
                        else
                        {
                            Slam.Slam.Instance.WalkTo(hitPos, 0);
                        }


                    }
                }

                DisableMarker();
            }
        }

        public void DoRotation(float rotationAmount)
        {
            if (rotationAmount != 0 && !fadeControl.Busy)
            {
                if (waitTime <= 0)
                {
                    waitTime = 0.5f;
                    transform.RotateAround(CameraCache.Main.transform.position, Vector3.up, rotationAmount);
                }
            }
        }

        public void DoStrafe(Vector3 strafeAmount)
        {
            if (strafeAmount.magnitude != 0 && !fadeControl.Busy)
            {
                if (waitTime <= 0)
                {
                    waitTime = 0.5f;

                    Transform transformToRotate = CameraCache.Main.transform;
                        transformToRotate.rotation = Quaternion.Euler(0, transformToRotate.rotation.eulerAngles.y, 0);
                        transform.Translate(strafeAmount, CameraCache.Main.transform);
                }
            }
        }

        /// <summary>
        /// Places the player in the specified position of the world
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            // There are two things moving the camera: the camera parent (that this script is attached to)
            // and the user's head (which the MR device is attached to. :)). When setting the world position,
            // we need to set it relative to the user's head in the scene so they are looking/standing where 
            // we expect.
            transform.position = worldPosition - (CameraCache.Main.transform.position - transform.position);
        }

        private void EnableMarker()
        {
            teleportMarker.SetActive(true);
            if (animationController != null)
            {
                animationController.StartPlayback();
            }
        }

        private void DisableMarker()
        {
            if (animationController != null)
            {
                animationController.StopPlayback();
            }
            teleportMarker.SetActive(false);
        }

        private void PositionMarker()
        {
            FocusDetails focusDetails = FocusManager.Instance.GetFocusDetails(currentPointingSource);

            if (focusDetails.Object != null && (Vector3.Dot(focusDetails.Normal, Vector3.up) > 0.90f))
            {
                isTeleportValid = true;

                teleportMarker.transform.position = focusDetails.Point;
            }
            else
            {
                isTeleportValid = false;
            }
            if (animationController != null)
            {
                animationController.speed = isTeleportValid ? 1 : 0;
            }
        }

        public void OnPositionChanged(SourcePositionEventData eventData)
        {
            //if (cursor != null && cursor.transform != null && eventData != null)
            //{

            //    RaycastHit hit;
            //    Ray ray = new Ray(Camera.main.transform.position, -Vector3.up);
            //    if (eventData.InputSource.TryGetPointingRay(eventData.SourceId, out ray))
            //    {
            //        if (Physics.Raycast(ray, out hit, 100.0f))
            //        {
            //            var pos2 = hit.point;
            //            var pos = eventData.GripPosition;
            //            //                   var pos2 = cursor.transform.position;
            //            StartCoroutine(linedrawer.DrawLine(this.transform, pos, pos2, Color.red));

            //        }
            //    }
               
            //}
            ////throw new NotImplementedException();
        }

        public void OnRotationChanged(SourceRotationEventData eventData)
        {
            //var a= eventData.currentInputModule.transform.parent.name;
            //InteractionSourceInfo si;
            //if(eventData.currentInputModule..InputSource.TryGetSourceKind(eventData.SourceId, out si))
            //{
            //   // si.
            //}
            //throw new NotImplementedException();
        }
    }
}
