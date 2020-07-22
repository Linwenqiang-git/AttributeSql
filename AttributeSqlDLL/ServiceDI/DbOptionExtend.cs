using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.Core.ServiceDI
{
    public static class DbOptionExtend
    {
        public static DbOption UseMysql(this DbOption option, string dbConnString)
        {
            option.dbType = DbType.DbEnum.Mysql;
            option.dbConnection = new MySqlConnection(dbConnString);            
            return option;
        }
        public static DbOption UseSqlServer(this DbOption option, string dbConnString)
        {
            option.dbType = DbType.DbEnum.SqlServer;
            option.dbConnection = new SqlConnection(dbConnString);
            return option;
        }
    }
}
