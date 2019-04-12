using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap
{
    public interface ISoapClient
    {
        Uri BaseUrl { get; set; }
        SoapContentType SoapContentType { get; set; }
        IDictionary<string, string> HttpHeaders { get; }
        Task<TRet> SendAsync<TRet, TBody>( string method, TBody body, params object[] headers ) where TRet : class;
        Task<TRet> SendAsync<TRet, TBody>( string method, Envelope<TBody> message ) where TRet : class;
    }
}
