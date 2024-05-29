using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class NotifyResponseModel
    {
        public List<string>? MailResponses { get; set; }
        public List<string>? PhoneResponses { get; set; }
    }
}
