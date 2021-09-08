using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeSqlDLL.Core.SqlAttribute.JoinTable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RightTableAttribute : Attribute
    {
        private string RightTableName;
        private string byName;
        private string mainTableField;
        private string joinField;
        private string mainTableName;
        private string[] otherJoinsWhere;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_RightTableName">右连接表名</param>
        /// <param name="_mainTableField">右表连接字段</param>
        /// <param name="_joinField">右连接表字段</param>
        /// <param name="_byName">右连接表别名</param>
        /// <param name="_mainTableName">连接主表别名(默认MainTable特性标记的为主表,也可以自己指定)</param>
        /// <param name="_otherJoinsWhere">其他连接条件，表别名需指定：A.a = B.a </param>
        public RightTableAttribute(string _RightTableName, string _mainTableField, string _joinField, string _byName = "", string _mainTableName = "", string[] _otherJoinsWhere = null)
        {
            RightTableName = _RightTableName;
            byName = _byName;
            mainTableField = _mainTableField;
            joinField = _joinField;
            mainTableName = _mainTableName;
            otherJoinsWhere = _otherJoinsWhere;
        }
        public string GetRightTableName()
        {
            return RightTableName;
        }
        public string GetRightTableByName()
        {
            return byName;
        }
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
        /// <summary>
        /// 返回其他连接条件
        /// </summary>
        /// <returns></returns>
        public string[] GetOtherJoinsWhere()
        {
            return otherJoinsWhere;
        }
    }
}
