using BusinessLayer.Services.Delivery;
using BusinessLayer.Services.DeliveryExceptions;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DeliveryExceptionController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDeliveryExceptionService _deliveryExceptionService;

        public DeliveryExceptionController(UserManager<AppUser> userManager, IDeliveryExceptionService deliveryExceptionService)
        {
            _userManager = userManager;
            _deliveryExceptionService = deliveryExceptionService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentuserAsync();

            if (currentUser == null)
                return Challenge();

            var solvedProblems=  await _deliveryExceptionService.GetUnresolvedProblemsByBranchAsync(currentUser.Id);
            return View(solvedProblems);
        }

        private async Task<AppUser?> GetCurrentuserAsync()
        {
            return await _userManager.GetUserAsync(User);


        }
    }
}
