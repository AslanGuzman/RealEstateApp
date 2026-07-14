using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.SaleType;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface ISaleTypeService : IGenericService<SaleTypeDto>
    {
        Task<bool> ExistsByNameAsync(string name, int excludeId = 0);
        Task<List<CatalogItemDto>> GetAllWithCountAsync();
    }
}
