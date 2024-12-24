using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Aldrigos.SmartSoap.Extensions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;

namespace Aldrigos.SmartSoap.AspNet
{
    public sealed class SoapEndpointMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<string> contentTypes;
        private readonly SoapMiddlewareConfig config;

        private readonly IDictionary<string, object> controllers = new Dictionary<string, object>();
        private readonly IXmlSerializer serializer;

        public SoapEndpointMiddleware(RequestDelegate next, SoapMiddlewareConfig config, IServiceProvider serviceProvider)
        {
            this.next = next;
            this.config = config;
            this.serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

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
            if(!Handle(context).GetAwaiter().GetResult())
                await next(context);
        }

        private async Task<bool> Handle(HttpContext context)
        {
            if (!string.Equals(context.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase) || 
                !contentTypes.Contains(context.Request.ContentType))
                return false;

            string requestPath = context.Request.Path.ToString();

            var controllerKv = controllers.FirstOrDefault(c => requestPath.Contains(c.Key));
            var controller = controllerKv.Value;
            if (controller == null)
                return false;

            string methodName = requestPath.Replace($"/{controllerKv.Key}/", "");
            var method = controller.GetType().GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
                return false;

            var requestBodyType = method.GetParameters()[0].ParameterType;
            string content;
            using (var reader = new StreamReader(context.Request.Body))
                content = await reader.ReadToEndAsync();
            IEnvelope requestBody;
            try
            {
                var envelopeT = typeof(Envelope<>).MakeGenericType(requestBodyType);
                requestBody = (IEnvelope)serializer.DeserializeObject(envelopeT, content);
            } catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("");
                return true;
            }

            var resp = method.Invoke(controller, new[] { requestBody.Body[0] });
            var envelopeRet = typeof(Envelope<>).MakeGenericType(method.ReturnType);
            var ret = Activator.CreateInstance(envelopeRet, resp);

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = context.Request.ContentType;
            await context.Response.WriteAsync( serializer.SerializeObject(ret) );
            return true;
        }
    }
}
