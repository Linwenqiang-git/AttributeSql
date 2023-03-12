using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    /// <summary>
    /// 主表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MainTableAttribute : Attribute
    {
        private string _mainTableName;
        private string _byName;
        public MainTableAttribute(string mainTableName, string byName = "")
        {
            _mainTableName = mainTableName;
            _byName = byName;
        }
        /// <summary>
        /// 获取主表名称
        /// </summary>
        /// <returns></returns>
        public string GetMainTableName()
        {
            return _mainTableName;
        }
        /// <summary>
        /// 获取主表别名
        /// </summary>
        /// <returns></returns>
        public string GetMainTableByName()
        {
            return _byName;
        }
    }
}
