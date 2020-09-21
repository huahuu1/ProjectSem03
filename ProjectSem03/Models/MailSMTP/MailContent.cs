using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSem03.Models.MailSMTP
{
    public class MailContent
    {
        public string To { get; set; }             
        public string Subject { get; set; }         
        public string Body { get; set; }  //support html
    }
}
