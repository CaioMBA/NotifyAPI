using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Settings
{
    public class ApiRequestModel
    {
        public string? Url { get; set; }
        public string? TypeRequest { get; set; }
        public string? Body { get; set; }
        public List<CustomHeaderModel> Headers { get; set; }
        public AuthApiModel Auth { get; set; }


    }
    public class AuthApiModel
    {
        public string? Type { get; set; }
        public string? Authorization { get; set; }
    }
    public class CustomHeaderModel
    {

        public string? Header { get; set; }
        public string? Value { get; set; }
    }
}
