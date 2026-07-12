namespace RealEstateApp.Core.Application.Dtos.User
{
    public class RegisterResponseDto
    {
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
        public string? Id { get; set; }
        public string? UserName { get; set; }
    }
}
