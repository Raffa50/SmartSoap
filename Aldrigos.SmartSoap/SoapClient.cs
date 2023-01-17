using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aldrigos.SmartSoap.Exceptions;
using Aldrigos.SmartSoap.Extensions;
using Microsoft.Extensions.Logging;

namespace Aldrigos.SmartSoap
{
    public sealed class SoapClient : ISoapClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IXmlSerializer xmlSerializer;
        private readonly ILogger log;

        public Uri BaseUrl { get; set; }
        public IDictionary<string, string> HttpHeaders { get; private set; } = new Dictionary<string, string>();
        public SoapContentType SoapContentType { get; set; } = SoapContentType.TextXml;

        public SoapClient(IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null, ILogger log = null)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.xmlSerializer = xmlSerializer ?? new SimpleXmlSerializer();
            this.log = log;
            Utf8EncodingProvider.Register();
        }

        public Task<TRet> SendAsync<TRet, TBody>(string method, TBody body, params object[] headers) where TRet : class where TBody : class
        {
            return SendAsync<TRet, TBody>(method, new Envelope<TBody>(body, headers));
        }

        public Task<TRet> SendAsync<TRet, TBody>(TBody body, params object[] headers) where TRet : class where TBody : class
        {
            return SendAsync<TRet, TBody>( "", new Envelope<TBody>( body, headers ) );
        }

        public async Task<TRet> SendAsync<TRet, TBody>(string method, Envelope<TBody> message) where TRet : class where TBody : class
        {
            var client = httpClientFactory.CreateClient();

            string content;
            try
            {
                content = xmlSerializer.SerializeObject(message);
            } catch(Exception ex)
            {
                throw new InvalidOperationException("Error during serialization", ex);
            }

            var url = new Uri(BaseUrl, method);
            using (var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(content, Encoding.UTF8, SoapContentType.ToEnumString())
            })
            {
                log?.LogTrace("POST: {0}\nBody: {1}", url, content);

                foreach (var header in HttpHeaders)
                    request.Headers.Add(header.Key, header.Value);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    var respCont = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    log?.LogTrace("RESPONSE code: {0}\nContent: {1}", response.StatusCode, respCont);

                    if (!response.IsSuccessStatusCode)
                        throw new SoapCallException("Soap call failed", response.StatusCode,
                            respCont);

                    try
                    {
                        string responseContent = respCont;
                        var resp= xmlSerializer.DeserializeObject<Envelope<TRet>>(responseContent);
                        return resp.Body[0];
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
