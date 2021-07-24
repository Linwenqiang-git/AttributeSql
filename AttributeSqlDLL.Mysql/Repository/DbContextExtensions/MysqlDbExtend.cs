using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using AttributeSqlDLL.Common;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.SqlExtendMethod;
using AttributeSqlDLL.Mysql.Repository;

using Dapper;

using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.Mysql.Repository.DbContextExtensions
{
    public class MySqlDbExtend : AbstractDbExtend
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
            IDbCommand cmd = conn.CreateCommand(sql, parameters);
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
        

        #region  DbNonQueryExtend        
        /// <summary>
        /// 新增返回主键
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override async Task<long> ExecuteNonQueryByKey(IDbConnection conn, string sql, IDbTransaction tran = null)
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
        public override async Task<long> ExecuteNonQueryByKey<TParamter>(IDbConnection conn, string sql, TParamter parameters, IDbTransaction tran = null)
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                await ClientDbCommand.ExecuteNonQueryAsync();
                IdentityId = ClientDbCommand.LastInsertedId;
            }, parameters, tran);
            return IdentityId;
        }
        #endregion

        public override string PaginationSql(int? Offset, int? Size)
        {
            return $"LIMIT {Offset},{Size}";
        }       
    }
}
