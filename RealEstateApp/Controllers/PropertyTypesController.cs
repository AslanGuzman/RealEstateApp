using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.PropertyType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.PropertyType;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PropertyTypesController : Controller
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyTypesController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _propertyTypeService.GetAllWithCountAsync();

            if (items.Count == 0)
            {
                ViewBag.EmptyMessage = "No existen tipos de propiedades registrados.";
            }

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new SavePropertyTypeViewModel { Name = null!, Description = null! });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _propertyTypeService.ExistsByNameAsync(vm.Name))
            {
                ModelState.AddModelError(string.Empty, "Ya existe un tipo de propiedad con ese nombre.");
                return View(vm);
            }

            await _propertyTypeService.AddAsync(new PropertyTypeDto { Id = 0, Name = vm.Name, Description = vm.Description });

            TempData["Toast"] = "El tipo de propiedad fue creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _propertyTypeService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "El tipo de propiedad seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SavePropertyTypeViewModel { Id = item.Id, Name = item.Name, Description = item.Description ?? "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePropertyTypeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (await _propertyTypeService.ExistsByNameAsync(vm.Name, vm.Id))
            {
                ModelState.AddModelError(string.Empty, "Ya existe un tipo de propiedad con ese nombre.");
                return View(vm);
            }

            await _propertyTypeService.UpdateAsync(new PropertyTypeDto { Id = vm.Id, Name = vm.Name, Description = vm.Description }, vm.Id);

            TempData["Toast"] = "El tipo de propiedad fue actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _propertyTypeService.GetById(id);

            if (item == null)
            {
                TempData["ToastError"] = "El tipo de propiedad seleccionado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _propertyTypeService.DeleteAsync(id);

            TempData["Toast"] = "El tipo de propiedad fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
