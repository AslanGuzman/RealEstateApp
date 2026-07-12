using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IFavoritePropertyRepository : IGenericRepository<FavoriteProperty>
    {
        Task<FavoriteProperty?> GetAsync(string clientId, int propertyId);
        Task DeleteAsync(string clientId, int propertyId);
    }
}
