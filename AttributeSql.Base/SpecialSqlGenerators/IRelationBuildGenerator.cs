using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.PersonalizedSqls
{
    public interface IRelationBuildGenerator
    {
        StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option);
        StringBuilder AdvanceQueryRelationBuild([NotNull] AdvObject advObject, [NotNull] PropertyInfo propertyInfo, string tableField);
    }
}
