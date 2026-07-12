using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.PropertyType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService : GenericService<PropertyType, PropertyTypeDto>, IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypeService(IPropertyTypeRepository propertyTypeRepository, IMapper mapper) : base(propertyTypeRepository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _propertyTypeRepository.GetAllQuery().AnyAsync(pt => pt.Name == name && pt.Id != excludeId);
        }
    }
}
