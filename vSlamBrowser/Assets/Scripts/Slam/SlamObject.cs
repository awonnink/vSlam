using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Slam
{

    public class SlamObject : SlamObjectHL
    {
        Vector3 defaultScale;
        public List<Constraint> Constraints = new List<Constraint>();
        List<Constraint> TempConstraints = null;
        public Vector3 Rotate = Vector3.zero;
        public Vector3 Center = Vector3.zero;
        public Vector3 ClingToCamera = Vector3.zero;
        Vector3 defaultPosition = Vector3.zero;
        Vector3 defaultLocalPosition = Vector3.zero;
        Quaternion defaultRotation = Quaternion.identity;
        Vector3 newPosition = Vector3.zero;
        Quaternion newRotation = Quaternion.identity;

        public bool conFirmLeaveApp = true;
        public bool ApplyMovementToParent = false;
        public InputType InputType = InputType.None;
        public string FormFieldName = null;
        public string FormFieldValue = null;
        public float floorLevel = 0;
        public float presentation = 0;
        public float presentationangle = 0;
        public float presentationspeed = 0;
        public float kinetic = 0;
        public bool presenting = false;
        public bool isHandDragable = false;
        bool move = false;
        public string FaceCamera = null;
        public string ToolTip = null;
        bool sceneInitiated = false;
        public string ActiveTransformName = null;

        //serverObject
        //public bool IsServerObject = false;
        //public Vector3 serverPosition;
        //public Quaternion serverRotation;
        //public Vector3 serverVelocity;
        //public Vector3 serverAngularVelocity;

        Transform activeTransform;
        Rigidbody rb = null;
        public Transform ActiveTransform
        {
            get
            {
                if(activeTransform==null)
                {
                   if(!string.IsNullOrEmpty(ActiveTransformName))
                    {
                        var o = GameObject.Find(ActiveTransformName);
                        if(o!=null)
                        {
                            activeTransform = o.transform;
                        }
                    }
                    if (activeTransform == null)
                    {
                        activeTransform = ApplyMovementToParent && transform.parent != null ? transform.parent : transform;
                    }
                    defaultScale = activeTransform.localScale;
                    defaultLocalPosition = activeTransform.localPosition;
                    if (activeTransform.parent != null)
                    {
                        defaultRotation = activeTransform.parent.rotation;
                    }
                }
                return activeTransform;
            }
        }
        // Use this for initialization
        public virtual void Start()
        {
            //serverPosition = activeTransform.position;
            //serverRotation = activeTransform.rotation;
            //rb = GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    serverVelocity = rb.velocity;
            //    serverAngularVelocity = rb.angularVelocity;
            //}
            //Target = Target._blank;
        }

        void CheckKinetic(Vector3 hitPos)
        {
            if(kinetic>0)
            {
                Vector3 direction = transform.position - hitPos;
                if(direction.magnitude>0)
                {
                    direction.Normalize();
                    var rb = GetComponent<Rigidbody>();
                    if(rb!=null)
                    {
                        rb.AddForce(kinetic * direction*UnityEngine.Random.Range(0.5f, 1.1f), ForceMode.Impulse);
                    }
                }
            }
        }
        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            //if(IsServerObject)
            //{
            //    transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime);
            //    transform.rotation = Quaternion.Lerp(transform.rotation, serverRotation, Time.deltaTime);
            //    if (rb != null)
            //    {
            //        rb.velocity=Vector3.Lerp(rb.velocity,  serverVelocity, Time.deltaTime);
            //        rb.angularVelocity= Vector3.Lerp(rb.angularVelocity, serverAngularVelocity, Time.deltaTime);
            //    }

            //}
            if (!sceneInitiated && Scene.Instance!=null)
            {
                defaultPosition = ActiveTransform.position - Scene.Instance.transform.position;
                if (!presenting)
                {
                    newPosition = defaultPosition;
                    newRotation = defaultRotation;
                }
                sceneInitiated = true;
            }
            if (Rotate != Vector3.zero)
            {
                //Transform t = ApplyMovementToParent ? transform.parent : transform;
                if (Center == Vector3.zero)
                {
                    ActiveTransform.parent.Rotate(Rotate, Rotate.magnitude * Time.deltaTime * 10.0F);
                }
                else
                {
                    ActiveTransform.RotateAround(Center, Rotate, Rotate.magnitude * Time.deltaTime * 10.0F);
                }
            }
            if (presentation > 0 && move)
            {
                if (ActiveTransform.parent != null)
                {
                    ActiveTransform.parent.rotation = Quaternion.Lerp(ActiveTransform.parent.rotation, newRotation, Time.deltaTime);
                }
                var presSpeed = presentationspeed > 0 ? presentationspeed : 1;
                if (presenting)
                {
                    ActiveTransform.position = Vector3.Lerp(ActiveTransform.position, newPosition, Time.deltaTime* presSpeed);
                    if (Vector3.Distance(ActiveTransform.position, newPosition) < 0.01f)
                    {
                        move = false;
                    }
                }
                else
                {
                    ActiveTransform.localPosition = Vector3.Lerp(ActiveTransform.localPosition, defaultLocalPosition, Time.deltaTime* presSpeed);
                    if (Vector3.Distance(ActiveTransform.localPosition, defaultLocalPosition) < 0.01f)
                    {
                        ActiveTransform.localPosition = defaultLocalPosition;
                        move = false;
                    }
                }

            }
            else if (FaceCamera != null && !presenting)
            {
                if (FaceCamera.Contains("face") || FaceCamera.Contains("back"))
                {
                    Transform t = FaceCamera.Contains("parent") && ActiveTransform.parent!=null ? ActiveTransform.parent : ActiveTransform;
                    if (FaceCamera.Contains("lock-y"))
                    {
                        t.LookAt(new Vector3(Camera.main.transform.position.x, t.position.y, Camera.main.transform.position.z));
                        //                   t.LookAt(new Vector3(Camera.main.transform.position.x, t.position.y, Camera.main.transform.position.z));
                    }
                    else
                    {
                        t.LookAt(Camera.main.transform.position);
                    }
                    if (FaceCamera.Contains("back"))
                    {
                        t.Rotate(Vector3.up, 180);
                    }
                }
            }

            if (ClingToCamera != Vector3.zero)
            {
                //Transform t2 = ApplyMovementToParent ? transform.parent : transform;
                //var angle = Vector3.Angle(Vector3.forward, Camera.main.transform.forward);
                //var rotVect = Quaternion.AngleAxis(angle, Vector3.up) * ClingToCamera;
                var camT = Camera.main.transform;
                var tPosition = camT.position + ClingToCamera.x * camT.right+ ClingToCamera.y* camT.up + ClingToCamera.z*camT.forward;
                if(FaceCamera.Contains("lock-y"))
                {
                    tPosition = new Vector3(tPosition.x, ClingToCamera.y, tPosition.z);
                }
                if(Vector3.Distance(ActiveTransform.position, tPosition)>0.4f)
                {
                    ActiveTransform.position = Vector3.Lerp(ActiveTransform.position, tPosition, Time.deltaTime);
                }
            }
            if (Slam.Instance.Handdraggable!=activeTransform)
            {
                ImplementConstraints(activeTransform, Constraints);
            }
        }
        private void OnMouseOver()
        {
            if ( !Slam.Instance.IsHoloLens())
            {
                //Vector3 hitPoint;
                //if (TryGetMousePosition(out hitPoint))
                //{
                    Slam.Instance.showWorkflowIndicator= WalkFloor;
                //}

            }
        }
        private void OnMouseEnter()
        {
            if (!Slam.Instance.IsHoloLens())
            {
                StartGaze();
            }
        }
        private void OnMouseExit()
        {

            if (!Slam.Instance.IsHoloLens())
            {
                //Slam.Instance.ShowWorkFloorIndicator(false, Vector3.zero);
                EndGaze();
            }
        }
        void OnMouseUp()
        {
            if (startMouseOffset==Vector3.zero || (startMouseOffset-Input.mousePosition).magnitude < 0.01)
            {
                if ((WalkFloor || SitPlane))
                {
                    Vector3 hitPoint;
                    if (Slam.Instance.TryGetMousePosition(out hitPoint))
                    {
                        DoSelect(hitPoint);
                    }
                }
                else
                {
                    DoSelect(Vector3.zero);
                }
            }
            else
            {
                //dragging=false;
            }
            startMouseOffset = Vector3.zero;
        }

        Vector3 offset = Vector3.zero;
        Vector3 screenPoint = Vector3.zero;
        Vector3 startMouseOffset= Vector3.zero;
        void OnMouseDown()
        {
            if (isHandDragable)
            {
                screenPoint = Camera.main.WorldToScreenPoint(activeTransform.position);
                offset = activeTransform.position - Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                startMouseOffset = Input.mousePosition;
            }
        }
        void OnMouseDrag()
        {
            //if (!HDChecked)
            //{
            //    checkHandDraggableScript();
            //}
            if (isHandDragable)
            {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                transform.position = curPosition;
            }
        }
        //public virtual void DoSelect()
        //{
        //    DoSelect(Vector3.zero);
        //}
        public override void DoSelect(Vector3 position, bool checkActionRecording=false)
        {
            CheckKinetic(position);
            if (position!=Vector3.zero && WalkFloor)
            {
                Debug.Log("Walk to: " + position.ToString());
                Slam.Instance.WalkTo(position, floorLevel);
            }
            if (position != Vector3.zero && SitPlane)
            {
                Slam.Instance.SitOn(position, ActiveTransform.forward);
            }
            else if (InputType!= InputType.None)
            {
                //SlamMenu.Instance.SetHelpText()
                Slam.Instance.OpenMenuForTextInput(gameObject);

            }
            else if (!string.IsNullOrEmpty(Href))
            {
//                SlamMenu.Instance.SaveTextInput();
                if(Href.ToLower().StartsWith("slam_avatarselect="))
                {
                    var no = Href.ToLower().Replace("slam_avatarselect=", "");
                    Slam.Instance.AvatarNo = no;
                    Slam.Instance.GoHome();
                    return;
                }
                switch(Href)
                {
                    case Constants.slam_menu:
                        Slam.Instance.OpenMenu(transform.position);
                        break;
                    case Constants.slam_home:
                        Slam.Instance.GoHome();
                        break;
                    case Constants.slam_close_message:
                        Slam.Instance.ShowMessage(false);
                        break;
                    case Constants.slam_ok_message:
                        Slam.Instance.MessageOKAction();
                        break;
                    default:
                        switch (Target)
                        {
                            case Target._2D:
                                if (conFirmLeaveApp)
                                {
                                    Slam.Instance.Browse2d(Href);
                                }
                                else
                                {
                                    Application.OpenURL(Href);
                                }
                                break;
                            default:
                                checkActionRecording = true;
                                if (!Slam.Instance.Switch(Href, ActiveTransform.gameObject))//, gameObject.name))
                                {
                                    Slam.Instance.Parse(Href, Target, null);

                                }
                                break;
                        }
                        break;
                }
            }
            if(presentation>0)
            {
                checkActionRecording=true;
                if (presenting)
                {
                    StopPresenting();
                }
                else
                {
                    SetPresenting();
                }
                move = true;
            }
            if(checkActionRecording)
            {
                Slam.Instance.QueryAction(gameObject.name);//record action
            }
        }
        public void StopPresenting()
        {
            newPosition = defaultPosition+Scene.Instance.transform.position;
            newRotation = defaultRotation;
            presenting = false;

        }
        public void SetPresenting()
        {
            newPosition = Camera.main.transform.position + presentation * Camera.main.transform.forward;
            newRotation = Camera.main.transform.rotation *Quaternion.Euler(0, 180+presentationangle,0);
            presenting = true;
            move = true;
        }

        public void MoveTo(Vector3 pos, Quaternion rot)
        {
            ApplyMovementToParent = true;
            newPosition = pos;
            newRotation = rot;
            presenting = true;
            presentationspeed = 0.002f;
            move = true;

        }
        Vector3 tempScale = Vector3.zero;
        float gazescale = 1;
        public override void StartGaze()
        {
            if (!string.IsNullOrEmpty(Href) && defaultScale.magnitude>0.000001f)
            {
                tempScale = ActiveTransform.localScale;
                gazescale = 1.1f;
                ActiveTransform.localScale  *= gazescale;
            }
            if (WalkFloor && Slam.Instance.DeviceManager.IsUWP)
            {
                Slam.Instance.showWorkflowIndicator=true;
            }
            Slam.Instance.ShowToolTip(ToolTip);
            Slam.Instance.SetSelectedSlamObject(this);
           // Slam.Instance.gazedObject = this;
        }

        public override void EndGaze()
        {
            if (!string.IsNullOrEmpty(Href) && defaultScale.magnitude > 0.000001f)
            {
                ActiveTransform.localScale = tempScale==Vector3.zero? defaultScale:tempScale;
                gazescale = 1;
            }
            Slam.Instance.ShowToolTip("");
            if (WalkFloor && Slam.Instance.DeviceManager.IsUWP)
            {
                Slam.Instance.showWorkflowIndicator = false;
            }
            Slam.Instance.SetSelectedSlamObject(null);
            //Slam.Instance.gazedObject = null;
        }


        #region constraints
        float defaultSpeed = 5;
        public void ImplementConstraints(Transform activeTransform, List<Constraint> constraints)
        {
            foreach (var constraint in constraints)
            {
                if (constraint.Enabled)
                {
                    ImplementConstraint(activeTransform, constraint);
                }
            }
            if(TempConstraints!=null)
            {
                constraints.AddRange(TempConstraints);
                TempConstraints = null;
            }
        }
        void ImplementConstraint(Transform activeTransform, Constraint constraint)
        {
            switch (constraint.ConstraintType)
            {
                case ConstraintType.sphere:
                    ImplementSphereConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.scaledsphere:
                    ImplementSphereConstraint(activeTransform, constraint, true);
                    break;
                case ConstraintType.block:
                    ImplementBlockConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.scale:
                    ImplementScaleConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.delete:
                    ImplementDeleteConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.snap:
                    ImplementSnapConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.selectclose:
                    ImplementSelectDistanceConstraint(activeTransform, constraint, true);
                    break;
                case ConstraintType.selectfar:
                    ImplementSelectDistanceConstraint(activeTransform, constraint);
                    break;
                case ConstraintType.constraintgroup:
                    if (TempConstraints == null)
                    {
                        ImplementConstraintGroup(constraint);
                    }
                    break;
            }
        }
        void ImplementScaleConstraint(Transform activeTransform, Constraint constraint)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
            }
            if(constraint.Target!=null && constraint.Target.activeSelf && constraint.Range>0)
            {
                var dist = Vector3.Distance(constraint.Target.transform.position, activeTransform.position);
                //if(dist>0)
                //{
                    var newScale = Mathf.Pow(1/(1+dist), constraint.Range)*defaultScale*gazescale;
                    activeTransform.localScale = Vector3.Lerp(activeTransform.localScale, newScale, defaultSpeed*Time.deltaTime * constraint.Speed);
                //}
            }

        }
        void ImplementSphereConstraint(Transform activeTransform, Constraint constraint, bool scaled=false)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
                if(constraint.Target!=null)
                {
                    constraint.TargetSO = constraint.Target.GetComponentInChildren<SlamObject>(); ;
                }
            }
            if (constraint.Target != null && constraint.Target.activeSelf)
            {
                if (Vector3.Distance(activeTransform.position, constraint.Target.transform.position) < 0.01)
                {
                    activeTransform.position += Statics.GetRandomUnitVector() * 0.015f;
                }
                float scaleFactor = 1;
                if (scaled)
                {
                    float targetScaleFactor = 1;
                    if (constraint.TargetSO != null && constraint.TargetSO.defaultScale.magnitude > 0 && defaultScale.magnitude>0)
                    {
                        targetScaleFactor = constraint.TargetSO.transform.localScale.magnitude / constraint.TargetSO.defaultScale.magnitude;
                        var localScaleFactor = transform.localScale.magnitude / defaultScale.magnitude;
                        scaleFactor = (targetScaleFactor + localScaleFactor) / 2;
                    }
                }
                var direction = constraint.Target.transform.position - activeTransform.position;
                var ndirection = direction.normalized * constraint.Range* scaleFactor;
                //var diif = direction - ndirection;
                //if (diif.magnitude>0.01)
                //{
                var correctpos = constraint.Target.transform.position - ndirection;
                activeTransform.position = Vector3.Lerp(activeTransform.position, correctpos, defaultSpeed * Time.deltaTime * constraint.Speed);
                if(constraint.Borders.magnitude>0)
                {
                    activeTransform.position = Vector3.Lerp(activeTransform.position, activeTransform.position+ constraint.Borders, defaultSpeed * Time.deltaTime * constraint.Speed);
                }
                //}
            }
        }
        void ImplementBlockConstraint(Transform activeTransform, Constraint constraint)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
            }
            if (constraint.Target != null && constraint.Target.activeSelf)
            {
                var borders = constraint.Borders;
                Vector3 correctpos = new Vector3();
                correctpos.x = CheckAxisCorrection(activeTransform.position.x, constraint.Target.transform.position.x, borders.x);
                correctpos.y = CheckAxisCorrection(activeTransform.position.y, constraint.Target.transform.position.y, borders.y);
                correctpos.z = CheckAxisCorrection(activeTransform.position.z, constraint.Target.transform.position.z, borders.z);
                if (correctpos.magnitude > 0.01)
                {
                    if (correctpos.x == 0) { correctpos.x = activeTransform.position.x; }
                    if (correctpos.y == 0) { correctpos.y = activeTransform.position.y; }
                    if (correctpos.z == 0) { correctpos.z = activeTransform.position.z; }
                    activeTransform.position = Vector3.Lerp(activeTransform.position, correctpos, defaultSpeed * Time.deltaTime * constraint.Speed);
                }
            }
        }
        void ImplementSelectDistanceConstraint(Transform activeTransform, Constraint constraint, bool close=false)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
            }
            if (constraint.Target != null && constraint.Range > 0)
            {
                var dist = Vector3.Distance(constraint.Target.transform.position, activeTransform.position);
                if ((close && dist < constraint.Range) || (!close && dist > constraint.Range))
                {
                    var so = constraint.Target.GetComponentInChildren<SlamObject>();
                    if(so!=null)
                    {
                        so.DoSelect(activeTransform.transform.position);
                        constraint.Enabled = false;
                    }
                }
            }

        }
        void ImplementDeleteConstraint(Transform activeTransform, Constraint constraint)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
            }
            if (constraint.Target != null && constraint.Range > 0)
            {
                var dist = Vector3.Distance(constraint.Target.transform.position, activeTransform.position);
                if (dist < constraint.Range)
                {
                    activeTransform.gameObject.SetActive(false);
                }
            }

        }
        void ImplementSnapConstraint(Transform activeTransform, Constraint constraint)
        {
            if (constraint.Target == null && !string.IsNullOrEmpty(constraint.TargetName))
            {
                constraint.Target = GameObject.Find(constraint.TargetName);
            }
            if (constraint.Target != null && constraint.Target.activeSelf && constraint.Range > 0)
            {
                var dist = Vector3.Distance(constraint.Target.transform.position, activeTransform.position);
                if (dist < constraint.Range)
                {
                    activeTransform.position = Vector3.Lerp(activeTransform.position, constraint.Target.transform.position, defaultSpeed * Time.deltaTime * constraint.Speed);
                }
            }

        }

        void ImplementConstraintGroup(Constraint constraint)
        {
            List<Constraint> list = null;
            if (!string.IsNullOrEmpty(constraint.TargetName) && Slam.Instance.constraintgroups.TryGetValue(constraint.TargetName, out list))
            {
                TempConstraints = list;
                constraint.Enabled = false;
            }
        }
        float CheckAxisCorrection(float transformX, float targetX, float borderX)
        {
            var borderAbs = Math.Abs(borderX);
            var posX = targetX + borderAbs;
            if (transformX > posX)
            {
                return posX;
            }
            else
            {
                posX = targetX - borderAbs;
                if (transformX < posX)
                {
                    return posX;
                }
            }
            return 0;
        }

        #endregion
    }
    public enum Target
    {
        _blank,
        _self,
        _inline,
        _2D

    }
    public interface ISlamObject
    {
        void DoSelect(Vector3 vector, bool checkActionRecording = false);
        void StartGaze();
        void EndGaze();
        string Href { get; set; }
        bool WalkFloor { get; set; }
        Target Target { get; set; }
    }
}
