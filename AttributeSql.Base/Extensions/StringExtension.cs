using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Extensions
{
    public static class StringExtension
    {
        public static string ToStr(this object source)
        {
            return source?.ToString().Trim() ?? string.Empty;
        }
    }
}
