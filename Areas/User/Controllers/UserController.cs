using BusinessLayer.Services.Cargos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyAcademy_CargoTrack.Areas.User.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ICargoService _cargoService;
        public UserController(UserManager<AppUser> userManager, ICargoService cargoService)
        {
            _cargoService = cargoService;
            _userManager = userManager;
        }

        private async Task<AppUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        public async Task<IActionResult> Index()
        {
           var user = await GetCurrentUserAsync();

            if (user == null)
            return RedirectToAction("Login", "Auth", new { area = "" });
        
            return View();
        }

        public async Task<IActionResult> MyCargos()
        {
            var user = await GetCurrentUserAsync();
            
            if (user == null)
                return RedirectToAction("Login", "Auth", new { area = "" });
            
            var userInComingCargos = await _cargoService.GetMyIncomingShipmentsAsync(user.Id);
            return View(userInComingCargos);
        }

        public async Task<IActionResult> MyOutGoingyCargos()
        {
            var user = await GetCurrentUserAsync();
            
            if (user == null)
                return RedirectToAction("Login", "Auth", new { area = "" });
            
            var userOutGoingCargos = await _cargoService.GetMyOutGoingShipmentsAsync(user.Id);
            return View(userOutGoingCargos);
        }

        public async Task<IActionResult> Profile()
        {
            
            var user = await GetCurrentUserAsync();

            if (user == null)
                return RedirectToAction("Login", "Auth", new { area = "" });
            
            return View();
           
        }

        public async Task<IActionResult> CargoDetail(Guid id)
        {
            
            var user = await GetCurrentUserAsync();
            if (user == null)
                return RedirectToAction("Login", "Auth", new { area = "" });
            
            var cargoDetail = await _cargoService.GetCargoDetailAsync(id);
            if (cargoDetail == null)
                return NotFound();
            return View(cargoDetail);
        }

        public async Task<IActionResult> DeliveryHistory()
        {
            var user = await GetCurrentUserAsync();

            
            if (user == null)
                return RedirectToAction("Login", "Auth", new { area = "" });
            

            var deliveryHistory = await _cargoService
      .GetMyDeliveryHistoryAsync(user.Id);


            return View(deliveryHistory);
        }

        [HttpGet]
        public async Task<IActionResult> MyDeliveryCodes()
        {
            
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
                return Challenge();
            
            var deliveryCodes =
                await _cargoService.GetMyActiveDeliveryCodesAsync(userId);
            
            return View(deliveryCodes);
        }
    }
}
