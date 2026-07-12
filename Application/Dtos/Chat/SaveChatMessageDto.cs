using RealEstateApp.Core.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Chat
{
    public class SaveChatMessageDto
    {
        public required int PropertyId { get; set; }
        public required string ClientId { get; set; }
        public required string AgentId { get; set; }

        [Required(ErrorMessage = "Debe escribir un mensaje")]
        public required string Content { get; set; }

        public required Roles SenderRole { get; set; }
    }
}
