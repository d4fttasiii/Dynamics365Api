using Dynamics365.Api.Client.Attributes;
using System;
using System.Collections.Generic;

namespace Dynamics365.Api.Client.Tests.Entities
{
    [CrmEntity("account")]
    public class AccountEntity : BaseCrmEntity
    {
        public AccountEntity() : base() { }

        public AccountEntity(Guid id, Dictionary<string, object> attributes = null) : base(id, attributes) { }

        [CrmStringField("address1_line1")]
        public string Street { get; set; }

        [CrmStringField("address1_city")]
        public string City { get; set; }

        [CrmStringField("address1_postalcode")]
        public string Zip { get; set; }

        [CrmStringField("name")]
        public string Name { get; set; }

        [CrmOptionField("test_sync_state", "test_sync_states")]
        public string SyncState { get; set; }

        [CrmReferenceField("originatingleadid")]
        public virtual LeadEntity OriginatingLead { get; set; }
    }
}
