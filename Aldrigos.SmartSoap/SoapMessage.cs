using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aldrigos.SmartSoap
{
    public class SoapMessage
    {
        public object Body { get; set; }
        public IEnumerable<object> Headers { get; set; }

        public SoapMessage()
        {
            Headers = Enumerable.Empty<object>();
        }

        public SoapMessage( object body )
        : base()
        {
            Body = body;
        }

        public SoapMessage(object body, IEnumerable<object> headers)
        {
            Body = body;
            Headers = headers;
        }
    }
}
