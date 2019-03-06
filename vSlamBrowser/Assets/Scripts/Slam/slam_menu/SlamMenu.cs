using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;

namespace Slam
{
    public class SlamMenu : Singleton<SlamMenu>, IDictationHandler
    {
        public TextMeshPro externalText = null;
        public InputType InputType = InputType.Text;
        TextMesh helpText = null;
        TextMeshPro urlText = null;
        TextMesh infoText = null;
        TextMesh instructionText = null;
        AudioSource aso;
        public string typedText = "";
        int typeIndex = 0;
        int maxCharInField = 40;
        bool blinkon = false;
        MenuInputType _menuInputType = MenuInputType.Url;
        Transform kb_normal;
        Transform kb_caps;
        bool kb_isnormal = true;
        public Vector3 targetPostion = new Vector3();
        Vector3 targetScale = 0.3f * Vector3.one;
        float animationSpeed = 6;
        GameObject largetextbackground = null;
        public MenuInputType MenuInputType
        {
            get
            {
                return _menuInputType;
            }
            set
            {
                _menuInputType = value;
                SetInstructionText();
            }
        }
        // Use this for initialization
        void Start()
        {
            var ltbt = transform.FindDeepChild("largetextbackground");
            if (ltbt != null)
            {
                largetextbackground = ltbt.gameObject;
                largetextbackground.SetActive(false);
            }
            kb_normal = transform.Find("slam_keyboard");
            kb_caps = transform.Find("slam_keyboard_caps");
            kb_caps.gameObject.SetActive(false);
            foreach (var mesh in GetComponentsInChildren<TextMesh>())
            {
                switch (mesh.gameObject.name)
                {
                    case "slam_urltext":
                       // urlText = mesh;
                        break;
                    case "slam_helptext":
                        helpText = mesh;
                        break;
                    case "slam_instructiontext":
                        instructionText = mesh;
                        break;
                    case "slam_infotext":
                        infoText = mesh;
                        break;
                }
            }
            urlText = GetComponentInChildren<TextMeshPro>();
            aso = GetComponent<AudioSource>();
            SetInstructionText();
            SetInfoText();
            InvokeRepeating("DoSetUrlText", 0, 0.4f);
        }

        public void SwitchKeyBoard()
        {
            kb_isnormal = !kb_isnormal;
            kb_caps.gameObject.SetActive(!kb_isnormal);
            kb_normal.gameObject.SetActive(kb_isnormal);
        }
        void SetInfoText()
        {
            if(infoText!=null)
            {
                infoText.text = Slam.Instance.GetInfo();
            }
        }
        public void HandleKey(string key, bool isRemote=false, int sequence = 0)
        {
            if (!isRemote)
            {
                if (key != Constants.slam_upper && key != Constants.slam_lower)
                {
                    //if (key == "slam_backspace") { key = "back"; }
                    Slam.Instance.Typing(key, typeIndex);
                }
            }
            switch (key)
            {
                case Constants.slam_upper:
                case Constants.slam_lower:
                    SwitchKeyBoard();
                    break;
                case Constants.slam_left:
                    if (typeIndex > 0)
                    {
                        typeIndex--;
                    }
                    break;
                case Constants.slam_right:
                    if (typeIndex <typedText.Length)
                    {
                        typeIndex++;
                    }
                    break;
                case Constants.slam_clear:
                    typedText = "";
                    typeIndex = 0;
                    break;
                case Constants.slam_enter:
                    Parse();
                    break;
                case Constants.slam_backspace:
                    if (typeIndex > 0 && typedText.Length>=typeIndex)
                    {
                        string tttext = typedText.Substring(0, typeIndex-1);
                        typeIndex--;
                        if (typeIndex+1 < typedText.Length)
                        {
                            tttext += typedText.Substring(typeIndex+1);
                        }
                        typedText = tttext;
                    }
                    break;
                default:
                    int l = key.Length;
                    string ttext = typedText.Substring(0, typeIndex) + key;
                    if(typeIndex<typedText.Length)
                    {
                        ttext += typedText.Substring(typeIndex);
                    }
                    typeIndex+=l;
                    typedText = ttext;
                    break;
            }
            Typespace.Typer.lasthandledsequence = sequence;
            if(externalText!=null && InputType != InputType.Community)
            {
                if (InputType == InputType.Password)
                {
                    externalText.text = new string('*', typedText.Length);
                }
                else
                {
                    externalText.text = typedText;
                }
                Slam.Instance.SaveTextInput(typedText, false);
            }
            aso.Play();
        }
        public void Parse()
        {
            Slam.Instance.StopTyping();
            switch (MenuInputType)
            {
                case MenuInputType.Url:
                    Slam.Instance.Parse(typedText, Target._blank);
                    break;
                case MenuInputType.NickName:
                    Slam.Instance.NickName = typedText;
                    break;
                case MenuInputType.Email:
                    //Slam.Instance.NickName = typedText;
                    break;
                case MenuInputType.Input:
                    if (InputType == InputType.Community)
                    {
                       if (typedText.Length < Constants.CommunityInputMaxLenth)
                        {
                            Slam.Instance.SaveCommunityInput(typedText);
                        }
                        else
                        {
                            instructionText.text=string.Format("Text should not exceed {0} chars!", Constants.CommunityInputMaxLenth);
                            return;
                        }
                    }
                    else
                    {
                        Slam.Instance.SaveTextInput(typedText, true);
                    }
                    break;
            }

            Slam.Instance.CloseMenu();
            aso.Play();
        }

