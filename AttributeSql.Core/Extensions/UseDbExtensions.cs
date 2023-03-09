using AttributeSql.Base.SqlExecutor;
using AttributeSql.MySql.SqlExecutor;
using AttributeSql.Oracle.SqlExecutor;
using AttributeSql.PgSql.SqlExecutor;
using AttributeSql.SqlServer.SqlExecutor;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace AttributeSql.Core.Extensions
{
    public static class UseDbExtensions
    {
        public static void UsePgsql([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(PgSqlSqlExecutor<>));
        }
        public static void UseOracle([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(OracleSqlExecutor<>));
        }
        public static void UseMysql([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(MysqlSqlExecutor<>));
        }
        public static void UseSqlServer([NotNull] this IServiceCollection service)
        {
            service.AddTransient(typeof(ISqlExecutor<>), typeof(SqlServerSqlExecutor<>));
        }
    }
}
