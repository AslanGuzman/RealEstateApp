using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Dtos.Common;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IChatService
    {
        Task<OperationResponseDto> SendAsync(SaveChatMessageDto dto);
        Task<List<ChatMessageDto>> GetConversationAsync(int propertyId, string clientId);
        Task<List<string>> GetClientsWithConversationAsync(int propertyId);
        Task<List<ConversationSummaryDto>> GetConversationSummariesAsync(int propertyId);
    }
}
