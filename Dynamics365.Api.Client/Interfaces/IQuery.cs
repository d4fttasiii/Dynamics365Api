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
        IExecutableQueryWithFilter<TEntity> Equal(object value);
        
        IExecutableQueryWithFilter<TEntity> NotEqual(object value);
        
        IExecutableQueryWithFilter<TEntity> GreaterThan(object value);
        
        IExecutableQueryWithFilter<TEntity> LessThan(object value);
        
        IExecutableQueryWithFilter<TEntity> GreaterThanOrEqual(object value);
        
        IExecutableQueryWithFilter<TEntity> LessThanOrEqual(object value);
    }

    public interface IExecutableQuery<TEntity> : IQuery
        where TEntity : BaseCrmEntity, new()
    {
        string PluralName { get; }

        List<string> Fields { get; }

        IFilter Filter { get; }

        int? Limit { get; }

        IExecutableQuery<TEntity> Select(Expression<Func<TEntity, object>> propertyExpression);

        IFilterQueryWithoutCondition<TEntity> Where<T>(Expression<Func<TEntity, T>> propertyExpression);
    }

    public interface IExecutableQueryWithFilter<TEntity> : IExecutableQuery<TEntity>
        where TEntity : BaseCrmEntity, new()
    {
        IFilterQueryWithoutCondition<TEntity> And<T>(Expression<Func<TEntity, T>> propertyExpression);

        IFilterQueryWithoutCondition<TEntity> Or<T>(Expression<Func<TEntity, T>> propertyExpression);
    }
}
