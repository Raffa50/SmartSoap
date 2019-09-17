using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap
{
    public interface IXmlSerializer
    {
        string SerializeObject(object o);
        object DeserializeObject(Type t, string s);
    }
}
