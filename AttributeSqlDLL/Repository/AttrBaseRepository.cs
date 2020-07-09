using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttributeSqlDLL.ExceptionExtension;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.Repository.DbContextExtensions;
using AttributeSqlDLL.SqlExtendedMethod;
using AttributeSqlDLL.SqlExtendedMethod.CudExtend;
using AttrSqlDbLite.SqlExtendedMethod;

namespace AttributeSqlDLL.Repository
{
    public class AttrBaseRepository
    {
        private readonly DbConnection Context;
        private DbTransaction Tran = null;
        public AttrBaseRepository(DbConnection conn)
        {
            Context = conn;
        }
        #region 内部使用
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
                    throw new AttrSqlException($"Sql语句有误:[{ex.Message}]\n" +
                        $"动态sql：[{sql}]");
                }
                throw new AttrSqlException($"Dto模型配置有误:[{ex.Message}]");
            }
            catch (Exception ex)
            {
                HasError = true;
                throw new Exception(ex.Message);
            }
            return HasError;
        }
        #endregion

        #region Debug sql
        public string DebugQuerySql<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                bool IngnorIntDefault = true,
                                                Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new()
        {
            TResultDto dto = new TResultDto();
            var page = new AttrPageResult<TResultDto>(pageSearch.Index, pageSearch.Size);
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
                    Limit = $"LIMIT {pageSearch.Offset},{pageSearch.Size}";
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

        #region 查询操作
        /// <summary>
        /// 通用单表查询，按照模型指定的where条件，查询指定单表的全部数据
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="tableName"></param>
        /// <param name="IngnorIntDefault">int类型的默认值是否忽略,默认忽略</param>
        /// <returns></returns>
        public async Task<AttrPageResult<TEntity>> GetAll<TPageSearch, TEntity>(TPageSearch pageSearch, string tableName = "", bool IngnorIntDefault = true)
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
                page.Rows = await Context.SqlQuery<TEntity, TPageSearch>($"select * from {tableName} {where} {sort}", pageSearch, Tran);
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
        /// <returns></returns>
        public async Task<AttrPageResult<TResultDto>> GetSpecifyResultDto<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                    bool IngnorIntDefault = true,
                                                    Func<string> whereSql = null)
            where TResultDto : AttrBaseResult, new()
            where TPageSearch : AttrPageSearch
        {
            TResultDto dto = new TResultDto();
            var page = new AttrPageResult<TResultDto>(pageSearch.Index, pageSearch.Size);
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
                    Limit = $"LIMIT {pageSearch.Offset},{pageSearch.Size}";
            }
            StringBuilder sql = new StringBuilder();
            sql.Append(select);
            sql.Append(join);
            sql.Append(where);
            sql.Append(groupByHaving);
            sql.Append(sort);
            sql.Append(Limit);
            await TryCatch(async () =>
            {
                page.Rows = await Context.SqlQuery<TResultDto, TPageSearch>($"{sql}", pageSearch, Tran);
                //如果有分页，统计当前查询共有多少条数据
                if (!string.IsNullOrEmpty(Limit))
                {
                    string Countsql = $"SELECT COUNT(1) as rownum {join} {where} {groupByHaving}";
                    try
                    {
                        page.Total = await Context.SqlCountQuery(Countsql, pageSearch);
                    }
                    catch (AttrSqlException ex)
                    {
                        if (ex.Message.ToLower().Contains($"unknown column"))
                        {
                            //去掉limit
                            Countsql = $"{select} {join} {where} {groupByHaving}";
                            page.Total = await Context.SqlRowsQuery(Countsql, pageSearch);
                        }
                    }
                }
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
        public async Task<AttrPageResult<TResultDto>> ExecQuerySql<TResultDto>(string sql)
           where TResultDto : AttrBaseResult, new()
        {
            var page = new AttrPageResult<TResultDto>();
            await this.TryCatch(async () =>
            {
                page.Rows = await Context.SqlQuery<TResultDto, AttrEntityBase>($"{sql}", null, Tran);
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
        public async Task<IEnumerable<TResult>> ExecQuerySqlInside<TResult>(string sql)
           where TResult : AttrBaseResult, new()
        {
            IEnumerable<TResult> result = null;
            await this.TryCatch(async () =>
            {
                result = await Context.SqlQuery<TResult, AttrEntityBase>($"{sql}", null, Tran);
                return sql;
            });
            return result;
        }

        #endregion

        #region 数据库动作执行前的一些辅助校验
        /// <summary>
        /// 判断指定字段是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="IsAddOrEdit">0新增，1编辑</param>
        /// <returns></returns>
        public async Task<bool> CheckFieldRepeat<TBaseModel>(TBaseModel model, int IsAddOrEdit)
            where TBaseModel : AttrBaseModel
        {
            List<string> sqls = model.NotAllowRepeatSql();
            if (sqls.Count == 0)
                return true;
            int nums = 1;
            foreach (var sql in sqls)
            {
                int resultnum = await Context.SqlRowsQuery(sql, model, Tran);
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
                        string keysql = model.NotAllowKeySql(nums);
                        var result = await Context.SqlQuery<dynamic, TBaseModel>(keysql, model, Tran);
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

        #region 新增操作
        /// <summary>
        /// 新增返回受影响行数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async virtual Task<int> Insert<T>(T entity)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.InsertEntity();
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
                return sql;
            });
            return result;
        }
        /// <summary>
        /// 新增返回自增主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async virtual Task<long> InsertReturnKey<T>(T entity)
            where T : AttrEntityBase
        {
            long newIncreaseKet = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.InsertEntity();
                newIncreaseKet = await Context.ExecuteNonQueryByKey(sql, entity, Tran);
                return sql;
            });
            return newIncreaseKet;
        }
        /// <summary>
        /// 批量新增(参数化)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> BatchInsert<TEntity>(TEntity[] entities) where TEntity : AttrEntityBase, new()
        {
            int result = 0;
            if (entities?.Count() > 0)
            {
                string sql = entities[0].InsertEntity();
                await this.TryCatch(async () =>
                {
                    //默认事务新增
                    if (Tran == null)
                    {
                        await TransactionRun(async () =>
                        {
                            foreach (var item in entities)
                            {
                                result += await Context.ExecuteNonQuery(sql.ToString(), item, Tran);
                            }
                            if (result == entities.Length)
                                return AttrResultModel.Success();
                            else
                                return AttrResultModel.Error("批量新增失败！");
                        });
                    }
                    else
                        foreach (var item in entities)
                        {
                            result += await Context.ExecuteNonQuery(sql.ToString(), item, Tran);
                        }
                    return sql.ToString();
                });
            }
            return result;
        }
        /// <summary>
        /// 批量新增（非参数化,建议在大批量插入数据时使用）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> NonPatameterBatchInsert<TEntity>(TEntity[] entities) where TEntity : AttrEntityBase, new()
        {
            int result = 0;
            if (entities?.Count() > 0)
            {
                string sql = entities.BatchInsertEntity();
                await this.TryCatch(async () =>
                {
                    result += await Context.ExecuteNonQuery<AttrEntityBase>(sql.ToString(), null, Tran);
                    return sql.ToString();
                });
            }
            return result;
        }
        #endregion        

        #region 更新操作
        /// <summary>
        /// 直接更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        public async virtual Task<int> UpdateField<T>(T entity, string PrimaryKey = "", bool IgnorIntDefault = true)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.UpdateField(PrimaryKey);
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
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
        public async virtual Task<int> UpdateHasValueFieldByDto<TDo, TEntity>(TDo dto, TEntity entity, bool IgnorIntDefault = true)
           where TDo : AttrBaseModel
           where TEntity : AttrEntityBase
        {
            string sql = dto.UpdateFieldByEntityCondition(entity, IgnorIntDefault);
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("请完善需要编辑的信息！");
            }
            await this.TryCatch(async () =>
            {
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
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
        public async virtual Task<int> UpdateHasValueField<T>(T entity, string Primary = "", bool IgnorIntDefault = true)
            where T : AttrEntityBase
        {
            string sql = entity.GetUpdateField(Primary, IgnorIntDefault);
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("请完善需要编辑的信息！");
            }
            await this.TryCatch(async () =>
            {
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
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
        public async virtual Task<int> ExecUpdateSql<T>(T entity, string sql)
            where T : AttrEntityBase
        {
            int result = 0;
            if (string.IsNullOrEmpty(sql))
            {
                throw new AttrSqlException("请完善需要编辑的信息！");
            }
            await this.TryCatch(async () =>
            {
                if (entity != null)
                {
                    result = await Context.ExecuteNonQuery(sql, entity, Tran);
                }
                else
                    result = await Context.ExecuteNonQuery(sql, Tran);
                return sql;
            });
            return result;
        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async virtual Task<int> Delete<T>(T entity, string primaryKey = null)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.DeleteByKey(primaryKey);
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
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
        public async virtual Task<int> Delete<T>(T entity, string[] condition)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.DeleteByCondition(condition);
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
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
        public async virtual Task<int> SoftDelete<T>(T entity, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true)
            where T : AttrEntityBase
        {
            int result = 0;
            await this.TryCatch(async () =>
            {
                string sql = entity.SoftDeleteByCondition(softDeleteField, value, PrimaryKey, IngnorIntDefault);
                result = await Context.ExecuteNonQuery(sql, entity, Tran);
                return sql;
            });
            return result;
        }
        #endregion

        #region 针对某些直接操作较多的数据库访问

        #endregion

        #region 事务相关操作
        public async Task<AttrResultModel> TransactionRun(Func<Task<AttrResultModel>> func)
        {
            AttrResultModel result = AttrResultModel.Success();
            if (Context.State != ConnectionState.Open)
            {
                Context.Open();
            }
            using (Context)
            {
                using (Tran = Context.BeginTransaction())
                {
                    try
                    {
                        result = await func.Invoke();
                        if (result.Code != 0)
                        {
                            Tran.Rollback();
                            Context.Close();
                            throw new Exception($"{result.Msg}");
                        }
                        else
                            Tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (Context.State == ConnectionState.Open)
                            Tran.Rollback();
                        result.Code = ResultCode.UnknownError;
                        result.Msg = ex.Message;
                    }
                }
                return result;
            }
        }
        #endregion

    }
}
