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
        //==============================================================================================
        // Dependency Injection
        //==============================================================================================
        #region
        private readonly ServiceRequestStatusService _statusService;
        private readonly IServiceRequestRepository _repo;
        public ServiceRequestController(ServiceRequestStatusService statusService, IServiceRequestRepository repo)
        {
            _statusService = statusService;
            _repo = repo;
        }
        #endregion
        //==============================================================================================

        //==============================================================================================
        // Service Request Status and Creation
        //==============================================================================================
        public async Task<IActionResult> Status()
        {
            // Get UserId from Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            // Build Data Structures
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
        //==============================================================================================

        //==============================================================================================
        // Create Service Request
        //==============================================================================================
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");
            return View();
        }
        //==============================================================================================

        //==============================================================================================
        // Handle Service Request Creation
        //==============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string description, int priority)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            // Basic Validation
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("", "Title is required");
                return View();
            }

            // Create and Save Service Request
            var req = new ServiceRequestModel
            {
                UserId = userId.Value,
                Title = title.Trim(),
                Description = description?.Trim(),
                Priority = priority
            };

            // Save to Repository
            await _repo.AddAsync(req);
            return RedirectToAction("Status");
        }
        //==============================================================================================

        //==============================================================================================
        // Track Service Request by Code
        //==============================================================================================
        [HttpPost]
        public async Task<IActionResult> Track(string trackingCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            // Basic Validation
            if (string.IsNullOrWhiteSpace(trackingCode)) return RedirectToAction("Status");

            // Track Request
            var req = await _statusService.TrackByCodeAsync(trackingCode.Trim());

            // Store Result in TempData for Display (detailed)
            if (req == null)
            {
                TempData["TrackFound"] = false;
                TempData["TrackMessage"] = "Not found";
            }
            else
            {
                TempData["TrackFound"] = true;
                TempData["TrackId"] = req.RequestId;
                TempData["TrackTitle"] = req.Title;
                TempData["TrackStatus"] = req.Status.ToString();
                TempData["TrackPriority"] = req.Priority;
                TempData["TrackCode"] = req.TrackingCode;
                TempData["TrackSubmittedAt"] = req.SubmittedAt.ToString("u");
            }

            return RedirectToAction("Status");
        }
        //==============================================================================================
    }
}
//======================================End=Of=File======================================================