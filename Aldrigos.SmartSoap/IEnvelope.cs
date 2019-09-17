using System;
using System.Collections.Generic;

namespace Aldrigos.SmartSoap
{
    public interface IEnvelope
    {
        object[] Body { get; }
        IEnumerable<object> Headers { get; }
    }
}
