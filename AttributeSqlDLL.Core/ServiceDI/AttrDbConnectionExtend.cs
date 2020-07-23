using System.Data.Common;
using AttributeSqlDLL.Common.SqlExtendMethod;
using AttributeSqlDLL.Core.ExceptionExtension;
using AttributeSqlDLL.Mysql.Repository.DbContextExtensions;
using AttributeSqlDLL.Oracle.Repository.DbContextExtensions;
using AttributeSqlDLL.SqlServer.Repository.DbContextExtensions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AttributeSqlDLL.Core.ServiceDI
{
    public static class AttrDbConnectionExtend
    {
        public static IServiceCollection AddDbConnection([NotNull] this IServiceCollection service, [NotNull] DbOption dbOption)
        {           
            service.AddTransient(c => dbOption.dbConnection as DbConnection);
            if (dbOption.dbType == DbType.DbEnum.Mysql)
            {
                service.AddTransient<IDbExtend, MySqlDbExtend>();
            }
            else if (dbOption.dbType == DbType.DbEnum.SqlServer)
            {
                service.AddTransient<IDbExtend, SqlServerDbExtend>();
            }
            else if (dbOption.dbType == DbType.DbEnum.Oracle)
            {
                service.AddTransient<IDbExtend, OracleDbExtend>();
            }
            else
            {
                throw new AttrSqlException("无法识别的数据库类型.");
            }
            return service;
        }        
    }
}
