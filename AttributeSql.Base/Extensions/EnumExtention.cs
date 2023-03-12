using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum en)
        {
            MemberInfo[] member = en.GetType().GetMember(en.ToStr());
            if (member != null && member.Length != 0)
            {
                object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
                if (customAttributes != null && customAttributes.Length != 0)
                {
                    return ((DescriptionAttribute)customAttributes[0]).Description;
                }
            }

            return en.ToStr();
        }
        public static TEnum ToEnum<TEnum>(this object para) where TEnum : Enum
        {
            Type typeFromHandle = typeof(TEnum);
            if (!Enum.IsDefined(typeFromHandle, para))
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
                defaultInterpolatedStringHandler.AppendLiteral("Value:");
                defaultInterpolatedStringHandler.AppendFormatted<object>(para);
                defaultInterpolatedStringHandler.AppendLiteral(" is not included ");
                defaultInterpolatedStringHandler.AppendFormatted(typeFromHandle.Name);
                defaultInterpolatedStringHandler.AppendLiteral("!");
                throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            return (TEnum)Enum.ToObject(typeFromHandle, para);
        }

        public static Dictionary<string, string> EnumToList<T>()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (object value2 in Enum.GetValues(typeof(T)))
            {
                string value = string.Empty;
                object[] customAttributes = value2.GetType().GetField(value2.ToStr())!.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
                if (customAttributes != null && customAttributes.Length != 0)
                {
                    value = (customAttributes[0] as DescriptionAttribute).Description;
                }

                dictionary.Add(value2.ToStr(), value);
            }

            return dictionary;
        }
    }         
}
