using System;
using System.Collections.Generic;
using System.Text;
using AttributeSql.Base.Exceptions;

namespace AttributeSql.Core.SqlAttribute.CudAttr
{
    /// <summary>
    /// 主表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UpdateTableAttribute : Attribute
    {
        private string updateTableName;
        public UpdateTableAttribute(string _updateTableName)
        {
            if (string.IsNullOrEmpty(_updateTableName))
            {
                throw new AttrSqlException("表名不能为空，请检查Dto特性配置!");
            }
            updateTableName = _updateTableName;
        }
        /// <summary>
        /// 获取更新表名称
        /// </summary>
        /// <returns></returns>
        public string GetUpdateTableName()
        {
            return updateTableName;
        }
    }
}
