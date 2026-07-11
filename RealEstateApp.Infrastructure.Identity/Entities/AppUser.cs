using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? ProfileImage { get; set; }
        public string? IdentityCard { get; set; }
        public required bool IsActive { get; set; }
    }
}
