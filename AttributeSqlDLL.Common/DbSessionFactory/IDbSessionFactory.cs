using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AttributeSqlDLL.Common.DbSessionFactory
{
    public interface IDbSessionFactory
    {
        IDbConnection Create(string connSql, DatabaseType databaseType = DatabaseType.MySql);
    }
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        SqlServer = 1,
        MySql = 2,
        Oracle = 3
    }
}
