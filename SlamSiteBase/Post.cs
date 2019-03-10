using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamSiteBase
{
    public class Post : PostI
    {
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
        public string NickName { get; set; }
        public string Guid { get; set; }
    }
    public interface PostI
    {
        DateTime CreationDate { get; set; }
        string Text { get; set; }
        string NickName { get; set; }
        string Guid { get; set; }
    }
}
