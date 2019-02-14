using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap
{
    public interface ISoapClient
    {
        Uri BaseUrl { get; }
        Task<T> SendAsync<T>( object body, params object[] headers );
        Task<T> SendAsync<T>( Envelope message );
    }
}
