namespace RealEstateApp.Core.Application.Dtos.User
{
    public class UserManagementResponseDto
    {
        public bool HasError { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
