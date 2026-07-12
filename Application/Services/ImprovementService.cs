using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Improvement;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ImprovementService : GenericService<Improvement, ImprovementDto>, IImprovementService
    {
        private readonly IImprovementRepository _improvementRepository;

        public ImprovementService(IImprovementRepository improvementRepository, IMapper mapper) : base(improvementRepository, mapper)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _improvementRepository.GetAllQuery().AnyAsync(i => i.Name == name && i.Id != excludeId);
        }
    }
}
