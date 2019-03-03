using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class SearchResultPresenter
    {
        public SearchResultPresenter(int pageSize=6, float slamVersion=1, bool hasNewSlamVersion=false) {
            PageSize = pageSize;
            SlamVersion = slamVersion;
            HasNewSlamVersion = hasNewSlamVersion;
        }
        public void CircleHalf(X3DTransform parent, List<IWebSceneProps> webScenePropes, int pageNo, float radius, float angle, out bool hasNextPage )
        {
            int counter = 0;
            counter = -pageNo * PageSize;
            hasNextPage = false;
            foreach (var website in webScenePropes)
            {
                string webUrl = website.Url;
                if ((!string.IsNullOrWhiteSpace(webUrl) && webUrl.ToLower() != "http://www.v-slam.org/index.aspx") || HasNewSlamVersion)
                {
                    if (counter >= 0)
                    {
                        AddSmoothCubeWebSite(website, counter, angle, radius, parent);
                    }
                    counter++;
                    if (counter > PageSize)
                    {
                        hasNextPage = true;
                        break;
                    }
                }
            }
        }

        public void CircleHalfPanel(X3DTransform parent, List<IWebSceneProps> webScenePropes, int pageNo, CallingDevices callingApp, out bool hasNextPage, WebPanelLayout webPanelLayout=null)
        {
            int counter = 0;
            counter = -pageNo * PageSize;
            hasNextPage = false;
            float angle = 0;
            Vector3 pos = null;
            int lcounter = 0;
            if(webPanelLayout==null)
            {
                webPanelLayout = new WebPanelLayout(callingApp);
            }
            foreach (var website in webScenePropes)
            {
                string webUrl = website.Url;
                if ((!string.IsNullOrWhiteSpace(webUrl) && webUrl.ToLower() != "http://www.v-slam.org/index.aspx") || HasNewSlamVersion)
                {
                    if (counter >= 0)
                    {
                        var w= CreateBusinesWebsite(website, webPanelLayout);
                        GetProps(lcounter, out angle, out pos);
                        w.Translation = pos;
                        w.EulerRotation = new Vector3(0, angle, 0);
                        
                        parent.Children.Add(w);
                        lcounter++;
                    }
                    counter++;
                    if (counter > PageSize-1)
                    {
                        hasNextPage = true;
                        break;
                    }
                }
            }
        }
        void GetProps(int number, out float angle, out Vector3 pos)
        {
            angle = 0;
            var smallAngle = 0f;
            var largeAngle = 30f;
            var smallXDistance = -0.56f;
            var largeXDistance = -1.7f;
            var smallZDistance = 0.5f;
            var largeZDistance = 0.3f;

            pos = new Vector3();
            switch(number)
            {
                case 0:
                    angle = smallAngle;
                    pos = new Vector3(smallXDistance, 2f, smallZDistance);
                    break;
                case 1:
                    angle = -smallAngle;
                    pos = new Vector3(-smallXDistance, 2f, smallZDistance);
                    break;
                case 2:
                    angle = smallAngle;
                    pos = new Vector3(smallXDistance, 0.7f, smallZDistance);
                    break;
                case 3:
                    angle = -smallAngle;
                    pos = new Vector3(-smallXDistance, 0.7f, smallZDistance);
                    break;
                case 4:
                    angle = -largeAngle;
                    pos = new Vector3(largeXDistance, 2f, largeZDistance);
                    break;
                case 5:
                    angle = largeAngle;
                    pos = new Vector3(-largeXDistance, 2f, largeZDistance);
                    break;
                case 6:
                    angle = -largeAngle;
                    pos = new Vector3(largeXDistance, 0.7f, largeZDistance);
                    break;
                case 7:
                    angle = largeAngle;
                    pos = new Vector3(-largeXDistance, 0.7f, largeZDistance);
                    break;
            }
        }

        X3DTransform CreateBusinesWebsite(IWebSceneProps websceneProps, 
            WebPanelLayout webPanelLayout
            )
        {
            X3DTransform t = new X3DTransform();
            X3DTransform panel = CreateSmoothCubePanel(websceneProps);
            var panelZposition = 0.025f;
            t.AddChild(panel);
            panel.Scale = new Vector3(0.4f, 0.4f, 0.02f);
            if (websceneProps.Favorite >= 0)
            {
                panel.Shape.Favorite = websceneProps.Favorite;
            }
            if (websceneProps.History >= 0)
            {
                panel.Shape.History = websceneProps.History;
            }
            //backPanel//new Vector3(f, f, f)
            X3DTransform panelBorder = CreateSmoothCubeTransForm(new Vector3(0, 0, panelZposition), new Vector3(0.44, 0.44, 0.02));
            t.AddChild(panelBorder);
            panelBorder.Shape.Appearance.Material = webPanelLayout.TopPanelBackGround;
            if (websceneProps.Favorite >= 0)
            {
                panelBorder.Shape.Favorite = websceneProps.Favorite;
            }
            //bottomPanel
            X3DTransform bottomPanel = CreateSmoothCubeTransForm(new Vector3(0, -0.605, panelZposition), new Vector3(0.44, 0.16, 0.02));
            t.AddChild(bottomPanel);
            bottomPanel.Shape.Appearance.Material = webPanelLayout.BottomPanelBackGround;
            bottomPanel.Shape.Url = websceneProps.Url;
            bottomPanel.Shape.Target = websceneProps.Target;
            if (websceneProps.Favorite >= 0)
            {
                bottomPanel.Shape.Favorite = websceneProps.Favorite;
            }
            if (websceneProps.History >= 0)
            {
                bottomPanel.Shape.History = websceneProps.History;
            }
            //bottomFrontPanel
            X3DTransform bottomFrontPanel = CreateSmoothCubeTransForm(new Vector3(0, -0.605, panelZposition), new Vector3(0.4, 0.147, 0.02));
            t.AddChild(bottomFrontPanel);
            bottomFrontPanel.Shape.Appearance.Material  = webPanelLayout.BottomPanelForground;
            bottomFrontPanel.Shape.Url = websceneProps.Url;
            bottomFrontPanel.Shape.Target = websceneProps.Target;
            if (websceneProps.Favorite >= 0)
            {
                bottomFrontPanel.Shape.Favorite = websceneProps.Favorite;
            }
            if (websceneProps.History >= 0)
            {
                bottomFrontPanel.Shape.History = websceneProps.History;
            }
            //text
            X3DTransform bottomText = X3DTransform.AddTransFormWithShape(ShapeType.Text, new Vector3(0, -0.584, 0), null, new Vector3(0.03, 0.03, 0.03));
            bottomText.Name = "txt";
            bottomText.Shape.Appearance.Material = webPanelLayout.BottomPanelText;
            bottomText.Shape.Url = websceneProps.Url;
            bottomText.Shape.Target = websceneProps.Target;
            bottomText.Shape.Text = "\"" + websceneProps.Name + "\"";
            bottomText.Shape.RectLength = 29;
            if (websceneProps.Favorite >= 0)
            {
                bottomText.Shape.Favorite = websceneProps.Favorite;
            }
            if (websceneProps.History >= 0)
            {
                bottomText.Shape.History = websceneProps.History;
            }
            if (websceneProps.Visitors > 0)
            {
                string txt = websceneProps.Visitors == 1 ? "(1 visitor)" : string.Format("({0} visitors)", websceneProps.Visitors);
                bottomText.Shape.Text += "\"" + txt + "\"";
            }
            t.AddChild(bottomText);

            return t;
        }
        X3DTransform CreateSmoothCubePanel(IWebSceneProps websceneProps)
        {
            X3DTransform panel = CreateSmoothCubeTransForm(new Vector3(), new Vector3(0.4, 0.4, 0.02));
            panel.Shape.Url = websceneProps.Url;
            panel.Shape.Appearance.ImageTexture.Url = websceneProps.ImageUrl;
            return panel;
        }
        X3DTransform CreateSmoothCubeTransForm(Vector3 translation, Vector3 scale)
        {
            X3DTransform panel = X3DTransform.AddTransFormWithShape(ShapeType.Prefab, null, null, scale);
            panel.Shape.Group = "primitives";
            panel.Shape.Item = "smoothcube";
            panel.Translation = translation;
            return panel;
        }
        void AddSmoothCubeWebSite(IWebSceneProps websceneProps, int counter, double angle, double radius, X3DTransform group)
        {
            string name = websceneProps.Name;
            string url = websceneProps.Url;
            string imageUrl = websceneProps.ImageUrl;
            Target t = websceneProps.Target;
            int visitors = websceneProps.Visitors;
            double tangle = angle + counter * Calc.ToRadians(40);
            double x = Math.Sin(tangle) * radius;
            double y = Math.Cos(tangle) * radius;
            Vector3 pos = new Vector3( x, 1, y);
            X3DTransform cubeT = null;
            cubeT = X3DTransform.AddTransFormWithShape(ShapeType.Prefab, pos, null, new Vector3(0.2, 0.2, 0.03));
            cubeT.Shape.Name = "smoothcube2";
            cubeT.Shape.Group = "primitives";
            cubeT.Shape.Url = url;
            cubeT.Shape.Target = t;
            cubeT.Shape.Appearance.Material.DEF = "Material" + name;
            cubeT.Shape.Appearance.Material.DiffuseColor = Vector3.One();
            cubeT.Shape.Appearance.ImageTexture.Url = imageUrl;
            cubeT.Shape.Appearance.Movement.Rotate = new Vector3(0.03, 1.2, 0);
            cubeT.Shape.Appearance.Movement.ApplyToParent = false;
            //transform5.Shape.Appearance.Movement.Center = "0.001 0 0";
            //           transform5.Shape.FaceCamera = FaceCamera.face_lock_y;
            group.AddChild(cubeT);
            Vector3 postxt = new Vector3( x, 0.7, y);
            X3DTransform textT = X3DTransform.AddTransFormWithShape(ShapeType.Text, postxt, null, new Vector3(0.03, 0.03, 0.03));
            textT.Shape.Url = url;
            textT.Shape.Target = t;
            textT.Shape.Text = "\"" + name + "\"";
            if (visitors > 0)
            {
                string txt = visitors == 1 ? "(1 visitor)" : string.Format("({0} visitors)", visitors);
                textT.Shape.Text += "\"" + txt + "\"";
            }
            textT.Shape.Appearance.Material.USE = "Material" + name;
            textT.Shape.Appearance.Material.DiffuseColor = new Vector3(0.1, 0.35, 0.46);
            group.AddChild(textT);
        }

        public int PageSize { get; set; }
        public float SlamVersion { get; set; }
        public bool HasNewSlamVersion { get; set; }
    }
    public class WebPanelLayout
    {
        public WebPanelLayout(X3DMaterial fgMat, X3DMaterial bgMat, X3DMaterial txtMat)
        {
            TopPanelBackGround = bgMat;
            BottomPanelForground = fgMat;
            BottomPanelBackGround = bgMat;
            BottomPanelText = txtMat;
        }
            public WebPanelLayout() {
            double transparancy_BG = 0.6;
            double transparancy_FG = 0.2;
            Vector3 bottomPanelDiffuceColor = new Vector3(0.1, 0.1, 0);
            TopPanelBackGround = new X3DMaterial("Menu_Panel_BG", bottomPanelDiffuceColor, transparancy_BG);
            BottomPanelForground = new X3DMaterial("bottom_panel_FG", bottomPanelDiffuceColor, transparancy_FG);
            BottomPanelBackGround = new X3DMaterial("Menu_Panel_BG", bottomPanelDiffuceColor, transparancy_BG);
            BottomPanelText = new X3DMaterial("Menu_FrontPanel_Txt", new Vector3());

        }
        public WebPanelLayout(CallingDevices callingApp)
        {
            double transparancy_BG = 0.6;
            double transparancy_FG = 0.2;
            Vector3 bottomPanelDiffuceColor = new Vector3(0.1, 0.1, 0);
            if (callingApp == CallingDevices.HoloLens)
            {
                bottomPanelDiffuceColor = new Vector3(0.4, 0.4, 0);
            }
            TopPanelBackGround = new X3DMaterial("Menu_Panel_BG", bottomPanelDiffuceColor, transparancy_BG);
            BottomPanelForground = new X3DMaterial("bottom_panel_FG", bottomPanelDiffuceColor, transparancy_FG);
            BottomPanelBackGround = new X3DMaterial("Menu_Panel_BG", bottomPanelDiffuceColor, transparancy_BG);
            BottomPanelText = new X3DMaterial("Menu_FrontPanel_Txt", new Vector3());

        }
        public X3DMaterial TopPanelBackGround { get; set; }
        public X3DMaterial BottomPanelForground { get; set; }
        public X3DMaterial BottomPanelBackGround { get; set; }
        public X3DMaterial BottomPanelText { get; set; }
    }
}
