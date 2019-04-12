using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Aldrigos.SmartSoap.Extensions;
using System.Linq;

namespace Aldrigos.SmartSoap.AspNet
{
    public sealed class SoapEndpointMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<string> contentTypes;

        public SoapEndpointMiddleware(RequestDelegate next)
        {
            this.next = next;
            contentTypes = Enum.GetValues(typeof(SoapContentType)).Cast<SoapContentType>().Select(e => e.ToEnumString());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Handle(context);
            await next(context);
        }

        private void Handle(HttpContext context)
        {
            if (!contentTypes.Contains(context.Request.ContentType))
                return;
        }
    }
}
