using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Queries;

namespace Dynamics365.Api.Client
{
    public class Query
    {
        public static IExecutableQuery<TEntity> From<TEntity>()
            where TEntity : BaseCrmEntity, new()
        {
            return new GetQuery<TEntity, object>();
        }
    }
}
