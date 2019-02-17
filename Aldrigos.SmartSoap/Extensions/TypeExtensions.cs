using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetCleanName( this Type t ) {
            if( !t.IsGenericType )
                return t.Name;

            Type g = t.GetGenericTypeDefinition();
            return g.Name.Remove(g.Name.IndexOf('`'));
        }
    }
}
