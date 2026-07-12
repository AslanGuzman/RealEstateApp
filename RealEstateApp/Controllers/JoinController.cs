using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Helpers;

namespace RealEstateApp.Controllers
{
    public class JoinController : Controller
    {
        private readonly IAccountServiceForWebApp _accountService;

        public JoinController(IAccountServiceForWebApp accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = new RegisterRequestDto
            {
                Name = vm.Name,
                LastName = vm.LastName,
                Phone = vm.Phone,
                UserName = vm.UserName,
                Email = vm.Email,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword
            };

            var origin = $"{Request.Scheme}://{Request.Host}";

            var response = vm.UserType == Roles.Agent
                ? await _accountService.RegisterAgentAsync(dto)
                : await _accountService.RegisterClientAsync(dto, origin);

            if (response.HasError || response.Id == null)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                if (response.Errors.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "No fue posible completar el registro. Intente nuevamente más tarde.");
                }

                return View(vm);
            }

            var imagePath = FileManager.Upload(vm.Photo, response.Id, "Users");

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                await _accountService.SetProfileImageAsync(response.Id, imagePath);
            }

            TempData["RegisterMessage"] = vm.UserType == Roles.Agent
                ? "Su cuenta de agente ha sido creada correctamente. Un administrador debe activar su usuario antes de que pueda iniciar sesión."
                : "Su cuenta ha sido creada correctamente. Revise su correo electrónico para activar su usuario.";

            return RedirectToAction("Index", "Login");
        }
    }
}
