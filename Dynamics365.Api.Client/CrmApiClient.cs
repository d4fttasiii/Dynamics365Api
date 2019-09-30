using Dynamics365.Api.Client.Attributes;
using Dynamics365.Api.Client.Exceptions;
using Dynamics365.Api.Client.Extensions;
using Dynamics365.Api.Client.Interfaces;
using Dynamics365.Api.Client.Models;
using Dynamics365.Api.Client.Services;
using Dynamics365.Api.Client.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.Api.Client
{
    public class CrmApiClient : ICrmClient
    {
        private static readonly HttpClient _client = new HttpClient();

        private readonly QueryBuilder _queryBuilder = new QueryBuilder();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        private readonly string _dynamicsUrl;
        private readonly string _accessToken;

        private string BaseUrl => $"{_dynamicsUrl}/api/data/v9.1";

        public CrmApiClient(string url, string bearerToken)
        {
            _dynamicsUrl = url;
            _accessToken = bearerToken;

            _ = InitOptionSetsAsync();
        }

        public CrmApiClient(string url, string tenantId, OAuth2Parameters authParams) :
            this(url, GetBearerTokenAsync(tenantId, authParams))
        { }

        public async Task<Guid> CreateAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new()
        {
            var entityAttribute = typeof(TEntity).GetEntityAttribute();
            var json = JsonConvert.SerializeObject(entity.Attributes, _serializerSettings);
            var message = ConstructMessage(HttpMethod.Post, $"{BaseUrl}/{entityAttribute.PluralName}", json);
            var response = await _client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                throw new CrmApiClientException("Error while creating entity")
                {
                    HttpMethod = message.Method,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }

            var odataEntityId = response.Headers.GetValues("OData-EntityId").First();
            var parsedId = odataEntityId.Replace($"{BaseUrl}/{entityAttribute.PluralName}/", "").Replace("(", "").Replace(")", "").Trim();

            return Guid.Parse(parsedId);
        }

        public async Task<BaseCrmEntity> GetByIdAsync(Type entityType, Guid id)
        {
            var entityAttribute = entityType.GetEntityAttribute();
            var message = ConstructMessage(HttpMethod.Get, $"{BaseUrl}/{entityAttribute.PluralName}/({id})");
            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new CrmApiClientException($"Error while getting entity ({id})")
                {
                    HttpMethod = message.Method,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }

            var attributeDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent, _serializerSettings);
            var typedEntity = Activator.CreateInstance(entityType, id, attributeDict) as BaseCrmEntity;

            return typedEntity;
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(Guid id) where TEntity : BaseCrmEntity, new() =>
            await GetByIdAsync(typeof(TEntity), id) as TEntity;


        public async Task<IEnumerable<TEntity>> GetByQueryAsync<TEntity>(IExecutableQuery<TEntity> query) where TEntity : BaseCrmEntity, new()
        {
            var queryString = _queryBuilder.ToQueryString(query);
            var message = ConstructMessage(HttpMethod.Get, $"{BaseUrl}/{queryString}");

            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new CrmApiClientException($"Error quering for entities")
                {
                    HttpMethod = message.Method,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }

            return ParseApiResponse<TEntity>(responseContent);
        }

        public async Task<IEnumerable<TEntity>> GetFromView<TEntity>(string viewName) where TEntity : BaseCrmEntity, new()
        {
            var savedQueryId = SavedQueryCache.Instance.GetSavedQueryId(viewName);
            if (savedQueryId == null)
            {
                savedQueryId = await GetSavedQueryId(viewName);
                SavedQueryCache.Instance.CacheSavedQueryId(viewName, savedQueryId.Value);
            }

            var message = ConstructMessage(HttpMethod.Get, $"");
            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new CrmApiClientException($"Error getting entities from {viewName}")
                {
                    HttpMethod = message.Method,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }

            return ParseApiResponse<TEntity>(responseContent);
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new()
        {
            entity.CommitChanges();
            var attribute = typeof(TEntity).GetEntityAttribute();
            var json = JsonConvert.SerializeObject(entity.Attributes, _serializerSettings);
            var message = ConstructMessage(new HttpMethod("PATCH"), $"{BaseUrl}/{attribute.PluralName}({entity.Id})", json);

            var response = await _client.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new CrmApiClientException($"Error while updating entity ({entity.Id})")
                    {
                        HttpMethod = message.Method,
                        HttpRequestUrl = message.RequestUri.OriginalString,
                        HttpResponseStatusCode = response.StatusCode,
                        HttpResponseBodyJson = responseContent
                    };
                }
            }
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : BaseCrmEntity, new()
        {
            if (!entity.Id.HasValue())
            {
                throw new ArgumentException($"{nameof(BaseCrmEntity.Id)} cannot be empty or null");
            }

            await DeleteByIdAsync(entity.PluralName, entity.Id);
        }

        public async Task DeleteByIdAsync<TEntity>(Guid id) where TEntity : BaseCrmEntity, new()
        {
            var pluralname = typeof(TEntity).GetEntityAttribute().PluralName;

            await DeleteByIdAsync(pluralname, id);
        }

        public async Task DeleteByIdAsync(string pluralName, Guid id)
        {
            var message = ConstructMessage(HttpMethod.Delete, $"{BaseUrl}/{pluralName}/({id})");
            var response = await _client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new CrmApiClientException("Error while getting entity by id")
                {
                    HttpMethod = HttpMethod.Delete,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }
        }

        private async Task InitOptionSetsAsync()
        {
            var message = ConstructMessage(HttpMethod.Get, $"{BaseUrl}/GlobalOptionSetDefinitions");
            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            OptionSetMapper.AddOptionSetMapping(responseContent);           
        }

        private async Task<Guid> GetSavedQueryId(string viewName)
        {
            var message = ConstructMessage(HttpMethod.Get, $"{BaseUrl}/savedqueries?$select=savedqueryid&$filter=name eq '{viewName}'");
            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new CrmApiClientException($"Error while savedQueryId for view: {viewName}")
                {
                    HttpMethod = HttpMethod.Get,
                    HttpRequestUrl = message.RequestUri.OriginalString,
                    HttpResponseStatusCode = response.StatusCode,
                    HttpResponseBodyJson = responseContent
                };
            }

            return JObject.Parse(responseContent)["value"].First["savedqueryid"].ToObject<Guid>();
        }

        private HttpRequestMessage ConstructMessage(HttpMethod method, string url, string jsonContent = "")
        {
            var message = new HttpRequestMessage(method, url);
            message.Headers.Add("Cookie", $"CrmOwinAuth={_accessToken}");

            if (!string.IsNullOrEmpty(jsonContent))
            {
                message.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            return message;
        }

        private static string GetBearerTokenAsync(string tenantId, OAuth2Parameters authParams)
        {
            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token?grant_type={authParams.GrantType}&client_id={authParams.ClientId}&client_secret={authParams.ClientSecret}&scope={authParams.Scope}&userName={authParams.Username}&password={authParams.Password}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var response = _client.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            var responseJson = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var jsonObj = JObject.Parse(responseJson);

            return jsonObj["access_token"].ToString();
        }

        private static IEnumerable<TEntity> ParseApiResponse<TEntity>(string responseContent) where TEntity : BaseCrmEntity, new()
        {
            var entityType = typeof(TEntity);
            var entityAttribute = entityType.GetEntityAttribute();
            var responseJObject = JObject.Parse(responseContent);
            var results = new List<TEntity>();

            foreach (var item in responseJObject["value"])
            {
                var attributeDict = item.ToObject<Dictionary<string, object>>();
                var id = Guid.Parse(attributeDict[$"{entityAttribute.LogicalName}id"].ToString());
                var typedEntity = Activator.CreateInstance(entityType, id, attributeDict) as TEntity;
                results.Add(typedEntity);
            }

            return results;
        }
    }
}
