using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.PropertyType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService : GenericService<PropertyType, PropertyTypeDto>, IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyTypeService(IPropertyTypeRepository propertyTypeRepository, IPropertyRepository propertyRepository, IMapper mapper)
            : base(propertyTypeRepository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _propertyTypeRepository.GetAllQuery().AnyAsync(pt => pt.Name == name && pt.Id != excludeId);
        }

        public async Task<List<CatalogItemDto>> GetAllWithCountAsync()
        {
            return await _propertyTypeRepository.GetAllQuery()
                .Select(pt => new CatalogItemDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    PropertiesQuantity = pt.Properties!.Count
                })
                .ToListAsync();
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var propertyType = await _propertyTypeRepository.GetById(id);

            if (propertyType == null)
            {
                return false;
            }

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                var properties = await _propertyRepository.GetAllQuery()
                    .Where(p => p.PropertyTypeId == id)
                    .ToListAsync();

                foreach (var property in properties)
                {
                    await _propertyRepository.DeleteAsync(property.Id);
                }

                await _propertyTypeRepository.DeleteAsync(id);
            });

            return true;
        }
    }
}
