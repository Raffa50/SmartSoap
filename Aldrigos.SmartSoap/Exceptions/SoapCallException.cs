using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Aldrigos.SmartSoap.Exceptions
{
    [Serializable]
    public class SoapCallException : Exception
    {
        public HttpResponseMessage Response { get; private set; }

        public SoapCallException()
        {
        }

        public SoapCallException(string message) : base(message)
        {
        }

        public SoapCallException(string message, HttpResponseMessage response) : this(message)
        {
            this.Response = response;
        }

        public SoapCallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SoapCallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}