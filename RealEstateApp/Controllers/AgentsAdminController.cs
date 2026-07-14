using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgentsAdminController : Controller
    {
        private readonly IUserAdminService _userAdminService;
        private readonly IAgentService _agentService;

        public AgentsAdminController(IUserAdminService userAdminService, IAgentService agentService)
        {
            _userAdminService = userAdminService;
            _agentService = agentService;
        }

        public async Task<IActionResult> Index()
        {
            var agents = await _userAdminService.GetByRoleAsync(Roles.Agent);

            if (agents.Count == 0)
            {
                ViewBag.EmptyMessage = "No hay agentes registrados en este momento.";
            }

            return View(agents);
        }

        public async Task<IActionResult> ToggleStatus(string id)
        {
            var agent = await _userAdminService.GetByIdAsync(id, Roles.Agent);

            if (agent == null)
            {
                TempData["ToastError"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(agent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusConfirmed(string id, bool isActive)
        {
            var changed = await _agentService.ChangeStatusAsync(id, isActive);

            TempData[changed ? "Toast" : "ToastError"] = changed
                ? (isActive ? "El agente fue activado correctamente." : "El agente fue inactivado correctamente.")
                : "El agente seleccionado no existe.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var agent = await _userAdminService.GetByIdAsync(id, Roles.Agent);

            if (agent == null)
            {
                TempData["ToastError"] = "El agente seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(agent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var deleted = await _agentService.DeleteAgentAsync(id);

            TempData[deleted ? "Toast" : "ToastError"] = deleted
                ? "El agente fue eliminado correctamente."
                : "No fue posible eliminar el agente. Intente nuevamente más tarde.";

            return RedirectToAction(nameof(Index));
        }
    }
}
