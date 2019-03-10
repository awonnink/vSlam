using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamSiteBase
{
    public static class GlobalBase
    {
        static int maxUsers = 30;
        static int idleCountLimit = 1000;
        static int verificationCodeTimeLimit = 10;
        static int roomCountTimeLimit = 10;

        static Dictionary<string, Dictionary<string, TransFormHolder>> _TransFormDict;
        static Dictionary<string, TimedServerObjectTransformList> _ServerObjectDict;
        static Dictionary<string, ActionHolder> _ActionsDict;
        static Dictionary<string, TimedCode> _VerificationDict;
        static Dictionary<string, TimedSession> _SessionDict;
        static List<TimedCode> _PresentationCode = new List<TimedCode>();
        static Object thisLock = new Object();

        static public List<TimedCode> PresentationCode
        {
            get
            {
                return _PresentationCode;
            }
            set
            {
                _PresentationCode = value;
            }
        }
        public static TimedCode GetMostRecentCode(string userg = null)
        {
            PresentationCode.RemoveAll(item => item.TimeStamp < DateTime.Now.AddMinutes(-30));
            if (string.IsNullOrEmpty(userg))
            {
                return PresentationCode.OrderBy(x => x.TimeStamp).FirstOrDefault();
            }
            else
            {
                return PresentationCode.Find(x => x.Code == userg);
            }
        }
        public static void AddTimedCode(string userg)
        {
            var tc = GetMostRecentCode(userg);
            if (tc == null)
            {
                tc = new TimedCode(userg);
                PresentationCode.Add(tc);
            }
            else
            {
                tc.TimeStamp = DateTime.Now;
            }
        }
        static Dictionary<string, TimedSession> SessionDict
        {
            get
            {
                if (_SessionDict == null)
                {
                    _SessionDict = new Dictionary<string, TimedSession>();
                }
                return _SessionDict;
            }
        }
        static Dictionary<string, TimedCode> VerificationDict
        {
            get
            {
                if (_VerificationDict == null)
                {
                    _VerificationDict = new Dictionary<string, TimedCode>();
                }
                return _VerificationDict;
            }
        }
        public static void AddUserIdToSession(string sessionCode, string loginId, string userGuid, string legacyAvatarUrl)
        {
            foreach (var item in SessionDict.Where(kvp => kvp.Value.TimeStamp < DateTime.Now.AddMinutes(-60)).ToList())
            {
                SessionDict.Remove(item.Key);
            }

            SessionDict[sessionCode] = new TimedSession(loginId, userGuid, legacyAvatarUrl);
        }
        public static bool HasLegacyAvatar(string userGuid, out string LegacyAvatarUrl)
        {
            LegacyAvatarUrl = null;
            var x = SessionDict.Where(kvp => kvp.Value.UserGuid == userGuid).ToList();
            if (x != null && x.Count > 0)
            {
                LegacyAvatarUrl = x[0].Value.LegacyAvatarUrl;
                return true;
            }
            return false;

        }
        public static bool TryGetUserIdFromSessionCode(string sessionCode, out string userId)
        {
            userId = null;
            TimedSession vcode = null;
            if (sessionCode != null && SessionDict.TryGetValue(sessionCode, out vcode))
            {
                if (vcode.TimeStamp > DateTime.Now.AddMinutes(-60))
                {
                    userId = vcode.Code;
                    //refresh timestamp:
                    vcode.TimeStamp = DateTime.Now;
                    SessionDict[sessionCode] = vcode;

                    return true;
                }
            }
            return false;
        }
        public static void AddVerificationCode(string email, string code)
        {
            foreach (var item in VerificationDict.Where(kvp => kvp.Value.TimeStamp < DateTime.Now.AddMinutes(-verificationCodeTimeLimit)).ToList())
            {
                VerificationDict.Remove(item.Key);
            }

            VerificationDict[email] = new TimedCode(code);
        }
        public static bool TryGetVerificationCode(string email, out string code)
        {
            code = null;
            TimedCode vcode = null;
            if (VerificationDict.TryGetValue(email, out vcode))
            {
                if (vcode.TimeStamp > DateTime.Now.AddMinutes(-verificationCodeTimeLimit))
                {
                    code = vcode.Code;
                    return true;
                }
            }
            return false;
        }
        static Dictionary<string, ActionHolder> ActionsDict
        {
            get
            {
                if (_ActionsDict == null)
                {
                    _ActionsDict = new Dictionary<string, ActionHolder>();
                }
                return _ActionsDict;
            }
        }
        static Dictionary<string, Dictionary<string, TransFormHolder>> TransFormDict
        {
            get
            {
                if (_TransFormDict == null)
                {
                    _TransFormDict = new Dictionary<string, Dictionary<string, TransFormHolder>>();
                }
                return _TransFormDict;
            }
        }
        static Dictionary<string, TimedServerObjectTransformList> ServerObjectDict
        {
            get
            {
                if (_ServerObjectDict == null)
                {
                    _ServerObjectDict = new Dictionary<string, TimedServerObjectTransformList>();
                }
                return _ServerObjectDict;
            }
        }
        public static string UpdateTransFormHolder(string roomKey, string userGuid, TransFormHolder transformHolder, SceneAction action, string userPosStr, PostI post, int Timezone_Offset, List<ServerObjectTransform> sot)
        {
            lock (thisLock)
            {
                Dictionary<string, TransFormHolder> tList = null;
                if (!TransFormDict.TryGetValue(roomKey, out tList))
                {
                    tList = new Dictionary<string, TransFormHolder>();
                }
                foreach (var item in tList.Where(kvp => kvp.Value.TimeStamp < DateTime.Now.AddSeconds(-4)).ToList())
                {
                    tList.Remove(item.Key);
                }
                //check idlecount
                if (tList.ContainsKey(userGuid))
                {
                    var lastTransform = tList[userGuid];
                    if (lastTransform != null && lastTransform.Rotation == transformHolder.Rotation)
                    {
                        transformHolder.IdleCount = lastTransform.IdleCount + 1;
                    }
                }
                tList[userGuid] = transformHolder;
                TransFormDict[roomKey] = tList;
                ActionHolder ah = null;
                ah = UpdateActionsHolder(roomKey, userGuid, action);
                List<ServerObjectTransform> sotRes = HandleServerObjects(roomKey, userGuid, sot);
                SlmControls.Vector3 userPos = SlmControls.Vector3.FromString(userPosStr);
                return MakeXml(tList, ah, userPos, post, Timezone_Offset, sotRes);
            }
        }
        static List<ServerObjectTransform> HandleServerObjects(string roomKey, string userGuid, List<ServerObjectTransform> sot)
        {
            List<ServerObjectTransform> res = new List<ServerObjectTransform>();
            TimedServerObjectTransformList timedServerObjectTransformList = null;
            if (!ServerObjectDict.TryGetValue(roomKey, out timedServerObjectTransformList))
            {
                timedServerObjectTransformList = new TimedServerObjectTransformList();
                timedServerObjectTransformList.ServerObjectTransformDict = new Dictionary<string, Queue<ServerObjectTransform>>();
                timedServerObjectTransformList.TimeStamp = DateTime.Now;
                // tList = new Dictionary<string, ServerObjectTransform>();
            }
            else
            {
                if (timedServerObjectTransformList.TimeStamp < DateTime.Now.AddMinutes(-5))
                {
                    timedServerObjectTransformList.ServerObjectTransformDict.Clear();
                }
            }
            if (sot != null)
            {
                timedServerObjectTransformList.TimeStamp = DateTime.Now;

                foreach (var t in sot)
                {
                    Queue<ServerObjectTransform> formerQ;
                    if (timedServerObjectTransformList.ServerObjectTransformDict.ContainsKey(t.Guid))
                    {
                        formerQ = timedServerObjectTransformList.ServerObjectTransformDict[t.Guid];
                    }
                    else
                    {
                        formerQ = new Queue<ServerObjectTransform>();

                    }
                    formerQ.Clear();
                    t.UserGuid = userGuid;
                    formerQ.Enqueue(t);
                    //ServerObjectTransform r = new ServerObjectTransform();
                    //List<Vector3Json> pos = new List<Vector3Json>();
                    //List<QuaternionJson> rot = new List<QuaternionJson>();
                    //List<Vector3Json> vel = new List<Vector3Json>();
                    //List<Vector3Json> avel = new List<Vector3Json>();
                    //foreach (var formerT in formerQ.Where(x=>x.UserGuid!= userGuid))
                    //{
                    //    pos.Add(formerT.Position);
                    //    rot.Add(formerT.Rotation);
                    //    vel.Add(formerT.Velocity);
                    //    avel.Add(formerT.AngularVelocity);
                    //}
                    //pos.Add(t.Position);
                    //rot.Add(t.Rotation);
                    //vel.Add(t.Velocity);
                    //avel.Add(t.AngularVelocity);

                    //t.UserGuid = userGuid;
                    //formerQ.Enqueue(t);
                    //if(formerQ.Count>3)
                    //{
                    //    formerQ.Dequeue();
                    //}
                    timedServerObjectTransformList.ServerObjectTransformDict[t.Guid] = formerQ;
                    //t.Position = Vector3Json.Mean(pos);
                    //t.Rotation = QuaternionJson.Mean(rot);
                    //t.Velocity = Vector3Json.Mean(vel);
                    //t.AngularVelocity = Vector3Json.Mean(avel);
                    //res.Add(t);
                }
                //  t.UserGuid = userGuid;
                //timedServerObjectTransformList.ServerObjectTransformDict[t.Guid] = t;


            }
            ServerObjectDict[roomKey] = timedServerObjectTransformList;
            foreach (var t2 in timedServerObjectTransformList.ServerObjectTransformDict)
            {
                var q = t2.Value;
                if (q.Count > 0)
                {
                    var p = q.Peek();
                    var tt = res.Find(x => x.Guid == p.Guid);
                    if (tt == null)
                    {
                        // res.Add(p);
                    }
                }
            }

            return res;
        }
        public static ActionHolder UpdateActionsHolder(string roomKey, string userGuid, SceneAction action)
        {
            foreach (var item in ActionsDict.Where(kvp => kvp.Value.TimeStamp < DateTime.Now.AddMinutes(-5)).ToList())
            {
                ActionsDict.Remove(item.Key);
            }
            ActionHolder aHolder = null;
            if (!ActionsDict.TryGetValue(roomKey, out aHolder))
            {
                aHolder = new ActionHolder();
                aHolder.ActionsOwnerId = userGuid;
            }

            if (action != null)
            {
                if (action.GoName == "slam_start_recording")
                {
                    if (aHolder.Actions.Find(x => x.GoName == "slam_end_recording") != null)
                    {
                        aHolder.Actions.Clear();
                        aHolder.ActionsOwnerId = userGuid;
                    }
                }
                if (aHolder.ActionsOwnerId != userGuid)
                {
                    return null;
                }
                //if (action.GoName == "slam_end_recording")
                //{
                //    ActionsDict.Remove(roomKey);
                //    return null;
                //}

                var ac = aHolder.Actions.Find(x => x.GoName == action.GoName && x.SceneTime == action.SceneTime);
                if (ac == null)
                {
                    aHolder.Actions.Add(action);
                }
                aHolder.TimeStamp = DateTime.Now; //update time
                ActionsDict[roomKey] = aHolder;

            }
            return aHolder;
        }
        public static int GetRoomCount(string roomKey)
        {
            //lock (thisLock)
            //{
            Dictionary<string, TransFormHolder> tList = null;
            if (TransFormDict.TryGetValue(roomKey, out tList))
            {
                return tList.Where(kvp => kvp.Value.TimeStamp > DateTime.Now.AddSeconds(-roomCountTimeLimit) && kvp.Value.IdleCount < idleCountLimit).ToList().Count;
            }
            return 0;
            //}
        }
        public static void AppendCloseByUsers(StringBuilder sb, Dictionary<string, TransFormHolder> tList, SlmControls.Vector3 userPos, double mindistance, double maxdistance, ref int counter)
        {
            foreach (var i in tList)
            {
                string pos = i.Value.Position;
                SlmControls.Vector3 posv = SlmControls.Vector3.FromString(pos);
                if (posv != null)
                {

                    var dist = SlmControls.Vector3.Distance(posv, userPos);
                    if (dist >= mindistance && dist < maxdistance)
                    {
                        AppendUser(sb, i.Key, i.Value);
                        counter++;
                        if (counter > maxUsers)
                        {
                            return;
                        }
                    }
                }
            }
        }
        public static void AppendUser(StringBuilder sb, string key, TransFormHolder th)
        {
            if (th.IdleCount < idleCountLimit)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("avatar shown {0}, {1}", th.IdleCount, th.Rotation));
                //       TransFormHolder th = i.Value;
                sb.Append("<u>");
                sb.Append("<i>");
                sb.Append(key);
                sb.Append("</i>");

                sb.Append("<p>");
                sb.Append(th.Position);
                sb.Append("</p>");
                sb.Append("<r>");
                sb.Append(th.Rotation);
                sb.Append("</r>");
                sb.Append("<n>");
                sb.Append(th.NickName);
                sb.Append("</n>");
                sb.Append("<a>");
                sb.Append(th.AvatarNo);
                sb.Append("</a>");
                sb.Append("<d>");
                sb.Append(th.DissonanceGuid);
                sb.Append("</d>");
                AppendIfNotNull(sb, "lhp", th.LHPosition);
                AppendIfNotNull(sb, "lhr", th.LHRotation);
                AppendIfNotNull(sb, "rhp", th.RHPosition);
                AppendIfNotNull(sb, "rhr", th.RHRotation);
                string legacyAvatarUrl = null;
                if (HasLegacyAvatar(key, out legacyAvatarUrl))
                {
                    sb.Append("<l>");
                    sb.Append(legacyAvatarUrl);
                    sb.Append("</l>");
                }
                sb.Append("</u>");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("avatar not shown {0}, {1}", th.IdleCount, th.Rotation));

            }
        }
        static void AppendIfNotNull(StringBuilder sb, string tag, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                sb.Append("<" + tag + ">");
                sb.Append(value);
                sb.Append("</" + tag + ">");
            }
        }
        public static string MakeXml( Dictionary<string, TransFormHolder> tList, ActionHolder actionHolder, SlmControls.Vector3 userPos, PostI post, int Timezone_Offset, List<ServerObjectTransform> sotRes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>");
            sb.Append("<ro>");
            int counter = 0;
            if (tList != null)
            {
                if (tList.Count <= maxUsers || userPos == null)
                {
                    foreach (var i in tList)
                    {
                        AppendUser(sb, i.Key, i.Value);
                        if (counter > maxUsers)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    AppendCloseByUsers(sb, tList, userPos, 0, 2, ref counter);
                    if (counter < maxUsers)
                    {
                        AppendCloseByUsers(sb, tList, userPos, 2, 4, ref counter);
                    }
                    if (counter < maxUsers)
                    {
                        AppendCloseByUsers(sb, tList, userPos, 4, 10, ref counter);
                    }
                }
                if (actionHolder != null && actionHolder.Actions.Count > 0)
                {
                    sb.Append("<ah>");
                    foreach (var act in actionHolder.Actions)
                    {
                        sb.Append("<ac>");
                        sb.Append("<n>");
                        sb.Append(act.GoName);
                        sb.Append("</n>");
                        sb.Append("<t>");
                        sb.Append(act.SceneTime.ToString());
                        sb.Append("</t>");
                        sb.Append("</ac>");

                    }
                    sb.Append("</ah>");
                }
                if (post != null)
                {
                    sb.Append("<p>");
                    sb.Append("<dt>");
                    sb.Append(((DateTime)post.CreationDate).AddHours(Timezone_Offset));//.ToString("yyyy-mm-dd HH:mm")
                    sb.Append("</dt>");
                    sb.Append("<t>");
                    sb.Append(post.Text);
                    sb.Append("</t>");
                    sb.Append("<n>");
                    sb.Append(post.NickName);
                    sb.Append("</n>");
                    sb.Append("<g>");
                    sb.Append(post.Guid);
                    sb.Append("</g>");
                    sb.Append("</p>");
                }
                if (sotRes != null)
                {
                    sb.Append("<sol>");
                    foreach (var t in sotRes)
                    {
                        if (!string.IsNullOrWhiteSpace(t.Guid))
                        {
                            sb.Append("<so>");
                            sb.Append("<ug>");
                            sb.Append(t.UserGuid);
                            sb.Append("</ug>");
                            sb.Append("<g>");
                            sb.Append(t.Guid);
                            sb.Append("</g>");
                            sb.Append("<p>");
                            if (t.Position != null)
                            {
                                sb.Append(t.Position.ToString());
                            }
                            sb.Append("</p>");
                            sb.Append("<r>");
                            if (t.Rotation != null)
                            {
                                sb.Append(t.Rotation.ToString());
                            }
                            sb.Append("</r>");
                            sb.Append("<v>");
                            if (t.Velocity != null)
                            {
                                sb.Append(t.Velocity.ToString());
                            }
                            sb.Append("</v>");
                            sb.Append("<av>");
                            if (t.Velocity != null)
                            {
                                sb.Append(t.AngularVelocity.ToString());
                            }
                            sb.Append("</av>");
                            sb.Append("</so>");
                        }

                    }

                    sb.Append("</sol>");
                }
            }
            //if (kbText != null)
            //{
            //    sb.Append("<kb>");
            //    sb.Append(kbText);
            //    sb.Append("</kb>");

            //}
            sb.Append("</ro>");
            return sb.ToString();
        }
    }

}
