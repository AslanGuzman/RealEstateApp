namespace RealEstateApp.Core.Application.Dtos.Common
{
    public class CatalogItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int PropertiesQuantity { get; set; }
    }
}
