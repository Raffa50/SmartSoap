using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap
{
    public interface IXmlSerializer
    {
        string SerializeObject(object o);
        T DeserializeObject<T>(string s);
    }
}
