using AttributeSql.Base.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Models.AdvancedSearchModels
{
    public class AdvObject : IAdvancedQueryBaseField<object>
    {
        public RelationEume Relation { get; set; }
        public OperatorEnum Operator { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}
