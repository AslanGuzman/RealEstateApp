using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyFilterViewModel : IValidatableObject
    {
        public string? Code { get; set; }

        public int? PropertyTypeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser menor que cero")]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio máximo no puede ser menor que cero")]
        public decimal? MaxPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero")]
        public int? Rooms { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero")]
        public int? Bathrooms { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            {
                yield return new ValidationResult("El precio mínimo no puede ser mayor que el precio máximo", [nameof(MinPrice)]);
            }
        }
    }
}
