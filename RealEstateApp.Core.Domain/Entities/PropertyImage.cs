namespace RealEstateApp.Core.Domain.Entities
{
    public class PropertyImage
    {
        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public Property? Property { get; set; }
        public required string ImagePath { get; set; }
    }
}
