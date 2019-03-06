using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class Avatar : SlamObject
    {
        public Vector3 NewPosition;
        public Quaternion NewRotation;
        public bool IsOwnAvatar = false;
        public bool Controlled = false;
        public float AvatarLength = 1.8F;
//        bool moveChecking = false;
        MoveState moveState = MoveState.idle;
        Animator anim;
        Transform neckT = null;
        Avatarcorrection avatarCorrection=null;
        GameObject avatarModel = null;
        float yCorrection = 0;
        float ySitCorrection = 0.45f;
        string currentTrigger = MoveState.idle.ToString();

        Vector3 previousPosition;
        public string AvatarId;
        public string AvatarDissId;
        public string NickName;

        public Transform leftHandT;
        public Transform rightHandT;
        // Use this for initialization
        public override void Start()
        {
            InitiateAvatar();
           // InvokeRepeating("moveCheck", 0, 1.0f);
            previousPosition = GetSceneRelativePosition();
        }
        Vector3 GetSceneRelativePosition()
        {
            return transform.localPosition;
        }
        void CheckMoveState()
        {
            if(anim==null)
            {
                anim = GetComponentInChildren<Animator>();
                anim.SetTrigger(currentTrigger);
            }
            if (anim != null)
            {
                //var a= GetAnimationClip("idle2");
                //var a = anim.GetComponent<Animation>();
                //Debug.Log("animation playing: "+ a.name);
            }
        }
        public AnimationClip GetAnimationClip(string name)
        {
            if (!anim) return null; // no animator

            foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            {
                if (clip.name == name)
                {
                    return clip;
                }
            }
            return null; // no clip by that name
        }
        public void InitiateAvatar()
        {
            NewPosition = transform.position;
            NewRotation = transform.rotation;
            neckT = transform.FindDeepChild(Constants.avatar_neck_bone_name);

            anim = GetComponentInChildren<Animator>();
            avatarCorrection = GetComponentInChildren<Avatarcorrection>();
            if(avatarCorrection!=null)
            {
                avatarModel = avatarCorrection.gameObject;
                yCorrection = avatarCorrection.y_correction;
                ySitCorrection = avatarCorrection.y_sitcorrection;
            }
            if (anim!=null)
            {
                SwitchRandomAnimation(MoveState.idle);
            }
            //hands
            InitiateHands();
        }
        public void InitiateHands()
        {
            if (leftHandT==null && rightHandT==null)
            {
                GameObject lgo = new GameObject();
                lgo.name = "leftHand";
                lgo.transform.SetTheParent(transform);

                var lh = Slam.Instance.GetHandPrefab(lgo.transform, true);
               // lh.transform.localScale *= 2;
                leftHandT = lgo.transform;
                lgo.SetActive(false);
                GameObject rgo = new GameObject();
                rgo.name = "rightHand";
                rgo.transform.SetTheParent(transform);
                var rh = Slam.Instance.GetHandPrefab(rgo.transform, false);
               // rh.transform.localScale *= 2;
                rightHandT = rgo.transform;
                rgo.SetActive(false);
            }
        }
        private void HandleNeckRotation()
        {
            if(neckT!=null)
            {
                Quaternion nr = Quaternion.Euler(NewRotation.eulerAngles.x, NewRotation.eulerAngles.y, 0);
                neckT.rotation = Quaternion.Lerp(neckT.rotation, nr, 1.6f * Time.deltaTime);
            }
        }
        // Update is called once per frame
        public override void Update()
        {
            //if (Camera.main != null)
            //{
            //    transform.position = Camera.main.transform.position + 0.5f * Camera.main.transform.forward;
            //    transform.LookAt(Camera.main.transform.position + Vector3.forward);
            //}
            if (Controlled)
            {
                if (!IsOwnAvatar)
                {
                    if (Vector3.Distance(transform.position, NewPosition) < 0.1f)
                    {
                        transform.position = NewPosition;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, NewPosition, Time.deltaTime);
                    }
                    if (moveState == MoveState.idle && neckT != null)
                    {
                        if(Mathf.Abs( transform.eulerAngles.y-NewRotation.eulerAngles.y)<30)
                        {
                            HandleNeckRotation();
                        }
                        else
                        {
                            Quaternion nRot = Quaternion.Euler(0, NewRotation.eulerAngles.y, 0);
                            transform.rotation = Quaternion.Lerp(transform.rotation, nRot, Time.deltaTime);
                            HandleNeckRotation();
                        }
                    }
                    else
                    {
                        Quaternion nRot = Quaternion.Euler(0, NewRotation.eulerAngles.y, 0);
                        transform.rotation = Quaternion.Lerp(transform.rotation, nRot, 5*Time.deltaTime);
                        HandleNeckRotation();
                    }
                    RandomAnimation();
                }
                else
                {
                    if (neckT != null)
                    {
                        neckT.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0);
                    }
                    transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                    transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y- AvatarLength+0.10f, Camera.main.transform.position.z);
                }
                moveCheck();
            }
        }
        float lastTime = 0;
        void RandomAnimation()
        {
            if(Random.value<0.004 && Time.timeSinceLevelLoad>lastTime+5)
            {
                lastTime = Time.timeSinceLevelLoad;
                if (moveState == MoveState.idle)
                {
                    SwitchRandomAnimation(MoveState.idle);
                }
                else if (moveState == MoveState.sit)
                {
                    SwitchRandomAnimation(MoveState.sit, 2);
                }
            }
        }
        public void SwitchRandomAnimation(MoveState moveState, int maxState=6)
        {
            currentTrigger = moveState.ToString()+(int)Random.Range(0, maxState);
            anim.SetTrigger(currentTrigger);

        }
        //       IEnumerator moveCheck()
        void moveCheck()
        {
            MoveState lmovestate = moveState;
            var p1 = new Vector2( previousPosition.x, previousPosition.z);
            previousPosition = GetSceneRelativePosition();
            var p2 = new Vector2(previousPosition.x, previousPosition.z);
            bool isMoving = Vector2.Distance(p1, p2) >Time.deltaTime *0.004f;
 
            if (isMoving)
            {
                lmovestate = MoveState.walk;
            }
            else
            {
                lmovestate = MoveState.idle;
            }
            CheckSit(lmovestate);
        }
        void CheckSit(MoveState lmovestate)
        {
           // MoveState lmovestate = moveState;
            //if( lmovestate == MoveState.sit)
            //{
            //    lmovestate = MoveState.idle;
            //}
            RaycastHit hit;
            Ray ray = new Ray(transform.position + new Vector3(0, Constants.cameraHeight, 0), -Vector3.up);
            if (Physics.Raycast(ray, out hit, 2.0f))
            {
                var hp = hit.point;
                var go = hit.collider.gameObject;
                if (go != null)
                {

                    var so = go.GetComponent<SlamObject>();
                    if (so != null && so.SitPlane)
                    {
                        lmovestate = MoveState.sit;
                    }
                }
            }
            SetMoveState(lmovestate);
        }
        void SetMoveState(MoveState m)
        {
            if (m != moveState)
            {
                moveState = m;
                var selectedYCorrection = yCorrection;

                if (anim != null)
                {
                    switch (m)
                    {
                        case MoveState.walk:
                            StartCoroutine( EnsureMoveState(Constants.avatar_walk_trigger, m));
                            break;
                        case MoveState.idle:
                            StartCoroutine(EnsureMoveState(Constants.avatar_idle_trigger, m));
                            break;
                        case MoveState.sit:
                            StartCoroutine(EnsureMoveState(Constants.avatar_sit_trigger, m));
                            selectedYCorrection = ySitCorrection;
                            break;
                    }
                }
                if (avatarModel != null)
                {
                    avatarModel.transform.localPosition = new Vector3(0, selectedYCorrection, 0);
                }
            }
        }
        IEnumerator EnsureMoveState(string trigger, MoveState movestate)
        {
            yield return new WaitForSeconds(0.1f);
            if (anim != null)
            {
                anim.SetTrigger(trigger);
            }
            switch(movestate)
            {
                case MoveState.idle:
                    SwitchRandomAnimation(MoveState.idle, 6);
                    break;
                case MoveState.sit:
                    SwitchRandomAnimation(MoveState.sit, 2);
                    break;
            }
        }

        public override void DoSelect(Vector3 position, bool checkActionRecording = false)
        {
            base.DoSelect(position);
            if (Controlled)
            {
                Slam.Instance.OpenAvatarMenu(AvatarId, AvatarDissId, NickName, transform.localPosition + Vector3.up);
            }
        }
    }
    public enum MoveState
    {
        idle,
        walk,
        sit
    }
}
