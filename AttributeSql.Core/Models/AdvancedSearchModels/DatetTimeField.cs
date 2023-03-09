using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.Models.AdvancedSearchModels
{
    public class DateTimeField : AdvancedSearchBaseField
    {
        public DateTime Value { get; set; }
        public List<DateTime> Values { get; set; }
    }
}
