using Aldrigos.SmartSoap.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap
{
    [XmlNameSpace("soap", "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope<T>
    {
        [XmlElement("Body", Namespace = "soap")]
        public virtual T Body { get; set; }
        [XmlElement("Header", Namespace = "soap")]
        public virtual IEnumerable<object> Headers { get; } = Enumerable.Empty<object>();

        public Envelope() { }

        public Envelope( T body )
        {
            Body = body;
        }

        public Envelope(T body, IEnumerable<object> headers)
            :this(body)
        {
            Headers = headers;
        }
    }
}
