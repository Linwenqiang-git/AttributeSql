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

namespace AttributeSql.SqlServer.SqlExecutor
{
    public class SqlServerSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public SqlServerSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public override string PaginationSql(int? Offset, int? Size)
        {
            throw new NotImplementedException("Paging is not supported for the time being");
        }
    }
}
