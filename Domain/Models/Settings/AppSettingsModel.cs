using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Settings
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? Version { get; set; }
        public List<DataBaseConnections> DataBaseConnections { get; set; }
        public List<ApiConnections> ApiConnections { get; set; }
    }
    public class DataBaseConnections
    {
        public string? Name { get; set; }
        public string? ConnectionString { get; set; }
    }
    public class ApiConnections
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
    }
}
