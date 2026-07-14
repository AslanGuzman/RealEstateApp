using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserAdminService _userAdminService;

        public AdminController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _userAdminService.GetDashboardAsync();
            return View(dashboard);
        }
    }
}
