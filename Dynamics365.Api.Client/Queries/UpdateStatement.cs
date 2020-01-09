using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics365.Api.Client.Queries
{
    public class UpdateStatement<TEntity>
        where TEntity : BaseCrmEntity
    {
        private TEntity _originalEntity;

        public Dictionary<string, object> OriginalValues { get; }
        public Dictionary<string, object> ChangedValues { get; }

        internal UpdateStatement(TEntity entity)
        {
            _originalEntity = entity;
        }
    }
}
