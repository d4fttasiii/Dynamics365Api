using Dynamics365.Api.Client.Services;
using Dynamics365.Api.Client.Tests.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Dynamics365.Api.Client.Tests
{
    [TestFixture]
    public class QueryTests
    {
        private QueryBuilder qb = new QueryBuilder();

        [Test]
        public void Should_Create_Select_One_Column_Query()
        {
            var q = Query
                .From<LeadEntity>()
                .Select(l => l.City);

            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$select=address1_city");
        }

        [Test]
        public void Should_Create_Multi_Column_Query()
        {
            var q = Query
                .From<LeadEntity>()
                .Select(l => new
                {
                    l.City,
                    l.Street
                });
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$select=address1_city,address1_line1");
        }

        [Test]
        public void Should_Create_Eq_Berlin_Lead_Filter_Query()
        {
            var q = Query
                .From<LeadEntity>()
                .Where(l => l.City)
                    .EqualsTo("Berlin");
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=address1_city eq 'Berlin'");
        }

        [Test]
        public void Should_Create_Gt_Than_100000_Lead_Filter_Query()
        {
            var q = Query
                .From<LeadEntity>()
                .Where(l => l.BudgetAmount)
                    .IsGreaterThan(100000.0m);
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=budgetamount gt 100000.0");
        }

        [Test]
        public void Should_Create_2_Filters_With_And()
        {
            var q = Query
                .From<LeadEntity>()
                .Where(l => l.BudgetAmount)
                    .IsGreaterThan(100000.0m)
                    .And(l => l.City)
                    .EqualsTo("Berlin")
                ;
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=(budgetamount gt 100000.0 and address1_city eq 'Berlin')");
        }

        [Test]
        public void Should_Create_Eq_To_Reference_Filter_Query_With_Select_And_Top_100()
        {
            var id = Guid.NewGuid();
            var account = new AccountEntity
            {
                Id = id
            };
            var q = Query
                .From<LeadEntity>()
                .Select(l => new
                {
                    l.BudgetAmount,
                    l.City
                })
                .Where(l => l.ParentAccount).EqualsTo(account)
                .Take(100)
                ;
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo($"$select=budgetamount,address1_city&$filter=_parentaccountid_value eq '{id}'&$top=100");
        }
    }
}
