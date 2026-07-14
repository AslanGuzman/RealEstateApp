using RealEstateApp.Core.Application.Dtos.Admin;
using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IUserAdminService
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<List<UserListDto>> GetByRoleAsync(Roles role);
        Task<UserListDto?> GetByIdAsync(string id, Roles role);
        Task<UserManagementResponseDto> CreateAsync(SaveUserDto dto, Roles role);
        Task<UserManagementResponseDto> UpdateAsync(SaveUserDto dto, Roles role);
        Task<OperationResponseDto> ChangeStatusAsync(string id, bool isActive, string currentUserId, Roles role);
    }
}
