using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoritePropertyRepository _favoritePropertyRepository;
        private readonly IPropertyService _propertyService;

        public FavoriteService(IFavoritePropertyRepository favoritePropertyRepository, IPropertyService propertyService)
        {
            _favoritePropertyRepository = favoritePropertyRepository;
            _propertyService = propertyService;
        }

        public async Task<bool> ToggleAsync(string clientId, int propertyId)
        {
            var favorite = await _favoritePropertyRepository.GetAsync(clientId, propertyId);

            if (favorite == null)
            {
                await _favoritePropertyRepository.AddAsync(new FavoriteProperty
                {
                    ClientId = clientId,
                    PropertyId = propertyId
                });

                return true;
            }

            await _favoritePropertyRepository.DeleteAsync(clientId, propertyId);
            return false;
        }

        public async Task<List<PropertyDto>> GetClientFavoritesAsync(string clientId, PropertyFilterDto? filters = null)
        {
            var favoriteIds = await _favoritePropertyRepository.GetAllQuery()
                .Where(f => f.ClientId == clientId)
                .Select(f => f.PropertyId)
                .ToListAsync();

            var properties = await _propertyService.GetAllWithFiltersAsync(filters ?? new PropertyFilterDto());

            return properties.Where(p => favoriteIds.Contains(p.Id)).ToList();
        }

        public async Task<HashSet<int>> GetFavoriteIdsAsync(string clientId)
        {
            var favoriteIds = await _favoritePropertyRepository.GetAllQuery()
                .Where(f => f.ClientId == clientId)
                .Select(f => f.PropertyId)
                .ToListAsync();

            return favoriteIds.ToHashSet();
        }
    }
}
