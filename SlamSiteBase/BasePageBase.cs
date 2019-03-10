using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlmControls;

namespace SlamSiteBase
{
    public class BasePageBase: System.Web.UI.Page
    {
        public const string RH_Timezone_Offset = "TimeZone-Offset";
        public const string RH_Calling_Device = "Calling-Device";
        public const string RH_USER_AGENT = "USER-AGENT";

        protected float SlamVersion = 2.42f;
        protected CallingDevices CallingApp = CallingDevices.PC;
        protected string UserAgent = null;
        protected int Timezone_Offset = 0;
        protected Dictionary<string, string> FormFields = new Dictionary<string, string>();
        protected void ReadFormFields()
        {
            foreach (var fld in Request.Form)
            {
                var a = fld.ToString();
                var b = Request.Form.GetValues(a);
                if (b.Length > 0)
                {
                    FormFields[a] = b[0];
                }
            }
        }
        protected void ReadRequestHeaders()
        {
            CallingApp = Calc.GetCallingDevice(Request.Headers[RH_Calling_Device]);
            UserAgent = Request.Headers[RH_USER_AGENT];
            if (!string.IsNullOrWhiteSpace(UserAgent))
            {
                List<string> uaList = UserAgent.Split(Convert.ToChar(" ")).ToList<string>();
                var slamItem = uaList.Find(x => x.ToLower().Contains("vslam"));
                if (!string.IsNullOrWhiteSpace(slamItem))
                {
                    var parts = slamItem.Split(Convert.ToChar("/"));
                    if (parts.Length > 1)
                    {
                        float.TryParse(parts[1], out SlamVersion);
                    }
                }
            }
            var tzo = Request.Headers[RH_Timezone_Offset];
            if (tzo != null)
            {
                int.TryParse(tzo, out Timezone_Offset);
            }
        }
        protected string GetUserIP()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }
        protected new bool IsPostBack
        {
            get
            {
                return FormFields.Count > 0 || Request.Url.ToString().Contains("#");
            }
        }

    }
}
