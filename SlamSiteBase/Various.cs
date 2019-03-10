using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamSiteBase
{
    public class TransFormHolder
    {
        public string Position { get; set; }
        public string Rotation { get; set; }
        public string LHPosition { get; set; }
        public string LHRotation { get; set; }
        public string RHPosition { get; set; }
        public string RHRotation { get; set; }
        public string NickName { get; set; }
        public string AvatarNo { get; set; }
        public string DissonanceGuid { get; set; }
        public int IdleCount { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public class SceneAction
    {
        public string GoName { get; set; }
        public float SceneTime { get; set; }
    }
    public class ActionHolder
    {
        public ActionHolder()
        {
            Actions = new List<SceneAction>();
        }
        public string ActionsOwnerId { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<SceneAction> Actions { get; set; }
    }
    public class TimedCode
    {
        public TimedCode(string code)
        {
            Code = code;
            TimeStamp = DateTime.Now;
        }
        public string Code { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public class TimedSession: TimedCode
    {
        public TimedSession(string code):base(code)
        {
        }
        public TimedSession(string code, string userGuid, string legacyAvatarUrl):base(code)
        {
            UserGuid = userGuid;
            LegacyAvatarUrl = legacyAvatarUrl;
        }
        public string UserGuid { get; set; }
        public string LegacyAvatarUrl { get; set; }
    }
    public class TimedServerObjectTransformList
    {
        public Dictionary<string,Queue< ServerObjectTransform>> ServerObjectTransformDict { get; set; }
        public DateTime TimeStamp { get; set; }

    }
    public class ServerObjectTransform
    {
        public Vector3Json Position { get; set; }
        public QuaternionJson Rotation { get; set; }
        public Vector3Json Velocity { get; set; }
        public string Guid { get; set; }
        public Vector3Json AngularVelocity { get; set; }
        public string UserGuid { get; set; }

    }
    public class Vector3Json
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public static Vector3Json Mediate(Vector3Json v1, Vector3Json v2, double minDis = 0)
        {
            if (minDis == 0 || Distance(v1, v2) > minDis)
            {
                Vector3Json res = new Vector3Json();
                res.X = (v1.X + v2.X) / 2;
                res.Y = (v1.Y + v2.Y) / 2;
                res.Z = (v1.Z + v2.Z) / 2;
                return res;
            }
            return v1;
        }
        public static Vector3Json Diff(Vector3Json v1, Vector3Json v2)
        {
            Vector3Json res = new Vector3Json();
            res.X = (v1.X - v2.X);
            res.Y = (v1.Y - v2.Y);
            res.Z = (v1.Z - v2.Z);

            return res;
        }
        public static double Distance(Vector3Json v1, Vector3Json v2)
        {
            Vector3Json res = Diff(v1, v2);
            return Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2));
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }
        public static Vector3Json operator + (Vector3Json v1, Vector3Json v2)
        {
            Vector3Json res = new Vector3Json();
            res.X = v1.X + v2.X;
            res.Y = v1.Y + v2.Y;
            res.Z = v1.Z + v2.Z;
            return res;
        }
        public static Vector3Json operator /(Vector3Json v1, float f)
        {
            Vector3Json res = new Vector3Json();
            res.X = v1.X / f;
            res.Y = v1.Y / f;
            res.Z = v1.Z / f;
            return res;
        }
        public static Vector3Json Mean(List<Vector3Json> lst)
        {
            if(lst!=null && lst.Count>0)
            {
                Vector3Json r = new Vector3Json();
                foreach(var i in lst)
                {
                    r += i;
                }
                return r / lst.Count;
            }
            return null;
        }
    }
    public class QuaternionJson
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public static QuaternionJson Mediate(QuaternionJson v1, QuaternionJson v2)
        {
            QuaternionJson res = new QuaternionJson();
            res.X = (v1.X + v2.X) / 2;
            res.Y = (v1.Y + v2.Y) / 2;
            res.Z = (v1.Z + v2.Z) / 2;
            res.W = (v1.W + v2.W) / 2;
            return res;
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", X, Y, Z, W);
        }
        public static QuaternionJson operator +(QuaternionJson v1, QuaternionJson v2)
        {
            QuaternionJson res = new QuaternionJson();
            res.X = v1.X + v2.X;
            res.Y = v1.Y + v2.Y;
            res.Z = v1.Z + v2.Z;
            res.W = v1.Z + v2.W;
            return res;
        }
        public static QuaternionJson operator /(QuaternionJson v1, float f)
        {
            QuaternionJson res = new QuaternionJson();
            res.X = v1.X / f;
            res.Y = v1.Y / f;
            res.Z = v1.Z / f;
            res.W = v1.W / f;
            return res;
        }
        public static QuaternionJson Mean(List<QuaternionJson> lst)
        {
            if (lst != null && lst.Count > 0)
            {
                QuaternionJson r = new QuaternionJson();
                foreach (var i in lst)
                {
                    r += i;
                }
                return r / lst.Count;
            }
            return null;
        }
    }


}
