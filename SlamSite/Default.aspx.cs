using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SlamSiteBase;
using SlmControls;



public partial class _Default : SlamSiteBase.BasePageBase
{
    float latestSlamVersion = 1.1f;
    float holoScale = 0.4f;
    bool firstTime = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        ReadRequestHeaders();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        X3DPage page = new X3DPage();
        X3DMeta meta = new X3DMeta();
        meta.MetaType = MetaType.keywords;
        meta.Content = "X3D, v-slam, information, browser";
        page.Head.MetaTags.Add(meta);
        X3DMeta metasingleUser = new X3DMeta();
        metasingleUser.MetaType = MetaType.singleuser;
        metasingleUser.Content = "true";
        page.Head.MetaTags.Add(metasingleUser);
        //page
        X3DMeta metaPage = new X3DMeta();
        metaPage.MetaType = MetaType.page;
        metaPage.Content = "0";
        page.Head.MetaTags.Add(metaPage);
        X3DMeta metaFilter = new X3DMeta();
        metaFilter.MetaType = MetaType.filter;
        metaFilter.Content = "";
        page.Head.MetaTags.Add(metaFilter);

        page.ViewPoint.DEF = "ViewUpClose";
        page.ViewPoint.Position = new Vector3(0, 1, -3);
        var now = DateTime.UtcNow.AddHours(1 + Timezone_Offset);

        if (CallingApp == CallingDevices.HoloLens)
        {
            page.ViewPoint.Position = new Vector3(0, 1, -3);
        }
        if (CallingApp != CallingDevices.HoloLens)
        {
            X3DBackGround backGround = new X3DBackGround();
            backGround.Name = "skyBox";
            backGround.SkyBox = "sky5X4";
            backGround.LightingMode = LightingMode.SkyBox;
            backGround.ReflectionMode = ReflectionMode.SkyBox;
            page.Scene.BackGround = backGround;
        }
        X3DFog fog = new X3DFog();
        page.Scene.AddChild(fog);
        fog.Density = 0;
        X3DGroup group = new X3DGroup();
        page.Scene.AddChild(group);
        double intens = 0.5;

        X3DTransform dLightt2 = X3DTransform.AddTransFormWithShape(ShapeType.Empty);
        group.AddChild(dLightt2);
        dLightt2.EulerRotation = new Vector3(140, 180, 0);
        X3DLight light2 = new X3DLight();
        light2.LightType = LightType.DirectionalLight;
        light2.Intensity = intens;
        dLightt2.AddChild(light2);

        if (CallingApp != CallingDevices.HoloLens)
        {
            X3DTransform environment = new X3DTransform();
            group.AddChild(environment);
            environment.Name = "environment";
            X3DInline inline = new X3DInline();
            environment.AddChild(inline);
            inline.Url = "exp/cliffhouse/cliffhouse.x3dx";
        }
        X3DTransform navigationGroup = X3DTransform.AddTransFormWithShape(ShapeType.Empty, new Vector3(0, 1.477, 0));
        group.AddChild(navigationGroup);
        navigationGroup.Name = "NavigationGroup";
        if (CallingApp == CallingDevices.HoloLens)
        {
            navigationGroup.Scale = new Vector3(holoScale);
        }
            
        //add transform earth
        X3DTransform transform = X3DTransform.AddTransFormWithShape(ShapeType.Sphere, new Vector3(), new Quaternion(0, 1, 0, 3), Vector3.One(0.3));
        navigationGroup.AddChild(transform);
        transform.Shape.Appearance.Material.DEF = "MaterialLightBlue";
        transform.Shape.Appearance.Material.DiffuseColor = new Vector3(0.1, 0.5, 1);
        transform.Shape.Appearance.Material.EmissiveColor = new Vector3(0, 0, 0.2);
        transform.Shape.Appearance.ImageTexture.Url = "images/earth-topo.png";
        transform.Shape.Appearance.Movement.Rotate = new Vector3(0, -1, 0);
        transform.Shape.Url = "#slam_menu";
        transform.Shape.ToolTip = "Select to open main menu";

        //text
        X3DTransform transform4 = X3DTransform.AddTransFormWithShape(ShapeType.Text, new Vector3(0, -0.3, 0), null,Vector3.One(0.03));
        navigationGroup.AddChild(transform4);
        //transform4.Translation = "0 0.855 0";
        transform4.Shape.Appearance.Material.USE = "MaterialLightBlue";
        transform4.Shape.Text = "\"v-Slam\" \"browse the world!\"";
        transform4.Shape.FaceCamera = FaceCamera.back_lock_y;
        transform4.Shape.FontStyle.Justify = Justify.MIDDLE;      
 
        //infoHolder
        X3DTransform transformInfoHolder = X3DTransform.AddTransFormWithShape(ShapeType.Empty, new Vector3(0, 0.685, -1));
        transformInfoHolder.Name = "infoHolder";
        if(CallingApp== CallingDevices.HoloLens)
        {
            transformInfoHolder.Scale = new Vector3(0.4);
            transformInfoHolder.Translation = new Vector3(0, 1.2, -1);
        }
        group.AddChild(transformInfoHolder);
   
