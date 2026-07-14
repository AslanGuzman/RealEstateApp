using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Application.ViewModels.Offer;
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
        private readonly IChatService _chatService;
        private readonly IOfferService _offerService;
        private readonly IAgentService _agentService;

        public ClientController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            IFavoriteService favoriteService,
            IChatService chatService,
            IOfferService offerService,
            IAgentService agentService)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _favoriteService = favoriteService;
            _chatService = chatService;
            _offerService = offerService;
            _agentService = agentService;
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

        public async Task<IActionResult> Detail(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null)
            {
                ViewBag.EmptyMessage = "La propiedad solicitada no existe o no se encuentra disponible.";
                return View(null);
            }

            await LoadDetailAsync(property);

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageViewModel vm)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(vm.PropertyId);

            if (property == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["ToastError"] = "Debe escribir un mensaje antes de enviarlo.";
                return RedirectToAction(nameof(Detail), new { id = vm.PropertyId });
            }

            var response = await _chatService.SendAsync(new SaveChatMessageDto
            {
                PropertyId = vm.PropertyId,
                ClientId = ClientId(),
                AgentId = property.AgentId,
                Content = vm.Content,
                SenderRole = Roles.Client
            });

            if (response.HasError)
            {
                TempData["ToastError"] = response.Error;
            }

            return RedirectToAction(nameof(Detail), new { id = vm.PropertyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOffer(SendOfferViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastError"] = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                    ?? "El monto de la oferta debe ser un valor numérico mayor que cero.";
                return RedirectToAction(nameof(Detail), new { id = vm.PropertyId });
            }

            var response = await _offerService.CreateOfferAsync(new Core.Application.Dtos.Offer.SaveOfferDto
            {
                PropertyId = vm.PropertyId,
                ClientId = ClientId(),
                Amount = vm.Amount!.Value
            });

            TempData[response.HasError ? "ToastError" : "Toast"] = response.HasError
                ? response.Error
                : "Su oferta fue enviada correctamente.";

            return RedirectToAction(nameof(Detail), new { id = vm.PropertyId });
        }

        private async Task LoadDetailAsync(PropertyDto property)
        {
            ViewBag.AgentContact = await _agentService.GetAgentContactAsync(property.AgentId);
            ViewBag.Conversation = await _chatService.GetConversationAsync(property.Id, ClientId());
            ViewBag.Offers = await _offerService.GetByPropertyAsync(property.Id, ClientId());
            ViewBag.HasPending = await _offerService.HasPendingOfferAsync(property.Id, ClientId());
            ViewBag.HasAccepted = await _offerService.HasAcceptedOfferAsync(property.Id);
        }

        private string ClientId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
