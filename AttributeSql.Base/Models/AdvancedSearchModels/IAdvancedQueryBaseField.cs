using AttributeSql.Base.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Models.AdvancedSearchModels
{
    public interface IAdvancedQueryBaseField<T>
    {
        /// <summary>
        /// 高级查询表达式关系
        /// </summary>
        public RelationEume Relation { get; set; }
        /// <summary>
        /// 操作符，Values多个值时该字段无效，只能使用In或者Not In        
        /// </summary>
        public OperatorEnum Operator { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public IEnumerable<T> Values { get; set; }
    }
}
