using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public ChatService(IChatMessageRepository chatMessageRepository, IPropertyRepository propertyRepository, IMapper mapper)
        {
            _chatMessageRepository = chatMessageRepository;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResponseDto> SendAsync(SaveChatMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new OperationResponseDto { HasError = true, Error = "Debe escribir un mensaje" };
            }

            var property = await _propertyRepository.GetById(dto.PropertyId);

            if (property == null || property.Status != PropertyStatus.Available)
            {
                return new OperationResponseDto { HasError = true, Error = "La propiedad no está disponible para recibir mensajes" };
            }

            await _chatMessageRepository.AddAsync(new ChatMessage
            {
                Id = 0,
                PropertyId = dto.PropertyId,
                ClientId = dto.ClientId,
                AgentId = dto.AgentId,
                Content = dto.Content,
                SenderRole = dto.SenderRole
            });

            return new OperationResponseDto();
        }

        public async Task<List<ChatMessageDto>> GetConversationAsync(int propertyId, string clientId)
        {
            var messages = await _chatMessageRepository.GetAllQuery()
                .Where(m => m.PropertyId == propertyId && m.ClientId == clientId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return _mapper.Map<List<ChatMessageDto>>(messages);
        }

        public async Task<List<string>> GetClientsWithConversationAsync(int propertyId)
        {
            return await _chatMessageRepository.GetAllQuery()
                .Where(m => m.PropertyId == propertyId)
                .Select(m => m.ClientId)
                .Distinct()
                .ToListAsync();
        }
    }
}
