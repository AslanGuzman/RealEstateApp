using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Helpers;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Agent")]
    public class PropertyMaintenanceController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly ISaleTypeService _saleTypeService;
        private readonly IImprovementService _improvementService;

        public PropertyMaintenanceController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            ISaleTypeService saleTypeService,
            IImprovementService improvementService)
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _saleTypeService = saleTypeService;
            _improvementService = improvementService;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _propertyService.GetAllWithFiltersAsync(new PropertyFilterDto
            {
                AgentId = AgentId(),
                OnlyAvailable = true
            });

            if (properties.Count == 0)
            {
                ViewBag.EmptyMessage = "No tiene propiedades disponibles registradas en este momento.";
            }

            return View(properties);
        }

        public async Task<IActionResult> Create()
        {
            var catalogError = await EnsureCatalogsAsync();
            if (catalogError != null)
            {
                TempData["ToastError"] = catalogError;
                return RedirectToAction(nameof(Index));
            }

            await LoadCatalogsAsync();
            return View(new SavePropertyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavePropertyViewModel vm)
        {
            var catalogError = await EnsureCatalogsAsync();
            if (catalogError != null)
            {
                TempData["ToastError"] = catalogError;
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await LoadCatalogsAsync();
                return View(vm);
            }

            var folderId = Guid.NewGuid().ToString();
            var imagePaths = UploadImages(vm.Images, folderId);

            var created = await _propertyService.CreateAsync(new SavePropertyDto
            {
                PropertyTypeId = vm.PropertyTypeId!.Value,
                SaleTypeId = vm.SaleTypeId!.Value,
                Price = vm.Price!.Value,
                Description = vm.Description!,
                LandSize = vm.LandSize!.Value,
                Rooms = vm.Rooms!.Value,
                Bathrooms = vm.Bathrooms!.Value,
                AgentId = AgentId(),
                Images = imagePaths,
                ImprovementIds = vm.ImprovementIds
            });

            if (created == null)
            {
                FileManager.Delete(folderId, "Properties");
                await LoadCatalogsAsync();
                ModelState.AddModelError(string.Empty, "No fue posible crear la propiedad. Intente nuevamente.");
                return View(vm);
            }

            TempData["Toast"] = "La propiedad fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null || property.AgentId != AgentId())
            {
                TempData["ToastError"] = "No tiene permisos para modificar esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            if (property.Status == PropertyStatus.Sold)
            {
                TempData["ToastError"] = "No se puede modificar una propiedad que ya fue vendida.";
                return RedirectToAction(nameof(Index));
            }

            await LoadCatalogsAsync();

            return View(new SavePropertyViewModel
            {
                Id = property.Id,
                Code = property.Code,
                PropertyTypeId = property.PropertyTypeId,
                SaleTypeId = property.SaleTypeId,
                Price = property.Price,
                Description = property.Description,
                LandSize = property.LandSize,
                Rooms = property.Rooms,
                Bathrooms = property.Bathrooms,
                ImprovementIds = property.Improvements.Select(i => i.Id).ToList(),
                CurrentImages = property.Images
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SavePropertyViewModel vm)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(vm.Id);

            if (property == null || property.AgentId != AgentId())
            {
                TempData["ToastError"] = "No tiene permisos para modificar esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            if (property.Status == PropertyStatus.Sold)
            {
                TempData["ToastError"] = "No se puede modificar una propiedad que ya fue vendida.";
                return RedirectToAction(nameof(Index));
            }

            vm.CurrentImages = property.Images;

            if (!ModelState.IsValid)
            {
                await LoadCatalogsAsync();
                return View(vm);
            }

            var newImages = vm.Images?.Where(i => i != null && i.Length > 0).ToList() ?? [];
            List<string> imagePaths = [];

            if (!vm.KeepCurrentImages)
            {
                DeletePropertyImages(property.Images);
            }

            if (newImages.Count > 0)
            {
                imagePaths = UploadImages(newImages, Guid.NewGuid().ToString());
            }

            await _propertyService.UpdateAsync(new SavePropertyDto
            {
                PropertyTypeId = vm.PropertyTypeId!.Value,
                SaleTypeId = vm.SaleTypeId!.Value,
                Price = vm.Price!.Value,
                Description = vm.Description!,
                LandSize = vm.LandSize!.Value,
                Rooms = vm.Rooms!.Value,
                Bathrooms = vm.Bathrooms!.Value,
                AgentId = AgentId(),
                Images = imagePaths,
                ImprovementIds = vm.ImprovementIds,
                KeepCurrentImages = vm.KeepCurrentImages
            }, vm.Id);

            TempData["Toast"] = "La propiedad fue actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null || property.AgentId != AgentId())
            {
                TempData["ToastError"] = "No tiene permisos para eliminar esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            if (property.Status == PropertyStatus.Sold)
            {
                TempData["ToastError"] = "No se puede eliminar una propiedad que ya fue vendida.";
                return RedirectToAction(nameof(Index));
            }

            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _propertyService.GetByIdWithDetailsAsync(id);

            if (property == null || property.AgentId != AgentId())
            {
                TempData["ToastError"] = "No tiene permisos para eliminar esta propiedad.";
                return RedirectToAction(nameof(Index));
            }

            var deleted = await _propertyService.DeleteAsync(id, AgentId());

            if (!deleted)
            {
                TempData["ToastError"] = "No se puede eliminar una propiedad que ya fue vendida.";
                return RedirectToAction(nameof(Index));
            }

            DeletePropertyImages(property.Images);
            TempData["Toast"] = "La propiedad fue eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> EnsureCatalogsAsync()
        {
            if ((await _propertyTypeService.GetAll()).Count == 0)
            {
                return "No existen tipos de propiedades registrados. Debe crear al menos un tipo de propiedad antes de registrar una propiedad.";
            }

            if ((await _saleTypeService.GetAll()).Count == 0)
            {
                return "No existen tipos de ventas registrados. Debe crear al menos un tipo de venta antes de registrar una propiedad.";
            }

            if ((await _improvementService.GetAll()).Count == 0)
            {
                return "No existen mejoras registradas. Debe crear al menos una mejora antes de registrar una propiedad.";
            }

            return null;
        }

        private async Task LoadCatalogsAsync()
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAll();
            ViewBag.SaleTypes = await _saleTypeService.GetAll();
            ViewBag.Improvements = await _improvementService.GetAll();
        }

        private static List<string> UploadImages(List<Microsoft.AspNetCore.Http.IFormFile>? images, string folderId)
        {
            List<string> paths = [];

            foreach (var image in images ?? [])
            {
                if (image == null || image.Length == 0)
                {
                    continue;
                }

                var path = FileManager.Upload(image, folderId, "Properties");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    paths.Add(path);
                }
            }

            return paths;
        }

        private static void DeletePropertyImages(List<string> imagePaths)
        {
            var first = imagePaths.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(first))
            {
                return;
            }

            var parts = first.Split('/');
            if (parts.Length >= 3)
            {
                FileManager.Delete(parts[2], "Properties");
            }
        }

        private string AgentId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }
    }
}
