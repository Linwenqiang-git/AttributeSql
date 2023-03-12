using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Base.PersonalizedSqls;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.SpecialSqlGenerators
{
    public abstract class ASpecialSqlGenerator : IPaginationGenerator, IParameterBuildGenerator, IRelationBuildGenerator
    {
        public abstract StringBuilder AdvanceQueryRelationBuild([NotNull] AdvObject advObject, [NotNull] PropertyInfo propertyInfo, string tableField);        

        public abstract DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class;

        public abstract StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option);       

        public abstract string PaginationSql(int? Offset, int? Size);
    }
}
