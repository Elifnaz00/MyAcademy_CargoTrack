using BusinessLayer.Services.Branches;
using BusinessLayer.Services.Cities;
using DtoLayer.DTOs.BranchDtos;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly ICityService _cityService;

        public BranchController(
            IBranchService branchService,
            ICityService cityService)
        {
            _branchService = branchService;
            _cityService = cityService;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return View(branches);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCitiesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBranchDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            try
            {
                await _branchService.CreateBranchAsync(dto);

                TempData["Success"] = "Şube başarıyla oluşturuldu.";
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
            var branch = await _branchService.GetBranchByIdAsync(id);

            if (branch == null)
                return NotFound();

            await LoadCitiesAsync();
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBranchDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCitiesAsync();
                return View(dto);
            }

            try
            {
                await _branchService.UpdateBranchAsync(dto);

                TempData["Success"] = "Şube başarıyla güncellendi.";
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
                await _branchService.DeleteBranchAsync(id);
                TempData["Success"] = "Şube başarıyla silindi.";
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
                await _branchService.ChangeBranchStatusAsync(id, isActive);
                TempData["Success"] = "Şube durumu güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCitiesAsync()
        {
            var cities = await _cityService.GetAllCitiesAsync();

            ViewBag.Cities = cities;
        }
    }
}

