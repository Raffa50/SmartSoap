using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Aldrigos.SmartSoap.TestApp
{
    public enum CipollaType
    {
        [Description("bianca")]
        Bianche,
        [Description("rossa")]
        Rosse,
        [Description("viola")]
        Viola
    }
}
