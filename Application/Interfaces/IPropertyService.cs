using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<PropertyDto>
    {
        Task<PropertyDto?> CreateAsync(SavePropertyDto dto);
        Task<PropertyDto?> UpdateAsync(SavePropertyDto dto, int id);
        Task<bool> DeleteAsync(int id, string agentId);
        Task<List<PropertyDto>> GetAllWithFiltersAsync(PropertyFilterDto filters);
        Task<PropertyDto?> GetByIdWithDetailsAsync(int id);
        Task<PropertyDto?> GetByCodeAsync(string code);
    }
}
