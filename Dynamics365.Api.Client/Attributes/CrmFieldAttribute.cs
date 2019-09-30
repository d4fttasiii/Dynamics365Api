using Dynamics365.Api.Client.Enums;
using System;

namespace Dynamics365.Api.Client.Attributes
{
    public abstract class BaseCrmFieldAttribute : Attribute
    {
        public string Name { get; }
        public FieldTypes Type { get; }
        public string OptionSetName { get; }

        public BaseCrmFieldAttribute(string name, FieldTypes type, string optionSetName = "")
        {
            Name = name;
            Type = type;
            OptionSetName = optionSetName;
        }
    }

    public class CrmStringFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmStringFieldAttribute(string name) : base(name, FieldTypes.String) { }
    }

    public class CrmOptionFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmOptionFieldAttribute(string name, string optionSetName) : base(name, FieldTypes.Option, optionSetName) { }
    }

    public class CrmIntFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmIntFieldAttribute(string name) : base(name, FieldTypes.Int) { }
    }

    public class CrmDoubleFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmDoubleFieldAttribute(string name) : base(name, FieldTypes.Double) { }
    }

    public class CrmDecimalFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmDecimalFieldAttribute(string name) : base(name, FieldTypes.Decimal) { }
    }

    public class CrmBoolFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmBoolFieldAttribute(string name) : base(name, FieldTypes.Boolean) { }
    }

    public class CrmDateTimeFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmDateTimeFieldAttribute(string name) : base(name, FieldTypes.DateTime) { }
    }

    public class CrmReferenceFieldAttribute : BaseCrmFieldAttribute
    {
        public CrmReferenceFieldAttribute(string name) : base(name, FieldTypes.Reference) { }
    }
}
