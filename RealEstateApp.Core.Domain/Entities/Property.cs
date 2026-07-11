using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Property
    {
        public required int Id { get; set; }
        public required string Code { get; set; }
        public required int PropertyTypeId { get; set; }
        public PropertyType? PropertyType { get; set; }
        public required int SaleTypeId { get; set; }
        public SaleType? SaleType { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required decimal LandSize { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required string AgentId { get; set; }
        public PropertyStatus Status { get; set; } = PropertyStatus.Available;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<PropertyImage>? Images { get; set; }
        public ICollection<PropertyImprovement>? PropertyImprovements { get; set; }
        public ICollection<Offer>? Offers { get; set; }
        public ICollection<ChatMessage>? ChatMessages { get; set; }
        public ICollection<FavoriteProperty>? Favorites { get; set; }
    }
}
