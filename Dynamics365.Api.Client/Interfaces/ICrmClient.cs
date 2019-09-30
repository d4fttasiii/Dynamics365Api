using Dynamics365.Api.Client.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dynamics365.Api.Client.Interfaces
{
    public interface ICrmClient
    {
        Task<Guid> CreateAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new();

        Task<BaseCrmEntity> GetByIdAsync(Type entityType, Guid id);
        Task<TEntity> GetByIdAsync<TEntity>(Guid id) where TEntity : BaseCrmEntity, new();
        Task<IEnumerable<TEntity>> GetByQueryAsync<TEntity>(IExecutableQuery<TEntity> query) where TEntity : BaseCrmEntity, new();
        Task<IEnumerable<TEntity>> GetFromView<TEntity>(string viewName) where TEntity : BaseCrmEntity, new();

        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new();

        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new();
        Task DeleteByIdAsync<TEntity>(Guid id) where TEntity : BaseCrmEntity, new();
        Task DeleteByIdAsync(string logicalName, Guid id);
    }
}
