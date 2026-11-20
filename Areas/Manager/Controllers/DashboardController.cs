using BusinessLayer.Services.Cargos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyAcademy_CargoTrack.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DashboardController : Controller
    {
        private readonly ICargoService _cargoService;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(ICargoService cargoService, UserManager<AppUser> userManager)
        {
            _cargoService = cargoService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
           
            return View();
        }

       
    }
}
