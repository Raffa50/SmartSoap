using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap
{
    internal class SoapClient : ISoapClient
    {
        public Uri BaseUri { get; private set; }

        public IReadOnlyDictionary<string, string> AdditionalXlmns { get; private set; }

        public Task<T> SendAsync<T>(object body, params object[] headers) {
            throw new NotImplementedException();
            //return SendAsync<T>( new SoapMessage( body, headers ) );
        }

        public Task<T> SendAsync<T>(SoapMessage message) {
            var client = new HttpClient();

            throw new NotImplementedException();
        }
    }
}
