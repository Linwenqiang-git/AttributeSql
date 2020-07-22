using System.Data.Common;
using AttributeSqlDLL.Common.Repository.DbContextExtensions;
using AttributeSqlDLL.Mysql.Repository.DbContextExtensions;
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
                service.AddTransient<IDbExtend, MysqlDbExtend>();
            }
            else if (dbOption.dbType == DbType.DbEnum.SqlServer)
            {
                service.AddTransient<IDbExtend, SqlServerDbExtend>();
            }
            return service;
        }        
    }
}
