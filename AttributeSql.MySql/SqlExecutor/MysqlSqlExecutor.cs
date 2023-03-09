using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttributeSql.Base.SqlExecutor;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace AttributeSql.MySql.SqlExecutor
{
    public class MysqlSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public MysqlSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"LIMIT {Offset},{Size}";
        }
    }
}
