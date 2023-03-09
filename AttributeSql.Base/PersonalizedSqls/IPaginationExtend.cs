using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Base.PersonalizedSqls
{
    public partial interface IPaginationExtend
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
