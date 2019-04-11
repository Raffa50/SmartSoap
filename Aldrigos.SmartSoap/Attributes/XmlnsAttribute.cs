using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Aldrigos.SmartSoap.Attributes
{
    public class XmlnsAttribute : Attribute
    {
        public XNamespace Namespace { get; set; }
        public string Prefix { get; set; }
        /// <summary>
        /// true if there is Prefix
        /// </summary>
        public bool UseForChilds => !string.IsNullOrWhiteSpace(Prefix);

        public XmlnsAttribute(string @namespace, string prefix = "")
        {
            Namespace = @namespace;
            Prefix = prefix;
        }
    }
}
