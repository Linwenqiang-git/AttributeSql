using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AttributeSqlDLL.Model;

namespace AttributeSqlDLL.IService
{
    public interface IAttrSqlClient
    {
        #region Debug sql
        string DebugQuerySql<TResultDto, TPageSearch>(TPageSearch pageSearch,
                                                bool IngnorIntDefault = true,
                                                Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new();

        #endregion

        #region 查询相关
        Task<AttrResultModel> GetAll<TPageSearch, TEntity>(TPageSearch pageSearch, string tableName, string ErrorMsg = "")
            where TPageSearch : AttrPageSearch
            where TEntity : AttrEntityBase, new();

        Task<AttrResultModel> GetSpecifyResultDto<TPageSearch, TResultDto>(TPageSearch pageSearch, string ErrorMsg = "", bool IngnorIntDefault = false, Func<string> whereSql = null)
            where TPageSearch : AttrPageSearch
            where TResultDto : AttrBaseResult, new();

        Task<AttrResultModel> ExecQuerySql<TResult>(string sql, string ErrorMsg)
            where TResult : AttrBaseResult, new();

        Task<IEnumerable<TResult>> ExecQuerySqlInside<TResult>(string sql, string ErrorMsg)
            where TResult : AttrBaseResult, new();

        #endregion

        #region  删除操作
        Task<AttrResultModel> Delete<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> DeleteAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string[] condition)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> DeleteAsync<TEntity>(TEntity entity, string ErrorMsg, string[] condition)
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> SoftDeleteAsync<TEntity>(TEntity entity, string ErrorMsg, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true)
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> SoftDeleteAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string softDeleteField, int value = 0, string PrimaryKey = "", bool IngnorIntDefault = true)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        #endregion

        #region 新增操作
        Task<AttrResultModel> InsertAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
           where TDto : AttrBaseModel
           where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> InsertReturnKey<TDto, TEntity>(TDto DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> InsertReturnKey<TEntity>(TEntity Entity)
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> InsertEntityAsync<TEntity>(TEntity entity)
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> BatchInsertAsync<TDto, TEntity>(TDto[] DtoModel, string ErrorMsg)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> BatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg)
           where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> NonPatameterBatchInsertAsync<TEntity>(TEntity[] Entities, string ErrorMsg)
           where TEntity : AttrEntityBase, new();
        #endregion

        #region 更新操作
        Task<AttrResultModel> UpdateHasValueFieldAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true, bool UpdateByKey = true)
           where TDto : AttrBaseModel
           where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> UpdateHasValueFieldAsync<TEntity>(TEntity entity, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> UpdateAsync<TDto, TEntity>(TDto DtoModel, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
            where TDto : AttrBaseModel
            where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> UpdateAsync<TEntity>(TEntity entity, string ErrorMsg, string PrimaryKey = "", bool IgnorIntDefault = true)
                    where TEntity : AttrEntityBase, new();
        Task<AttrResultModel> ExecUpdateSql<TEntity>(TEntity entity, string sql, string ErrorMsg)
                where TEntity : AttrEntityBase, new();
        #endregion

        #region  事务操作
        Task<AttrResultModel> TransactionRun(Func<Task<AttrResultModel>> func);
        #endregion
    }
}
