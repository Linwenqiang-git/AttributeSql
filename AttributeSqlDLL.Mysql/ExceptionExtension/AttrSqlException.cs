using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Mysql.ExceptionExtension
{
    public class AttrSqlException : Exception
    {
        public AttrSqlException(string errorMessage) : base(errorMessage) { }
    }
}
