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
    public class DefaultDeviceManager : BaseDeviceManager,  IDeviceManager
    {
        public DefaultDeviceManager()
        {
            _presentationDistanceCorrection = 1f;
            mainMenuDistance = 0.66f;
            bundleExtension = BundleExtension._x64;
            name= CallingDevices.PC.ToString();
            isUWP = false;
            isOpaque = true;
        }


    }
}
