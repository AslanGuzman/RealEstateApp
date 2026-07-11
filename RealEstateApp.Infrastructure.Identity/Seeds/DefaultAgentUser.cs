using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAgentUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "Pedro",
                LastName = "Santana",
                Email = "agent@email.com",
                UserName = "agent",
                PhoneNumber = "809-555-0101",
                EmailConfirmed = true,
                IsActive = true
            };

            var entityUser = await userManager.FindByEmailAsync(user.Email);
            if (entityUser == null)
            {
                await userManager.CreateAsync(user, "123Pa$$word!");
                await userManager.AddToRoleAsync(user, Roles.Agent.ToString());
            }
        }
    }
}
