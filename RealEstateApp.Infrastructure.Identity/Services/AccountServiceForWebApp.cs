using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Email;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp : BaseAccountService, IAccountServiceForWebApp
    {
        private readonly IEmailService _emailService;

        public AccountServiceForWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
            : base(userManager, signInManager)
        {
            _emailService = emailService;
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

            var user = await UserManager.FindByNameAsync(loginDto.UserName)
                ?? await UserManager.FindByEmailAsync(loginDto.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("Los datos de acceso son inválidos");
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Errors.Add("Debe activar su cuenta desde el enlace enviado a su correo electrónico");
                return response;
            }

            if (!user.IsActive)
            {
                response.HasError = true;
                response.Errors.Add("El usuario se encuentra inactivo y no puede iniciar sesión");
                return response;
            }

            var roles = await UserManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                response.HasError = true;
                response.Errors.Add("El usuario no tiene un rol válido asignado. Póngase en contacto con un administrador");
                return response;
            }

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
                    response.Errors.Add("Los datos de acceso son inválidos");
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

        public async Task<RegisterResponseDto> RegisterClientAsync(RegisterRequestDto dto, string origin)
        {
            var response = await RegisterUserAsync(dto, Roles.Client.ToString(), true, false);

            if (response.HasError || response.Id == null)
            {
                return response;
            }

            var user = await UserManager.FindByIdAsync(response.Id);

            if (user != null)
            {
                var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                var verificationUri = $"{origin}/Login/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = dto.Email,
                    Subject = "Activación de cuenta en RealEstateApp",
                    HtmlBody = $"<p>Hola {dto.Name},</p>" +
                        "<p>Su cuenta ha sido registrada correctamente en RealEstateApp.</p>" +
                        "<p>Para activar su usuario y poder iniciar sesión, utilice el siguiente enlace:</p>" +
                        $"<p><a href='{verificationUri}'>Activar mi cuenta</a></p>" +
                        "<p>Si usted no realizó este registro, puede ignorar este mensaje.</p>"
                });
            }

            return response;
        }

        public async Task<RegisterResponseDto> RegisterAgentAsync(RegisterRequestDto dto)
        {
            return await RegisterUserAsync(dto, Roles.Agent.ToString(), false, true);
        }

        public async Task<string?> ConfirmEmailAsync(string userId, string token)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var result = await UserManager.ConfirmEmailAsync(user, token);

            return result.Succeeded ? user.UserName : null;
        }

        public async Task SetProfileImageAsync(string userId, string imagePath)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.ProfileImage = imagePath;
                await UserManager.UpdateAsync(user);
            }
        }

        public async Task<Dictionary<string, string>> GetUserNamesAsync(List<string> userIds)
        {
            return await UserManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => $"{u.Name} {u.LastName}");
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserProfileDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                ProfileImage = user.ProfileImage
            };
        }

        public async Task UpdateUserProfileAsync(string userId, UserProfileDto dto)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return;
            }

            user.Name = dto.Name;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.ProfileImage))
            {
                user.ProfileImage = dto.ProfileImage;
            }

            await UserManager.UpdateAsync(user);
        }
    }
}
