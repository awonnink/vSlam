using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    static class Constants
    {
        public const string SlamVersion = "2.42";
        public const string Slamkey = "ndfhdsfUSCcsaaffbg743&^56d77qw712";
        public const string BaseHomeUrl = "https://www.v-slam.org/";
        public const string BaseHomeUrlPrev = "https://www.v-slam.org/prev/";
        public const string DevelopBaseHomeUrl = "http://localhost:56319/";
        public const string TyperUrl = "https://www.v-slam.org/sharescreenservice/photos";
        public const string DeveloperTyperUrl = "http://localhost:2557/photos";

        public const string SoundServer = "37.97.189.248";
        public const string NetworkManager = "NetworkManager";
        public const string DissonanceSetup = "DissonanceSetup";
        public const int SizeInBytesLevel = 22000000;

        //        public const string SoundServer = "127.0.0.1";
        public const string SlamSettingsKey = "slamSettingsKey";
        public const string RH_Timezone_Offset = "TimeZone-Offset";
        public const string RH_Calling_Device = "Calling-Device";
        public const string RH_USER_AGENT = "USER-AGENT";

        //public const string DeviceName_Default = "Default Application";
        //public const string DeviceName_HoloLens = "HoloLens";
        //public const string DeviceName_MR = "MR";
        // public const string DeviceName_OSX = "OSX";

        public const string Speech_Submit_Text = "enter text";
        public const string Speech_Open_Menu = "open menu";
        public const string Speech_Go_Home = "go home";


        public const string Settings_Datafile_Name = "slam_settings.gd";

        public const string LayerNameMainCamHidden = "MainCamHidden";
        public const string slam_version_text = "version {0}";
        public const string fld_login = "fld_login";
        public const string fld_password = "fld_password";
        public const string cameraMR = "CameraMR";
        public const string cameraPC = "CameraPC";
        public const string slam_menu_name = "slam_menu";
        public const string slam_avatar_menu_name = "slam_avatar_menu";
        public const string slam_notification = "slam_notification";
        

        public const string slam_waitCursor_name = "slam_waitCursor";
        public const string slam_msgbox_name = "slam_msgbox_name";
        public const string scene_gameobject_name = "scene";
        public const string rootGo_gameobject_name = "rootGO";
        public const string startRecordingAction = "slam_start_recording";
        public const string endRecordingAction = "slam_end_recording";
        public const string slam_sessionfld = "slam_session";
        public const string slam_presentation_guid = "slam_presentation_guid";
        public const string slam_userguidfld = "slam_userguid";
        public const string slam_notification_prefab = "slam_notification";
        public const string slam_backspace="slam_backspace";
        public const string slam_enter="slam_enter";
        public const string slam_clear = "slam_clear";
        public const string slam_upper = "slam_upper";
        public const string slam_lower = "slam_lower";
        public const string slam_left = "slam_left";
        public const string slam_right = "slam_right";

        public const string slam_prefabgroup_various = "various";

        public const string returnPageCodefld = "fld_returnPageCode";
        public const string slam_WalkFloorIndicator = "slam_WalkFloorIndicator";
        public const string slam_standard_shader_name = "Standard";
        public const string slam_standard_specular_shader_name = "Standard (Specular setup)";
        public const string slam_standard_occluded_shader_name = "MixedRealityToolkit/Occlusion";
        
        public const string slam_ok_message = "#slam_ok_message";
        public const string slam_close_message = "#slam_close_message";
        public const string slam_menu = "#slam_menu";
        public const string slam_home = "#slam_home";
        public const string slam_msgBox_OK_btn = "okButton";
        public const string slam_msgBox_Cancel_btn = "cancelButton";
        public const string legacy_avatar_name = "avatar";
        public const string avatar_neck_bone_name = "slam:neck";
        public const string clearTransformTag = "slam_clean";
//        public const string communityPostsGo = "slam_communityposts";



        public const string avatar_walk_trigger = "walk";
        public const string avatar_idle_trigger = "idle";
        public const string avatar_sit_trigger = "sit";

        public static string callingDeviceHeader = "Unity Editor";
        public const string speechIndicatorName= "speechIndicator";

        public const string primitivesGroup = "primitives";
        public const string spherePrefabName = "sphere";
        public const string arrowPrefabName = "arrow1";
        public const string conePrefabName = "cone";
        public const string cylinderPrefabName = "cylinder";

        public const float cameraHeight = 1.65f;
        public const float cameraHeightSitting = 0.9f;
        public const float mrCameraCheckTime = 2f;

        
        public const float mouseHitDistance = 40f;
        internal static string resetActions="slam_reset_actions";

        public static int CommunityInputMaxLenth = 200;
        public static string protocolCode = "x3dx://";
        public static string httpsCode = "https://";
        public static string uriIdentifier = "uri=";
    }
}
