using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Aldrigos.SmartSoap.Extensions
{
    internal static class EnumExtensions
    {
        public static string ToEnumString(this Enum enumValue)
        {
            var memInfo = enumValue.GetType().GetMember(enumValue.ToString())[0];
            string str = memInfo.GetCustomAttributes<EnumMemberAttribute>().FirstOrDefault()?.Value;
            if (str == null)
                str = memInfo.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;

            return str ?? enumValue.ToString();
        }

        public static Enum ToEnum( this string enumValue, Type enumType ) {
            if(!enumType.IsEnum)
                throw new InvalidOperationException(enumType.Name + " is not an enum");

            foreach( var em in enumType.GetMembers(BindingFlags.Public | BindingFlags.Static) ) {
                if (em.GetCustomAttributes<EnumMemberAttribute>().FirstOrDefault()?.Value == enumValue  ||
                    em.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description == enumValue ||
                    em.Name == enumValue )

                    return (Enum) Enum.Parse( enumType, em.Name );
            }
            
            throw new InvalidOperationException(enumValue+" not found in "+enumType.Name);
        }
    }
}
