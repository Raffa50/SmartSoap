using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap
{
    internal class SoapClient : ISoapClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IXmlSerializer xmlSerializer;

        public Uri BaseUrl { get; private set; }

        public IReadOnlyDictionary<string, string> AdditionalXlmns { get; private set; }

        public SoapClient(Uri baseUrl, IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null)
            : this(baseUrl, null, httpClientFactory, xmlSerializer) { }

        public SoapClient(Uri baseUrl, IReadOnlyDictionary<string, string> additionalXlmns, IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null)
        {
            this.BaseUrl = baseUrl;
            this.AdditionalXlmns = additionalXlmns ?? new Dictionary<string,string>();
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.xmlSerializer = xmlSerializer ?? new SimpleXmlSerializer();
        }

        public Task<T> SendAsync<T>(object body, params object[] headers) {
            return SendAsync<T>( new Envelope( body, headers ) );
        }

        public async Task<T> SendAsync<T>(Envelope message) {
            var client = httpClientFactory.CreateClient();

            throw new NotImplementedException();
        }
    }
}
