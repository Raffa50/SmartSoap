using Aldrigos.SmartSoap.Attributes;
using Aldrigos.SmartSoap.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        public T DeserializeObject<T>(string s) where T : class {
            T ret;
            try {
                ret = Activator.CreateInstance<T>();
            } catch( MissingMemberException ex ) {
                throw new SerializationException(typeof(T).Name + " cannot be serialized because it does not have a parameterless constructor.", ex);
            }

            using (var stream = new StringReader(s))
            using( var xmlReader = XmlReader.Create( stream ) ) {
                xmlReader.MoveToContent();
                DeserializeObject( ret, typeof(T).GetCleanName(), xmlReader );
            }

            return ret;
        }

        private void DeserializeObject( object o, string elementName, XmlReader xmlReader ) {
            if( xmlReader.NodeType != XmlNodeType.Element )
                throw new SerializationException("Deserialization error. Expected element "+ elementName);

            XElement xmlElement = XNode.ReadFrom( xmlReader ) as XElement;
            if(xmlElement == null)
                throw new SerializationException("Can't read node for "+ elementName);
            if( xmlElement.Name.LocalName != elementName )
                throw new SerializationException($"Expected element '{elementName}' but got "+xmlElement.Name.LocalName);

            if( xmlElement.HasAttributes ) {
                var attributeProps = o.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance )
                    .Where( p => p.CanWrite && Attribute.IsDefined( p, typeof(XmlAttributeAttribute) ) );

                //foreach (var attrProp in attributeProps)
                //{
                //    var attrName = attrProp.GetCustomAttribute<XmlAttributeAttribute>().AttributeName;
                //    if (string.IsNullOrWhiteSpace(attrName))
                //        attrName = attrProp.Name;

                //    var attrValue = xmlReader.GetAttribute(attrName);
                //    DeserializeBasicType(o, attrProp, attrValue);
                //}

                if (attributeProps.Any())
                    foreach (var xmlAttr in xmlElement.Attributes())
                    {
                        if (xmlAttr.Value == "null")
                            continue;

                        var matchingProp = attributeProps.FirstOrDefault( p => p.Name == xmlAttr.Name );
                        if( matchingProp == null ) {
                            matchingProp = attributeProps.FirstOrDefault(
                                p => p.GetCustomAttribute<XmlAttributeAttribute>().AttributeName == xmlAttr.Name );

                            if( matchingProp == null )
                                continue;
                        }

                        DeserializeBasicType(o, matchingProp, xmlAttr.Value);
                    }
            }

            if( xmlElement.HasElements ) {
                var elementProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && !p.GetCustomAttributes<NonSerializedAttribute>().Any() && !p.GetCustomAttributes<XmlAttributeAttribute>().Any());

                foreach( var subEl in xmlElement.Elements() ) {
                    var subElName = subEl.Name.LocalName;

                    var matchingProp = elementProps.FirstOrDefault( p => p.Name == subElName );
                    if( matchingProp == null ) {
                        matchingProp = elementProps.Where( p => Attribute.IsDefined( p, typeof(XmlElementAttribute) ) )
                            .FirstOrDefault(
                                p => p.GetCustomAttribute<XmlElementAttribute>().ElementName == subElName );

                        if( matchingProp == null )
                            continue;
                    }

                    if(matchingProp.PropertyType.GetInterfaces().Contains( typeof(IConvertible) ))
                        DeserializeBasicType( o, matchingProp, subEl.Value );
                    else if (matchingProp.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        //TODO deserialize collection
                        //var col = DeserializeList( matchingProp, subEl );
                    }
                    else if (matchingProp.PropertyType.GetInterfaces().Contains(typeof(IDictionary)))
                        throw new NotSupportedException("Dictionaries are not supported");
                    else if (matchingProp.PropertyType.IsInterface)
                        throw new SerializationException("Can't deserialize interfaces");
                    else {
                        var sc = Activator.CreateInstance( matchingProp.PropertyType );
                        matchingProp.SetValue(o, sc);

                        var reader = subEl.CreateReader();
                        reader.Read();
                        DeserializeObject( sc, subElName, reader);
                    }
                }
            }
        }

        private void DeserializeBasicType( object o, PropertyInfo prop, string value ) {
            if( prop.PropertyType.IsEnum ) {
                Enum enumValue = value.ToEnum( prop.PropertyType );
                prop.SetValue( o, enumValue );
            } else if( prop.PropertyType.GetInterfaces().Contains( typeof(IConvertible) ) ) {
                if( prop.PropertyType == typeof(string) )
                    prop.SetValue( o, value );
                else if( prop.PropertyType == typeof(bool) )
                    prop.SetValue( o, bool.Parse( value ) );
                else if(prop.PropertyType == typeof(char))
                    prop.SetValue( o, char.Parse( value ) );
                else if(prop.PropertyType == typeof(byte))
                    prop.SetValue( o, byte.Parse( value ) );
                else if(prop.PropertyType == typeof(short))
                    prop.SetValue( o, short.Parse( value ) );
                else if(prop.PropertyType == typeof(int))
                    prop.SetValue( o, int.Parse( value ) );
                else if(prop.PropertyType == typeof(long))
                    prop.SetValue( o, long.Parse( value ) );
                else if(prop.PropertyType == typeof(float))
                    prop.SetValue( o, float.Parse( value ) );
                else if(prop.PropertyType == typeof(double))
                    prop.SetValue( o, double.Parse( value ) );
                else
                    throw new SerializationException("Invalid basic type "+ prop.PropertyType.Name);
            } else
                throw new SerializationException( $"Can't deserialize {prop.Name} with value "+value );
        }

        public string SerializeObject( object o ) {
            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    xmlWriter.WriteStartDocument();
                    SerializeElement(o, o.GetType().GetCleanName(), xmlWriter, new Dictionary<string, string>());
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
                    throw new SerializationException($"NameSpace '{ns}' must be defined before");
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
            else if(o is IDictionary)
                throw new NotSupportedException("Dictionaries are not supported");
            else if (o is IEnumerable)
            {
                var en = (IEnumerable)o;
                foreach (var el in en)
                    SerializeElement(el, el.GetType().Name, xmlWriter, nameSpaces);
            }
            else
            {
                var attributeProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && Attribute.IsDefined( p, typeof(XmlAttributeAttribute) ));
                foreach(var attr in attributeProps)
                {
                    if( !attr.PropertyType.GetInterfaces().Contains(typeof(IConvertible)) )
                        throw new NotSupportedException("Properties marked with XmlAttributes must extend IConvertible");

                    var attrName = attr.GetCustomAttribute<XmlAttributeAttribute>().AttributeName;
                    if (string.IsNullOrWhiteSpace(attrName))
                        attrName = attr.Name;

                    if( attr.PropertyType.IsEnum ) {
                        Enum enumVal = (Enum)attr.GetValue( o, null );
                        xmlWriter.WriteAttributeString( attrName, enumVal.ToEnumString() );
                    } else
                        xmlWriter.WriteAttributeString(attrName, attr.GetValue(o, null).ToString());
                }

                var elementProps = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && !Attribute.IsDefined( p, typeof(NonSerializedAttribute) ) && !Attribute.IsDefined( p, typeof(XmlAttributeAttribute) ));
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
