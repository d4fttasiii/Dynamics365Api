using Dynamics365.Api.Client.Attributes;
using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Exceptions;
using Dynamics365.Api.Client.Extensions;
using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Services;
using Dynamics365.Api.Client.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dynamics365.Api.Client
{
    public class BaseCrmEntity : IEntity
    {
        private readonly Dictionary<string, Action<ICrmClient>> _lazyLoadingActions = new Dictionary<string, Action<ICrmClient>>();

        public Guid Id { get; set; }
        public string LogicalName { get; }
        public string PluralName { get; }
        public Dictionary<string, object> Attributes { get; }

        [CrmDateTimeField("createdon")]
        public DateTime CreatedOn { get; private set; }

        [CrmDateTimeField("modifiedon")]
        public DateTime ModifiedOn { get; private set; }

        /// <summary>
        /// You can add specific crm field --> property mapping logic related to the entity's certain field.
        /// Example: add 3 zeros to the crefo number before call property set
        /// </summary>
        public Dictionary<string, Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>> FieldSpecificPropertyMappers { get; }
            = new Dictionary<string, Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>>();

        /// <summary>
        /// You can add specific crm field --> property mapping logic to handle certain type conversions between the crm and the property's type
        /// </summary>
        public Dictionary<(FieldTypes, Type), Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>> TypeSpecificPropertyMappers { get; }
            = new Dictionary<(FieldTypes, Type), Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>>
        {
            { (FieldTypes.Option, typeof(int)),
                (fieldAttr, crmValue, prop, instance) =>
                {
                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        var intOptionSetValue = Convert.ToInt32(OptionSetMapper.FromOptionSet((int)crmValue, fieldAttr.OptionSetName));
                        prop.SetValue(instance, intOptionSetValue);
                    }
                    else if (prop.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(prop.PropertyType, OptionSetMapper.FromOptionSet((int)crmValue, fieldAttr.OptionSetName).ToString());
                        prop.SetValue(instance, enumValue);
                    }
                    // Handle nullable Enum types
                    else if(prop.PropertyType.IsNullableType() && prop.PropertyType.GenericTypeArguments.First().IsEnum)
                    {
                         var enumValue = Enum.Parse(prop.PropertyType.GenericTypeArguments.First(), OptionSetMapper.FromOptionSet((int)crmValue, fieldAttr.OptionSetName).ToString());
                        prop.SetValue(instance, enumValue);
                    }
                    else
                    {
                        prop.SetValue(instance, OptionSetMapper.FromOptionSet((int)crmValue, fieldAttr.OptionSetName));
                    }
                }
            },
            {
                (FieldTypes.DateTime, typeof(DateTime)), (fieldAttr, crmValue, prop, instance) =>
                {
                    if (prop.PropertyType == typeof(int) && crmValue is DateTime date)
                    {
                        prop.SetValue(instance, date.Year);
                    }
                    else
                    {
                        prop.SetValue(instance, crmValue);
                    }
                }
            },
            {
                (FieldTypes.Reference, typeof(Reference)), (fieldAttr, crmValue, prop, instance) =>
                {
                    var er = crmValue as Reference;
                    // Setting a placeholder typed entity for lazy loading
                    var referencedEntityType = prop.PropertyType;
                    var referencedEntity = Activator.CreateInstance(referencedEntityType, er.Id);
                    // Adding actions to lazy load all related entites
                    instance._lazyLoadingActions[fieldAttr.Name] = (client) =>
                    {
                        var loadedChild = client.GetByIdAsync(referencedEntityType, er.Id).ConfigureAwait(false).GetAwaiter().GetResult();
                        var refEntityWithData = Activator.CreateInstance(referencedEntityType, loadedChild);
                        prop.SetValue(instance, refEntityWithData);
                    };
                    prop.SetValue(instance, referencedEntity);
                }
            }
        };

        public Dictionary<FieldTypes, Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>> PropertyFieldMappers { get; }
            = new Dictionary<FieldTypes, Action<BaseCrmFieldAttribute, object, PropertyInfo, BaseCrmEntity>>
        {
            { FieldTypes.Double, (fieldAttr, crmValue, prop, instance) => prop.SetValue(instance, crmValue) },
            { FieldTypes.String, (fieldAttr, crmValue, prop, instance) => prop.SetValue(instance, crmValue?.ToString() ?? string.Empty) },
            { FieldTypes.Int,
                (fieldAttr, crmValue, prop, instance) =>
                {
                    if (crmValue is int intValue)
                    {
                        prop.SetValue(instance, intValue);
                    }
                    else
                    {
                        prop.SetValue(instance, Convert.ToInt32(crmValue));
                    }
                }
            },
            { FieldTypes.Decimal, (fieldAttr, crmValue, prop, instance) => prop.SetValue(instance, crmValue) },
            { FieldTypes.Boolean,
                (fieldAttr, crmValue, prop, instance) =>
                {
                    if (crmValue is bool boolValue)
                    {
                        prop.SetValue(instance, boolValue);
                    }
                    else
                    {
                        prop.SetValue(instance, Convert.ToBoolean(crmValue));
                    }
                }
            },
            { FieldTypes.DateTime,
                (fieldAttr, crmValue, prop, instance) =>
                {
                    if (crmValue is DateTime dateTime)
                    {
                        prop.SetValue(instance, dateTime);
                    }
                    else
                    {
                        prop.SetValue(instance, Convert.ToDateTime(crmValue));
                    }
                }
            }
        };

        /// <summary>
        /// You can add specific property --> crm field mapping logic related to the entity's certain field.
        /// Example: do a substring before saving the value in the kap_description field
        /// </summary>
        public Dictionary<string, Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>> FieldSpecificCrmFieldMappers { get; }
            = new Dictionary<string, Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>>();

        /// <summary>
        /// You can add specific property --> crm field mapping logic to handle certain type conversions between the propery and the crm field's type
        /// </summary>
        public Dictionary<(FieldTypes, Type), Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>> TypeSpecificaCrmFieldMappers { get; }
            = new Dictionary<(FieldTypes, Type), Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>>
        {
            { (FieldTypes.DateTime, typeof(DateTime)), (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = (propValue is int year) ? new DateTime(year, 1, 1) : propValue },
        };

        public Dictionary<FieldTypes, Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>> CrmFieldMappers { get; }
            = new Dictionary<FieldTypes, Action<BaseCrmFieldAttribute, object, PropertyInfo, Dictionary<string, object>>>
        {
            { FieldTypes.Option,
               (fieldAttr, propValue, prop, attributes) =>
               {
                   attributes[fieldAttr.Name] = propValue is Enum e?
                       OptionSetMapper.ToOptionSet(Enum.GetName(propValue.GetType(), propValue), fieldAttr.OptionSetName) :
                       OptionSetMapper.ToOptionSet(propValue, fieldAttr.OptionSetName);
               }
            },
            { FieldTypes.Reference,
                (fieldAttr, propValue, prop, attributes) =>
                {
                    if (propValue is BaseCrmEntity referencedEntity)
                    {
                        attributes[fieldAttr.Name] = referencedEntity.Id;
                    }
                }
            },
            { FieldTypes.String, (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = propValue.ToString() },
            { FieldTypes.Int, (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = propValue },
            { FieldTypes.Double, (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = propValue },
            { FieldTypes.Decimal, (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = propValue },
            { FieldTypes.Boolean, (fieldAttr, propValue, prop, attributes) => attributes[fieldAttr.Name] = propValue }
        };

        public BaseCrmEntity(Dictionary<string, object> attributes = null)
        {
            var attribute = GetType().GetEntityAttribute();

            LogicalName = attribute.LogicalName;
            PluralName = attribute.PluralName;
            Attributes = (attributes ?? new Dictionary<string, object>())
                .Where(d => d.Value != null)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            CreatedOn = DateTime.Now;
            ModifiedOn = DateTime.Now;

            PopulateProperties();
        }

        public BaseCrmEntity(Guid id, Dictionary<string, object> attributes = null) : this(attributes)
        {
            Id = id;
        }

        private void PopulateProperties()
        {
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes().FirstOrDefault(a => a.GetType().BaseType == typeof(BaseCrmFieldAttribute)) is BaseCrmFieldAttribute fieldAttr)
                {
                    var value = Attributes.GetCrmValue(fieldAttr);
                    if (value == null)
                    {
                        continue;
                    }

                    // Transform the crm value and set the property with a field specific action
                    if (FieldSpecificPropertyMappers.ContainsKey(fieldAttr.Name))
                    {
                        try
                        {
                            var mapperFn = FieldSpecificPropertyMappers[fieldAttr.Name];
                            mapperFn(fieldAttr, value, prop, this);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    // Transform the crm value and set the property with a property type and crm type specific action
                    if (TypeSpecificPropertyMappers.ContainsKey((fieldAttr.Type, value.GetType())))
                    {
                        try
                        {
                            var mapperFn = TypeSpecificPropertyMappers[(fieldAttr.Type, value.GetType())];
                            mapperFn(fieldAttr, value, prop, this);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    // Transform the crm value and set the property with a crm type specific action
                    if (PropertyFieldMappers.ContainsKey(fieldAttr.Type))
                    {
                        try
                        {
                            var mapperFn = PropertyFieldMappers[fieldAttr.Type];
                            mapperFn(fieldAttr, value, prop, this);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    throw new CrmMappingException($"{value} cannot be transformed")
                    {
                        FieldAttribute = fieldAttr,
                        Value = value
                    };
                }
            }
        }

        public void CommitChanges()
        {
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes().FirstOrDefault(a => a.GetType().BaseType == typeof(BaseCrmFieldAttribute)) is BaseCrmFieldAttribute fieldAttr)
                {
                    var value = prop.GetValue(this);
                    if (value == null)
                    {
                        continue;
                    }

                    switch (value)
                    {
                        case string str when string.IsNullOrEmpty(str):
                            continue;
                        case DateTime dateTime when dateTime == DateTime.MinValue:
                            continue;
                        case Guid g when g == Guid.Empty:
                            continue;
                    }

                    // Mapping the property's content back to the crm entity with a field specific method
                    if (FieldSpecificCrmFieldMappers.ContainsKey(fieldAttr.Name))
                    {
                        try
                        {
                            var mapperFn = FieldSpecificCrmFieldMappers[fieldAttr.Name];
                            mapperFn(fieldAttr, value, prop, Attributes);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed back to CRM specific type for {fieldAttr.Name}", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    // Mapping the property's content back to the crm entity with a field after type transformation
                    if (TypeSpecificaCrmFieldMappers.ContainsKey((fieldAttr.Type, value.GetType())))
                    {
                        try
                        {
                            var mapperFn = TypeSpecificaCrmFieldMappers[(fieldAttr.Type, value.GetType())];
                            mapperFn(fieldAttr, value, prop, Attributes);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed back to CRM specific type for {fieldAttr.Name}", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    // Trying to map the property's content back to the crm entity with the standard handlers
                    if (CrmFieldMappers.ContainsKey(fieldAttr.Type))
                    {
                        try
                        {
                            var mapperFn = CrmFieldMappers[fieldAttr.Type];
                            mapperFn(fieldAttr, value, prop, Attributes);
                        }
                        catch (Exception ex)
                        {
                            throw new CrmMappingException($"{value} cannot be transformed back to CRM specific type for {fieldAttr.Name}", ex)
                            {
                                FieldAttribute = fieldAttr,
                                Value = value
                            };
                        }

                        continue;
                    }

                    throw new CrmMappingException($"{value} cannot be transformed back to CRM specific type for {fieldAttr.Name}")
                    {
                        FieldAttribute = fieldAttr,
                        Value = value
                    };
                }
            }
        }
    }
}
