using RealEstateApp.Core.Application.Dtos.Improvement;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class PropertyDto
    {
        public required int Id { get; set; }
        public required string Code { get; set; }
        public required int PropertyTypeId { get; set; }
        public string? PropertyTypeName { get; set; }
        public required int SaleTypeId { get; set; }
        public string? SaleTypeName { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required decimal LandSize { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required string AgentId { get; set; }
        public string? AgentName { get; set; }
        public PropertyStatus Status { get; set; }
        public string StatusName => Status == PropertyStatus.Available ? "Disponible" : "Vendida";
        public DateTime CreatedAt { get; set; }
        public List<string> Images { get; set; } = [];
        public List<ImprovementDto> Improvements { get; set; } = [];
    }
}
