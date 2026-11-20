using BusinessLayer.Services.AuditLog;
using Microsoft.AspNetCore.Mvc;

namespace MyAcademy_CargoTrack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogController : Controller
    {
        private readonly IAuditService _auditService;

        public AuditLogController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _auditService.GetAllAuditLogsAsync();

            return View(values);
        }
    
}
}
