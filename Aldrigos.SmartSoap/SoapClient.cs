﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aldrigos.SmartSoap.Exceptions;
using Aldrigos.SmartSoap.Extensions;

namespace Aldrigos.SmartSoap
{
    public sealed class SoapClient : ISoapClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IXmlSerializer xmlSerializer;

        public Uri BaseUrl { get; set; }
        public IDictionary<string, string> HttpHeaders { get; private set; } = new Dictionary<string, string>();
        public SoapContentType SoapContentType { get; set; } = SoapContentType.TextXml;

        public SoapClient(IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer = null)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.xmlSerializer = xmlSerializer ?? new SimpleXmlSerializer();
        }

        public Task<TRet> SendAsync<TRet, TBody>(string method, TBody body, params object[] headers) where TRet : class where TBody : class
        {
            return SendAsync<TRet, TBody>( method, new Envelope<TBody>( body, headers ) );
        }

        public async Task<TRet> SendAsync<TRet, TBody>(string method, Envelope<TBody> message) where TRet : class where TBody : class
        {
            var url = BaseUrl == null ? new Uri(method) : new Uri(BaseUrl, method);
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
                Content = new StringContent(content, Encoding.UTF8, SoapContentType.ToEnumString())
            })
            {
                foreach (var header in HttpHeaders)
                    request.Headers.Add(header.Key, header.Value);

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new SoapCallException("Soap call failed", response.StatusCode, await response.Content.ReadAsStringAsync());

                    try
                    {
                        var resp= xmlSerializer.DeserializeObject<Envelope<TRet>>(await response.Content.ReadAsStringAsync());
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