        X3DTransform light1 = X3DTransform.AddTransFormWithShape(ShapeType.Empty);
        group.AddChild(light1);

        light1.EulerRotation = new Vector3(40, -28, -20);
        light1.Name = "light1";
        X3DLight light = new X3DLight();
        light1.AddChild(light);
        
        light.LightType = LightType.DirectionalLight;
        light.Intensity = 0.3;
        if(CallingApp== CallingDevices.HoloLens)
        {
            light.Intensity = 1.5;
        }
        light.ShadowIntensity = 1.0;
        if (CallingApp == CallingDevices.HoloLens)
        {

            X3DTransform pl = new X3DTransform();
            group.AddChild(pl);
            pl.Translation = new Vector3(-2.4, 3.17, -2.19);
            X3DLight light3 = new X3DLight();
            pl.AddChild(light3);
            light3.LightType = LightType.PointLight;
            light3.Direction = new Vector3(-230, -115, 70);
            light3.Intensity = 3.0;
            light3.ShadowIntensity = 1.0;

            //arrows
            X3DTransform arrows = new X3DTransform();
            group.AddChild(arrows);
            arrows.Translation = new Vector3(0, 1, 0);
            X3DTransform arr1 = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            arrows.AddChild(arr1);
            arr1.Shape.Group = "primitives";
            arr1.Shape.Item = "roundedarrow";
            arr1.Translation = new Vector3(2, 0, -2);
            arr1.Shape.Appearance.Material.DEF = "arcMat";
            arr1.Shape.Appearance.Material.DiffuseColor = new Vector3(0.3, 0.3, 0.7);
            arr1.Shape.Appearance.Material.Transparency = 0.7;
            arr1.EulerRotation = new Vector3(0, -90, 0);
            arr1.Scale = new Vector3(1.22, 0.7, 0.7);

            X3DTransform arr2 = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            arrows.AddChild(arr2);
            arr2.Shape.Group = "primitives";
            arr2.Shape.Item = "roundedarrow";
            arr2.Translation = new Vector3(-2, 0, -2);
            arr2.Shape.Appearance.Material.USE = "arcMat";
            arr2.Shape.Appearance.Material.DiffuseColor = new Vector3(0.3, 0.3, 0.7);
            arr2.Shape.Appearance.Material.Transparency = 0.7;
            arr2.EulerRotation = new Vector3(180, -90, 0);
            arr2.Scale = new Vector3(1.22, 0.7, 0.7);

            X3DTransform arr3 = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            arrows.AddChild(arr3);
            arr3.Shape.Group = "primitives";
            arr3.Shape.Item = "roundedarrow";
            arr3.Translation = new Vector3(-1, 0, -5);
            arr3.Shape.Appearance.Material.USE = "arcMat";
            arr3.Shape.Appearance.Material.DiffuseColor = new Vector3(0.3, 0.3, 0.7);
            arr3.Shape.Appearance.Material.Transparency = 0.7;
            arr3.EulerRotation = new Vector3(180, -120, 0);
            arr3.Scale = new Vector3(1.22, 0.7, 0.7);
            X3DTransform arr4 = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            arrows.AddChild(arr4);
            arr4.Shape.Group = "primitives";
            arr4.Shape.Item = "roundedarrow";
            arr4.Translation = new Vector3(1, 0, -5);
            arr4.Shape.Appearance.Material.USE = "arcMat";
            arr4.Shape.Appearance.Material.DiffuseColor = new Vector3(0.3, 0.3, 0.7);
            arr4.Shape.Appearance.Material.Transparency = 0.7;
            arr4.EulerRotation = new Vector3(0, -60, 0);
            arr4.Scale = new Vector3(1.22, 0.7, 0.7);
        }

        X3DSound sound = new X3DSound();
        page.Scene.AddChild(sound);
        sound.Url = "\"sound/soft.wav\"";
        sound.Loop = true;
        sound.Enabled = true;
        sound.Volume = 0.0;
        page.Render(sb);
        Response.Clear();
        Response.ContentType = "text/xml";
        Response.Write( sb.ToString());
        Response.End();
    }
    bool newSlamVersion()
    {
        if(SlamVersion<latestSlamVersion)
        {
            return true;
        }
        return false;
    }
    string AddUrlProperty(string url,  string code, string value)
    {
        string res=url;
        if(!string.IsNullOrWhiteSpace(value))
        {
            if(!url.Contains("?"))
            {
                res += "?";
            }
            if(res.LastIndexOf("?")!=res.Length-1)
            {
                res += "&";
            }
            res += code + "=" + value;
        }
        return res;
    }
    int CheckVisitors(string url)
    {
        if(!string.IsNullOrWhiteSpace( url))
        {
            string eUrl = HttpUtility.UrlEncode(url);
            return SlamSiteBase.GlobalBase.GetRoomCount(eUrl);
        }
        return 0;
    }
}