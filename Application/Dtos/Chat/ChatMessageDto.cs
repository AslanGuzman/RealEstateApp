using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Chat
{
    public class ChatMessageDto
    {
        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public required string ClientId { get; set; }
        public required string AgentId { get; set; }
        public required string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Roles SenderRole { get; set; }
    }
}
