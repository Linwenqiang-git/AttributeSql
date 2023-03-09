using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Models.AdvancedSearchModels
{
    public class IntField : AdvancedSearchBaseField
    {
        public int Value { get; set; }
        public List<int> Values { get; set; }
    }
}
