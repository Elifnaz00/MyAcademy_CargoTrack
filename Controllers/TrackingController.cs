using BusinessLayer.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ICargoService _cargoService;

        public TrackingController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        public async Task<IActionResult> Index()
        {
            var recentCargos = await _cargoService.GetRecentCargosAsync();
            return View(recentCargos);
        }


        public async Task<IActionResult> Detail(Guid id)
        {
           var cargoDetail = await _cargoService.GetCargoDetailAsync(id);
            if(cargoDetail is null)
                return NotFound();
            return View(cargoDetail);
        }
    }
}
