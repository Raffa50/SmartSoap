using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aldrigos.SmartSoap.AspNet
{
    public class SoapMiddlewareConfig
    {
        internal IEnumerable<Assembly> Assemblies { get; }

        internal SoapMiddlewareConfig(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies;
        }
    }
}
