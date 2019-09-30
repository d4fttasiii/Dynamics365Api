using Dynamics365.Api.Client.Attributes;
using System;

namespace Dynamics365.Api.Client.Exceptions
{
    public class CrmMappingException : Exception
    {
        public BaseCrmFieldAttribute FieldAttribute { get; set; }
        public object Value { get; set; }

        public CrmMappingException(string message) : base(message) { }
        public CrmMappingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
