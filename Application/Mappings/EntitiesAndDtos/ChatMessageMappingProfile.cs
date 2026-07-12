using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Chat;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class ChatMessageMappingProfile : Profile
    {
        public ChatMessageMappingProfile()
        {
            CreateMap<Domain.Entities.ChatMessage, ChatMessageDto>()
                .ReverseMap()
                .ForMember(dest => dest.Property, opt => opt.Ignore());
        }
    }
}
