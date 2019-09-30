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
    public class GetQuery<TEntity> :
        Query,
        IFilterQueryWithoutCondition<TEntity>,
        IExecutableQuery<TEntity>,
        IExecutableQueryWithFilter<TEntity>
        where TEntity : BaseCrmEntity, new()
    {
        private Dictionary<string, BaseCrmFieldAttribute> _propertyCrmAttributeDict;
        private string _tempField;

        public string PluralName { get; private set; }
        public List<string> Fields { get; } = new List<string>();
        public ComparisonFilter Condition { get; private set; }
        public int? Limit { get; private set; }
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

        public static IExecutableQuery<TEntity> From()
        {
            return new GetQuery<TEntity>();
        }

        public IExecutableQuery<TEntity> Select(Expression<Func<TEntity, object>> propertyExpression)
        {
            Fields.Clear();
            Fields.AddRange(new List<string>(GetPropertyAttiributeFieldName(propertyExpression)));

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity> Where<T>(Expression<Func<TEntity, T>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            return this;
        }

        public IExecutableQuery<TEntity> Top(int top)
        {
            Limit = top;

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> Equal(object value)
        {
            ToFilter(FilterOperations.Equal, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> NotEqual(object value)
        {
            ToFilter(FilterOperations.NotEqual, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> GreaterThan(object value)
        {
            ToFilter(FilterOperations.Greater, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> LessThan(object value)
        {
            ToFilter(FilterOperations.Less, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> GreaterThanOrEqual(object value)
        {
            ToFilter(FilterOperations.GreaterThanOrEqual, value);

            return this;
        }

        public IExecutableQueryWithFilter<TEntity> LessThanOrEqual(object value)
        {
            ToFilter(FilterOperations.LessThanOrEqual, value);

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity> And<T>(Expression<Func<TEntity, T>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            AddLogicalFilter(LogicalOperations.And);

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity> Or<T>(Expression<Func<TEntity, T>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            AddLogicalFilter(LogicalOperations.Or);

            return this;
        }

        private void AddLogicalFilter(LogicalOperations logicalOperations)
        {
            Filter = new LogicalFilter
            {
                Left = new ComparisonFilter
                {
                    Field = Condition.Field,
                    Operation = Condition.Operation,
                    Value = Condition.Value
                },
                Operator = logicalOperations
            };
        }

        private void ToFilter(FilterOperations op, object value)
        {
            if (Condition == null)
            {
                Condition = new ComparisonFilter
                {
                    Field = _tempField,
                    Operation = op,
                    Value = value
                };
                Filter = Condition;
            }
            else if (Filter is LogicalFilter lf)
            {
                lf.Right = new ComparisonFilter
                {
                    Field = _tempField,
                    Operation = op,
                    Value = value
                };

                Condition = null;
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
    }
}
