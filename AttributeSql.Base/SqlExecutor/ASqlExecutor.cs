using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using AttributeSql.Base.Helper;
using System.Security.Cryptography;
using AttributeSql.Base.PersonalizedSqls;
using Volo.Abp.DependencyInjection;

namespace AttributeSql.Base.SqlExecutor
{
    public abstract class ASqlExecutor<TDbContext> : ISqlExecutor<TDbContext> where TDbContext : IEfCoreDbContext, IDisposable, ITransientDependency
    {
        private TDbContext dbcontext;

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;
        public ASqlExecutor(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }
        #region Db Context
        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns></returns>
        public async Task<TDbContext> GetDbContextAsync()
        {
            if (dbcontext is null)
            {
                dbcontext = await _dbContextProvider.GetDbContextAsync();
            }
            return dbcontext;
        }
        #endregion

        #region  DbQueryExtend
        public async Task<IEnumerable<T>> QueryListBySqlAsync<T>(string sql, params object[] parameters)
                 where T : class, new() => await QueryListBySqlAsync<T>(sql, 30, parameters);

        /// <summary>
        /// 执行指定的查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters">参数化的字段</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryListBySqlAsync<T>(string sql,int timeout, params object[] parameters)
            where T : class, new()
        {
            var _context = await GetDbContextAsync();
            var dbConnection = _context.Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                await _context.Database.OpenConnectionAsync();
            }

            await using var command = dbConnection.CreateCommand();
            //验证事务是否开启
            if (_context.Database.CurrentTransaction != null)
            {
                command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
            }

            command.CommandTimeout = timeout;
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            command.CommandType = CommandType.Text;
            await using var reader = await command.ExecuteReaderAsync();
            if (reader == null || reader.HasRows == false)
                return default;
            //返回结果
            var resultList = new List<T>();
            //读取一行
            while (await reader.ReadAsync())
                resultList.Add(ExpressionToGeneric<T>.ToClass(reader));
            return resultList;
        }
        /// <summary>
        /// 通过count(*)返回查询的数据总数
        /// </summary>
        /// <typeparam name="TParamter"></typeparam>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public async ValueTask<int> QueryCountBySqlAsync(string sql, int timeout, object[] parameters)            
        {
            var _context = await GetDbContextAsync();
            var dbConnection = _context.Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                await _context.Database.OpenConnectionAsync();
            }

            await using var command = dbConnection.CreateCommand();
            //验证事务是否开启
            if (_context.Database.CurrentTransaction != null)
            {
                command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
            }

            command.CommandTimeout = timeout;
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            command.CommandType = CommandType.Text;
            await using var reader = await command.ExecuteReaderAsync();
            if (reader == null || reader.HasRows == false)
                return default;
            var dt = new DataTable();
            dt.Load(reader: reader);
            if (dt.Rows?.Count <= 0)
            {
                return default;
            }
            return Convert.ToInt32(dt.Rows[0]["rownum"].ToString());
        }
        public async ValueTask<int> QueryCountBySqlAsync(string sql, params object[] parameters)
            =>await QueryCountBySqlAsync(sql,30,parameters);
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
        public async ValueTask<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, params object[] parameters)            
        {
            var _context = await GetDbContextAsync();
            var dbConnection = _context.Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                await _context.Database.OpenConnectionAsync();
            }

            await using var command = dbConnection.CreateCommand();
            //验证事务是否开启
            if (_context.Database.CurrentTransaction != null)
            {
                command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
            }

            command.CommandText = sql;
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            command.CommandType = commandType;
            return await command.ExecuteNonQueryAsync();
        }
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async ValueTask<int> ExecuteNonQueryAsync(string sql)
        {
            var _context = await GetDbContextAsync();
            var dbConnection = _context.Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                await _context.Database.OpenConnectionAsync();
            }
            await using var command = dbConnection.CreateCommand();
            //验证事务是否开启
            if (_context.Database.CurrentTransaction != null)
            {
                command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
            }
            command.CommandText = sql;            
            return await command.ExecuteNonQueryAsync();
        }
        #endregion

        #region Personalized Sql
        /// <summary>
        /// 分页查询sql
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public abstract string PaginationSql(int? Offset, int? Size);
        #endregion

        public void Dispose()
        {
            
        }

    }
}
