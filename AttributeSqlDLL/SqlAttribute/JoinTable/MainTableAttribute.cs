using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.SqlAttribute.JoinTable
{
    /// <summary>
    /// 主表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MainTableAttribute : Attribute
    {
        private string mainTableName;
        private string byName;
        public MainTableAttribute(string _mainTableName, string _byName = "")
        {
            mainTableName = _mainTableName;
            byName = _byName;
        }
        /// <summary>
        /// 获取主表名称
        /// </summary>
        /// <returns></returns>
        public string GetMainTableName()
        {
            return mainTableName;
        }
        /// <summary>
        /// 获取主表别名
        /// </summary>
        /// <returns></returns>
        public string GetMainTableByName()
        {
            return byName;
        }
    }
}
