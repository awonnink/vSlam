//#define USE_DISS //***Uncomment #define when using Dissonance***
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Dissonance.Integrations.UNet_LLAPI;
#if USE_DISS
using Dissonance.Integrations.UNet_HLAPI;
#endif
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityGLTF;
using GLTF;
//using GLTF.Schema;
using UnityGLTF.Cache;
using UnityGLTF.Extensions;
using UnityEngine.XR;

//#if UNITY_WSA && !UNITY_EDITOR
//using HoloToolkit.Unity;
//#endif
using TMPro;
using HoloToolkit.Unity;
using UnityEngine.Experimental.UIElements;

namespace Slam
{
    public class Slam : Singleton<Slam>
    {
        #region Fields
        public static string startupArgs;
        public string activationNavigateUrl = null;
        public float activationNavigateTime = 0;
        public bool AllowScroll = true;
        //HoloToolkit.Unity.InputModule.FocusManager focusManager;
        //public SlamObject gazedObject;
        public UrlHandler UrlHandler = new UrlHandler();
        int firstspass = 0;
        public bool showWorkflowIndicator = false;
//        private const string startRecordingAction = "slam_start_recording";
        string slam_session = null;
        string returnPage = null;
        string currentSearchTerms = null;
        string currentFilter = null;
        string currentPage = null;
        public string AvatarAction = "i";
        Downloading downloading = new Downloading();
        IDeviceManager _deviceManager = null;
#if USE_DISS

        Dissonance.VoiceBroadcastTrigger voiceBroadCastTrigger = null;
        Dissonance.VoiceReceiptTrigger voiceReceiptTrigger = null;
        Dissonance.DissonanceComms dissonanceComms = null;
#endif
        bool HasConversation = true;
        GameObject textToSpeechGO = null;
        HoloToolkit.Unity.TextToSpeech txtToSpeech;
        GameObject msgBox = null;
        GameObject myAvatarPresentation;
        public GameObject sceneRotator=null;
#if USE_DISS
        Dissonance.RoomMembership roomMembership;
#endif
        FogSettings CurrentFogSettings = null;
        int ActiveNotifications = 0;

        MessageOkType messageOkType = MessageOkType.None;
        Transform cameraTransform = null;
        bool showSmallMenu = false;
        bool hideLeaveApp = false;
        bool TryRetrieveMeshesStarted = false;
        public bool retrievingMesh = false;
        string communityInput = null;
        GameObject smallMenu = null;
        GameObject notification = null;
        public GameObject menu = null;
        SlamMenu slamMenu;
//        avatarMenu avatarMenu = null;
        GameObject leaveAppObject = null;
        GameObject waitCursor = null;
        bool hideWaitCursor = false;
        bool hideWaitCursor2 = false;
        ViewPoint _viewPoint = null;
        AudioSource audioSource;
        AudioSource audioSourceEffect;
        AudioSource audioSourceAvatar;
        Vector3 viewpointPosition = Vector3.zero;
        Inline currentInline = null;
        Dictionary<string, UnityEngine.Material> matDict = new Dictionary<string, UnityEngine.Material>();
        Dictionary<string, TextureContainer> textures = new Dictionary<string, TextureContainer>();
        Dictionary<string, TextureContainer> updatableTextures = new Dictionary<string, TextureContainer>();
        Dictionary<string, List<Inline>> inlines = new Dictionary<string, List<Inline>>();
        Dictionary<string, Inline> updatableInlines = new Dictionary<string, Inline>();
        Dictionary<string, MeshContainer> meshDict = new Dictionary<string, MeshContainer>();
        Dictionary<string, object> definitions = new Dictionary<string, object>();
        Dictionary<string, AssetBundleData> assetBundleDownloads = new Dictionary<string, AssetBundleData>();
        Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();
        Dictionary<string, ServerObjectTransform> serverGameObjectLastTransforms = new Dictionary<string, ServerObjectTransform>();
        public Dictionary<string, List<Constraint>> constraintgroups = new Dictionary<string, List<Constraint>>();
        List<GameObject> formFields = new List<GameObject>();
        List<GameObject> serverGameObjects = new List<GameObject>();
        List<GameObject> newserverGameObjects = new List<GameObject>();
        public Dictionary<string, TimedGameObject> avatars = new Dictionary<string, TimedGameObject>();
        List<string> playersPresentCache = new List<string>();
        Dictionary<string, bool> legacyAvatarRetrieved = new Dictionary<string, bool>();
        List<Post> posts = new List<Post>();
        List<PlayerActivated> playerActivatedObjects = new List<PlayerActivated>();
        public string latestPostGuid = null;
        public int postsPageno = 0;
        public TextMeshPro communityPosts = null;
        GameObject rootGO;
        bool isListening = false;


        GameObject _sceneGO = null;
        GameObject inlineParentGP = null;
//        GameObject slam_avatar = null;
        GameObject walkFloorIndicator = null;
        Shader standardShader = null;
        Shader standardSpecularShader = null;
        Shader occludedShader = null;
        public Queue<SceneAction> Actions = new Queue<SceneAction>();
        public Queue<SceneAction> ActionsToPerform = new Queue<SceneAction>();
        public List<SceneAction> ActionsHandled = new List<SceneAction>();
        public bool AllowRecording = false;
        public bool RecordActions = false;
        bool AutoStartRecord = false;
        bool FastForwardActions = false;
        public float SceneTimer = 0;
        List<IEnumerator> SceneCoroutines = new List<IEnumerator>();
        SlamObject selectedObject = null;

#if !UNITY_STANDALONE_OSX
        HoloToolkit.Unity.InputModule.DictationInputManager dictationInputManager;
#endif
        HoloToolkit.Unity.InputModule.SpeechInputSource speechInputSource;
        SoundRecordState soundRecordState = SoundRecordState.NotSupported;
        soundrecord soundrecordObj;
        MicOnOff micScript;
        GameObject cursor;
        GameObject mousecursor;
        Typespace.Typer typer = null;
        HoloToolkit.Unity.InputModule.InteractionInputSource ControllerSource = null;
        uint SourceId;
        bool IsHandDraggable;
        bool IsHandRotatable;
        byte[] _gltfData;
        Vector3 cursorOffset = Vector3.zero;
        Vector3 controllerStart = Vector3.zero;
        float defaultDistance = 0;
        Quaternion defaultControllerRotation;
        Quaternion defaultRotation;
        bool soundToolsInitiated = false;
        float homeTimer = 0;
        float realCamY;
        float mrCameraCheckTime = 0;

#endregion Fields
#region Properties
        Transform handdraggable = null;
        public Transform Handdraggable
        {
            get
            {
                return handdraggable;
            }
            set
            {
                handdraggable = value;
                if (handdraggable != null && ControllerSource != null)
                {
                    Vector3 cpos;
                    Quaternion crot;
                    ControllerSource.TryGetGripPosition(SourceId, out cpos);
                    ControllerSource.TryGetGripRotation(SourceId, out crot);

                    defaultDistance = (cpos - handdraggable.position).magnitude;
                    cursorOffset = handdraggable.position - cursor.transform.position;
                    controllerStart = cpos;
                    defaultControllerRotation = crot;
                    defaultRotation = handdraggable.rotation;
                }
            }
        }
        Transform _LeftController;
        public Transform LeftController
        {
            get
            {
                if (IsImmersive())
                {
                    if (_LeftController == null)
                    {
                        var go = GameObject.Find("LeftController");
                        if (go != null)
                        {
                            _LeftController = go.transform;
                        }
                    }
                }
                return _LeftController;

            }
        }
        Transform _RightController;
        public Transform RightController
        {
            get
            {
                if (IsImmersive())
                {
                    if (_RightController == null)
                    {
                        var go = GameObject.Find("RightController");
                        if (go != null)
                        {
                            _RightController = go.transform;
                        }
                    }
                }
                return _RightController;

            }
        }
        public bool ActiveControllerIsLeft = false;
        public Transform ActiveController
        {
            get
            {
                return ActiveControllerIsLeft ? LeftController : RightController;

            }
        }
        public Transform CameraTransForm
        {
            get
            {
                if (cameraTransform == null)
                {
                    if (DeviceManager.IsUWP && !IsHoloLens())
                    {
                        var camObj = GameObject.Find(Constants.cameraMR);
                        if (camObj != null)
                        {
                            cameraTransform = camObj.transform;
                        }
                    }
                    else
                    {
                        cameraTransform = Camera.main.transform.parent;
                    }
                }
                return cameraTransform;
            }
        }
        public IDeviceManager DeviceManager
        {
            get
            {
                if (_deviceManager == null)
                {
                    _deviceManager = new DefaultDeviceManager();
                }
                return _deviceManager;
            }
            set
            {
                _deviceManager = value;
            }
        }
        public bool micMute
        {
#if USE_DISS
            get
            {
                return voiceBroadCastTrigger != null ? !voiceBroadCastTrigger.enabled : true;
            }
            set
            {
                if (voiceBroadCastTrigger != null)
                {
                    voiceBroadCastTrigger.enabled = !value;
                    if (micScript != null)
                    {
                        micScript.CheckTexture(value);
                    }
                }

            }
#else
            get { return true; }
            set { }
#endif 
        }
        public string CommunityInput
        {
            get
            {
                return communityInput;
            }
            set
            {
                communityInput = value;
            }
        }
        ViewPoint ViewPoint
        {
            get
            {
                if (_viewPoint == null)
                {
                    _viewPoint = new ViewPoint();
                    _viewPoint.Position = new Vector3(0, 1, 7);
                }
                return _viewPoint;
            }
            set
            {
                _viewPoint = value;
            }
        }
        public GameObject RootGO
        {
            get
            {
                if (rootGO == null)
                {
                    rootGO = new GameObject(Constants.rootGo_gameobject_name);
                }
                return rootGO;
            }
            set
            {
                rootGO = value;
            }
        }
        public GameObject SceneGO
        {
            get
            {
                if (_sceneGO == null)
                {
                    _sceneGO = new GameObject(Constants.scene_gameobject_name);
                    _sceneGO.transform.parent = RootGO.transform;
                    _sceneGO.AddComponent<Scene>();
                }
                return _sceneGO;
            }
        }
        public string DissonanceGuid
        {
            get
            {
#if USE_DISS
                if(dissonanceComms!=null)
                {
                    return dissonanceComms.LocalPlayerName;
                }
#endif
                return null;
            }
        }
#endregion Properties

        void Start()
        {
#if UNITY_WSA// && !UNITY_EDITOR
            DeviceManager = new HLDeviceManager();
#endif
#if UNITY_STANDALONE_OSX
            DeviceManager = new OSXDeviceManager();
#endif
            string disableCamera = Constants.cameraMR;
            string enabledCamera = Constants.cameraPC;
#if UNITY_WSA
            if (DeviceManager.Name == CallingDevices.MR.ToString())
            {
                disableCamera = Constants.cameraPC;
                enabledCamera = Constants.cameraMR;
            }
#endif
//            disableCamera = Constants.cameraPC;
            var dcamToDisable =GameObject.Find(disableCamera);
            Debug.Log("Disable camera: "+disableCamera);
            if(dcamToDisable!=null)
            {
                dcamToDisable.SetActive(false);
                var camObject = dcamToDisable.GetComponentInChildren<Camera>();
                //HoloToolkit.Unity.CameraCache.Refresh(camObject);
                //StartCoroutine(resetControllers(2));
            }
            var dcamEnabled = GameObject.Find(disableCamera);
            if (dcamEnabled != null)
            {
                var camObject = dcamEnabled.GetComponentInChildren<Camera>();
                HoloToolkit.Unity.CameraCache.Refresh(camObject);
                StartCoroutine(resetControllers(2));
            }
            if (!XRSettings.enabled)
            {
                Camera.main.fieldOfView = 60;
            }


            DeviceManager.LoadData();
            
            var audioSources=GetComponents<AudioSource>();
            if (audioSources.Length > 0)
            {
                audioSource = audioSources[0];
            }
            if (audioSources.Length > 2)
            {
                audioSourceEffect = audioSources[2];
            }
            txtToSpeech = GetComponent<HoloToolkit.Unity.TextToSpeech>();

            audioSourceAvatar = GetComponents<AudioSource>()[1];
            walkFloorIndicator = GameObject.Find(Constants.slam_WalkFloorIndicator);
            standardShader = Shader.Find(Constants.slam_standard_shader_name);
            standardSpecularShader = Shader.Find(Constants.slam_standard_specular_shader_name);
            occludedShader = Shader.Find(Constants.slam_standard_occluded_shader_name);
            menu = GameObject.Find(Constants.slam_menu_name);
            if (menu != null)
            {
                soundrecordObj = menu.GetComponentInChildren<soundrecord>();
                slamMenu = menu.GetComponent<SlamMenu>();
            }
#if UNITY_STANDALONE_OSX
            if (soundrecordObj != null)
            {
                soundrecordObj.gameObject.SetActive(false);
            }
#endif
            //var avMenu= GameObject.Find(Constants.slam_avatar_menu_name);
            //if (avMenu != null)
            //{
            //   // avMenu.transform.parent = rootGO.transform;
            //    avatarMenu = avMenu.GetComponentInChildren<avatarMenu>();
            //}
            waitCursor = GameObject.Find(Constants.slam_waitCursor_name);
            InitiateMessageBox();
            StartCoroutine(DoShowMessage(0, false));

            HoloToolkit.Unity.SimpleTagalong tagalong = menu.GetComponent<HoloToolkit.Unity.SimpleTagalong>();
            if (tagalong != null)
            {
                tagalong.TagalongDistance = DeviceManager.MainMenuDistance;
            }
            menu.transform.position = new Vector3(menu.transform.position.x, menu.transform.position.y, -DeviceManager.MainMenuDistance);

            menu.SetActive(false);
            sceneRotator = GameObject.Find("sceneRotator");
            SetSceneRotator(false);

            textToSpeechGO = GameObject.Find("TextToSpeech");
            InvokeRepeating("HandleUpdatableTextures", 2, 0.5f);
            InvokeRepeating("HandleUpdatableInlines", 2, 2f);
            InvokeRepeating("CheckTyping", 1, 0.21f);
            
            cursor = GameObject.Find("Cursor");
            mousecursor = GameObject.Find("MouseCursor");
            mousecursor.SetActive(false);
        }
        void Update()
        {
#if UNITY_WSA && !UNITY_EDITOR
            CheckStartUpArgs();
            var appMemory = Windows.System.MemoryManager.AppMemoryUsage;
            var appMemoryLimit = Windows.System.MemoryManager.AppMemoryUsageLimit;
            if(appMemory>appMemoryLimit-3000000)
            {
                GoHome();
                ShowNotification("Last webscene exceeded allowed memomry usage");
            }
#endif
            if (!string.IsNullOrEmpty( activationNavigateUrl))
            {
                if (activationNavigateTime <= 0)
                {
                    ShowNotification(activationNavigateUrl);
                    Parse(activationNavigateUrl, Target._blank);
                    activationNavigateUrl = null;
                    return;
                }
                else
                {
                    activationNavigateTime -= Time.deltaTime;
                }
            }
            if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Loaded)
            {
                audioSource.Play();
            }
            if(HasConversation)
            {
                IsTalking();
            }
            //if(RecordActions)
            //{
                SceneTimer += Time.deltaTime;
                CheckActionsToPerform();
            //}
            //CheckgazedObjectClicked();


            CheckCameraHeight();
            CheckPlayerActivated();
            UpdataHanddraggable();
            UpdateWorkFlowIndicatior();
            if(homeTimer>0)
            {
                homeTimer -= Time.deltaTime;
            }
        }

#region External typing
        public void StartTyping(string startText)
        {
            if (!string.IsNullOrEmpty(DeviceManager.PresentationGuid))
            {
                if (typer == null)
                {
                    typer = new Typespace.Typer(DeviceManager.PresentationGuid);
                }
                Typespace.Typer.sequence = 0;
                Typespace.Typer.lasthandledsequence = 0;
                Typespace.Typer.AddCommandToSend(Typespace.typecmd.clear);
                Typespace.Typer.AddWordTosend(startText, 0);
            }
        }
        public void StopTyping()
        {
            if (typer != null)
            {
                Typespace.Typer.AddCommandToSend(Typespace.typecmd.stop);
                Typespace.Typer.sequence = 0;
                Typespace.Typer.lasthandledsequence = 0;
            }
        }
        public void Typing(string key, int cursor)
        {
            if (typer != null)
            {
                switch (key)
                {
                    case Constants.slam_backspace:
                        Typespace.Typer.AddCommandToSend(Typespace.typecmd.backspace, cursor);
                        break;
                    case Constants.slam_clear:
                        Typespace.Typer.AddCommandToSend(Typespace.typecmd.clear, cursor);
                        break;
                    case Constants.slam_left:
                        Typespace.Typer.AddCommandToSend(Typespace.typecmd.none, cursor-1);
                        break;
                    case Constants.slam_right:
                        Typespace.Typer.AddCommandToSend(Typespace.typecmd.none, cursor+1);
                        break;
                    default:
                        Typespace.Typer.AddWordTosend(key, cursor);
                        break;
                }
            }
        }

        void CheckTyping()
        {
            if (typer != null && slamMenu.isActiveAndEnabled)
            {
                StartCoroutine( typer.Typing(UrlHandler.GetTyperUrl()));
                while(Typespace.Typer.WordsRecieved.Count>0)
                {
                    string word = null;
                    int cursor = 0;
                    int sequence = 0;
                    Typespace.typecmd cmd = Typespace.Typer.GetNextRecievedWord(out word, out cursor, out sequence);
                    if(word!=null)
                    {
                        switch(cmd)
                        {
                            case Typespace.typecmd.backspace:
                                word = Constants.slam_backspace;
                                break;
                            case Typespace.typecmd.clear:
                                word = Constants.slam_clear;
                                break;
                            case Typespace.typecmd.enter:
                                word = Constants.slam_enter;
                                break;
                        }
                    }
                    slamMenu.Type(word, cursor, sequence);
                }
            }
        }
#endregion external typing
#region Various
        IEnumerator LoadGLTF(string url, Transform parent)
        {
            GLTFSceneImporter sceneImporter = null;
            // GLTFSceneImporter loader = null;
            try
            {

                sceneImporter = new GLTFSceneImporter(
                     url,
                     parent
                     );
                sceneImporter.MaximumLod = 300;
            }
            catch (Exception x) { }

            yield return sceneImporter.Load(-1);


        }
        public void SetHandDraggable(Transform handDraggable, HoloToolkit.Unity.InputModule.InteractionInputSource controller, uint sourceId, bool isHandDragable, bool isHandRotatable)
        {
            if (handDraggable != Handdraggable)
            {
                if (Handdraggable == null)
                {
                    ControllerSource = controller;//RightController;// =
                    SourceId = sourceId;
                }
                Handdraggable = handDraggable;
                IsHandDraggable = isHandDragable;
                IsHandRotatable = isHandRotatable;
            }
        }
        public void UpdataHanddraggable()
        {
            if (Handdraggable != null && ControllerSource!=null)
            {
                double grip = 0;
                bool selectPressed = false;
                if(ControllerSource.TryGetSelect(SourceId,out selectPressed, out grip))
                {
                    if(!selectPressed)
                    {
                        Handdraggable = null;
                        return;
                    }
                }
                if (cursor != null)
                {
                    Vector3 cpos;
                    Quaternion crot;
                    ControllerSource.TryGetGripPosition(SourceId, out cpos);
                    ControllerSource.TryGetGripRotation(SourceId, out crot);

                    var move = (cursor.transform.position- cpos).normalized;
                    var controllerdist = cpos - controllerStart;
                    Plane plane = new Plane(move,controllerStart);
                    var dist = plane.GetDistanceToPoint(cpos);
                    if (IsHandDraggable)
                    {
                        Handdraggable.position = cpos + move * defaultDistance * (1 + dist * 10) + cursorOffset;
                    }
                    if (IsHandRotatable)
                    {
                        var rotationOffSet = crot * Quaternion.Inverse(defaultControllerRotation);
                        Handdraggable.rotation = defaultRotation * rotationOffSet;
                    }
                }
            }
 
        }
        public bool IsHigherOrEqualSlamversion(float version)
        {
            float currentVersion = 0;
            if(float.TryParse(Constants.SlamVersion, out currentVersion)&& currentVersion>=version)
            {
                return true;
            }
            return false;
        }
        public void UpdateWorkFlowIndicatior()
        {
            if (showWorkflowIndicator)
            {
                
                Vector3 pos = Vector3.zero;
                if (DeviceManager.IsUWP)
                {
                    if(!IsHoloLens())
                    {
                        return;
                    }
#if UNITY_WSA
                    pos = HoloToolkit.Unity.InputModule.GazeManager.Instance.HitPosition;
#endif
                }
                else
                {
                    TryGetMousePosition(out pos);
                }

                ShowWorkFloorIndicator(true, pos);
            }
            else
            {
                Slam.Instance.ShowWorkFloorIndicator(false, Vector3.zero);

            }
        }
        public bool TryGetMousePosition(out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            Vector2 mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(ray, out hit, Constants.mouseHitDistance))
            {
                hitPoint = hit.point;
                return true;
            }
            return false;
        }
        public void SaveCommunityInput(string typedText)
        {
            communityInput = typedText;
        }
        public void ShowWorkFloorIndicator(bool show, Vector3 hitPoint)
        {
            if (walkFloorIndicator != null)
            {
                walkFloorIndicator.transform.position = hitPoint + new Vector3(0, 0.25f, 0);
                walkFloorIndicator.SetActive(show);
                walkFloorIndicator.transform.LookAt(Camera.main.transform);
            }
        }
        public void ShowToolTip(string toolTip)
        {
            if (toolTip != null && DeviceManager != null)
            {
                DeviceManager.SetToolTip(toolTip);
            }
        }
        internal void SitOn(Vector3 position, Vector3 forward)
        {
            WalkTo(position, 0);
        }
        public void WalkTo(Vector3 groundPosition, float level, bool fast = false)
        {
            Vector3 newPos = Camera.main.transform.position - new Vector3(groundPosition.x, Camera.main.transform.position.y, groundPosition.z) + Scene.Instance.transform.position;
            //#if UNITY_WSA
            //newPos=new Vector3( groundPosition.x, Camera.main.transform.position.y, groundPosition.z)-Camera.main.transform.position + Scene.Instance.transform.position;
            //#endif
            Scene.Instance.fast = fast;
            Scene.Instance.walkTo = new Vector3(newPos.x, newPos.y, newPos.z);
            Debug.Log("Scene walk to: " + Scene.Instance.walkTo.ToString());

            CameraManager.Instance.floorLevel = level;
        }
        void CleanUpPostSystem()
        {
            communityPosts = null;
            posts.Clear();
            latestPostGuid = null;
            postsPageno = 0;
        }
        public Vector3 ScenePosition(Vector3 pos)
        {
            return SceneGO.transform.position - pos;
        }
        ServerObjectTransform CreateServerObjectTransForm(GameObject so, ServerObjectTransform sot = null)
        {
            if (sot == null)
            {
                sot = new ServerObjectTransform();

            }
            sot.Position = Statics.ToJson(ScenePosition(so.transform.position));
            sot.Rotation = Statics.ToJson(so.transform.rotation);
            var rb = so.GetComponent<Rigidbody>();
            if (rb != null)
            {
                sot.Velocity = Statics.ToJson(rb.velocity);
                sot.AngularVelocity = Statics.ToJson(rb.angularVelocity);
            }
            sot.Guid = so.name;
            return sot;
        }
        public string SerialiseServerObjects()
        {
            while (newserverGameObjects.Count > 0)
            {
                var go = newserverGameObjects[0];
                serverGameObjects.Add(go);
                SetServerObjectLastTransform(go, go.name);
                newserverGameObjects.Remove(go);

            }
            if (serverGameObjects.Count == 0) { return null; }
            List<ServerObjectTransform> sotList = new List<ServerObjectTransform>();
            foreach (var so in serverGameObjects)
            {
                ServerObjectTransform sot = null;
                bool publish = false;
                if (serverGameObjectLastTransforms.ContainsKey(so.name))
                {
                    sot = serverGameObjectLastTransforms[so.name];
                    publish = CheckServerObjectTransform(sot, so.transform);
                }
                else
                {
                    sot = new ServerObjectTransform();
                    serverGameObjectLastTransforms[so.name] = sot;
                    publish = true;
                }
                //publish = true;
                if (publish)
                {
                    sot = CreateServerObjectTransForm(so, sot);
                    //serverGameObjectLastTransforms[so.name] = sot;
                    sotList.Add(sot);
                }

            }
            if (sotList.Count == 0) { return null; }
            return JsonConvert.SerializeObject(sotList);
        }
        bool CheckServerObjectTransform(ServerObjectTransform sot, Transform tr)
        {
            if (Vector3.Distance(Statics.FromJson(sot.Position), ScenePosition(tr.position)) > 0.01f)
            {
                return true;
            }
            return false;
        }
        public void SetSelectedSlamObject(SlamObject slamobject)
        {
            selectedObject = slamobject;
        }
        public void ActivateSelectedSlamObject()
        {
            if (selectedObject != null)
            {
                selectedObject.DoSelect(Vector3.zero);
            }
        }
        public float SetSceneRotator(bool active)
        {
            float dist = 1;
            if (sceneRotator != null)
            {
                if (active)
                {
                    var srsc = sceneRotator.GetComponent<sceneRotator>();
                    if (srsc != null)
                    {
                        srsc.Init();
                        dist = srsc.hDis;
                    }
                }
                sceneRotator.SetActive(active);
            }
            return dist;
        }
        IEnumerator resetControllers(float seconds = 1)
        {
            yield return new WaitForSeconds(seconds);
            Debug.Log("Resetting controllers");
            Camera.main.transform.localPosition = Vector3.zero;
            var mc = GameObject.Find("MotionControllers");
            if (mc != null)
            {
                mc.transform.localPosition = Vector3.zero;
            }

        }
        public string GetInfo()
        {
            return string.Format(Constants.slam_version_text, Constants.SlamVersion);
        }
        public void GoHome()
        {
            CameraManager.Instance.ResetTransform();
            RecordActions = false;
            if (homeTimer <= 0)//avoid repeating
            {
                Parse(UrlHandler.GetHomeUrl(!UrlHandler.AtHome), Target._blank);
                homeTimer = 2;
            }
        }
        public void CleanUpScene()
        {
            for (int nn = 0; nn < SceneGO.transform.childCount; nn++)
            {
                Destroy(SceneGO.transform.GetChild(nn).gameObject);
            }
            //for (int nn = 0; nn < Camera.main.transform.childCount; nn++)
            //{
            //    Destroy(Camera.main.transform.GetChild(nn).gameObject);
            //}
            CleanTextureList(textures);
            CleanTextureList(updatableTextures);

            //textures.Clear();

            matDict.Clear();
            meshDict.Clear();
            serverGameObjectLastTransforms.Clear();
            avatars.Clear();
            formFields.Clear();
            definitions.Clear();
            assetBundleDownloads.Clear();
            serverGameObjects.Clear();
            playersPresentCache.Clear();
            playerActivatedObjects.Clear();
            constraintgroups.Clear();
            updatableInlines.Clear();

            CameraManager.Instance.floorLevel = 0;
            DeviceManager.SetToolTip("");
            currentSearchTerms = null;
            currentFilter = null;
            currentPage = null;
            AllowRecording = false;
            AllowScroll = true;
            HideLeaveApp(true, 0.0001f);
            myAvatarPresentation = null;
            UrlHandler.SingleUser = false;
            //CameraCache.Main.transform.position = Vector3.zero;
            //Camera.main.transform.position = Vector3.zero;
            placeHolderCounter = 0;
            SceneTimer = 0;
            ClearActions();
            hideWaitCursor = false;
            hideWaitCursor2 = false;
            CleanUpPostSystem();
            AutoStartRecord = false;
            selectedObject = null;
            currentInline = null;
            Resources.UnloadUnusedAssets();
            CameraTransForm.transform.localPosition = Vector3.zero;
            if (!UrlHandler.AtHome)
            {
                CameraTransForm.transform.localRotation = Quaternion.identity;
            }
            if (!DeviceManager.IsUWP)
            {
                CameraTransForm.transform.localRotation = Quaternion.identity;
                Camera.main.transform.localRotation = Quaternion.identity;
            }
#if UNITY_WSA
            if (!IsHoloLens()) { 
                if (CameraCache.Main != null && CameraCache.Main.transform.parent != null)
                {
                    CameraCache.Main.transform.parent.position =new Vector3(0, -CameraCache.Main.transform.localPosition.y - 0.3f, 0);
                }
        }
#endif
            var cammov = CameraTransForm.GetComponentInChildren<CameraMovement>();
            if (cammov != null)
            {
                cammov.ReInit();
            }
            SetSceneRotator(false);
            ResetFog();
        }
        void CleanTextureList(Dictionary<string, TextureContainer> textureList)
        {
            List<string> keys = new List<string>();
            foreach (var t in textureList)
            {
                var tc = t.Value;
                if (tc != null)
                {
                    foreach (var g in tc.GameObjects)
                    {
                        Destroy(g);
                    }
                    keys.Add(t.Key);
                }

            }
            foreach (var key in keys)
            {
                textureList.Remove(key);
            }

        }
        void CheckNotification()
        {
            if (newAvatars.Count > 0)
            {
                string msg = string.Format("{0} entered the scene", string.Join(",", newAvatars.ToArray()));
                ShowNotification(msg);
            }
        }
        void CheckPlayerActivated()
        {
            foreach (var pa in playerActivatedObjects.FindAll(x => x.Triggered == false && x.Transform != null))
            {
                if (Vector3.Distance(pa.Transform.position, CameraCache.Main.transform.position) < pa.Distance)
                {
                    switch (pa.PlayerActivatedType)
                    {
                        case PlayerActivatedType.Speak:
                            PlayerActivatedVoice playerActivatedVoice = null;
                            if (pa.PlayerAtivatedArgs != null && pa.PlayerAtivatedArgs is PlayerActivatedVoice)
                            {
                                playerActivatedVoice = (PlayerActivatedVoice)pa.PlayerAtivatedArgs;
                                if (TrySpeakText(pa.Transform, playerActivatedVoice.Text, playerActivatedVoice.Voice, !playerActivatedVoice.Interrupt))
                                {
                                    pa.Triggered = true;
                                }
                            }
                            break;
                    }
                }
            }
        }

        void CheckStartUpArgs()
        {
#if UNITY_WSA && !UNITY_EDITOR
            //if (!string.IsNullOrEmpty(UnityEngine.WSA.Application.arguments) && UnityEngine.WSA.Application.arguments!=startupArgs)// && UnityEngine.WSA.Application.arguments.ToLower().StartsWith(Constants.protocolCode))
            //{
            //    startupArgs = UnityEngine.WSA.Application.arguments;

            //    activationNavigateUrl = startupArgs.ToLower().Replace(Constants.uriIdentifier, "").Replace(Constants.protocolCode, Constants.httpsCode);
            //    activationNavigateTime = 1;
            //}
            if (!string.IsNullOrEmpty(startupArgs) )// && UnityEngine.WSA.Application.arguments.ToLower().StartsWith(Constants.protocolCode))
            {
                activationNavigateUrl = startupArgs.ToLower().Replace(Constants.uriIdentifier, "").Replace(Constants.protocolCode, Constants.httpsCode);
                activationNavigateTime = 1;
                startupArgs = null;
            }
#endif
        }
