using AttributeSql.Base.Enums;
using AttributeSql.Base.Models.AdvancedSearchModels;
using AttributeSql.Base.SpecialSqlGenerators;

using Oracle.ManagedDataAccess.Client;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Oracle.SpecialSqlGenerator
{
    public class OracleSqlGenerator : ASpecialSqlGenerator
    {
        #region BuildParameter
        public override DbParameter[] BuildParameter<TModel>(TModel model) where TModel : class
        {
            if (model == null)
                return default(DbParameter[]);
            int length = model.GetType().GetProperties().Length;
            OracleParameter[] param = new OracleParameter[length];
            int cursor = 0;
            foreach (var item in model.GetType().GetProperties())
            {
                param[cursor] = new OracleParameter();
                param[cursor].ParameterName = $"@{item.Name}";
                param[cursor].Value = item.GetValue(model, null);
                //参数类型是list的，需要转换成string
                if (param[cursor].Value?.GetType() == typeof(List<int>))
                    param[cursor].Value = ToContainString((List<int>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<byte>))
                    param[cursor].Value = ToContainString((List<byte>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<long>))
                    param[cursor].Value = ToContainString((List<long>)param[cursor].Value);
                else if (param[cursor].Value?.GetType() == typeof(List<string>))
                    param[cursor].Value = ToContainString((List<string>)param[cursor].Value);
                ++cursor;
            }
            return param;
        }
        private string ToContainString<T>(List<T> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.Append($"{item},");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
        #endregion

        

        public override string PaginationSql(int? Offset, int? Size)
        {
            throw new NotImplementedException("Paging is not supported for the time being");
        }

        public override StringBuilder GeneralQueryRelationBuild([NotNull] object obj, [NotNull] PropertyInfo propertyInfo, string tableField, OperatorEnum option)
        {
            throw new NotImplementedException();
        }

        public override StringBuilder AdvanceQueryRelationBuild([NotNull] AdvObject advObject, [NotNull] PropertyInfo propertyInfo, string tableField)
        {
            throw new NotImplementedException();
        }
    }
}
