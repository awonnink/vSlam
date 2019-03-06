using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slam
{
    public class HtmlParser
    {

        public void HandleHtml(X3DParse.Node node, Transform parent)
        {
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "body":
                        HandleBody(child, parent);
                        break;
                }
            }

        }
        void HandleBody(X3DParse.Node node, Transform parent)
        {
            //add canvas
            GameObject g=new GameObject("Canvas");
            g.transform.parent = parent;
            var canvas = g.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            SetAnchorAndPivot(canvas.transform);
            CanvasScaler cs = g.AddComponent<CanvasScaler>();
            cs.scaleFactor = 1.0f;
            cs.dynamicPixelsPerUnit = 10f;
            CssStyle cssStyle = new CssStyle();
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                HandleCommonElements(child, g.transform, cssStyle.Clone());
            }
        }
        void SetAnchorAndPivot(Transform transForm)
        {
            RectTransform rectTransForm = transForm as RectTransform;
            rectTransForm.anchorMin = new Vector2(0, 1);
            rectTransForm.anchorMax = new Vector2(0, 1);
            rectTransForm.pivot = new Vector2(0, 1);
        }
        void HandleCommonElements(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
 
            string name = node.Name.ToLower();
            // var a = 1;
            switch (name)
            {
                case "div":
                    HandleDiv(node, canvas, cssStyle.Clone());
                    break;
                case "span":
                    HandleSpan(node, canvas, cssStyle.Clone());
                    break;
                case "img":
                    HandleImg(node, canvas, cssStyle.Clone());
                    break;
                case "a":
                    HandleAnchor(node, canvas, cssStyle.Clone());
                    break;
            }
        }
        void CheckLink(GameObject go, CssStyle cssStyle)
        {
            if(!string.IsNullOrEmpty(cssStyle.Href))
            {
                var col=go.AddComponent<BoxCollider2D>();
                float x = cssStyle.Width;
                float y = cssStyle.Height;

                col.size = new Vector2(x, y);
                col.offset= new Vector2(x/2, -y/2);
                var so = go.AddComponent<SlamObject>();
                so.Href = cssStyle.Href;
                so.Target = cssStyle.Target;
                so.ToolTip = cssStyle.ToolTip;
            }
        }
        void HandleAnchor(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
            var at = CheckAttributes(node, cssStyle);
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                HandleCommonElements(child, canvas, cssStyle.Clone());
            }
        }
        void HandleImg(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
            GameObject panel = new GameObject("Img");
            var rt = panel.AddComponent<RectTransform>();
            SetAnchorAndPivot(rt.transform);

            var at = CheckAttributes(node, cssStyle);


            var cr = panel.AddComponent<CanvasRenderer>();

            Image i = panel.AddComponent<Image>();
            panel.transform.parent = canvas;
            Slam.Instance.AddTexture(panel, at.Src);
            ApplyCssStyle(panel, cssStyle);
        }

        void HandleDiv(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
            CheckAttributes(node, cssStyle);
            GameObject panel = new GameObject("Panel");
            var rt = panel.AddComponent<RectTransform>();
            SetAnchorAndPivot(rt.transform);



            var cr = panel.AddComponent<CanvasRenderer>();

            Image i = panel.AddComponent<Image>();
            panel.transform.parent = canvas;
            ApplyCssStyle(panel, cssStyle);
            CheckTextNode(node, rt, cssStyle.Clone());
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                HandleCommonElements(child, rt, cssStyle.Clone());
            }
        }
        void ApplyCssStyle(GameObject go, CssStyle cssStyle)
        {
            RectTransform rectTransForm = go.transform as RectTransform;
            if (rectTransForm != null)
            {
                rectTransForm.anchoredPosition = new Vector2(cssStyle.Left, -cssStyle.Top);
                rectTransForm.sizeDelta = new Vector2(cssStyle.Width, cssStyle.Height);
            }
            Image image = go.GetComponent<Image>();
            if(image!=null)
            {
                image.color = cssStyle.BackGroundColor;
            }
            CheckLink(go, cssStyle);
        }
        void HandleSpan(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
            CheckAttributes(node, cssStyle);
            CheckTextNode(node, canvas, cssStyle.Clone());
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Element))
            {
                HandleCommonElements(child, canvas, cssStyle.Clone());
            }
        }
        Attributes CheckAttributes(X3DParse.Node node, CssStyle cssStyle)
        {
            Attributes at = new Attributes();
            foreach (X3DParse.Node child in node.Children.FindAll(x => x.NodeType == X3DParse.NodeType.Attribute))
            {
                string name = child.Name.ToLower();
                // var a = 1;
                switch (name)
                {
                    case "style":
                        HandleStyle(child, cssStyle);
                        break;
                    case "scr":
                        at.Src = child.Value;
                        break;
                    case "title":
                        cssStyle.ToolTip = child.Value;
                        break;
                    case "href":
                        cssStyle.Href= child.Value;
                        break;
                }
            }
            return at;
        }
        void HandleStyle(X3DParse.Node node, CssStyle cssStyle)
        {

            var styles = node.Value.Split(System.Convert.ToChar(";"));
            foreach(var p in styles)
            {
                var styleP=p.Split(System.Convert.ToChar(":"));
                if(styleP.Length==2)
                {
                    var style = styleP[0].Trim().ToLower();
                    var val= styleP[1].Trim();
                    switch(style)
                    {
                        case "top":
                            cssStyle.Top +=TryFloatValue(val);
                            break;
                        case "left":
                            cssStyle.Left += TryFloatValue(val);
                            break;
                        case "width":
                            cssStyle.Width = TryFloatValue(val);
                            break;
                        case "margin-top":
                            cssStyle.Margin_top = TryFloatValue(val);
                            break;
                        case "margin-bottom":
                            cssStyle.Margin_bottom = TryFloatValue(val);
                            break;
                        case "margin-left":
                            cssStyle.Margin_left = TryFloatValue(val);
                            break;
                        case "margin-right":
                            cssStyle.Margin_right = TryFloatValue(val);
                            break;
                        case "margin":
                            cssStyle.Margin = TryFloatValue(val);
                            break;
                        case "height":
                            cssStyle.Height = TryFloatValue(val);
                            break;
                        case "color":
                            cssStyle.Color = TryColorValue(val, Color.black);
                            break;
                        case "background-color":
                            cssStyle.BackGroundColor = TryColorValue(val, Color.white);
                            break;
                        case "font-size":
                            cssStyle.FontSize = TryIntValue(val, 5);
                            break;
                    }
                }

            }
        }
        Color TryColorValue(string color, Color defaultC)
        {
            Color c = defaultC;
            ColorUtility.TryParseHtmlString(color, out c); 
            return c;
        }
        float TryFloatValue(string val)
        {
            float fval = 0;
            val = val.Replace("px","");
            float.TryParse(val, out fval);
            return fval;
        }
        int TryIntValue(string val, int defaultI)
        {
            int fval = defaultI;
            int.TryParse(val, out fval);
            return fval;
        }
        void CheckTextNode(X3DParse.Node node, Transform canvas, CssStyle cssStyle)
        {
            if(!string.IsNullOrEmpty(node.Value))
            {
                GameObject g2 = new GameObject();
                g2.name = "Text";
                g2.transform.parent = canvas;
                var t = g2.AddComponent<TMPro.TextMeshProUGUI>();
                g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
                g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
                SetAnchorAndPivot(g2.transform);
                ApplyCssStyle(g2, cssStyle);
                t.alignment = TMPro.TextAlignmentOptions.TopLeft;
                t.overflowMode = TMPro.TextOverflowModes.Ellipsis;
                t.fontSize = cssStyle.FontSize;
                t.text = node.Value;
                t.enabled = true;
                t.color = cssStyle.Color;
                t.margin = new Vector4( cssStyle.Margin_left, cssStyle.Margin_top, cssStyle.Margin_bottom,cssStyle.Margin_right);
                t.extraPadding = true;
                
            }
        }


    }
    public class Attributes
    {
        public string Src { get; set; }
    }
    public class CssStyle
    {
        public CssStyle()
        {
            Top = 0;
            Left = 0;
            Width = 0;
            Height = 0;
            Color = Color.black;
            BackGroundColor = Color.white;
            FontSize = 5;
            Target = Target._self;
        }
        public float Top { get; set; }
        public float Left { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public float Margin_top { get; set; }
        public float Margin_bottom { get; set; }
        public float Margin_left { get; set; }
        public float Margin_right { get; set; }
        public float Margin
        {
            set
            {
                Margin_top = value;
                Margin_bottom = value;
                Margin_left = value;
                Margin_right = value;
            }
        }

        public Color Color { get; set; }
        public Color BackGroundColor { get; set; }
        public int FontSize { get; set; }
        public string Href { get; set; }
        public string ToolTip { get; set; }
        public Target Target { get; set; }

        public CssStyle Clone()
        {
            var c = new CssStyle();
            //c.Top = Top;
            //c.Left = Left;
            c.Width = Width;
            c.Height = Height;
            c.Color = Color;
            c.BackGroundColor = BackGroundColor;
            c.FontSize = FontSize;
            c.Href = Href;
            c.Target = Target;
            c.ToolTip = ToolTip;
            c.Margin_bottom = Margin_bottom;
            c.Margin_left = Margin_left;
            c.Margin_right = Margin_right;
            c.Margin_top = Margin_top;
            return c;
        }
    }
}
