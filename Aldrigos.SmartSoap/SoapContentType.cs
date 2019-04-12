using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aldrigos.SmartSoap
{
    public enum SoapContentType
    {
        [EnumMember(Value = "text/xml")]
        TextXml,
        [EnumMember(Value = "application/soap+xml")]
        ApplicationSoap,
        [EnumMember(Value = "application/xml")]
        ApplicationXml
    }
}
