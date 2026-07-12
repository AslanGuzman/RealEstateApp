using Microsoft.AspNetCore.Http;
using RealEstateApp.Core.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class RegisterUserViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "La foto de usuario es requerida")]
        public required IFormFile Photo { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        public required Roles UserType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserType != Roles.Client && UserType != Roles.Agent)
            {
                yield return new ValidationResult("El tipo de usuario seleccionado debe ser Cliente o Agente", [nameof(UserType)]);
            }

            if (Photo != null)
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
