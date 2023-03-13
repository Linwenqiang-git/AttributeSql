using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Data
{
    public interface IAttrSqlWithAbpDataFilter
    {
        Guid? GetCurrentTenantId();
        bool? IsFilterDelete();
        /// <summary>
        /// 启用单个数据过滤项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDisposable Enable<T>();
        /// <summary>
        /// 禁用单个数据过滤项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDisposable Disable<T>();
        /// <summary>
        /// 启用全部数据过滤项
        /// </summary>
        /// <returns></returns>
        IDisposable Enable();
        /// <summary>
        /// 禁用全部数据过滤项
        /// </summary>
        /// <returns></returns>
        IDisposable Disable();
    }
}
