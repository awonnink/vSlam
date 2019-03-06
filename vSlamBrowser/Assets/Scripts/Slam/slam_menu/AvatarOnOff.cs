using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class AvatarOnOff : SlamObject
    {
        Texture2D textureOn;
        Texture2D textureOff;
        public override void Start()
        {
            base.Start();
            textureOn = Resources.Load<Texture2D>("Slam/avatar_on");
            textureOff = Resources.Load<Texture2D>("Slam/avatar_off");
            SetMaterial();
        }
        public override void DoSelect(Vector3 v, bool checkActionRecording = false)
        {
            Communicator.Instance.AvatarOn = !Communicator.Instance.AvatarOn;
            Slam.Instance.EnableAvatars(Communicator.Instance.AvatarOn);
            SetMaterial();
            Slam.Instance.CloseMenu(0.7f);
        }
        void SetMaterial()
        {
            var texture = Communicator.Instance.AvatarOn ? textureOn : textureOff;
            Renderer rend = gameObject.GetComponent<Renderer>();
            Material mat = rend.material;
            mat.mainTexture = texture;
        }
        public override void StartGaze()
        {
            base.StartGaze();
            SlamMenu.Instance.SetHelpText("Toggle avatars function");
        }
        public override void EndGaze()
        {
            base.EndGaze();
            SlamMenu.Instance.SetHelpText("");
        }

    }
}
