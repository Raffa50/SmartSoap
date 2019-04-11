using Aldrigos.SmartSoap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aldrigos.SmartSoap
{
    [Xmlns("http://schemas.xmlsoap.org/soap/envelope/", "soap")]
    public class Envelope<T>
    {
        public virtual T[] Body { get; }
        public virtual IEnumerable<object> Headers { get; } = Enumerable.Empty<object>();

        public Envelope() { }

        public Envelope( T body )
        {
            Body = new T[] { body };
        }

        public Envelope(T body, IEnumerable<object> headers)
            :this(body)
        {
            Headers = headers;
        }
    }
}
