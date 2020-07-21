using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using AttributeSqlDLL.ExceptionExtension;
using AttributeSqlDLL.IService;
using AttributeSqlDLL.Model;
using AttributeSqlDLL.Repository;
using AutoMapper;

namespace AttributeSqlDLL.Service
{
    public class AttrSqlClient : IAttrSqlClient
    {
        private readonly AttrBaseRepository Repo;
        private readonly IMapper _mapper;
        public AttrSqlClient(AttrBaseRepository repo, IMapper mapper)
        {
            Repo = repo;
            _mapper = mapper;
        }

        #region 异常通用处理方法
        /// <summary>
        /// 异常通用处理方法
        /// </summary>
        /// <param name="func"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IsShowExceptionMsg">前端是否弹出异常捕获的信息,默认不显示</param>
        /// <returns></returns>
        private async Task<AttrResultModel> TryCatch(Func<Task<AttrResultModel>> func, string ErrorMsg, bool IsShowExceptionMsg = false)
        {
            var rm = AttrResultModel.Success();
            try
            {
                rm = await func.Invoke();
            }
            catch (AttrSqlException ex)
            {
                //Dto模型配置问题暴露配置错误原因
                rm.Code = ResultCode.AttributeError;
                rm.Msg = ex.Message;
            }
            catch (Exception ex)
            {
                rm.Code = ResultCode.UnknownError;
                rm.Msg = string.IsNullOrEmpty(ErrorMsg) ? ex.Message : ErrorMsg;
                if (IsShowExceptionMsg)
                    rm.Msg = $"{ErrorMsg}:{ex.Message}:{ex.StackTrace}";
            }
            return rm;
        }
        #endregion

        #region  Debug sql
        /// <summary>
        /// GetSpecifyResultDto sql 生成查看
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <typeparam name="TResultDto"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="IngnorIntDefault"></param>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public string DebugQuerySql<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                bool IngnorIntDefault = true,
                                                Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new()
        {
            return Repo.DebugQuerySql<TResultDto, TPageSearch>(pageSearch, IngnorIntDefault, whereSql);
        }

        #endregion

