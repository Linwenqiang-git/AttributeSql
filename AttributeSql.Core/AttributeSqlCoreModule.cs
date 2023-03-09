﻿using AttributeSql.Base;
using AttributeSql.Core.Repository;
using AttributeSql.MySql;
using AttributeSql.Oracle;
using AttributeSql.Pgsql;
using AttributeSql.SqlServer;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;

namespace AttributeSql.Core
{
    [DependsOn(typeof(AttributeSqlPgsqlModule),
               typeof(AttributeSqlMySqlModule),
               typeof(AttributeSqlOracleModule),
               typeof(AttributeSqlSqlServerModule))]    
    [DependsOn(typeof(AttributeSqlBaseModule))]
    public class AttributeSqlCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapper(typeof(AttributeSqlCoreModule));
            //添加缓存机制
            context.Services.AddSingleton<IMemoryCache>(factory => new MemoryCache(new MemoryCacheOptions()));
            context.Services.AddTransient(typeof(AttrSqlGenerator<>));
        }
    }
}
