using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Domain.Common.Enums;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agent")]
    public class AgentHomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IChatService _chatService;
        private readonly IOfferService _offerService;
        private readonly IAccountServiceForWebApp _accountService;

        public AgentHomeController(
            IPropertyService propertyService,
            IChatService chatService,
            IOfferService offerService,
            IAccountServiceForWebApp accountService)
        {
            _propertyService = propertyService;
            _chatService = chatService;
            _offerService = offerService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
            {
                AgentId = AgentId(),
                OnlyAvailable = false
            });

            if (properties.Count == 0)
            {
                ViewBag.EmptyMessage = "No tiene propiedades registradas en este momento.";
            }

            return View(properties);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var property = await OwnedPropertyAsync(id);

            if (property == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var conversations = await _chatService.GetConversationSummariesAsync(id);
            var offerSummaries = await _offerService.GetClientSummariesAsync(id);

            var names = await _accountService.GetUserNamesAsync(
                conversations.Select(c => c.ClientId).Union(offerSummaries.Select(o => o.ClientId)).Distinct().ToList());

            foreach (var conversation in conversations)
            {
                conversation.ClientName = names.GetValueOrDefault(conversation.ClientId, "Cliente");
            }

            foreach (var summary in offerSummaries)
            {
                summary.ClientName = names.GetValueOrDefault(summary.ClientId, "Cliente");
            }

            ViewBag.Conversations = conversations;
            ViewBag.OfferSummaries = offerSummaries;

            return View(property);
        }

        public async Task<IActionResult> Conversation(int propertyId, string clientId)
        {
            var property = await OwnedPropertyAsync(propertyId);

            if (property == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Property = property;
            ViewBag.ClientId = clientId;
            var names = await _accountService.GetUserNamesAsync([clientId]);
            ViewBag.ClientName = names.GetValueOrDefault(clientId, "Cliente");
            ViewBag.Messages = await _chatService.GetConversationAsync(propertyId, clientId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(SendMessageViewModel vm)
        {
            var property = await OwnedPropertyAsync(vm.PropertyId);

            if (property == null || string.IsNullOrWhiteSpace(vm.ClientId))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["ToastError"] = "Debe escribir un mensaje antes de enviarlo.";
                return RedirectToAction(nameof(Conversation), new { propertyId = vm.PropertyId, clientId = vm.ClientId });
            }

            var response = await _chatService.SendAsync(new SaveChatMessageDto
            {
                PropertyId = vm.PropertyId,
                ClientId = vm.ClientId,
                AgentId = AgentId(),
                Content = vm.Content,
                SenderRole = Roles.Agent
            });

            if (response.HasError)
            {
                TempData["ToastError"] = response.Error;
            }

            return RedirectToAction(nameof(Conversation), new { propertyId = vm.PropertyId, clientId = vm.ClientId });
        }

        public async Task<IActionResult> Offers(int propertyId, string clientId)
        {
            var property = await OwnedPropertyAsync(propertyId);

            if (property == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Property = property;
            var names = await _accountService.GetUserNamesAsync([clientId]);
            ViewBag.ClientName = names.GetValueOrDefault(clientId, "Cliente");
            ViewBag.Offers = await _offerService.GetByPropertyAsync(propertyId, clientId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOffer(int offerId, int propertyId, string clientId)
        {
            var response = await _offerService.AcceptOfferAsync(offerId, AgentId());

            SetOfferToast(response, "La oferta fue aceptada correctamente y la propiedad fue marcada como vendida.");

            return RedirectToAction(nameof(Offers), new { propertyId, clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOffer(int offerId, int propertyId, string clientId)
        {
            var response = await _offerService.RejectOfferAsync(offerId, AgentId());

            SetOfferToast(response, "La oferta fue rechazada correctamente.");

            return RedirectToAction(nameof(Offers), new { propertyId, clientId });
        }

        private void SetOfferToast(Core.Application.Dtos.Common.OperationResponseDto response, string successMessage)
        {
            if (response.HasError)
            {
                TempData["ToastError"] = response.Error == "La oferta no existe o no está pendiente"
                    ? "Esta oferta ya fue respondida."
                    : response.Error;
            }
            else
            {
                TempData["Toast"] = successMessage;
            }
        }

        private async Task<PropertyDto?> OwnedPropertyAsync(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null || property.AgentId != AgentId())
            {
                return null;
            }

            return property;
        }

        private string AgentId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
