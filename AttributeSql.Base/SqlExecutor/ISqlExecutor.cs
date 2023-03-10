using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

using AttributeSql.Base.PersonalizedSqls;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Base.SqlExecutor
{
    /// <summary>
    /// Sql执行器
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface ISqlExecutor<TDbContext> : IPaginationExtend, IParameterBuildExtend, IRelationBuildExtend where TDbContext : IEfCoreDbContext
    {
        #region Db Context
        Task<TDbContext> GetDbContextAsync();
        #endregion

        #region  DbQueryExtend
        Task<IEnumerable<T>> QueryListBySqlAsync<T>(string sql, int timeout, params object[] parameters)
            where T : class, new();
        Task<IEnumerable<T>> QueryListBySqlAsync<T>(string sql, params object[] parameters)
            where T : class, new();

        ValueTask<int> QueryCountBySqlAsync(string sql, int timeout, object[] parameters);
        ValueTask<int> QueryCountBySqlAsync(string sql, params object[] parameters);
        #endregion

        #region DbNonQueryExtend
        ValueTask<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, params object[] parameters);
        ValueTask<int> ExecuteNonQueryAsync(string sql);
        #endregion
    }
}
