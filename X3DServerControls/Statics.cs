using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace SlmControls
{
    public static class Statics
    {
        public static string CleanString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            return input.Replace("\"", "‘").Replace("'", "‘");
        }
        public static bool TryVector3FromString(string vectString, out Vector3 v)
        {
            v = new Vector3(0);
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Trim().Split(Convert.ToChar(" "));
                if (w.Length > 2)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if (float.TryParse(w[0], out x) && float.TryParse(w[1], out y) && float.TryParse(w[2], out z))
                    {
                        v = new Vector3(x, y, z);
                        return true;
                    }
                }
            }
            return false;
        }

        public static Vector3 Vector3FromString(string vectString)
        {
            if (!string.IsNullOrEmpty(vectString))
            {
                string[] w = vectString.Split(Convert.ToChar(" "));
                return new Vector3((float)Convert.ToDecimal(w[0]), (float)Convert.ToDecimal(w[1]), (float)Convert.ToDecimal(w[2]));
            }
            return new Vector3(0);
        }

        public static long GetDirectorySize(string p)
        {
            string[] a = Directory.GetFiles(p, "*.*");

            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }

        public static string ToSlamString(this bool? b)
        {
            return b == null ? null : (bool)b ? "true" : "false";
        }
        public static string ToSlamString(this double? b)
        {
            return b == null ? null : b.ToString();
        }
        public static string ToSlamString(this int? b)
        {
            return b == null ? null : b.ToString();
        }

        public static string MakeFullUrl(System.Web.HttpRequest request, string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                url = url.Replace("~/", GetBaseURL(request) + "/");
            }
            return url;
        }
        public static string MakeProtocolUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                return url.Replace("http://", "x3dx://").Replace("https://", "x3dx://");
            }
            return url;
        }
        public static string GetBaseURL(System.Web.HttpRequest request)
        {
            return request.Url.GetLeftPart(UriPartial.Authority);
        }
        public static X3DTransform GetTextTransform(X3DTransform parent, string text, Vector3 translation = null, double rectLength = 20)
        {
            var llabel = X3DTransform.AddTransFormWithShape(ShapeType.Text, null, null, Vector3.One(0.05));
            llabel.Shape.Text = text;
            llabel.Translation = translation;
            llabel.Shape.Appearance.Material.DEF = "mat_text";
            llabel.Shape.Appearance.Material.DiffuseColor = new Vector3();
            llabel.Shape.FontStyle.Justify = Justify.LEFT;
            llabel.Shape.RectHeight = 3;
            llabel.Shape.RectLength = rectLength;
            parent.AddChild(llabel);
            return llabel;

        }
        public static X3DTransform GetBoard(X3DTransform parent, string imageUrl = null, Vector3 translation = null)
        {
            var boardT = X3DTransform.AddTransFormWithShape(ShapeType.Cube);
            boardT.Translation = translation;
            boardT.EulerRotation = new Vector3(0, 180, 0);
            boardT.Shape.Appearance.Material.DiffuseColor = new Vector3(1);
            boardT.Shape.Appearance.ImageTexture.Url = imageUrl;
            boardT.Scale = new Vector3(1, 1, 0.01);
            parent.AddChild(boardT);
            return boardT;

        }
        public static X3DTransform GetPrefab(X3DTransform parent, string group, string item)
        {
            var prefabT = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            prefabT.Shape.Group = group;
            prefabT.Shape.Item = item;
            parent.AddChild(prefabT);
            return prefabT;

        }
        public static X3DTransform GetSmoothButton(X3DTransform parent, string text, string url, Vector3 translation)
        {
            var bp = new X3DTransform();
            parent.AddChild(bp);
            bp.Scale = new Vector3(0.22, 0.1, -0.007);
            bp.Translation = translation;

            var sb = X3DTransform.AddTransFormWithShape(ShapeType.Prefab);
            sb.Scale = new Vector3(1.7, 1, 1);
            sb.Shape.Item = "smoothcube";
            sb.Shape.Group = "primitives";
            sb.Shape.Appearance.Material.DiffuseColor = new Vector3(0.7, 0.7, 0.7);
            sb.Shape.Appearance.Material.DEF = "button_color";
            sb.Shape.Url = url;
            sb.Shape.Target = Target._Blank;
            bp.AddChild(sb);
            var t = GetTextTransform(bp, text, new Vector3(-0.14, 0, 1.4));
            t.Scale = Vector3.One(0.23);
            //t.Shape.FontStyle.Justify = Justify.MIDDLE;
            t.Shape.RectLength = 10;
            return bp;
        }
    }
    public enum CallingDevices
    {
        PC,
        Unity_Editor,
        HoloLens,
        MR,
        OSX
    }

}
