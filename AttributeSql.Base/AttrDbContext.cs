using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Base
{
    public abstract class AttrDbContext<TDbContext> : AbpDbContext<TDbContext> where TDbContext : DbContext
    {
        protected AttrDbContext(DbContextOptions<TDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLowerCaseNamingConvention();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);           
        }
    }
}
