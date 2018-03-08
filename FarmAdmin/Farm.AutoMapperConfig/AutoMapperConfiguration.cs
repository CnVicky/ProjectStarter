using AutoMapper;
using Farm.Domain;
using Farm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.AutoMapperConfig
{
    /// <summary>
    /// AutoMapper的全局实体映射配置静态类
    /// </summary>
    public static class AutoMapperConfiguration
    {
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                #region DTO
                cfg.CreateMap<DTO, VM>(MemberList.None);
                #endregion
            });

            Mapper = MapperConfiguration.CreateMapper();
        }

        public static IMapper Mapper { get; private set; }

        public static MapperConfiguration MapperConfiguration { get; private set; }
    }
}
