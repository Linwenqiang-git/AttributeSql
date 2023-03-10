using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.PersonalizedSqls
{
    public interface IParameterBuildExtend
    {
        DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class;
    }
}
