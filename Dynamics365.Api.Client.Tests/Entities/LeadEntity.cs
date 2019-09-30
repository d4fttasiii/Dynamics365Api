using Dynamics365.Api.Client.Attributes;
using System;
using System.Collections.Generic;

namespace Dynamics365.Api.Client.Tests.Entities
{
    [CrmEntity("lead")]
    public class LeadEntity : BaseCrmEntity
    {
        public LeadEntity() : base() { }

        public LeadEntity(Guid id, Dictionary<string, object> attributes = null) : base(id, attributes) { }

        [CrmStringField("address1_line1")]
        public string Street { get; set; }

        [CrmStringField("address1_city")]
        public string City { get; set; }

        [CrmStringField("address1_postalcode")]
        public string Zip { get; set; }

        [CrmDecimalField("budgetamount")]
        public decimal? BudgetAmount { get; set; }

        [CrmOptionField("test_sync_state", "test_sync_states")]
        public SyncStates SyncState { get; set; }

        [CrmReferenceField("parentaccountid")]
        public virtual AccountEntity ParentAccount { get; set; }
    }

    public enum SyncStates
    {
        Synchronized = 1,
        OutOfSync = 2,
        Error = 3
    }
}