        public void SetHelpText(string text)
        {
            if (helpText != null)
            {
                helpText.text = text;
            }
        }
        public void SetAsHome()
        {
            var idx = Slam.Instance.UrlHandler.CurrentPageUrl.LastIndexOf("/");

            string page = idx>-1? Slam.Instance.UrlHandler.CurrentPageUrl.Substring(idx): Slam.Instance.UrlHandler.CurrentPageUrl;
            if (typedText.ToLower() == Slam.Instance.UrlHandler.CurrentPageUrl.ToLower() || typedText.ToLower() == Slam.Instance.UrlHandler.BaseUrl.ToLower()+ page.ToLower())
            {
                Slam.Instance.UrlHandler.HomeUrl = typedText;
            }
        }
        void SetInstructionText()
        {
            string txt = "";
            switch(_menuInputType)
            {
                case MenuInputType.Url:
                    txt = "Search term or URL:";
                    break;
                case MenuInputType.Email:
                    txt = "Enter your email adress:";
                    break;
                case MenuInputType.NickName:
                    txt = "Enter your nickname:";
                    break;
            }
            instructionText.text = txt;
        }
        public void PasteInputText(string text)
        {
            foreach(var key in text)
            {
                HandleKey( key.ToString());
            }
            //SetInputText(text, true, MenuInputType, InputType);
            //Slam.Instance.SaveTextInput(typedText, false);
        }
        public void SetInputText(string text, bool init=true, MenuInputType menuInputType= MenuInputType.Url, InputType inputType= InputType.Text)
        {
            text = text == null ? "" : text;
            if (init)
            {
                ////check for existing input
                //if(MenuInputType!= MenuInputType.Url && !string.IsNullOrEmpty(typedText))
                //{
                //    Parse(false);
                //}
                typedText = text;
                typeIndex = text.Length;
                MenuInputType = menuInputType;
                InputType = inputType;
                //StartExternalTyping();
            }
        }
        public void StartExternalTyping()
        {
            Slam.Instance.StartTyping(typedText);
        }
        void DoSetUrlText()
        {
            if(typedText!=null)
            {
                string txt = "";
                if(blinkon)
                {
                    txt = typedText.Substring(0, typeIndex) + "|" + typedText.Substring(typeIndex);
                }
                else
                {
                    txt = typedText;
                }
                var urltxt = InputType == InputType.Password ? new string('*', txt.Length) : txt;
                var skipChar = typeIndex-maxCharInField;
                //while(typeIndex>skipChar+ maxCharInField+4)
                //{
                //    skipChar += maxCharInField;
                //}
                //if (skipChar > 0)
                //{
                //   // urltxt = urltxt.Substring(skipChar);
                //}
                //var checkLength = blinkon ? maxCharInField + 1 : maxCharInField;
                //if (urltxt.Length > checkLength)
                //{
                //    urltxt = urltxt.Substring(0, checkLength)+ " ...";
                //}
                urlText.text = urltxt;
                if(largetextbackground!=null &&!blinkon)
                {
                    if (largetextbackground.activeSelf != urlText.isTextOverflowing)
                    {
                        largetextbackground.SetActive(urlText.isTextOverflowing);
                    }
                        //Vector3[] lst = new Vector3[8];
                        //urlText.bounds.GetCornerPositionsFromRendererBounds(ref lst);
                        //var t = lst;
                }
                
                  blinkon = !blinkon;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                HandleKey(Constants.slam_left);
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                HandleKey(Constants.slam_right);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                HandleKey(Constants.slam_enter);
            }
            else
            {
                if (Input.anyKey)
                {
                    var chr = Input.inputString;
                    switch (chr)
                    {
                        case "\b":
                            chr = Constants.slam_backspace;
                            break;
                        case "\r":
                            chr = Constants.slam_enter;
                            break;
                    }
                    if (chr.Length>0)
                    {
                        HandleKey(chr);
                    }
                }
            }
            if (Slam.Instance != null)
            {
                animationSpeed = Slam.Instance.IsHoloLens() ? 1 : 6;
            }

            if (Vector3.Distance(transform.position, targetPostion)>0.04)
            {
                transform.position = Vector3.Lerp(transform.position, targetPostion, animationSpeed * Time.deltaTime);
            }
            if(Vector3.Distance(transform.localScale, targetScale)>0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
            }
            transform.LookAt(Camera.main.transform.position);
        }
        //System.Text.StringBuilder textSoFar = new System.Text.StringBuilder();
        string starttext = "";
        bool textSoFarInitiated = false;
        public void CheckTextSoFarInitiated()
        {
            if (!textSoFarInitiated)
            {
                starttext = typedText;
                textSoFarInitiated = true;
            }
        }
        public void OnDictationHypothesis(DictationEventData eventData)
        {
            CheckTextSoFarInitiated();
            DisplayText(starttext + " " + eventData.DictationResult + "...", false);
        }

