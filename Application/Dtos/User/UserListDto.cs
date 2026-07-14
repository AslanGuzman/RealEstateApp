namespace RealEstateApp.Core.Application.Dtos.User
{
    public class UserListDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? UserName { get; set; }
        public string? IdentityCard { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public int PropertiesQuantity { get; set; }
    }
}
