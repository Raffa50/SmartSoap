using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aldrigos.SmartSoap.Extensions
{
    internal static class EnumExtensions
    {
        public static string ToEnumString(this Enum enumValue)
        {
            var memInfo = enumValue.GetType().GetMember(enumValue.ToString())[0];
            string str = memInfo.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault()?.Value;
            if (str == null)
                str = memInfo.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault()?.Description;

            return str ?? enumValue.ToString();
        }
    }
}
