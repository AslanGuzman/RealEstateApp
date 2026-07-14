namespace RealEstateApp.Core.Application.Dtos.User
{
    public class SaveUserDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string IdentityCard { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? Password { get; set; }
    }
}
