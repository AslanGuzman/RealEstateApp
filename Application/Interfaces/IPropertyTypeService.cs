using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.PropertyType;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyTypeService : IGenericService<PropertyTypeDto>
    {
        Task<bool> ExistsByNameAsync(string name, int excludeId = 0);
        Task<List<CatalogItemDto>> GetAllWithCountAsync();
    }
}
