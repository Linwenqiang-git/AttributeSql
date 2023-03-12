using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Core.Enums;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.GroupHaving;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;
using AttributeSql.Demo.StaticEntities;

using System;
using System.Collections.Generic;

namespace AttributeSql.Demo.Dtos
{
    [MainTable(CM_ArticleEntity.TableName,"a")]
    [LeftTable(Cm_LikerecordEntity.TableName, CM_ArticleEntity.Id, Cm_LikerecordEntity.Articalid,"b")]
    public class ArticleResult : AttrBaseResult
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [DbFieldName(CM_ArticleEntity.ArticleTitle)]
        [GroupBy("a", CM_ArticleEntity.ArticleTitle)]
        public string DtoArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        [DbFieldName(CM_ArticleEntity.IsPublish)]
        [GroupBy("a", CM_ArticleEntity.IsPublish)]
        public bool DtoIsPublish { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        [DbFieldName(CM_ArticleEntity.Content)]
        [GroupBy("a", CM_ArticleEntity.Content)]
        public string DtoContent { get; set; }
        /// <summary>
        /// 是否回收
        /// </summary>
        [DbFieldName(CM_ArticleEntity.IsRecycle)]
        [GroupBy("a", CM_ArticleEntity.IsRecycle)]
        public bool DtoIsRecycle { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [DbFieldName(CM_ArticleEntity.PublishTime)]
        [GroupBy("a", CM_ArticleEntity.PublishTime)]
        public DateTime? DtoPublishTime { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [DbFieldName(CM_ArticleEntity.PublishMan)]
        [GroupBy("a", CM_ArticleEntity.PublishMan)]
        public string? DtoPublishMan { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DbFieldName(CM_ArticleEntity.Sort)]
        [GroupBy("a", CM_ArticleEntity.Sort)]
        public int DtoSort { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [AggregateFuncField(AggregateFunctionEnum.Count, Cm_LikerecordEntity.Id, "b")]
        public long LikeCount { get; set; }        
    }
    public class ArticlePageSearch : AttrPageSearch
    {
        /// <summary>
        /// 文章标题集合查询
        /// </summary>
        [DbFieldName(CM_ArticleEntity.ArticleTitle)]
        [OperationCode(OperatorEnum.In)]
        public List<string> ArticleTitles { get; set; }
        /// <summary>
        /// 文章标题-高级查询
        /// </summary>
        [DbFieldName(CM_ArticleEntity.ArticleTitle)]
        public AdvString ArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        [DbFieldName(CM_ArticleEntity.IsPublish)]
        [OperationCode(OperatorEnum.Equal)]
        public bool IsPublish { get; set; }
    }
}
