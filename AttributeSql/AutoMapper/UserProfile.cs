using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttributeSql.Entity;
using AttributeSql.Model;
using AutoMapper;

namespace AttributeSql.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //添加实体和dto模型的映射
            CreateMap<CreateOrderDto, R01_Order>();
            CreateMap<UpdateOrderDto, R01_Order>();
        }
    }
}
