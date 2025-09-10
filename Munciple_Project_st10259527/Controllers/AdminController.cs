using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try 
            {
                var dashboardData = await _adminRepository.GetDashboardDataAsync();
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading the dashboard.");
            }
        }
        private readonly IAdminRepository _adminRepository;
        private const int PageSize = 10;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports(ReportStatus? status = null, int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(status, page, PageSize, searchTerm);
            var totalCount = status.HasValue 
                ? await _adminRepository.GetReportCountByStatusAsync(status.Value)
                : await _adminRepository.GetTotalReportCountAsync();

            var viewModel = new AdminReportsListViewModel
            {
                Reports = reports,
                FilterStatus = status,
                PageNumber = page,
                PageSize = PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                SearchTerm = searchTerm,
                PendingCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Pending),
                InReviewCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.InReview),
                ApprovedCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Approved),
                RejectedCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Rejected),
                CompletedCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Completed)
            };

            return View(viewModel);
        }
        
        // GET: Admin/AllReports
        public async Task<IActionResult> AllReports(ReportStatus? status = null, int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(status, page, PageSize, searchTerm);
            return View(reports);
        }

        // GET: Admin/PendingReports
        public async Task<IActionResult> PendingReports(int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(ReportStatus.Pending, page, PageSize, searchTerm);
            return View("AllReports", reports);
        }

        // GET: Admin/ApprovedReports
        public async Task<IActionResult> ApprovedReports(int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(ReportStatus.Approved, page, PageSize, searchTerm);
            return View("AllReports", reports);
        }

        // GET: Admin/ViewReport/5
        public async Task<IActionResult> ViewReport(int id)
        {
            var report = await _adminRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return View("ReportDetails", report);
        }

        // GET: Admin/Review/5
        public async Task<IActionResult> Review(int id)
        {
            var report = await _adminRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new ReportReviewViewModel
            {
                ReportId = report.ReportId,
                ReportType = report.ReportType,
                Description = report.Description,
                ReportDate = report.ReportDate,
                Location = report.Location,
                Status = report.Status,
                FilePath = report.FilePath,
                SubmittedBy = $"{report.User?.FirstName} {report.User?.LastName}",
                UserEmail = report.User?.Email,
                NewStatus = report.Status,
                AdminNotes = report.AdminNotes
            };

            return View(viewModel);
        }

        // POST: Admin/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus()
        {
            try
            {
                Console.WriteLine("UpdateStatus called");
                var form = await Request.ReadFormAsync();
                
                if (!form.ContainsKey("reportId") || !form.ContainsKey("status"))
                {
                    return BadRequest(new { success = false, error = "Missing required parameters" });
                }

                if (!int.TryParse(form["reportId"], out int reportId))
                {
                    return BadRequest(new { success = false, error = "Invalid report ID" });
                }

                if (!Enum.TryParse<ReportStatus>(form["status"], out var status))
                {
                    return BadRequest(new { success = false, error = "Invalid status value" });
                }

                string adminNotes = form["adminNotes"];
                Console.WriteLine($"Updating report {reportId} to status {status}");
                
                await _adminRepository.UpdateReportStatusAsync(reportId, status, adminNotes);
                
                // Check if this is an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("ApprovedReports") });
                }
                
                // For non-AJAX requests, redirect based on status
                if (status == ReportStatus.Approved || status == ReportStatus.Completed)
                {
                    return RedirectToAction("ApprovedReports");
                }
                
                return RedirectToAction("PendingReports");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateStatus: {ex}");
                return StatusCode(500, new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
