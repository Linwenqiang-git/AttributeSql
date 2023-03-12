using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Exceptions
{
    public class AttrSqlSyntaxError : Exception
    {
        public AttrSqlSyntaxError(string errorMessage) : base(errorMessage) { }
    }
}
