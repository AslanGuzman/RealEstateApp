using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    public class AgentController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IPropertyService _propertyService;

        public AgentController(IAgentService agentService, IPropertyService propertyService)
        {
            _agentService = agentService;
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index(string? name)
        {
            var agents = await _agentService.GetAllAgentsAsync(true, name);

            if (agents.Count == 0)
            {
                ViewBag.EmptyMessage = string.IsNullOrWhiteSpace(name)
                    ? "No hay agentes activos registrados en este momento."
                    : "No se encontraron agentes activos con el nombre ingresado.";
            }

            ViewBag.SearchName = name;

            return View(agents);
        }

        public async Task<IActionResult> Properties(string id)
        {
            var agent = await _agentService.GetAgentByIdAsync(id);

            if (agent == null || !agent.IsActive)
            {
                ViewBag.EmptyMessage = "El agente solicitado no existe o no se encuentra disponible.";
                return View(new List<PropertyDto>());
            }

            ViewBag.AgentName = $"{agent.Name} {agent.LastName}";

            var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
            {
                AgentId = id,
                OnlyAvailable = true
            });

            if (properties.Count == 0)
            {
                ViewBag.EmptyMessage = "Este agente no tiene propiedades disponibles en este momento.";
            }

            return View(properties);
        }
    }
}
