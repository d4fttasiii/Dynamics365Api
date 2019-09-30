using System;
using System.Collections.Generic;

namespace Dynamics365.Api.Client.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }

        string LogicalName { get; }

        Dictionary<string, object> Attributes { get; }

        DateTime CreatedOn { get; }

        DateTime ModifiedOn { get; }

        void CommitChanges();
    }
}
