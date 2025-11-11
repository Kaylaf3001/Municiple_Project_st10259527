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
        // View Service Request Details
        //==============================================================================================
        public async Task<IActionResult> Detail(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return View("ServiceRequestDetail", request);
        }

        //==============================================================================================
        // Complete Service Request and Get Related Requests
        //==============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRequest(int id)
        {
            try
            {
                // Get the request to complete
                var request = await _repo.GetByIdAsync(id);
                if (request == null)
                {
                    return Json(new { success = false, error = "Request not found." });
                }

                // Update the request status
                request.Status = ServiceRequestStatus.Completed;
                request.CompletedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(request);

                // Get related requests
                var indexes = await _statusService.BuildGlobalIndexesAsync();
                var relatedRequests = new List<ServiceRequestModel>();
                var currentNode = indexes.Graph?.Nodes().FirstOrDefault(n => n.Val.RequestId == id);
                
                if (currentNode != null)
                {
                    var mstEdges = indexes.Graph.PrimMst();
                    relatedRequests = mstEdges
                        .Where(e => e.Item1 == currentNode || e.Item2 == currentNode)
                        .OrderBy(e => e.Item3) // Sort by weight (most related first)
                        .Take(3)
                        .Select(e => e.Item1 == currentNode ? e.Item2.Val : e.Item1.Val)
                        .Where(r => r.Status != ServiceRequestStatus.Completed)
                        .ToList();
                }

                return Json(new { 
                    success = true, 
                    relatedRequests = relatedRequests.Select(r => new {
                        id = r.RequestId,
                        title = r.Title,
                        status = r.Status.ToString(),
                        category = r.Category,
                        location = r.Location,
                        priority = r.Priority,
                        submittedAt = r.SubmittedAt.ToString("g")
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occurred while completing the request." });
            }
        }

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

