using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
