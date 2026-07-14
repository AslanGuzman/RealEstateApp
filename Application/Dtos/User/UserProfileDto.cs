namespace RealEstateApp.Core.Application.Dtos.User
{
    public class UserProfileDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
    }
}
