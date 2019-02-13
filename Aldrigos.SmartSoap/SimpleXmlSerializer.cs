using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap
{
    public class SimpleXmlSerializer : IXmlSerializer
    {
        public T DeserializeObject<T>(string s)
        {
            throw new NotImplementedException();
        }

        public string SerializeObject( object o ) {
            var stream = new StringBuilder();
            using( var xmlWriter = XmlWriter.Create( stream ) ) {
                xmlWriter.WriteStartDocument();
                SerializeElement(o, o.GetType().Name, xmlWriter);
            }
            return stream.ToString();
        }

        private void SerializeElement( object o, string elementName, XmlWriter xmlWriter ) {
            xmlWriter.WriteStartElement(elementName);

            if (o is IConvertible)
                xmlWriter.WriteString(o.ToString());
            else if (o is IEnumerable)
                SerializeCollection((IEnumerable)o, xmlWriter);
            else
            {
                var attributeProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttributes<XmlAttributeAttribute>().Any());

                var elementProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !p.GetCustomAttributes<NonSerializedAttribute>().Any() && !p.GetCustomAttributes<XmlAttributeAttribute>().Any());
                foreach (var element in elementProps)
                {
                    var xmlElementAttr = element.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
                    var subElementName = xmlElementAttr?.ElementName ?? element.Name;

                    SerializeElement(element.GetValue(0, null), subElementName, xmlWriter);
                }
            }

            xmlWriter.WriteEndElement();
        }

        private void SerializeCollection(IEnumerable o, XmlWriter xmlWriter)
        {
            foreach (var el in o) {
                SerializeElement(el, el.GetType().Name, xmlWriter);
            }
        }
    }
}
