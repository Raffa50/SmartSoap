using Aldrigos.SmartSoap.Exceptions;
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
        public IReadOnlyDictionary<string, string> httpHeaders { get; private set; }

        public SoapClient(Uri baseUrl, IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null)
            : this(baseUrl, null, httpClientFactory, xmlSerializer) { }

        public SoapClient(Uri baseUrl, IReadOnlyDictionary<string, string> httpHeaders, IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null)
        {
            this.BaseUrl = baseUrl;
            this.httpHeaders = httpHeaders ?? new Dictionary<string,string>();
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.xmlSerializer = xmlSerializer ?? new SimpleXmlSerializer();
        }

        public Task<T> SendAsync<T>(string method, object body, params object[] headers) {
            return SendAsync<T>( method, new Envelope( body, headers ) );
        }

        public async Task<T> SendAsync<T>(string method, Envelope message) {
            var client = httpClientFactory.CreateClient();

            string content;
            try
            {
                content = xmlSerializer.SerializeObject(message);
            } catch(Exception ex)
            {
                throw new InvalidOperationException("Error during serialization", ex);
            }

            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(BaseUrl, method))
            {
                Content = new StringContent(content, Encoding.UTF8, "text/xml")
            })
            {
                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new SoapCallException("Soap call failed", response);

                    try
                    {
                        return xmlSerializer.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Error during derialization", ex);
                    }
                }
            }
        }
    }
}
