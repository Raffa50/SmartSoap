using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.Factories
{
    public interface ISoapClientFactory
    {
        ISoapClient Make(Uri baseUrl);
        ISoapClient Make(Uri baseUrl, IReadOnlyDictionary<string, string> httpHeaders);
    }
}
