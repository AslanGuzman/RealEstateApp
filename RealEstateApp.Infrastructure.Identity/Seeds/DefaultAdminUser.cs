using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "Ana",
                LastName = "Reyes",
                Email = "admin@email.com",
                UserName = "admin",
                EmailConfirmed = true,
                IsActive = true,
                IdentityCard = "001-0000001-1"
            };

            var entityUser = await userManager.FindByEmailAsync(user.Email);
            if (entityUser == null)
            {
                await userManager.CreateAsync(user, "123Pa$$word!");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
    }
}
