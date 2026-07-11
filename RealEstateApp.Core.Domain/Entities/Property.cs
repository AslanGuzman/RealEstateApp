using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Property : BaseEntity
    {
        public string Code { get; set; } = null!; 
        public int PropertyTypeId { get; set; }
        public int SaleTypeId { get; set; }
        public string AgentId { get; set; } = null!; 
        public decimal Price { get; set; }
        public decimal Size { get; set; }
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = "Disponible";

        public PropertyType PropertyType { get; set; } = null!;
        public SaleType SaleType { get; set; } = null!;
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<FavoriteProperty> Favorites { get; set; } = new List<FavoriteProperty>();
        public ICollection<Improvement> Improvements { get; set; } = new List<Improvement>();
    }
}