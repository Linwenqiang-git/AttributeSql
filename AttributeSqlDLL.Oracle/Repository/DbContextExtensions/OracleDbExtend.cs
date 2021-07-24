using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Reflection;
using Dapper;
using System.Threading.Tasks;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.SqlExtendMethod;
using AttributeSqlDLL.Common;

namespace AttributeSqlDLL.Oracle.Repository.DbContextExtensions
{
    public class OracleDbExtend : AbstractDbExtend
    {
        public OracleDbExtend() { }

        private async Task CommonExecute<TParamter>(IDbConnection conn, string sql, Func<OracleCommand, Task> func, TParamter parameters = null, IDbTransaction tran = null)
            where TParamter : class
        {
            IDbCommand cmd = conn.CreateCommand(sql, parameters);
            try
            {
                OracleCommand oracleCommand = cmd as OracleCommand;
                if (tran != null)
                {
                    //包含事务就不要释放连接，由事务调用处统一关闭
                    oracleCommand.Transaction = tran as OracleTransaction;
                    await func(oracleCommand);
                }
                else
                {
                    using (conn)
                    {
                        await func(oracleCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AttrSqlException(ex.Message);
            }
        }
       

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
        public override async Task<long> ExecuteNonQueryByKey<TParamter>(IDbConnection conn, string sql, TParamter parameters,IDbTransaction tran = null)
        {
            long IdentityId = 0;
            await CommonExecute(conn, sql, async (ClientDbCommand) => {
                IdentityId = Convert.ToInt64(await ClientDbCommand.ExecuteScalarAsync());
            }, parameters, tran);
            return IdentityId;
        }
        #endregion

        #region  IPaginationExtend       
        public override string PaginationSql(int? Offset, int? Size)
        {
            throw new NotImplementedException("暂不支持分页");
        }
       
        #endregion
    }
}
