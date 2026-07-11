using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Domain.Entities
{
    public class ChatMessage
    {
        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public Property? Property { get; set; }
        public required string ClientId { get; set; }
        public required string AgentId { get; set; }
        public required string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public required Roles SenderRole { get; set; }
    }
}
