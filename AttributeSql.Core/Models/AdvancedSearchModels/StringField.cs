using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Models.AdvancedSearchModels
{
    public class StringField : AdvancedSearchBaseField
    {
        public string Value { get; set; }
        public List<string> Values { get; set; }
    }
}
