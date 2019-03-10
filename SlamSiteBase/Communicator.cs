using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamSiteBase
{
    public abstract class Communicator: BasePageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReadRequestHeaders();
            string userData = Request["u"]; //User- and avatar data
            string text = Request["ci"]; //New text posted on community board 
            string lastMessageGuid = Request["lmg"]; //The guid from the last community message that was returned to the user
            string communityBoardPage = Request["p"]; //Current page for the community board
            string leftHandPosition = Request["lhp"]; //position for the left hand controller
            string leftHandRotation = Request["lhr"]; //rotation for the left hand controller
            string rightHandPosition = Request["rhp"]; //position for the right hand controller
            string rightHandRotation = Request["rhr"]; //rotation for the right hand controller

            string postData = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            List<SlamSiteBase.ServerObjectTransform> sot = null;
            if (userData != null)
            {
                if (!string.IsNullOrWhiteSpace(postData)) //Handling exchange of game object transformations
                {
                    var tp = Server.UrlDecode(postData);
                    var tsot = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SlamSiteBase.ServerObjectTransform>>(tp);
                    if (tsot.Count > 0)
                    {
                        sot = tsot;
                    }
                }
                string[] uS = userData.Split(Convert.ToChar("|"));
                if (uS.Length > 3)
                {
                    string room = uS[0];
                    string userGuid = uS[1];
                    string pos = uS[2];
                    string rot = uS[3];
                    SlamSiteBase.SceneAction action = null;
                    SlamSiteBase.TransFormHolder transformHolder = new SlamSiteBase.TransFormHolder();
                    if (uS.Length > 4)
                    {
                        transformHolder.NickName = uS[4].Trim();
                    }
                    if (uS.Length > 5)
                    {
                        transformHolder.AvatarNo = uS[5].Trim();
                    }
                    if (uS.Length > 6)
                    {
                        transformHolder.DissonanceGuid = uS[6].Trim();
                    }
                    if (uS.Length > 7)
                    {
                        string actionName = uS[7].Trim();
                        string sceneTimeS = uS[8].Trim();
                        float sceneTime = 0;
                        if (float.TryParse(sceneTimeS, out sceneTime) && !string.IsNullOrEmpty(actionName))
                        {
                            action = new SlamSiteBase.SceneAction();
                            action.GoName = actionName;
                            action.SceneTime = sceneTime;
                        }
                    }
                    transformHolder.Position = pos;
                    transformHolder.Rotation = rot;
                    transformHolder.LHPosition = leftHandPosition;
                    transformHolder.LHRotation = leftHandRotation;
                    transformHolder.RHPosition = rightHandPosition;
                    transformHolder.RHRotation = rightHandRotation;
                    transformHolder.TimeStamp = DateTime.Now;
                    int pageNo = 0;
                    PostI post = null;
                    if (communityBoardPage != null)
                    {
                        int.TryParse(communityBoardPage.ToString(), out pageNo);
                        post = UpdateCommunityInfo( room, text, transformHolder.NickName, lastMessageGuid, pageNo);
                    }
                    Response.Clear();
                    Response.ContentType = "text/xml";
                    Response.Write(GlobalBase.UpdateTransFormHolder(room, userGuid, transformHolder, action, pos, post, Timezone_Offset, sot));
                    Response.End();
                }
            }

        }
        /// <summary>
        /// Implement this method to use communication boards in scenes. The method should save an incomming post, and return the post 
        /// following the post having the lastMessageGuid
        /// </summary>
        /// <param name="room">An indicator for the site, normally the URL</param>
        /// <param name="text">Text from a new post</param>
        /// <param name="nickName">The nickName from the one who send the post</param>
        /// <param name="lastMessageGuid">A guid indicating the last post recieved</param>
        /// <param name="pageNo">The page number when scrolled to earlier pages of the posts</param>
        /// <returns></returns>
        public abstract PostI UpdateCommunityInfo(string room, string text, string nickName, string lastMessageGuid, int pageNo);
    }
}
