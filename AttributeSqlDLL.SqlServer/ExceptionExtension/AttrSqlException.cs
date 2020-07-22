using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlServer.ExceptionExtension
{
    public class AttrSqlException : Exception
    {
        public AttrSqlException(string errorMessage) : base(errorMessage) { }
    }
}
