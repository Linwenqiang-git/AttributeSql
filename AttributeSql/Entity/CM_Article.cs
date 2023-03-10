using AttributeSql.Base.Enums;
using AttributeSql.Core.Extensions;
using AttributeSql.Core.Models;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Core.SqlAttribute.JoinTable;
using AttributeSql.Core.SqlAttribute.Select;
using AttributeSql.Core.SqlAttribute.Where;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Domain.Entities;

namespace UFX.SCM.Cloud.CmsCenter.Domain.AggregatesModel.ContentManage
{
    [MainTable("cm_article")]
    public class CM_Article : AttrBaseResult
    {

        #region 属性        
        /// <summary>
        /// 文章标题
        /// </summary>
        public string ArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublish { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否回收
        /// </summary>
        public bool IsRecycle { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string? PublishMan { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual Guid? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }

        public string CreatorName { get; set; }

        public string DeleterName { get; set; }

        public string LastModifierName { get; set; }

        public virtual DateTime TrackTime { get; set; }

        public virtual DateTime? SyncDateTime { get; set; }
        #endregion        
    }
    public class CM_Article_Search : AttrPageSearch
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [DbFieldName("ArticleTitle")]
        public StringField ArticleTitle { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        [OperationCode(OperatorEnum.Equal)]
        public bool IsPublish { get; set; }
    }    
}
