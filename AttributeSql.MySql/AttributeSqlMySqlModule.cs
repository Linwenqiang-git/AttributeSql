using AttributeSql.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace AttributeSql.MySql
{
    [DependsOn(typeof(AttributeSqlBaseModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreMySQLModule))]
    public class AttributeSqlMySqlModule : AbpModule
    {
    }
}
