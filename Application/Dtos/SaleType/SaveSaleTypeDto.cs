using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.SaleType
{
    public class SaveSaleTypeDto
    {
        [Required(ErrorMessage = "Debe ingresar el nombre del tipo de venta")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Debe ingresar la descripción del tipo de venta")]
        public required string Description { get; set; }
    }
}
