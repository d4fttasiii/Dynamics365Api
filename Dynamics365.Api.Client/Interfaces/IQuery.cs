using Dynamics365.Api.Client.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dynamics365.Api.Client.Interfaces
{
    public interface IQuery { }

    public interface IFilterQueryWithoutCondition<TEntity> : IQuery
        where TEntity : BaseCrmEntity, new()
    {
        IExecutableQuery<TEntity> Equal(object value);

        IExecutableQuery<TEntity> NotEqual(object value);

        IExecutableQuery<TEntity> GreaterThan(object value);

        IExecutableQuery<TEntity> LessThan(object value);

        IExecutableQuery<TEntity> GreaterThanOrEqual(object value);

        IExecutableQuery<TEntity> LessThanOrEqual(object value);
    }

    public interface IExecutableQuery<TEntity> : IQuery
        where TEntity : BaseCrmEntity, new()
    {
        string PluralName { get; }

        List<string> Fields { get; }

        ComparisonFilter Condition { get; }

        IFilter Filter { get; }

        int? Limit { get; }

        IExecutableQuery<TEntity> Select(Expression<Func<TEntity, object>> propertyExpression);

        IFilterQueryWithoutCondition<TEntity> AddFilter<T>(Expression<Func<TEntity, T>> propertyExpression);
    }
}
