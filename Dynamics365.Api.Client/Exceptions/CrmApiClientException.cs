using System;
using System.Net;
using System.Net.Http;

namespace Dynamics365.Api.Client.Exceptions
{
    public class CrmApiClientException : Exception
    {
        public HttpMethod HttpMethod { get; set; }
        public string HttpRequestUrl { get; set; }
        public string HttpRequestBodyJson { get; set; }

        public string HttpResponseBodyJson { get; set; }
        public HttpStatusCode HttpResponseStatusCode { get; set; }

        public CrmApiClientException(string message) : base(message) { }
        public CrmApiClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
