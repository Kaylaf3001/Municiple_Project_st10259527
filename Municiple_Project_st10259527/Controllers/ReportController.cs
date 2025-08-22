using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;

namespace Municiple_Project_st10259527.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult ReportIssueStep1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep1(string location)
        {
            TempData["Location"] = location; // store data temporarily
            return RedirectToAction("ReportIssueStep2");
        }

        public IActionResult ReportIssueStep2()
        {
            return View(); // 
        }

        [HttpPost]
        public IActionResult ReportIssueStep2(string category)
        {
            TempData["Category"] = category; // save for later steps
            return RedirectToAction("ReportIssueStep3");
        }

        public IActionResult ReportIssueStep3()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep3(string description)
        {
            TempData["Description"] = description; // save for later steps
            return RedirectToAction("ReportIssueStep4");
        }

        public IActionResult ReportIssueStep4()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReportIssueStep4(string confirmation)
        {
            // Here you would typically save the report to the database
            // using TempData["Location"], TempData["Category"], and TempData["Description"]
            // For now, just redirect to a confirmation page
            return RedirectToAction("Confirmation");
        }
    }
}
