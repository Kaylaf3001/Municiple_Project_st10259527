using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using System.Diagnostics;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;

        public HomeController(IUserRepository userRepository, IReportRepository reportRepository)
        {
            _userRepository = userRepository;
            _reportRepository = reportRepository;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            IEnumerable<ReportModel> recentUserReports = null;
            
            try
            {
                // Get user details
                var user = _userRepository.GetUserById(userId.Value);
                if (user != null)
                {
                    ViewBag.UserFirstName = user.FirstName;
                }
                
                // Get total reports count
                ViewBag.TotalReports = _reportRepository.GetReportsCountByUserId(userId.Value);

                // Get recent user reports (limited to 5)
                recentUserReports = _reportRepository.GetReportsByUserId(userId.Value, 5);
                ViewBag.RecentReports = recentUserReports;
                ViewBag.HasRecentReports = recentUserReports.Any();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in Home/Index: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Initialize empty collections in case of error to prevent null reference
                recentUserReports = Enumerable.Empty<ReportModel>();
                ViewBag.RecentReports = Enumerable.Empty<ReportModel>();
            }

            return View(recentUserReports); // pass user reports as model
        }

        [HttpGet]
        public IActionResult MyReports()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var reports = _reportRepository.GetReportsByUserId(userId.Value);
        
            var viewModel = new UserReportsViewModel
            {
                Reports = reports,
                TotalReports = reports.Count(),
                ResolvedCount = reports.Count(r => r.Status == ReportStatus.Completed),
                InReviewCount = reports.Count(r => r.Status == ReportStatus.InReview),
                DeniedCount = reports.Count(r => r.Status == ReportStatus.Rejected),
                PendingCount = reports.Count(r => r.Status == ReportStatus.Pending)
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Issue()
        {
            return RedirectToAction("ReportIssueStep1", "Report");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
