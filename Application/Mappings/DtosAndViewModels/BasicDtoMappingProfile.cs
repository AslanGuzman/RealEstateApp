using AutoMapper;
using RealEstateApp.Core.Application.Dtos;
using RealEstateApp.Core.Application.ViewModels;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class BasicDtoMappingProfile : Profile
    {
        public BasicDtoMappingProfile()
        {
            CreateMap(typeof(BasicDto<>), typeof(BasicViewModel<>)).ReverseMap();
        }
    }
}
