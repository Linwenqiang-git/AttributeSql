using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.ServiceDI
{
    public static class AttrDbConnectionExtend
    {
        public static IServiceCollection AddDbConnection<TContext>([NotNull] this IServiceCollection service, [CanBeNull] Func<DbConnection, DbConnection> optionsFunc = null, ServiceLifetime contextLifetime = ServiceLifetime.Transient, ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
            where TContext : DbConnection
        {
            DbConnection dbConnection = null;
            //根据数据库类型决定实例化的对象，这里暂时写死用mysql
            service.AddTransient(c => optionsFunc(dbConnection));
            return service;
        }
        public static DbConnection UseMysql(this DbConnection connection, string connStr)
        {
            connection = new MySqlConnection(connStr);
            return connection;
        }        
    }
}
