using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Developer")]
    public class PropertyController : BaseApiController
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>Lista todas las propiedades registradas, disponibles y vendidas</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto { OnlyAvailable = false });

                if (properties.Count == 0)
                {
                    return NoContent();
                }

                return Ok(properties);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene una propiedad por su identificador</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var property = await _propertyService.GetByIdWithDetailsAsync(id);

                if (property == null)
                {
                    return NotFound("No existe una propiedad con el id indicado");
                }

                return Ok(property);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene una propiedad por su código de seis dígitos</summary>
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                if (code.Length != 6 || !code.All(char.IsDigit))
                {
                    return BadRequest("El código debe tener seis dígitos numéricos");
                }

                var property = await _propertyService.GetByCodeAsync(code);

                if (property == null)
                {
                    return NotFound("No existe una propiedad con el código indicado");
                }

                return Ok(property);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }
    }
}
