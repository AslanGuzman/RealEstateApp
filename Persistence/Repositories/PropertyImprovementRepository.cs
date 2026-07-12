using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyImprovementRepository : GenericRepository<PropertyImprovement>, IPropertyImprovementRepository
    {
        private readonly RealEstateAppContext _context;

        public PropertyImprovementRepository(RealEstateAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task DeleteByPropertyAsync(int propertyId)
        {
            var relations = await _context.PropertyImprovements
                .Where(pi => pi.PropertyId == propertyId)
                .ToListAsync();

            _context.PropertyImprovements.RemoveRange(relations);
            await _context.SaveChangesAsync();
        }
    }
}
