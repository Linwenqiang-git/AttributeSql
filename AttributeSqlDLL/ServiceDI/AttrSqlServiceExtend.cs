using System;
using System.Collections.Generic;
using System.Text;
using AttributeSqlDLL.IService;
using AttributeSqlDLL.Repository;
using AttributeSqlDLL.Service;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace AttributeSqlDLL.ServiceDI
{
    public static class AttrSqlServiceExtend
    {
        /// <summary>
        /// 添加特性sql服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbConnString">数据库连接字符串</param>
        public static IServiceCollection AddAttributeSqlService(this IServiceCollection services, string dbConnString, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            //模型转换成实体需要AutuMapper做映射
            services.AddAutoMapper(typeof(AttrSqlServiceExtend));
            //暂时只支持mysql,后续在扩展支持其他数据库
            //仓储层注入           
            services.AddDbConnection<MySqlConnection>(option => { return option.UseMysql(dbConnString); });
            services.AddTransient<AttrBaseRepository>();
            //对外服务层注入        
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IAttrSqlClient, AttrSqlClient>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<IAttrSqlClient, AttrSqlClient>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<IAttrSqlClient, AttrSqlClient>();
                    break;
                default:
                    break;
            }
            return services;
        }
    }
}
