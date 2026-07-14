using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class UserProfileViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        public required string Phone { get; set; }

        public IFormFile? Photo { get; set; }

        public string? CurrentImage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Photo != null && Photo.Length > 0)
            {
                var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    yield return new ValidationResult("El archivo seleccionado no tiene un formato de imagen válido", [nameof(Photo)]);
                }
            }
        }
    }
}
