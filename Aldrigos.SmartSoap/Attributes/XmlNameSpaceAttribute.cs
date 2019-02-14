using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class XmlNameSpaceAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public XmlNameSpaceAttribute( string name ) {
            Name = name;
        }

        public XmlNameSpaceAttribute( string name, string value )
            : this( name ) {
            Value = value;
        }
    }
}
