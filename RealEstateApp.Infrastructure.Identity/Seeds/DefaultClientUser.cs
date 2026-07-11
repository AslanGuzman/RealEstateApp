using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultClientUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "Laura",
                LastName = "Mendez",
                Email = "client@email.com",
                UserName = "client",
                PhoneNumber = "809-555-0102",
                EmailConfirmed = true,
                IsActive = true
            };

            var entityUser = await userManager.FindByEmailAsync(user.Email);
            if (entityUser == null)
            {
                await userManager.CreateAsync(user, "123Pa$$word!");
                await userManager.AddToRoleAsync(user, Roles.Client.ToString());
            }
        }
    }
}
