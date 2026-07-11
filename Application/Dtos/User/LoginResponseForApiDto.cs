namespace RealEstateApp.Core.Application.Dtos.User
{
    public class LoginResponseForApiDto
    {
        public bool HasError { get; set; }
        public bool IsForbidden { get; set; }
        public required List<string> Errors { get; set; }
        public string? Token { get; set; }
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
