using Aldrigos.SmartSoap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap
{
    [XmlNameSpace("soap", "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement("Body", Namespace = "soap")]
        public object Body { get; set; }
        [XmlElement("Header", Namespace = "soap")]
        public IEnumerable<object> Headers { get; set; }

        public Envelope()
        {
            Headers = Enumerable.Empty<object>();
        }

        public Envelope( object body )
        : this()
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
