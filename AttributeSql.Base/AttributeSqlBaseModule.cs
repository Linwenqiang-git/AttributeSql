using AttributeSql.Base.SqlExecutor;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AttributeSql.Base
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class AttributeSqlBaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {            
        }
    }
}
