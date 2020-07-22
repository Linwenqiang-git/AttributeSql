using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using AttributeSqlDLL.SqlServer.ExceptionExtension;

namespace AttributeSqlDLL.SqlServer.Repository.DbContextExtensions
{
    /// <summary>
    /// 非查询类扩展方法
    /// </summary>
    public static class DbNonQueryExtend
    {
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQuery<TParamter>(this DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null)
            where TParamter : class
        {
            int Rows = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                Rows = await ClientDbCommand.ExecuteNonQueryAsync();
            }, parameters, tran);
            return Rows;
        }
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQuery(this DbConnection conn, string sql, DbTransaction tran = null)
        {
            int Rows = 0;
            await CommonExecute<object>(conn, sql, async (ClientDbCommand) => {
                Rows = await ClientDbCommand.ExecuteNonQueryAsync();
            }, null, tran);
            return Rows;
        }
        /// <summary>
        /// 新增返回主键
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task<long> ExecuteNonQueryByKey(this DbConnection conn, string sql, DbTransaction tran = null)
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                IdentityId = Convert.ToInt64(await ClientDbCommand.ExecuteScalarAsync());                
            }, tran);
            return IdentityId;
        }
        /// <summary>
        /// 新增返回主键
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<long> ExecuteNonQueryByKey<TParamter>(this DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null) where TParamter : class
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                IdentityId = Convert.ToInt64(await ClientDbCommand.ExecuteScalarAsync());
            }, parameters, tran);
            return IdentityId;
        }
        #region  内部使用
        private static async Task CommonExecute<TParamter>(DbConnection conn, string sql, Func<SqlCommand, Task> func, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class
        {
            DbCommand cmd = conn.CreateCommand(sql, parameters);
            try
            {                
                SqlCommand sqlCommand = cmd as SqlCommand;
                if (tran != null)
                {
                    //包含事务就不要释放连接，由事务调用出统一关闭
                    sqlCommand.Transaction = tran as SqlTransaction;
                    await func(sqlCommand);
                }
                else
                {
                    using (conn)
                    {
                        await func(sqlCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AttrSqlException(ex.Message);
            }
        }
        #endregion

    }
}
