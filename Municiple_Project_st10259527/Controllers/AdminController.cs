using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Municiple_Project_st10259527.ViewModels;
using Municiple_Project_st10259527.Attributes;

namespace Municiple_Project_st10259527.Controllers
{
    [CheckAdmin]
    public class AdminController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(
            IReportRepository reportRepository, 
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var viewModel = await _adminRepository.GetDashboardDataAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error loading admin dashboard");
                return StatusCode(500, "An error occurred while loading the dashboard. Please try again later.");
            }
        }

        public async Task<IActionResult> AllReports(ReportStatus? status = null)
        {
            try
            {
                IEnumerable<ReportModel> reports;
                
                if (status.HasValue)
                {
                    reports = await _reportRepository.GetReportsByStatusAsync(status.Value);
                }
                else
                {
                    reports = await _reportRepository.GetAllReportsWithUsersAsync();
                }

                ViewBag.CurrentFilter = status;
                return View(reports);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving reports.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReport(int id, string? adminNotes = null)
        {
            try
            {
                var adminId = HttpContext.Session.GetInt32("UserId");
                if (!adminId.HasValue)
                    return Unauthorized();

                var success = await _reportRepository.ApproveReportAsync(id, adminId.Value, adminNotes);
                if (!success)
                    return NotFound();

                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while approving the report.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReport(int id, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                    return BadRequest("Rejection reason is required.");

                var adminId = HttpContext.Session.GetInt32("UserId");
                if (!adminId.HasValue)
                    return Unauthorized();

                var success = await _reportRepository.RejectReportAsync(id, adminId.Value, reason);
                if (!success)
                    return NotFound();

                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while rejecting the report.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkInReview(int id)
        {
            try
            {
                var adminId = HttpContext.Session.GetInt32("UserId");
                if (!adminId.HasValue)
                    return Unauthorized();

                var success = await _reportRepository.MarkInReviewAsync(id, adminId.Value);
                if (!success)
                    return NotFound();

                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while updating the report status.");
            }
        }

        public async Task<IActionResult> UpdateReportStatus(int reportId, ReportStatus status, string adminNotes = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return Unauthorized();

            var report = await _reportRepository.GetReportByIdAsync(reportId);
            if (report == null)
                return NotFound();

            report.Status = status;
            report.AdminNotes = adminNotes;
            report.LastUpdated = DateTime.Now;
            report.AssignedAdminId = userId.Value;

            await _reportRepository.UpdateReportAsync(report);
            
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> ReportDetails(int id)
        {
            var report = await _reportRepository.GetReportWithDetailsAsync(id);
            if (report == null)
                return NotFound();

            return View(report);
        }
    }
}
