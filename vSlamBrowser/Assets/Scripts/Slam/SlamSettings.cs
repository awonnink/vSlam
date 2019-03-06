using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    [System.Serializable]
    public class SlamSettings
    {
        public SlamSettings()
        {
            Favorites = new List<string>();
            Histories = new List<string>();
            StoredFieldItems = new List<StoredFormField>();
        }
        public string UserGuid { get; set; }
        public string NickName { get; set; }
        public string AvatarNo { get; set; }
        public string PresentationGuid { get; set; }
        public List<string> Favorites { get; set; }
        public List<string> Histories { get; set; }
        public List<StoredFormField> StoredFieldItems { get; set; }
    }
    [System.Serializable]
    public class StoredFormField
    {
        public string Url { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