#if UNITY_WSA
        void OnGUI()
        {
            RaycastHit hit;
            Event e = Event.current;

            if (mousecursor != null)
            {
                Vector2 mouse = new Vector2(e.mousePosition.x, Screen.height - e.mousePosition.y);
                Ray ray = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    var camPos = Camera.main.transform.position;
                    var vFromCam = hit.point - camPos;
                    var norm = vFromCam.normalized;
                    mousecursor.transform.position = hit.point-0.1f*norm;
                    //mousecursor.SetActive(true);
                    mousecursor.transform.localScale = new Vector3(3, 3, 3) * vFromCam.magnitude / 3;
                }
                else
                {
                    mousecursor.transform.position = ray.direction * 2;
                    mousecursor.SetActive(false);
                }
                mousecursor.transform.LookAt(Camera.main.transform);
            }
        }
#endif
        public void QueryAction(string goName)
        {
            if (AllowRecording && RecordActions && !string.IsNullOrEmpty(goName))
            {
                var act = new SceneAction(goName, SceneTimer);
                ActionsHandled.Add(act);
                Actions.Enqueue(act);
            }
        }
        public GameObject InitiateVertextShape(Transform parent, bool useMesh = true)
        {
            GameObject go = new GameObject("shape");
            go.transform.SetTheParent(parent);
            if (useMesh)
            {
                go.SetActive(false);
                go.AddComponent(typeof(MeshFilter));
                go.AddComponent(typeof(MeshRenderer));
            }
            return go;
        }
        public void HandleVertexShapeNode(GameObject go, IndexedFaceSet indexedFaceSet)
        {
            // AsyncMeshLoader meshLoader = new AsyncMeshLoader();
            //StartCoroutine(AsyncMeshLoader.Instance.AssignMesh(go, indexedFaceSet.Clone()));
            if (string.IsNullOrEmpty(go.name) || go.name.ToLower() == "shape")
            {
                go.name += "_" + System.Guid.NewGuid().ToString();
            }
            if (!meshDict.ContainsKey(go.name))
            {
                MeshContainer meshContainer = new MeshContainer();
                meshContainer.GameObject = go;
                meshContainer.IndexedFaceSet = indexedFaceSet;
                meshContainer.Counter = 2;
                meshContainer.Mesh = null;
                meshDict[go.name] = meshContainer;

            }
            else
            {
                AssignMesh(go, meshDict[go.name].Mesh);

            }
        }
        public void AssignMesh(GameObject go, Mesh mesh)
        {
            if (go != null)
            {
                MeshFilter mf = go.GetComponent<MeshFilter>();
                mf.mesh = mesh;
                go.SetActive(true);
            }
        }
        void TryRetrieveMeshes()
        {
            if (!retrievingMesh)
            {
                bool meshesLeft = false;
                foreach (var mdc in meshDict)
                {
                    ShowWaitCursor(true, DownloadType.Mesh);
                    MeshContainer mc = mdc.Value;
                    if (mc.Mesh == null && mc.Counter > 0)
                    {
                        retrievingMesh = true;
                        mc.Counter--;
                        meshesLeft = true;
                        AsyncMeshLoader.Instance.AssignMesh(mc);

                        break;
                    }
                }
                if (!meshesLeft)
                {
                    CancelInvoke("TryRetrieveMeshes");
                    TryRetrieveMeshesStarted = false;
                    ShowWaitCursor(false, DownloadType.Mesh);
                }
            }
        }
        public void SetCallingApplicationHeaderValue()
        {
            string callingDevice = DeviceManager.Name;
            Constants.callingDeviceHeader = callingDevice;
        }
        public bool Switch(string url, GameObject sourceObj)
        {
            if (sourceObj != null && url != null && url.ToLower().StartsWith("switch:"))
            {
                var targetObj = url.Replace("switch:", "").Trim();
                Transform t = SceneGO.transform.FindDeepChild(targetObj);
                if (t != null)
                {
                    t.gameObject.SetActive(true);
                    sourceObj.SetActive(false);
                    return true;
                }
            }
            return false;
        }
#endregion Various
#region speech
        IEnumerator InitiateSoundTools(float wait)
        {
            yield return new WaitForSeconds(wait);
            if (!soundToolsInitiated)
            {

#if !UNITY_EDITOR
                var textControlManager = Instantiate(Resources.Load<GameObject>("Prefabs/TextControlManager"));
                if (textControlManager != null)
                {
                    textControlManager.name = "TextControlManager";
                    speechInputSource = textControlManager.GetComponent<HoloToolkit.Unity.InputModule.SpeechInputSource>();
                    var sih = textControlManager.GetComponent<HoloToolkit.Unity.InputModule.SpeechInputHandler>();
                    if(sih!=null)
                    {
                        List<HoloToolkit.Unity.InputModule.SpeechInputHandler.KeywordAndResponse> kwr = new List<HoloToolkit.Unity.InputModule.SpeechInputHandler.KeywordAndResponse>();
                        kwr.Add(AddKW("go home", GoHome));
                        kwr.Add(AddKW("open menu", OpenMenu));
                        kwr.Add(AddKW("close menu", CloseMenu));
                        kwr.Add(AddKW("select", ActivateSelectedSlamObject));
                        kwr.Add(AddKW("start recording", startRecording));
                        kwr.Add(AddKW("enter text", SubmitText));
                        kwr.Add(AddKW("clear text", ClearText));

                        sih.Keywords = kwr.ToArray();
                    }
                }
#endif
#if !UNITY_STANDALONE_OSX
                var dictationInputManagerObj = Instantiate(Resources.Load<GameObject>("Prefabs/DictationInputManager"));
                if (dictationInputManagerObj != null)
                {
                    dictationInputManagerObj.name = "DictationInputManager";
                    dictationInputManager = dictationInputManagerObj.GetComponent<HoloToolkit.Unity.InputModule.DictationInputManager>();
                }
#endif
                soundToolsInitiated = true;
            }

        }
        HoloToolkit.Unity.InputModule.SpeechInputHandler.KeywordAndResponse AddKW(string keyword, UnityEngine.Events.UnityAction action)
        {
            HoloToolkit.Unity.InputModule.SpeechInputHandler.KeywordAndResponse kar = new HoloToolkit.Unity.InputModule.SpeechInputHandler.KeywordAndResponse();
            kar.Keyword = keyword;
            kar.Response = new UnityEngine.Events.UnityEvent();
            kar.Response.AddListener(action);
            return kar;
        }
        public void ClearText()
        {
            if (menu.activeSelf)
            {
                var ret = menu.GetComponentInChildren<ClearText>();
                ret.DoSelect(Vector3.zero);
            }
        }
        public void SubmitText()
        {
            if (menu.activeSelf)
            {
                stopRecording();
                var ret = menu.GetComponentInChildren<ParseTo>();
                ret.DoSelect(Vector3.zero);
            }
        }
        public void startRecording()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA
            if (dictationInputManager != null && soundRecordState != SoundRecordState.Recording && menu.activeSelf)
            {
                speechInputSource.StopKeywordRecognizer();
                StartCoroutine(HoloToolkit.Unity.InputModule.DictationInputManager.StartRecording());
                soundRecordState = SoundRecordState.Recording;
                SetSoundRecordState();
            }
#endif
        }
        public void stopRecording()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA
            if (dictationInputManager != null)
            {
                StartCoroutine(HoloToolkit.Unity.InputModule.DictationInputManager.StopRecording());
                StartCoroutine(StartKeywordRecognizer());

                soundRecordState = SoundRecordState.RecordingStopped;
                SetSoundRecordState();
            }
#endif
        }
        IEnumerator StartKeywordRecognizer()
        {
            yield return new WaitForSeconds(1);
#if UNITY_STANDALONE_WIN || UNITY_WSA
            if (!HoloToolkit.Unity.InputModule.DictationInputManager.IsListening && speechInputSource != null)
            {
                speechInputSource.StartKeywordRecognizer();
            }
#endif
        }

        void SetSoundRecordState()
        {
            if (soundrecordObj != null)
            {
                soundrecordObj.SetRecordState(soundRecordState);
            }
        }
        //IEnumerator StartSound()
        //{
        //    yield return new WaitForSeconds(2);
        //    InitiateSound();
        //}
        public void StopSpeakText()
        {
            if (txtToSpeech != null)
            {
                if (txtToSpeech.IsSpeaking())
                {
                    txtToSpeech.StopSpeaking();
                }
            }
        }
        TextToSpeechVoice TryGetTextToSpeechVoice(string name)
        {
            TextToSpeechVoice voice = TextToSpeechVoice.Default;
            TryParseVoice(name, out voice);
            return voice;
        }
        bool TryParseVoice(string t, out TextToSpeechVoice voice)
        {
            voice = TextToSpeechVoice.Default;
            if (t != null)
            {
                switch (t)
                {
                    case "david":
                        voice = TextToSpeechVoice.David;
                        return true;
                    case "default":
                        voice = TextToSpeechVoice.Default;
                        return true;
                    case "mark":
                        voice = TextToSpeechVoice.Mark;
                        return true;
                    case "zira":
                        voice = TextToSpeechVoice.Zira;
                        return true;
                }
            }
            return false;
        }
        public bool TrySpeakText(Transform parent,  string aText, string aName, bool onlyWhenSilent = false)
        {
            if (!string.IsNullOrEmpty(aText) && txtToSpeech != null)
            {
                if (!onlyWhenSilent && txtToSpeech.IsSpeaking())
                {
                    txtToSpeech.StopSpeaking();
                }
                if (!txtToSpeech.IsSpeaking())
                {
                    //                    assureTextToSpeechGO();
                    //                    textToSpeechGO.transform.SetTheParent(parent);
                    if (parent != null)
                    {
                        textToSpeechGO.transform.position = parent.position;
                    }
                    txtToSpeech.Voice = TryGetTextToSpeechVoice(aName);
                    txtToSpeech.StartSpeaking(aText);
                    return true;
                }
            }
            return false;
        }
        void assureTextToSpeechGO()
        {
            if(textToSpeechGO==null)
            {
                textToSpeechGO = new GameObject();
                var asou=textToSpeechGO.AddComponent<AudioSource>();
                UnityEngine.Audio.AudioMixer mixer = Resources.Load("Main") as UnityEngine.Audio.AudioMixer;
                if (mixer != null)
                {
                    asou.outputAudioMixerGroup = mixer.FindMatchingGroups("TextToSpeech")[0];
                }
            }
        }
       public void CheckSoundInitiated()
        {
#if USE_DISS
            if (dissonanceComms==null)
            {
                var dissonanceGO = GameObject.Find(Constants.DissonanceSetup);
                var networkManagerGO = GameObject.Find(Constants.NetworkManager);

                if (dissonanceGO != null && networkManagerGO != null)
                {
                    //                var comm = prefab.GetComponent<UNetCommsNetwork>();
                    var comm = networkManagerGO.GetComponent<UnityEngine.Networking.NetworkManager>();
                    // Debug.Log("AJW: Starting InitializeAsClientusing :" + UrlHandler.SoundServer);
                    if (comm != null)
                    {
                        //#if !UNITY_WSA

                        voiceBroadCastTrigger = dissonanceGO.GetComponent<Dissonance.VoiceBroadcastTrigger>();
                        voiceReceiptTrigger = dissonanceGO.GetComponent<Dissonance.VoiceReceiptTrigger>();
                        dissonanceComms = dissonanceGO.GetComponent<Dissonance.DissonanceComms>();
                        if (dissonanceComms != null)
                        {
                            comm.StartClient();
                            dissonanceComms.OnPlayerStartedSpeaking += DissonanceComms_OnPlayerStartedSpeaking;
                            dissonanceComms.OnPlayerStoppedSpeaking += DissonanceComms_OnPlayerStoppedSpeaking;
                            voiceBroadCastTrigger.RoomName = "Global";
                            voiceReceiptTrigger.RoomName = "Global";
                            voiceBroadCastTrigger.enabled = false;
                            dissonanceComms.IsMuted = false;
                        }
                        //#endif
                    }
                    //micMute = false;
                }
            }
#endif
        }

#if USE_DISS
            private void DissonanceComms_OnPlayerStoppedSpeaking(Dissonance.VoicePlayerState obj)
        {
           // HasConversation = false;
        }

        private void DissonanceComms_OnPlayerStartedSpeaking(Dissonance.VoicePlayerState obj)
        {
            HasConversation=true;
        }
#endif

        private void AddSpeekIndicator(Transform avatar)
        {
            if (!avatar.FindDeepChild(Constants.speechIndicatorName))
            {
                var bubbles = GetPrefab(null, "various", "bubbles");
                bubbles.name = Constants.speechIndicatorName;
                bubbles.transform.localScale = 0.05f * Vector3.one;
                bubbles.transform.SetTheParent(avatar);
                bubbles.transform.position = avatar.position + new Vector3(-0.2f, 2.0f, 0.1f);
                bubbles.SetActive(false);
            }
        }
        private void IsTalking()
        {
#if USE_DISS
           if (dissonanceComms != null)
            {
                foreach (var player in dissonanceComms.Players)
                {
                    foreach (var av in avatars)
                    {
                        if (av.Value.DissonanceGuid == player.Name)
                        {
                            var bubble = av.Value.GameObject.transform.FindDeepChild(Constants.speechIndicatorName);
                            if (bubble != null)
                            {
                                bubble.gameObject.SetActive(player.IsSpeaking);
                                bubble.localScale = 1.5f * player.Amplitude * Vector3.one;
                            }
                            break;
                        }
                    }
                }
            }
#endif
        }
        public string chatroom = null;
#if USE_DISS
        Dissonance.RoomChannel channel;
#endif
        public void StartSpeak()
        {
            CheckSoundInitiated();
#if USE_DISS
            if (chatroom != null && dissonanceComms != null)
            {

#if UNITY_STANDALONE_WIN || UNITY_WSA
                if (speechInputSource != null)
                {
                    speechInputSource.StopKeywordRecognizer();
                }

                if (dictationInputManager != null && dictationInputManager.isActiveAndEnabled)
                {
                    dictationInputManager.enabled = false;
                }
#if UNITY_STANDALONE_WIN
                dissonanceComms.ResetMicrophoneCapture();
#endif
#endif
                // channel =dissonanceComms.RoomChannels.Open(chatroom, true, Dissonance.ChannelPriority.Default);
                //dissonanceComms.
                //voiceBroadCastTrigger.enabled = true;
                ShowNotification("Voice chat enabled");
                //voiceReceiptTrigger.enabled = true;
                micMute = false;
            }
#endif
        }
        public void StopSpeak()
        {
            micMute = true;
#if UNITY_STANDALONE_WIN || UNITY_WSA
            if (speechInputSource != null)
            {
                speechInputSource.StartKeywordRecognizer();
            }
#if !UNITY_STANDALONE_OSX
            if (dictationInputManager != null && !dictationInputManager.isActiveAndEnabled)
            {
                dictationInputManager.enabled = true;
            }
#endif
#endif
        }
        public void ToggleSoundRecord()
        {
            if (soundRecordState != SoundRecordState.Recording)
            {
                startRecording();
            }
            else
            {
                stopRecording();
            }
        }
        public RecordingState Record()
        {
            if (AllowRecording)
            {
                if (isListening)
                {
                    return RecordingState.Listening;
                }
                RecordActions = !RecordActions;
                if (RecordActions)
                {
                    ClearActions();
                    var act = new SceneAction(Constants.startRecordingAction, 0.001f);
                    Actions.Enqueue(act);
                    ActionsHandled.Add(act);
                }
                else
                {
                    var act = new SceneAction(Constants.endRecordingAction, 10000f);
                    Actions.Enqueue(act);
                    ActionsHandled.Add(act);

                }
                return RecordActions ? RecordingState.Recording : RecordingState.RecordingStopped;
            }
            return RecordingState.NotSupported;
        }
#endregion speech
#region asset bundles
        void CheckAssetBundles()
        {
            if(assetBundleDownloads.Count>0)
            {
                long totSize = 0;
                foreach (var item in assetBundleDownloads)
                {
                    totSize += (long)item.Value.SizeInBytes;
                }
                messageOkType = MessageOkType.DownloadAssets;
                if (totSize > Constants.SizeInBytesLevel)
                {
                    ShowMessage(true, string.Format("This scene wants to download extra models (approx. {0} bytes). Is that OK?", totSize.HumanReadableBytes()), Constants.slam_ok_message, Constants.slam_close_message);
                }
                else
                {
                    DoDownloadAssetBundles();
                }
            }
         }
        public void DoDownloadAssetBundles()
        {
            ShowWaitCursor(true, DownloadType.AssetBundle);
            foreach (var item in assetBundleDownloads)
            {
                    StartCoroutine(DownLoadAsset(item.Key, item.Value.Name));
            }
        }
        string GetUnityBundleFolderName()
        {
            string unityFolder = Application.unityVersion.Substring(0, Application.unityVersion.Length - 2);
            switch(unityFolder)
            {
                case "2017.3.1":
                case "2017.4.6":
                case "2017.4.9":
                case "2017.4.10":
                default:
                    return "2017.3.1";
            }
            return unityFolder;
        }
        IEnumerator DownLoadAsset(string url, string name, bool isAvatarBundle=false)
        {
            var pos = url.LastIndexOf("/");
            string turl;
            string unityFolder = GetUnityBundleFolderName();
            if (pos>0)
            {
                turl = url.Substring(0, pos) + @"/"  + unityFolder + url.Substring(pos);
            }
            else
            {
                turl = unityFolder + @"/" + url;
            }
            turl += DeviceManager.BundleExtension.ToString();
            UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(turl, 0);
            yield return request.SendWebRequest();
            if (string.IsNullOrEmpty(request.error))
            {


                var handler = (DownloadHandlerAssetBundle)request.downloadHandler;
                // PicTexture = handler.texture;

                var bundle = handler.assetBundle;//  DownloadHandlerMovieTexture.GetContent(www); 

                //AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                if (bundle != null)
                {
                    assetBundles[name] = bundle;
                    if (isAvatarBundle)
                    {
                        legacyAvatarRetrieved[name] = true;
                    }
                    else
                    {
                        assetBundleDownloads.Remove(url);
                        if (assetBundleDownloads.Count == 0)
                        {
                            HandleLateObjects();
                        }
                    }
                }
            }
            ShowWaitCursor(false, DownloadType.AssetBundle);

        }
