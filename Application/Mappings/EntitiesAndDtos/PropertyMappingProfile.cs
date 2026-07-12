using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Domain.Entities.Property, PropertyDto>()
                .ForMember(dest => dest.PropertyTypeName, opt => opt.MapFrom(src => src.PropertyType != null ? src.PropertyType.Name : null))
                .ForMember(dest => dest.SaleTypeName, opt => opt.MapFrom(src => src.SaleType != null ? src.SaleType.Name : null))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(i => i.ImagePath).ToList() : new List<string>()))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src => src.PropertyImprovements != null
                    ? src.PropertyImprovements.Where(pi => pi.Improvement != null).Select(pi => pi.Improvement)
                    : new List<Domain.Entities.Improvement>()));
        }
    }
}
