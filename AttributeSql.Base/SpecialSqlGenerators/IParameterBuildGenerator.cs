using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.SpecialSqlGenerators
{
    public interface IParameterBuildGenerator
    {
        DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class;
    }
}