#endregion asset bundles
#region messagebox, notification
        public void MessageOKAction()
        {
            switch(messageOkType)
            {
                case MessageOkType.Close:
                    ShowMessage(false);
                    break;
                case MessageOkType.DownloadAssets:
                    DoDownloadAssetBundles();
                    ShowMessage(false);
                    break;
            }
            messageOkType = MessageOkType.None;
        }
        void InitiateMessageBox()
        {
            if (msgBox==null)
            {
                msgBox = GameObject.Find(Constants.slam_msgbox_name);
                if(IsHoloLens())
                {
                    msgBox.transform.localScale *= 0.4f;
                }

            }
        }
        public void ShowMessage(bool show, string msg=null, string okUrl= Constants.slam_close_message, string cancelUrl= "#slam_close_message", Target okTarget=Target._self, Target cancelTarget=Target._self)
        {
            InitiateMessageBox();

            if (msgBox!=null)
            {
                CloseMenu(0);
                StartCoroutine(DoShowMessage(0.5f, show));
                if (show)
                {
                    var so = msgBox.GetComponent<SlamObject>();
                    so.presentation = 1.3f;
                    so.presentationspeed = 5;
                    so.SetPresenting();
                    //so.ClingToCamera = Camera.main.transform.position + 1.3f * Camera.main.transform.forward;
                    //so.FaceCamera = "lock-y";
                    //msgBox.transform.LookAt(Camera.main.transform);
                    var txt = msgBox.GetComponentInChildren<TextMeshPro>();
                    if (txt != null)
                    {
                        txt.text = msg;
                    }
                    var okBtn = msgBox.transform.FindDeepChild(Constants.slam_msgBox_OK_btn);
                    if(okBtn!=null && okUrl!=null)
                    {
                        var slm_ok = okBtn.gameObject.GetComponent<MsgboxButton>();
                        if(slm_ok!=null)
                        {
                            slm_ok.Href = okUrl;
                            slm_ok.Target = okTarget;
                        }
                    }
                    var cancelBtn = msgBox.transform.FindDeepChild(Constants.slam_msgBox_Cancel_btn);
                    if (cancelBtn != null && cancelUrl != null)
                    {
                        var slm_cancel = cancelBtn.gameObject.GetComponent<MsgboxButton>();
                        if (slm_cancel != null)
                        {
                            slm_cancel.Href = cancelUrl;
                            slm_cancel.Target = cancelTarget;
                        }
                    }
                }
            }
        }
        IEnumerator DoShowMessage(float t, bool show)
        {
            yield return new WaitForSeconds(t);
            if(msgBox!=null)
            {
                msgBox.SetActive(show);
            }

        }
        public void ReLoad()
        {
            if (!string.IsNullOrEmpty(UrlHandler.CurrentPageUrl))
            {
                Parse(UrlHandler.CurrentPageUrl, Target._blank);
            }
        }
        public void ShowNotification(string text, string textureName=null)
        {
            var notification_pref = GetPrefab(SceneGO.transform, Constants.slam_prefabgroup_various, Constants.slam_notification_prefab);
            notification_pref.SetActive(true);
            slam_notification sn = notification_pref.GetComponent<slam_notification>();
            sn.Notify(text, ActiveNotifications, textureName);
            ActiveNotifications++;
        }
        public void HideNotification(GameObject go)
        {
            Destroy(go);
            if(ActiveNotifications>0)
            {
                ActiveNotifications--;
            }
            //notification.gameObject.SetActive(false);
        }
#endregion messagebox, notification
#region waitcursor
        public void ShowWaitCursor(bool show = true, DownloadType downloadtype= DownloadType.Xml)
        {
            if (waitCursor != null)
            {
                switch(downloadtype)
                {
                    case DownloadType.Xml:
                        downloading.Xml = show;
                        break;
                    case DownloadType.Mesh:
                        downloading.Mesh = show;
                        break;
                    case DownloadType.Texture:
                        downloading.Texture = show;
                        break;
                    case DownloadType.Video:
                        downloading.Video = show;
                        break;
                    case DownloadType.AssetBundle:
                        downloading.AssetBundle = show;
                        break;
                }
                DoShowWaitCursor();
                //if(show)
                //{
                //    waitCursor.transform.position = Camera.main.transform.position + 2 * Camera.main.transform.forward;
                //}
                //waitCursor.SetActive(show);
            }
        }
        public bool IsDownloading()
        {
            if(hideWaitCursor || hideWaitCursor2)
            {
                return false;
            }
            return downloading.Xml || downloading.Mesh || downloading.Texture || downloading.Video || downloading.AssetBundle;
        }
        public bool IsxmlOrTextureDownloading()
        {
            return downloading.Xml || downloading.Texture;
        }
        void DoShowWaitCursor()
        {
            bool show = IsDownloading();
            if (show)
            {
                if (!waitCursor.activeSelf)
                {
                    waitCursor.transform.position = Camera.main.transform.position + 2 * Camera.main.transform.forward;
                }
            }
            waitCursor.SetActive(show);

        }
