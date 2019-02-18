using Aldrigos.SmartSoap;
using Aldrigos.SmartSoap.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aldrigos.SmartSoap.TestApp
{
    [XmlNameSpace("my", "http://aldrigo.sf.net/")]
    public class C
    {
        public string s { get; set; }
        public string Null { get; set; }
        public CipollaType Cipolla { get; set; }
    };
    public class Test
    {
        [XmlAttribute]
        public string Attribute { get; set; }
        public string A { get; set; }
        public int B { get; set; }
        [XmlElement("Class")]
        public C cl { get; set; }
        public int[] Numbers { get; set; }
        public List<C> ccs { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var t = new Test()
            {
                Attribute = "Attribute1",
                A = "asd",
                B = 123,
                cl = new C
                {
                    s = "s",
                    Cipolla = CipollaType.Rosse
                },
                Numbers = new int[] { 1, 2, 3 },
                ccs = new List<C> { new C { s = "Inner" } }
        };

            var ser = new SimpleXmlSerializer();
            string s = ser.SerializeObject(new Envelope<Test>(t, new[] { new C() }));
            Console.WriteLine(s);

            var res= ser.DeserializeObject<Envelope<Test>>( s );
            Console.ReadKey();
        }
    }
}
