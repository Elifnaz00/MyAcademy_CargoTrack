using BusinessLayer.Services.QuikServices;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.ViewComponents
{
    public class QuikServicesViewComponent : ViewComponent
    {
        private readonly IQuikService _quikService;

        public QuikServicesViewComponent(IQuikService quikService)
        {
            _quikService = quikService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           var quikServices= await _quikService.GetQuikListAsync();
            return View(quikServices);
        }
    }
}
