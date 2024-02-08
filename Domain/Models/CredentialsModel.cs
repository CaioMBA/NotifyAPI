using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CredentialsModel
    {
        public long? NOTIFYCREDENTIALID { get; set; }
        public string? CREDENTIAL { get; set; }
        public string? PASSWORD { get; set; }
        public string? TYPE { get; set; }
        public string? PROTOCOL { get; set; }
        public int? ACTIVE { get; set; }
    }
}
