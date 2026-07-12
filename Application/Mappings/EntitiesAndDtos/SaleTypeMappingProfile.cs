using AutoMapper;
using RealEstateApp.Core.Application.Dtos.SaleType;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class SaleTypeMappingProfile : Profile
    {
        public SaleTypeMappingProfile()
        {
            CreateMap<Domain.Entities.SaleType, SaleTypeDto>()
                .ReverseMap()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());
        }
    }
}
