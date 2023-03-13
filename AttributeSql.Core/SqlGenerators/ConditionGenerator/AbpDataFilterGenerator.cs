using AttributeSql.Core.Models;
using AttributeSql.Core.SqlAttribute.JoinTable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Core.SqlGenerators.ConditionGenerator
{
    internal class AbpDataFilterGenerator
    {
        private string _isdeletedFieldName;
        private string _tenantidFieldName;
        public AbpDataFilterGenerator()
        {
            _isdeletedFieldName = "isdeleted";
            _tenantidFieldName = "tenantid";
        }
        public string Generate<TResultDto>(string whereSql) where TResultDto : AttrBaseResult, new()
        {            
            if (whereSql.Contains(_isdeletedFieldName) || whereSql.Contains(_tenantidFieldName))
            {
                object[] allTableObj = typeof(TResultDto).GetCustomAttributes(true);
                List<Type> joinTableTypes = new List<Type>()
                {
                    typeof(LeftTableAttribute),
                    typeof(RightTableAttribute),
                    typeof(InnerTableAttribute),
                    typeof(SublistAttribute),
                };
                if (allTableObj.Any(t => joinTableTypes.Contains(t.GetType())))
                {
                    var mainTable = typeof(TResultDto).GetCustomAttributes(typeof(MainTableAttribute), true).FirstOrDefault();
                    if (mainTable != null)
                    {
                        var mainTableByName = (mainTable as MainTableAttribute).GetMainTableByName();
                        var mainTableName = (mainTable as MainTableAttribute).GetMainTableByName();
                        string mainTablePrefix = mainTableByName ?? mainTableName;
                        //添加前缀，防止二义性
                        whereSql = whereSql.Replace(_tenantidFieldName,$"{mainTablePrefix}.{_tenantidFieldName}")
                                           .Replace(_isdeletedFieldName,$"{mainTablePrefix}.{_isdeletedFieldName}");
                    }                                        
                }    
            }
            return whereSql;
        }
    }
}
