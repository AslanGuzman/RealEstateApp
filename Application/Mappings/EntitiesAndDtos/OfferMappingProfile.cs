using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Offer;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class OfferMappingProfile : Profile
    {
        public OfferMappingProfile()
        {
            CreateMap<Domain.Entities.Offer, OfferDto>()
                .ReverseMap()
                .ForMember(dest => dest.Property, opt => opt.Ignore());
        }
    }
}
