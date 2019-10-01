using Dynamics365.Api.Client.Attributes;
using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dynamics365.Api.Client.Queries
{
    public class GetQuery<TEntity, TValue> :
        Query,
        IFilterQueryWithoutCondition<TEntity, TValue>,
        IExecutableQuery<TEntity>,
        IExecutableQueryWithFilter<TEntity>
        where TEntity : BaseCrmEntity, new()
    {
        private Dictionary<string, BaseCrmFieldAttribute> _propertyCrmAttributeDict;
        private string _tempField;

        public string PluralName { get; private set; }
        public List<string> Fields { get; private set; } = new List<string>();
        public int? Top { get; private set; }
        public IFilter Filter { get; private set; }

        internal GetQuery()
        {
            _propertyCrmAttributeDict = new Dictionary<string, BaseCrmFieldAttribute>();

            foreach (var prop in typeof(TEntity).GetProperties())
            {
                var attribute = prop.GetCustomAttributes().FirstOrDefault(a => a.GetType().BaseType == typeof(BaseCrmFieldAttribute)) as BaseCrmFieldAttribute;
                _propertyCrmAttributeDict.Add(prop.Name, attribute);
            }
        }

        public IExecutableQuery<TEntity> Select(Expression<Func<TEntity, object>> propertyExpression)
        {
            Fields.Clear();
            Fields.AddRange(new List<string>(GetPropertyAttiributeFieldName(propertyExpression)));

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity, TValueNew> Where<TValueNew>(Expression<Func<TEntity, TValueNew>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();

            return DeepCopy<TValueNew>();
        }

        public IExecutableQuery<TEntity> Take(int take)
        {
            Top = take;

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> EqualsTo(TValue value)
        {
            ToFilter(FilterOperations.Equal, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> DoesNotEqualTo(TValue value)
        {
            ToFilter(FilterOperations.NotEqual, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> IsGreaterThan(TValue value)
        {
            ToFilter(FilterOperations.Greater, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> IsLessThan(TValue value)
        {
            ToFilter(FilterOperations.Less, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> IsGreaterThanOrEqual(TValue value)
        {
            ToFilter(FilterOperations.GreaterThanOrEqual, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> IsLessThanOrEqual(TValue value)
        {
            ToFilter(FilterOperations.LessThanOrEqual, value);

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity, TValueNew> And<TValueNew>(Expression<Func<TEntity, TValueNew>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            AddLogicalFilter(LogicalOperations.And);

            return DeepCopy<TValueNew>();
        }

        public IFilterQueryWithoutCondition<TEntity, TValueNew> Or<TValueNew>(Expression<Func<TEntity, TValueNew>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            AddLogicalFilter(LogicalOperations.Or);

            return DeepCopy<TValueNew>();
        }

        private void AddLogicalFilter(LogicalOperations logicalOperations)
        {
            var previousFilter = Filter as ComparisonFilter;

            Filter = new LogicalFilter
            {
                Left = new ComparisonFilter
                {
                    Field = previousFilter.Field,
                    Operation = previousFilter.Operation,
                    Value = previousFilter.Value
                },
                Operator = logicalOperations
            };
        }

        private void ToFilter(FilterOperations op, object value)
        {
            if (Filter == null)
            {
                Filter = new ComparisonFilter
                {
                    Field = _tempField,
                    Operation = op,
                    Value = value
                };
            }
            else if (Filter is LogicalFilter lf)
            {
                lf.Right = new ComparisonFilter
                {
                    Field = _tempField,
                    Operation = op,
                    Value = value
                };

                _tempField = null;
            }
        }

        private IEnumerable<string> GetPropertyAttiributeFieldName(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memExpression:
                    var attribute = _propertyCrmAttributeDict[memExpression.Member.Name];
                    var fieldName = attribute.Type == FieldTypes.Reference ? $"_{attribute.Name}_value" : attribute.Name;

                    return new List<string>
                    {
                        fieldName
                    };

                case LambdaExpression lambaExpression:
                    return GetPropertyAttiributeFieldName(lambaExpression.Body);

                case MethodCallExpression callExpression:
                    throw new ArgumentException("Method call expression not supported for property path evaluation");

                case UnaryExpression unaryExpression:
                    return GetPropertyAttiributeFieldName(unaryExpression.Operand);

                case NewExpression newExpression:
                    var lists = newExpression.Arguments.Select(aEx => GetPropertyAttiributeFieldName(aEx));
                    return lists.SelectMany(l => l);

                default:
                    return new List<string>();
            }
        }

        private IFilterQueryWithoutCondition<TEntity, TValueNew> DeepCopy<TValueNew>()
        {
            var newQ = new GetQuery<TEntity, TValueNew> { Filter = this.Filter, Fields = this.Fields, Top = this.Top };
            newQ._tempField = this._tempField;

            return newQ;
        }
    }
}
