using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

public class HomeController : Controller
{
    private readonly IPropertyService _propertyService;

    public HomeController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var properties = await _propertyService.GetAllAvailablePropertiesAsync();
        return View(properties);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var property = await _propertyService.GetPropertyByIdAsync(id);
        return View(property);
    }
}