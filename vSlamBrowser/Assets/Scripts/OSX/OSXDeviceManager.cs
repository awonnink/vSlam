using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WSA || UNITY_EDITOR

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif
namespace Slam
{
    public class OSXDeviceManager : BaseDeviceManager, IDeviceManager
    {
        public OSXDeviceManager()
        {
            _presentationDistanceCorrection = 1f;
            mainMenuDistance = 0.66f;
            bundleExtension = BundleExtension._osx;
            name = CallingDevices.OSX.ToString();
            isUWP = false;
            isOpaque = true;
        }

    }
}
