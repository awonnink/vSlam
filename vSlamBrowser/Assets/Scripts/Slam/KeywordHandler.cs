using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace Slam
{
    public class KeywordHandler : MonoBehaviour, ISpeechHandler
    {
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            switch(eventData.RecognizedText)
            {
                case "go home":
                    Slam.Instance.GoHome();
                    break;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
