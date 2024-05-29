using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.GeneralSettings
{
    public class NonRelationalDataBaseSearchConfig
    {
        public Dictionary<string, object?>? filters { get; set; }
        public List<String>? ExcludeTags { get; set; }
        public List<String>? IncludeTags { get; set; }
        public string? SortIndex { get; set; }
        public bool? SortDescending { get; set; }
        public int? Limit { get; set; }
    }
}
