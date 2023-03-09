using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSql.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LeftTableAttribute : Attribute
    {
        private string LeftTableName;
        private string byName;
        private string mainTableField;
        private string joinField;
        private string mainTableName;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_LeftTableName">左连接表名</param>
        /// <param name="_mainTableField">主表连接字段</param>
        /// <param name="_joinField">左连接表字段</param>
        /// <param name="_byName">左连接表别名</param>
        /// <param name="_mainTableName">连接主表别名(默认MainTable特性标记的为主表,也可以自己指定)</param>
        public LeftTableAttribute(string _LeftTableName, string _mainTableField, string _joinField, string _byName = "", string _mainTableName = "")
        {
            LeftTableName = _LeftTableName;
            byName = _byName;
            mainTableField = _mainTableField;
            joinField = _joinField;
            mainTableName = _mainTableName;
        }
        public string GetLeftTableName()
        {
            return LeftTableName;
        }
        public string GetLeftTableByName()
        {
            return byName;
        }
        /// <summary>
        /// 获取左表连接字符串
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
        /// <summary>
        /// 返回连接主表别名
        /// </summary>
        /// <returns></returns>
        public string GetMainTableByName()
        {
            return mainTableName;
        }
    }
}
