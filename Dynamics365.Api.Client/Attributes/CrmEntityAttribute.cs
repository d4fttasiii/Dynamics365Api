using Dynamics365.Api.Client.Extensions;
using System;

namespace Dynamics365.Api.Client.Attributes
{
    public class CrmEntityAttribute : Attribute
    {
        public string LogicalName { get; }

        public string PluralName { get; set; }

        public CrmEntityAttribute(string logicalName, string pluralName = "")
        {
            LogicalName = logicalName;
            PluralName = string.IsNullOrEmpty(pluralName) ? logicalName.Pluralize() : pluralName;
        }
    }
}
