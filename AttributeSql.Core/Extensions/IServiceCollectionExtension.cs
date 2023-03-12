using AttributeSql.Base.SpecialSqlGenerators;
using AttributeSql.Base.SqlExecutor;
using AttributeSql.MySql.SpecialSqlGenerator;
using AttributeSql.MySql.SqlExecutor;
using AttributeSql.Oracle.SpecialSqlGenerator;
using AttributeSql.Oracle.SqlExecutor;
using AttributeSql.PgSql.SpecialSqlGenerator;
using AttributeSql.PgSql.SqlExecutor;
using AttributeSql.SqlServer.SqlExecutor;
using AttributeSql.SqlServer.SqlGenerator;

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
            service.AddTransient(typeof(ASpecialSqlGenerator), typeof(PgSqlSqlGenerator));            
        }
        public static void AddOracleExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(OracleSqlExecutor<>));
            service.AddTransient(typeof(ASpecialSqlGenerator), typeof(OracleSqlGenerator));
        }
        public static void AddMySqlExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(MysqlSqlExecutor<>));
            service.AddTransient(typeof(ASpecialSqlGenerator), typeof(MySqlSqlGenerator));
        }
        public static void AddSqlServerExecutorService([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(SqlServerSqlExecutor<>));
            service.AddTransient(typeof(ASpecialSqlGenerator), typeof(SqlServerSqlGenerator));
        }
    }
}
