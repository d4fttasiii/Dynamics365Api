using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Types;
using System;
using System.Linq;
using System.Text;

namespace Dynamics365.Api.Client.Services
{
    public class QueryBuilder
    {
        public string ToQueryString<TEntity>(IExecutableQuery<TEntity> query)
            where TEntity : BaseCrmEntity, new()
        {
            var queryStringBuilder = new StringBuilder(query.PluralName);

            if (query.Fields.Any())
            {
                queryStringBuilder.Append("$select=");
                queryStringBuilder.Append(string.Join(",", query.Fields));
            }

            if (query.Filter != null)
            {
                if (query.Fields.Any())
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append("$filter=");
                queryStringBuilder.Append(ParseFilter(query.Filter));
            }

            if (query.Limit.HasValue)
            {
                if (query.Fields.Any() || query.Filter != null)
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append($"$top={query.Limit}");
            }

            return queryStringBuilder.ToString();
        }

        private string ParseFilter(IFilter filter)
        {
            switch (filter)
            {
                case ComparisonFilter comparisonFilter:
                    return ParseComparisonFilter(comparisonFilter);

                case LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.Not:
                    return $"not {ParseFilter(logicalFilter.Left)}";

                case LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.And:
                    return $"({ParseFilter(logicalFilter.Left)} and {ParseFilter(logicalFilter.Right)})";

                case LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.Or:
                    return $"({ParseFilter(logicalFilter.Left)} or {ParseFilter(logicalFilter.Right)})";
            }

            throw new ArgumentException($"{filter.GetType().Name} filter type is not supported!");
        }

        private string ParseComparisonFilter(ComparisonFilter filter)
        {
            switch (filter.Operation)
            {
                case FilterOperations.Equal:
                    return $"{filter.Field} eq {ParseValue(filter.Value)}";

                case FilterOperations.NotEqual:
                    return $"{filter.Field} ne {ParseValue(filter.Value)}";

                case FilterOperations.Greater:
                    return $"{filter.Field} gt {ParseValue(filter.Value)}";

                case FilterOperations.GreaterThanOrEqual:
                    return $"{filter.Field} ge {ParseValue(filter.Value)}";

                case FilterOperations.Less:
                    return $"{filter.Field} lt {ParseValue(filter.Value)}";

                case FilterOperations.LessThanOrEqual:
                    return $"{filter.Field} le {ParseValue(filter.Value)}";

                case FilterOperations.Contains:
                    return $"contains({filter.Field}, {ParseValue(filter.Value)})";

                case FilterOperations.EndsWith:
                    return $"endswith({filter.Field}, {ParseValue(filter.Value)})";

                case FilterOperations.StartsWith:
                    return $"startswith({filter.Field}, {ParseValue(filter.Value)})";
            }

            throw new ArgumentException($"{filter.Operation} operation is not supported!");
        }

        private static string ParseValue(object value)
        {
            switch (value)
            {
                case DateTime dt:
                    return $"'{dt.ToString("yyyy-MM-dd HH:mm:ss")}'";

                case string str:
                    return $"'{str}'";

                case Reference er:
                    return $"'{er.Id}'";

                //case ICollection collection:
                //    return $"'({string.Join("," collection)})'";

                default:
                    return value.ToString();
            }
        }
    }
}