        #region 查询相关
        /// <summary>
        /// 通用查询，按照模型指定的where条件，查询指定单表的全部数据
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="tableName"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> GetAll<TPageSearch, TEntity>(TPageSearch pageSearch, string tableName, string ErrorMsg = "")
            where TPageSearch : AttrPageSearch
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var result = await Repo.GetAll<TPageSearch, TEntity>(pageSearch, tableName);
                rm.Result = result;
                rm.Total = result.Total;
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 通用查询，按照指定的Dto结果集模型获取多表查询的数据
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <typeparam name="TResultDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> GetSpecifyResultDto<TPageSearch, TResultDto>(TPageSearch pageSearch,
                                                string ErrorMsg = "",
                                                bool IngnorIntDefault = true,
                                                Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var result = await Repo.GetSpecifyResultDto<TResultDto, TPageSearch>(pageSearch, IngnorIntDefault, whereSql);
                rm.Result = result;
                rm.Total = result.Total;
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 执行指定的sql查询语句
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> ExecQuerySql<TResult>(string sql, string ErrorMsg = "")
            where TResult : AttrBaseResult, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var result = await Repo.ExecQuerySql<TResult>(sql);
                rm.Result = result;
                rm.Total = result.Total;
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 执行指定的sql语句内部使用
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TResult>> ExecQuerySqlInside<TResult>(string sql, string ErrorMsg)
            where TResult : AttrBaseResult, new()
        {
            try
            {
                return await Repo.ExecQuerySqlInside<TResult>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ErrorMsg);
            }
        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);
                await Repo.Delete(entity);
                rm.Result = true;
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string[] condition, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);
                rm.Result = await Repo.Delete(entity, condition);
                return rm;
            }, ErrorMsg);
        }
        public async Task<AttrResultModel> DeleteAsync<TEntity>(TEntity entity, string[] condition, string ErrorMsg)
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                rm.Result = await Repo.Delete(entity, condition);
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 软删除
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="softDeleteField"></param>
        /// <param name="value"></param>
        /// <param name="PrimaryKey">主键字段</param>
        /// <param name="IngnorIntDefault"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> SoftDeleteAsync<TDto, TEntity>(TDto DtoModel, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true, string ErrorMsg = "")
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);
                rm.Result = await Repo.SoftDelete(entity, softDeleteField, value, PrimaryKey, IngnorIntDefault);
                return rm;
            }, ErrorMsg);
        }
        public async Task<AttrResultModel> SoftDeleteAsync<TEntity>(TEntity entity,string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true, string ErrorMsg = "")
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                rm.Result = await Repo.SoftDelete(entity, softDeleteField, value, PrimaryKey, IngnorIntDefault);
                return rm;
            }, ErrorMsg);
        }
        #endregion

        #region 新增操作
        /// <summary>
        /// 新增模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg">出错类型</param>
        /// <returns></returns>
        public async Task<AttrResultModel> InsertAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);
                //校验名称是否重复
                if (await Repo.CheckFieldRepeat(DtoModel, 0))
                {
                    // 执行添加
                    var rows = await Repo.Insert(entity);
                    if (rows == 0)
                    {
                        rm.Code = ResultCode.UnknownError;
                        rm.Msg = "创建数据失败!";
                    }
                }
                return rm;
            }, ErrorMsg, true);
        }
        /// <summary>
        /// 新增实体，并返回主键
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> InsertReturnKey<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);
                //校验名称是否重复
                if (await Repo.CheckFieldRepeat(DtoModel, 0))
                {
                    // 执行添加
                    var rows = await Repo.InsertReturnKey(entity);
                    if (rows == 0)
                    {
                        rm.Code = ResultCode.UnknownError;
                        rm.Msg = "创建数据失败!";
                    }
                    rm.Result = rows;
                }
                return rm;
            }, ErrorMsg, true);
        }
        public async Task<AttrResultModel> InsertReturnKey<TEntity>(TEntity Entity)
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var rows = await Repo.InsertReturnKey(Entity);
                if (rows == 0)
                {
                    rm.Code = ResultCode.UnknownError;
                    rm.Msg = "创建数据失败!";
                }
                rm.Result = rows;
                return rm;
            }, "创建实体失败！", true);
        }
        /// <summary>
        /// 直接新增实体，不需要映射
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRepo"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> InsertEntityAsync<TEntity>(TEntity entity)
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var rows = await Repo.Insert(entity);
                if (rows == 0)
                {
                    rm.Code = ResultCode.UnknownError;
                    rm.Msg = "创建数据失败!";
                }
                return rm;
            }, "新增失败！");
        }
        /// <summary>
        /// 批量添加模型
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> BatchInsertAsync<TDto, TEntity>(TDto[] DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                if (DtoModel?.Length <= 0)
                {
                    rm.Code = ResultCode.UnknownError;
                    rm.Msg = "新增实体不能为空";
                }
                else
                {
                    var entity = _mapper.Map<TEntity[]>(DtoModel);
                    // 执行添加
                    rm.Result = await Repo.BatchInsert(entity);
                }

                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 批量添加实体（参数化）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Entities"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> BatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg) where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                if (Entities?.Length <= 0)
                {
                    rm.Code = ResultCode.UnknownError;
                    rm.Msg = "新增实体不能为空";
                }
                else
                {
                    // 执行添加
                    rm.Result = await Repo.BatchInsert(Entities);
                }

                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 批量新增实体（非参数化,建议在大批量插入数据时使用）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Entities"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> NonPatameterBatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg) where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                if (Entities?.Length <= 0)
                {
                    rm.Code = ResultCode.UnknownError;
                    rm.Msg = "新增实体不能为空";
                }
                else
                {
                    // 执行添加
                    rm.Result = await Repo.NonPatameterBatchInsert(Entities);
                }
                return rm;
            }, ErrorMsg);
        }
        #endregion

        #region 更新操作
        /// <summary>
        /// 根据ID主键修改实体有值的部分
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IgnorIntDefault">是否忽略int的默认值0</param>
        /// <param name="UpdateByKey">是否根据主键更新，默认是，否则按Dto模型配置的来更新</param>
        /// <returns></returns>
        public async Task<AttrResultModel> UpdateHasValueFieldAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true, bool UpdateByKey = true)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                var entity = _mapper.Map<TEntity>(DtoModel);

                //校验名称是否重复
                if (await Repo.CheckFieldRepeat(DtoModel, 1))
                {
                    // 执行更新
                    if (UpdateByKey)
                        rm.Result = await Repo.UpdateHasValueField(entity, PrimaryKey, IgnorIntDefault);
                    else
                        rm.Result = await Repo.UpdateHasValueFieldByDto(DtoModel, entity, IgnorIntDefault);
                }
                return rm;
            }, ErrorMsg);
        }
        public async Task<AttrResultModel> UpdateHasValueFieldAsync<TEntity>(TEntity entity, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                rm.Result = await Repo.UpdateHasValueField(entity, PrimaryKey, IgnorIntDefault);
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// AutoMapper映射并更新实体
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> UpdateAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {

                var entity = _mapper.Map<TEntity>(DtoModel);

                //校验名称是否重复                              
                if (await Repo.CheckFieldRepeat(DtoModel, 1))
                {
                    // 执行更新
                    rm.Result = await Repo.UpdateField(entity, PrimaryKey, IgnorIntDefault);
                }
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> UpdateAsync<TEntity>(TEntity entity, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
            where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                // 执行更新
                rm.Result = await Repo.UpdateHasValueField(entity, PrimaryKey, IgnorIntDefault);
                return rm;
            }, ErrorMsg);
        }
        /// <summary>
        /// 执行指定的更新语句
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> ExecUpdateSql<TEntity>(TEntity entity, string sql, string ErrorMsg)
                where TEntity : AttrEntityBase, new()
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () =>
            {
                rm.Result = await Repo.ExecUpdateSql(entity, sql);
                return rm;
            }, ErrorMsg);
        }
        #endregion

        #region 针对某些直接操作较多的数据库访问

        #endregion

        #region 事务相关操作
        /// <summary>
        /// 事务处理
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<AttrResultModel> TransactionRun(Func<Task<AttrResultModel>> func)
        {
            var rm = AttrResultModel.Success();
            return await TryCatch(async () => {
                rm = await Repo.TransactionRun(func);
                return rm;
            }, "事务执行出错");
        }
        public IDbTransaction GetDBTransaction()
        {
            return Repo.GetDBTransaction();
        }
        #endregion

    }
}
