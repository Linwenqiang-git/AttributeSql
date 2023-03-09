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

namespace AttributeSql.PgSql.SqlExecutor
{
    public class PgSqlSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public PgSqlSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public override string PaginationSql(int? Offset, int? Size)
        {
            throw new NotImplementedException("Paging is not supported for the time being");
        }
    }
}
