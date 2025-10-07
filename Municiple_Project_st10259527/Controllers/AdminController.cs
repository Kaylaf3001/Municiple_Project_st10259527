using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Controllers
{
    public class AdminController : Controller
    {
        //===============================================================================================
        // Dependency Injection for Repositories and Constants
        //===============================================================================================
        #region
        private readonly IAdminRepository _adminRepository;
        private const int PageSize = 10;
        private readonly IEventsRepository _eventsRepository;
        private readonly IAnnouncementsRepository _announcementsRepository;

        public AdminController(IAdminRepository adminRepository, IEventsRepository eventsRepository, IAnnouncementsRepository announcementsRepository)
        {
            _adminRepository = adminRepository;
            _eventsRepository = eventsRepository;
            _announcementsRepository = announcementsRepository;
        }
        #endregion
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/Dashboard
        //===============================================================================================
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
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/Reports
        //===============================================================================================
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
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/AllReports
        //===============================================================================================
        public async Task<IActionResult> AllReports(ReportStatus? status = null, int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(status, page, PageSize, searchTerm);
            return View(reports);
        }
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/PendingReports
        //===============================================================================================
        public async Task<IActionResult> PendingReports(int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(ReportStatus.Pending, page, PageSize, searchTerm);
            return View("AllReports", reports);
        }
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/ApprovedReports
        //===============================================================================================
        public async Task<IActionResult> ApprovedReports(int page = 1, string searchTerm = null)
        {
            var reports = await _adminRepository.GetReportsAsync(ReportStatus.Approved, page, PageSize, searchTerm);
            return View("AllReports", reports);
        }
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/ViewReport/id
        //===============================================================================================
        public async Task<IActionResult> ViewReport(int id)
        {
            var report = await _adminRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return View("ReportDetails", report);
        }
        //===============================================================================================

        //===============================================================================================
        // GET: Admin/Review/id
        //===============================================================================================
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
        //===============================================================================================

        //===============================================================================================
        // POST: Admin/UpdateStatus/id
        //===============================================================================================
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateStatus: {ex}");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        //===============================================================================================

        //===============================================================================================
        // Events and Announcements
        //===============================================================================================

        //===============================================================================================
        // POST: Admin/AddEvent
        //===============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvent(Models.EventModel eventModel)
        {
            // Get AdminId from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                eventModel.UserId = userId.Value;
            }
            else
            {
                eventModel.UserId = 1; // fallback
            }

            // Set Status
            if (string.IsNullOrWhiteSpace(eventModel.Status))
            {
                eventModel.Status = "Normal";
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return View("Events/ManageEvents", eventModel);
            }

            await _eventsRepository.AddEventAsync(eventModel);
            return RedirectToAction(nameof(ManageEvents));

        }
        //===============================================================================================

        //===============================================================================================
        // Create Events
        //===============================================================================================
        public IActionResult CreateEvent()
        {
            return View("Events/CreateEvents");
        }
        //===============================================================================================

        //===============================================================================================
        // Manage Events, get all events
        //===============================================================================================
        [HttpGet]
        public async Task<IActionResult> ManageEvents(Models.EventModel eventModel)
        {
            try
            {
                var events = await _eventsRepository.GetAllEventsAsync();
                return View("Events/ManageEvents", events);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading events.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageEvents()
        {
            if (!ModelState.IsValid)
            {
                return View("Events/ManageEvents");
            }
            return RedirectToAction(nameof(ManageEvents));
        }

        //===============================================================================================
        // Edit Event
        //===============================================================================================
        public IActionResult EditEvent()
        {
            return View("Events/EditEvent");
        }
        //===============================================================================================

        //==============================================================================================
        // Announcements
        //==============================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnnouncements(Models.AnnouncementModel announcementModel)
        {
            // Get AdminId from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                announcementModel.UserId = userId.Value;
            }
            else
            {
                announcementModel.UserId = 1; // fallback
            }

            // Set Status
            if (string.IsNullOrWhiteSpace(announcementModel.Status))
            {
                announcementModel.Status = "Normal";
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }
                return View("Announcements/ManageAnnouncements", announcementModel);
            }

            await _announcementsRepository.AddAnnouncementAsync(announcementModel);
            return RedirectToAction(nameof(ManageAnnouncements));
        }
        //==============================================================================================

        //==============================================================================================
        // Manage Announcements, get all announcements
        //==============================================================================================
        [HttpGet]
        public async Task<IActionResult> ManageAnnouncements(Models.EventModel eventModel)
        {
            try
            {
                var announcements = await _announcementsRepository.GetAllAnnouncementsAsync();
                return View("Announcements/ManageAnnouncements", announcements);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while loading events.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageAnnouncements()
        {
            if (!ModelState.IsValid)
            {
                return View("Announcements/ManageAnnouncements");
            }
            return RedirectToAction(nameof(ManageAnnouncements));
        }
        //==============================================================================================

        public IActionResult EditAnnouncement()
        {
            
            return View("Announcements/EditAnnouncement");
        }
        //==============================================================================================
    }

}
//================================================End=Of=File================================================
