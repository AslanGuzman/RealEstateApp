using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class FavoritePropertyRepository : GenericRepository<FavoriteProperty>, IFavoritePropertyRepository
    {
        private readonly RealEstateAppContext _context;

        public FavoritePropertyRepository(RealEstateAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FavoriteProperty?> GetAsync(string clientId, int propertyId)
        {
            return await _context.FavoriteProperties
                .FirstOrDefaultAsync(f => f.ClientId == clientId && f.PropertyId == propertyId);
        }

        public async Task DeleteAsync(string clientId, int propertyId)
        {
            var favorite = await GetAsync(clientId, propertyId);
            if (favorite != null)
            {
                _context.FavoriteProperties.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
