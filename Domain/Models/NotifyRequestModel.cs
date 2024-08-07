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
        public required List<string> MailDestinations { get; set; }
        public string? Subject { get; set; }

        public string? Msg { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
    public class Attachment
    {
        public required string FileName { get; set; }
        public required string Base64File { get; set; }
    }
    public class SendPhoneMsgModel
    {
        public required List<string> Phones { get; set; }
        public string? Type { get; set; }
        public string? Msg { get; set; }
    }
}