        internal void Type(string key, int cursor, int sequence)
        {
            typeIndex = cursor;
            HandleKey(key, true, sequence);
        }

        public void OnDictationResult(DictationEventData eventData)
        {
            CheckTextSoFarInitiated();

            // 3.a: Set DictationDisplay text to be textSoFar
            DisplayText(starttext + " " + eventData.DictationResult, true);
        }
        public void OnDictationComplete(DictationEventData eventData)
        {
            textSoFarInitiated = false;
    //        textSoFar.Append(eventData.DictationResult);// + ". ");

            // 3.a: Set DictationDisplay text to be textSoFar
            DisplayText(starttext + " " + eventData.DictationResult, true);
            textSoFarInitiated = false;
            Slam.Instance.stopRecording();
        }

        public void OnDictationError(DictationEventData eventData)
        {
            if(eventData.DictationResult.Contains("-2147199735"))
            {
                Slam.Instance.ShowMessage(true, "Dictation support is not enabled on this device (see 'Get to know me' in Settings > Privacy > Speech, inking, & typing)");
                //ERROR:  Dictation support is not enabled on this device (see 'Get to know me' in Settings > Privacy > Speech, inking, & typing)
                //HRESULT: -2147199735
            }
            textSoFarInitiated = false;
            Slam.Instance.stopRecording();
        }
        private void DisplayText(string text, bool checkForAction = false)
        {
            if(text.ToLower().Contains(Constants.Speech_Submit_Text))
            {
                int pos = text.IndexOf(Constants.Speech_Submit_Text);
                text = text.Substring(0, pos) + text.Substring(pos + Constants.Speech_Submit_Text.Length);
                Slam.Instance.SubmitText();
            }
            SetInputText(text, true, _menuInputType, InputType);
        }
        protected virtual void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }
        protected virtual void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

    }
    public enum MenuInputType
    {
        Url,
        Email,
        NickName,
        Input
    }
}
