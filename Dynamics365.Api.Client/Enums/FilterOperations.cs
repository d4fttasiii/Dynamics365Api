using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Dynamics365.Api.Client.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FilterOperations
    {
        [EnumMember(Value = "eq")]
        Equal,
        [EnumMember(Value = "ne")]
        NotEqual,
        [EnumMember(Value = "gt")]
        Greater,
        [EnumMember(Value = "ge")]
        GreaterThanOrEqual,
        [EnumMember(Value = "lt")]
        Less,
        [EnumMember(Value = "le")]
        LessThanOrEqual,

        [EnumMember(Value = "contains")]
        Contains,
        [EnumMember(Value = "endswith")]
        EndsWith,
        [EnumMember(Value = "startswith")]
        StartsWith
    }
}
