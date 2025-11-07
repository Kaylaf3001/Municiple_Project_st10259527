using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Controllers
{
    [AllowAnonymous]
    public class PublicTrackingController : Controller
    {
        private readonly ServiceRequestStatusService _statusService;
        public PublicTrackingController(ServiceRequestStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        public IActionResult Index(string code = null)
        {
            ViewData["Code"] = code;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lookup(string trackingCode)
        {
            if (string.IsNullOrWhiteSpace(trackingCode))
            {
                TempData["TrackResult"] = "Please enter a tracking code.";
                return RedirectToAction("Index");
            }

            var req = await _statusService.TrackByCodeAsync(trackingCode.Trim());
            TempData["TrackResult"] = req == null
                ? "Not found"
                : $"{req.Title} - {req.Status} (ID: {req.RequestId})";
            TempData["Code"] = trackingCode.Trim();
            return RedirectToAction("Index");
        }
    }
}
