using AttributeSql.Base.SqlExecutor;
using AttributeSql.MySql.SqlExecutor;
using AttributeSql.Oracle.SqlExecutor;
using AttributeSql.PgSql.SqlExecutor;
using AttributeSql.SqlServer.SqlExecutor;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Core.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static void AddPgsqlExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(PgSqlSqlExecutor<>));            
        }
        public static void AddOracleExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(OracleSqlExecutor<>));            
        }
        public static void AddMySqlExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(MysqlSqlExecutor<>));
        }
        public static void AddSqlServerExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(SqlServerSqlExecutor<>));
        }
    }
}
