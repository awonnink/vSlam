using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WSA || UNITY_EDITOR
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Slam
{
    public class BaseDeviceManager
    {
        protected SlamSettings slamSettings = new SlamSettings();
        protected string dataFileName = Constants.Settings_Datafile_Name;
        bool _avatarFunction = true;
        protected float _presentationDistanceCorrection = 1f;
        protected float mainMenuDistance = 0.66f;
        protected BundleExtension bundleExtension = BundleExtension._x64;
        protected string name = CallingDevices.PC.ToString();
        protected string userGuid = null;
        protected bool isUWP = false;
        protected bool isOpaque = false;
        public List<StoredFormField> StoredFormFields
        {
            get
            {
                if (slamSettings.StoredFieldItems == null)
                {
                    slamSettings.StoredFieldItems = new List<StoredFormField>();
                }
                
                return slamSettings.StoredFieldItems;
            }
        }
        virtual public void LoadData()
        {
#if !UNITY_WSA || UNITY_EDITOR
            string tfile = Application.persistentDataPath + "/" + dataFileName;
            if (File.Exists(tfile))
            {
                FileStream file = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(tfile, FileMode.Open);
                    slamSettings = (SlamSettings)bf.Deserialize(file);
                }
                catch (Exception x)
                {
                    Debug.Log(x.Message);
                }
                finally
                {
                    if (file != null)
                    {
                        file.Close();
                    }
                }

            }
#endif
        }
        public void AssureStoredField(string url, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(fieldName))
            {
                StoredFormField item = FindStoredFieldItem(url, fieldName);
                if (item != null)
                {
                    item.FieldName = fieldName.ToLower();
                    item.FieldValue = fieldValue;
                    SaveData();
                    return;
                }
                item = new StoredFormField();
                item.FieldName = fieldName.ToLower();
                item.FieldValue = fieldValue;
                item.Url = url.ToLower();
                StoredFormFields.Add(item);
                SaveData();

            }
        }
        public StoredFormField FindStoredFieldItem(string url, string fieldName)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var turl = UrlHandler.GetStrippedUrl(url);
                return StoredFormFields.Find(x => x.FieldName == fieldName.ToLower() && x.Url == turl.ToLower());
            }
            return null;
        }
        public void AddFavorite(string url)
        {
            if (!slamSettings.Favorites.Contains(url.ToLower()))
            {
                slamSettings.Favorites.Add(url.ToLower());
                SaveData();
            }
        }
        public void AddHistory(string url)
        {
            if (slamSettings!=null && slamSettings.Histories != null && !url.ToLower().Contains("favorites.aspx"))
            {
                if (slamSettings.Histories.Count > 16)
                {
                    slamSettings.Histories.RemoveAt(0);
                }
                if (!slamSettings.Histories.Contains(url.ToLower()))
                {
                    slamSettings.Histories.Add(url.ToLower());
                    SaveData();
                }
            }
        }
        public void RemoveFavorite(string url)
        {
            if (slamSettings.Favorites.Contains(url.ToLower()))
            {
                slamSettings.Favorites.Remove(url.ToLower());
                SaveData();
            }
        }
        virtual public void SaveData()
        {
#if !UNITY_WSA || UNITY_EDITOR 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + dataFileName);
            bf.Serialize(file, slamSettings);
            file.Close();
#endif
        }
        public List<string> Favorites
        {
            get
            {
                return slamSettings.Favorites;
            }
        }
        public List<string> Histories
        {
            get
            {
                if(slamSettings.Histories==null)
                {
                    slamSettings.Histories = new List<string>();
                }
                return slamSettings.Histories;
            }
        }
        public string NickName
        {
            get
            {
                if (string.IsNullOrEmpty(slamSettings.NickName))
                { slamSettings.NickName = Slam.Instance.GetRandomavatarName(); }
                return slamSettings.NickName;
            }

            set
            {
                slamSettings.NickName = value;
                SaveData();
            }
        }
        public string AvatarNo
        {
            get
            {
                if (string.IsNullOrEmpty(slamSettings.AvatarNo))
                { slamSettings.AvatarNo = "0"; }
                return slamSettings.AvatarNo;
            }

            set
            {
                slamSettings.AvatarNo = value;
                SaveData();
            }
        }
        public bool AvatarFunction
        {
            get
            {
                return _avatarFunction;
            }

            set
            {
                _avatarFunction = value;
            }
        }
        public string PresentationGuid
        {
            get
            {
                return slamSettings.PresentationGuid;
            }

            set
            {
                slamSettings.PresentationGuid = value;
                SaveData();
            }
        }
        public float PresentationDistanceCorrection
        {
            get
            {
                return _presentationDistanceCorrection;
            }
        }
        public float MainMenuDistance
        {
            get
            {
                return mainMenuDistance;
            }
        }
        public BundleExtension BundleExtension
        {
            get
            {
                return bundleExtension;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string UserGuid
        {
            get
            {
                if (string.IsNullOrEmpty(slamSettings.UserGuid))
                {
                    slamSettings.UserGuid = System.Guid.NewGuid().ToString();
                    SaveData();
                }
#if UNITY_EDITOR
                return slamSettings.UserGuid + "x";
#endif
                return slamSettings.UserGuid;
            }
        }
        public bool IsUWP
        {
            get
            {
                return isUWP;
            }
        }
        public bool IsOpaque
        {
            get
            {
                return isOpaque;
            }
        }
        public void SetToolTip(string toolTip)
        {
            if (ToolTip.Instance != null)
            {
                ToolTip.Instance.SetTip(toolTip);
            }
        }
    }

}