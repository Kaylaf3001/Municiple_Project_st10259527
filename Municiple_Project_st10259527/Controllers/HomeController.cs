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

            // Dashboard number
            ViewBag.TotalReports = _reportRepository.GetTotalReports();

            // User-specific reports (for recent activity)
            var recentUserReports = _reportRepository.GetReportsByUserId(userId.Value)
                                                     .Take(5)
                                                     .ToList();

            // Optionally, show global recent activity (all users)
            ViewBag.RecentReports = _reportRepository.GetRecentReports(5);

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
    }
}
