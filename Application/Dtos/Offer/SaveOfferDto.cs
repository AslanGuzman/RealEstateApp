using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Offer
{
    public class SaveOfferDto
    {
        public required int PropertyId { get; set; }
        public required string ClientId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto de la oferta debe ser mayor que cero")]
        public required decimal Amount { get; set; }
    }
}
