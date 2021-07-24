using System.Data.Common;
using System.Data.SqlClient;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.Model;
using AttributeSqlDLL.Common.SqlExtendMethod;
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
            service.AddScoped(c => dbOption.dbConnection as DbConnection);
            
            if (dbOption.dbType == DbType.DbEnum.Mysql)
            {
                service.AddTransient<IDbExtend, MySqlDbExtend>();
            }
            else if (dbOption.dbType == DbType.DbEnum.SqlServer)
            {
                service.AddTransient<IDbExtend, SqlServerDbExtend>();
                //sqlserve 在第二次获取上下文对象时,连接字符串为空(原因未知),需要重新为对象赋值
                DbContextModel dbContext = new DbContextModel()
                {
                    connStr = dbOption.dbConnStr
                };
                service.AddTransient(c => dbContext);
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
