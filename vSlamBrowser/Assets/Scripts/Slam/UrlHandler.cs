using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slam
{
    public class UrlHandler
    {
        public string LastBaseUrl = null;
        public string BaseUrl = "";
        public string Homepage = "default.aspx";
        public string HomePagePage = null;
        public string HomeUrl = null;
        public string FullSceneUrl = null;
        public bool AtHome
        {
            get
            {
                if (!string.IsNullOrEmpty(FullSceneUrl))
                {
                    return FullSceneUrl.ToLower().Contains(GetHomeUrl(false).ToLower());
                }
                return false;
            }
        }
            
        List<string> History = new List<string>();
        private string _currentPageUrl;
        public string CurrentPageUrl
        {
            get
            {
                return _currentPageUrl;
            }
            set
            {
                if(_currentPageUrl!=null)
                {
                    History.Add(_currentPageUrl);
                    LastBaseUrl = BaseUrl;
                }
                _currentPageUrl = value;
            }
        }

        public string InlineUrl { get; set; }
        public bool SingleUser = false;

        public string AudioUrl = null;
        public string DomainBaseUrl = "";
        public bool IsSameAsPreviousPage()
        {
            if(LastBaseUrl==null)
            {
                return false;
            }
            return LastBaseUrl == BaseUrl;
        }

        public string GetBaseUrl()
        {
            string url = Constants.BaseHomeUrl;
#if UNITY_EDITOR
    url = Constants.DevelopBaseHomeUrl;
#endif
            return url;
        }
        public string GetTyperUrl()
        {
            string url = Constants.TyperUrl;
#if UNITY_EDITOR

            url = Constants.DeveloperTyperUrl;
#endif
            return url;
        }
        public static string GetStrippedUrl(string url)
        {
            int pos = url.IndexOf("#");
            if (pos > 0)
            {
                url = url.Substring(0, pos);
            }
            pos = url.IndexOf("?");
            if (pos > 0)
            {
                url = url.Substring(0, pos);
            }
            return url;
        }
        public string GetHomeUrl(bool addPaging=true)
        {
            var url = HomeUrl;
            if (string.IsNullOrEmpty(HomeUrl))
            {
                url = GetBaseUrl() + Homepage;
            }
            if(addPaging && !string.IsNullOrEmpty(HomePagePage))
            {
                url = Statics.AddUrlProperty(url, "p", HomePagePage);
                //url += "?p=" + HomePagePage;
            }
            return url;
        }
        public string GetUrl(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("file://"))
            {
                return url;
            }
            if (url.Contains("."))
            {
                string tbaseUrl = BaseUrl;
                while (url.StartsWith("../"))
                {
                    tbaseUrl = tbaseUrl.Substring(0, tbaseUrl.LastIndexOf("/"));
                    url = url.Substring(3);
                }
                return tbaseUrl + "/" + url;
            }
            if (AtHome && !url.Contains("?"))
            {
                //url is search term:
                return Statics.AddUrlProperty( GetHomeUrl() ,"s", url);
            }
            else
            {
                return GetStrippedUrl(FullSceneUrl) + url;
            }
        }
        public bool AllowPost(string url)
        {
            if(IsCallWithinDomain(url) && url.Contains(".aspx"))
            {
                return true;
            }
            return false;
        }
        public bool IsCallWithinDomain(string url)
        {
            if(!string.IsNullOrEmpty(DomainBaseUrl))
            {
                var domain = GetDomain(url);
                if(string.IsNullOrEmpty(domain)|| domain==DomainBaseUrl)
                {
                    return true;
                }
            }
            return false;
        }
        public void SetUrlProperties(string url, Target target)
        {
            var domain = GetDomain(url);
            if(domain != null && target== Target._blank)
            {
                DomainBaseUrl = domain;
            }
            if (target == Target._blank)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    BaseUrl = url.Substring(0, url.LastIndexOf("/"));
                }
                if (HomeUrl == null)
                {
                    HomeUrl = url;
                }
            }
        }
        public string AssureFullUrl(string url)
        {
            if (url != null)
            {
                url = url.Replace("\"", "");
                if (url.StartsWith("http"))
                {

                }
                else
                {
                    url = BaseUrl + "/" + url;
                }
            }
            return url;
        }
        private string GetDomain(string url)
        {
            if (url!=null && url.StartsWith("http"))
            {
               return url.Length > 9 ? url.Substring(0, url.IndexOf("/", 9)).ToLower()   : null;
            }
            return null;
        }

        public string CheckInlineUrl(string url)
        {
            if (!string.IsNullOrEmpty(InlineUrl) && !string.IsNullOrEmpty(url) && !url.StartsWith("http") && !url.StartsWith("/"))
            {
                return InlineUrl+"/" + url;
            }
            return url;
        }
        public void ClearInlineUrl()
        {
            InlineUrl=null;
        }
        public string CheckTilde(string url)
        {
            if(!string.IsNullOrEmpty(url) && url.StartsWith("~/"))
            {
                return GetBaseUrl() + url.Substring(2);
            }
            return url;
        }
        public void SetInlineUrl(string url)
        {
            InlineUrl = null;
            if(!string.IsNullOrEmpty(url)&&!url.StartsWith("/") && url.Contains("/")) //&& !url.StartsWith("http")
            {
                InlineUrl = url.Substring(0, url.LastIndexOf("/"));

            }
        }
        public static string GetNamePart(string url)
        {
            if(!string.IsNullOrEmpty(url))
            {
                if(url.Contains("/") && url.LastIndexOf("/")<url.Length)
                {
                    string turl = url.Substring(url.LastIndexOf("/")+1);
                    if(turl.Contains("."))
                    {
                        turl = turl.Substring(0, turl.LastIndexOf("."));
                    }
                    return turl;
                }
            }
            return url;
        }
        public string GetRoomFromUrl(string url)
        {
            if(Slam.Instance.IsHigherOrEqualSlamversion(2.19f))
            {
                int pos = url.IndexOf("?");
                if (pos >= 0)
                {
                    var qparams = GetQueryParams(url);
                    var guid = "";
                    if(qparams.TryGetValue("g", out guid) && guid.Length>6)
                    {
                        return guid;
                    }
                    var turl = url.Substring(0, pos).ToLower().Replace("http://", "").Replace("https://", "");
                    if(turl.StartsWith("www."))
                    {
                        turl = turl.Substring(4);
                    }
                    if(turl.Length>6)
                    {
                        return turl;
                    }

                }
            }
            return url;
        }
        private Dictionary<string, string> GetQueryParams(string url)
        {
            Dictionary<string, string> qparams = new Dictionary<string, string>();
            string[] parts = url.Split(Convert.ToChar("?"));
            if(parts.Length>1)
            {
                url = parts[1];
            }
            string[] qparts = url.Split(Convert.ToChar("&"));
            foreach(var p in qparts)
            {
                string[] part=p.Split(Convert.ToChar("="));
                if(part.Length==2)
                {
                    qparams.Add(part[0], part[1]);
                }
            }

            return qparams; 
        }
    }
}
