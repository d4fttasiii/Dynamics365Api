using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Dynamics365.Api.Client.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogicalOperations
    {
        [EnumMember(Value = "and")]
        And,
        [EnumMember(Value = "or")]
        Or,
        [EnumMember(Value = "not")]
        Not
    }
}
