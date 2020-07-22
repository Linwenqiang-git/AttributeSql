using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InnerTableAttribute : Attribute
    {
        private string InnerTableName;
        private string byName;
        private string mainTableField;
        private string joinField;
        private string mainTableName;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_InnerTableName">内连接表名</param主
        /// <param name="_mainTableField">主表连接字段</param>
        /// <param name="_joinField">内连接表字段</param>
        /// <param name="_byName">内连接表别名</param>
        /// <param name="_mainTableName">内连接住表别名</param>
        public InnerTableAttribute(string _InnerTableName, string _mainTableField, string _joinField, string _byName = "", string _mainTableName = "")
        {
            InnerTableName = _InnerTableName;
            byName = _byName;
            mainTableField = _mainTableField;
            joinField = _joinField;
            mainTableName = _mainTableName;
        }
        public string GetInnerTableName()
        {
            if (string.IsNullOrEmpty(InnerTableName))
                throw new Exception("内连接表不能为空，请检查Dto特性配置");
            return InnerTableName;
        }
        public string GetInnerTableByName()
        {
            return byName;
        }
        /// <summary>
        /// 获取内表连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectField()
        {
            if (string.IsNullOrEmpty(mainTableField) || string.IsNullOrEmpty(joinField))
                throw new Exception("表连接字段不能为空，请检查Dto特性配置");
            StringBuilder join = new StringBuilder();
            if (!string.IsNullOrEmpty(byName))
            {
                join.Append($"{byName}.{joinField}");
            }
            else
            {
                join.Append($"{joinField}");
            }
            join.Append("=");
            return join.ToString();
        }
        /// <summary>
        /// 获取主表连接字段
        /// </summary>
        /// <returns></returns>
        public string GetMainTableField()
        {
            if (string.IsNullOrEmpty(mainTableField))
                throw new Exception("主表连接字段不能为空，请检查Dto特性配置");
            return mainTableField;
        }
        public string GetMainTableByName()
        {
            return mainTableName;
        }
    }
}
