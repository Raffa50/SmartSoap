using Aldrigos.SmartSoap.Attributes;
using Aldrigos.SmartSoap.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap
{
    public sealed class SimpleXmlSerializer : IXmlSerializer
    {
        #region Deserializer
        public object DeserializeObject(Type t, string s)
        {
            using (var stream = new StringReader(s))
            using (var xmlReader = XmlReader.Create(stream))
            {
                xmlReader.MoveToContent();
                XElement xmlElement = XNode.ReadFrom(xmlReader) as XElement;

                string elementName = GetXmlTypeName2(t).LocalName;
                if (xmlElement.Name.LocalName != elementName)
                    throw new SerializationException($"Expected element with name='{elementName}' but got '{xmlElement.Name.LocalName}'");

                return DeserializeObject(t, xmlElement);
            }
        }

        private object DeserializeObject(Type type, XElement xmlElement)
        {
            if (xmlElement.Value == "null")
                return null;
            if (type.GetInterfaces().Contains(typeof(IConvertible)))
                return DeserializeBasicType(type, xmlElement.Value);
            else if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                return DeserializeList(type, xmlElement.Elements());
            else if (type.GetInterfaces().Contains(typeof(IDictionary)))
                throw new NotSupportedException("Dictionaries are not supported");
            else if (type.IsInterface)
                throw new SerializationException("Can't deserialize interfaces");
            else
            {
                var sc = Activator.CreateInstance(type);

                if (xmlElement.HasAttributes)
                {
                    var attributeProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && Attribute.IsDefined(p, typeof(XmlAttributeAttribute)));

                    if (attributeProps.Any())
                        foreach (var xmlAttr in xmlElement.Attributes())
                        {
                            if (xmlAttr.Value == "null")
                                continue;

                            var matchingProp = attributeProps.FirstOrDefault(p => p.Name == xmlAttr.Name);
                            if (matchingProp == null)
                            {
                                matchingProp = attributeProps.FirstOrDefault(
                                    p => p.GetCustomAttribute<XmlAttributeAttribute>().AttributeName == xmlAttr.Name);

                                if (matchingProp == null)
                                    continue;
                            }

                            var value = DeserializeBasicType(matchingProp.PropertyType, xmlAttr.Value);
                            matchingProp.SetValue(sc, value);
                        }
                }

                if (xmlElement.HasElements)
                {
                    var elementProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && p.SetMethod != null &&
                            !Attribute.IsDefined(p, typeof(NotMappedAttribute)) && !Attribute.IsDefined(p, typeof(XmlAttributeAttribute)));

                    foreach (var subEl in xmlElement.Elements())
                    {
                        var subElName = subEl.Name.LocalName;

                        var matchingProp = elementProps.FirstOrDefault(p => p.Name == subElName);
                        if (matchingProp == null)
                        {
                            matchingProp = elementProps.Where(p => Attribute.IsDefined(p, typeof(XmlElementAttribute)))
                                .FirstOrDefault(
                                    p => p.GetCustomAttribute<XmlElementAttribute>().ElementName == subElName);

                            if (matchingProp == null)
                                continue;
                        }

                        var value = DeserializeObject(matchingProp.PropertyType, subEl);
                        matchingProp.SetValue(sc, value);
                    }
                }

                return sc;
            }
        }

        private ICollection DeserializeList(Type type, IEnumerable<XElement> elements)
        {
            var elementsType = type.GetElementType() ?? type.GenericTypeArguments[0];
            var elArray = Array.CreateInstance(elementsType, elements.Count());
            int i = 0;
            foreach (var el in elements)
            {
                elArray.SetValue(DeserializeObject(elementsType, el), i++);
            }

            if (type.IsArray)
                return elArray;

            if (type.IsInterface)
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(elementsType);

                return (ICollection)Activator.CreateInstance(constructedListType, elArray);
            }
            return (ICollection)Activator.CreateInstance(type, elArray);
        }

        private object DeserializeBasicType(Type type, string value)
        {
            if (type.IsEnum)
                return value.ToEnum(type);
            else if (type.GetInterfaces().Contains(typeof(IConvertible)))
            {
                if (type == typeof(string))
                    return value;
                else if (type == typeof(bool))
                    return bool.Parse(value);
                else if (type == typeof(char))
                    return char.Parse(value);
                else if (type == typeof(byte))
                    return byte.Parse(value);
                else if (type == typeof(short))
                    return short.Parse(value);
                else if (type == typeof(int))
                    return int.Parse(value);
                else if (type == typeof(long))
                    return long.Parse(value);
                else if (type == typeof(float))
                    return float.Parse(value);
                else if (type == typeof(double))
                    return double.Parse(value);
                else
                    throw new SerializationException("Invalid basic type " + type.Name);
            }
            else
                throw new SerializationException($"Can't deserialize {type.Name} with value " + value);
        }
        #endregion

        #region Serializer
        public string SerializeObject(object o)
        {
            using (var stream = new MemoryStream())
            {
                var document = new XDocument(SerializeElement(o, GetXmlTypeName2(o.GetType())));
                using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    document.WriteTo(xmlWriter);
                }

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private XElement CreateElement(Type t, XName elementName)
        {
            var nsAttr = t.GetCustomAttributes<XmlnsAttribute>().FirstOrDefault();
            if(nsAttr != null)
            {
                if (!string.IsNullOrEmpty(nsAttr.Prefix))
                    return new XElement(nsAttr.Namespace + elementName.LocalName,
                        new XAttribute(XNamespace.Xmlns + nsAttr.Prefix, nsAttr.Namespace)
                    );
                else
                    return new XElement(nsAttr.Namespace + elementName.LocalName);
            }

            var typeAttr = t.GetCustomAttributes<XmlTypeAttribute>().FirstOrDefault();
            if (string.IsNullOrWhiteSpace(typeAttr?.Namespace))
                return new XElement(elementName);

            return new XElement(XNamespace.Get(typeAttr.Namespace) + elementName.LocalName);
        }

        private XElement SerializeElement(object o, XName elementName)
        {
            XElement element = !string.IsNullOrEmpty(elementName.Namespace?.NamespaceName) ?
                new XElement(elementName) :
                CreateElement(o.GetType(), elementName);

            if (o is Enum)
            {
                var @enum = (Enum)o;
                element.Value = @enum.ToEnumString();
            }
            else if (o is IConvertible)
                element.Value = o.ToString();
            else if (o is IDictionary)
                throw new NotSupportedException("Dictionaries are not supported");
            else if (o is IEnumerable)
            {
                var en = (IEnumerable)o;
                foreach (var el in en)
                    element.Add(SerializeElement(el, GetXmlTypeName2(el.GetType())));
            }
            else
            {
                var attributeProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && Attribute.IsDefined(p, typeof(XmlAttributeAttribute)));
                foreach (var attr in attributeProps)
                {
                    if (!attr.PropertyType.GetInterfaces().Contains(typeof(IConvertible)))
                        throw new NotSupportedException("Properties marked with XmlAttributes must extend IConvertible");

                    var attrName = attr.GetCustomAttribute<XmlAttributeAttribute>().AttributeName;
                    if (string.IsNullOrWhiteSpace(attrName))
                        attrName = attr.Name;

                    if (attr.PropertyType.IsEnum)
                    {
                        Enum enumVal = (Enum)attr.GetValue(o, null);
                        element.SetAttributeValue(attrName, enumVal.ToEnumString());
                    }
                    else
                        element.SetAttributeValue(attrName, attr.GetValue(o, null).ToString());
                }

                var elementProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && !Attribute.IsDefined(p, typeof(NonSerializedAttribute)) && !Attribute.IsDefined(p, typeof(XmlAttributeAttribute)));
                foreach (var childEl in elementProps)
                {
                    var xmlElementAttr = childEl.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
                    XNamespace childNs = !string.IsNullOrEmpty(xmlElementAttr?.Namespace) ? 
                        XNamespace.Get(xmlElementAttr.Namespace) : 
                        element.Name.Namespace;

                    XName subElementName =  childNs + (xmlElementAttr?.ElementName ?? childEl.Name);

                    var value = childEl.GetValue(o, null);
                    if (value != null)
                        element.Add(SerializeElement(value, subElementName));
                }

            }

            return element;
        }
        #endregion

        private XName GetXmlTypeName2(Type t)
        {
            var nsAttr = t.GetCustomAttributes<XmlTypeAttribute>().FirstOrDefault();
            if(!string.IsNullOrEmpty(nsAttr?.TypeName))
            {
                return !string.IsNullOrEmpty(nsAttr.Namespace) ?
                    XNamespace.Get(nsAttr.Namespace) + nsAttr.TypeName
                    : nsAttr.TypeName;
            }
            return t.GetCleanName();
        }
    }
}
