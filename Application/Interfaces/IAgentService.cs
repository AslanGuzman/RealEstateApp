using RealEstateApp.Core.Application.Dtos.Agent;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAgentService
    {
        Task<List<AgentDto>> GetAllAgentsAsync(bool onlyActive = true);
        Task<AgentDto?> GetAgentByIdAsync(string id);
        Task<bool> ChangeStatusAsync(string id, bool isActive);
        Task<bool> DeleteAgentAsync(string id);
    }
}
