using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agent")]
    public class AgentHomeController : Controller
    {
        private readonly IPropertyService _propertyService;

        public AgentHomeController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index()
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
            {
                AgentId = agentId,
                OnlyAvailable = false
            });

            if (properties.Count == 0)
            {
                ViewBag.EmptyMessage = "Aún no tienes propiedades registradas.";
            }

            return View(properties);
        }
    }
}
