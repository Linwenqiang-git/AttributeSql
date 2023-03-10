using AttributeSql.Base.Enums;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.PersonalizedSqls
{
    public interface IRelationBuildExtend
    {
        StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option);
        StringBuilder AdvanceQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option);
    }
}
