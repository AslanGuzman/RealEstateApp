using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.PropertyType;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Developer")]
    public class PropertyTypeController : BaseApiController
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyTypeController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        /// <summary>Crea un tipo de propiedad; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PropertyTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SavePropertyTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _propertyTypeService.ExistsByNameAsync(dto.Name))
                {
                    return BadRequest("Ya existe un tipo de propiedad con ese nombre");
                }

                var created = await _propertyTypeService.AddAsync(new PropertyTypeDto
                {
                    Id = 0,
                    Name = dto.Name,
                    Description = dto.Description
                });

                if (created == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible crear el tipo de propiedad");
                }

                return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1" }, created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Actualiza un tipo de propiedad y devuelve el registro actualizado; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SavePropertyTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existing = await _propertyTypeService.GetById(id);

                if (existing == null)
                {
                    return NotFound("No existe un tipo de propiedad con el id indicado");
                }

                if (await _propertyTypeService.ExistsByNameAsync(dto.Name, id))
                {
                    return BadRequest("Ya existe un tipo de propiedad con ese nombre");
                }

                var updated = await _propertyTypeService.UpdateAsync(new PropertyTypeDto
                {
                    Id = id,
                    Name = dto.Name,
                    Description = dto.Description
                }, id);

                if (updated == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible actualizar el tipo de propiedad");
                }

                return Ok(updated);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Lista todos los tipos de propiedad</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyTypeDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var propertyTypes = await _propertyTypeService.GetAll();

                if (propertyTypes.Count == 0)
                {
                    return NoContent();
                }

                return Ok(propertyTypes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene un tipo de propiedad por su identificador</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var propertyType = await _propertyTypeService.GetById(id);

                if (propertyType == null)
                {
                    return NotFound("No existe un tipo de propiedad con el id indicado");
                }

                return Ok(propertyType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Elimina un tipo de propiedad junto con sus propiedades asociadas; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _propertyTypeService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound("No existe un tipo de propiedad con el id indicado");
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }
    }
}
