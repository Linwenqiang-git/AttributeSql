using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSqlDLL.Common.SqlExtendMethod
{
    public partial interface IDbExtend : IPaginationExtend
    {       
        #region  DbQueryExtend
        Task<IEnumerable<T>> SqlQuery<T, TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where T : class, new()
            where TParamter : class;

        Task<int> SqlCountQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class;
        Task<int> SqlRowsQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class;
        #endregion

        #region DbNonQueryExtend
        Task<int> ExecuteNonQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null)
            where TParamter : class;
        Task<int> ExecuteNonQuery(IDbConnection conn, string sql, IDbTransaction tran = null);
        Task<long> ExecuteNonQueryByKey(IDbConnection conn, string sql, IDbTransaction tran = null);
        Task<long> ExecuteNonQueryByKey<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null)
            where TParamter : class;
        #endregion

    }
}
