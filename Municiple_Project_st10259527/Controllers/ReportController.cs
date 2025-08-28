using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        // Multi-step reporting process
        public IActionResult ReportIssueStep1()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep1(string location)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            TempData["Location"] = location;
            return RedirectToAction("ReportIssueStep2");
        }

        public IActionResult ReportIssueStep2()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep2(string category)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            TempData["Category"] = category;
            return RedirectToAction("ReportIssueStep3");
        }

        public IActionResult ReportIssueStep3()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep3(string description)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            TempData["Description"] = description;
            return RedirectToAction("ReportIssueStep4");
        }

        public IActionResult ReportIssueStep4()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep4(string confirmation)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            var userId = HttpContext.Session.GetInt32("UserId");
            var report = new ReportModel
            {
                UserId = userId.Value,
                Location = TempData["Location"]?.ToString(),
                ReportType = TempData["Category"]?.ToString(),
                Description = TempData["Description"]?.ToString(),
                Status = "Pending",
                ReportDate = DateTime.Now
            };

            _context.Reports.Add(report);
            _context.SaveChanges();

            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }
    }
}
