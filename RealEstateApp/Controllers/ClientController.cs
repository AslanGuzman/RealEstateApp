using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IFavoriteService _favoriteService;

        public ClientController(IPropertyService propertyService, IPropertyTypeService propertyTypeService, IFavoriteService favoriteService)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index(PropertyFilterViewModel filters)
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAll();
            ViewData["FavoriteIds"] = await _favoriteService.GetFavoriteIdsAsync(ClientId());

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int propertyId, string? returnUrl)
        {
            var added = await _favoriteService.ToggleAsync(ClientId(), propertyId);

            TempData["Toast"] = added
                ? "La propiedad fue agregada a sus favoritas correctamente."
                : "La propiedad fue eliminada de sus favoritas correctamente.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Favorites()
        {
            var favorites = await _favoriteService.GetClientFavoritesAsync(ClientId(), new PropertyFilterDto { OnlyAvailable = true });

            ViewData["FavoriteIds"] = await _favoriteService.GetFavoriteIdsAsync(ClientId());

            if (favorites.Count == 0)
            {
                ViewBag.EmptyMessage = "No tiene propiedades favoritas disponibles en este momento.";
            }

            return View(favorites);
        }

        private string ClientId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
