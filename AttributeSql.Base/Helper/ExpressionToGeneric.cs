using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AttributeSql.Base.Helper
{
    /// <summary>
    /// 使用表达式目录树+泛型缓存 将datareader转换成对象
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public class ExpressionToGeneric<TOut> where TOut : class
    {
        /// <summary>
        /// 存储委托
        /// </summary>
        private static Func<DbDataReader, TOut> func;

        /// <summary>
        /// 获取对象 信息
        /// </summary>
        /// <param name="dbDataReader"></param>
        /// <returns></returns>
        public static TOut ToClass(DbDataReader dbDataReader)
        {
            if (dbDataReader == null || dbDataReader.HasRows == false)
                return default;
            if (func != null)
                return func(dbDataReader);

            //定义一个输入变量
            var parameter = Expression.Parameter(typeof(DbDataReader), "dbDataReader");
            //创建属性绑定的集合
            List<MemberBinding> memberBinds = new List<MemberBinding>();

            #region 获取dbreader中的所有的name
            // Enumerable.Range(0, j.FieldCount).Select(a => j.GetName(a)).Contains("Id");
            //调用rang方法
            var rang = Expression.Call(typeof(Enumerable).GetMethod("Range"), new Expression[] {
                        Expression.Constant(0),
                       Expression.Property(parameter,"FieldCount")
                    });
            //定义一个下标 参数
            var i = Expression.Parameter(typeof(int), "i");

            //执行一个lambda表达式
            var lambdaGetName = Expression.Lambda<Func<int, string>>(
                //调用DbDataReader 的GetName 方法获取字段值
                Expression.Call(parameter, typeof(DbDataReader).GetMethod("GetName"), new Expression[]{
                                i
                              }),
                new ParameterExpression[] {
                           i
                });

            //调用select 方法 返回集合
            var select = Expression.Call(typeof(Enumerable), "Select", new Type[] {
                        typeof(int),
                        typeof(string)
                    }, new Expression[] {
                        rang,
                        lambdaGetName
                    });

            #endregion

            //遍历要返回的对象的属性信息
            foreach (var item in typeof(TOut).GetProperties())
            {
                //验证属性是否为对象或者泛型，因为此类型，不属于数据库类型，所以过滤掉
                if (!CheckType(item.PropertyType) && (item.PropertyType.IsAutoClass || item.PropertyType.IsGenericType))
                {
                    continue;
                }
                //调用Contains 方法
                var contains = Expression.Call(typeof(Enumerable), "Contains", new Type[] { typeof(string) }, new Expression[] {
                        select,
                        Expression.Constant(item.Name)
                    });
                //获取数据库返回值的类型
                var getDbType = Expression.Call(Expression.Call(typeof(DataReaderExtensions).GetMethod("GetValue"), new Expression[] {
                    parameter,
                Expression.Constant(item.Name)
                }), typeof(object).GetMethod("GetType"));

                //验证当前值 比较是否为dbnull 如果是dbnull的话，就取默认值
                var isDBNull = Expression.Condition(Expression.Equal(getDbType, Expression.Constant(typeof(DBNull))),
                           Expression.Default(item.PropertyType),
                        //当为true的时候
                        Expression.Convert(Expression.Call(typeof(DataReaderExtensions).GetMethod("GetValue"), new Expression[] {
                    parameter,
                Expression.Constant(item.Name)
                    }), item.PropertyType)
                        );

                //绑定属性
                var memberBind = Expression.Bind(item,
                     //条件表达式
                     Expression.Condition(
                      //匹配条件 验证当前输出的对象中的和dbreader中的对象是否满足一样的
                      contains,
                      isDBNull,
                     //当为false的时候
                     Expression.Default(item.PropertyType)
                     ));
                memberBinds.Add(memberBind);
            }
            //初始化对象信息
            var memberInit = Expression.MemberInit(Expression.New(typeof(TOut)), memberBinds);
            //转换成lambda
            var lambda = Expression.Lambda<Func<DbDataReader, TOut>>(memberInit, new ParameterExpression[] {
                parameter
            });
            func = lambda.Compile();
            return func(dbDataReader);
        }
        /// <summary>
        /// 校检类型 返回对应的表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CheckType(Type type)
        {
            if (type == typeof(int) || type == typeof(int?))
                return true;
            else if (type == typeof(string))
                return true;
            else if (type == typeof(long) || type == typeof(long?))
                return true;
            else if (type == typeof(float) || type == typeof(float?))
                return true;
            else if (type == typeof(double) || type == typeof(double?))
                return true;
            else if (type == typeof(decimal) || type == typeof(decimal?))
                return true;
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                return true;
            else if (type == typeof(bool) || type == typeof(bool?))
                return true;
            else if (type == typeof(short) || type == typeof(short?))
                return true;
            else if (type == typeof(byte) || type == typeof(byte?))
                return true;
            else if (type == typeof(byte[]))
                return true;
            else if (type == typeof(uint) || type == typeof(uint?))
                return true;
            else if (type == typeof(ulong) || type == typeof(ulong?))
                return true;
            else if (type == typeof(char) || type == typeof(char?))
                return true;
            else if (type == typeof(Guid) || type == typeof(Guid?))
                return true;
            else if (type == typeof(object))
                return true;
            else
                return false;
        }
    }
}
