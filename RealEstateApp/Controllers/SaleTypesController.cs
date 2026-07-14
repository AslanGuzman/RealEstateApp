using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.SaleType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.SaleType;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SaleTypesController : Controller
    {
        private readonly ISaleTypeService _saleTypeService;

        public SaleTypesController(ISaleTypeService saleTypeService)
        {
            _saleTypeService = saleTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _saleTypeService.GetAllWithCountAsync();

            if (items.Count == 0)
            {
                ViewBag.EmptyMessage = "No existen tipos de ventas registrados.";
            }

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new SaveSaleTypeViewModel { Name = null!, Description = null! });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveSaleTypeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _saleTypeService.ExistsByNameAsync(vm.Name))
            {
                ModelState.AddModelError(string.Empty, "Ya existe un tipo de venta con ese nombre.");
                return View(vm);
            }

            await _saleTypeService.AddAsync(new SaleTypeDto { Id = 0, Name = vm.Name, Description = vm.Description });

            TempData["Toast"] = "El tipo de venta fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _saleTypeService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "El tipo de venta seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SaveSaleTypeViewModel { Id = item.Id, Name = item.Name, Description = item.Description ?? "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveSaleTypeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _saleTypeService.ExistsByNameAsync(vm.Name, vm.Id))
            {
                ModelState.AddModelError(string.Empty, "Ya existe un tipo de venta con ese nombre.");
                return View(vm);
            }

            await _saleTypeService.UpdateAsync(new SaleTypeDto { Id = vm.Id, Name = vm.Name, Description = vm.Description }, vm.Id);

            TempData["Toast"] = "El tipo de venta fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _saleTypeService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "El tipo de venta seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _saleTypeService.DeleteAsync(id);

            TempData["Toast"] = "El tipo de venta fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
