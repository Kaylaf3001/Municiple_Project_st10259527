using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Attributes;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Controllers
{
    [CheckAdmin]
    public class ServiceRequestAdminController : Controller
    {

        //==============================================================================================
        // Dependency Injection
        //==============================================================================================
        #region

        //calling the repository and service
        private readonly IServiceRequestRepository _repo;
        private readonly ServiceRequestStatusService _statusService;

        public ServiceRequestAdminController(IServiceRequestRepository repo, ServiceRequestStatusService statusService)
        {
            _repo = repo;
            _statusService = statusService;
        }
        #endregion
        //==============================================================================================

        //==============================================================================================
        // Manage Service Request
        //==============================================================================================
        public async Task<IActionResult> ManageServiceRequest(string statusFilter = null, string categoryFilter = null, int? priorityFilter = null, string locationFilter = null, int? startId = null)
        {
            var indexes = await _statusService.BuildGlobalIndexesAsync(statusFilter, categoryFilter, priorityFilter, locationFilter);
            ViewData["StatusFilter"] = statusFilter;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["PriorityFilter"] = priorityFilter;
            ViewData["LocationFilter"] = locationFilter;
            ViewData["StartId"] = startId;
            return View("ManageServiceRequest", indexes);
        }
        //==============================================================================================

        //==============================================================================================
        // Update Service Request Status
        //==============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ServiceRequestStatus status, int priority)
        {
            await _repo.UpdateStatusAndPriorityAsync(id, status, priority);
            return RedirectToAction("ManageServiceRequest");
        }

        //==============================================================================================
        // Complete Service Request
        //==============================================================================================
        public async Task<IActionResult> Complete(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request != null)
            {
                await _repo.UpdateStatusAndPriorityAsync(id, ServiceRequestStatus.Completed, request.Priority);
            }
            return RedirectToAction("ManageServiceRequest");
        }

        //==============================================================================================
        // Cancel Service Request
        //==============================================================================================
        public async Task<IActionResult> Cancel(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request != null)
            {
                await _repo.UpdateStatusAndPriorityAsync(id, ServiceRequestStatus.Cancelled, request.Priority);
            }
            return RedirectToAction("ManageServiceRequest");
        }
        //==============================================================================================
    }
}
//====================================End=Of=File========================================================

