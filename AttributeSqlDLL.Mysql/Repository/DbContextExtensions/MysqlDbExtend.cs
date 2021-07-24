using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.SqlExtendMethod;

using Dapper;

using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.Mysql.Repository.DbContextExtensions
{
    public class MySqlDbExtend : IDbExtend
    {
        public MySqlDbExtend() { }

        #region  内部使用
        /// <summary>
        /// 仅用于返回新增主键时使用
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        private async Task CommonExecute<TParamter>(IDbConnection conn, string sql, Func<MySqlCommand, Task> func, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class
        {
            DbCommand cmd = new DbCommand(sql, parameters);
            try
            {
                //暂时写死，后续根据连接情况设置多数据库连接
                MySqlCommand mysqlCommand = cmd as MySqlCommand;
                if (tran != null)
                {
                    //包含事务就不要释放连接，由事务调用出统一关闭
                    mysqlCommand.Transaction = tran as MySqlTransaction;
                    await func(mysqlCommand);
                }
                else
                {                    
                    await func(mysqlCommand);                    
                }
            }
            catch (Exception ex)
            {
                throw new AttrSqlException(ex.Message);
            }
        }        
        #endregion

        #region  DbQueryExtend
        /// <summary>
        /// 执行指定的查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">参数化的字段</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SqlQuery<T, TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where T : class, new()
            where TParamter : class
        {
            return await conn.QueryAsync<T>(sql, parameters, tran);            
        }
        /// <summary>
        /// 通过count(1)返回查询的数据总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> SqlCountQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class
        {
            return await conn.ExecuteScalarAsync<int>(sql, parameters, tran);
        }
        /// <summary>
        /// 通过完整sql语句查询数据总数
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public async Task<int> SqlRowsQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class
        {
            var Count = await conn.QueryAsync<int>(sql, parameters, tran);
            if ((bool)Count?.Any())
            {
                return Count.Count();
            }
            return 0;
        }
        #endregion

        #region  DbNonQueryExtend
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQuery<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null)
            where TParamter : class
        {
            return await conn.ExecuteAsync(sql, parameters, tran);                        
        }
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQuery(IDbConnection conn, string sql, IDbTransaction tran = null)
        {
            return await conn.ExecuteAsync(sql,transaction: tran);
        }
        /// <summary>
        /// 新增返回主键
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<long> ExecuteNonQueryByKey(IDbConnection conn, string sql, IDbTransaction tran = null)
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                await ClientDbCommand.ExecuteNonQueryAsync();
                IdentityId = ClientDbCommand.LastInsertedId;
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
        public async Task<long> ExecuteNonQueryByKey<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null) where TParamter : class
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                await ClientDbCommand.ExecuteNonQueryAsync();
                IdentityId = ClientDbCommand.LastInsertedId;
            }, parameters, tran);
            return IdentityId;
        }
        #endregion

        public string PaginationSql(int? Offset, int? Size)
        {
            return $"LIMIT {Offset},{Size}";
        }

        
    }
}