#endregion waitcursor
#region textinput, menu
        GameObject activeTextInput = null;
        public void OpenMenuForTextInput(GameObject go)
        {
            activeTextInput = go;
            SlamMenu.Instance.externalText = go.GetComponent<TextMeshPro>();
            var txt = SlamMenu.Instance.externalText.text;
            var so = activeTextInput.GetComponent<SlamObject>();
            if(txt.StartsWith("{") && txt.EndsWith("}")|| so.InputType== InputType.Community )
            {
                txt = "";
            }
            SlamMenu.Instance.SetInputText(txt, true, MenuInputType.Input, so.InputType);
            menu.SetActive(true);
            // ClearMenuMessage();
            PositionMenu(go.transform.position);
            SlamMenu.Instance.StartExternalTyping();
        }
        public void SaveTextInput(string text, bool finish=false)
        {
            if(activeTextInput!=null)
            {
                var so = activeTextInput.GetComponent<SlamObject>();
                text = text.Replace("'", "’");
                so.FormFieldValue = text;
                if (finish)
                {
                    activeTextInput = null;
                }
               // CloseMenu(0.001f);
            }
        }
        public void OpenMenu()
        {
            OpenMenu(Vector3.zero);

        }
        public void OpenMenu(Vector3 startPosition)
        {
            if (SlamMenu.Instance != null)
            {
                StartCoroutine(InitiateSoundTools(1));
                SlamMenu.Instance.externalText = null;
                // menu.transform.position = Camera.main.transform.position + 2 * new Vector3( Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized ;
                if (SlamMenu.Instance.MenuInputType != MenuInputType.Url)
                {
                    SlamMenu.Instance.SetInputText(UrlHandler.CurrentPageUrl, true, MenuInputType.Url);
                }
                //else
                //{
                    SlamMenu.Instance.StartExternalTyping();
                //}
                menu.SetActive(true);
                ClearMenuMessage();
                PositionMenu(startPosition);
            }
        }
        public void PositionMenu(Vector3 startPosition)
        {
            var menuT = SlamMenu.Instance.gameObject.transform;
            menuT.position = startPosition;
            menuT.localScale = 0.01f * Vector3.one;
            SlamMenu.Instance.targetPostion = Camera.main.transform.position + DeviceManager.MainMenuDistance * new Vector3( Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
            menuT.LookAt(Camera.main.transform);

        }
        public string GetTypedTextForExternalKeyboard()
        {
            if (slamMenu != null && menu.activeSelf)
            {
                return slamMenu.typedText;
            }
            return null;
        }
        public void CheckExternalTextInput(string externalKeyboardText)
        {
            if (externalKeyboardText!=null && slamMenu  != null && menu.activeSelf)
            {
                slamMenu.typedText = externalKeyboardText;
            }
        }
        public void CopyPaste(bool paste)
        {
#if !UNITY_WSA// && !UNITY_EDITOR
            if (menu!=null)
            {
                var ms = menu.GetComponent<SlamMenu>();
                if(ms!=null)
                {
                    if(paste)
                    {

                        ms.PasteInputText(GUIUtility.systemCopyBuffer); 
                    }
                    else if(ms.InputType!= InputType.Password)
                    {
                        GUIUtility.systemCopyBuffer = ms.typedText;
                    }
                }
            }
#endif
        }
        public void CloseMenu()//needs to be here!
        {
            CloseMenu(1);
        }
        public void CloseMenu(float time = 1)
        {
            stopRecording();
            Typespace.Typer.WordsRecieved.Clear();
            Typespace.Typer.WordsToSend.Clear();
            Typespace.Typer.AddCommandToSend(Typespace.typecmd.stop);
            StartCoroutine(DoCloseMenu(time));
        }
        IEnumerator DoCloseMenu(float time = 1)
        {
            yield return new WaitForSeconds(time);
            yield return Typespace.Typer.WordsToSend.Count > 0;
            menu.SetActive(false);
            typer = null;

        }
        public void ClearMenuMessage()
        {
            Transform t = GameObject.Find("slam_returnMessage").transform;
            for (int nn = 0; nn < t.transform.childCount; nn++)
            {
                Destroy(t.transform.GetChild(nn).gameObject);
            }

        }
        public void Like()
        {
            string url = UrlHandler.FullSceneUrl;
            Transform t = GameObject.Find("slam_returnMessage").transform;
            string parseUrl = UrlHandler.GetBaseUrl() + "formhandler.aspx?u=" + WWW.EscapeURL(url);
            parseUrl += "&l=true";
            Parse(parseUrl, Target._self, t);
        }
        public void SuggestUrl()
        {
            string url = UrlHandler.CurrentPageUrl;
            Transform t = GameObject.Find("slam_returnMessage").transform;
            string parseUrl = UrlHandler.GetBaseUrl() + "/formhandler.aspx?u=" + WWW.EscapeURL(url);
            if (SlamMenu.Instance.typedText.ToLower() != UrlHandler.FullSceneUrl.ToLower())
            {
                parseUrl += "&f=" + WWW.EscapeURL(SlamMenu.Instance.typedText.ToLower());
            }
            else if (!string.IsNullOrEmpty(currentSearchTerms))
            {
                parseUrl += "&s=" + WWW.EscapeURL(currentSearchTerms);
            }
            Parse(parseUrl, Target._self, t);
        }
        public void InviteAFriend()
        {
            if (slam_session == null)
            {
                string loginUrl = UrlHandler.GetBaseUrl() + "forms/login.aspx";
                returnPage = UrlHandler.FullSceneUrl;
                ShowMessage(true, "You need to be logged in to use this feature. Do you want to login now?", loginUrl, Constants.slam_close_message, Target._blank, Target._self);
            }
            else
            {
                CloseMenu();
                returnPage = UrlHandler.FullSceneUrl;
                string inviteUrl = UrlHandler.GetBaseUrl() + "forms/invite.aspx";
                Parse(inviteUrl, Target._blank);
            }

        }
        void CreateSmallMenu()
        {
            if (smallMenu == null)
            {
                smallMenu = new GameObject();
                smallMenu.name = "smallMenu";
                var homeButton = MakeMenuPart(smallMenu.transform, "slam_home", new Vector3(0.3f, 0.3f, 0.03f), new Vector3(0, 0, 0));
                var hm = homeButton.AddComponent<GoHomeMenu>();
                hm.ToolTip = "Say: " + Constants.Speech_Go_Home;
                var sm = smallMenu.AddComponent<SmallMenu>();
                if (IsHoloLens())
                {
                    sm.hDis = 2f;
                }
                sm.FaceCamera = "face";
                //main menu button
                GameObject openMainMenu = MakeMenuPart(smallMenu.transform, "slam_mainmenu", new Vector3(0.2f, 0.2f, 0.02f), new Vector3(0, 20, 0));
                openMainMenu.transform.localPosition = new Vector3(-0.5f, -0.15f, 0.15f);
                SlamObject so = openMainMenu.AddComponent<SlamObject>();
                so.Href = Constants.slam_menu;
                so.ToolTip = "Say: " + Constants.Speech_Open_Menu;
                //mic button
                GameObject micOnOf = MakeMenuPart(smallMenu.transform, "slam_mic_off", new Vector3(0.2f, 0.2f, 0.02f), new Vector3(0, -20, 0));
                micOnOf.transform.localPosition = new Vector3(0.5f, -0.15f, 0.15f);
                micScript = micOnOf.AddComponent<MicOnOff>();
                //micScript.Href = "#slam_menu";
                micScript.ToolTip = "Microphone On/Off";
                if (!(DeviceManager.IsUWP && DeviceManager.IsOpaque))
                {
                    //left turn
                    GameObject rightTurn = MakeMenuPart(smallMenu.transform, "slam_turn_right", new Vector3(0.1f, 0.1f, 0.01f), new Vector3(0, 0, 0));
                    rightTurn.transform.localPosition = new Vector3(-0.14f, -0.27f, 0.15f);
                    Turn rturnScript = rightTurn.AddComponent<Turn>();
                    //micScript.Href = "#slam_menu";
                    rturnScript.TurnSpeed = 10;
                    rturnScript.ToolTip = "Turn right";
                    //left turn
                    GameObject leftTurn = MakeMenuPart(smallMenu.transform, "slam_turn_left", new Vector3(0.1f, 0.1f, 0.01f), new Vector3(0, 0, 0));
                    leftTurn.transform.localPosition = new Vector3(0.14f, -0.27f, 0.15f);
                    Turn turnScript = leftTurn.AddComponent<Turn>();
                    //micScript.Href = "#slam_menu";
                    turnScript.TurnSpeed = -10;
                    turnScript.ToolTip = "Turn left";
                }


                //test button
                //GameObject testBtn = MakeMenuPart(smallMenu.transform, "test", new Vector3(0.2f, 0.2f, 0.02f), new Vector3(0, -20, 0));
                //testBtn.transform.localPosition = new Vector3(0, -0.6f, 0.15f);
                //var testScript = testBtn.AddComponent<TestScript>();
                //testScript.ToolTip = "Test";


                var lightGo = new GameObject();
                var light = lightGo.AddComponent<Light>();
                light.range = 1;
                light.type = LightType.Point;
                light.intensity = 3;
                lightGo.transform.position = new Vector3(0f, -0.26f, 0.5f);
                lightGo.transform.SetTheParent(smallMenu.transform);

            }
            smallMenu.transform.position = -viewpointPosition + new Vector3(0, 2.0f, 0);
            smallMenu.transform.localScale = 0.7f * Vector3.one;
            if (IsHoloLens())
            {
                smallMenu.transform.localScale = 0.4f * Vector3.one;
            }


            StartSceneCoroutine(CheckSmallMenuVisibility());
        }

        GameObject MakeMenuPart(Transform parent, string textureName, Vector3 localScale, Vector3 rotate)
        {
            GameObject go = GetPrefab(parent, Constants.primitivesGroup, Constants.spherePrefabName);
            go.transform.localScale = localScale;
            go.transform.Rotate(rotate);
            go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            go.name = textureName;
            Renderer renderer = go.GetComponent<Renderer>();
            UnityEngine.Material mat = GetMaterial("1 1 1");
            AssignMaterialToGameObject(go, mat);
            renderer.material.mainTexture = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
            return go;
        }

        IEnumerator CheckSmallMenuVisibility()
        {
            yield return new WaitForEndOfFrame();
            smallMenu.SetActive(showSmallMenu);
        }
        public void AddToHistory()
        {
            if (!string.IsNullOrEmpty(UrlHandler.FullSceneUrl) && !UrlHandler.AtHome)
            {
                DeviceManager.AddHistory(UrlHandler.FullSceneUrl);
            }
        }
        public void AddToFavorites()
        {
            if (!UrlHandler.AtHome && !string.IsNullOrEmpty(UrlHandler.FullSceneUrl))
            {
                DeviceManager.AddFavorite(UrlHandler.FullSceneUrl);
            }
        }
        public void RemoveFromFavorites()
        {
            DeviceManager.RemoveFavorite(UrlHandler.FullSceneUrl);
        }
        public void Browse2d(string url)
        {
            LeaveApp sc = null;
            if (leaveAppObject == null)
            {
                leaveAppObject = GetPrefab(null, "slam", "leaveApp");
                //leaveAppObject.name = "Leave3dExperienceMenu";
                sc = leaveAppObject.GetComponent<LeaveApp>();
                if (IsHoloLens())
                {
                    //leaveAppObject.transform.localScale = 0.02f * Vector3.one;
                }

            }
            else
            {
                sc = leaveAppObject.GetComponent<LeaveApp>();
            }
            sc.Href = url;
            sc.Target = Target._2D;
            sc.conFirmLeaveApp = false;
            //leaveAppObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            hideLeaveApp = false;
            leaveAppObject.transform.position = Camera.main.transform.position + DeviceManager.PresentationDistanceCorrection * Camera.main.transform.forward;
            leaveAppObject.SetActive(true);


        }
        public void HideLeaveApp(bool hide, float time = 2)
        {
            hideLeaveApp = hide;
            if (hideLeaveApp)
            {
                StartCoroutine(DoHideLeaveApp(time));
            }
        }
        IEnumerator DoHideLeaveApp(float time = 2)
        {
            yield return new WaitForSeconds(time);
            if (hideLeaveApp && leaveAppObject != null)
            {
                if (leaveAppObject != null)
                {
                    leaveAppObject.gameObject.SetActive(false);
                }
            }
        }
#endregion  textinput, menu
#region Parse
        public void Parse(string url, Target target = Target._blank, Transform transform = null, bool showWaitCursor=true)
        {
            if (target != Target._2D && !string.IsNullOrEmpty(url))
            {
                downloading = new Downloading();
                if (!showWaitCursor)
                {
                    hideWaitCursor2 = true;
                }
                else
                {
                    ShowWaitCursor(true, DownloadType.Xml);
                }
                url = UrlHandler.CheckTilde(url);
                if (target == Target._blank)
                {
                    SetCallingApplicationHeaderValue();
                    inlineParentGP = null;
                    UrlHandler.CurrentPageUrl = url;
                    StopDownloadCoroutines();
                    StopSpeak();
                    chatroom = null;
                    //if (voiceBroadCastTrigger!=null)
                    //{
                    //    voiceBroadCastTrigger.RoomName = null;
                    //    voiceBroadCastTrigger.enabled = false;
                    //}
                    //if (voiceReceiptTrigger != null)
                    //{
                    //    voiceReceiptTrigger.RoomName = null;
                    //if (voiceReceiptTrigger != null)
                    //{
                    //   // voiceReceiptTrigger.enabled = true;
                    //}
                    //}
                    if (walkFloorIndicator!=null)
                    {
                        walkFloorIndicator.SetActive(false);
                    }
                }
                else if(target== Target._self || target== Target._inline)
                {
                    UrlHandler.SetInlineUrl(url);
                }
                StartSceneCoroutine(GetXML(UrlHandler.GetUrl(url), target, transform, UrlHandler.InlineUrl));
            }
        }
        private void StartSceneCoroutine(IEnumerator coroutine)
        {
            SceneCoroutines.Add(coroutine);
            StartCoroutine(coroutine);
        }

        private void StopDownloadCoroutines()
        {
            foreach(var coroutine in SceneCoroutines)
            {
                StopCoroutine(coroutine);
            }
            SceneCoroutines.Clear();
        }

        IEnumerator GetXML(string url, Target target, Transform transform, string inlineUrl)
        {
            /* For using rest service*/
            if (string.IsNullOrEmpty(url))
            {
                throw new System.Exception("No Url provided");
            }
            byte[] rawData = null;
            WWWForm form = null;
            if ((formFields.Count>0 ||!string.IsNullOrEmpty(returnPage) ||  !string.IsNullOrEmpty(slam_session)) && UrlHandler.AllowPost(url))
            {
                form = new WWWForm();
                if (!string.IsNullOrEmpty(slam_session))
                {
                    form.AddField(Constants.slam_sessionfld, slam_session);
                }
                if (!string.IsNullOrEmpty(DeviceManager.UserGuid))
                {
                    form.AddField(Constants.slam_userguidfld, DeviceManager.UserGuid);
                }
                if (!string.IsNullOrEmpty(returnPage))
                {
                    form.AddField(Constants.returnPageCodefld, returnPage);
                }
                foreach (var item in formFields)
                {
                    if (item != null && item.activeSelf)
                    {
                        var slamobj = item.GetComponent<SlamObject>();
                        if (slamobj != null)
                        {
                            form.AddField(slamobj.FormFieldName, slamobj.FormFieldValue);
                            if(slamobj.FormFieldName== Constants.fld_login || slamobj.FormFieldName == Constants.fld_password)
                            {
                                if (!string.IsNullOrEmpty(slamobj.FormFieldValue))
                                {
                                    var val = slamobj.FormFieldValue;
#if !UNITY_WSA //&& !UNITY_EDITOR
                                    val = AESCrypt.EncryptData(slamobj.FormFieldValue, Constants.Slamkey);
#endif
                                    SaveFormField(slamobj.FormFieldName, val);
                                }
                            }
                        }
                    }
                }
                rawData = form.data;
            }
            var headers = GetSlamHttpHeaders();
            if (target == Target._self || target== Target._inline)
            {
                Debug.Log(string.Format("Start downloading 'Inline' {0} at {1}", url, Time.realtimeSinceStartup.ToString()));
            }
            UnityWebRequest www;
            if (form==null)
            {
                www = UnityWebRequest.Get(url);
            }
            else
            {
                www = UnityWebRequest.Post(url, form);
            }
            foreach(var h in headers)
            {
                www.SetRequestHeader(h.Key, h.Value);
            }
          //  WWW www = new WWW(url, rawData, headers);
            yield return www.SendWebRequest();
                if (string.IsNullOrEmpty( www.error))
            {
                UrlHandler.InlineUrl = inlineUrl;
                if (target == Target._self || target == Target._inline)
                {
                    Debug.Log(string.Format("End downloading 'Inline' {0} at {1}, start parsing", url, Time.realtimeSinceStartup.ToString()));

                }
                else if (target == Target._blank)
                {
                    UrlHandler.FullSceneUrl = url;
                }
                if (SceneGO != null && target == Target._blank)
                {
                    SceneGO.transform.position = Vector3.zero;
                }
                HandleXml(www.downloadHandler.text, url, target, transform);
            }
            else
            {
                if (target == Target._blank)
                {
                    ShowMessage(true, string.Format("Error trying to get {0}: {1}", url, www.error), UrlHandler.CurrentPageUrl, Constants.slam_close_message, Target._blank);
                    ShowWaitCursor(false, DownloadType.Xml);
                }
            }
            if (target == Target._blank || target == Target._self || target== Target._inline)
            {
                int inlinecnt = inlines.Count;
                foreach (var key in inlines)
                {
                    foreach (var t in key.Value)
                    {
                        if (!t.Handled)
                        {
                            t.Handled = true;
                            UrlHandler.SetInlineUrl(key.Key);
                            Parse(key.Key, Target._self, t.Transform);
                        }
                    }
                }
                inlineParentGP = null;
                if (inlinecnt == inlines.Count)
                {
                    inlines.Clear();
                }
                UrlHandler.ClearInlineUrl();
                if (!TryRetrieveMeshesStarted)
                {
                    InvokeRepeating("TryRetrieveMeshes", 0.1f, 0.5f);
                    TryRetrieveMeshesStarted = true;
                }
                CheckHandDraggablePreps();
                SetFog();
                ShowWaitCursor(false, DownloadType.Xml);
                //

            }
        }

        private void SaveFormField(string fieldName, string fieldValue)
        {
            DeviceManager.AssureStoredField(UrlHandler.FullSceneUrl, fieldName, fieldValue);
        }
 
        public Dictionary<string, string> GetSlamHttpHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add(Constants.RH_Calling_Device,Constants.callingDeviceHeader);
            headers.Add(Constants.RH_USER_AGENT, "vSlam/" + Constants.SlamVersion);
            TimeSpan offset = TimeSpan.FromHours(0);
#if !UNITY_WSA
            System.TimeZone localZone = TimeZone.CurrentTimeZone;
            offset = localZone.GetUtcOffset(DateTime.UtcNow);
#else
            offset= DateTime.Now.Subtract(DateTime.Now.ToUniversalTime());
#endif

            headers.Add(Constants.RH_Timezone_Offset, offset.Hours.ToString());
            return headers;

        }
        void HandleXml(string text, string url, Target target, Transform transform)
        {
            if (transform != null)
            {
                //transform.position -= viewpointPosition;
                inlineParentGP = transform.gameObject;
                if(transform.tag== Constants.clearTransformTag)
                {
                    for (int nn = 0; nn < transform.childCount; nn++)
                    {
                        Destroy(transform.GetChild(nn).gameObject);
                    }

                }
            }
            UrlHandler.SetUrlProperties(url, target);
            if (target == Target._blank)
            {
                CleanUpScene();

                showSmallMenu = true;//getBaseHomeUrl().ToLower() != url.ToLower();
                SlamMenu.Instance.SetInputText(url);
                AddToHistory();
                if(!UrlHandler.AtHome )
                {
                    CheckSoundInitiated();
#if USE_DISS
                   if (voiceBroadCastTrigger != null && voiceReceiptTrigger != null)
                    {
                        chatroom = UrlHandler.GetRoomFromUrl(url);
                        dissonanceComms.Rooms.Leave(roomMembership);
                        roomMembership=dissonanceComms.Rooms.Join(chatroom);
                        
                        voiceBroadCastTrigger.RoomName = chatroom;
                        voiceReceiptTrigger.RoomName = chatroom;

                    }
#endif
                }
                if(!micMute)
                {
                    StartSpeak();
                }
                mrCameraCheckTime = Constants.mrCameraCheckTime;
            }
            else if (target == Target._self || target==Target._inline)
            {
                int pos = url.IndexOf("#");
                if (pos > 0)
                {
                    string goName = url.Substring(pos + 1);
                    pos = goName.IndexOf("?");
                    if(pos>0)
                    {
                        goName = goName.Substring(0, pos);
                    }
                    var tgoName = goName;
                    GameObject go = null;
                    if (goName.EndsWith("@"))
                    {
                        tgoName = goName.Substring(0, goName.Length - 1)+placeHolderCounter.ToString();
                        go = GameObject.Find(tgoName);
                        if(go==null)
                        {
                            placeHolderCounter = 0;
                            tgoName = goName.Substring(0, goName.Length - 1) + placeHolderCounter.ToString();
                            go = GameObject.Find(tgoName);
                        }
                        placeHolderCounter++;
                    }
                    else
                    {
                       go = GameObject.Find(goName);
                    }
                    if (go != null)
                    {
                        for (int nn = 0; nn < go.transform.childCount; nn++)
                        {
                            Destroy(go.transform.GetChild(nn).gameObject);
                        }

                        inlineParentGP = go;
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("The anchor '{0}' was specified for an inline, but a gameobject with that name was not found", goName));
                    }
                }

            }

            Parse(text, url);
            foreach(var tx in textures)
            {

                if (tx.Value!=null && tx.Value.Texture == null)
                {
                    StartCoroutine(GetTexture(tx.Key, tx.Value));
                }
            }
            if (target == Target._blank)
            {

                if (!UrlHandler.IsSameAsPreviousPage()&& !UrlHandler.AtHome )
                {
                    if (!UrlHandler.SingleUser)
                    {
                        Vector2 r = UrlHandler.AtHome ? Vector2.zero : UnityEngine.Random.insideUnitCircle;
                        ViewPoint.Position += new Vector3(r.x, 1, r.y);
                    }
                }
                if (UrlHandler.AtHome)
                {
                    UrlHandler.HomePagePage = currentPage;
                    ShowMessage(false);
                }
                else
                {
                    var entryMessage = UrlHandler.SingleUser ? "Single user page" : "Multiple user page";
                    var textureName = UrlHandler.SingleUser ? "single" : "multiple";
                    ShowNotification(entryMessage, textureName);
                     // //ToDo: add extra conditions
                    // if(!RecordActions)
                    // {
                    //     SceneTimer = 0;
                    // }
                    //// RecordActions = true;
                }
                SceneGO.transform.position = -ViewPoint.Position;
                Scene.Instance.StartPosition = -ViewPoint.Position;
                Camera.main.transform.position = Vector3.zero;
                Camera.main.transform.rotation = Quaternion.identity;
                CreateSmallMenu();
                Scene.Instance.Init(true);
                firstspass = 10;
                if(AutoStartRecord && !RecordActions && menu!=null)
                {
                    var rec= menu.GetComponentInChildren<Record>();
                    if(rec!=null)
                    {
                        rec.DoSelect(new Vector3());
                    }
                }
            }
            CheckAssetBundles();

        }
        void HandleUpdatableInlines()
        {
            Resources.UnloadUnusedAssets();

            foreach (var il in updatableInlines)
            {
                UrlHandler.SetInlineUrl(il.Key);
                Parse(il.Key, Target._self, il.Value.Transform, false);
                currentInline = il.Value;
            }
        }
        void HandleUpdatableTextures()
        {
            Resources.UnloadUnusedAssets();

            foreach (var tx in updatableTextures)
            {
                tx.Value.Texture = null;
                StartCoroutine(GetTexture(tx.Key, tx.Value, true));
            }

        }
        void Parse(string text, string url = "")
        {
            //try
            //{

                X3DParse.X3DParse parser = new X3DParse.X3DParse();
                X3DParse.Node root = parser.Parse(text);

                if (root != null)
                {
                    HandleElementParseNode(root);
                }
            //}
            //catch (System.Exception x)
            //{
            //    Debug.LogWarning(string.Format("Catched Error in Slam.cs: {0}", x.Message));
            //}
            ShowWaitCursor(false, DownloadType.Xml);
        }


        public void Test()
        {
            //if(dissonanceComms!=null)
            //{
            //    dissonanceComms.ResetMicrophoneCapture();
            //}
           // ShowNotification(UnityEngine.WSA.Application.arguments);

        }
        void HandleElementParseNode(X3DParse.Node node, Transform parentT = null)
        {
            if (node != null && !string.IsNullOrEmpty(node.Name) && node.NodeType == X3DParse.NodeType.Element)
            {
                string name = node.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "slm:targetplatform":
                        if (HandleTargetPlatform(node))
                        {
                            foreach (var child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
                            {
                                HandleElementParseNode(child, parentT);
                            }
                        }
                        break;
                    case "slm:assetbundle":
                        HandleAssetBundle(node);
                        break;
                    case "head":
                        HandleHeadNode(node);
                        break;
                    case "scene":
                        HandleSceneNode(node);
                        break;
                    case "group":
                        HandleGroupNode(node, parentT);
                        break;
                    case "inline":
                        HandleInlineNode(node, parentT);
                        break;
                    case "viewpoint":
                        HandleViewPoint(node);
                        break;
                    case "transform":
                        HandleTransFormNode(node, parentT);
                        break;
                    case "shape":
                        HandleShapeNode(node, parentT);
                        break;
                    case "audioclip":
                        HandleAudioClip(node);
                        break;
                    case "pointlight":
                        HandleLight(node, LightType.Point, parentT);
                        break;
                    case "directionallight":
                        HandleLight(node, LightType.Directional, parentT);
                        break;
                    case "spotlight":
                        HandleLight(node, LightType.Spot, parentT);
                        break;
                    case "background":
                        HandleBackGround(node);
                        break;
                    case "fog":
                        HandleFog(node);
                        break;
                    case "navigationinfo":
                        HandleNavigationInfo(node);
                        break;

                    case "slm:speak":
                        HandleSpeak(parentT, node);
                        break;
                    case "slm:treeview3d":
                        HandleTreeView3d(parentT, node);
                        break;
                    case "slm:listview3d":
                        HandleListView3d(parentT, node);
                        break;
                    case "slm:sceneaction":
                        HandleSceneAction(node);
                        break;
                    case "constraints":
                        HandleConstraints(node);
                        break;
                    case "html":
                        HandleHtml(node, parentT);
                        break;



                    default:
                        foreach (var child in node.Children)
                        {
                            HandleElementParseNode(child, parentT);
                        }
                        break;
                }
            }

        }
        
        void HandleHtml(X3DParse.Node node, Transform parent)
        {
            HtmlParser htmlParser = new HtmlParser();
            htmlParser.HandleHtml(node, parent);
        }

        void HandleConstraints(X3DParse.Node head)
        {
            string groupname = null;

            foreach (X3DParse.Node child in head.Children.FindAll(x=>x.NodeType==X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "name":
                        groupname = child.Value;
                        break;
                }
            }
            if(!string.IsNullOrEmpty(groupname))
            {
                HandleConstraints(groupname, head);
            }
        }
        void HandleConstraints(string groupName, X3DParse.Node head)
        {
            List<Constraint> list = null;
            constraintgroups.TryGetValue(groupName, out list);
            if(list==null)
            {
                list= new List<Constraint>(); 
            }
            foreach (var child in head.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "constraint":
                        list.Add(DoHandleConstraint(child));
                        break;
                }
            }
            constraintgroups[groupName] = list;
        }

        void HandleSceneAction(X3DParse.Node head)
        {
            float sceneTime = 0;
            string actionname = null;

            foreach (X3DParse.Node child in head.Children)
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "actionname":
                        actionname = child.Value;
                        break;
                    case "scenetime":
                       float.TryParse( child.Value, out sceneTime);
                        break;
                }
            }
            if(!string.IsNullOrEmpty(actionname))
            {
                if (actionname.ToLower() == Constants.resetActions)
                {
                    ClearActions();
                    SceneTimer = sceneTime;
                }
                else
                {
                    ActionsToPerform.Enqueue(new SceneAction(actionname, sceneTime));
                }
            }

        }
        void HandleAssetBundle(X3DParse.Node head)
        {
            string url = null;
            float sizeInBytes=0;
            string bundleName = null;

            foreach (X3DParse.Node child in head.Children)
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "url":
                        url = UrlHandler.CheckInlineUrl(child.Value.Replace("\"", ""));
                        break;
                    case "name":
                        bundleName= child.Value.ToLower();
                        break;
                    case "sizeinbytes":
                        string zib= child.Value.ToLower();
                        float.TryParse(zib, out sizeInBytes);
                        break;
                }
             }
            if(!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(bundleName) && !assetBundles.ContainsKey(bundleName))
            {
                AssetBundleData abd = new AssetBundleData();
                abd.SizeInBytes = sizeInBytes;
                abd.Name = bundleName;
                url = UrlHandler.GetUrl(url);
                assetBundleDownloads[url] = abd;
            }
        }
       void HandleHeadNode(X3DParse.Node head)
        {
            foreach (X3DParse.Node child in head.Children)
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "meta":
                        HandleMetaNode(child);
                        break;
                }
            }
        }
        void HandleMetaNode(X3DParse.Node head)
        {
            string tname = null;
            string content = null;
            foreach (X3DParse.Node child in head.Children.FindAll(x=>x.NodeType== X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "name":
                        tname=child.Value.ToLower();
                        break;
                    case "content":
                        content = child.Value.ToLower();
                        break;
                }
            }
            MetaType metaType = MetaType.none;
            if(TryGetMetatType(tname, out metaType) && ! string.IsNullOrEmpty(content))
            {
                switch(metaType)
                {
                    case MetaType.keywords:
                        currentSearchTerms = content;
                        break;
                    case MetaType.filter:
                        currentFilter = content;
                        break;
                    case MetaType.page:
                        currentPage = content;
                        break;
                    case MetaType.allowrecording:
                        AllowRecording = Statics.CheckBoolString(content);
                        break;
                    case MetaType.autostartrecord:
                        AutoStartRecord = Statics.CheckBoolString(content);
                        break;
                        
                    case MetaType.singleuser:
                        UrlHandler.SingleUser=Statics.CheckBoolString(content);
                        break;

                }
            }
        }
        bool TryGetMetatType(string metattypestring, out MetaType metaType)
        {
            metaType = MetaType.none;
            metattypestring = string.IsNullOrEmpty(metattypestring) ? "" : metattypestring.ToLower();
            switch (metattypestring)
            {
                case "keywords":
                    metaType= MetaType.keywords;
                    break;
                case "filter":
                    metaType=MetaType.filter;
                    break;
                case "page":
                    metaType=MetaType.page;
                    break;
                case "allowrecording":
                    metaType = MetaType.allowrecording;
                    break;
                case "autostartrecord":
                    metaType = MetaType.autostartrecord;
                    break;
                case "singleuser":
                    metaType = MetaType.singleuser;
                    break;
                default:
                    return false;
            }
            return true;
        }
        void HandleSceneNode(X3DParse.Node scene)
        {
            foreach (X3DParse.Node child in scene.Children.FindAll(x=>x.NodeType== X3DParse.NodeType.Attribute))
            {
                var name = child.Name.ToLower();
                switch(name)
                {
                    case "id":
                        var sceneId = child.Value;
                        if (currentInline != null)
                        {
                            if (sceneId == currentInline.Guid)
                            {
                                //hideWaitCursor2 = false;
                                return;
                            }
                            currentInline.Guid = sceneId;
                            for (int nn = 0; nn < currentInline.Transform.childCount; nn++)
                            {
                                Destroy(currentInline.Transform.GetChild(nn).gameObject);
                            }

                        }
                        break;
                }
            }
            foreach (X3DParse.Node child in scene.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                if (inlineParentGP != null)
                {
                    HandleElementParseNode(child, inlineParentGP.transform);//group
                }
                else
                {
                    HandleElementParseNode(child, SceneGO.transform);//group
                }
            }
        }
        void CheckNameDefProperties(X3DParse.Node node, GameObject go)
        {
            string goname = null;
            string def = null;
            foreach (var child in node.Children.FindAll(x=>x.NodeType==X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "name":
                        goname = child.Value;
                        break;
                    case "def":
                        def = child.Value;
                        break;
                }
            }
            if(!string.IsNullOrEmpty(goname))
            {
                go.name = goname;
            }
            else if (!string.IsNullOrEmpty(def))
            {
                go.name = def;
            }
        }
        void HandleGroupNode(X3DParse.Node group, Transform parentT)
        {
            GameObject gameObject = new GameObject("group");
            gameObject.transform.SetTheParent(parentT);
            CheckNameDefProperties(group, gameObject);
            ApplyTransformations(group, gameObject.transform);
            //gameobjects.Add(gameObject);
            foreach (var child in group.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                HandleElementParseNode(child, gameObject.transform);
            }
            //if (viewPoint != null)
            //{
            //    sceneGO.transform.position = -viewPoint.Position;
            //    gameObject.transform.localPosition = Vector3.zero;
            //}
        }
        void HandleInlineNode(X3DParse.Node group, Transform parentT)
        {
            string url = null;
            bool updatable = false;
            foreach (var child in group.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "url":
                        url = child.Value;
                        break;
                    case "clearparent":
                        if (Statics.CheckBoolString(child.Value) && parentT!=null)
                        {
                            parentT.tag = Constants.clearTransformTag;
                        }
                        break;
                    case "updatable":
                        if (Statics.CheckBoolString(child.Value))
                        {
                            updatable=true;
                        }
                        break;

                }
            }
            if (url != null)
            {
                if (updatable)
                {
                    Inline il = new Inline();
                    updatableInlines[url] = il;
                    il.Transform = parentT;

                }
                else
                {
                    if (!inlines.ContainsKey(url))
                    {
                        inlines[url] = new List<Inline>();
                    }
                    Inline il = new Inline();
                    il.Transform = parentT;
                    inlines[url].Add(il);
                }
            }
        }
        void HandleViewPoint(X3DParse.Node viewPointNode)
        {
            ViewPoint = new ViewPoint();
            foreach (var child in viewPointNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "position":
                        ViewPoint.Position = Vector3FromString(child.Value);
                        viewpointPosition = ViewPoint.Position;
                        break;
                }
            }

            //ApplyTransformations(viewPointNode, parentT);
        }
        GameObject HandleTransFormNode(X3DParse.Node transformNode, Transform parent)
        {
            GameObject gameObject = new GameObject("transform");
            gameObject.transform.SetTheParent(parent);
           CheckNameDefProperties(transformNode, gameObject);
            foreach (var child in transformNode.Children.FindAll(x=>x.NodeType== X3DParse.NodeType.Element))
            {
                HandleElementParseNode(child, gameObject.transform);
            }
            ApplyTransformations(transformNode, gameObject.transform);
            //if (inlineParentGP != null)
            //{
            //    gameObject.transform.localPosition = Vector3.zero;
            //    inlineParentGP = null;
            //}
            return gameObject;
        }
        bool HandleTargetPlatform(X3DParse.Node platformNode)
        {
            bool include = false;
            string platform = null;
            foreach (var child in platformNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name=child.Name.ToLower();
                switch(name)
                {
                    case "platform":
                        platform = child.Value.ToLower();
                        break;
                    case "mode":
                        include = child.Value == "include";
                        break;
                }
            }
            if (platform.Contains(DeviceManager.Name.ToLower()))
            {
                return include;
            }
            else
            {
                if(include)
                {
                    return false;
                }
            }
            return true;
        }
        GameObject HandleAvatar(X3DParse.Node shapeNode, Transform parent)
        {
            myAvatarPresentation=new GameObject();
            myAvatarPresentation.transform.SetTheParent(parent);
            return myAvatarPresentation;
        }
        void HandleListTypeSimple(GameObject listboxObject, X3DParse.Node treeViewNode, GameObject prefab, GameObject nextPrefab, GameObject prevPrefab, Vector3 navigationButtonPosition, Vector3 navigationButtonScale, int pageSize)
        {
            var slv = listboxObject.AddComponent<SimpleListBox>();
            slv.nodePrefab = prefab;
            slv.nextPrefab = nextPrefab;
            slv.previousPrefab = prevPrefab;
            slv.navigationButtonPosition = navigationButtonPosition;
            slv.navigationButtonScale = navigationButtonScale;
            slv.PageSize = pageSize;
            foreach(var node in treeViewNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                if(node.Name.ToLower()=="node")
                {
                    slv.ParentNode.Children.Add(node);
                }
            }
            slv.Initiate();
        }
        void HandleListView3d(Transform parent, X3DParse.Node treeViewNode)
        {
            if (assetBundleDownloads.Count > 0)
            {
                LateGameObjects.Add(new LateGameObject(treeViewNode, parent, LateGameObjectType.ListView));
                return;
            }
            GameObject listboxObject = new GameObject();
            listboxObject.transform.SetTheParent(parent);
            listboxObject.transform.localPosition = Vector3.zero;
            CheckNameDefProperties(treeViewNode, listboxObject);
            string listname = "ListView3D";
            Vector3 startP = Vector3.zero;
            Vector3 middle1P = Vector3.zero;
            Vector3 middle2P = Vector3.zero;
            Vector3 endP = Vector3.zero;
            Vector3 startRot = Vector3.zero;
            Vector3 middle1Rot = Vector3.zero;
            Vector3 middle2Rot = Vector3.zero;
            Vector3 endRot = Vector3.zero;
            Vector3 navigationButtonScale = Vector3.one;
            Vector3 navigationButtonPosition = Vector3.zero;
            bool startVectorSet = false;
            bool middle1VectorSet = false;
            bool middle2VectorSet = false;
            bool endVectorSet = false;
            int pageSize = 7;
            ListType listtype = ListType.standard;
            foreach (var child in treeViewNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "name":
                        listname = child.Value;
                        break;
                    case "start":
                        if(TryVector3FromString(child.Value, out startP))
                        {
                            startVectorSet = true;
                        }
                        break;
                    case "middle1":
                        if (TryVector3FromString(child.Value, out middle1P))
                        {
                            middle1VectorSet = true;
                        }
                        break;
                    case "middle2":
                        if (TryVector3FromString(child.Value, out middle2P))
                        {
                            middle2VectorSet = true;
                        }
                        break;
                    case "end":
                        if (TryVector3FromString(child.Value, out endP))
                        {
                            endVectorSet = true;
                        }
                        break;
                    case "startrot":
                        if (TryVector3FromString(child.Value, out startRot))
                        {
                            startVectorSet = true;
                        }
                        break;
                    case "middle1rot":
                        if (TryVector3FromString(child.Value, out middle1Rot))
                        {
                            middle1VectorSet = true;
                        }
                        break;
                    case "middle2rot":
                        if (TryVector3FromString(child.Value, out middle2Rot))
                        {
                            middle2VectorSet = true;
                        }
                        break;
                    case "endrot":
                        if (TryVector3FromString(child.Value, out endRot))
                        {
                            endVectorSet = true;
                        }
                        break;
                    case "pagesize":
                        int.TryParse(child.Value, out pageSize);
                        break;
                    case "navigationbuttonscale":
                        TryVector3FromString(child.Value, out navigationButtonScale);
                        break;
                    case "navigationbuttonposition":
                        TryVector3FromString(child.Value, out navigationButtonPosition);
                        break;
                    case "listtype":
                        TryListTypeFromString(child.Value, out listtype);
                        break;

                }
            }
            GameObject prefab = null;
            GameObject nextButtonPrefab = null;
            GameObject resetButtonPrefab = null;
            bool hasParentTransform = false;
            foreach (var prefchild in treeViewNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = prefchild.Name.ToLower();
                switch (name)
                {
                    case "nodeprefab":
                        prefab = HandleNodePrefab(prefab, prefchild, out hasParentTransform);
                        break;
                    case "nextbuttonprefab":
                        nextButtonPrefab = HandleNodePrefab(prefab, prefchild, out hasParentTransform);
                        break;
                    case "resetbuttonprefab":
                        resetButtonPrefab = HandleNodePrefab(prefab, prefchild, out hasParentTransform);
                        break;
                }
            }
            if (listtype == ListType.simple)
            {
                HandleListTypeSimple(listboxObject, treeViewNode, prefab, nextButtonPrefab, resetButtonPrefab, navigationButtonPosition, navigationButtonScale, pageSize);
                return;
            }

            VSlamHL.Listbox3d listbox = listboxObject.AddComponent<VSlamHL.Listbox3d>();
            if (startVectorSet)
            {
                listbox.firstItemDuoVector = new VSlamHL.DuoVector3(startP, startRot);
            }
            if (middle1VectorSet)
            {
                listbox.middle1 = new VSlamHL.DuoVector3(middle1P, middle1Rot);
            }
            if (middle2VectorSet)
            {
                listbox.middle2 = new VSlamHL.DuoVector3(middle2P, middle2Rot);
            }
            if (endVectorSet)
            {
                listbox.end = new VSlamHL.DuoVector3(endP, endRot);
            }
            listbox.PageSize = pageSize;
            bool nextButtonHasPrefab = false;
            bool resetButtonHasPrefab = false;
            nextButtonPrefab = AssureNavigationButtonPrefab(listbox, nextButtonPrefab, false, navigationButtonScale, out nextButtonHasPrefab);
            resetButtonPrefab = AssureNavigationButtonPrefab(listbox, resetButtonPrefab, true, navigationButtonScale, out resetButtonHasPrefab);
            GameObject navigation = new GameObject();
            nextButtonPrefab.transform.parent = navigation.transform;
            resetButtonPrefab.transform.parent = navigation.transform;
            if (!nextButtonHasPrefab)
            {
                nextButtonPrefab.transform.localPosition = new Vector3(-0.4f, 0.2f, 0);
            }
            if (!resetButtonHasPrefab)
            {
                resetButtonPrefab.transform.localPosition = new Vector3(-0.4f, 0.6f, 0);
            }
            listbox.navigation = navigation;
            navigation.transform.SetTheParent(listboxObject.transform);
            navigation.SetActive(false);
            int no = 0;
            foreach (var child in treeViewNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "node":
                        no++;
                        HandleListBoxNode(listbox, child,no, ref prefab);
                        break;

                }
            }
            if(prefab!=null)
            {
                Destroy(prefab);
            }
            StartSceneCoroutine(initListBox(listbox));
            //StartCoroutine(initListBox(listbox));
        }
        IEnumerator initListBox(VSlamHL.Listbox3d listbox)
        {
            yield return new WaitForSeconds(1.5f);
            if (listbox != null)
            {
                listbox.MoveNext(true);
            }

        }
        void HandleListBoxNode(VSlamHL.Listbox3d listbox, X3DParse.Node listBoxNode, int no ,ref GameObject prefab)
        {
            VSlamHL.ListboxItem3d listboxItem3d = new VSlamHL.ListboxItem3d();

            var spec = ClickableTextSpec.FromX3dParseNode(listBoxNode);
 
            bool hasParentTransform=false;
            foreach (var prefchild in listBoxNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = prefchild.Name.ToLower();
                switch (name)
                {
                    case "nodeprefab":
                        prefab = HandleNodePrefab(prefab, prefchild, out hasParentTransform);
                        break;
                }
            }

            prefab = AssureNodePrefab(prefab, 0.05f);
           var localPrefab = PrepareListBoxItem(prefab, spec, hasParentTransform, no);
           listbox.Items.Add(localPrefab);
        }
        GameObject PrepareListBoxItem(GameObject prefab, ClickableTextSpec spec, bool hasParentTransform, int no)
        {
            var localPrefab = Instantiate(prefab);
            localPrefab.SetActive(true);
            localPrefab.transform.SetTheParent(SceneGO.transform);
            var firstCollider = localPrefab.gameObject.GetComponentInChildren<Collider>();
            if (firstCollider != null)
            {
                var so = firstCollider.gameObject.GetComponent<SlamObject>();
                if (so == null)
                {
                    so=firstCollider.gameObject.AddComponent<SlamObject>();
                }
                so.Target = spec.Target;
                so.Href = spec.Href;
                so.ToolTip = spec.Tooltip;
                so.name = "lbItem" + no.ToString();

            }
            //text
            if (!string.IsNullOrEmpty(spec.Text))
            {
                var textMeshObj = localPrefab.GetComponentInChildren<TextMeshPro>();
                if (textMeshObj == null)
                {
                    textMeshObj = GetTextMeshObject(spec.Text, 10, 1, 15);
                    var textMeshParent = hasParentTransform && localPrefab.transform.childCount > 0 ? localPrefab.transform.GetChild(0) : localPrefab.transform;
                    textMeshObj.gameObject.transform.SetTheParent(textMeshParent);
                    textMeshObj.gameObject.transform.localPosition = new Vector3(7, 0, 0);
                    textMeshObj.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    textMeshObj.alignment = TextAlignmentOptions.Left;
                    textMeshObj.color = Color.black;

                }
                else
                {
                    textMeshObj.text = spec.Text;
                }
            }
            return localPrefab;
        }

        void HandleTreeView3d(Transform parent, X3DParse.Node treeViewNode)
        {
            if (assetBundleDownloads.Count > 0)
            {
                LateGameObjects.Add(new LateGameObject(treeViewNode, parent, LateGameObjectType.TreeView));
                return;
            }
            GameObject treeViewObject = new GameObject();
            treeViewObject.transform.SetTheParent(parent);
            treeViewObject.transform.localPosition = Vector3.zero;
            CheckNameDefProperties(treeViewNode, treeViewObject);
            int level = 0;
            int startlevel = 2;
            float scale = 1;
            string treename= "TreeView3D";
            Color lineStartColor = Color.blue;
            Color lineEndColor = Color.green;
            float lineStartWidth = 0.001f;
            float lineEndWidth = 0.003f;
            Vector3 drift = new Vector3(-10,10,0);
            float parentDistance = 1;
            float childrenDistance = 0.4f;
            float selectedSizeFactor = 1.4f;
            bool faceCamera = true;
            VSlamHL.TreeView3D tree = treeViewObject.AddComponent<VSlamHL.TreeView3D>();
            foreach (var child in treeViewNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "name":
                        treename = child.Value;
                        break;
                    case "startlevel":
                        int.TryParse(child.Value, out startlevel);
                        break;
                    case "linestartcolor":
                        TryColorFromString(child.Value, out lineStartColor);
                        break;
                    case "lineendcolor":
                        TryColorFromString(child.Value, out lineEndColor);
                        break;
                    case "linestartwidth":
                        float.TryParse(child.Value, out lineStartWidth);
                        break;
                    case "lineendwidth":
                        float.TryParse(child.Value, out lineEndWidth);
                        break;
                    case "drift":
                        TryVector3FromString(child.Value, out drift);
                        break;
                    case "parentdistance":
                        float.TryParse(child.Value, out parentDistance);
                        break;
                    case "childrendistance":
                        float.TryParse(child.Value, out childrenDistance);
                        break;
                    case "selectedsizefactor":
                        float.TryParse(child.Value, out selectedSizeFactor);
                        break;
                    case "facecamera":
                        faceCamera = Statics.CheckBoolString(child.Value);
                        break;
                }
            }
            tree.LineStartColor = lineStartColor;
            tree.LineEndColor = lineEndColor;
            tree.LineStartWidth = lineStartWidth;
            tree.LineEndWidth = lineEndWidth;
            tree.drift = drift;
            tree.parentDistance = parentDistance;
            tree.childrenDistance = childrenDistance;
            tree.selectedSizeFactor = selectedSizeFactor;

            treeViewObject.name = treename;
            bool hasParentTransform = false;
            GameObject prefab = null;
            int no = 0;
            HandleTreeViewChildNodes(treeViewNode, tree, null, level, startlevel, scale, ref no ,ref prefab, hasParentTransform, faceCamera);

            if(prefab!=null)
            {
                Destroy(prefab);
            }
       }
        void HandleTreeViewChildNodes(X3DParse.Node parentNode, VSlamHL.TreeView3D tree, VSlamHL.TreeView3DItem parentItem, int level, int startlevel, float scale,ref int no, ref GameObject prefab,bool hasParentTransform=false, bool faceCamera=true)
        {
            string text = null;
            string href = null;
            Target target = Target._blank;
            foreach (var child in parentNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "text":
                        text = child.Value;
                        break;
                    case "scale":
                        float.TryParse(child.Value, out scale);
                        break;
                    case "slm:href":
                    case "href":
                        href=child.Value;
                        break;
                    case "slm:target":
                    case "target":
                        Statics.TryGetTarget(child.Value, out target);
                        break;
                }
            }
            foreach (var child in parentNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "nodeprefab":
                        prefab= HandleNodePrefab(prefab, child, out hasParentTransform);
                        break;
                }
            }
 
            prefab = AssureNodePrefab(prefab);
            var deltaPos = level == 0 ? Vector3.zero : 0.1f * UnityEngine.Random.onUnitSphere;
            VSlamHL.TreeView3DItem node = CreateNodeObject(tree, parentItem, transform.position + deltaPos, text, scale, level, startlevel, ref no, ref prefab, href, target, hasParentTransform, faceCamera);
          
            level++;
            foreach (var child in parentNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "node":
                        HandleTreeViewChildNodes(child, tree, node, level, startlevel, scale, ref no, ref prefab, hasParentTransform, faceCamera);
                        break;
                }
            }
        }

        GameObject HandleNodePrefab(GameObject prefab, X3DParse.Node prefabNode, out bool hasParentTransform)
        {
            Appearance appearance = null;
            GameObject t = null;
            hasParentTransform = false;
            foreach (var child in prefabNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "appearance":
                        appearance = HandleAppearanceNode(child);
                        break;
                    case "transform":
                        t = HandleTransFormNode(child, null);
                        hasParentTransform = true;
                        break;
                }
            }
            if(t!=null)
            {
                if(prefab!=null)
                {
                    Destroy(prefab);
                }
                prefab = t;
            }
            else if(appearance!=null)
            {
                prefab = AssureNodePrefab(prefab);
                UnityEngine.Material mat = DefineMaterial(appearance.Material);
                AssignMaterialToGameObject(prefab, mat);
            }
            if(prefab.transform.parent==null)
            {
                prefab.transform.SetParent(SceneGO.transform);
            }
            return prefab;
        }
        GameObject AssureNavigationButtonPrefab(VSlamHL.Listbox3d listbox, GameObject prefab, bool isUp, Vector3 navigationButtonScale, out bool buttonHasPrefab)
        {
            buttonHasPrefab = true;
            if (prefab == null)
            {
                prefab = GetPrefab(null, Constants.primitivesGroup, Constants.arrowPrefabName);
                prefab.transform.localScale = 7f * navigationButtonScale;
                var rotation=isUp?new Vector3(0,-90f,90f): new Vector3(0, -90f, 270f);
                prefab.transform.localRotation=Quaternion.Euler(rotation);
                buttonHasPrefab = false;
            }
            var coll = prefab.GetComponentInChildren<Collider>();
            if(coll!=null)
            {
                var l3dnb=coll.gameObject.AddComponent<List3DNavigationButton>();
                l3dnb.reset = isUp;
                l3dnb.listbox = listbox;
            }
            return prefab;

        }
        GameObject AssureNodePrefab(GameObject prefab, float localScale=0.2f)
        {
            if (prefab == null)
            {
                prefab = GetPrefab(null, Constants.primitivesGroup, Constants.spherePrefabName);
                prefab.transform.localScale = localScale * Vector3.one;
                prefab.SetActive(false);
            }
            return prefab;

        }
        VSlamHL.TreeView3DItem CreateNodeObject(VSlamHL.TreeView3D treeView3d, VSlamHL.TreeView3DItem parent, Vector3 pos, string aText,  float scale, int level, int startTreeLevel,ref int no, ref GameObject treeView3dPrefabGO, string href=null, Target target= Target._blank, bool hasParentTransform=false, bool faceCamera=true)
        {

            var x = Instantiate(treeView3dPrefabGO.transform, pos, treeView3dPrefabGO.transform.rotation);
            x.gameObject.SetActive(true);
           VSlamHL.TreeView3DItem item = treeView3d.CreateItem(parent, x.gameObject, hasParentTransform);
            item.transform.localPosition = pos;
            item.SetDefaultScale( scale * item.transform.localScale);
           // treeNodeObjects.Add(x.gameObject);
            item.level = level;
            item.FaceCamera = faceCamera;
            var firstCollider = x.gameObject.GetComponentInChildren<Collider>();
            item.setVisible(level < startTreeLevel);
            if (firstCollider != null)
            {
                var so = firstCollider.gameObject.GetComponent<SlamObject>();
                if(so!=null)
                {
                    Destroy(firstCollider.gameObject.GetComponent<SlamObject>());
                }
                var c3dNO = firstCollider.gameObject.AddComponent<Control3DNodeObject>();
                c3dNO.TreeView3D = treeView3d;
                if (!string.IsNullOrEmpty(href))
                {
                    c3dNO.Href = href;
                    c3dNO.Target = target;
                }
                no++;
                firstCollider.gameObject.name = "tvItem" + no.ToString();
            }
            if (!string.IsNullOrEmpty(aText))
            {
                var textMeshObj = x.GetComponentInChildren<TextMeshPro>();
                if (textMeshObj == null)
                {
                    textMeshObj = GetTextMeshObject(aText, 50, 1, 5);
                    var textMeshParent = hasParentTransform && x.childCount > 0 ? x.GetChild(0) : x;
                    textMeshObj.gameObject.transform.SetTheParent(textMeshParent);
                    textMeshObj.gameObject.transform.localPosition = new Vector3(-26, 0, 0);
                }
                else
                {
                    textMeshObj.text = aText;
                }
            }
   
             return item;
        }
        void HandleSpeak(Transform parent, X3DParse.Node shapeNode)
        {
            string text = null;
            bool interrupt = true;
            string voice = null;
            float distance = 0;
            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "text":
                        text = child.Value;
                        break;
                    case "voice":
                        voice = child.Value;
                        break;
                    case "interrupt":
                        interrupt = Statics.CheckBoolString(child.Value);
                        break;
                    case "distance":
                        float.TryParse(child.Value, out distance);
                        break;
                }
            }
            if(!string.IsNullOrEmpty(text))
            {
                if (distance > 0)
                {
                    PlayerActivated pa = new PlayerActivated(parent, PlayerActivatedType.Speak, distance);
                    PlayerActivatedVoice pav = new PlayerActivatedVoice();
                    pav.Voice = voice;
                    pav.Text = text;
                    pav.Interrupt = interrupt;
                    pa.PlayerAtivatedArgs = pav;
                    playerActivatedObjects.Add(pa);
                }
                else
                {
                    TrySpeakText(parent, text, voice, !interrupt);
                }
            }
        }
        void ResetFog()
        {
            RenderSettings.fog = false;
            RenderSettings.fogColor = Color.gray;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.02f;
        }
        void SetFog()
        {
            FogSettings fs = CurrentFogSettings;
            if (fs == null)
            {
                fs = new FogSettings();
            }
            RenderSettings.fog = fs.Active;
            RenderSettings.fogColor = fs.Color;
            RenderSettings.fogDensity = fs.Density;
            if (fs.VisibilityRange != null)
            {
                RenderSettings.fogStartDistance = (float)fs.VisibilityRange;
                RenderSettings.fogEndDistance = (float)fs.VisibilityRange + 20f;
            }
            switch(fs.FogType)
            {
                case FogTypeEnum.linear:
                    RenderSettings.fogMode = FogMode.Linear;
                    break;
                case FogTypeEnum.exponential:
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;
                case FogTypeEnum.exponentialsquared:
                    RenderSettings.fogMode = FogMode.ExponentialSquared;
                    break;
            }
        }

        void HandleNavigationInfo(X3DParse.Node shapeNode)
        {
            string navType=null;
            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "type":
                        navType = child.Value;
                        break;
                }
            }
            if(navType=="slm:noscroll")
            {
                AllowScroll = false;
            }
        }
        void HandleFog(X3DParse.Node shapeNode)
        {
            string density = null;
            string fogType = null;
            string color = null;
            string visibilityRange = null;

            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "slm:density":
                        density = child.Value;
                        break;
                    case "fogType":
                        fogType = child.Value.ToLower();
                        break;
                    case "color":
                        color = child.Value;
                        break;
                    case "visibilityrange":
                        visibilityRange = child.Value;
                        break;

                }

            }
            CurrentFogSettings = new FogSettings();
            CurrentFogSettings.Active = true;
            Color c;
            if (color != null && TryColorFromString(color, out c))
            {
                CurrentFogSettings.Color = c;
            }
            float d;
            if (density!=null && float.TryParse(density, out d))
            {
                CurrentFogSettings.Density = d;
            }
            if(fogType=="linear")
            {
                CurrentFogSettings.FogType = FogTypeEnum.linear;
            }
            else if (fogType == "exponential")
            {
                CurrentFogSettings.FogType = FogTypeEnum.exponential;
            }
            else if (fogType == "exponentialsquared")
            {
                CurrentFogSettings.FogType = FogTypeEnum.exponentialsquared;
            }
            float vr;
            if(float.TryParse(visibilityRange, out vr))
            {
                CurrentFogSettings.VisibilityRange = vr;
            }
        }
        void HandleBackGround(X3DParse.Node shapeNode)
        {
            string skybox = null;
            string source = null;
            string color = null;
            string reflectionMode = null;
            
            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "skybox":
                        skybox = child.Value;
                        break;
                    case "slm:lightingsource":
                        source = child.Value.ToLower();
                        break;
                    case "slm:lightingcolor":
                        color = child.Value;
                        break;
                    case "slm:reflectionmode":
                        reflectionMode = child.Value.ToLower();
                        break;

                }

            }
            if (!string.IsNullOrEmpty(skybox))
            {
                //               var box =Camera.main.GetComponent<Skybox>();
                UnityEngine.Material sbMat = Resources.Load<UnityEngine.Material>("sky5X/sky5X_skyboxes/" + skybox);
                if (sbMat != null)
                {
                    var sb = Camera.main.GetComponent<Skybox>();
                    if (sb != null)
                    {
                        sb.material = sbMat;
                        DynamicGI.UpdateEnvironment();
                    }
                }
            }
            if (!string.IsNullOrEmpty(source))
            {
                switch (source)
                {
                    case "skybox":
                        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                        break;
                    case "depth":
                        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                        Camera.main.clearFlags = CameraClearFlags.Depth;
                        break;
                    case "color":
                        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                        Camera.main.clearFlags = CameraClearFlags.Color;
                        break;
                }
            }
            if (!string.IsNullOrEmpty(color))
            {
                Color c = Color.white;
                if(TryColorFromString(color, out c))
                {
                    RenderSettings.ambientLight = c;
                }
            }
            if (!string.IsNullOrEmpty(reflectionMode))
            {
                switch (reflectionMode)
                {
                    case "skybox":
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;// = c;
                        break;
                    case "custom":
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;// = c;
                        break;
                }
            }
        }
        GameObject HandleModel(X3DParse.Node shapeNode, Transform parent)
        {
            string href = null;
            string prefabName = null;
            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "name":
                        prefabName = child.Value;
                        break;
                    case "href":
                    case "slm:href":
                        href = child.Value;
                        break;
                }
            }
            if(!string.IsNullOrEmpty(href) && parent!=null) 
            {
                var thref = UrlHandler.AssureFullUrl(href);
             //   GLTFComponent gltfScript = parent.gameObject.AddComponent<GLTFComponent>();
      //          UnityEngine.Material GLTFMaterial
                //gltfScript.Url = thref;
                //gltfScript.UseStream = false;
                //gltfScript.MaximumLod = 1000000000;
      //          gltfScript.GLTFConstant = gltfScript.GLTFStandard = gltfScript.GLTFStandardSpecular = GLTFMaterial.shader;
                StartCoroutine( LoadGLTF(thref,parent));
            }
            return transform.gameObject;
        }
        GameObject HandlePrefab(X3DParse.Node shapeNode, Transform parent , ref bool hasPrefab)
        {
            string goupName=null;
            string prefabName = null;
            string prefabItem = null;
            string bundleName = null;
            foreach (var child in shapeNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch(name)
                {
                    case "name":
                        prefabName = child.Value;
                        break;
                    case "item":
                        prefabItem = child.Value.ToLower();
                        break;
                    case "group":
                        goupName = child.Value.ToLower();
                        break;
                    case "bundle":
                        bundleName = child.Value.ToLower();
                        break;
                }
            }
            GameObject go = null;
            try
            {
                string prefab = string.IsNullOrEmpty(prefabItem) ? prefabName : prefabItem;
                go = GetPrefab(parent, goupName, prefab, ref hasPrefab, bundleName);
                go=CheckAvatarPrefab(go, prefab);

            }
            catch (Exception x)
            {
                Debug.LogWarning(string.Format("Prefab {0} could not be found in folder {1}", prefabName, goupName));
            }
            return go;
        }
        public GameObject GetHandPrefab(Transform parent, bool leftH=false)
        {
//            string h = leftH ? "handl" : "handr";
            string h = leftH ? "controllerL" : "controllerR";
            return GetPrefab(parent, "avatars", h);
        }
        public GameObject GetPrefab(Transform parent, string goupName, string prefabName)
        {
            bool hasBundle = true;
            return GetPrefab(parent, goupName, prefabName, ref hasBundle);
        }
        public GameObject GetPrefab(Transform parent, string goupName, string prefabName, ref bool hasBundle, string bundleName=null)
        {
            if (!string.IsNullOrEmpty(prefabName) && (!string.IsNullOrEmpty(goupName)|| !string.IsNullOrEmpty(bundleName)))
            {
                GameObject go = null;
                if (bundleName == null)
                {
                    go = Instantiate(Resources.Load<GameObject>("_pf/" + goupName + "/" + prefabName));
                }
                else if(assetBundles.ContainsKey(bundleName))
                {
                    var bundle = assetBundles[bundleName];
                    if (bundle != null)
                    {
                        var tgo = bundle.LoadAsset<GameObject>(prefabName);
                        go = Instantiate(tgo);
                    }
                }
                if (go != null)
                {
                    hasBundle = true;
                    go.transform.SetTheParent(parent);
                    go.SetActive(true);
                }
                return go;
            }
            return null;
        }
        GameObject CheckAvatarPrefab(GameObject go, string name)
        {
            if(go!=null  && name.StartsWith("avatar"))
            {
                GameObject avatarParent = new GameObject();
                 var av = PrepAvatarParent(avatarParent);
                var currenParent = go.transform.parent;
                go.transform.parent = null;
                if (currenParent != null)
                {
                    avatarParent.transform.SetTheParent(currenParent);
                }
                go.transform.SetTheParent(avatarParent.transform);
                CheckAvatarCorrection(go);
//                av.SwitchRandomAnimation(MoveState.idle);
                return  avatarParent;
            }
            return go;
        }
        void CheckAvatarCorrection(GameObject go, Avatarcorrection avatarcorrection=null)
        {

            var correction=new Vector3(0, 0,0);
            if (avatarcorrection == null)
            {
                avatarcorrection = go.GetComponent<Avatarcorrection>();
            }
            if(avatarcorrection != null)
            {
                correction = new Vector3(0, avatarcorrection.y_correction, 0);
            }
            go.transform.localPosition = correction;
        }
        bool PrepShapeNode(X3DParse.Node node, ref GameObject go, ref Transform parent, ref Appearance appearance, ref IndexedFaceSet indexedFaceSet,ref IndexedLineSet indexedLineSet, ref X3DParse.Node selectedGoChild, ref bool isText, ref InputType inputType)
        {
            string name = node.Name.ToLower();
            switch (name)
            {
                case "slm:targetplatform":
                    if (HandleTargetPlatform(node))
                    {
                        foreach (var child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
                        {
                            if(!PrepShapeNode(child, ref go, ref parent, ref appearance, ref indexedFaceSet,ref indexedLineSet, ref selectedGoChild, ref isText, ref inputType))
                            {
                                return false;
                            }
                        }
                    }
                    break;
                case "appearance":
                    appearance = HandleAppearanceNode(node);
                    break;
                case "indexedfaceset":
                case "indexedtriangleset":
                    indexedFaceSet = HandleindexedFaceSet(node);
                    break;
                case "indexedlineset":
                    indexedLineSet = HandleIndexedLineSet(node);
                    break;
                case "sphere":
                   // go = CreatePrimitiveNode(parent, PrimitiveType.Sphere);
                    go= GetPrefab(parent, Constants.primitivesGroup, Constants.spherePrefabName);
                    break;
                case "cube":
                case "box":
                    go = CreatePrimitiveNode(parent, PrimitiveType.Cube);
                    break;
                case "plane":
                    go = CreatePrimitiveNode(parent, PrimitiveType.Plane);
                    break;
                case "capsule":
                    go = CreatePrimitiveNode(parent, PrimitiveType.Capsule);
                    break;
                case "cylinder":
                    //                    go = CreatePrimitiveNode(parent, PrimitiveType.Cylinder);
                   go = GetPrefab(parent, Constants.primitivesGroup, Constants.cylinderPrefabName);
                    break;
                case "quad":
                    go = CreatePrimitiveNode(parent, PrimitiveType.Quad);
                    break;
                case "cone":
                    go = GetPrefab(parent, Constants.primitivesGroup, Constants.conePrefabName);
                    break;
                case "text":
                    go = HandleTextNode(node, parent, ref inputType);
                    isText = true;
                    break;
                case "slm:avatar":
                    go = HandleAvatar(node, parent);
                    break;
                case "slm:prefab":
                    bool hasPrefab = false;
                    go = HandlePrefab(node, parent, ref hasPrefab);
                    if(!hasPrefab)
                    {
                        return false;
                    }
                    break;
                case "slm:model":
                    go = HandleModel(node, parent);
                    break;
                case "slm:empty":
                    go = new GameObject();
                    go.transform.SetTheParent(parent);
                    break;
            }

            if (go != null && selectedGoChild == null)
            {
                selectedGoChild = node;
            }
            return true;
        }
        List<HandDraggablePrep> HandDraggablePrepList = new List<HandDraggablePrep>();
        void CheckHandDraggablePreps()
        {
            foreach(var h in HandDraggablePrepList)
            {
                MakeHandDraggable(h.Parent, h.GameObject, h.IsHandDragable, h.IsHandRotatable);
            }
            HandDraggablePrepList.Clear();
        }
        void MakeHandDraggable(Transform parent, GameObject go, bool isHandDragable, bool isHandRotatable)
        {
            var got = go;
            var so = go.GetComponentInChildren<SlamObject>();
            if (so != null)
            {
                got = so.ActiveTransform.gameObject;
            }
            if(parent!=null)
            {
                got = parent.gameObject;
            }
            Handdraggable hd=null;
            if (IsHoloLens())
            {
                var hhd=got.AddComponent<HoloToolkit.Unity.InputModule.Utilities.Interactions.TwoHandManipulatable>();
                hhd.ManipulationMode = HoloToolkit.Unity.InputModule.Utilities.Interactions.ManipulationMode.MoveAndRotate;
            }
            else
            {
                hd = got.AddComponent<Handdraggable>();
                hd.IsHandDragable = isHandDragable;
                hd.IsHandRotatable = isHandRotatable;
            }
            foreach (var c in got.GetComponentsInChildren<Collider>())
            {
                if (c.gameObject != got)
                {
                }
            }
            var coll = got.GetComponent<Collider>();
            if (coll == null || coll is MeshCollider)
            {
                if (coll is MeshCollider)
                {
                   // coll.enabled = false;
                }
            }
            if (coll == null || coll.enabled == false)
            {
                var bcoll = got.AddComponent<BoxCollider>();
            }
            //var renderer = got.gameObject.GetComponentInChildren<Renderer>();
            //if(renderer!=null)
            //{
            //    Bounds bigBounds = renderer.bounds;
            //    foreach (var ch in GetComponentsInChildren<Renderer>())
            //    {
            //        bigBounds.Encapsulate(ch.bounds);
            //    }
            //        bcoll.size = bigBounds.size;
            //    bcoll.center = bigBounds.center;
            //}
            var r = got.AddComponent<Rigidbody>();
            if (r != null)
            {
                // r.freezeRotation = true;
                r.isKinematic = true;
                r.useGravity = false;
                r.mass = 0;
                r.drag = 100;
            }
            // hd.ManipulationMode = HoloToolkit.Unity.InputModule.Utilities.Interactions.MRTwoHandManipulatable.TwoHandedManipulation.MoveRotateScale;
            //var bbpref = Instantiate(Resources.Load<HoloToolkit.Unity.UX.BoundingBox>("Prefabs/BoundingBoxBasic"));
            //hd.BoundingBoxPrefab = bbpref;
            //bbpref.transform.parent = go.transform;
            //hd.HostTransform = go.transform;
            if (parent != null && hd!=null)
            {
                //if (applyMovementToParent && parent.parent != null)
                //{
                //    hd.HostTransform = parent.parent;
                //}
                //else
                //{
                    hd.HostTransform = parent;
                //}
            }
            
        }
        List<LateGameObject> LateGameObjects = new List<LateGameObject>();

        void HandleLateObjects()
        {
            foreach( var lateGameObject in LateGameObjects)
            {
                switch (lateGameObject.LateGameObjectType)
                {
                    case LateGameObjectType.ShapeNode:
                        HandleShapeNode(lateGameObject.ShapeNode, lateGameObject.Parent);
                        break;
                    case LateGameObjectType.TreeView:
                        HandleTreeView3d(lateGameObject.Parent, lateGameObject.ShapeNode);
                        break;
                    case LateGameObjectType.ListView:
                        HandleListView3d(lateGameObject.Parent, lateGameObject.ShapeNode);
                        break;
                }
            }
            LateGameObjects.Clear();
        }
        void HandleShapeNode(X3DParse.Node shapeNode, Transform parent)
        {
            GameObject go = null;
            Appearance appearance = null;
            IndexedFaceSet indexedFaceSet = null;
            IndexedLineSet indexedLineSet = null;
            bool isText = false;
            InputType inputType = InputType.None;
            bool assignScript = true;
            X3DParse.Node selectedGoChild = null;
            foreach (var child in shapeNode.Children.FindAll(x=>x.NodeType== X3DParse.NodeType.Element))
            {
                if(!PrepShapeNode(child,ref go, ref parent, ref appearance, ref indexedFaceSet,ref indexedLineSet, ref selectedGoChild, ref isText, ref inputType))
                {
                    LateGameObjects.Add(new LateGameObject(shapeNode, parent));
                }
            }
            if (indexedFaceSet != null)//point object
            {
                go = InitiateVertextShape(parent);
                CheckNameDefProperties(shapeNode, go);
            }
            if(indexedLineSet!=null)
            {
                go = InitiateVertextShape(parent,false);
                CheckNameDefProperties(shapeNode, go);
            }
            //from her 'go' should not be null
            if (go==null)
            {
                return;
            }
            UnityEngine.Material mat = null;
            if (appearance != null && appearance.Material!=null)
            {
                if(string.IsNullOrEmpty(appearance.Material.Name))
                {
                    appearance.Material.Name = System.Guid.NewGuid().ToString();
                }
                if (!string.IsNullOrEmpty(appearance.Material.Name))
                {
                    if (appearance.Material.Define && matDict.ContainsKey(appearance.Material.Name))
                    {
                        appearance.Material.Define = false;
                        //Debug.LogWarning(string.Format("Material {0} is defined multiple times", appearance.Material.Name));
                    }
                    else if (!appearance.Material.Define && !matDict.ContainsKey(appearance.Material.Name))
                    {
                        appearance.Material.Define = true;
                        //Debug.LogWarning(string.Format("Material {0} is used before defining it", appearance.Material.Name));
                    }
                    if (appearance.Material.Define)
                    {
                        mat = DefineMaterial(appearance.Material);                      
                    }
                    else
                    {
                        matDict.TryGetValue(appearance.Material.Name, out mat);
                    }
                    AssignMaterialToGameObject(go, mat, isText);

                }
                if (appearance.ImageTexture != null && !string.IsNullOrEmpty(appearance.ImageTexture.Url))
                {
                    if (appearance.Movement != null && appearance.Movement.Updatable == true)
                    {
                        AddUpdatableTexture(go, appearance.ImageTexture.Url);
                    }
                    else
                    {
                        AddTexture(go, appearance.ImageTexture.Url);
                    }
                }
                else if (appearance.MovieTexture != null && !string.IsNullOrEmpty(appearance.MovieTexture.Url))
                {
                    StartSceneCoroutine( PlayMovieTexture(go, appearance.MovieTexture.Url, appearance.MovieTexture.Loop));
                }
                if (appearance.Material != null && !string.IsNullOrEmpty(appearance.Material.Physics))
                {
                    var coll = go.GetComponent<Collider>();
                    if (go != null)
                    {
                        coll.material = Resources.Load<PhysicMaterial>("PhysicMaterials/" + appearance.Material.Physics);
                    }
                }

            }
            ShapeProperties shapeProperties = new ShapeProperties();
            CheckUSE<ShapeProperties>(selectedGoChild, ref shapeProperties);
            string goName = null;
            string formfield = null;
            string formvalue = null;
            float presentation = 0;
            float presentationangle = 0;
            float presentationspeed = 0;
            float kinetic = 0;

            if (selectedGoChild != null)
            {
                foreach (var child in selectedGoChild.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "name":
                        case "def":
                            goName = child.Value;
                            break;
                        case "slm:href":
                            shapeProperties.Href = child.Value;
                            assignScript = true;
                            break;
                        case "slm:target":
                            Target t = Target._blank;
                            if (Statics.TryGetTarget(child.Value, out t))
                            {
                                shapeProperties.Target = t ;
                            }
                            break;
                        case "slm:facecamera":
                            shapeProperties.FaceCamera = child.Value.ToLower();
                            break;
                        case "slm:walkfloor":
                            int w = 0;
                            if(int.TryParse(child.Value, out w))
                            {
                                shapeProperties.WalkFloor = w;
                            }
                            break;
                        case "slm:sitplane":
                            int s = 0;
                            if (int.TryParse(child.Value, out s))
                            {
                                shapeProperties.SitPlane = s;
                            }
                            break;
                        case "slm:tooltip":
                            shapeProperties.ToolTip = Statics.X3dTextToMultiline( child.Value);
                            break;
                        case "slm:favorite":
                            int favorite = 0;
                            if(int.TryParse(child.Value, out favorite))
                            {
                                shapeProperties.Favorite = favorite;
                            }
                            break;
                        case "slm:history":
                            int history = 0;
                            if (int.TryParse(child.Value, out history))
                            {
                                shapeProperties.History = history;
                            }
                            break;
                        case "radius":
                            float radius = 0;
                            if (float.TryParse(child.Value, out radius))
                            {
                                shapeProperties.Radius = radius;
                            }
                            break;
                        case "bottomradius":
                            float bottomradius = 0;
                            if (float.TryParse(child.Value, out bottomradius))
                            {
                                shapeProperties.BottomRadius = bottomradius;
                            }
                            break;
                        case "height":
                            float height = 0;
                            if (float.TryParse(child.Value, out height))
                            {
                                shapeProperties.Height = height;
                            }
                            break;
                        case "slm:formfield":
                            formfield= child.Value;
                            break;
                        case "slm:formvalue":
                            formvalue = child.Value;
                            break;
                        case "slm:hidden":
                            string v=child.Value.ToLower();
                            shapeProperties.Hidden = v == "true" || v == "yes";
                            break;
                    }
                }
            }
            CheckFavorite(parent, go, ref shapeProperties, isText, ref assignScript);
            CheckHistory(parent, go, ref shapeProperties, isText, ref assignScript);
            if (!string.IsNullOrEmpty(goName))
            {
                go.name = goName;
            }
            Vector3 rotate = Vector3.zero;
            Vector3 center = Vector3.zero;
            Vector3 clingToCamera = Vector3.zero;
            bool applyMovementToParent = false;
            bool handDraggable = false;
            bool handRotatable = false;
            bool serverObject = false;
            string activeTransform = null;
            List<Constraint> constraints = null;
            if (appearance != null)
            {
                if (appearance.Movement != null)
                {
                    var movement = appearance.Movement;
                    activeTransform = movement.ActiveTransform;
                    if (!string.IsNullOrEmpty(movement.Rotation) && TryVector3FromString(movement.Rotation, out rotate))
                    {
                        assignScript = true;
                        if (!string.IsNullOrEmpty(movement.Center) && TryVector3FromString(movement.Center, out center))
                        {

                        }
                    }
                    if (!string.IsNullOrEmpty(movement.ApplyToParent) && "true;yes;1".Contains(movement.ApplyToParent.Trim().ToLower()))
                    {
                        applyMovementToParent = true;
                    }
                    if (!string.IsNullOrEmpty(movement.ClingToCamera) && TryVector3FromString(movement.ClingToCamera, out clingToCamera))
                    {
                        applyMovementToParent = true;
                        assignScript = true;
                    }
                    if (movement.Presentation>0)
                    {
                        assignScript = true;
                        presentation = movement.Presentation;
                        presentationangle = movement.PresentationAngle;
                        presentationspeed = movement.PresentationSpeed;
                    }
                    //kinetic
                    if (movement.Kinetic > 0 && movement.ColliderType == ColliderType.NotSet)
                    {
                        movement.ColliderType = ColliderType.Sphere;
                    }
                    UnityEngine.Collider coll = null;
                    switch (movement.ColliderType)
                    {
                        case ColliderType.Sphere:
                            coll = go.AddComponent<SphereCollider>();
                            break;
                        case ColliderType.Box:
                            coll = go.AddComponent<BoxCollider>();
                            break;
                        case ColliderType.Capsule:
                            coll = go.AddComponent<CapsuleCollider>();
                            break;
                        case ColliderType.None:
                            disableAllColliders(go);
                            break;
                    }
                    if (movement.Kinetic>0)
                    {
                        assignScript = true;
                        kinetic = movement.Kinetic;
 
                        if (movement.Bounciness > 0)
                        {
                            coll.material.bounciness = movement.Bounciness;
                        }
                        //coll.material.bounceCombine = PhysicMaterialCombine.Maximum;
                         var rb=go.AddComponent<Rigidbody>();
                        rb.drag = 0.3f;
                        rb.angularDrag = 0.3f;
                        var mc = go.GetComponent<MeshCollider>();
                        if(mc!=null)
                        {
                            mc.enabled = false;
                        }
                    }
                    if (movement.HandDraggable == true)
                    {
                        handDraggable = true;
                    }
                    if (movement.HandRotatable == true)
                    {
                        handRotatable = true;
                    }
                    if (movement.ServerObject==true)
                    {
                        serverObject = true;
                    }
                    if(movement.Constraints.Count>0)
                    {
                        constraints = movement.Constraints;
                        if(applyMovementToParent)
                        {
                            var p = go.transform.parent;
                            if(parent!=null)
                            {
                                parent.gameObject.AddComponent<SphereCollider>();
                                var rb= parent.gameObject.AddComponent<Rigidbody>();
                                rb.useGravity = false;
                                rb.drag = 5;
                                rb.angularDrag = 5;
                            }
                        }
                    }
                }
            }
            if(inputType!= InputType.None)
            {
                assignScript = true;
            }
            if(!string.IsNullOrEmpty(formfield))
            {
                if (formfield == "slam_session" && !string.IsNullOrEmpty(formvalue))
                {
                    slam_session = formvalue;
                }
                if (formfield == Constants.slam_presentation_guid && !string.IsNullOrEmpty(formvalue))
                {
                    DeviceManager.PresentationGuid = formvalue;
                }
                
                else if (formfield == Constants.returnPageCodefld && !string.IsNullOrEmpty(formvalue))
                {
                    returnPage = formvalue;
                }
                else
                {
                    if(string.IsNullOrEmpty(formvalue))
                    {
                        var item = DeviceManager.FindStoredFieldItem(UrlHandler.FullSceneUrl, formfield);
                        if(item!=null)
                        {
                            var val = item.FieldValue ;
#if !UNITY_WSA //&& !UNITY_EDITOR
                            val = AESCrypt.DecryptData(item.FieldValue, Constants.Slamkey) ;
#endif
                            formvalue = val;
                        }
                    }
                    assignScript = true;
                }
            }
            if((handDraggable ||handRotatable) && DeviceManager.IsUWP)
            {
                Transform parentToUse = null;
                if (parent != null)
                {
                    if (applyMovementToParent && parent != null)
                    {
                        parentToUse = parent;
                    }
                }
                HandDraggablePrepList.Add(new HandDraggablePrep(parentToUse, go, handDraggable, handRotatable));
            }
            if (assignScript)
            {
                SlamObject so = null;
                if (applyMovementToParent && go.transform.parent != null)
                {
                    so = go.transform.parent.gameObject.AddComponent<SlamObject>();
                }
                else
                {
                    so = go.AddComponent<SlamObject>();
                }
                so.Href = shapeProperties.Href;
                so.Target = shapeProperties.Target;
                so.ActiveTransformName = activeTransform;
                so.isHandDragable = handDraggable; //for operating mouse
                if(shapeProperties.Favorite!=null|| shapeProperties.History != null)
                {
                    so.ToolTip = so.Href;
                }
                if (rotate != Vector3.zero)
                {
                    so.Rotate = rotate;
                }
                if (center != Vector3.zero)
                {
                    so.Center = center - viewpointPosition;
                }
                if (clingToCamera != Vector3.zero)
                {
                    so.ClingToCamera = clingToCamera;
                }
                if (applyMovementToParent)
                {
                   // so.ApplyMovementToParent = true;
                }
                if (shapeProperties.WalkFloor != null)
                {
                    so.WalkFloor = true;
                    so.floorLevel = (float)shapeProperties.WalkFloor;
                }
                if (shapeProperties.SitPlane != null)
                {
                    so.SitPlane = true;
                   // so.floorLevel = (float)shapeProperties.SitPlane;
                }
                if (shapeProperties.ToolTip!=null)
                {
                    so.ToolTip = shapeProperties.ToolTip;
                }
                so.FaceCamera = shapeProperties.FaceCamera;
                so.presentation = presentation * DeviceManager.PresentationDistanceCorrection;
                so.presentationangle = presentationangle;
                so.presentationspeed = presentationspeed;
                so.kinetic = kinetic;
                so.InputType = inputType;
                if (!string.IsNullOrEmpty(formfield))
                {
                    so.FormFieldName = formfield;
                    so.FormFieldValue = formvalue;
                    if (!string.IsNullOrEmpty(formvalue))
                    {
                        var t = so.gameObject.GetComponent<TextMeshPro>();
                        if (t != null)
                        {
                            if (so.InputType == InputType.Password)
                            {
                                t.text = "*****";
                            }
                            else
                            {
                                t.text = formvalue;
                            }
                        }
                    }
                    formFields.Add(go);
                }
                if(constraints!=null)
                {
                    so.Constraints = constraints;
                }
                //assure that gameobject that needs a collider has one
                if(!string.IsNullOrEmpty( so.Href) || so.presentation>0)
                {
                    var coll = so.gameObject.GetComponent<Collider>();
                    if(coll==null)
                    {
                        so.gameObject.AddComponent<BoxCollider>();
                    }
                }
            }

            if (indexedFaceSet != null)
            {
                HandleVertexShapeNode(go, indexedFaceSet);
            }
            if (indexedLineSet != null)
            {
                DrawLines(go, indexedLineSet, appearance);
            }
            if(shapeProperties.Hidden)
            {
                go.SetActive(false);
            }
            if(shapeProperties.Radius!=null)
            {
                go.transform.localScale=(new Vector3((float)shapeProperties.Radius, go.transform.localScale.y, (float)shapeProperties.Radius));
            }
            if (shapeProperties.BottomRadius != null)
            {
                go.transform.localScale = (new Vector3((float)shapeProperties.BottomRadius, go.transform.localScale.y, (float)shapeProperties.BottomRadius));
            }
            if (shapeProperties.Height != null && !isText)
            {
                go.transform.SetGlobalScale(go.transform.localScale = new Vector3(go.transform.localScale.x, (float)shapeProperties.Height, go.transform.localScale.z));
            }

            CheckDEF<ShapeProperties>(selectedGoChild, ref shapeProperties);
            if (serverObject && parent != null)
            {
                var sgo = (applyMovementToParent && parent.parent != null) ? parent.parent.gameObject : parent.gameObject;
                if(kinetic>0 && parent.GetChild(0)!=null)
                {
                    sgo = parent.GetChild(0).gameObject;
                }
                if (newserverGameObjects.Find(x => x.name == sgo.name) == null)
                {
                    newserverGameObjects.Add(sgo);
                }
            }

            return;

        }
        void disableAllColliders(GameObject go)
        {
            if (go != null)
            {
                foreach (var collider in go.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }
        void CheckFavorite(Transform parent, GameObject go, ref ShapeProperties shapeProperties,bool isText, ref bool assignScript)
        {
            if (shapeProperties.Favorite != null && shapeProperties.Favorite >= 0)
            {
                if (shapeProperties.Favorite == 0 && DeviceManager.Favorites.Count == 0)
                {
                    ShowMessage(true, "You have no favorites selected yet");
                    Destroy(parent.gameObject);
                    return;
                }
                else if (shapeProperties.Favorite >= DeviceManager.Favorites.Count)
                {
                    Destroy(parent.gameObject);
                    return;
                }
                else
                {
                    shapeProperties.Target = Target._blank;
                    shapeProperties.Href = DeviceManager.Favorites[(int)shapeProperties.Favorite];

                    assignScript = true;
                    if (isText)
                    {
                        var m = go.GetComponent<TextMeshPro>();
                        m.text = UrlHandler.GetNamePart(shapeProperties.Href);

                    }
                    else
                    {
                        string faviconUrl = shapeProperties.Href.Substring(0, shapeProperties.Href.LastIndexOf("/")) + "/favicon.png";
                        AddTexture(go, faviconUrl);
                    }
                }
            }
        }
        void CheckHistory(Transform parent, GameObject go, ref ShapeProperties shapeProperties, bool isText, ref bool assignScript)
        {
            if (shapeProperties.History != null && shapeProperties.History >= 0)
            {
                var count = DeviceManager.Histories.Count;
                if (shapeProperties.History == 0 && count == 0)
                {
                    ShowMessage(true, "No history yet");
                    Destroy(parent.gameObject);
                    return;
                }
                else if (shapeProperties.History >= count)
                {
                    Destroy(parent.gameObject);
                    return;
                }
                else
                {
                    shapeProperties.Target = Target._blank;
                    shapeProperties.Href = DeviceManager.Histories[count-(int)shapeProperties.History-1];

                    assignScript = true;
                    if (isText)
                    {
                        var m = go.GetComponent<TextMeshPro>();
                        m.text = UrlHandler.GetNamePart(shapeProperties.Href);

                    }
                    else
                    {
                        string faviconUrl = shapeProperties.Href.Substring(0, shapeProperties.Href.LastIndexOf("/")) + "/favicon.png";
                        AddTexture(go, faviconUrl);
                    }
                }
            }
        }
        public void AssignMaterialToGameObject(GameObject go, Material mat, bool isText=false)
        {
            if (mat != null && go != null)
            {
                Renderer rend = go.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (isText)
                    {
                        var m = go.GetComponent<TextMeshPro>();
                        m.faceColor = mat.color;
                        m.color = mat.color;
                    }
                    else
                    {
                        rend.material = mat;
                        rend.UpdateGIMaterials();
                    }
                }
            }
        }
        UnityEngine.Material DefineMaterial(MaterialSettings materialSettings)
        {
            UnityEngine.Material mat = GetMaterial(materialSettings.DiffuseColor,
                           materialSettings.Transparency,
                           materialSettings.EmissiveColor,
                           materialSettings.AmbientIntensity,
                           materialSettings.Shininess,
                           materialSettings.TextureScale,
                           materialSettings.Occluded
                           );
            if(!string.IsNullOrEmpty( materialSettings.Name))
            {
                matDict[materialSettings.Name]= mat;
            }
            return mat;

        }
        public UnityEngine.Material GetMaterial(string diffuseColorStr, string transparencyStr = null, string emisiveColorStr = null, string ambientIntensityStr = null, string shininessStr = null, string textureScaleStr = null, bool occluded=false)
        {
            Material mat = null;
            float transparency = 0;
            if (transparencyStr != null)
            {
                float.TryParse(transparencyStr, out transparency);
                transparency = transparency > 1 ? 1 : transparency;
                transparency = transparency < 0 ? 0 : transparency;
            }
            Shader sh = transparency <= 1 && transparency > 0 ? standardSpecularShader : standardShader;
            if(occluded&& occludedShader!=null)
            {
                sh = occludedShader;
            }
            if (sh != null)
            {
                mat = new UnityEngine.Material(sh);
                if (transparency > 0)
                {
                    mat.SetAsTransparant();
                }
                Color color;
                if (TryColorFromString(diffuseColorStr, out color, 1 - transparency))
                {
                    mat.SetColor("_Color", color);
                }
                if (TryColorFromString(emisiveColorStr, out color))
                {
                    //mat.
                    mat.SetColor("_EmissionColor", color);
                    mat.EnableKeyword("_EMISSION");
                }
                Vector2 textureScale;
                if (TryVector2FromString(textureScaleStr, out textureScale))
                {
                    mat.SetTextureScale("_MainTex", textureScale);

                }
                //if (TryColorFromString(appearance.Material.SpecularColor, out color))
                //{
                //    mat.SetColor("_EmissionColor", color);
                //}
                float intensity;
                if (float.TryParse(ambientIntensityStr, out intensity))
                {
                    if (intensity >= 0 && intensity <= 1)
                        mat.SetFloat("_Glossiness", intensity);
                }
                float shininess;
                if (float.TryParse(shininessStr, out shininess))
                {
                    if (shininess >= 0 && shininess <= 1)
                        mat.SetFloat("_Metallic", shininess);
                }
            }
            return mat;
        }
        IEnumerator PlayMovieTexture(GameObject go, string url, bool loop = false)
        {
            ShowWaitCursor(true, DownloadType.Video);
            yield return new WaitForSeconds(2);
            var www = UnityWebRequestMultimedia.GetMovieTexture(UrlHandler.GetUrl(url));

            yield return www.SendWebRequest();

            var handler = (DownloadHandlerMovieTexture) www.downloadHandler;

            var movieTexture = handler.movieTexture;
            //while (!movieTexture.isReadyToPlay)
            //{
            //    yield return www;
            //}


            // Initialize gui texture to be 1:1 resolution centered on screen
            go.GetComponent<Renderer>().material.mainTexture = movieTexture;

            // Assign clip to audio source
            // Sync playback with audio
            var aud = go.GetComponent<AudioSource >();
            if(aud==null)
            {
                aud = go.AddComponent<AudioSource>();
            }
            aud.clip = movieTexture.audioClip;
            aud.loop = loop;
            movieTexture.loop = loop;
            // Play both movie & sound
            movieTexture.Play();
            aud.Play();
            ShowWaitCursor(false, DownloadType.Video);

        }
        public void AddTexture(GameObject go, string textureUrl)
        {
            if (!string.IsNullOrEmpty(textureUrl))
            {
                TextureContainer tc = null;
                if (!textures.ContainsKey(textureUrl))
                {
                    tc = new TextureContainer();
                    textures.Add(textureUrl, tc);
                }
                else
                {
                    tc = textures[textureUrl];
                }
                tc.GameObjects.Add(go);
            }
        }
        void AddUpdatableTexture(GameObject go, string textureUrl)
        {
            if (!string.IsNullOrEmpty(textureUrl))
            {
                TextureContainer tc = null;
                if (!updatableTextures.ContainsKey(textureUrl))
                {
                    tc = new TextureContainer();
                    updatableTextures.Add(textureUrl, tc);
                }
                else
                {
                    tc = updatableTextures[textureUrl];
                }
                tc.GameObjects.Add(go);
            }
        }
        void DrawLines(GameObject go,  IndexedLineSet indexedLineSet, Appearance appearence)
        {
            List<Pair> indices = TryGetIndices(indexedLineSet.CoordIndex);
            List<Vector3> points = TryGetPointList(indexedLineSet.Coordinate.Point);
            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Additive"));
            lr.widthMultiplier = 0.02f;
            lr.positionCount = 2*indices.Count;
            lr.startColor = Color.green;
            lr.endColor = Color.blue;
            int nn = 0;
            foreach (var idx in indices)
            {
                lr.SetPosition(nn, points[idx.First]);
                nn++;
                lr.SetPosition(nn, points[idx.Second]);
                nn++;
            }
        }
        List<Vector3> TryGetPointList(string point)
        {
            List<Vector3> pointList = new List<Vector3>();
            if(!string.IsNullOrEmpty(point))
            {
                string[] p = point.Split(Convert.ToChar(" "));
                for (int nn = 0; nn < p.Length - 2; nn = nn + 3)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if (float.TryParse(p[nn], out x) && float.TryParse(p[nn + 1], out y) && float.TryParse(p[nn + 2], out z))
                    {
                        pointList.Add(new Vector3(x, y, z));
                    }
                }

            }
            return pointList;

        }
        List<Pair> TryGetIndices(string coordIndex)
        {
            List<Pair> pairs = new List<Pair>();
            if (!string.IsNullOrEmpty(coordIndex))
            {
                string[] p = coordIndex.Split(Convert.ToChar(" "));
                for(int nn=0;nn<p.Length-1;nn=nn+2)
                {
                    int first = 0;
                    int sec = 0;
                    if(int.TryParse(p[nn], out first) && int.TryParse(p[nn+1], out sec))
                    {
                        pairs.Add(new Pair(first, sec));
                    }
                }
            }

            return pairs;
        }
        IndexedLineSet HandleIndexedLineSet(X3DParse.Node indexedLineSetNode)
        {
            IndexedLineSet indexedLineSet = new IndexedLineSet();
            foreach (var child in indexedLineSetNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "coordindex":
                        indexedLineSet.CoordIndex = child.Value;
                        break;
                    case "coordinate":
                        indexedLineSet.Coordinate = HandleCoordinate(child);
                        break;
                }

            }
            return indexedLineSet;

        }
        IndexedFaceSet HandleindexedFaceSet(X3DParse.Node indexedFaceSetNode)
        {
            IndexedFaceSet indexedFaceSet = new IndexedFaceSet();
            foreach (var child in indexedFaceSetNode.Children)//check attribs and elements
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "coordindex":
                        indexedFaceSet.CoordIndex = child.Value;
                        break;
                    case "index":
                        indexedFaceSet.Index = child.Value;
                        break;
                    case "texcoordindex":
                        indexedFaceSet.TextCoordIndex = child.Value;
                        break;
                    case "normal":
                        indexedFaceSet.Normal = HandleNormal(child);
                        break;
                    case "coordinate":
                        indexedFaceSet.Coordinate = HandleCoordinate(child);
                        break;
                    case "texturecoordinate":
                        indexedFaceSet.TextureCoordinate = HandleTextureCoordinate(child);
                        break;
                    case "multitexturecoordinate":
                        indexedFaceSet.MultiTextureCoordinate = HandleMultiTextureCoordinate(child);
                        break;
                }

            }


            return indexedFaceSet;

        }
        List<TextureCoordinate2D> HandleMultiTextureCoordinate(X3DParse.Node multiTextureCoordinateNode)
        {
            List<TextureCoordinate2D> multiTextureCoordinate = new List<TextureCoordinate2D>();
            foreach (var child in multiTextureCoordinateNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "texturecoordinate2d":
                        multiTextureCoordinate.Add(HandleTextureCoordinate2D(child));
                        break;
                }
            }
            return multiTextureCoordinate;
        }
        TextureCoordinate2D HandleTextureCoordinate2D(X3DParse.Node textureCoordinate2DNode)
        {
            TextureCoordinate2D textureCoordinate2D = new TextureCoordinate2D();
            foreach (var child in textureCoordinate2DNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "point":
                        textureCoordinate2D.Point = child.Value;
                        break;
                }
            }
            return textureCoordinate2D;
        }
        TextureCoordinate HandleTextureCoordinate(X3DParse.Node normalNode)
        {
            TextureCoordinate textureCoordinate = new TextureCoordinate();
            foreach (var child in normalNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "point":
                        textureCoordinate.Point = child.Value;
                        break;
                }
            }
            return textureCoordinate;
        }
        Coordinate HandleCoordinate(X3DParse.Node normalNode)
        {
            Coordinate coordinate = new Coordinate();
            foreach (var child in normalNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "point":
                        coordinate.Point = child.Value;
                        break;
                }
            }
            return coordinate;
        }
        Normal HandleNormal(X3DParse.Node normalNode)
        {
            Normal normal = new Normal();
            foreach (var child in normalNode.Children)//check attribs
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "vector":
                        normal.Vector = child.Value;
                        break;
                }
            }
            return normal;
        }
 
        bool TryColorFromString(string colorString, out Color color, float transparancy = 0)
        {
            color = Color.black;
            if (!string.IsNullOrEmpty(colorString))
            {
                Vector3 vect;
                if (TryVector3FromString(colorString, out vect))
                {
                    color = new Color(vect.x, vect.y, vect.z, transparancy);
                    return true;
                }
                else
                {
                    color = colorString.ToColor();
                    return color != null;
                }
            }
            return false;
        }
        Movement HandleMovementNode(X3DParse.Node materialNode)
        {
            Movement movement = new Movement();
            foreach (var child in materialNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "rotate":
                        movement.Rotation = child.Value;
                        break;
                    case "center":
                        movement.Center = child.Value;
                        break;
                    case "clingtocamera":
                        movement.ClingToCamera = child.Value;
                        break;
                    case "applytpparent":
                    case "applytoparent":
                        movement.ApplyToParent = child.Value;
                        break;
                    case "activetransform":
                        movement.ActiveTransform = child.Value;
                        break;
                    case "presentation":
                        float presentation = 0;
                        if (float.TryParse(child.Value, out presentation))
                        { movement.Presentation = presentation; }
                        break;
                    case "presentationangle":
                        float presentationangle = 0;
                        if (float.TryParse(child.Value, out presentationangle))
                        { movement.PresentationAngle = presentationangle; }
                        break;
                    case "presentationspeed":
                        float presentationspeed = 0;
                        if (float.TryParse(child.Value, out presentationspeed))
                        { movement.PresentationSpeed = presentationspeed; }
                        break;
                    case "kinetic":
                        float kinetic = 0;
                        if (float.TryParse(child.Value, out kinetic))
                        { movement.Kinetic = kinetic; }
                        break;
                    case "collidertype":
                        var val = child.Value.ToLower();
                        switch(val)
                        {
                            case "sphere":
                                movement.ColliderType = ColliderType.Sphere;
                                break;
                            case "box":
                                movement.ColliderType = ColliderType.Box;
                                break;
                            case "capsule":
                                movement.ColliderType = ColliderType.Capsule;
                                break;
                            case "none":
                                movement.ColliderType = ColliderType.None;
                                break;
                        }
                        break;
                    case "bounciness":
                        float bounciness = 0;
                        if (float.TryParse(child.Value, out bounciness))
                        { movement.Bounciness = bounciness; }
                        break;
                    case "handdraggable":
                        movement.HandDraggable= Statics.CheckBoolString(child.Value);
                        break;
                    case "handrotatable":
                        movement.HandRotatable = Statics.CheckBoolString(child.Value);
                        break;
                    case "updatable":
                        movement.Updatable = Statics.CheckBoolString(child.Value);
                        break;
                    case "serverobject":
                        movement.ServerObject = Statics.CheckBoolString(child.Value);
                        break;
                }
            }
            foreach (var child in materialNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "constraint":
                        HandleConstraint(movement, child);
                        break;
                }
            }
            return movement;
        }
        void HandleConstraint(Movement movement, X3DParse.Node constraintNode)
        {
            movement.Constraints.Add(DoHandleConstraint(constraintNode));
       }
        Constraint DoHandleConstraint( X3DParse.Node constraintNode)
        {
            Constraint constraint = new Constraint();
            foreach (var child in constraintNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "type":
                        switch (child.Value.ToLower())
                        {
                            case "sphere":
                                constraint.ConstraintType = ConstraintType.sphere;
                                break;
                            case "scaledsphere":
                                constraint.ConstraintType = ConstraintType.scaledsphere;
                                break;
                            case "block":
                                constraint.ConstraintType = ConstraintType.block;
                                break;
                            case "scale":
                                constraint.ConstraintType = ConstraintType.scale;
                                break;
                            case "delete":
                                constraint.ConstraintType = ConstraintType.delete;
                                break;
                            case "selectclose":
                                constraint.ConstraintType = ConstraintType.selectclose;
                                break;
                            case "selectfar":
                                constraint.ConstraintType = ConstraintType.selectfar;
                                break;
                            case "snap":
                                constraint.ConstraintType = ConstraintType.snap;
                                break;
                            case "constraintgroup":
                                constraint.ConstraintType = ConstraintType.constraintgroup;
                                break;
                                
                        }
                        break;
                    case "range":
                        float r;
                        if (float.TryParse(child.Value, out r))
                        {
                            constraint.Range = r;
                        }
                        break;
                    case "speed":
                        float s;
                        if (float.TryParse(child.Value, out s))
                        {
                            constraint.Speed = s;
                        }
                        break;
                    case "borders":
                        Vector3 v;
                        if (TryVector3FromString(child.Value, out v))
                            constraint.Borders = v;
                        break;
                    case "target":
                        constraint.TargetName = child.Value;
                        break;
                }
            }
            return constraint;
        }
        LineProperties HandleLineProperties(X3DParse.Node materialNode)
        {
            LineProperties lineProperties = new LineProperties();
            foreach (var child in materialNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "linetype":
                        lineProperties.LineType = child.Value;
                        break;
                }
            }
            return lineProperties;
        }
        MaterialSettings HandleMaterialNode(X3DParse.Node materialNode)
        {
            MaterialSettings material = new MaterialSettings();
            foreach (var child in materialNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "def":
                        material.Name = child.Value;
                        material.Define = true;
                        break;
                    case "use":
                        material.Name = child.Value;
                        material.Define = false;
                        break;
                    case "diffusecolor":
                        material.DiffuseColor = child.Value;
                        break;
                    case "specularcolor":
                        material.SpecularColor = child.Value;
                        break;
                    case "emissivecolor":
                        material.EmissiveColor = child.Value;
                        break;
                    case "ambientintensity":
                        material.AmbientIntensity = child.Value;
                        break;
                    case "shininess":
                        material.Shininess = child.Value;
                        break;
                    case "transparency":
                        material.Transparency = child.Value;
                        break;
                    case "texturescale":
                        material.TextureScale = child.Value;
                        break;
                    case "occluded":
                        material.Occluded = Statics.CheckBoolString( child.Value);
                        break;
                    case "physics":
                        material.Physics = child.Value;
                        break;
                }
            }
            return material;
            //XmlNode def = appearanceNode.SelectSingleNode("@DEF");
            //if (def != null)
            //{
            //    XmlNode diffuseColorNode = appearanceNode.SelectSingleNode("@diffuseColor");
            //    if (diffuseColorNode != null)
            //    {
            //        matName = def.InnerText;
            //        Material mat = new Material(Shader.Find("Standard"));
            //        Vector3 vect = VectFromString(diffuseColorNode.InnerText);
            //        mat.SetColor("_Color", new Color(vect.x, vect.y, vect.z));
            //        matDict.Add(matName, mat);
            //    }
            //}
            //else
            //{
            //    XmlNode useN = appearanceNode.SelectSingleNode("@USE");
            //    if (useN != null)
            //    {
            //        matName = useN.InnerText;
            //    }

            //}
            //return matName;
        }
        ImageTexture HandleImageTextureNode(X3DParse.Node imageTextureNode)
        {
            ImageTexture imageTexture = new ImageTexture();
            foreach (var child in imageTextureNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "url":
                        imageTexture.Url = UrlHandler.CheckInlineUrl( child.Value.Replace("\"", ""));
                        break;
                }
            }
            return imageTexture;
        }
        //public string TakeFirstOfX3DArray(string array)
        //{
        //    if(!string.IsNullOrEmpty( array))
        //    {
        //        //string[] arr
        //    }
        //}
        MovieTexture HandleMovieTextureNode(X3DParse.Node movieTextureNode)
        {
            MovieTexture movieTexture = new MovieTexture();
            foreach (var child in movieTextureNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "url":
                        movieTexture.Url = child.Value;
                        break;
                    case "loop":
                        movieTexture.Loop = child.Value.ToLower()=="true"|| child.Value == "1";
                        break;
                }
            }
            return movieTexture;
        }
        Appearance HandleAppearanceNode(X3DParse.Node appearanceNode)
        {
            Appearance appearance = new Appearance();
            CheckUSE<Appearance>(appearanceNode, ref appearance);
            foreach (var child in appearanceNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "material":
                        appearance.Material = HandleMaterialNode(child);
                        break;
                    case "imagetexture":
                        appearance.ImageTexture = HandleImageTextureNode(child);
                        break;
                    case "movietexture":
                        appearance.MovieTexture = HandleMovieTextureNode(child);
                        break;
                    case "lineproperties":
                        appearance.LineProperties = HandleLineProperties(child);
                        break;
                    case "slm:movement":
                        appearance.Movement = HandleMovementNode(child);
                        break;
                }
            }
            CheckDEF<Appearance>(appearanceNode, ref appearance);
            return appearance;
        }
        void HandleLight(X3DParse.Node lightNode, LightType lightType, Transform parent=null)
        {

            var go = new GameObject();
            go.name = lightType.ToString() + "Light";
            CheckNameDefProperties(lightNode, go);
            if(parent!=null)
            {
                go.transform.SetTheParent(parent);
            }
            else if (inlineParentGP != null)
            {
                go.transform.SetTheParent(inlineParentGP.transform);
            }
            else
            {
                go.transform.parent = SceneGO.transform;
            }
            // go.transform.position = Vector3(0, 5, 0);
            var light = go.AddComponent<Light>();
            light.type = lightType;
            LightProperties lightProperties = new LightProperties();
            CheckUSE<LightProperties>(lightNode, ref lightProperties);
            foreach (var child in lightNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "location":
                        Vector3 loc = Vector3.zero;
                        if (TryVector3FromString(child.Value, out loc))
                        {
                            lightProperties.Location = loc;
                            //go.transform.localPosition = loc;
                        }
                        break;
                    case "direction":
                        Vector3 dir = Vector3.zero;
                        if (TryVector3FromString(child.Value, out dir))
                        {
                            lightProperties.Rotation=Quaternion.Euler(dir);
                            //go.transform.rotation = 
                        }
                        break;
                    case "slm:eulerrotation":
                        Vector3 rotEuler;
                        if (TryVector3FromString(child.Value, out rotEuler))
                        {
                            lightProperties.Rotation = Quaternion.Euler(rotEuler);
                            //go.transform.rotation = Quaternion.Euler(rotEuler);
                        }
                        break;
                    case "shadowintensity":
                        float shintens = 0;
                        if (float.TryParse(child.Value, out shintens))
                        {
                            lightProperties.ShadowStrength = shintens;
                            //light.shadowStrength = shintens;
                            //light.shadows = LightShadows.Soft;
                        }
                        break;
                    case "intensity":
                        float intens = 0;
                        if (float.TryParse(child.Value, out intens))
                        {
                            lightProperties.Intensity = intens;
                        }
                        break;
                    case "cutoffangle":
                        float bw = 0;
                        if (float.TryParse(child.Value, out bw))
                        {
                            lightProperties.CutOffAngle = bw;
                            //light.spotAngle = bw;
                        }
                        break;
                    case "range":
                        float r = 0;
                        if (float.TryParse(child.Value, out r))
                        {
                            lightProperties.Range = r;
                            //light.range = r;
                        }
                        break;
                    case "color":
                        Color col = Color.white;
                        if (TryColorFromString(child.Value, out col))
                        {
                            lightProperties.Color = col;
                            //light.color = col;
                        }
                        break;
                }
            }
            
            //apply lightProperties:
            if (lightProperties.Location!=null)
            {
                go.transform.localPosition = (Vector3)lightProperties.Location;
            }
            if (lightProperties.Rotation != null)
            {
                go.transform.rotation = (Quaternion)lightProperties.Rotation;
            }
            if (lightProperties.ShadowStrength != null)
            {
                light.shadowStrength = (float)lightProperties.ShadowStrength;
                light.shadows = LightShadows.Soft;
            }
            if (lightProperties.Intensity != null)
            {
                light.intensity = (float)lightProperties.Intensity;
            }
            if (lightProperties.CutOffAngle != null)
            {
                light.spotAngle = (float)lightProperties.CutOffAngle;
            }
            if (lightProperties.Range != null)
            {
                light.range = (float)lightProperties.Range;
            }
            light.color = lightProperties.Color;
            
            CheckDEF<LightProperties>(lightNode, ref lightProperties);
        }
        void HandleAudioClip(X3DParse.Node audioClipNode)
        {
            if (audioSource != null)
            {
                string url = null;
                bool loop = false;
                bool effect = false;
                float volume = 1;
                foreach (var child in audioClipNode.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "url":
                            List<string> urls = Statics.ParseStringParts(child.Value);
                            if (urls.Count > 0)
                            {
                                //if (string.IsNullOrEmpty(UrlHandler.InlineUrl))
                                //{
                                    url = UrlHandler.GetUrl(urls[0]);
                                //}
                                //else
                                //{
                                //    url = UrlHandler.CheckInlineUrl(urls[0]);
                                //}
                                // url = 
                            }
                            break;
                        case "loop":
                            loop = child.Value.ToLower() == "true";
                            break;
                        case "slm:effect":
                            effect = child.Value.ToLower() == "true";
                            break;
                        case "volume":
                            float.TryParse(child.Value, out volume);
                            break;

                    }
                }
                if (!string.IsNullOrEmpty(url))
                {
                    StartSceneCoroutine(PlayAudio(url, loop, volume, effect));
                }

            }
        }
        IEnumerator PlayAudio(string url, bool loop, float volume, bool effect)
        {
            yield return new WaitForSeconds(0.1f);
            if (UrlHandler.AudioUrl == url.ToLower()&& !effect)
            {
                yield return null;
            }
            else
            {
                // Start a download of the given URL
                // Wait for download to complete
                AudioType at = AudioType.UNKNOWN;
                if(url.ToLower().EndsWith(".wav"))
                {
                    at = AudioType.WAV;
                }
                if (url.ToLower().EndsWith(".ogg"))
                {
                    at = AudioType.OGGVORBIS;
                }

                UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(UrlHandler.GetUrl(url), at);
                yield return www.SendWebRequest();
                if (true || !www.isNetworkError)
                {
                    var handler = (DownloadHandlerAudioClip)www.downloadHandler;
                    var audioClip = handler.audioClip; 


                    if (effect && audioSourceEffect != null)
                    {
                        audioSourceEffect.volume = volume;
                        audioSourceEffect.PlayOneShot(audioClip);
                    }
                    else
                    {
                        audioSource.loop = loop;
                        audioSource.volume = volume;
                        audioSource.clip = audioClip;

                    }
                    UrlHandler.AudioUrl = url.ToLower();
                }

            }
        }

        void ApplyTextures(TextureContainer tc, bool keepGameObjects=false)
        {
            foreach (var go in tc.GameObjects)
            {
                if (go != null)
                {
                    Renderer renderer = go.GetComponent<Renderer>();
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.mainTexture = tc.Texture;
                    }
                    //guitexture
                    else
                    {
                        var rt = go.GetComponent<RectTransform>();
                        var image = go.GetComponent<UnityEngine.UI.Image>();
                        if (rt != null && image != null)
                        {
                            Texture2D t2d = (Texture2D)tc.Texture;
                            image.type = UnityEngine.UI.Image.Type.Filled;
                            var mySprite = Sprite.Create(t2d, new Rect(0,0,t2d.width, t2d.height), rt.pivot);
                            image.sprite = mySprite;
                        }
                    }
                }
            }
            if (!keepGameObjects)
            {
                tc.GameObjects.Clear();
                tc.Texture = null;//[ajw20180722 line restored]
            }
        }
        IEnumerator GetTexture(string url, TextureContainer tc, bool updatable=false)
        {
            if (tc.GameObjects.Count > 0)
            {
                if (tc.Texture != null)
                {
                    ApplyTextures(tc, updatable);
                }
                yield return null;
            }
            if (!updatable)
            {
                ShowWaitCursor(true, DownloadType.Texture);
            }
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(UrlHandler.GetUrl(url));
            yield return www.SendWebRequest();

            //

            if (string.IsNullOrEmpty(www.error))
            {
                tc.Texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                ApplyTextures(tc, updatable);
                 ShowWaitCursor(false, DownloadType.Texture);
                //remove when finished
//                if()
           }
            else
            {
                ShowWaitCursor(false, DownloadType.Texture);
            }
        }
        GameObject CreatePrimitiveNode(Transform parent, PrimitiveType primitive)
        {
            GameObject primitiveObj = GameObject.CreatePrimitive(primitive);
            primitiveObj.transform.SetTheParent(parent);
            primitiveObj.transform.localPosition = Vector3.zero;
            primitiveObj.transform.localRotation = Quaternion.identity;
            primitiveObj.transform.localScale = Vector3.one;
            return primitiveObj;
        }
        public TextMeshPro GetTextMeshObject(string txt=null, float rectLength=50, float rectHeight=5, float fontSize=20, TextAlignmentOptions tao= TextAlignmentOptions.Right)
        {
            GameObject go = new GameObject("text");
            var text = go.AddComponent<TextMeshPro>();
            text.alignment = tao;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(rectLength, rectHeight);
            text.fontSize = fontSize;
            txt = txt.Replace("&quot;", "\"").Replace("&apos;","'");
            text.text = txt;
            return text;
        }
        GameObject HandleTextNode(X3DParse.Node textNode, Transform parent, ref InputType inputType)
        {
            inputType = InputType.None;
            FontStyle fontStyle = null;
            float rectLength = 50;
            float rectHeight = 20;
            string[] parameters=null;
            string totText="";
            foreach (var child in textNode.Children)
            {
                if (child.NodeType == X3DParse.NodeType.Attribute)
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "string":
                            totText = Statics.X3dTextToMultiline(child.Value);
                            break;
                        case "slm:rowlength":
                            float.TryParse(child.Value, out rectLength);
                            break;
                        case "length":
                            float.TryParse(child.Value, out rectLength);
                            break;
                        case "height":
                            float.TryParse(child.Value, out rectHeight);
                            break;
                        case "slm:params":
                            parameters = child.Value.ToLower().Split(Convert.ToChar(";"));
                            break;
                        case "slm:input":
                            TryParse(child.Value.ToLower(), out inputType);
                            break;

                    }
                }
                else if (child.NodeType == X3DParse.NodeType.Element)
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "fontstyle":
                            fontStyle = HandleFontStyle(child);
                            break;
                    }
                }
            }
            TextAlignmentOptions tao = TextAlignmentOptions.Center;
            float fontSize = 20;
            if (fontStyle != null)
            {
                if (fontStyle.Justify != null)
                {
                    if (fontStyle.Justify.Contains("topleft"))
                    {
                        tao = TextAlignmentOptions.TopLeft;
                    }
                    else if (fontStyle.Justify.Contains("topright"))
                    {
                        tao = TextAlignmentOptions.TopRight;
                    }
                    else if (fontStyle.Justify.Contains("topmiddle"))
                    {
                        tao = TextAlignmentOptions.Top;
                    }
                    else if (fontStyle.Justify.Contains("bottomleft"))
                    {
                        tao = TextAlignmentOptions.BottomLeft;
                    }
                    else if (fontStyle.Justify.Contains("left"))
                    {
                        tao = TextAlignmentOptions.Left;
                    }
                    else if (fontStyle.Justify.Contains("right"))
                    {
                        tao = TextAlignmentOptions.Right;
                    }
                    else if (fontStyle.Justify.Contains("middle"))
                    {
                        tao = TextAlignmentOptions.Center;
                    }
                }
            }
            if (fontStyle!=null && fontStyle.Size != null)
            {
                fontSize = (float)fontStyle.Size;
            }
            var text = GetTextMeshObject(totText, rectLength, rectHeight, fontSize, tao);
            GameObject go = text.gameObject;
            CheckNameDefProperties(textNode, go);


            if(inputType!= InputType.None)
            {
                var bc=go.AddComponent<BoxCollider>();
                bc.size = new Vector3(rectLength, rectHeight, 0.01f);
                if(inputType== InputType.Community)
                {
                    communityPosts = go.GetComponentInChildren<TextMeshPro>();
                }
            }
 
            text.transform.SetTheParent(parent);
            HandleTextDynamics(go, text.text, parameters);
            return go;
        }
 
        bool TryParse(string t, out InputType inputType)
        {
            inputType = InputType.None;
            if (t != null)
            {
                switch (t)
                {
                    case "text":
                        inputType = InputType.Text;
                        return true;
                    case "password":
                        inputType = InputType.Password;
                        return true;
                    case "number":
                        inputType = InputType.Number;
                        return true;
                    case "datetime":
                        inputType = InputType.DateTime;
                        return true;
                    case "date":
                        inputType = InputType.Date;
                        return true;
                    case "community":
                        inputType = InputType.Community;
                        return true;

                }
            }
            return false;
        }
        void HandleTextDynamics(GameObject go,string text, string[] parameters)
        {
            if(parameters!=null && parameters.Length>0)
            {
                var td= go.AddComponent<TextDynamics>();
                for(int nn=0;nn< parameters.Length;nn++)
                {
                    TextDynamic etd = TextDynamic.CurrentUser;
                    if(TryGetTextDynamic(parameters[nn], out etd))
                    {    
                        td.TextDynamicList.Add(etd);
                    }
                }
                td.Text = text;
            }
        }
        bool TryGetTextDynamic(string text, out TextDynamic textDynamic)
        {
           textDynamic = TextDynamic.CurrentUser;
           foreach (TextDynamic e in Enum.GetValues(typeof(TextDynamic)))
            {
                if(e.ToString().ToLower()==text)
                {
                    textDynamic = e;
                    return true;
                }
            }
            return false;
        }
        FontStyle HandleFontStyle(X3DParse.Node node)
        {
            FontStyle fontStyle = new FontStyle();
            CheckUSE<FontStyle>(node, ref fontStyle);
            float size = 0;
            foreach (var child in node.Children)
            {
                if (child.NodeType == X3DParse.NodeType.Attribute)
                {
                    string name = child.Name.ToLower();
                    switch (name)
                    {
                        case "justify":
                            fontStyle.Justify = child.Value.ToLower();
                            break;
                        case "size":
                            if (float.TryParse(child.Value, out size))
                            {
                                fontStyle.Size = size;
                            }
                            break;
                    }
                }
            }
            CheckDEF<FontStyle>(node, ref fontStyle);
            return fontStyle;
        }
