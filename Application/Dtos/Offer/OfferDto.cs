using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Offer
{
    public class OfferDto
    {
        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public required string ClientId { get; set; }
        public required decimal Amount { get; set; }
        public DateTime OfferDate { get; set; }
        public OfferStatus Status { get; set; }
    }
}
