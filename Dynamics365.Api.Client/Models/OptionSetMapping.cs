using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dynamics365.Api.Client.Models
{
    public class OptionSetMap
    {
        public ConcurrentDictionary<string, List<OptionSet>> OptionSetDict { get; set; }
    }

    public class OptionSet
    {
        public string Name { get; set; }
        public Mapping Mapping { get; set; }
    }

    public class Mapping
    {
        public int Crm { get; set; }
        public object Value { get; set; }
    }
}
