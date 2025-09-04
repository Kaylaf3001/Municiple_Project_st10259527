using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;
using System.Diagnostics;

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
                // Get total reports count
                ViewBag.TotalReports = _reportRepository.GetReportsCountByUserId(userId.Value);

                // Get recent user reports (limited to 5)
                recentUserReports = _reportRepository.GetReportsByUserId(userId.Value, 5);

                // Get global recent activity (all users, limited to 5)
                var recentReportsQuery = _reportRepository.GetRecentReports(5);
                ViewBag.HasRecentReports = recentReportsQuery.Any();
                ViewBag.RecentReports = recentReportsQuery;
                
                // Add debug information
                Console.WriteLine($"[DEBUG] Recent reports query created");
                foreach (var report in recentReportsQuery)
                {
                    Console.WriteLine($"[DEBUG] Report - Location: {report.Location}, Description: {report.Description}");
                }
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
        
        [HttpGet]
        public IActionResult AddTestReport()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return RedirectToAction("Login", "User");
                
                var testReport = new ReportModel
                {
                    UserId = userId.Value,
                    ReportType = "Test",
                    Description = "This is a test report with location and description",
                    Location = "Test Location",
                    Status = "Pending",
                    ReportDate = DateTime.Now
                };
                
                _reportRepository.AddReport(testReport, null);
                TempData["Message"] = "Test report added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error adding test report: {ex.Message}";
                Console.WriteLine($"[ERROR] Error in AddTestReport: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            }
            
            return RedirectToAction("Index");
        }
    }
}
