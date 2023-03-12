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
using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;

namespace AttributeSql.SqlServer.SqlExecutor
{
    public class SqlServerSqlExecutor<TDbContext> : ASqlExecutor<TDbContext>, ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        public SqlServerSqlExecutor(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }        
    }
}
