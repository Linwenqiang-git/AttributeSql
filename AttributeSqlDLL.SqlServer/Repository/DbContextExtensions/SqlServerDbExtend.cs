using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AttributeSqlDLL.Common;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.Model;
using AttributeSqlDLL.Common.SqlExtendMethod;

namespace AttributeSqlDLL.SqlServer.Repository.DbContextExtensions
{
    public class SqlServerDbExtend : AbstractDbExtend
    {
        private string ConnStr { get; set; }
        public SqlServerDbExtend(DbContextModel dbContext) 
        {
            ConnStr = dbContext.connStr;
        }

        #region  内部使用
        private void BuildConn(ref IDbConnection conn)
        {
            if (conn == null || string.IsNullOrEmpty(conn.ConnectionString))
            {
                try
                {
                    conn = new SqlConnection(ConnStr);
                }
                catch (Exception ex)
                {
                    throw new AttrSqlException($"实例化数据库上下文出错:{ex.Message}");
                }
            }
        }
        private async Task CommonExecute<TParamter>(IDbConnection conn, string sql, Func<SqlCommand, Task> func, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class
        {
            BuildConn(ref conn);
            IDbCommand cmd = conn.CreateCommand(sql, parameters);
            try
            {
                SqlCommand sqlCommand = cmd as SqlCommand;
                if (tran != null)
                {
                    //包含事务就不要释放连接，由事务调用处统一关闭
                    sqlCommand.Transaction = tran as SqlTransaction;
                    await func(sqlCommand);
                }
                else
                {                    
                    await func(sqlCommand);
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
            await CommonExecute(conn, sql, async (ClientIDbCommand) => {
                IdentityId = Convert.ToInt64(await ClientIDbCommand.ExecuteScalarAsync());
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
            await CommonExecute(conn, sql, async (ClientIDbCommand) => {
                IdentityId = Convert.ToInt64(await ClientIDbCommand.ExecuteScalarAsync());
            }, parameters, tran);
            return IdentityId;
        }
        #endregion

        #region  IPaginationExtend
        /// <summary>
        /// 仅支持2012版本及以上版本
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public override string PaginationSql(int? Offset, int? Size)
        {
            return $" Offset {Offset} Rows Fetch Next {Size} Rows Only";
        }
       
        #endregion
    }
}
