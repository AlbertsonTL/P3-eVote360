using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    private readonly IAdminDashboardService _dashboardService;

    public HomeController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var years = await _dashboardService.GetAvailableYearsAsync();
        ViewBag.Years = years;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetResumen(int year)
    {
        var result = await _dashboardService.GetResumenElectoralAsync(year);
        return View("Index", result.Data);
    }
}
