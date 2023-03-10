using AttributeSql.Base.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Models.AdvancedSearchModels
{
    public sealed class StringField : IAdvancedQueryBaseField<string>
    {
        public RelationEume Relation { get; set; }
        public OperatorEnum? Operator { get; set; }
        public List<string> Values { get; set; }
    }
}
