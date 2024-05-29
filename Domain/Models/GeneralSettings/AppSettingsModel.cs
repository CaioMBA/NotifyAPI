using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.GeneralSettings
{
    public class AppSettingsModel
    {
        public string? AppName { get; set; }
        public double? AppVersion { get; set; }
        public List<DataBaseConnections> DataBaseConnections { get; set; }
        public List<ApiConnections> ApiConnections { get; set; }
    }
    public class DataBaseConnections
    {
        public required string DataBaseID { get; set; }
        public required string Type { get; set; }
        public required string ConnectionString { get; set; }
        public string? Name { get; set; }
        public string? Collection { get; set; }
    }
    public class ApiConnections
    {
        public string? ApiID { get; set; }
        public required string Url { get; set; }
        public required string Type { get; set; }
    }
}
