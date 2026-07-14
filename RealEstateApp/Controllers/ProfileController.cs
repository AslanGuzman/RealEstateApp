using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Helpers;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agent")]
    public class ProfileController : Controller
    {
        private readonly IAccountServiceForWebApp _accountService;

        public ProfileController(IAccountServiceForWebApp accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _accountService.GetUserProfileAsync(UserId());

            if (profile == null)
            {
                return RedirectToAction("Index", "AgentHome");
            }

            return View(new UserProfileViewModel
            {
                Name = profile.Name,
                LastName = profile.LastName,
                Phone = profile.Phone ?? "",
                CurrentImage = profile.ProfileImage
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string? imagePath = null;

            if (vm.Photo != null && vm.Photo.Length > 0)
            {
                imagePath = FileManager.Upload(vm.Photo, UserId(), "Users", true, vm.CurrentImage);
            }

            await _accountService.UpdateUserProfileAsync(UserId(), new UserProfileDto
            {
                Name = vm.Name,
                LastName = vm.LastName,
                Phone = vm.Phone,
                ProfileImage = imagePath
            });

            TempData["Toast"] = "Su perfil fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private string UserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
