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

            if (query.Condition != null)
            {
                if (query.Fields.Any())
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append("$filter=");
                queryStringBuilder.Append(ToFilterString(query.Condition));
            }

            if (query.Limit.HasValue)
            {
                if (query.Fields.Any() || query.Condition != null)
                {
                    queryStringBuilder.Append("&");
                }

                queryStringBuilder.Append($"$top={query.Limit}");
            }

            return queryStringBuilder.ToString();
        }

        private string ToFilterString(ComparisonFilter filter)
        {
            switch (filter.Operation)
            {
                case FilterOperations.Equal:
                    return $"{filter.Field} eq {ToValueString(filter.Value)}";

                case FilterOperations.NotEqual:
                    return $"{filter.Field} ne {ToValueString(filter.Value)}";

                case FilterOperations.Greater:
                    return $"{filter.Field} gt {ToValueString(filter.Value)}";

                case FilterOperations.GreaterThanOrEqual:
                    return $"{filter.Field} ge {ToValueString(filter.Value)}";

                case FilterOperations.Less:
                    return $"{filter.Field} lt {ToValueString(filter.Value)}";

                case FilterOperations.LessThanOrEqual:
                    return $"{filter.Field} le {ToValueString(filter.Value)}";

                case FilterOperations.Contains:
                    return $"contains({filter.Field}, {ToValueString(filter.Value)})";

                case FilterOperations.EndsWith:
                    return $"endswith({filter.Field}, {ToValueString(filter.Value)})";

                case FilterOperations.StartsWith:
                    return $"startswith({filter.Field}, {ToValueString(filter.Value)})";
            }

            throw new ArgumentException($"{filter.Operation} operation is not supported!");
        }

        private static string ToValueString(object value)
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
