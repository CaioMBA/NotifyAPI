using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CredentialInfoModel
    {
        public required long CredentialID { get; set; }
        public required string Credential { get; set; }
        public string? SecretKey { get; set; }
        public required string Type { get; set; }
        public required string Protocol { get; set; }
        public required bool Active { get; set; }
        public DateTime? DateCreation { get; set; }
    }
}
