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
    public class AdministratorsController : Controller
    {
        private readonly IUserAdminService _userAdminService;

        public AdministratorsController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _userAdminService.GetByRoleAsync(Roles.Admin);
            ViewBag.CurrentUserId = CurrentUserId();
            return View(admins);
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

            var response = await _userAdminService.CreateAsync(ToDto(vm), Roles.Admin);

            if (response.HasError)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(vm);
            }

            TempData["Toast"] = "El administrador fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == CurrentUserId())
            {
                TempData["ToastError"] = "No puede editar su propio usuario desde este mantenimiento.";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _userAdminService.GetByIdAsync(id, Roles.Admin);

            if (admin == null)
            {
                TempData["ToastError"] = "El administrador seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SaveUserViewModel
            {
                Id = admin.Id,
                Name = admin.Name,
                LastName = admin.LastName,
                IdentityCard = admin.IdentityCard ?? "",
                Email = admin.Email ?? "",
                UserName = admin.UserName ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveUserViewModel vm)
        {
            if (vm.Id == CurrentUserId())
            {
                TempData["ToastError"] = "No puede editar su propio usuario desde este mantenimiento.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var response = await _userAdminService.UpdateAsync(ToDto(vm), Roles.Admin);

            if (response.HasError)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(vm);
            }

            TempData["Toast"] = "El administrador fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(string id)
        {
            var admin = await _userAdminService.GetByIdAsync(id, Roles.Admin);

            if (admin == null)
            {
                TempData["ToastError"] = "El administrador seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusConfirmed(string id, bool isActive)
        {
            var response = await _userAdminService.ChangeStatusAsync(id, isActive, CurrentUserId(), Roles.Admin);

            if (response.HasError)
            {
                TempData["ToastError"] = response.Error;
            }
            else
            {
                TempData["Toast"] = isActive
                    ? "El administrador fue activado correctamente."
                    : "El administrador fue inactivado correctamente.";
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
