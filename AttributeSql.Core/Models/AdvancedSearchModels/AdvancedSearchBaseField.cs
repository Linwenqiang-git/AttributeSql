using AttributeSql.Core.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Models.AdvancedSearchModels
{
    public class AdvancedSearchBaseField
    {
        /// <summary>
        /// 高级查询表达式关系
        /// </summary>
        public OperatorEums Relation { get; set; }
        /// <summary>
        /// 是否包含多个值
        /// </summary>
        public bool IsContainMultipleValues { get; set; } = false;
    }
}
