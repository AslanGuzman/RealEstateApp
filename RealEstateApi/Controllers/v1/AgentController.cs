using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Developer")]
    public class AgentController : BaseApiController
    {
        private readonly IAgentService _agentService;
        private readonly IPropertyService _propertyService;

        public AgentController(IAgentService agentService, IPropertyService propertyService)
        {
            _agentService = agentService;
            _propertyService = propertyService;
        }

        /// <summary>Lista todos los agentes registrados</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AgentDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var agents = await _agentService.GetAllAgentsAsync(false);

                if (agents.Count == 0)
                {
                    return NoContent();
                }

                return Ok(agents);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Obtiene un agente por su identificador, validando que el usuario tenga rol de agente</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Debe indicar el id del agente");
                }

                var agent = await _agentService.GetAgentByIdAsync(id);

                if (agent == null)
                {
                    return NotFound("No existe un agente con el id indicado");
                }

                return Ok(agent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error procesando la solicitud");
            }
        }

        /// <summary>Lista las propiedades del agente indicado</summary>
        [HttpGet("{id}/properties")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAgentProperty(string id)
        {
            try
            {
                var agent = await _agentService.GetAgentByIdAsync(id);

                if (agent == null)
                {
                    return NotFound("No existe un agente con el id indicado");
                }

                var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
                {
                    AgentId = id,
                    OnlyAvailable = false
                });

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

        /// <summary>Activa o inactiva un agente; solo disponible para Administradores</summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeAgentStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(ModelState);
                }

                var changed = await _agentService.ChangeStatusAsync(id, dto.Status);

                if (!changed)
                {
                    return NotFound("No existe un agente con el id indicado");
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
