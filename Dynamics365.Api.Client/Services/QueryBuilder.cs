using Dynamics365.Api.Client.Enums;
using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Types;
using System;
using System.Globalization;
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

            if (query.Top.HasValue)
            {
                if (query.Fields.Any() || query.Filter != null)
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append($"$top={query.Top}");
            }

            return queryStringBuilder.ToString();
        }

        private string ParseFilter(IFilter filter)
        {
            return filter switch
            {
                ComparisonFilter comparisonFilter => ParseComparisonFilter(comparisonFilter),
                LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.Not => $"not {ParseFilter(logicalFilter.Left)}",
                LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.And => $"({ParseFilter(logicalFilter.Left)} and {ParseFilter(logicalFilter.Right)})",
                LogicalFilter logicalFilter when logicalFilter.Operator == LogicalOperations.Or => $"({ParseFilter(logicalFilter.Left)} or {ParseFilter(logicalFilter.Right)})",
                _ => throw new ArgumentException($"{filter.GetType().Name} filter type is not supported!"),
            };
        }

        private string ParseComparisonFilter(ComparisonFilter filter)
        {
            return filter.Operation switch
            {
                FilterOperations.Equal => $"{filter.Field} eq {ParseValue(filter.Value)}",
                FilterOperations.NotEqual => $"{filter.Field} ne {ParseValue(filter.Value)}",
                FilterOperations.Greater => $"{filter.Field} gt {ParseValue(filter.Value)}",
                FilterOperations.GreaterThanOrEqual => $"{filter.Field} ge {ParseValue(filter.Value)}",
                FilterOperations.Less => $"{filter.Field} lt {ParseValue(filter.Value)}",
                FilterOperations.LessThanOrEqual => $"{filter.Field} le {ParseValue(filter.Value)}",
                FilterOperations.Contains => $"contains({filter.Field}, {ParseValue(filter.Value)})",
                FilterOperations.EndsWith => $"endswith({filter.Field}, {ParseValue(filter.Value)})",
                FilterOperations.StartsWith => $"startswith({filter.Field}, {ParseValue(filter.Value)})",
                _ => throw new ArgumentException($"{filter.Operation} operation is not supported!"),
            };
        }

        private static string ParseValue(object value)
        {
            return value switch
            {
                DateTime dt => $"'{dt.ToString("yyyy-MM-dd HH:mm:ss")}'",
                string str => $"'{str}'",
                BaseCrmEntity entity => $"'{entity.Id}'",
                float fAmount => fAmount.ToString(CultureInfo.InvariantCulture),
                double doubleAmount => doubleAmount.ToString(CultureInfo.InvariantCulture),
                decimal decAmount => decAmount.ToString(CultureInfo.InvariantCulture),
                _ => value.ToString(),
            };
        }
    }
}