#endregion Parse
#region Helperfuctions
        List<string> ParseStringParts(string text)
        {
            List<string> lst = new List<string>();
            char[] chars = text.ToCharArray();
            string t = "";
            string quoteChar = null;
            bool wordstarted = false;

            for (int nn = 0; nn < chars.Length; nn++)
            {
                string c = chars[nn].ToString();
                if (quoteChar==null && (c == "\"" || c == "'"))
                {
                    quoteChar = c;
                }
                if ( c == quoteChar)
                {
                    if (wordstarted)
                    {
                        lst.Add(t);
                        t = "";
                    }
                    wordstarted = !wordstarted;
                }
                else if (wordstarted)
                {
                    t += c;
                }

            }
            if(lst.Count==0)
            {
                lst.Add(text);
            }
            return lst;
        }
        void ApplyTransformations(X3DParse.Node node, Transform transform)
        {
            //transform.ClearAll();
            foreach (var child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "translation":
                        Vector3 trans;
                        if (TryVector3FromString(child.Value, out trans))
                        {
                            //x3d convention: 
                            Vector3 transCor = new Vector3(trans.x, trans.y, trans.z);
                            transform.localPosition = transCor;
                        }
                        break;
                    case "position":
                        Vector3 pos;
                        if (TryVector3FromString(child.Value, out pos))
                        {
                            Vector3 posCor = new Vector3(pos.x, pos.y, pos.z);
                            transform.position = posCor;
                        }
                        break;
                    case "rotation":
                        Quaternion rot;
                        if (TryQuaternionFromString(child.Value, out rot))
                        {
                            Quaternion rotCor = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                            transform.localRotation = rotCor;
                        }
                        break;
                    case "slm:eulerrotation":
                        Vector3 rotEuler;
                        if (TryVector3FromString(child.Value, out rotEuler))
                        {
                            Quaternion rotE =Quaternion.Euler(rotEuler);
                            //Quaternion rotECor = new Quaternion(rotE.x, rotE.y, -rotE.z, rotE.w);
                            transform.localRotation = rotE;
                        }
                        break;
                    case "centerOfRotation":
                        Vector3 cor;
                        if (TryVector3FromString(child.Value, out cor))
                        {
                            transform.LookAt(cor);
                        }
                        break;
                    case "scale":
                        Vector3 ls;
                        if (TryVector3FromString(child.Value, out ls))
                        {
                            transform.localScale = ls;
                        }
                        break;
                    case "name":
                        if (!string.IsNullOrEmpty(child.Value))
                        {
                            transform.gameObject.name = child.Value;
                        }
                        break;
                }
            }

        }
        void CheckUSE<T>(X3DParse.Node node, ref T defObject)
        {
            if (node != null && node.Children != null)
            {
                var use = node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute && (x.Name.ToLower() == "use" || x.Name.ToLower() == "def"));
                if (use != null && use.Count > 0)
                {
                    var key = use[0].Value.ToLower();
                    if (definitions.ContainsKey(key))
                    {
                        var o = definitions[key];
                        if (o is T)
                        {
                            defObject = (T)o;
                        }
                    }
                }
            }
        }
        void CheckDEF<T>(X3DParse.Node node, ref T defObject)
        {
            if (node != null && node.Children!=null)
            {
                var def = node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute && (x.Name.ToLower() == "def"));
                if (def != null && def.Count > 0)
                {
                    var key = def[0].Value.ToLower();
                    definitions[key] = defObject;
                }
            }
        }
        Quaternion QuatFromString(string quatString)
        {
            if (!string.IsNullOrEmpty(quatString))
            {
                string[] w = quatString.Split(Convert.ToChar(" "));
                return new Quaternion((float)Convert.ToDecimal(w[0]), (float)Convert.ToDecimal(w[1]), (float)Convert.ToDecimal(w[2]), (float)Convert.ToDecimal(w[3]));
            }
            return Quaternion.identity;
        }
        bool TryQuaternionFromString(string vectString, out Quaternion v)
        {
            v = Quaternion.identity;
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Trim().Split(Convert.ToChar(" "));
                if (w.Length > 3)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    float q = 0;
                    if (float.TryParse(w[0], out x) && float.TryParse(w[1], out y) && float.TryParse(w[2], out z) && float.TryParse(w[3], out q))
                    {
                        v = new Quaternion(x, y, z, q);
                        return true;
                    }
                }
            }
            return false;
        }
        bool TryVector2FromString(string vectString, out Vector2 v)
        {
            v = Vector2.zero;
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Trim().Split(Convert.ToChar(" "));
                if (w.Length > 1)
                {
                    float x = 0;
                    float y = 0;
                    if (float.TryParse(w[0], out x) && float.TryParse(w[1], out y))
                    {
                        v = new Vector2(x, y);
                        return true;
                    }
                }
            }
            return false;
        }
        bool TryListTypeFromString(string listtypestr, out ListType listtype)
        {
            listtype = ListType.standard;
            if (!string.IsNullOrEmpty(listtypestr))
            {
                if (listtypestr.ToLower() == ListType.simple.ToString())
                {
                    listtype = ListType.simple;
                    return true;
                }
                if (listtypestr.ToLower() == ListType.standard.ToString())
                {
                    listtype = ListType.standard;
                    return true;
                }
            }
            return false;
        }
        bool TryVector3FromString(string vectString, out Vector3 v)
        {
            v = Vector3.zero;
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Trim().Split(Convert.ToChar(" "));
                if (w.Length > 2)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if (float.TryParse(w[0], out x) && float.TryParse(w[1], out y) && float.TryParse(w[2], out z))
                    {
                        v = new Vector3(x, y, z);
                        return true;
                    }
                }
            }
            return false;
        }
        Vector3 Vector3FromString(string vectString)
        {
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Split(Convert.ToChar(" "));
                return new Vector3((float)Convert.ToDecimal(w[0]), (float)Convert.ToDecimal(w[1]), (float)Convert.ToDecimal(w[2]));
            }
            return Vector3.zero;
        }
        public bool IsHoloLens()
        {
            return DeviceManager.Name == CallingDevices.HoloLens.ToString();
        }
        public bool IsImmersive()
        {
            return DeviceManager.Name == CallingDevices.MR.ToString();
        }
