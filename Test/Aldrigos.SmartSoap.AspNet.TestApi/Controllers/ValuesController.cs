using Aldrigos.SmartSoap.AspNet.TestApi31.Models;

namespace Aldrigos.SmartSoap.AspNet.TestApi31.Controllers
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
