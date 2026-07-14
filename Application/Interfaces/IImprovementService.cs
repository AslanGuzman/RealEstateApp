using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.Improvement;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IImprovementService : IGenericService<ImprovementDto>
    {
        Task<bool> ExistsByNameAsync(string name, int excludeId = 0);
        Task<List<CatalogItemDto>> GetAllWithCountAsync();
    }
}
