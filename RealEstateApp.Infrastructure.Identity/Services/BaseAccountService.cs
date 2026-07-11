using Microsoft.AspNetCore.Identity;
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
    }
}
