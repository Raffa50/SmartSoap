using Aldrigos.SmartSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldrigos.SmartSoap.TestApp
{
    public class C
    {
        public string s { get; set; }
    };
    public class Test
    {
        public string A { get; set; } = "asd";
        public int B { get; set; } = 123;
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
            string s = ser.SerializeObject(new Test());
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}
