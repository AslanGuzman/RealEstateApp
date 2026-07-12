using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IAgentService _agentService;

        public HomeController(IPropertyService propertyService, IPropertyTypeService propertyTypeService, IAgentService agentService)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _agentService = agentService;
        }

        public async Task<IActionResult> Index(PropertyFilterViewModel filters)
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAll();

            if (!ModelState.IsValid)
            {
                return View(new List<PropertyDto>());
            }

            if (!string.IsNullOrWhiteSpace(filters.Code))
            {
                var property = await _propertyService.GetByCodeAsync(filters.Code.Trim());

                if (property == null || property.Status != PropertyStatus.Available)
                {
                    ViewBag.EmptyMessage = "No se encontró ninguna propiedad disponible con el código ingresado.";
                    return View(new List<PropertyDto>());
                }

                return View(new List<PropertyDto> { property });
            }

            var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
            {
                PropertyTypeId = filters.PropertyTypeId,
                MinPrice = filters.MinPrice,
                MaxPrice = filters.MaxPrice,
                Rooms = filters.Rooms,
                Bathrooms = filters.Bathrooms
            });

            if (properties.Count == 0)
            {
                ViewBag.EmptyMessage = "No se encontraron propiedades disponibles con los filtros seleccionados.";
            }

            return View(properties);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null || property.Status != PropertyStatus.Available)
            {
                ViewBag.EmptyMessage = "La propiedad solicitada no existe o no se encuentra disponible.";
                return View(null);
            }

            ViewBag.AgentContact = await _agentService.GetAgentContactAsync(property.AgentId);

            return View(property);
        }
    }
}
