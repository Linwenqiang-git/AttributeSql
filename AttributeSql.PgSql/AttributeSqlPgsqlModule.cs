using AttributeSql.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace AttributeSql.Pgsql
{
    [DependsOn(typeof(AttributeSqlBaseModule))]
    [DependsOn(typeof(AbpEntityFrameworkCorePostgreSqlModule))]
    public class AttributeSqlPgsqlModule : AbpModule
    {
    }
}
