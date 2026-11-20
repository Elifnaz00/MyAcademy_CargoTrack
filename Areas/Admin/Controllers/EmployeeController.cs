using BusinessLayer.Services.Employee;
using DtoLayer.DTOs.EmployeeDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeController(IEmployeeService employeeService, UserManager<AppUser> userManager)
        {
            _employeeService = employeeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();
                return View(dto);
            }

            try
            {
                var currentUser = await GetCurrentUserAsync();

                if (currentUser is null)
                    return Unauthorized();

                var currentUserId = currentUser.Id;

                await _employeeService.CreateEmployeeAsync(dto, currentUserId);

                TempData["Success"] = "Personel başarıyla oluşturuldu.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();
                return View(dto);
            }

            try
            {
                var currentUser= await GetCurrentUserAsync();
                if (currentUser is null)
                    return Unauthorized();

                var currentUserId = currentUser.Id;

                await _employeeService.UpdateEmployeeAsync(dto, currentUserId);

                TempData["Success"] = "Personel bilgileri başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Branches = await _employeeService.GetActiveBranchesAsync();

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, bool isActive)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser is null)
                    return Unauthorized();

                var currentUserId = currentUser.Id;

                await _employeeService.ChangeEmployeeStatusAsync(id, isActive, currentUserId);

                TempData["Success"] = "Personel durumu başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<AppUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
