using BusinessLayer.Services.CargoService;
using DtoLayer.DTOs.CargoPriceDtos;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoPriceController : Controller
    {
        private readonly ICargoPriceService _cargoPriceService;

        public CargoPriceController(ICargoPriceService cargoPriceService)
        {
            _cargoPriceService = cargoPriceService;
        }

      
        public async Task<IActionResult> Index()
        {
            var values = await _cargoPriceService.GetActiveCargoPriceAsync();

            return View(values);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCargoPriceDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _cargoPriceService.CreateCargoPriceAsync(dto);

                TempData["Success"] = "Fiyat tarifesi başarıyla oluşturuldu.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

      
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {

            var dto = await _cargoPriceService.GetCargoPriceByIdAsync(id);

            if (dto == null)
                return NotFound();

            return View(dto);

        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, UpdateCargoPriceDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(dto);
            }

            try
            {
                await _cargoPriceService.UpdateCargoPriceAsync(id, dto);

                TempData["Success"] = "Fiyat tarifesi başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Id = id;
                return View(dto);
            }
        }
    }
}
