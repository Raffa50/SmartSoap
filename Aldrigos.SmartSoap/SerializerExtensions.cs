using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap
{
    public static class SerializerExtensions
    {
        public static T DeserializeObject<T>(this IXmlSerializer serializer, string str) where T : class
        {
            return (T)serializer.DeserializeObject(typeof(T), str);
        }
    }
}
