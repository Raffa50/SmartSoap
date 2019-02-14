using Aldrigos.SmartSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap.TestApp
{
    public class C
    {
        public string s { get; set; } = "cebolla";
        public string Null { get; set; }
    };
    public class Test
    {
        [XmlAttribute]
        public string Attribute { get; set; } = "attrValue";
        public string A { get; set; } = "asd";
        public int B { get; set; } = 123;
        [XmlElement("Class")]
        public C cl { get; set; }

        public Test()
        {
            cl = new C
            {
                s = "s"
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ser = new SimpleXmlSerializer();
            string s = ser.SerializeObject(new Envelope(new Test(), new[] { new C() }));
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
