using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DevelopersController : Controller
    {
        private readonly IUserAdminService _userAdminService;

        public DevelopersController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        public async Task<IActionResult> Index()
        {
            var developers = await _userAdminService.GetByRoleAsync(Roles.Developer);
            return View(developers);
        }

        public IActionResult Create()
        {
            return View(new SaveUserViewModel { Name = null!, LastName = null!, IdentityCard = null!, Email = null!, UserName = null! });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var response = await _userAdminService.CreateAsync(ToDto(vm), Roles.Developer);

            if (response.HasError)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(vm);
            }

            TempData["Toast"] = "El desarrollador fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var developer = await _userAdminService.GetByIdAsync(id, Roles.Developer);

            if (developer == null)
            {
                TempData["ToastError"] = "El desarrollador seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SaveUserViewModel
            {
                Id = developer.Id,
                Name = developer.Name,
                LastName = developer.LastName,
                IdentityCard = developer.IdentityCard ?? "",
                Email = developer.Email ?? "",
                UserName = developer.UserName ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var response = await _userAdminService.UpdateAsync(ToDto(vm), Roles.Developer);

            if (response.HasError)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(vm);
            }

            TempData["Toast"] = "El desarrollador fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(string id)
        {
            var developer = await _userAdminService.GetByIdAsync(id, Roles.Developer);

            if (developer == null)
            {
                TempData["ToastError"] = "El desarrollador seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(developer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusConfirmed(string id, bool isActive)
        {
            var response = await _userAdminService.ChangeStatusAsync(id, isActive, CurrentUserId(), Roles.Developer);

            if (response.HasError)
            {
                TempData["ToastError"] = response.Error;
            }
            else
            {
                TempData["Toast"] = isActive
                    ? "El desarrollador fue activado correctamente."
                    : "El desarrollador fue inactivado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static SaveUserDto ToDto(SaveUserViewModel vm)
        {
            return new SaveUserDto
            {
                Id = vm.Id,
                Name = vm.Name,
                LastName = vm.LastName,
                IdentityCard = vm.IdentityCard,
                Email = vm.Email,
                UserName = vm.UserName,
                Password = vm.Password
            };
        }

        private string CurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
