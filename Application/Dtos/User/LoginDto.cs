using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Debe ingresar el usuario o correo electrónico")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public required string Password { get; set; }
    }
}
