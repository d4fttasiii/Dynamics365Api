using Dynamics365.Api.Client.Services;
using Dynamics365.Api.Client.Tests.Entities;
using FluentAssertions;
using NUnit.Framework;

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
                    .Equal("Berlin");
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=address1_city eq 'Berlin'");
        }

        [Test]
        public void Should_Create_Gt_Than_100000_Lead_Filter_Query()
        {
            var q = Query
                .From<LeadEntity>()
                .Where(l => l.BudgetAmount)
                    .GreaterThan(100000.0m);
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=budgetamount gt 100000.0");
        }

        [Test]
        public void Should_Create_2_Filters_With_And()
        {
            var q = Query
                .From<LeadEntity>()
                .Where(l => l.BudgetAmount).GreaterThan(100000.0m)
                .And(l => l.City).Equal("Berlin")
                ;
            var queryString = qb.ToQueryString(q);

            queryString.Should().BeEquivalentTo("$filter=(budgetamount gt 100000.0 and address1_city eq 'Berlin')");
        }
    }
}
