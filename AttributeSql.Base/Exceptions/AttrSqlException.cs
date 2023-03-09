using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Base.Exceptions
{
    public class AttrSqlException : Exception
    {
        public AttrSqlException(string errorMessage) : base(errorMessage) { }
    }
}
