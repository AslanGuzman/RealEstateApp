using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Admin;
using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPropertyRepository _propertyRepository;

        public UserAdminService(UserManager<AppUser> userManager, IPropertyRepository propertyRepository)
        {
            _userManager = userManager;
            _propertyRepository = propertyRepository;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var agents = await _userManager.GetUsersInRoleAsync(Roles.Agent.ToString());
            var clients = await _userManager.GetUsersInRoleAsync(Roles.Client.ToString());
            var developers = await _userManager.GetUsersInRoleAsync(Roles.Developer.ToString());

            return new DashboardDto
            {
                AvailableProperties = await _propertyRepository.GetAllQuery().CountAsync(p => p.Status == PropertyStatus.Available),
                SoldProperties = await _propertyRepository.GetAllQuery().CountAsync(p => p.Status == PropertyStatus.Sold),
                ActiveAgents = agents.Count(u => u.IsActive),
                InactiveAgents = agents.Count(u => !u.IsActive),
                ActiveClients = clients.Count(u => u.IsActive),
                InactiveClients = clients.Count(u => !u.IsActive),
                ActiveDevelopers = developers.Count(u => u.IsActive),
                InactiveDevelopers = developers.Count(u => !u.IsActive)
            };
        }

        public async Task<List<UserListDto>> GetByRoleAsync(Roles role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.ToString());
            var result = new List<UserListDto>();

            foreach (var user in users.OrderBy(u => u.Name).ThenBy(u => u.LastName))
            {
                result.Add(new UserListDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    IdentityCard = user.IdentityCard,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    PropertiesQuantity = role == Roles.Agent
                        ? await _propertyRepository.GetAllQuery().CountAsync(p => p.AgentId == user.Id)
                        : 0
                });
            }

            return result;
        }

        public async Task<UserListDto?> GetByIdAsync(string id, Roles role)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                return null;
            }

            return new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                UserName = user.UserName,
                IdentityCard = user.IdentityCard,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<UserManagementResponseDto> CreateAsync(SaveUserDto dto, Roles role)
        {
            UserManagementResponseDto response = new();

            var uniquenessError = await CheckUniquenessAsync(dto, null);
            if (uniquenessError != null)
            {
                response.HasError = true;
                response.Errors.Add(uniquenessError);
                return response;
            }

            AppUser user = new()
            {
                Name = dto.Name,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                IdentityCard = dto.IdentityCard,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password ?? "");

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            await _userManager.AddToRoleAsync(user, role.ToString());

            return response;
        }

        public async Task<UserManagementResponseDto> UpdateAsync(SaveUserDto dto, Roles role)
        {
            UserManagementResponseDto response = new();

            var user = dto.Id != null ? await _userManager.FindByIdAsync(dto.Id) : null;

            if (user == null || !await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                response.HasError = true;
                response.Errors.Add("El usuario seleccionado no existe.");
                return response;
            }

            var uniquenessError = await CheckUniquenessAsync(dto, user.Id);
            if (uniquenessError != null)
            {
                response.HasError = true;
                response.Errors.Add(uniquenessError);
                return response;
            }

            user.Name = dto.Name;
            user.LastName = dto.LastName;
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.IdentityCard = dto.IdentityCard;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(updateResult.Errors.Select(e => e.Description));
                return response;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                if (!passwordResult.Succeeded)
                {
                    response.HasError = true;
                    response.Errors.AddRange(passwordResult.Errors.Select(e => e.Description));
                }
            }

            return response;
        }

        public async Task<OperationResponseDto> ChangeStatusAsync(string id, bool isActive, string currentUserId, Roles role)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                return Error("El usuario seleccionado no existe.");
            }

            if (role == Roles.Admin && !isActive)
            {
                if (user.Id == currentUserId)
                {
                    return Error("No puede inactivar a su propio usuario.");
                }

                var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin.ToString());
                if (admins.Count(a => a.IsActive) <= 1)
                {
                    return Error("Debe existir al menos un administrador activo en el sistema.");
                }
            }

            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);

            return new OperationResponseDto();
        }

        private async Task<string?> CheckUniquenessAsync(SaveUserDto dto, string? excludeId)
        {
            var byUserName = await _userManager.FindByNameAsync(dto.UserName);
            if (byUserName != null && byUserName.Id != excludeId)
            {
                return "Ya existe un usuario registrado con este nombre de usuario";
            }

            var byEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (byEmail != null && byEmail.Id != excludeId)
            {
                return "Ya existe un usuario registrado con este correo electrónico";
            }

            var byCard = await _userManager.Users.FirstOrDefaultAsync(u => u.IdentityCard == dto.IdentityCard);
            if (byCard != null && byCard.Id != excludeId)
            {
                return "Ya existe un usuario registrado con esta cédula";
            }

            return null;
        }

        private static OperationResponseDto Error(string message)
        {
            return new OperationResponseDto { HasError = true, Error = message };
        }
    }
}
