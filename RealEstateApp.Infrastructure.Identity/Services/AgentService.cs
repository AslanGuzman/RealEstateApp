using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AgentService : IAgentService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPropertyRepository _propertyRepository;

        public AgentService(UserManager<AppUser> userManager, IPropertyRepository propertyRepository)
        {
            _userManager = userManager;
            _propertyRepository = propertyRepository;
        }

        public async Task<List<AgentDto>> GetAllAgentsAsync(bool onlyActive = true)
        {
            var users = await _userManager.GetUsersInRoleAsync(Roles.Agent.ToString());
            var agents = new List<AgentDto>();

            foreach (var user in users.Where(u => !onlyActive || u.IsActive).OrderBy(u => u.Name).ThenBy(u => u.LastName))
            {
                agents.Add(await MapAgentAsync(user));
            }

            return agents;
        }

        public async Task<AgentDto?> GetAgentByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, Roles.Agent.ToString()))
            {
                return null;
            }

            return await MapAgentAsync(user);
        }

        public async Task<bool> ChangeStatusAsync(string id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, Roles.Agent.ToString()))
            {
                return false;
            }

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> DeleteAgentAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.IsInRoleAsync(user, Roles.Agent.ToString()))
            {
                return false;
            }

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                var properties = await _propertyRepository.GetAllQuery()
                    .Where(p => p.AgentId == id)
                    .ToListAsync();

                foreach (var property in properties)
                {
                    await _propertyRepository.DeleteAsync(property.Id);
                }
            });

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        private async Task<AgentDto> MapAgentAsync(AppUser user)
        {
            var propertiesQuantity = await _propertyRepository.GetAllQuery().CountAsync(p => p.AgentId == user.Id);

            return new AgentDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                ProfileImage = user.ProfileImage,
                IsActive = user.IsActive,
                PropertiesQuantity = propertiesQuantity
            };
        }
    }
}
