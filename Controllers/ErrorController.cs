using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
