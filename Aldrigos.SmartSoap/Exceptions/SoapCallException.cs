using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Aldrigos.SmartSoap.Exceptions
{
    [Serializable]
    public class SoapCallException : Exception
    {
        public HttpStatusCode ResponseStatusCode { get; }
        public string ResponseContent { get; }

        public SoapCallException()
        {
        }

        public SoapCallException(string message) : base(message)
        {
        }

        public SoapCallException(string message, HttpStatusCode responseStatusCode, string responseContent) : this(message)
        {
            this.ResponseStatusCode = responseStatusCode;
            this.ResponseContent = responseContent;
        }

        public SoapCallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SoapCallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}