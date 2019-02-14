using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap
{
    public interface ISoapClient
    {
        Uri BaseUrl { get; }
        Task<T> SendAsync<T>( string method, object body, params object[] headers );
        Task<T> SendAsync<T>( string method, Envelope message );
    }
}
