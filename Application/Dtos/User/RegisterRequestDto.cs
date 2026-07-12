using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.User
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Debe ingresar el nombre")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Debe ingresar el apellido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre de usuario")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Debe ingresar el correo")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        public required string Email { get; set; }

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Debe ingresar la cédula")]
        public required string IdentityCard { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public required string ConfirmPassword { get; set; }
    }
}
