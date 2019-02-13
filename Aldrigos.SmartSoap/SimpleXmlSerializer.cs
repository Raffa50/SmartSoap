using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aldrigos.SimpleSoap
{
    public class SimpleXmlSerializer
    { 
        public string SerializeObject( object o ) {
            var stream = new StringBuilder();
            using( var xmlWriter = XmlWriter.Create( stream ) ) {
                xmlWriter.WriteStartDocument();
            }
            return stream.ToString();
        }

        private void Serialize( object o, XmlWriter xmlWriter ) {
            var elementProps = o.GetType().GetProperties( BindingFlags.Public & BindingFlags.GetProperty )
                .Where( p => !p.GetCustomAttributes<NonSerializedAttribute>().Any() );
            foreach( var element in elementProps) {
                
            }
        }
    }
}
