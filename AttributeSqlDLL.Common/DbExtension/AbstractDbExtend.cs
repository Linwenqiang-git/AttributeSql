using AttributeSqlDLL.Common.SqlExtendMethod;

using Dapper;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSqlDLL.Common
{
    public abstract class AbstractDbExtend : IDbExtend
    {
        public virtual async Task<int> ExecuteNonQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null) where TParamter : class
        {
            return await conn.ExecuteAsync(sql, parameters, tran);
        }

        public virtual async Task<int> ExecuteNonQuery(IDbConnection conn, string sql, IDbTransaction tran = null)
        {
            return await conn.ExecuteAsync(sql, transaction: tran);
        }

        public abstract Task<long> ExecuteNonQueryByKey(IDbConnection conn, string sql, IDbTransaction tran = null);
        public abstract Task<long> ExecuteNonQueryByKey<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null) where TParamter : class;        

        public abstract string PaginationSql(int? Offset, int? Size);        

        public virtual async Task<int> SqlCountQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null) where TParamter : class
        {
            return await conn.ExecuteScalarAsync<int>(sql, parameters, tran);
        }

        public virtual async Task<IEnumerable<T>> SqlQuery<T, TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where T : class, new()
            where TParamter : class
        {
            return await conn.QueryAsync<T>(sql, parameters, tran);
        }

        public virtual async Task<int> SqlRowsQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null) where TParamter : class
        {
            var Count = await conn.QueryAsync<int>(sql, parameters, tran);
            if ((bool)Count?.Any())
            {
                return Count.Count();
            }
            return 0;
        }
    }
}
