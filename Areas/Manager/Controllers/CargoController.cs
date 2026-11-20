using BusinessLayer.Services.Branches;
using BusinessLayer.Services.Cargos;
using BusinessLayer.Services.Customers;
using BusinessLayer.Services.Delivery;
using DtoLayer.DTOs.CargoDtos;
using DtoLayer.DTOs.DeliveryDtos;
using EntityLayer.Entities;
using EntityLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAcademy_CargoTrack.Areas.Manager.Models;
using System.Threading.Tasks;

namespace MyAcademy_CargoTrack.Areas.Manager.Controllers
{
    [Area("Manager")]
    //[Authorize(Roles = "Manager,Admin")]
    public class CargoController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly ICustomerService _customerService;
        private readonly ICargoService _cargoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDeliveryService _deliveryService;
        public CargoController(IDeliveryService deliveryService,IBranchService branchService, ICustomerService customerService, ICargoService cargoService, UserManager<AppUser> userManager)
        {
            _branchService = branchService;
            _customerService = customerService;
            _cargoService = cargoService;
            _userManager = userManager;
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<IActionResult> Incoming()
        {
            var currentUser = await GetCurrentuserAsync();
            if (currentUser == null)
                return Challenge();

            var incomingCargos = await _branchService.GetIncomingCargosOnBranchAsync(currentUser.Id);
            return View(incomingCargos);
        }

        [HttpGet]
        public async Task<IActionResult> Outgoing()
        {
            var currentUser = await GetCurrentuserAsync();
            if (currentUser == null)
                return Challenge();

            var outgoingCargos = await _branchService.GetOutcomingCargosOnBranchAsync(currentUser.Id);
            return View(outgoingCargos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid cargoId, CargoStatus newStatus, string returnAction = "Incoming")
        {
            var currentUser = await GetCurrentuserAsync();

            if (currentUser == null)
                return Challenge();

            await _cargoService.UpdateCargoStatusAsync(cargoId, newStatus, currentUser.Id);

            TempData["SuccessMessage"] = "Kargo durumu başarıyla güncellendi.";

            return RedirectToAction(returnAction);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateCargoViewModel();

            await LoadDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoViewModel createCargoViewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(createCargoViewModel);
                return View(createCargoViewModel);
            }
            var currentUser = await GetCurrentuserAsync();
            if (currentUser == null)
                return Challenge();

            var createCargoDto = new CreateCargoDto
            {
                SenderCustomerId = createCargoViewModel.SenderCustomerId,
                ReceiverCustomerId = createCargoViewModel.ReceiverCustomerId,
                OriginBranchId = createCargoViewModel.OriginBranchId,
                DestinationBranchId = createCargoViewModel.DestinationBranchId,
                Weight = createCargoViewModel.Weight,
                Length = createCargoViewModel.Length,
                Width = createCargoViewModel.Width,
                Height = createCargoViewModel.Height,
                CargoType = createCargoViewModel.CargoType,
                DeliveryType = createCargoViewModel.DeliveryType,
                DeclaredValue = createCargoViewModel.DeclaredValue
            };
            await _cargoService.CreateCargoAsync(createCargoDto, currentUser.Id);
            return RedirectToAction(nameof(Index));
        }


        private async Task LoadDropdownsAsync(CreateCargoViewModel model)
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var branches = await _branchService.GetAllBranchesAsync();

            model.Customers = customers.Select(x => new SelectListItem
            {
                Text = $"{x.FullName} - {x.CustomerNumber}",
                Value = x.AppUserId.ToString()
            }).ToList();

            model.Branches = branches.Select(x => new SelectListItem
            {
                Text = $"{x.Name} - {x.CityName}",
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<AppUser?> GetCurrentuserAsync()
        {
            return await _userManager.GetUserAsync(User);


        }

        [HttpGet]
        public async Task<IActionResult> DeliveryFailed(Guid cargoId)
        {
            var model = new DeliveryFailedViewModel
            {
                CargoId = cargoId
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliveryFailed(DeliveryFailedViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await GetCurrentuserAsync();

            if (currentUser == null)
                return Challenge();

            var dto = new CreateDeliveryExceptionDto
            {
                CargoId = model.CargoId,
                ExceptionType = model.ExceptionType,
                Description = model.Description
            };

            await _cargoService.RegisterDeliveryFailureAsync(
                dto,
                currentUser.Id);

            TempData["SuccessMessage"] =
                "Teslimat başarısızlığı başarıyla kaydedildi.";

            return RedirectToAction("Incoming");
        }

        [HttpGet]
        public IActionResult CompleteDelivery(Guid cargoId)
        {
            var model = new CompleteDeliveryViewModel
            {
                CargoId = cargoId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteDelivery(
            CompleteDeliveryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await GetCurrentuserAsync();

            if (currentUser == null)
                return Challenge();

            try
            {
                var dto = new VerifyDeliveryCodeDto
                {
                    CargoId = model.CargoId,
                    DeliveryCode = model.DeliveryCode,
                    RecipientName = model.RecipientName
                };

                await _deliveryService.VerifyAndCompleteDeliveryAsync(
                    dto,
                    currentUser.Id);

                TempData["SuccessMessage"] =
                    "Kargo başarıyla teslim edildi.";

                return RedirectToAction("Incoming");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        public async Task<IActionResult> Delayed()
        {
            var currentUser = await GetCurrentuserAsync();

            if (currentUser == null)
                return Challenge();

            var delayedCargos= await _cargoService.GetDelayedCargosByBranchAsync(currentUser.Id);
            return View(delayedCargos);
        }


    }
    }
