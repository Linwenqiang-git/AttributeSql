using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using AttributeSqlDLL.Common.ExceptionExtension;
using AttributeSqlDLL.Common.SqlExtendMethod;
using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.Mysql.Repository.DbContextExtensions
{
    public class MySqlDbExtend : IDbExtend
    {
        public MySqlDbExtend() { }

        #region  内部使用
        private async Task CommonExecute<TParamter>(DbConnection conn, string sql, Func<MySqlCommand, Task> func, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class
        {
            DbCommand cmd = conn.CreateCommand(sql, parameters);
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
                    using (conn)
                    {
                        await func(mysqlCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AttrSqlException(ex.Message);
            }
        }
        /// <summary>
        /// 内部使用-执行最终的查询
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<DataTable> SqlQuery<TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null) where TParamter : class
        {
            try
            {
                DbCommand cmd = conn.CreateCommand(sql, parameters);
                DataSet ds = new DataSet();
                //暂时写死，后续根据连接情况设置多数据库连接                
                MySqlCommand mysqlCommand = cmd as MySqlCommand;
                MySqlDataAdapter adapter = new MySqlDataAdapter(mysqlCommand);
                if (tran != null)
                {
                    await adapter.FillAsync(ds);
                    return ds.Tables[0];
                }
                using (conn)
                {
                    await adapter.FillAsync(ds);
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw new AttrSqlException(ex.Message);
            }
        }
        /// <summary>
        /// 将datatable转换成IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private IEnumerable<T> ToEnumerable<T>(DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            string fieldName = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    T t = new T();
                    foreach (PropertyInfo p in propertyInfos)
                    {
                        fieldName = p.Name;
                        if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        {
                            p.SetValue(t, row[p.Name]);
                        }
                    }
                    ts[i] = t;
                    i++;
                }
                catch (Exception ex)
                {
                    throw new AttrSqlException($"查询结果[{fieldName}]类型转换出错,请检查Dto模型参数类型配置：{ex.Message}");
                }
            }
            return ts;
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
        public async Task<IEnumerable<T>> SqlQuery<T, TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where T : class, new()
            where TParamter : class
        {
            DataTable dt = await SqlQuery(conn, sql, parameters, tran);
            return ToEnumerable<T>(dt);
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
        public async Task<int> SqlCountQuery<TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class
        {
            DataTable dt = await SqlQuery(conn, sql, parameters, tran);
            int Rownum = 0;
            if (dt?.Rows?.Count > 0)
            {
                Rownum = Convert.ToInt32(dt.Rows[0]["rownum"].ToString());
            }
            return Rownum;
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
        public async Task<int> SqlRowsQuery<TParamter>(DbConnection conn, string sql, TParamter parameters = null, DbTransaction tran = null)
            where TParamter : class
        {
            DataTable dt = await SqlQuery(conn, sql, parameters, tran);
            return (int)dt?.Rows?.Count;
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
        public async Task<int> ExecuteNonQuery<TParamter>(DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null)
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
        public async Task<int> ExecuteNonQuery(DbConnection conn, string sql, DbTransaction tran = null)
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
        public async Task<long> ExecuteNonQueryByKey(DbConnection conn, string sql, DbTransaction tran = null)
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
        public async Task<long> ExecuteNonQueryByKey<TParamter>(DbConnection conn, string sql, TParamter parameters, DbTransaction tran = null) where TParamter : class
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
