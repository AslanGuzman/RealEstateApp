using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApi
    {
        Task<LoginResponseForApiDto> AuthenticateAsync(LoginDto loginDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto, string role);
    }
}
