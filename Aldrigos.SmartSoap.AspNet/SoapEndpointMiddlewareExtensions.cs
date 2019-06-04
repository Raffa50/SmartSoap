using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aldrigos.SmartSoap.AspNet
{
    public static class SoapEndpointMiddlewareExtensions
    {
        public static IServiceCollection AddSoap(this IServiceCollection service, IEnumerable<Assembly> assembly)
        {
            return service
                .AddSingleton<IXmlSerializer, SimpleXmlSerializer>()
                .AddSingleton(new SoapMiddlewareConfig(assembly));
        }

        public static IApplicationBuilder UseSoapEndpoint(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SoapEndpointMiddleware>();
        }
    }
}
