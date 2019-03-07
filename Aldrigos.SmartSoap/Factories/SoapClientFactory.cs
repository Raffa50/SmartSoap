using System.Net.Http;

namespace Aldrigos.SmartSoap.Factories
{
    public sealed class SoapClientFactory : ISoapClientFactory
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IXmlSerializer xmlSerializer;

        public SoapClientFactory(IHttpClientFactory httpClientFactory, IXmlSerializer xmlSerializer) {
            this.httpClientFactory = httpClientFactory;
            this.xmlSerializer = xmlSerializer;
        }

        public ISoapClient Make()
        {
            return new SoapClient(httpClientFactory, xmlSerializer);
        }
    }
}
