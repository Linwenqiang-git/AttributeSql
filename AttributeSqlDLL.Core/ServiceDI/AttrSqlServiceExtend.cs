using System;

using AttributeSqlDLL.Core.IService;
using AttributeSqlDLL.Core.Repository;
using AttributeSqlDLL.Core.Service;
using AttributeSqlDLL.Core.ServiceDI;
using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

namespace AttributeSqlDLL.Core.ServiceDI
{
    public static class AttrSqlServiceExtend
    {
        /// <summary>
        /// 添加特性sql服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbConnString">数据库连接字符串</param>
        public static IServiceCollection AddAttributeSqlService(this IServiceCollection services, Action<DbOption> dbOption, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            //模型转换成实体需要AutuMapper做映射
            services.AddAutoMapper(typeof(AttrSqlServiceExtend));

            //仓储层注入   
            var option = new DbOption();
            dbOption(option);
            services.AddDbConnection(option);
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
