using System;
using UnityEngine;
using Newtonsoft;
#if UNITY_WSA && !UNITY_EDITOR
using System.Threading.Tasks;
#endif
namespace Slam
{
    public class HLDeviceManager : BaseDeviceManager, IDeviceManager
    {

        float mainMenuDistanceHL = 2;
        float mainMenuDistanceMR = 1.2f;
        float _presentationDistanceCorrectionHL = 2.5f;
        float _presentationDistanceCorrectionMR = 1f;
        Vector3 cameraOfsetHL = new Vector3(-0.27f, 0.27f, 2);
        Vector3 cameraOfsetMR = new Vector3(-0.35f, 0.7f, 2);
        public HLDeviceManager()
        {
            bundleExtension = BundleExtension._uwp;
            name = CallingDevices.HoloLens.ToString();
            isUWP = true;
        }

        public override void SaveData()
        {
 //           string sl = JsonUtility.ToJson(slamSettings);
            
#if UNITY_WSA && !UNITY_EDITOR
            SaveFile();
#endif

        }
#if UNITY_WSA && !UNITY_EDITOR
        async void SaveFile()
        {
            try
            {
                string content = null;
                await Task.Factory.StartNew(() => content = Newtonsoft.Json.JsonConvert.SerializeObject(slamSettings));
                if (content != null)
                {
                    Windows.Storage.StorageFolder storageFolder =
                        Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile dataFile =
                        await storageFolder.CreateFileAsync(dataFileName,
                            Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteTextAsync(dataFile, content);
                }
            }
            catch (Exception x) { }
        }
#endif
        override public void LoadData()
        {
#if UNITY_WSA && !UNITY_EDITOR
            try
            {

                var localSlamSettings = LoadFile();
                if (localSlamSettings != null)
                {
                    slamSettings = localSlamSettings;
                }
            }
            catch (Exception) { };
            if(slamSettings==null)
            {
                slamSettings = new SlamSettings();
            }
            if( isOpaque)
            {
                name = CallingDevices.MR.ToString();
            }
#endif

        }
        public new string Name
        {
           
            get
            {
#if UNITY_WSA// && !UNITY_EDITOR
                ;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                isOpaque = UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque;
#else
                isOpaque = true;
#endif
                if (IsOpaque)
                {
                    name = CallingDevices.MR.ToString();
                }
#endif
                return name;
            }
        }
#if UNITY_WSA && !UNITY_EDITOR
        SlamSettings LoadFile()
        {
            SlamSettings sl = null;
            string ret = null;
            Task<Task> task = Task<Task>.Factory.StartNew(
                            async () =>
                            {
                                try
                                {
                                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                                    //C:\Users\awonn\AppData\Local\Packages\19034vSlam.org.vSlam3DPro_1h1swmnfvff8a\LocalState
                                    var f = await storageFolder.TryGetItemAsync(dataFileName);
                                    if (f != null)
                                    {
                                        Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(dataFileName);
                                        ret = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                                        await Task.Factory.StartNew(() => sl = Newtonsoft.Json.JsonConvert.DeserializeObject<SlamSettings>(ret));
                                    }
                                }
                                catch (Exception x) { }
                            });
            task.Wait();
            task.Result.Wait();
   
            return sl;
        }
#endif
        public new float MainMenuDistance
        {
            get
            {
                return isOpaque ? mainMenuDistanceMR : mainMenuDistanceHL;
            }
        }

        public new float PresentationDistanceCorrection
        {
            get
            {
                return isOpaque ? _presentationDistanceCorrectionMR : _presentationDistanceCorrectionHL;
            }
        }
        public new void SetToolTip(string toolTip)
        {
            if (ToolTip.Instance != null)
            {
                Vector3 co = isOpaque ? cameraOfsetMR : cameraOfsetHL;
                ToolTip.Instance.SetTip(toolTip, co);
            }
        }

    }

}
