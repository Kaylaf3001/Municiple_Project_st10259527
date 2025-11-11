using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Attributes;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using System;
using System.Linq;
using System.Collections.Generic;

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

                // Get related requests using minimum spanning tree
                var indexes = await _statusService.BuildGlobalIndexesAsync();
                var relatedRequests = new List<ServiceRequestModel>();
                var currentNode = indexes.Graph?.Nodes().FirstOrDefault(n => n.Val.RequestId == id);

                List<(ServiceRequestModel Model, int Weight)> relatedWithWeights = new();
                if (currentNode != null)
                {
                    // Always use MST-adjacent edges for recommendations per rubric
                    var mstEdges = indexes.Graph.PrimMst(currentNode);
                    relatedWithWeights = mstEdges
                        .Where(e => e.Item1 == currentNode || e.Item2 == currentNode)
                        .Select(e => (Model: (e.Item1 == currentNode ? e.Item2.Val : e.Item1.Val), Weight: e.Item3))
                        .Where(x => x.Model != null && x.Model.Status != ServiceRequestStatus.Completed)
                        .OrderBy(x => x.Weight)
                        .ThenBy(x => x.Model.Priority)
                        .ThenBy(x => x.Model.SubmittedAt)
                        .Take(3)
                        .ToList();

                    relatedRequests = relatedWithWeights.Select(x => x.Model).ToList();
                }

                var relatedPayload = relatedRequests.Select(r =>
                {
                    var match = relatedWithWeights.FirstOrDefault(x => x.Model.RequestId == r.RequestId);
                    var w = match.Weight;
                    var baseReason = GetRelationReason(request, r);
                    var reason = baseReason == "Related" && w > 0 ? $"Connected via MST (weight {w})" : baseReason;
                    return new
                    {
                        id = r.RequestId,
                        title = r.Title,
                        status = r.Status.ToString(),
                        category = r.Category,
                        location = r.Location,
                        priority = r.Priority,
                        submittedAt = r.SubmittedAt.ToString("g"),
                        weight = w,
                        reason = reason
                    };
                }).ToList();

                return Json(new {
                    success = true,
                    redirectUrl = Url.Action("Detail", new { id }),
                    relatedRequests = relatedPayload
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
        // Helpers
        //==============================================================================================
        private string GetRelationReason(ServiceRequestModel a, ServiceRequestModel b)
        {
            if (a == null || b == null) return "Related";
            if (!string.IsNullOrWhiteSpace(a.Location) && a.Location.Equals(b.Location, StringComparison.OrdinalIgnoreCase))
                return "Same location";
            if (!string.IsNullOrWhiteSpace(a.Category) && a.Category.Equals(b.Category, StringComparison.OrdinalIgnoreCase))
                return "Same category";
            if (a.Status == b.Status)
            {
                // Only call out time proximity when also same status and within 1 day
                var dtA = a.SubmittedAt;
                var dtB = b.SubmittedAt;
                if (dtA != default && dtB != default && Math.Abs((dtA - dtB).TotalDays) <= 1)
                    return "Same status and submitted around same time";
                return "Same status";
            }
            // Simple keyword overlap hint
            var at = ((a.Title ?? "") + " " + (a.Description ?? "")).ToLowerInvariant();
            var bt = ((b.Title ?? "") + " " + (b.Description ?? "")).ToLowerInvariant();
            if ((at.Contains("electrical") && bt.Contains("paint")) || (bt.Contains("electrical") && at.Contains("paint")))
                return "Electrical work before painting";
            return "Related";
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

