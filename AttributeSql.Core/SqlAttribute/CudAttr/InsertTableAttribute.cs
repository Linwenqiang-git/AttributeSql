using System;
using System.Collections.Generic;
using System.Text;
using AttributeSql.Base.Exceptions;

namespace AttributeSql.Core.SqlAttribute.CudAttr
{
    /// <summary>
    /// 表名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InsertTableAttribute : Attribute
    {
        private string _insertTableName;
        public InsertTableAttribute(string insertTableName)
        {
            if (string.IsNullOrEmpty(_insertTableName))
            {
                throw new AttrSqlException("表名不能为空，请检查Dto特性配置!");
            }
            _insertTableName = insertTableName;
        }
        /// <summary>
        /// 获取更新表名称
        /// </summary>
        /// <returns></returns>
        public string GetInsertTableName()
        {
            return _insertTableName;
        }
    }
}
