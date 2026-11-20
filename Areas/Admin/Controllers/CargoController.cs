using BusinessLayer.Services.Cargos;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoController : Controller
    {
        private readonly ICargoService _cargoService;

        public CargoController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var cargos = await _cargoService.GetAllCargosAsync(search);

            ViewBag.Search = search;

            return View(cargos);
        }
    }
}
