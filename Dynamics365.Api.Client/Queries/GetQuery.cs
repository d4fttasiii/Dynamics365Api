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
        IExecutableQuery<TEntity>
        where TEntity : BaseCrmEntity, new()
    {
        private Dictionary<string, BaseCrmFieldAttribute> _propertyCrmAttributeDict;
        private string _tempField;

        public string PluralName { get; set; }
        public List<string> Fields { get; set; } = new List<string>();
        public ComparisonFilter Condition { get; set; }
        public int? Limit { get; set; }

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
            Fields = new List<string>(GetPropertyAttiributeFieldName(propertyExpression));

            return this;
        }

        public IFilterQueryWithoutCondition<TEntity> Filter<T>(Expression<Func<TEntity, T>> propertyExpression)
        {
            _tempField = GetPropertyAttiributeFieldName(propertyExpression).FirstOrDefault();
            return this;
        }

        public IExecutableQuery<TEntity> Top(int top)
        {
            Limit = top;

            return this;
        }

        public IExecutableQuery<TEntity> Equal(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.Equal,
                Value = value
            };

            return this;
        }

        public IExecutableQuery<TEntity> NotEqual(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.NotEqual,
                Value = value
            };

            return this;
        }

        public IExecutableQuery<TEntity> GreaterThan(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.Greater,
                Value = value
            };

            return this;
        }

        public IExecutableQuery<TEntity> LessThan(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.Less,
                Value = value
            };

            return this;
        }

        public IExecutableQuery<TEntity> GreaterThanOrEqual(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.GreaterThanOrEqual,
                Value = value
            };

            return this;
        }

        public IExecutableQuery<TEntity> LessThanOrEqual(object value)
        {
            Condition = new ComparisonFilter
            {
                Field = _tempField,
                Operation = FilterOperations.LessThanOrEqual,
                Value = value
            };

            return this;
        }

        private IEnumerable<string> GetPropertyAttiributeFieldName(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memExpression:
                    var attribute = _propertyCrmAttributeDict[memExpression.Member.Name];
                    return new List<string>
                    {
                        attribute.Name
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

        IExecutableQuery<TEntity> IExecutableQuery<TEntity>.Select(Expression<Func<TEntity, object>> propertyExpression)
        {
            throw new NotImplementedException();
        }
    }
}
