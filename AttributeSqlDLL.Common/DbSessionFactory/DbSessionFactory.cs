using MySql.Data.MySqlClient;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace AttributeSqlDLL.Common.DbSessionFactory
{
    public class DbSessionFactory : IDbSessionFactory
    {
        public IDbConnection Create(string connSql, DatabaseType databaseType = DatabaseType.MySql)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlConnection(connSql);
                case DatabaseType.Oracle:
                    return new OracleConnection(connSql);
                default:
                    return new MySqlConnection(connSql);                    
            }
        }
        
    }
}
