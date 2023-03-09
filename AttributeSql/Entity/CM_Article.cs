using AttributeSql.Core.Models;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Domain.Entities;

namespace UFX.SCM.Cloud.CmsCenter.Domain.AggregatesModel.ContentManage
{
    /// <summary>
    /// 文章
    /// </summary>
    public class CM_Article : AttrBaseResult
    {
        #region 构造函数
        public CM_Article()
        {
            IsRecycle = false;
            IsPublish = false;
            Content = string.Empty;
            PublishTime = null;
            PublishMan = null;            
        }
        #endregion

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

        #region 行为
        /// <summary>
        /// 发布文章
        /// </summary>
        public void PublishArtical(string publishMan)
        {
            this.IsPublish = true;
            this.PublishTime = DateTime.Now;
            this.PublishMan = publishMan;
        }
        /// <summary>
        /// 撤销文章
        /// </summary>
        public void RescindArtical()
        {
            this.IsPublish = false;
            this.PublishTime = null;
            this.PublishMan = null;
        }
        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="sort"></param>
        public void SetSort(int? sort)
        {
            if (sort == null)
            {
                return;
            }
            this.Sort = (int)sort;
        }
        #endregion
    }
}
