using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public enum FogTypeEnum
    {
        linear,
        exponential,
        exponentialsquared
    }
    public enum PlayerActivatedType
    {
        Unknown,
        Speak,
    }
    enum ParentNodeType
    {
        Element,
        Shape
    }
    public enum DownloadType
    {
        Xml,
        Texture,
        Video,
        Mesh,
        AssetBundle
    }
    public enum InputType
    {
        None,
        Text,
        Password,
        Date,
        DateTime,
        Number,
        Community
    }
    public enum MessageOkType
    {
        None,
        Close,
        DownloadAssets,
    }
    public enum MetaType
    {
        none,
        keywords,
        page,
        filter,
        singleuser,
        allowrecording,
        autostartrecord
    }
    public enum BundleExtension
    {
        _x64,
        _uwp,
        _osx,
    }
    public enum CallingDevices
    {
        PC,
        Unity_Editor,
        HoloLens,
        MR,
        OSX
    }
    public enum SoundRecordState
    {
        NotSupported,
        Recording,
        RecordingStopped,
    }
    public enum RecordingState
    {
        NotSupported,
        Recording,
        RecordingStopped,
        Listening
    }
    public enum ColliderType
    {
        NotSet,
        None,
        Sphere,
        Box,
        Capsule,
    }
    public enum ListType
    {
        standard,
        simple
    }
    public enum ConstraintType
    {
        notset,
        sphere,
        block,
        scale,
        delete,
        snap,
        constraintgroup,
        scaledsphere,
        selectfar,
        selectclose
    }
    public enum LateGameObjectType
    {
        ShapeNode,
        TreeView,
        ListView
    }
}
