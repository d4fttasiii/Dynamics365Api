using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dynamics365.Api.Client.Interfaces
{
    public interface IQuery { }

    public interface IFilterQueryWithoutCondition<TEntity, TValue> : IQuery
        where TEntity : BaseCrmEntity, new()
    {
        IExecutableQueryWithFilter<TEntity> EqualsTo(TValue value);
        
        IExecutableQueryWithFilter<TEntity> DoesNotEqualTo(TValue value);
        
        IExecutableQueryWithFilter<TEntity> IsGreaterThan(TValue value);
        
        IExecutableQueryWithFilter<TEntity> IsLessThan(TValue value);
        
        IExecutableQueryWithFilter<TEntity> IsGreaterThanOrEqual(TValue value);
        
        IExecutableQueryWithFilter<TEntity> IsLessThanOrEqual(TValue value);
    }

    public interface IExecutableQuery<TEntity> : IQuery
        where TEntity : BaseCrmEntity, new()
    {
        string PluralName { get; }

        List<string> Fields { get; }

        IFilter Filter { get; }

        int? Top { get; }

        IExecutableQuery<TEntity> Select(Expression<Func<TEntity, object>> propertyExpression);

        IFilterQueryWithoutCondition<TEntity, TValue> Where<TValue>(Expression<Func<TEntity, TValue>> propertyExpression);

        IExecutableQuery<TEntity> Take(int take = 50);
    }

    public interface IExecutableQueryWithFilter<TEntity> : IExecutableQuery<TEntity>
        where TEntity : BaseCrmEntity, new()
    {
        IFilterQueryWithoutCondition<TEntity, TValue> And<TValue>(Expression<Func<TEntity, TValue>> propertyExpression);

        IFilterQueryWithoutCondition<TEntity, TValue> Or<TValue>(Expression<Func<TEntity, TValue>> propertyExpression);
    }
}
