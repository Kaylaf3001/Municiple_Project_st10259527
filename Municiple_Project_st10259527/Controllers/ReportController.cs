using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository) // ✅ inject repository only
        {
            _reportRepository = reportRepository;
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
        public IActionResult ReportIssueStep4(IFormFile? uploadedFile)
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

            _reportRepository.AddReport(report, uploadedFile); // ✅ delegate work to repo

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

        [HttpPost]
        public IActionResult CreateReport(ReportModel report, IFormFile? uploadedFile)
        {
            _reportRepository.AddReport(report, uploadedFile);
            return RedirectToAction("Index", "Home");
        }

    }
}
