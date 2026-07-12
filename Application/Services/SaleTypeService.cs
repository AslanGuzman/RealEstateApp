using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.SaleType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SaleTypeService : GenericService<SaleType, SaleTypeDto>, ISaleTypeService
    {
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IPropertyRepository _propertyRepository;

        public SaleTypeService(ISaleTypeRepository saleTypeRepository, IPropertyRepository propertyRepository, IMapper mapper)
            : base(saleTypeRepository, mapper)
        {
            _saleTypeRepository = saleTypeRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _saleTypeRepository.GetAllQuery().AnyAsync(st => st.Name == name && st.Id != excludeId);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var saleType = await _saleTypeRepository.GetById(id);

            if (saleType == null)
            {
                return false;
            }

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                var properties = await _propertyRepository.GetAllQuery()
                    .Where(p => p.SaleTypeId == id)
                    .ToListAsync();

                foreach (var property in properties)
                {
                    await _propertyRepository.DeleteAsync(property.Id);
                }

                await _saleTypeRepository.DeleteAsync(id);
            });

            return true;
        }
    }
}
