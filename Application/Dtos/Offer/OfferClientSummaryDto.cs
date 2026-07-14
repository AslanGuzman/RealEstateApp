using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Dtos.Offer
{
    public class OfferClientSummaryDto
    {
        public required string ClientId { get; set; }
        public string? ClientName { get; set; }
        public int OffersCount { get; set; }
        public decimal LastAmount { get; set; }
        public OfferStatus LastStatus { get; set; }
    }
}
