using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApp
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task SignOutAsync();
        Task<RegisterResponseDto> RegisterClientAsync(RegisterRequestDto dto, string origin);
        Task<RegisterResponseDto> RegisterAgentAsync(RegisterRequestDto dto);
        Task<string?> ConfirmEmailAsync(string userId, string token);
        Task SetProfileImageAsync(string userId, string imagePath);
    }
}
