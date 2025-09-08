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
            {
                return RedirectToAction("Login", "User");
            }
                
            if (string.IsNullOrWhiteSpace(location))
            {
                ModelState.AddModelError("", "Location is required");
                return View();
            }
            
            // Store location in TempData
            TempData["Location"] = location.Trim();
            
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
            {
                return RedirectToAction("Login", "User");
            }
                
            if (string.IsNullOrWhiteSpace(category))
            {
                ModelState.AddModelError("", "Please select a category");
                return View();
            }
            
            // Store all data in TempData
            TempData["Category"] = category.Trim();
            
            // Keep existing TempData
            TempData.Keep("Location");
            
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
            {
                return RedirectToAction("Login", "User");
            }
            
            // Get existing data from TempData
            var location = TempData.Peek("Location")?.ToString()?.Trim();
            var category = TempData.Peek("Category")?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required";
                return View();
            }
            
            // Store all data in TempData for the next step
            TempData["Description"] = description.Trim();
            
            // Keep existing TempData
            TempData.Keep("Location");
            TempData.Keep("Category");

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
            {
                return RedirectToAction("Login", "User");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            // Get values from TempData without removing them
            var location = TempData.Peek("Location")?.ToString()?.Trim();
            var category = TempData.Peek("Category")?.ToString()?.Trim();
            var description = TempData.Peek("Description")?.ToString()?.Trim();
            
            Console.WriteLine($"[DEBUG] Form data - Location: '{location}', Category: '{category}', Description: '{description}'");
            
            // Validate required fields with detailed logging
            string missingFields = "";
            if (string.IsNullOrWhiteSpace(location)) missingFields += "Location, ";
            if (string.IsNullOrWhiteSpace(category)) missingFields += "Category, ";
            if (string.IsNullOrWhiteSpace(description)) missingFields += "Description, ";
            
            if (!string.IsNullOrWhiteSpace(missingFields))
            {
                // Remove trailing comma and space
                missingFields = missingFields.TrimEnd(',', ' ');
                
                Console.WriteLine($"[ERROR] Missing required fields: {missingFields}");
                TempData["Error"] = $"Missing required information: {missingFields}. Please start over.";
                return RedirectToAction("ReportIssueStep1");
            }

            try
            {
                Console.WriteLine("[DEBUG] Creating new report model");
                
                var report = new ReportModel
                {
                    UserId = userId.Value,
                    Location = location!,
                    ReportType = category!,
                    Description = description!,
                    Status = "Pending",
                    ReportDate = DateTime.Now
                };

                Console.WriteLine("[DEBUG] Report model created with values:" +
                               $"\n  UserId: {report.UserId}" +
                               $"\n  Location: {report.Location}" +
                               $"\n  ReportType: {report.ReportType}" +
                               $"\n  Description: {report.Description}" +
                               $"\n  Status: {report.Status}" +
                               $"\n  ReportDate: {report.ReportDate}");

                Console.WriteLine("[DEBUG] Calling AddReport...");
                _reportRepository.AddReport(report, uploadedFile);
                
                Console.WriteLine("[SUCCESS] Report added successfully!");
                
                // Clear TempData after successful submission
                TempData.Remove("Location");
                TempData.Remove("Category");
                TempData.Remove("Description");
                
                TempData["Success"] = "Your report has been submitted successfully!";
                return RedirectToAction("Confirmation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error in ReportIssueStep4: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"[ERROR] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                TempData["Error"] = "An error occurred while submitting your report. Please try again.";
                return RedirectToAction("ReportIssueStep1");
            }
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
