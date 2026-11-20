using BusinessLayer.Services.Cities;
using BusinessLayer.Services.TransferCenters;
using DtoLayer.DTOs.TransferCenterDtos;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransferCenterController : Controller
    {
        private readonly ITransferCenterService _transferCenterService;
        private readonly ICityService _cityService;

        public TransferCenterController(
          ITransferCenterService transferCenterService,
          ICityService cityService)
        {
            _transferCenterService = transferCenterService;
            _cityService = cityService;
        }


        public async Task<IActionResult> Index()
        {
            var values = await _transferCenterService.GetAllTransferCentersAsync();

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCitiesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransferCenterCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            try
            {
                await _transferCenterService.CreateTransferCenterAsync(dto);

                TempData["Success"] = "Transfer merkezi başarıyla oluşturuldu.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadCitiesAsync();

                return View(dto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var value = await _transferCenterService.GetTransferCenterByIdAsync(id);

            if (value == null)
                return NotFound();

            await LoadCitiesAsync();

            return View(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransferCenterUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            try
            {
                await _transferCenterService.UpdateTransferCenterAsync(dto);

                TempData["Success"] = "Transfer merkezi başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadCitiesAsync();

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _transferCenterService.DeleteTransferCenterAsync(id);

                TempData["Success"] = "Transfer merkezi başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, bool isActive)
        {
            try
            {
                await _transferCenterService.ChangeTransferCenterStatusAsync(id, isActive);

                TempData["Success"] = "Transfer merkezi durumu güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            ViewBag.Cities = await _cityService.GetAllCitiesAsync();
        }
      
    }
}
