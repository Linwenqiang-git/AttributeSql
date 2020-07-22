using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSqlDLL.Common.Repository.DbContextExtensions
{
    public interface IDbExtend
    {
        #region  DbQueryExtend
        Task<IEnumerable<T>> SqlQuery<T, TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where T : class, new()
            where TParamter : class;

        Task<int> SqlCountQuery<TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class;
        Task<int> SqlRowsQuery<TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class;
        #endregion

            #region DbNonQueryExtend
        Task<int> ExecuteNonQuery<TParamter>(DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null)
            where TParamter : class;
        Task<int> ExecuteNonQuery(DbConnection conn, string sql, DbTransaction tran = null);
        Task<long> ExecuteNonQueryByKey(DbConnection conn, string sql, DbTransaction tran = null);
        Task<long> ExecuteNonQueryByKey<TParamter>(DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null) where TParamter : class;
        #endregion

    }
}
