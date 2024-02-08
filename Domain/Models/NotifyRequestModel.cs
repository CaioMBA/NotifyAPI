using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class NotifyRequestModel
    {
        public List<SendMailModel>? Mails { get; set; }
        public List<SendPhoneMsgModel>? Phones { get; set; }
    }
    public class SendMailModel
    {
        public List<string>? MailDestinations { get; set; }
        public string? Subject { get; set; }

        public string? Msg { get; set; }
    }
    public class SendPhoneMsgModel
    {
        public string? Name { get; set; }
        public List<string>? Phones { get; set; }
        public string? Type { get; set; }
        public string? Msg { get; set; }
    }
}
