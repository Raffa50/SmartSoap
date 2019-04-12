using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aldrigos.SmartSoap.AspNet
{
    public static class SoapEndpointMiddlewareExtensions
    {
        public static IServiceCollection AddSoap(this IServiceCollection service)
        {
            return service.AddSingleton<IXmlSerializer, SimpleXmlSerializer>()
                ;
        }

        public static IApplicationBuilder UseSoapEndpoint(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SoapEndpointMiddleware>();
        }
    }
}
