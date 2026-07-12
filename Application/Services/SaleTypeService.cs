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

        public SaleTypeService(ISaleTypeRepository saleTypeRepository, IMapper mapper) : base(saleTypeRepository, mapper)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _saleTypeRepository.GetAllQuery().AnyAsync(st => st.Name == name && st.Id != excludeId);
        }
    }
}
