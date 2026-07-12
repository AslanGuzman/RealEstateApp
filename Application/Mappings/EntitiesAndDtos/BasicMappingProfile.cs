using AutoMapper;
using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class BasicMappingProfile : Profile
    {
        public BasicMappingProfile()
        {
            CreateMap(typeof(BasicDto<>), typeof(BasicEntity<>)).ReverseMap();
        }
    }
}
