using AttributeSql.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Oracle;
using Volo.Abp.Modularity;

namespace AttributeSql.Oracle
{
    [DependsOn(typeof(AttributeSqlBaseModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreOracleModule))]
    public class AttributeSqlOracleModule : AbpModule
    {
    }
}
