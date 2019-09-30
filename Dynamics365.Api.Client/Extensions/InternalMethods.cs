using Dynamics365.Api.Client.Attributes;
using Dynamics365.Api.Client.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dynamics365.Api.Client.Extensions
{
    internal static class InternalExtensionMethods
    {
        internal static object GetCrmValue(this Dictionary<string, object> attributes, BaseCrmFieldAttribute fieldAttribute)
        {
            switch (fieldAttribute.Type)
            {
                case FieldTypes.Decimal:
                    var decimalValue = attributes.ContainsKey(fieldAttribute.Name) ? Convert.ToDecimal(attributes[fieldAttribute.Name]) : (decimal?)null;
                    if (decimalValue != null)
                    {
                        return decimalValue.Value;
                    }
                    break;

                case FieldTypes.Double:
                    var doubleValue = attributes.ContainsKey(fieldAttribute.Name) ? Convert.ToDouble(attributes[fieldAttribute.Name]) : (double?)null;
                    if (doubleValue != null)
                    {
                        return doubleValue.Value;
                    }
                    break;

                case FieldTypes.DateTime:
                    var dtValue = attributes.ContainsKey(fieldAttribute.Name) ? Convert.ToDateTime(attributes[fieldAttribute.Name]) : (DateTime?)null;
                    if (dtValue != null)
                    {
                        return dtValue.Value;
                    }
                    break;

                case FieldTypes.Boolean:
                    var boolValue = attributes.ContainsKey(fieldAttribute.Name) ? Convert.ToBoolean(attributes[fieldAttribute.Name]) : (bool?)null;
                    if (boolValue != null)
                    {
                        return boolValue.Value;
                    }
                    break;

                case FieldTypes.String:
                case FieldTypes.Int:
                case FieldTypes.Option:
                    return attributes.ContainsKey(fieldAttribute.Name) ? attributes[fieldAttribute.Name] : null;

                case FieldTypes.Reference:
                    return attributes.ContainsKey(fieldAttribute.Name.ToReferenceEntityKey()) ? attributes[fieldAttribute.Name.ToReferenceEntityKey()] : null;
            }

            return null;
        }

        internal static string ToReferenceEntityKey(this string fieldName) => $"_{fieldName}_value";

        internal static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static bool HasValue(this Guid guid)
        {
            if (guid != null && guid != Guid.Empty)
            {
                return true;
            }

            return false;
        }

        internal static CrmEntityAttribute GetEntityAttribute(this Type entity)
        {
            var entityAttribute = entity.GetCustomAttribute<CrmEntityAttribute>();
            if (string.IsNullOrWhiteSpace(entityAttribute.LogicalName))
            {
                throw new ArgumentException($"{nameof(CrmEntityAttribute)} is missing from the entity class");
            }

            return entityAttribute;
        }

        internal static string Pluralize(this string logicalname) => logicalname.EndsWith("y") ? $"{logicalname.Substring(0, logicalname.Length - 1)}ies" : $"{logicalname}s";
    }
}
