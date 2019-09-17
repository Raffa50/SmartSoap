using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aldrigos.SmartSoap.AspNet.TestApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aldrigos.SmartSoap.AspNet.TestApi.Controllers
{
    [SoapController("values")]
    public class ValuesController
    {
        public Response Test(Request request)
        {
            return new Response { Content = request.Param1?.ToLower() };
        }
    }
}
