using AttributeSql.Base.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Models.AdvancedSearchModels
{
    public sealed class AdvString : AdvObject,IAdvancedQueryBaseField<string>
    {
        public RelationEume Relation { get; set; }
        public OperatorEnum Operator { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
