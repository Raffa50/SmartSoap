using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Aldrigos.SmartSoap.Extensions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aldrigos.SmartSoap.AspNet
{
    public sealed class SoapEndpointMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<string> contentTypes;
        private readonly SoapMiddlewareConfig config;

        private readonly IDictionary<string, object> controllers = new Dictionary<string, object>();

        public SoapEndpointMiddleware(RequestDelegate next, SoapMiddlewareConfig config, IServiceProvider serviceProvider)
        {
            this.next = next;
            this.config = config;
            contentTypes = Enum.GetValues(typeof(SoapContentType)).Cast<SoapContentType>().Select(e => e.ToEnumString());

            foreach(var assembly in config.Assemblies)
                foreach (Type type in assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(SoapControllerAttribute))))
                {
                    var controller = ActivatorUtilities.CreateInstance(serviceProvider, type);
                    var routeAttribute = type.GetCustomAttribute<SoapControllerAttribute>();

                    if (!routeAttribute.Routes.Any())
                        controllers.Add( type.Name.Replace("Controller", ""), controller );
                    else
                        foreach (string route in routeAttribute.Routes)
                            controllers.Add(route, controller);
                }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Handle(context);
            await next(context);
        }

        private void Handle(HttpContext context)
        {
            if (!string.Equals(context.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase) || 
                !contentTypes.Contains(context.Request.ContentType))
                return;

            //var controller = controllers.FirstOrDefault(c => context.Request.)
        }
    }
}
