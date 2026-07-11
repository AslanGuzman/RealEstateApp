using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Offer
    {
        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public Property? Property { get; set; }
        public required string ClientId { get; set; }
        public required decimal Amount { get; set; }
        public DateTime OfferDate { get; set; } = DateTime.UtcNow;
        public OfferStatus Status { get; set; } = OfferStatus.Pending;
    }
}
