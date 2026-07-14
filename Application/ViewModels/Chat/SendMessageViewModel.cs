using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Chat
{
    public class SendMessageViewModel
    {
        [Required]
        public int PropertyId { get; set; }

        public string? ClientId { get; set; }

        [Required(ErrorMessage = "Debe escribir un mensaje antes de enviarlo.")]
        public required string Content { get; set; }
    }
}
