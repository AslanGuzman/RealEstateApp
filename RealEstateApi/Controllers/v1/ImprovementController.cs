using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Improvement;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Developer")]
    public class ImprovementController : BaseApiController
    {
        private readonly IImprovementService _improvementService;

        public ImprovementController(IImprovementService improvementService)
        {
            _improvementService = improvementService;
        }

        /// <summary>Crea una mejora; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImprovementDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SaveImprovementDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _improvementService.ExistsByNameAsync(dto.Name))
                {
                    return BadRequest("Ya existe una mejora con ese nombre");
                }

                var created = await _improvementService.AddAsync(new ImprovementDto
                {
                    Id = 0,
                    Name = dto.Name,
                    Description = dto.Description
                });

                if (created == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible crear la mejora");
                }

                return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1" }, created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Actualiza una mejora y devuelve el registro actualizado; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImprovementDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveImprovementDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existing = await _improvementService.GetById(id);

                if (existing == null)
                {
                    return NotFound("No existe una mejora con el id indicado");
                }

                if (await _improvementService.ExistsByNameAsync(dto.Name, id))
                {
                    return BadRequest("Ya existe una mejora con ese nombre");
                }

                var updated = await _improvementService.UpdateAsync(new ImprovementDto
                {
                    Id = id,
                    Name = dto.Name,
                    Description = dto.Description
                }, id);

                if (updated == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible actualizar la mejora");
                }

                return Ok(updated);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Lista todas las mejoras</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ImprovementDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var improvements = await _improvementService.GetAll();

                if (improvements.Count == 0)
                {
                    return NoContent();
                }

                return Ok(improvements);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene una mejora por su identificador</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImprovementDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var improvement = await _improvementService.GetById(id);

                if (improvement == null)
                {
                    return NotFound("No existe una mejora con el id indicado");
                }

                return Ok(improvement);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Elimina una mejora y sus relaciones con propiedades, sin eliminar las propiedades; solo disponible para Administradores</summary>
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
                var existing = await _improvementService.GetById(id);

                if (existing == null)
                {
                    return NotFound("No existe una mejora con el id indicado");
                }

                var deleted = await _improvementService.DeleteAsync(id);

                if (!deleted)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No fue posible eliminar la mejora");
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
