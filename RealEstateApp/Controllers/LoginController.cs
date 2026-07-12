using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServiceForWebApp _accountService;

        public LoginController(IAccountServiceForWebApp accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToRoleHome();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var response = await _accountService.AuthenticateAsync(new LoginDto
            {
                UserName = vm.UserName,
                Password = vm.Password
            });

            if (response.HasError)
            {
                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault() ?? "Los datos de acceso son inválidos");
                return View(vm);
            }

            var roles = response.Roles ?? [];

            if (roles.Contains(Roles.Admin.ToString()))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (roles.Contains(Roles.Agent.ToString()))
            {
                return RedirectToAction("Index", "AgentHome");
            }

            return RedirectToAction("Index", "Client");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                ViewBag.Confirmed = false;
                ViewBag.Message = "El enlace de activación no es válido.";
                return View();
            }

            var userName = await _accountService.ConfirmEmailAsync(userId, token);

            if (userName == null)
            {
                ViewBag.Confirmed = false;
                ViewBag.Message = "No fue posible activar la cuenta. El enlace no es válido o ya expiró.";
                return View();
            }

            ViewBag.Confirmed = true;
            ViewBag.Message = $"La cuenta {userName} ha sido activada correctamente. Ya puede iniciar sesión.";
            return View();
        }

        private IActionResult RedirectToRoleHome()
        {
            if (User.IsInRole(Roles.Admin.ToString()))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (User.IsInRole(Roles.Agent.ToString()))
            {
                return RedirectToAction("Index", "AgentHome");
            }

            if (User.IsInRole(Roles.Client.ToString()))
            {
                return RedirectToAction("Index", "Client");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
