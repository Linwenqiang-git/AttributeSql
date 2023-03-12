using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Base.SpecialSqlGenerators
{
    public interface IPaginationGenerator
    {
        /// <summary>
        /// 分页sql
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        string PaginationSql(int? Offset,int? Size);
    }
}
