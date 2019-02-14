using Aldrigos.SmartSoap.Attributes;
using Aldrigos.SmartSoap.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    xmlWriter.WriteStartDocument();
                    SerializeElement(o, o.GetType().Name, xmlWriter, new Dictionary<string, string>());
                }

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private void SerializeElement( object o, string elementName, XmlWriter xmlWriter, IDictionary<string, string> nameSpaces, string ns = null ) {
            if (ns != null)
            {
                if (!nameSpaces.Keys.Contains(ns))
                    throw new InvalidOperationException($"NameSpace '{ns}' must be defined before");
                xmlWriter.WriteStartElement(ns, elementName, nameSpaces[ns]);
            }
            else
            {
                var nsAttr = o.GetType().GetCustomAttributes<XmlNameSpaceAttribute>().FirstOrDefault();
                if (nsAttr != null)
                {
                    if (!nameSpaces.Keys.Contains(nsAttr.Name))
                        nameSpaces.Add(nsAttr.Name, nsAttr.Value);

                    xmlWriter.WriteStartElement(nsAttr.Name, elementName, nameSpaces[nsAttr.Name]);
                }
                else
                    xmlWriter.WriteStartElement(elementName);
            }

            if(o is Enum)
            {
                var @enum = (Enum)o;
                xmlWriter.WriteString(@enum.ToEnumString());
            }
            else if (o is IConvertible)
                xmlWriter.WriteString(o.ToString());
            else if (o is IEnumerable)
            {
                var en = (IEnumerable)o;
                foreach (var el in en)
                    SerializeElement(el, el.GetType().Name, xmlWriter, nameSpaces);
            }
            else
            {
                var attributeProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttributes<XmlAttributeAttribute>().Any());
                foreach(var attr in attributeProps)
                {
                    if( !attr.PropertyType.GetInterfaces().Contains(typeof(IConvertible)) )
                        throw new InvalidOperationException("Properties marked with XmlAttributes must extend IConvertible");

                    var attrName = attr.GetCustomAttribute<XmlAttributeAttribute>().AttributeName;
                    if (string.IsNullOrWhiteSpace(attrName))
                        attrName = attr.Name;

                    xmlWriter.WriteAttributeString(attrName, attr.GetValue(o, null).ToString());
                }

                var elementProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !p.GetCustomAttributes<NonSerializedAttribute>().Any() && !p.GetCustomAttributes<XmlAttributeAttribute>().Any());
                foreach (var element in elementProps)
                {
                    var xmlElementAttr = element.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
                    var subElementName = xmlElementAttr?.ElementName ?? element.Name;

                    var value = element.GetValue(o, null);
                    if(value != null)
                        SerializeElement(value, subElementName, xmlWriter, nameSpaces, xmlElementAttr?.Namespace);
                }
            }

            xmlWriter.WriteEndElement();
        }
    }
}
