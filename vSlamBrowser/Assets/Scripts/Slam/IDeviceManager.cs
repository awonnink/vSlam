using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public interface IDeviceManager
    {
        void SetToolTip(string toolTip);
        string UserGuid { get; }
        string Name { get; }
        float MainMenuDistance { get; }
        float PresentationDistanceCorrection { get; }
        string NickName { get; set; }
        string AvatarNo { get; set; }
        bool AvatarFunction { get; set; }
        bool IsUWP { get; }
        bool IsOpaque { get; }
        List<string> Favorites { get; }
        List<string> Histories { get; }
        List<StoredFormField> StoredFormFields { get; }
        BundleExtension BundleExtension { get; }
        string PresentationGuid { get; set; }

        void LoadData();
        void SaveData();
        void AssureStoredField(string url, string fieldName, string fieldValue);
        StoredFormField FindStoredFieldItem(string url, string fieldName);
        void AddFavorite(string url);
        void AddHistory(string url);
        void RemoveFavorite(string url);
    }
}
