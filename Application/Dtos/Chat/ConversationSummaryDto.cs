namespace RealEstateApp.Core.Application.Dtos.Chat
{
    public class ConversationSummaryDto
    {
        public required string ClientId { get; set; }
        public string? ClientName { get; set; }
        public required string LastMessage { get; set; }
        public DateTime LastDate { get; set; }
    }
}
