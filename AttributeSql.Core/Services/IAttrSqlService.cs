using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using AttributeSql.Core.Models;

using Volo.Abp.EntityFrameworkCore;

namespace AttributeSql.Core.Services
{
    public interface IAttrSqlService<TDbContext> where TDbContext : IEfCoreDbContext
    {
        #region Debug sql
        string DebugQuerySql<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                bool IngnorIntDefault = true,
                                                Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new();

        #endregion

        #region 查询相关
        /// <summary>
        /// 查询指定表的所有字段
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="tableName"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> GetAll<TPageSearch, TEntity>(TPageSearch pageSearch, string tableName, string ErrorMsg = "")
            where TPageSearch : AttrPageSearch
            where TEntity : AttrEntityBase, new();

        /// <summary>
        /// 根据Dto模型指定的字段查询
        /// </summary>
        /// <typeparam name="TPageSearch"></typeparam>
        /// <typeparam name="TResultDto"></typeparam>
        /// <param name="pageSearch"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IngnorIntDefault"></param>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        Task<AttrResultModel> GetSpecifyResultDto<TPageSearch, TResultDto>(TPageSearch pageSearch, string ErrorMsg = "", bool IngnorIntDefault = false, Func<string> whereSql = null, bool usingCache = true)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new();
        /// <summary>
        /// 执行指定的sql语句
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> ExecQuerySql<TResult>(string sql, string ErrorMsg = "")
            where TResult : AttrBaseResult, new();
        /// <summary>
        /// 执行指定的sql语句(内部使用)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> ExecQuerySqlInside<TResult>(string sql, string ErrorMsg = "")
            where TResult : AttrBaseResult, new();

        #endregion

        #region  删除操作
        /// <summary>
        /// 根据主键删除数据（默认实体内第一个带ID的字段为主键,且该字段必须有值）
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "")
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 删除数据（需要转换为实体）,根据指定的字段为删除条件
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel">模型内删除条件字段必须有值</param>
        /// <param name="ErrorMsg"></param>
        /// <param name="condition">删除的条件字段</param>
        /// <returns></returns>
        Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string[] condition, string ErrorMsg = "")
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 删除数据（不需要转换为实体）,根据指定的字段为删除条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<AttrResultModel> DeleteAsync<TEntity>(TEntity entity, string[] condition, string ErrorMsg = "")
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 软删除（不需要转换为实体）,需要指定软删除的字段,以及然删除字段的无效值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="softDeleteField">软删除字段名</param>
        /// <param name="value">软删除的无效值</param>
        /// <param name="PrimaryKey">主键</param>
        /// <param name="IngnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> SoftDeleteAsync<TEntity>(TEntity entity, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true, string ErrorMsg = "")
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 软删除（需要转换为实体）,需要指定软删除的字段,以及然删除字段的无效值
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="softDeleteField">软删除字段名</param>
        /// <param name="value">软删除的无效值</param>
        /// <param name="PrimaryKey">主键</param>
        /// <param name="IngnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> SoftDeleteAsync<TDto, TEntity>(TDto DtoModel, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true, string ErrorMsg = "")
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        #endregion

        #region 新增操作
        /// <summary>
        /// 新增实体
        /// 需要配置Dto模型 
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> InsertAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "")
           where TDto : AttrBaseModel
           where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 新增实体
        /// 无需配置Dto模型
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<AttrResultModel> InsertEntityAsync<TEntity>(TEntity entity)
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 高效直接批量新增
        /// 该方式不支持参数化新增,所以一般在一次性新增万级数据时使用
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Entities"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> NonPatameterBatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg = "")
           where TEntity : AttrEntityBase, new();
        #endregion

        #region 更新操作
        /// <summary>
        /// 更新实体
        /// 按照Dto模型中有值的部分就行更新,没有值的部分将忽略,适用于表单提交类更新
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <param name="UpdateByKey"></param>
        /// <returns></returns>
        Task<AttrResultModel> UpdateHasValueFieldAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "", string PrimaryKey = "", bool IgnorIntDefault = true, bool UpdateByKey = true)
           where TDto : AttrBaseModel
           where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> UpdateHasValueFieldAsync<TEntity>(TEntity entity, string ErrorMsg = "", string PrimaryKey = "", bool IgnorIntDefault = true)
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 整个实体字段全部更新（若不指定主键则按照第一个包含ID的字段为更新主键,建议指定）
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="DtoModel"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> UpdateAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg = "", string PrimaryKey = "", bool IgnorIntDefault = true)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 整个实体字段全部更新（若不指定主键则按照第一个包含ID的字段为更新主键,建议指定）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> UpdateAsync<TEntity>(TEntity entity, string ErrorMsg = "", string PrimaryKey = "", bool IgnorIntDefault = true)
                    where TEntity : AttrEntityBase, new();
        /// <summary>
        /// 根据Dto模型字段特性更新指定表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dto"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="IgnorIntDefault"></param>
        /// <returns></returns>
        Task<AttrResultModel> UpdateAsync<TDto>(TDto dto, string ErrorMsg = "", bool IgnorIntDefault = true)
                    where TDto : AttrBaseModel, new();
        /// <summary>
        /// 执行指定的更新语句
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sql"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        Task<AttrResultModel> ExecUpdateSql<TEntity>(TEntity entity, string sql, string ErrorMsg = "")
                where TEntity : AttrEntityBase, new();
        #endregion        
    }
}
