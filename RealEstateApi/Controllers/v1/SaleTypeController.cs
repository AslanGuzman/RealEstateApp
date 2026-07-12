using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.SaleType;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Developer")]
    public class SaleTypeController : BaseApiController
    {
        private readonly ISaleTypeService _saleTypeService;

        public SaleTypeController(ISaleTypeService saleTypeService)
        {
            _saleTypeService = saleTypeService;
        }

        /// <summary>Crea un tipo de venta; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SaleTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SaveSaleTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _saleTypeService.ExistsByNameAsync(dto.Name))
                {
                    return BadRequest("Ya existe un tipo de venta con ese nombre");
                }

                var created = await _saleTypeService.AddAsync(new SaleTypeDto
                {
                    Id = 0,
                    Name = dto.Name,
                    Description = dto.Description
                });

                if (created == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible crear el tipo de venta");
                }

                return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1" }, created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Actualiza un tipo de venta y devuelve el registro actualizado; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SaleTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveSaleTypeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existing = await _saleTypeService.GetById(id);

                if (existing == null)
                {
                    return NotFound("No existe un tipo de venta con el id indicado");
                }

                if (await _saleTypeService.ExistsByNameAsync(dto.Name, id))
                {
                    return BadRequest("Ya existe un tipo de venta con ese nombre");
                }

                var updated = await _saleTypeService.UpdateAsync(new SaleTypeDto
                {
                    Id = id,
                    Name = dto.Name,
                    Description = dto.Description
                }, id);

                if (updated == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible actualizar el tipo de venta");
                }

                return Ok(updated);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Lista todos los tipos de venta</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SaleTypeDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var saleTypes = await _saleTypeService.GetAll();

                if (saleTypes.Count == 0)
                {
                    return NoContent();
                }

                return Ok(saleTypes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene un tipo de venta por su identificador</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SaleTypeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var saleType = await _saleTypeService.GetById(id);

                if (saleType == null)
                {
                    return NotFound("No existe un tipo de venta con el id indicado");
                }

                return Ok(saleType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Elimina un tipo de venta junto con sus propiedades asociadas; solo disponible para Administradores</summary>
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
                var deleted = await _saleTypeService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound("No existe un tipo de venta con el id indicado");
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
