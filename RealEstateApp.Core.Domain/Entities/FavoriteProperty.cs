namespace RealEstateApp.Core.Domain.Entities
{
    public class FavoriteProperty
    {
        public required string ClientId { get; set; }
        public required int PropertyId { get; set; }
        public Property? Property { get; set; }
    }
}
