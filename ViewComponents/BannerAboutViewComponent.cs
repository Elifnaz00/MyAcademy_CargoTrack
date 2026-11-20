using BusinessLayer.Services.Abouts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyAcademy_CargoTrack.ViewComponents
{
    public class BannerAboutViewComponent : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public BannerAboutViewComponent(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about= await _aboutService.GetAboutAsync();
            return View(about);
        }
    }
}
