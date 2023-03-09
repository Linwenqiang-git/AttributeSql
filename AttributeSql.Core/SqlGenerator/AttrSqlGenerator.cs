using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttributeExtensions;
using System.Diagnostics;
using AttributeSql.Base.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using AttributeSql.Base.SqlExecutor;
using AttributeSql.Base;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using AttrSqlDbLite.Core.SqlAttributeExtensions;

namespace AttributeSql.Core.Repository
{
    /// <summary>
    /// Sql生成器
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class AttrSqlGenerator<TDbContext> : ITransientDependency where TDbContext : IEfCoreDbContext
    {
        /// <summary>
        /// sql执行器
        /// </summary>
        private ISqlExecutor<TDbContext> _sqlExecutor;
        /// <summary>
        /// sql缓存
        /// </summary>
        private IMemoryCache _sqlMemoryCache;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public AttrSqlGenerator(ISqlExecutor<TDbContext> sqlExecutor, IMemoryCache memoryCache)
        {
            _sqlExecutor = sqlExecutor;
            _sqlMemoryCache = memoryCache;
        }
        #region Private
        private async Task<bool> TryCatch(Func<Task<string>> action)
        {
            string sql = string.Empty;
            bool HasError = false;
            try
            {
                sql = await action();
            }
            catch (AttrSqlException ex)
            {
                HasError = true;
                //配置以及sql问题直接抛出
                if (!string.IsNullOrEmpty(sql))
                {
                    throw new AttrSqlException($"Sql Error:[{ex.Message}]\n" +
                        $"Dynamic sql：[{sql}]");
                }
                throw new AttrSqlException($"The dto model configuration is incorrect:[{ex.Message}]");
            }
            catch (Exception ex)
            {
                HasError = true;
                throw new Exception(ex.Message);
            }
            return HasError;
        }
        /// <summary>
        /// 获取方法调用的堆栈信息
        /// </summary>
        /// <returns></returns>
        private void GetStackTraceModelName<TResultDto>(out string select, out string join, out string groupByHaving) where TResultDto : AttrBaseResult, new()
        {
            //当前堆栈信息
            StackTrace st = new StackTrace();
            select = join = groupByHaving = string.Empty;
            var sts = st.GetFrames()
                     .Select(s => s.GetMethod()).ToList()
                     .Where(s => s.DeclaringType == null ? false : s.DeclaringType.FullName.ToLower().Contains("controller") &&
                            !s.DeclaringType.Name.ToLower().Contains("controller"))
                     .Select(s => s.DeclaringType.FullName
                                   .Replace("__", "").Replace("<", "").Replace(">", "")
                                   + "__" + s.DeclaringType.Name.Replace("__", "").Replace("<", "").Replace(">", ""))
                     .ToList();
            AttrSqlCacheModel value = null;
            if (_sqlMemoryCache.TryGetValue(sts[0], out value))
            {
                select = value.Select;//获取查询的字段
                join = value.Join;//获取连接的表
                groupByHaving = value.GroupByHaving;
                value.CallNum += 1;
            }
            else
            {
                TResultDto dto = new TResultDto();
                select = dto.Select();//获取查询的字段
                join = dto.Join<TResultDto>();//获取连接的表
                groupByHaving = dto.GroupByHaving();
                AttrSqlCacheModel model = new AttrSqlCacheModel()
                {
                    Select = select,
                    Join = join,
                    GroupByHaving = groupByHaving,
                    CallNum = 1
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromHours(8))
                .SetPriority(CacheItemPriority.Normal);
                _sqlMemoryCache.Set(sts[0], model, cacheEntryOptions);
            }
        }
        #endregion

        #region Debug sql
        internal string DebugQuerySql<TResultDto, TPageSearch>(TPageSearch pageSearch,bool IngnorIntDefault = true,Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new()
        {
            TResultDto dto = new TResultDto();            
            string select = dto.Select();//获取查询的字段
            string join = dto.Join<TResultDto>();//获取连接的表
            string where = pageSearch.ParaWhere(IngnorIntDefault);//获取参数化查询where条件      
            if (whereSql != null)
            {
                if (string.IsNullOrEmpty(where))
                {
                    where += $" Where {whereSql.Invoke()}";
                }
                else
                {
                    where += $" and {whereSql.Invoke()}";
                }
            }
            string groupByHaving = dto.GroupByHaving(); //获取分组部分

            //排序规则
            string sort = string.Empty;
            if (!string.IsNullOrEmpty(pageSearch.SortField))
            {
                if (pageSearch.SortWay.ToUpper().Trim() == "ASC" || pageSearch.SortWay.ToUpper().Trim() == "DESC")
                    sort = $" Order by {pageSearch.SortField} {pageSearch.SortWay} ";
                else
                {
                    throw new AttrSqlException("Unrecognized sort order");
                }
            }
            else
            {
                sort = $" {pageSearch.DefaultSort()}";
            }
            //分页规则
            string Limit = string.Empty;
            if (pageSearch.Index != null && pageSearch.Size != null)
            {
                if (pageSearch.Index < 1 || pageSearch.Size < 1)
                {
                    throw new AttrSqlException("unrecognized paged data");
                }
                else
                    Limit = _sqlExecutor.PaginationSql(pageSearch.Offset, pageSearch.Size);
            }
            StringBuilder sql = new StringBuilder();
            sql.Append(select);
            sql.Append(join);
            sql.Append(where);
            sql.Append(groupByHaving);
            sql.Append(sort);
            sql.Append(Limit);
            return sql.ToString();
        }
        #endregion

        #region Query Operate
        /// <summary>
        /// 通用单表查询，按照模型指定的where条件，查询指定单表的全部数据
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="tableName"></param>
        /// <param name="IngnorIntDefault">int类型的默认值是否忽略,默认忽略</param>
        /// <returns></returns>
        internal async Task<AttrPageResult<TEntity>> GetAll<TPageSearch, TEntity>(TPageSearch pageSearch, string tableName = "", bool IngnorIntDefault = true)
            where TPageSearch : AttrPageSearch
            where TEntity : AttrEntityBase, new()
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(TEntity).Name;
            }
            var page = new AttrPageResult<TEntity>(pageSearch.Index, pageSearch.Size);

            string where = pageSearch.ParaWhere(IngnorIntDefault);//获取参数化查询where条件

            string sort = string.Empty;

            if (!string.IsNullOrEmpty(pageSearch.SortField))
            {
                sort = $" Order by {pageSearch.SortField} {pageSearch.SortWay}";
            }
            else
            {
                sort = $" {pageSearch.DefaultSort()}";
            }
            await this.TryCatch(async () =>
            {
                page.Rows = await _sqlExecutor.QueryListBySqlAsync<TEntity>($"select * from {tableName} {where} {sort}", pageSearch);
                return $"select * from {tableName} {where} {sort}";
            });
            return page;
        }
        /// <summary>
        /// 获取指定Dto模型的全部结果集
        /// 若前端传递排序方式，则默认设置的排序方式不生效
        /// </summary>
        /// <typeparam name="TResultDto"></typeparam>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="whereSql">返回指定的where语句</param>
        /// <param name="IngnorIntDefault"></param>
        /// <param name="usingCache">是否启用sql缓存(默认启用)</param>
        /// <returns></returns>
        internal async Task<AttrPageResult<TResultDto>> GetSpecifyResultDto<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                    bool IngnorIntDefault = true,
                                                    Func<string> whereSql = null,
                                                    bool usingCache = true)
            where TResultDto : AttrBaseResult, new()
            where TPageSearch : AttrPageSearch
        {
            StringBuilder sql = new StringBuilder();
            string select = string.Empty, join = string.Empty, groupByHaving = string.Empty;
            if (usingCache)
            {
                GetStackTraceModelName<TResultDto>(out select, out join, out groupByHaving);
            }
            else
            {
                TResultDto dto = new TResultDto();
                select = dto.Select();//获取查询的字段
                join = dto.Join<TResultDto>();//获取连接的表
                groupByHaving = dto.GroupByHaving(); //获取分组部分
            }
            pageSearch = pageSearch?.WhereValueConvert();
            string where = pageSearch.ParaWhere(IngnorIntDefault);//获取参数化查询where条件      
            if (whereSql != null)
            {
                if (string.IsNullOrEmpty(where))
                {
                    where += $" Where {whereSql.Invoke()}";
                }
                else
                {
                    where += $" and {whereSql.Invoke()}";
                }
            }            
            //排序规则
            string sort = string.Empty;
            if (!string.IsNullOrEmpty(pageSearch.SortField))
            {
                if (pageSearch.SortWay.ToUpper().Trim() == "ASC" || pageSearch.SortWay.ToUpper().Trim() == "DESC")
                    sort = $" Order by {pageSearch.SortField} {pageSearch.SortWay} ";
                else
                {
                    throw new AttrSqlException("无法识别的排序方式！");
                }
            }
            else
            {
                sort = $" {pageSearch.DefaultSort()}";
            }
            //分页规则
            string Limit = string.Empty;
            if (pageSearch.Index != null && pageSearch.Size != null)
            {
                if (pageSearch.Index < 1 || pageSearch.Size < 1)
                {                    
                    throw new AttrSqlException("无法识别的分页数据！");
                }
                else
                    Limit = _sqlExecutor.PaginationSql(pageSearch.Offset, pageSearch.Size);
            }            
            sql.Append(select);
            sql.Append(join);
            sql.Append(where);
            sql.Append(groupByHaving);
            sql.Append(sort);
            sql.Append(Limit);
            var page = new AttrPageResult<TResultDto>(pageSearch.Index, pageSearch.Size);
            await TryCatch(async () =>
            {
                var queryTask = _sqlExecutor.QueryListBySqlAsync<TResultDto>($"{sql}", pageSearch);
                int count = 0;
                //如果有分页，统计当前查询共有多少条数据
                if (!string.IsNullOrEmpty(Limit))
                {
                    string countsql = $"SELECT COUNT(1) as rownum {join} {where} {groupByHaving}";
                    try
                    {
                        count = await _sqlExecutor.QueryCountBySqlAsync(countsql, pageSearch);
                    }
                    catch (AttrSqlException ex)
                    {
                        if (ex.Message.ToLower().Contains($"unknown column"))
                        {
                            //去掉limit
                            countsql = $"{select} {join} {where} {groupByHaving}";
                            count = await _sqlExecutor.QueryCountBySqlAsync(countsql, pageSearch);
                        }
                    }
                }
                page.Rows = await queryTask;
                page.Total = count;
                return sql.ToString();
            });
            return page;
        }
        /// <summary>
        /// 执行指定的sql语句
        /// </summary>
        /// <typeparam name="TResultDto"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal async Task<AttrPageResult<TResultDto>> ExecQuerySql<TResultDto>(string sql)
           where TResultDto : AttrBaseResult, new()
        {
            var page = new AttrPageResult<TResultDto>();
            await this.TryCatch(async () =>
            {
                page.Rows = await _sqlExecutor.QueryListBySqlAsync<TResultDto>($"{sql}");
                return sql;
            });
            return page;

        }
        /// <summary>
        /// 执行指定的sql语句内部使用
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal async Task<IEnumerable<TResult>> ExecQuerySqlInside<TResult>(string sql)
           where TResult : AttrBaseResult, new()
        {
            IEnumerable<TResult> result = null;
            await this.TryCatch(async () =>
            {
                result = await _sqlExecutor.QueryListBySqlAsync<TResult>($"{sql}");
                return sql;
            });
            return result;
        }

        #endregion

        #region Data Check
        /// <summary>
        /// 判断指定字段是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="IsAddOrEdit">0新增，1编辑</param>
        /// <returns></returns>
        internal async Task<bool> CheckFieldRepeat<TBaseModel>(TBaseModel model, int IsAddOrEdit)
            where TBaseModel : AttrBaseModel
        {
            List<string> sqls = model.NotAllowRepeatSql();
            if (sqls.Count == 0)
                return true;
            int nums = 1;
            foreach (var sql in sqls)
            {
                int resultnum = await _sqlExecutor.QueryCountBySqlAsync(sql, model);
                if (IsAddOrEdit == 0)
                {
                    if (resultnum != 0)
                        throw new AttrSqlException($"{model.GetErrorMsg(nums)}");
                }
                else
                {
                    if (resultnum > 1)
                        throw new AttrSqlException($"{model.GetErrorMsg(nums)}");
                    if (resultnum == 1)
                    {
                        //判断返回的是否是当前编辑的实体
                        string keySql = model.NotAllowKeySql(nums);
                        var result = await _sqlExecutor.QueryListBySqlAsync<dynamic>(keySql, model);
                        resultnum = result.Count();
                        if (resultnum == 0)
                        {
                            throw new AttrSqlException($"{model.GetErrorMsg(nums)}");
                        }
                    }
                }
                ++nums;
            }
            return true;
        }
        #endregion

        #region Insert Operate
        /// <summary>
        /// 新增返回受影响行数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal async virtual Task<int> InsertAsync<T>(T entity)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.InsertEntity();
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }                
        /// <summary>
        /// 批量新增（非参数化,建议在大批量插入数据时使用）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        internal async Task<int> NonPatameterBatchInsertAsync<TEntity>(TEntity[] entities) where TEntity : AttrEntityBase, new()
        {
            int result = 0;
            if (entities?.Count() > 0)
            {
                string sql = entities.BatchInsertEntity();
                await this.TryCatch(async () =>
                {
                    result += await _sqlExecutor.ExecuteNonQueryAsync(sql.ToString());
                    return sql.ToString();
                });
            }
            return result;
        }
        #endregion        

        #region Update Operate
        /// <summary>
        /// 直接更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        internal async virtual Task<int> UpdateField<T>(T entity, string PrimaryKey = "", bool IgnorIntDefault = true)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.UpdateField(PrimaryKey);
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 根据Dto特性配置为更新条件,更新有值的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        internal async virtual Task<int> UpdateHasValueFieldByDto<TDo, TEntity>(TDo dto, TEntity entity, bool IgnorIntDefault = true)
           where TDo : AttrBaseModel
           where TEntity : AttrEntityBase
        {
            string sql = dto.UpdateFieldByEntityCondition(entity, IgnorIntDefault);
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("Please refine the information that you need to edit ");
            }
            await this.TryCatch(async () =>
            {
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 更新有值的字段,更新条件为字段中第一个包含ID的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="Primary">主键字段，不指定则默认实体第一个带有Id的字段为主键</param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        internal async virtual Task<int> UpdateHasValueField<T>(T entity, string Primary = "", bool IgnorIntDefault = true)
            where T : AttrEntityBase
        {
            string sql = entity.GetUpdateField(Primary, IgnorIntDefault);
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("Please refine the information that you need to edit ");
            }
            await this.TryCatch(async () =>
            {
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 根据Dto模型字段特性更新指定表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        public async virtual Task<int> UpdateDtoAttributeField<T>(T dto, bool IgnorIntDefault = true)
            where T : AttrBaseModel
        {
            string sql = dto.UpdateFieldByDtoAttribute<T>(IgnorIntDefault);
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("Please refine the information that you need to edit ");
            }
            await this.TryCatch(async () =>
            {
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 执行指定的更新语句
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal async virtual Task<int> ExecUpdateSql<T>(T entity, string sql)
            where T : AttrEntityBase
        {
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("Please refine the information that you need to edit");
            }
            await this.TryCatch(async () =>
            {
                if (entity != null)
                {
                    result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                }
                else
                    result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        #endregion

        #region Delete Operate
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal async virtual Task<int> Delete<T>(T entity, string primaryKey = null)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.DeleteByKey(primaryKey);
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 根据指定的字段删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal async virtual Task<int> Delete<T>(T entity, string[] condition)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.DeleteByCondition(condition);
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 软删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal async virtual Task<int> SoftDelete<T>(T entity, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.SoftDeleteByCondition(softDeleteField, value, PrimaryKey, IngnorIntDefault);
                result = await _sqlExecutor.ExecuteNonQueryAsync(sql);
                return sql;
            });
            return result;
        }
        #endregion        
    }
}
