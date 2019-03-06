using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Slam
{
    public class avatarMenu : SlamObject
    {
        public string AvatarId;
        public string AvatarDissId;
        public string NickName;
        avatarMenuMute menuMute;
        TMPro.TextMeshPro avatarName;
        GameObject mutelabel;
        GameObject mutebutton;
        bool isMuted = false;
        // Use this for initialization
        public override void Start()
        {
            menuMute = GetComponentInChildren<avatarMenuMute>();
            menuMute.AvatarMenu = this;
            avatarName = transform.FindDeepChild("slam_av_name").GetComponent<TextMeshPro>();
            mutelabel = transform.FindDeepChild("slam_av_mute_lbl").gameObject;
            mutebutton = transform.FindDeepChild("slam_av_mute").gameObject;

        }

        public override void Update()
        {
            base.Update();
            if(!string.IsNullOrEmpty(NickName) && avatarName.text!=NickName)
            {
                avatarName.text = NickName;
            }
        }
        public bool ToggleMute()
        {
            return Slam.Instance.ToggleMute(AvatarDissId);
        }
        public void ShowMuteOption(bool show)
        {
            mutelabel.SetActive(show);
        }
    }
}
