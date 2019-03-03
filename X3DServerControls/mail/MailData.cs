using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls.mail
{
    public class MailData
    {
        public MailData() { }
        public MailData(string From, string To, DateTime SendDate, string Subject, string Body)
        {
            this.From = From;
            this.To = To;
            this.SendDate = SendDate;
            this.Subject = Subject;
            this.Body = Body;
        }

        public string From { get; set; }
        public string To { get; set; }
        public DateTime SendDate { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
