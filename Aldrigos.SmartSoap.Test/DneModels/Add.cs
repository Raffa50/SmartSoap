using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap.Test.DneModels
{
    [XmlType(Namespace = "http://tempuri.org/")]
    public class Add
    {
        public int intA { get; set; }
        public int intB { get; set; }
    }
}
