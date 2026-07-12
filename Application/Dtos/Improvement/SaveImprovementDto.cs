using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Improvement
{
    public class SaveImprovementDto
    {
        [Required(ErrorMessage = "Debe ingresar el nombre de la mejora")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Debe ingresar la descripción de la mejora")]
        public required string Description { get; set; }
    }
}
