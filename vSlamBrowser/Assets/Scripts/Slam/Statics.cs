using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Slam
{
    public static class Statics
    {
        public static Vector3 GetRandomUnitVector()
        {
            return UnityEngine.Random.onUnitSphere;
        }
        public static bool TryGetTarget(string targetstring, out Target target)
        {
            target = Target._blank;
            targetstring = string.IsNullOrEmpty(targetstring) ? "" : targetstring.ToLower();
            switch (targetstring)
            {
                case "_blank":
                    target = Target._blank;
                    return true;
                case "_self":
                    target = Target._self;
                    return true;
                case "_inline":
                    target = Target._inline;
                    return true;
                case "_2d":
                    target = Target._2D;
                    return true;

            }
            return false;
        }
        public static Vector3Json ToJson(Vector3 v)
        {
            Vector3Json o = new Vector3Json();
            o.X = v.x;
            o.Y = v.y;
            o.Z = v.z;
            return o;
        }
        public static QuaternionJson ToJson(Quaternion v)
        {
            QuaternionJson o = new QuaternionJson();
            o.X = v.x;
            o.Y = v.y;
            o.Z = v.z;
            o.W = v.w;
            return o;
        }
        public static Vector3 FromJson(Vector3Json v)
        {
            Vector3 o = new Vector3();
            o.x = v.X;
            o.y = v.Y;
            o.z = v.Z;
            return o;
        }
        public static Quaternion FromJson(QuaternionJson v)
        {
            Quaternion o = new Quaternion();
            o.x = v.X;
            o.y = v.Y;
            o.z = v.Z;
            o.w = v.W;
            return o;
        }
        public static string Vector3ToString(this Vector3 v)
        {
            return string.Format("{0} {1} {2}", v.x, v.y, v.z);
        }
        public static string QuaternionToString(this Quaternion q)
        {
            return string.Format("{0} {1} {2} {3}", q.x, q.y, q.z, q.w);
        }
        public static string X3dTextToMultiline(string text)
        {
            string totText = "";
            var txts = ParseStringParts(text);
            bool started = false;
            foreach (var tx in txts)
            {
                if (started)
                {
                    totText += "\r\n";
                }
                totText += tx;
                started = true;
            }
            return totText;
        }
        public static List<string> ParseStringParts(string text)
        {
            List<string> lst = new List<string>();
            char[] chars = text.ToCharArray();
            string t = "";
            string quoteChar = null;
            bool wordstarted = false;

            for (int nn = 0; nn < chars.Length; nn++)
            {
                string c = chars[nn].ToString();
                if (quoteChar == null && (c == "\"" || c == "'"))
                {
                    quoteChar = c;
                }
                if (c == quoteChar)
                {
                    if (wordstarted)
                    {
                        lst.Add(t);
                        t = "";
                    }
                    wordstarted = !wordstarted;
                }
                else if (wordstarted)
                {
                    t += c;
                }

            }
            if (lst.Count == 0)
            {
                lst.Add(text);
            }
            return lst;
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }
        public static T AssureComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            var x = go.GetComponent<T>();
            if (x == null)
            {
                x = go.AddComponent<T>();
            }
            return x;
        }
        /// <summary>
        /// Convert string to Color (if defined as a static property of Color)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(this string color)
        {
#if !UNITY_WSA
            var li = color.ToLowerInvariant();
            var toc = typeof(Color).GetProperty(li);
            if (toc != null)
            {
                var piv = toc.GetValue(null, null);
                return (Color)piv;
            }
#endif
            return Color.black;
        }
        public static Transform DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
        public static void ClearAll(this Transform transform)
        {
            ;
            transform.position = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.Euler(0,0,0);
            transform.localScale = Vector3.one;
        }
        public static void ClearPosition(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
        }
         public static string HumanReadableBytes(this long size)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int s = 0;

            while (size >= 1024)
            {
                s++;
                size /= 1024;
            }

            return System.String.Format("{0} {1}", size, suffixes[s]);
        }
        public static bool CheckBoolString(string b)
        {
            if(b!=null)
            {
                b = b.ToLower().Trim();
                return b == "1" || b == "true" || b == "yes";
            }
            return false;
        }
        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            foreach (Transform child in aParent)
            {
                if (child.name == aName)
                    return child;
                var result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
        public static void SetAsTransparant(this Material material)
        {
            //https://forum.unity3d.com/threads/standard-material-shader-ignoring-setfloat-property-_mode.344557/
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        public static string AddUrlProperty(string url, string code, string value)
        {
            string res = url;
            if (!string.IsNullOrEmpty(value))
            {
                if (!url.Contains("?"))
                {
                    res += "?";
                }
                if (res.LastIndexOf("?") != res.Length - 1)
                {
                    res += "&";
                }
                res += code + "=" + value;
            }
            return res;
        }

        public static string CleanWhiteSpace(string text)
        {
            if (text != null)
            {
                text = text.Replace("\r\n", " ");
                text = text.Replace("\r", " ");
                text = text.Replace("\n", " ");
                text = text.Replace("  ", " ");
                text = text.Replace("  ", " ");
            }
            return text;
        }

        //public static string CheckText(string text, int lineLetters)
        //{
        //    string rest = text;
        //    string res = "";
        //    while (rest.Length > lineLetters)
        //    {

        //        res += rest.Substring(0, lineLetters) + "\r\n";
        //        rest = rest.Substring(lineLetters);
        //    }
        //    res += rest;
        //    return res;
        //}
        public static string Replace(string str, string oldValue, string newValue, System.StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));
            return sb.ToString();
        }
        public static string GetLines(string input, int pageSize, int page)
        {
            List<string> stack = new List<string>(input.Split('\r'));
            float totLines = (float)stack.Count;
            int currPage = 0;
            int currLine = 0;

            stack.Reverse();
            List<string> retList = new List<string>();
            foreach (string line in stack)
            {
                currLine++;
                if (currPage == page)
                {
                    retList.Add(line);
                }

                if (currLine == pageSize)
                {
                    currPage++;
                    currLine = 0;
                }
                if (retList.Count == pageSize)
                {
                    break;
                }
            }
            retList.Reverse();
            return string.Join("\r", retList.ToArray());
        }
        public static int NumberOfPages(string input, int pageSize)
        {
            List<string> stack = new List<string>(input.Split('\r'));
            float totLines = (float)stack.Count;
            return (int)(totLines / ((float)pageSize));
        }
        public static char[] splitOnCharacters = " ".ToCharArray();

        public static string SplitToLines(string text, int maxStringLength)
        {
            var sb = new StringBuilder();
            var index = 0;
             while (text.Length > index)
            {
                // start a new line, unless we've just started
                if (index != 0)
                    sb.AppendLine();

                // get the next substring, else the rest of the string if remainder is shorter than `maxStringLength`
                var splitAt = -1;
                if(index + maxStringLength <= text.Length)
                {
                    var nextPart = text.Substring(index, maxStringLength);
                    var tidx = nextPart.IndexOf("\r\n");
                    if(tidx>0)
                    {
                        splitAt = tidx;
                    }
                    else
                    {
                        splitAt = nextPart.LastIndexOfAny(splitOnCharacters);
                    }
                }
                else
                {
                    splitAt= text.Length - index; ;
                }
                    

                // if can't find split location, take `maxStringLength` characters
                splitAt = (splitAt == -1) ? maxStringLength : splitAt;

                // add result to collection & increment index
                sb.Append(text.Substring(index, splitAt).Trim());
                index += splitAt;
            }
            return sb.ToString();
        }


 
    }
}