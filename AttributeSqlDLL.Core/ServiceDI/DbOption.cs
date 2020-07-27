using System.Collections.Generic;
using System.Data;
using AttributeSqlDLL.Core.DbType;
using Microsoft.Extensions.DependencyInjection;

namespace AttributeSqlDLL.Core.ServiceDI
{
    public class DbOption
    {
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection dbConnection { get; set; }
        /// <summary>
        /// 连接数据库类型
        /// </summary>
        public DbEnum dbType { get; set; }

    }
}
