using AttributeSql.Base;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Demo.DbContext
{
    [ConnectionStringName("Default")]
    public class AttributeSqlDemoDbContext : AttrDbContext<AttributeSqlDemoDbContext>, IEfCoreDbContext
    {
        public AttributeSqlDemoDbContext(DbContextOptions<AttributeSqlDemoDbContext> options) : base(options)
        {
        }        
    }
}
