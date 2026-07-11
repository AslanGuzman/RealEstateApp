using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class FavoritePropertyRepository : GenericRepository<FavoriteProperty>, IFavoritePropertyRepository
    {
        public FavoritePropertyRepository(RealEstateAppContext context) : base(context)
        {
        }
    }
}
