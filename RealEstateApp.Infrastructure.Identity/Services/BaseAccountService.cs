using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public abstract class BaseAccountService
    {
        protected readonly UserManager<AppUser> UserManager;
        protected readonly SignInManager<AppUser> SignInManager;

        protected BaseAccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        protected static bool IsInactive(AppUser user)
        {
            return !user.IsActive || !user.EmailConfirmed;
        }

        protected async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto dto, string role, bool isActive, bool emailConfirmed)
        {
            RegisterResponseDto response = new()
            {
                HasError = false,
                Errors = []
            };

            var userWithSameUserName = await UserManager.FindByNameAsync(dto.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Errors.Add($"El nombre de usuario {dto.UserName} ya está en uso");
                return response;
            }

            var userWithSameEmail = await UserManager.FindByEmailAsync(dto.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Errors.Add($"El correo {dto.Email} ya está registrado");
                return response;
            }

            var identityCardInUse = await UserManager.Users.AnyAsync(u => u.IdentityCard == dto.IdentityCard);
            if (identityCardInUse)
            {
                response.HasError = true;
                response.Errors.Add($"La cédula {dto.IdentityCard} ya está registrada");
                return response;
            }

            AppUser user = new()
            {
                Name = dto.Name,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                IdentityCard = dto.IdentityCard,
                IsActive = isActive,
                EmailConfirmed = emailConfirmed
            };

            var result = await UserManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            await UserManager.AddToRoleAsync(user, role);

            response.Id = user.Id;
            response.UserName = user.UserName;

            return response;
        }
    }
}
