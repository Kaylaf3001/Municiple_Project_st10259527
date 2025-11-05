using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly ServiceRequestStatusService _statusService;
        private readonly IServiceRequestRepository _repo;
        public ServiceRequestController(ServiceRequestStatusService statusService, IServiceRequestRepository repo)
        {
            _statusService = statusService;
            _repo = repo;
        }

        public async Task<IActionResult> Status()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var indexes = await _statusService.BuildIndexesAsync(userId.Value);
            var vm = new ServiceRequestStatusViewModel
            {
                UserId = userId.Value,
                RequestsTree = indexes.Tree,
                PriorityHeap = indexes.Heap,
                RelationshipGraph = indexes.Graph
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string description, int priority)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("", "Title is required");
                return View();
            }
            var req = new ServiceRequestModel
            {
                UserId = userId.Value,
                Title = title.Trim(),
                Description = description?.Trim(),
                Priority = priority
            };
            await _repo.AddAsync(req);
            return RedirectToAction("Status");
        }

        [HttpPost]
        public async Task<IActionResult> Track(string trackingCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");
            if (string.IsNullOrWhiteSpace(trackingCode)) return RedirectToAction("Status");

            var req = await _statusService.TrackByCodeAsync(trackingCode.Trim());
            TempData["TrackResult"] = req == null ? "Not found" : $"{req.Title} - {req.Status} (ID: {req.RequestId})";
            return RedirectToAction("Status");
        }
    }
}
