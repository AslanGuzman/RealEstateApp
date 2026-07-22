using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyService : GenericService<Property, PropertyDto>, IPropertyService
    {
        private static readonly List<string> DetailIncludes = ["PropertyType", "SaleType", "Images", "PropertyImprovements.Improvement"];

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IPropertyImprovementRepository _propertyImprovementRepository;
        private readonly IAgentService _agentService;
        private readonly IMapper _mapper;

        public PropertyService(
            IPropertyRepository propertyRepository,
            IPropertyImageRepository propertyImageRepository,
            IPropertyImprovementRepository propertyImprovementRepository,
            IAgentService agentService,
            IMapper mapper) : base(propertyRepository, mapper)
        {
            _propertyRepository = propertyRepository;
            _propertyImageRepository = propertyImageRepository;
            _propertyImprovementRepository = propertyImprovementRepository;
            _agentService = agentService;
            _mapper = mapper;
        }

        public async Task<PropertyDto?> CreateAsync(SavePropertyDto dto)
        {
            if (dto.Images.Count < 1 || dto.Images.Count > 4)
            {
                return null;
            }

            var code = await GenerateUniqueCodeAsync();
            if (code == null)
            {
                return null;
            }

            Property property = new()
            {
                Id = 0,
                Code = code,
                PropertyTypeId = dto.PropertyTypeId,
                SaleTypeId = dto.SaleTypeId,
                Price = dto.Price,
                Description = dto.Description,
                LandSize = dto.LandSize,
                Rooms = dto.Rooms,
                Bathrooms = dto.Bathrooms,
                AgentId = dto.AgentId
            };

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                await _propertyRepository.AddAsync(property);
                await AddImagesAsync(property.Id, dto.Images);
                await AddImprovementsAsync(property.Id, dto.ImprovementIds);
            });

            return await GetByIdWithDetailsAsync(property.Id);
        }

        public async Task<PropertyDto?> UpdateAsync(SavePropertyDto dto, int id)
        {
            if (dto.Images.Count > 4)
            {
                return null;
            }

            var property = await _propertyRepository.GetById(id);

            if (property == null || property.AgentId != dto.AgentId || property.Status == PropertyStatus.Sold)
            {
                return null;
            }

            property.PropertyTypeId = dto.PropertyTypeId;
            property.SaleTypeId = dto.SaleTypeId;
            property.Price = dto.Price;
            property.Description = dto.Description;
            property.LandSize = dto.LandSize;
            property.Rooms = dto.Rooms;
            property.Bathrooms = dto.Bathrooms;

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                await _propertyRepository.UpdateAsync(id, property);
                await _propertyImprovementRepository.DeleteByPropertyAsync(id);
                await AddImprovementsAsync(id, dto.ImprovementIds);

                if (!dto.KeepCurrentImages)
                {
                    await ReplaceImagesAsync(id, dto.Images);
                }
                else if (dto.Images.Count > 0)
                {
                    await AddImagesAsync(id, dto.Images);
                }
            });

            return await GetByIdWithDetailsAsync(id);
        }

        public async Task<bool> DeleteAsync(int id, string agentId)
        {
            var property = await _propertyRepository.GetById(id);

            if (property == null || property.AgentId != agentId || property.Status == PropertyStatus.Sold)
            {
                return false;
            }

            await _propertyRepository.DeleteAsync(id);
            return true;
        }

        public async Task<List<PropertyDto>> GetAllWithFiltersAsync(PropertyFilterDto filters)
        {
            var query = _propertyRepository.GetAllQueryWithInclude(DetailIncludes);

            if (filters.OnlyAvailable)
            {
                query = query.Where(p => p.Status == PropertyStatus.Available);
            }

            if (!string.IsNullOrWhiteSpace(filters.Code))
            {
                query = query.Where(p => p.Code == filters.Code);
            }

            if (filters.PropertyTypeId.HasValue)
            {
                query = query.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value);
            }

            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            }

            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            if (filters.Rooms.HasValue)
            {
                query = query.Where(p => p.Rooms == filters.Rooms.Value);
            }

            if (filters.Bathrooms.HasValue)
            {
                query = query.Where(p => p.Bathrooms == filters.Bathrooms.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.AgentId))
            {
                query = query.Where(p => p.AgentId == filters.AgentId);
            }

            if (filters.AllowedAgentIds != null)
            {
                query = query.Where(p => filters.AllowedAgentIds.Contains(p.AgentId));
            }

            var properties = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            return _mapper.Map<List<PropertyDto>>(properties);
        }

        public async Task<PropertyDto?> GetByIdWithDetailsAsync(int id)
        {
            var property = await _propertyRepository.GetAllQueryWithInclude(DetailIncludes)
                .FirstOrDefaultAsync(p => p.Id == id);

            return await MapWithAgentAsync(property);
        }

        public async Task<PropertyDto?> GetByCodeAsync(string code)
        {
            var property = await _propertyRepository.GetAllQueryWithInclude(DetailIncludes)
                .FirstOrDefaultAsync(p => p.Code == code);

            return await MapWithAgentAsync(property);
        }

        private async Task<PropertyDto?> MapWithAgentAsync(Property? property)
        {
            if (property == null)
            {
                return null;
            }

            var dto = _mapper.Map<PropertyDto>(property);
            var agent = await _agentService.GetAgentByIdAsync(property.AgentId);
            dto.AgentName = agent != null ? $"{agent.Name} {agent.LastName}" : null;

            return dto;
        }

        private async Task<string?> GenerateUniqueCodeAsync()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var code = Random.Shared.Next(100000, 1000000).ToString();
                var exists = await _propertyRepository.GetAllQuery().AnyAsync(p => p.Code == code);

                if (!exists)
                {
                    return code;
                }
            }

            return null;
        }

        private async Task AddImagesAsync(int propertyId, List<string> images)
        {
            foreach (var path in images)
            {
                await _propertyImageRepository.AddAsync(new PropertyImage
                {
                    Id = 0,
                    PropertyId = propertyId,
                    ImagePath = path
                });
            }
        }

        private async Task AddImprovementsAsync(int propertyId, List<int> improvementIds)
        {
            foreach (var improvementId in improvementIds.Distinct())
            {
                await _propertyImprovementRepository.AddAsync(new PropertyImprovement
                {
                    PropertyId = propertyId,
                    ImprovementId = improvementId
                });
            }
        }

        private async Task ReplaceImagesAsync(int propertyId, List<string> images)
        {
            var existing = await _propertyImageRepository.GetAllQuery()
                .Where(i => i.PropertyId == propertyId)
                .ToListAsync();

            foreach (var image in existing)
            {
                await _propertyImageRepository.DeleteAsync(image.Id);
            }

            await AddImagesAsync(propertyId, images);
        }
    }
}
