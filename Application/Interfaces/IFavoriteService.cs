using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> ToggleAsync(string clientId, int propertyId);
        Task<List<PropertyDto>> GetClientFavoritesAsync(string clientId, PropertyFilterDto? filters = null);
        Task<HashSet<int>> GetFavoriteIdsAsync(string clientId);
    }
}
