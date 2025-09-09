using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Municiple_Project_st10259527.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private const int PageSize = 10;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Check if user is logged in and is admin
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            if (!isAdmin)
            {
                return RedirectToAction("Login", "User");
            }

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

        // GET: Admin/PendingReports
        public async Task<IActionResult> PendingReports()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            if (!isAdmin)
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                // Set counts for the sidebar
                ViewBag.TotalReports = await _adminRepository.GetTotalReportCountAsync();
                ViewBag.PendingCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Pending);
                ViewBag.ApprovedCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Approved);
                
                var pendingReports = await _adminRepository.GetReportsByStatusAsync(ReportStatus.Pending);
                return View(pendingReports);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading pending reports.");
            }
        }

        // GET: Admin/ApprovedReports
        public async Task<IActionResult> ApprovedReports()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            if (!isAdmin)
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                // Set counts for the sidebar
                ViewBag.TotalReports = await _adminRepository.GetTotalReportCountAsync();
                ViewBag.PendingCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Pending);
                ViewBag.ApprovedCount = await _adminRepository.GetReportCountByStatusAsync(ReportStatus.Approved);
                
                var approvedReports = await _adminRepository.GetReportsByStatusAsync(ReportStatus.Approved);
                return View(approvedReports);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading approved reports.");
            }
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports(ReportStatus? status = null, int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(status, page, PageSize, searchTerm);
            var totalCount = await _adminRepository.GetTotalReportCountAsync();

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
        public async Task<IActionResult> UpdateStatus(int id, ReportReviewViewModel model)
        {
            if (id != model.ReportId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _adminRepository.UpdateReportStatusAsync(model.ReportId, model.NewStatus, model.AdminNotes);
                    TempData["SuccessMessage"] = "Report status updated successfully.";
                    return RedirectToAction(nameof(Review), new { id = model.ReportId });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while updating the report status.");
                }
            }

            // If we got this far, something failed, redisplay form
            var report = await _adminRepository.GetReportByIdAsync(model.ReportId);
            if (report == null)
            {
                return NotFound();
            }

            // Repopulate the view model with the original report data
            model.ReportType = report.ReportType;
            model.Description = report.Description;
            model.ReportDate = report.ReportDate;
            model.Location = report.Location;
            model.Status = report.Status;
            model.FilePath = report.FilePath;
            model.SubmittedBy = $"{report.User?.FirstName} {report.User?.LastName}";
            model.UserEmail = report.User?.Email;

            return View("Review", model);
        }
    }
}