#endregion Helperfuctions
#region Avatars
        public Vector3 AvatarRelPosition(Vector3 pos)
        {
            return Camera.main.transform.position - pos;
        }
        private void IndicateNewAvatar(string name=null)
        {
            if(audioSourceAvatar != null &&!UrlHandler.AtHome )
            {
                newAvatars.Add(name);
                audioSourceAvatar.Play();
            }
        }
        GameObject GetAvatar(string avatNo, string id, string legacyAvatarBundleUrl, out Avatarcorrection avatarCorrection)
        {
            GameObject avatar = null;
            if ( !string.IsNullOrEmpty(legacyAvatarBundleUrl))
            {
                var tavatar = CheckLegacyAvatar(id, legacyAvatarBundleUrl);
                if (tavatar != null)
                {
                      avatar = tavatar;
                }
            }
            if (avatar == null)
            {
                avatar = GetPrefab(SceneGO.transform, "avatars", "avatar" + avatNo);
            }
            if (avatar.GetComponent<Avatar>() != null)
            {
                Destroy(avatar.GetComponent<Avatar>());
            }
            avatarCorrection=avatar.AssureComponent< Avatarcorrection >();
            var anim = avatar.AssureComponent<Animator>();
            anim.runtimeAnimatorController= Resources.Load("Animator/humanMaleCtrl") as RuntimeAnimatorController;
            return avatar;
        }
        TimedGameObject AssureTimedGameObjectWithAvatar(Transform parent, string nickN,  string dissGuid, string avatNo, string id, string legacyAvatarBundleUrl, bool isOwnAvatar, out GameObject avatar, out bool isNew)
        {
            TimedGameObject tgo = null;
            avatar = null;
            if (!avatars.TryGetValue(id, out tgo))//no registred avatar
            {
                GameObject avatarParent = new GameObject();
                tgo = new TimedGameObject(avatarParent, dissGuid);
                tgo.AvatarNo = avatNo;
                tgo.GameObject.transform.SetTheParent(parent);
                Avatar avScrpt = PrepAvatarParent(avatarParent);
                avScrpt.ToolTip = string.IsNullOrEmpty(nickN) ? "Unknown" : nickN;
                avScrpt.Controlled = true;
                avScrpt.AvatarId = id;
                avScrpt.AvatarDissId = dissGuid;
                avScrpt.NickName = nickN;
#if USE_DISS
                var player = avatarParent.AddComponent<LlapiPlayer>();
#endif
                AddSpeekIndicator(tgo.GameObject.transform);
                //add avatar
                Avatarcorrection avCorr = null;
                avatar = GetAvatar( avatNo, id, legacyAvatarBundleUrl, out avCorr);
                avatar.transform.ClearPosition();
                avatar.transform.SetTheParent(avatarParent.transform);
                
                CheckAvatarCorrection(avatar, avCorr);
               if (isOwnAvatar)
                {
#if USE_DISS
                    player.Type = Dissonance.NetworkPlayerType.Local;
#endif
                    avatarParent.transform.position = new Vector3(0, -Constants.cameraHeight, 0);
                    avatarParent.transform.rotation = Quaternion.identity * Quaternion.Euler(0, 180, 0);
                    avScrpt.IsOwnAvatar = true;
                    tgo.GameObject.SetLayerRecursively( LayerMask.NameToLayer(Constants.LayerNameMainCamHidden));
                }
                 //avatar.SetActive(!isOwnAvatar);
                avatars[id] = tgo;
                isNew = true;
            }
            else
            {
                var x = tgo.GameObject.GetComponentInChildren<Avatarcorrection>();
                if (x != null)
                {
                    avatar = x.gameObject;
                }
                if(tgo.AvatarNo!=avatNo||(!string.IsNullOrEmpty(legacyAvatarBundleUrl) && legacyAvatarRetrieved.ContainsKey(id) && legacyAvatarRetrieved[id] == true) )//Other avatar chosen or legacy avatar present
                {
                    Avatarcorrection avCorr = null;
                    var newavatar = GetAvatar(avatNo, id, legacyAvatarBundleUrl, out avCorr);
                    if(newavatar!=null)
                    {
                        var oldavatar = avatar;
                        //newavatar.transform.localPosition = oldavatar.transform.localPosition;
                        //newavatar.transform.localRotation = oldavatar.transform.localRotation;
                        Destroy(oldavatar);
                        newavatar.transform.SetTheParent(tgo.GameObject.transform);
                        var avScript = tgo.GameObject.GetComponent<Avatar>();
                        if(avScript!=null)
                        {
                            avScript.InitiateAvatar();
                        }
                        if (isOwnAvatar)
                        {
                             tgo.GameObject.SetLayerRecursively(LayerMask.NameToLayer(Constants.LayerNameMainCamHidden));
                        }

                    }
                    CheckAvatarCorrection(newavatar, avCorr);
                }
                if (!string.IsNullOrEmpty(dissGuid))
                {
#if USE_DISS
                    var player = tgo.GameObject.GetComponent<LlapiPlayer>();
                    if (player != null && string.IsNullOrEmpty(player.PlayerId))
                    {
                        player.SetPlayerId(dissGuid);
                    }
#endif
                }
                isNew = false;
            }
            return tgo;
        }

        public void CheckAvatar(string id, Vector3 pos, Quaternion rot, string nickN, string avatNo, string dissGuid, string legacyAvatarBundleUrl, string lhp, string lhr, string rhp, string rhr)
        {
            TimedGameObject tgo = null;
            bool isOwnAvatar = id == DeviceManager.UserGuid;
            if(UrlHandler.AtHome && !isOwnAvatar) { return; }
            Transform parent =  SceneGO.transform;//isOwnAvatar ? Camera.main.transform :
            bool isNew = false;

            GameObject avatar = null;
            tgo = AssureTimedGameObjectWithAvatar(parent, nickN, dissGuid, avatNo, id, legacyAvatarBundleUrl, isOwnAvatar, out avatar, out isNew);

            if(isNew)
            {
                if (myAvatarPresentation!=null && isOwnAvatar)
                {
                    var nw = Instantiate(avatar);
                    if(nw.GetComponent<Avatar>()!=null)
                    {
                        Destroy(nw.GetComponent<Avatar>());
                    }
                    nw.transform.SetTheParent(myAvatarPresentation.transform);
                }
                if (!isOwnAvatar && !playersPresentCache.Contains(id))
                {
                    IndicateNewAvatar(nickN);
                    playersPresentCache.Add(id);
                }
            }

            tgo.DateTime = DateTime.Now;

            Avatar avScr = tgo.GameObject.GetComponent<Avatar>();
            if (!isOwnAvatar)
            {
                avScr.NewPosition = ScenePosition(pos) + new Vector3(0, -Constants.cameraHeight, 0);
                avScr.NewRotation = rot;// Quaternion.Euler(rot.eulerAngles.y,0);
                if (isNew)
                {
                    tgo.GameObject.transform.position = avScr.NewPosition;
                    tgo.GameObject.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
                }
                CheckHand(avScr.leftHandT, lhp, lhr, ref lhTimer);
                CheckHand(avScr.rightHandT, rhp, rhr, ref rhTimer);

            }

        }
        float rhTimer = 0;
        float lhTimer = 0;
        void CheckHand(Transform handT, string hp, string hr, ref float htimer)
        {
            if (handT != null)
            {
                if (!string.IsNullOrEmpty(hp) && !string.IsNullOrEmpty(hr))
                {
                    Vector3 hpos;
                    Quaternion hrot;
                    if (TryVector3FromString(hp, out hpos) && TryQuaternionFromString(hr, out hrot))
                    {
                        htimer = Time.time + 1;
                        //avScr.leftHandT.parent = null;
                        handT.position = ScenePosition(hpos);
                        handT.rotation = hrot;
                        // avScr.leftHandT.parent = avScr.transform;
                    }
                }

                if (Time.time < htimer)
                {
                    if (!handT.gameObject.activeSelf)
                    {
                        handT.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (handT.gameObject.activeSelf)
                    {
                        handT.gameObject.SetActive(false);
                    }
                }
            }
        }
        private Avatar PrepAvatarParent(GameObject avatarParent)
        {
            Avatar scpt= avatarParent.AssureComponent<Avatar>();
            var coll = avatarParent.AssureComponent<CapsuleCollider>();
            coll.height = 1.1f;
            coll.radius = 0.25f;
            coll.center = new Vector3(0, 1.4f, 0);
            return scpt;
        }
        private GameObject CheckLegacyAvatar(string id, string legacyAvatarUrl)
        {
            GameObject go = null;
            if(!assetBundles.ContainsKey(id) && !legacyAvatarRetrieved.ContainsKey(id))
            {
                //AssetBundleData ad = new AssetBundleData();
                legacyAvatarRetrieved[id] = false;
                StartCoroutine(DownLoadAsset(legacyAvatarUrl, id, true));
            }
            else if(assetBundles.ContainsKey(id) && assetBundles[id].Contains(Constants.legacy_avatar_name))
            {
                var tgo = assetBundles[id].LoadAsset<GameObject>(Constants.legacy_avatar_name);
                go = Instantiate(tgo);
                if(legacyAvatarRetrieved.ContainsKey(id))
                {
                    legacyAvatarRetrieved.Remove(id);
                }

            }
            return go;
        }

        public void CorrectAvatarsPosition(Vector3 sceneMove)
        {
            foreach(var avatar in avatars)
            {
                var avScr=avatar.Value.GameObject.GetComponent<Avatar>();
                if(!avScr.IsOwnAvatar)
                {
                    avScr.NewPosition += sceneMove;
                  //  avScr.transform.position += sceneMove;
                }
            }
        }
        void CheckCameraHeight()
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, -Vector3.up);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                var hp = hit.point;
                var go = hit.collider.gameObject;
                if (go != null)
                {
                    float cameraHeight = Constants.cameraHeight;
                    var so = go.GetComponent<SlamObject>();
                    var currentAction = "i";
                    if (so != null && so.SitPlane)
                    {
                        cameraHeight = Constants.cameraHeightSitting;
                        currentAction = "s";
                    }
                    Slam.Instance.AvatarAction = currentAction;
#if UNITY_WSA
                   if (IsHoloLens())
                    {
                        Vector3 camCP = CameraTransForm.position;
                        Vector3 camNewPosition = new Vector3(camCP.x, hp.y + cameraHeight, camCP.z);
                        CameraTransForm.position = Vector3.Lerp(camCP, camNewPosition, 10 * Time.deltaTime);
                    }
                    else if (CameraCache.Main != null && CameraCache.Main.transform.parent!=null && Vector3.Distance(CameraCache.Main.transform.position, hp)>1.8 )
                    {
                        realCamY = Camera.main.transform.localPosition.y;
                        Vector3 camCP = CameraCache.Main.transform.parent.position;
                        Vector3 camNewPosition = new Vector3(camCP.x, hp.y + cameraHeight-realCamY, camCP.z);
                        CameraCache.Main.transform.parent.position = camNewPosition;// Vector3.Lerp(camCP, camNewPosition, 10 * Time.deltaTime);
                    }
                    if(mrCameraCheckTime>0)
                    {
                        mrCameraCheckTime -= Time.deltaTime;
                    }
#else

                    Vector3 camCP = CameraTransForm.position;
                    Vector3 camNewPosition = new Vector3(camCP.x, hp.y+ cameraHeight, camCP.z);
                    CameraTransForm.position = Vector3.Lerp(camCP, camNewPosition, 10*Time.deltaTime);
#endif
                }
            }

        }




        public void EnableAvatars(bool enable)
        {
 
            Communicator.Instance.AvatarOn = enable;
            if (!enable)
            {
                RemoveAvatars(DateTime.Now.AddYears(100));
            }
            DeviceManager.AvatarFunction = enable;
        }
        void RemoveAvatars(DateTime dateTime)
        {
            List<string> keys = new List<string>();
            foreach (var tgo in avatars)
            {
                if (tgo.Value.DateTime < dateTime)
                {
                    Destroy(tgo.Value.GameObject);
                    keys.Add(tgo.Key);
                }
            }
            foreach (var key in keys)
            {
                avatars.Remove(key);
            }

        }
        List<string> newAvatars = new List<string>();
        public void HandleCommunication(X3DParse.Node node)
        {
            RemoveAvatars(DateTime.Now.AddSeconds(-3));
            if (true)//!UrlHandler.AtHome)
            {
                newAvatars.Clear();
                foreach (var child in node.Children.FindAll(x => x.Name == "u"))
                {
                    HandleAvatarUser(child);
                }
                CheckNotification();
                //if (RecordActions)
                //{
                    var ah = node.Children.Find(x => x.Name == "ah");
                    if(ah!=null)
                    {
                        HandleActions(ah);
                    }
                var p = node.Children.Find(x => x.Name == "p");
                if (p != null)
                {
                    HandlePosts(p);
                }
                var sol = node.Children.Find(x => x.Name == "sol");
                if (sol != null)
                {
                    HandleServerObjects(sol);
                }
            }
        }
        float checkActionTimer = 0;
        private int placeHolderCounter=0;

        void CheckActionsToPerform()
        {
            if(ActionsToPerform.Count>0)
            {
                hideWaitCursor = true;
                if (IsxmlOrTextureDownloading())
                {
                    checkActionTimer = 0.1f;
                }
                checkActionTimer -= Time.deltaTime;
                if (checkActionTimer <= 0)
                {
                    var act = ActionsToPerform.Peek();
                    if ((FastForwardActions || act.SceneTime < SceneTimer) && SceneGO != null)
                    {
                        var go = SceneGO.transform.FindDeepChild(act.GoName);
                        if (go != null)
                        {
                            var so = go.gameObject.GetComponent<SlamObject>();
                            if (so != null)
                            {
                                so.DoSelect(Vector3.zero);
                                ActionsToPerform.Dequeue();
                                ActionsHandled.Add(act);
                                checkActionTimer = 0.3f;
                            }
                        }
                    }
                }
            }
            else
            {
                hideWaitCursor = false;
            }
        }
        void HandlePosts(X3DParse.Node node)
        {
            Post p = new Post();
            foreach (var a in node.Children)
            {
                string name = a.Name;
                switch (name)
                {
                    case "dt":
                        DateTime t;
                        if(DateTime.TryParse(a.Value, out t))
                        {
                            p.Time = t;
                        }
                        break;
                    case "t":
                        p.Text = a.Value;
                        break;
                    case "n":
                        p.NickName = a.Value;
                        break;
                    case "g":
                        p.Guid = a.Value;
                        break;

                }
                if (p.Guid!=null && posts.Find(x => x.Guid == p.Guid) == null)
                {
                    posts.Add(p);
                }
                if (!string.IsNullOrEmpty(p.Guid))
                {
                    latestPostGuid = p.Guid;
                    ApplyPosts();
                }
            }
        }
        void ApplyPosts()
        {
            //if(communityPosts==null)
            //{
            //    GameObject go = GameObject.Find(Constants.communityPostsGo);
            //    if(go!=null)
            //    {
            //        communityPosts = go.GetComponentInChildren<TMPro.TextMeshPro>();
            //    }
            //}
            if(communityPosts!=null)
            {
                string t = "";
                posts.Sort(delegate (Post x, Post y)
                {
                   return x.Time.CompareTo(y.Time);
                });

                foreach (var p in posts)
                {
                    t += string.Format("{0:yyyy-MM-dd HH:mm} {1}: {2}", p.Time, p.NickName, p.Text) + "\r\n";
                }
                communityPosts.text = t;
            }
        }
        void HandleServerObjects(X3DParse.Node node)
        {
            try
            {
                foreach (var so in node.Children.FindAll(x => x.Name == "so"))
                {
                    string userGuid = null;
                    string obname = null;
                    string pos = null;
                    string rot = null;
                    string velo = null;
                    string avelo = null;

                    foreach (var i in so.Children)
                    {
                        var name = i.Name.ToLower();
                        switch (name)
                        {
                            case "ug":
                                userGuid = i.Value;
                                break;
                            case "g":
                                obname = i.Value;
                                break;
                            case "p":
                                pos = i.Value;
                                break;
                            case "r":
                                rot = i.Value;
                                break;
                            case "v":
                                velo = i.Value;
                                break;
                            case "av":
                                avelo = i.Value;
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(obname))// && (userGuid != DeviceManager.UserGuid || firstspass > 0))
                    {
                        firstspass--;
                        Transform tr = SceneGO.transform.FindDeepChild(obname);
                        if (tr != null)
                        {
                            Vector3 p;
                            if (TryVector3FromString(pos, out p))
                            {
                                tr.position = ScenePosition(p);
                            }
                            Quaternion q;
                            if (TryQuaternionFromString(rot, out q))
                            {
                                tr.rotation = q;
                            }
                            Rigidbody rb = tr.GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                Vector3 v;
                                if (TryVector3FromString(velo, out v))
                                {
                                    rb.velocity = v;
                                }
                                Vector3 av;
                                if (TryVector3FromString(avelo, out av))
                                {
                                    rb.angularVelocity = av;
                                }
                            }
                            SetServerObjectLastTransform(tr.gameObject, obname);
                        }

                    }


                }
            }
            catch (Exception) { }
        }
        void SetServerObjectLastTransform(GameObject go, string obname)
        {
            var sot = CreateServerObjectTransForm(go);
            serverGameObjectLastTransforms[obname] = sot;

        }
        void HandleActions(X3DParse.Node node)
        {
            int counter = 0;
            foreach( var ac in node.Children.FindAll(x=>x.Name=="ac"))
            {
                string goName = null;
                float sceneTime = 0;
                foreach(var a in ac.Children)
                {
                    string name = a.Name;
                    switch(name)
                    {
                        case "n":
                            goName = a.Value;
                            break;
                        case "t":
                            string t = a.Value;
                            float.TryParse(t, out sceneTime);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(goName) && sceneTime > 0)
                {
                    if (Actions.Count > 0) //own action
                    {
                        SceneAction act = Actions.Peek();
                        if (goName == act.GoName && Math.Abs(sceneTime - act.SceneTime) < 0.01f)//< 0.001f)
                        {
                            Actions.Dequeue();
                        }
                    }
                    else
                    {
                        //PerfomAction;
                        SceneAction act2 = new SceneAction(goName, sceneTime);
                        if (goName == Constants.startRecordingAction && ActionsHandled.Count == 0)//skip the initial 'start_recording' action
                        {
                            Actions.Clear();
                            ActionsToPerform.Clear();
                            ActionsHandled.Add(act2);
                            isListening = true;
                        }
                        else if (goName == Constants.endRecordingAction)//skip the initial 'start_recording' action
                        {
                            isListening = false;
                        }
                        else if (!ActionsHandled.Contains(act2) && !ActionsToPerform.Contains(act2))
                        {
                            ActionsToPerform.Enqueue(act2);
                        }
                    }
                }
                counter++;
            }
        }

        void HandleAvatarUser(X3DParse.Node node)
        {
            string id = null;
            string posS = null;
            string rotS = null;
            string nickN = null;
            string avatNo = null;
            string dissGuid = null;
            string hasLegacyAvatar = null;
            string lhp = null;
            string lhr = null;
            string rhp = null;
            string rhr = null;

            foreach (var child in node.Children)
            {
                string name = child.Name.ToLower();
                switch (name)
                {
                    case "p":
                        posS = child.Value;
                        break;
                    case "r":
                        rotS = child.Value;
                        break;
                    case "i":
                        id = child.Value;
                        break;
                    case "n":
                        nickN = child.Value;
                        break;
                    case "a":
                        avatNo = child.Value;
                        break;
                    case "d":
                        dissGuid = child.Value;
                        break;
                    case "l":
                        hasLegacyAvatar =  child.Value;
                        break;
                    case "lhp":
                        lhp = child.Value;
                        break;
                    case "lhr":
                        lhr = child.Value;
                        break;
                    case "rhp":
                        rhp = child.Value;
                        break;
                    case "rhr":
                        rhr = child.Value;
                        break;
                }
            }
            Vector3 pos;
            Quaternion rot;
            if (!string.IsNullOrEmpty(id) && TryVector3FromString(posS, out pos) && TryQuaternionFromString(rotS, out rot))
            {
                CheckAvatar(id, pos, rot, nickN, avatNo, dissGuid, hasLegacyAvatar, lhp, lhr, rhp, rhr);
            }
        }

        void ClearActions()
        {
            SceneTimer = 0;
            Actions.Clear();
            ActionsHandled.Clear();
            ActionsToPerform.Clear();
            isListening = false;
        }
 
        public string NickName
        {
            get
            {
                return DeviceManager.NickName;
            }
            set
            {
                DeviceManager.NickName = value;
            }
        }
        public string AvatarNo
        {
            get
            {
                return DeviceManager.AvatarNo;
            }
            set
            {
                DeviceManager.AvatarNo = value;
            }
        }

        public string GetRandomavatarName()
        {
            return "unknown" +((int)(1000*UnityEngine.Random.value));
        }
        GameObject avatarMenuParent;
        public void OpenAvatarMenu(string avatarId, string avatarDissId, string nickName, Vector3 position)
        {
            if (avatarId != DeviceManager.UserGuid)
            {
                CloseAvatarMenu();
                avatarMenuParent = new GameObject();
                avatarMenuParent.transform.SetTheParent(SceneGO.transform);
                var avMenuObj = GetPrefab(avatarMenuParent.transform, "slam", Constants.slam_avatar_menu_name);
                if (avMenuObj != null)
                {
                    if (IsHoloLens())
                    {
                        avMenuObj.transform.localScale *= 0.4f;
                    }
                    var avatarMenu = avMenuObj.GetComponentInChildren<avatarMenu>();
                    if (avatarMenu != null)
                    {
                        avatarMenu.AvatarDissId = avatarDissId;
                        avatarMenu.AvatarId = avatarId;
                        avatarMenu.NickName = nickName;
                        avatarMenu.transform.localPosition = position;
                        avatarMenu.gameObject.SetActive(true);
                        avatarMenu.presentation = DeviceManager.PresentationDistanceCorrection;
                        avatarMenu.presentationspeed = 10;
                        avatarMenu.SetPresenting();
                    }
                }
            }
        }
        public void CloseAvatarMenu()
        {
            if (avatarMenuParent != null)
            {
                Destroy(avatarMenuParent);
            }
        }
        public bool ToggleMute(string playerId)
        {
#if USE_DISS
            if(dissonanceComms!=null)
            {
                var pl = dissonanceComms.FindPlayer(playerId);
                if(pl!=null)
                {
                    pl.Volume = pl.Volume > 0 ? 0:1 ;
                    return pl.Volume == 0;
                }
            }
#endif
            return false;
        }
#endregion Avatars
    }
 
 
}