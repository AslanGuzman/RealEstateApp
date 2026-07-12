using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Improvement;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class ImprovementMappingProfile : Profile
    {
        public ImprovementMappingProfile()
        {
            CreateMap<Domain.Entities.Improvement, ImprovementDto>()
                .ReverseMap()
                .ForMember(dest => dest.PropertyImprovements, opt => opt.Ignore());
        }
    }
}
