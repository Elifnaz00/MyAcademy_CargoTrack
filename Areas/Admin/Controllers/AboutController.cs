using BusinessLayer.Services.Abouts;
using DtoLayer.DTOs.AboutDtos;
using Microsoft.AspNetCore.Mvc;
using MyAcademy_CargoTrack.Consts;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IActionResult> Index()
        {
            var abouts= await _aboutService.GetAllAsync();
            return View(abouts);
        }

        [HttpGet]
        public IActionResult Create() { 
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateAboutDto createAboutDto)
        {
            if(!ModelState.IsValid)
            {
                return View(createAboutDto);
            }
            await _aboutService.CreateAsync(createAboutDto);    
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var about= await _aboutService.GetByIdAsync(id);
            if (about != null) { 
                return NotFound();
            }
            return View(about);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateAboutDto updateAboutDto)
        {

            if (!ModelState.IsValid) { 
                return View(updateAboutDto);
            }
            await _aboutService.UpdateAsync(updateAboutDto);
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _aboutService.DeleteAsync(id);
           
            return RedirectToAction(nameof(Index));
        }
    }
}
