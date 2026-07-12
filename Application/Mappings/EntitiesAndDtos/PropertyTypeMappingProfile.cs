using AutoMapper;
using RealEstateApp.Core.Application.Dtos.PropertyType;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class PropertyTypeMappingProfile : Profile
    {
        public PropertyTypeMappingProfile()
        {
            CreateMap<Domain.Entities.PropertyType, PropertyTypeDto>()
                .ReverseMap()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());
        }
    }
}
