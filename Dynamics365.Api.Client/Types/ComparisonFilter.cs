using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Interfaces;

namespace Dynamics365.Api.Client.Types
{
    public class ComparisonFilter : IFilter
    {
        public string Field { get; set; }

        public FilterOperations Operation { get; set; }

        public object Value { get; set; }
    }
}
