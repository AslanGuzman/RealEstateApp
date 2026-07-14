using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Improvement;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ImprovementsController : Controller
    {
        private readonly IImprovementService _improvementService;

        public ImprovementsController(IImprovementService improvementService)
        {
            _improvementService = improvementService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _improvementService.GetAllWithCountAsync();

            if (items.Count == 0)
            {
                ViewBag.EmptyMessage = "No existen mejoras registradas.";
            }

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new SaveImprovementViewModel { Name = null!, Description = null! });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveImprovementViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _improvementService.ExistsByNameAsync(vm.Name))
            {
                ModelState.AddModelError(string.Empty, "Ya existe una mejora con ese nombre.");
                return View(vm);
            }

            await _improvementService.AddAsync(new ImprovementDto { Id = 0, Name = vm.Name, Description = vm.Description });

            TempData["Toast"] = "La mejora fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _improvementService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "La mejora seleccionada no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SaveImprovementViewModel { Id = item.Id, Name = item.Name, Description = item.Description ?? "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveImprovementViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _improvementService.ExistsByNameAsync(vm.Name, vm.Id))
            {
                ModelState.AddModelError(string.Empty, "Ya existe una mejora con ese nombre.");
                return View(vm);
            }

            await _improvementService.UpdateAsync(new ImprovementDto { Id = vm.Id, Name = vm.Name, Description = vm.Description }, vm.Id);

            TempData["Toast"] = "La mejora fue actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _improvementService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "La mejora seleccionada no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _improvementService.DeleteAsync(id);

            TempData["Toast"] = "La mejora fue eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
