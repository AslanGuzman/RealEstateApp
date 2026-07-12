using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.PropertyType
{
    public class SavePropertyTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del tipo de propiedad")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Debe ingresar la descripción del tipo de propiedad")]
        public required string Description { get; set; }
    }
}
