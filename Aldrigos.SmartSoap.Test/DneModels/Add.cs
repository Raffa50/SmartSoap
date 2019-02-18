using Aldrigos.SmartSoap.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.Test.DneModels
{
    [XmlNameSpace("t", "http://tempuri.org/")]
    public class Add
    {
        public int intA { get; set; }
        public int intB { get; set; }
    }
}
