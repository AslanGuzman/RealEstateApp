using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp : BaseAccountService, IAccountServiceForWebApp
    {
        public AccountServiceForWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            LoginResponseDto response = new()
            {
                Id = "",
                Name = "",
                LastName = "",
                Email = "",
                UserName = "",
                HasError = false,
                Errors = []
            };

            var user = await UserManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("Credenciales inválidas, intente de nuevo");
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Errors.Add("Debe activar su cuenta desde el enlace enviado a su correo");
                return response;
            }

            if (!user.IsActive)
            {
                response.HasError = true;
                response.Errors.Add("Su cuenta está inactiva, contacte al administrador");
                return response;
            }

            var roles = await UserManager.GetRolesAsync(user);

            if (roles.Contains(Roles.Developer.ToString()))
            {
                response.HasError = true;
                response.Errors.Add("Los usuarios desarrolladores no tienen acceso a la aplicación web");
                return response;
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
            {
                response.HasError = true;
                if (result.IsLockedOut)
                {
                    response.Errors.Add("Su cuenta ha sido bloqueada por múltiples intentos fallidos, intente de nuevo en 5 minutos");
                }
                else
                {
                    response.Errors.Add("Credenciales inválidas, intente de nuevo");
                }
                return response;
            }

            response.Id = user.Id;
            response.Name = user.Name;
            response.LastName = user.LastName;
            response.Email = user.Email ?? "";
            response.UserName = user.UserName ?? "";
            response.IsVerified = user.EmailConfirmed;
            response.Roles = roles.ToList();

            return response;
        }

        public async Task SignOutAsync()
        {
            await SignInManager.SignOutAsync();
        }
    }
}
