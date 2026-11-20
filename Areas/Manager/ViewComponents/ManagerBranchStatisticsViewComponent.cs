using BusinessLayer.Services.Cargos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Manager.ViewComponents
{
    public class ManagerBranchStatisticsViewComponent : ViewComponent
    {
        private readonly ICargoService _cargoService;
        private readonly UserManager<AppUser> _userManager;

        public ManagerBranchStatisticsViewComponent(ICargoService cargoService, UserManager<AppUser> userManager)
        {
            _cargoService = cargoService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return Content("Kullanıcı bulunamadı.");


            var statistic = await _cargoService.GetManagerBranchStatisticAsync(user.Id);
            return View(statistic);
            
        }
    }
}
