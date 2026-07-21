namespace RealEstateApp.Core.Application.Dtos.Property
{
    public class SavePropertyDto
    {
        public required int PropertyTypeId { get; set; }
        public required int SaleTypeId { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required decimal LandSize { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required string AgentId { get; set; }
        public List<string> Images { get; set; } = [];
        public List<int> ImprovementIds { get; set; } = [];
        public bool KeepCurrentImages { get; set; } = true;
    }
}
