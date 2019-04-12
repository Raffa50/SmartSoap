using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.AspNet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SoapControllerAttribute : Attribute
    {
        public IEnumerable<string> Routes { get; set; }

        public SoapControllerAttribute(params string[] routes)
        {
            Routes = routes;
        }
    }
}
