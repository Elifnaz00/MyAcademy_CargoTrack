using BusinessLayer.Services.Admins;
using EntityLayer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = RoleNames.Admin)]
    public class DashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public DashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardDto = await _adminDashboardService.GetDashboardAsync();

            return View(dashboardDto);
        }
    }
}
