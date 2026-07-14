using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class SaveUserViewModel : IValidatableObject
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "La cédula es requerida")]
        public required string IdentityCard { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public required string UserName { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var isCreate = string.IsNullOrWhiteSpace(Id);

            if (isCreate && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("La contraseña es requerida", [nameof(Password)]);
            }

            if (isCreate && string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                yield return new ValidationResult("La confirmación de contraseña es requerida", [nameof(ConfirmPassword)]);
            }

            if (!string.IsNullOrWhiteSpace(Password) && Password != ConfirmPassword)
            {
                yield return new ValidationResult("La contraseña y la confirmación de contraseña no coinciden", [nameof(ConfirmPassword)]);
            }
        }
    }
}
