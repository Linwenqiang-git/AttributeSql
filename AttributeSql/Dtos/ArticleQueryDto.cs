using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

using System;
using System.Collections.Generic;

namespace AttributeSql.Demo.Dtos
{
    [MainTable("cm_article")]
    public class ArticleResult : AttrBaseResult
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [DbFieldName("ArticleTitle")]
        public string DtoArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        [DbFieldName("IsPublish")]
        public bool DtoIsPublish { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        [DbFieldName("Content")]
        public string DtoContent { get; set; }
        /// <summary>
        /// 是否回收
        /// </summary>
        [DbFieldName("IsRecycle")]
        public bool DtoIsRecycle { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [DbFieldName("PublishTime")]
        public DateTime? DtoPublishTime { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [DbFieldName("PublishMan")]
        public string? DtoPublishMan { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DbFieldName("Sort")]
        public int DtoSort { get; set; }        
    }
    public class ArticlePageSearch : AttrPageSearch
    {
        /// <summary>
        /// 文章标题集合查询
        /// </summary>
        [DbFieldName("ArticleTitle")]
        [OperationCode(OperatorEnum.In)]
        public List<string> ArticleTitles { get; set; }
        /// <summary>
        /// 文章标题-高级查询
        /// </summary>
        [DbFieldName("ArticleTitle")]
        public AdvString ArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        [OperationCode(OperatorEnum.Equal)]
        public bool IsPublish { get; set; }
    }
}
