using AttributeSql.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace AttributeSql.SqlServer
{
    [DependsOn(typeof(AttributeSqlBaseModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
    public class AttributeSqlSqlServerModule : AbpModule
    {
    }
}
