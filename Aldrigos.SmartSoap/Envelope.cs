using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap
{
    public class Envelope
    {
        public object Body { get; set; }
        [XmlElement("Header")]
        public IEnumerable<object> Headers { get; set; }

        public Envelope()
        {
            Headers = Enumerable.Empty<object>();
        }

        public Envelope( object body )
        : base()
        {
            Body = body;
        }

        public Envelope(object body, IEnumerable<object> headers)
        {
            Body = body;
            Headers = headers;
        }
    }
}
