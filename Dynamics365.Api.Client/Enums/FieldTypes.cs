using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Dynamics365.Api.Client.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldTypes
    {
        [EnumMember(Value = "string")]
        String,
        [EnumMember(Value = "int")]
        Int,
        [EnumMember(Value = "decimal")]
        Decimal,
        [EnumMember(Value = "bool")]
        Boolean,
        [EnumMember(Value = "datetime")]
        DateTime,
        [EnumMember(Value = "option")]
        Option,
        [EnumMember(Value = "double")]
        Double,
        [EnumMember(Value = "reference")]
        Reference
    }
}
