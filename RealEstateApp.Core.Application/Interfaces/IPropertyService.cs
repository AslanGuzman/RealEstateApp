using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<List<Property>> GetAllAvailablePropertiesAsync();
        Task<Property> GetPropertyByIdAsync(int id);
    }
}