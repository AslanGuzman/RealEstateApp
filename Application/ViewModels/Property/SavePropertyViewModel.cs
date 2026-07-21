using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class SavePropertyViewModel : IValidatableObject
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        [Required(ErrorMessage = "El tipo de propiedad es requerido")]
        public int? PropertyTypeId { get; set; }

        [Required(ErrorMessage = "El tipo de venta es requerido")]
        public int? SaleTypeId { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor numérico mayor que cero")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El tamaño de la propiedad es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser un valor numérico mayor que cero")]
        public decimal? LandSize { get; set; }

        [Required(ErrorMessage = "La cantidad de habitaciones es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de habitaciones no puede ser menor que cero")]
        public int? Rooms { get; set; }

        [Required(ErrorMessage = "La cantidad de baños es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser menor que cero")]
        public int? Bathrooms { get; set; }

        public List<int> ImprovementIds { get; set; } = [];

        public List<IFormFile>? Images { get; set; }

        public List<string> CurrentImages { get; set; } = [];

        public bool KeepCurrentImages { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImprovementIds.Count == 0)
            {
                yield return new ValidationResult("Debe seleccionarse al menos una mejora", [nameof(ImprovementIds)]);
            }

            var newImagesCount = Images?.Count(i => i != null && i.Length > 0) ?? 0;

            if (Id == 0 && newImagesCount == 0)
            {
                yield return new ValidationResult("Debe cargar al menos una imagen de la propiedad", [nameof(Images)]);
            }

            var keepCount = Id != 0 && KeepCurrentImages ? CurrentImages.Count : 0;
            var totalImages = keepCount + newImagesCount;

            if (Id != 0 && totalImages == 0)
            {
                yield return new ValidationResult("La propiedad debe mantener al menos una imagen", [nameof(Images)]);
            }

            if (totalImages > 4)
            {
                yield return new ValidationResult("Solo se permite registrar hasta 4 imágenes por propiedad", [nameof(Images)]);
            }

            foreach (var image in Images ?? [])
            {
                if (image == null || image.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    yield return new ValidationResult("El archivo seleccionado no tiene un formato de imagen válido", [nameof(Images)]);
                    break;
                }
            }
        }
    }
}
