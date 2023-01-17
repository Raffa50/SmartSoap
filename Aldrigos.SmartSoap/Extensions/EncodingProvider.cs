using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.Extensions
{
    public class Utf8EncodingProvider : EncodingProvider
    {
        public override Encoding GetEncoding(string name)
        {
            return string.Equals(name, "\"UTF-8\"", StringComparison.InvariantCultureIgnoreCase) ? Encoding.UTF8 : null;
        }

        public override Encoding GetEncoding(int codepage)
        {
            return null;
        }

        public static void Register()
        {
            Encoding.RegisterProvider(new Utf8EncodingProvider());
        }
    }
}
