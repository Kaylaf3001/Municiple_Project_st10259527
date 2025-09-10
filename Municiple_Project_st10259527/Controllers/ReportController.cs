using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;


namespace Municiple_Project_st10259527.Controllers
{
    public class ReportController : Controller
    {
        //===============================================================================================
        // Dependency Injection for Repositories
        //===============================================================================================
        #region
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;

        //===============================================================================================
        public ReportController(IReportRepository reportRepository, IUserRepository userRepository)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
        }
        #endregion
        //===============================================================================================

        //===============================================================================================================
        // Multi-step Report Submission Process (Step 1)
        //===============================================================================================================
        public IActionResult ReportIssueStep1()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            // Pass the dictionary/list of locations to the view
            ViewBag.WesternCapeLocations = LocationService.WesternCapeLocations.Values.ToList();

            return View();
        }
        //===============================================================================================================

        //===============================================================================================================
        //In this step, we capture the location of the issue from the user.
        // We use a dictionary of Western Cape locations to populate a dropdown or selection list in the view.
        //===============================================================================================================
        [HttpPost]
        public IActionResult ReportIssueStep1(string location)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(location))
            {
                ModelState.AddModelError("", "Location is required");

                // Repopulate locations
                ViewBag.WesternCapeLocations = LocationService.WesternCapeLocations.Values.ToList();

                return View();
            }

            TempData["Location"] = location.Trim();
            return RedirectToAction("ReportIssueStep2");
        }
        //===============================================================================================================

        //===============================================================================================================
        // Multi-step Report Submission Process (Step 2)
        //===============================================================================================================
        public IActionResult ReportIssueStep2()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }
        //===============================================================================================================

        //===============================================================================================================
        // In this step, we capture the category/type of the issue being reported.
        // We validate that a category has been selected before proceeding.
        //===============================================================================================================
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
        //===============================================================================================================

        //===============================================================================================================
        // Multi-step Report Submission Process (Step 3)
        //===============================================================================================================
        public IActionResult ReportIssueStep3()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }
        //===============================================================================================================

        //===============================================================================================================
        // In this step, we capture a detailed description of the issue from the user.
        // We validate that a description has been provided before proceeding.
        //===============================================================================================================
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
        //===============================================================================================================

        //===============================================================================================================
        // Multi-step Report Submission Process (Step 4) - Final Step with File Upload
        //===============================================================================================================
        public IActionResult ReportIssueStep4()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "User");
            return View();
        }
        //===============================================================================================================

        //===============================================================================================================
        // In this final step, we handle the submission of the report along with an optional file upload.
        // We validate that all required information has been provided and then create the report.
        //===============================================================================================================
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
            
            // Validate required fields with detailed logging
            string missingFields = "";
            if (string.IsNullOrWhiteSpace(location)) missingFields += "Location, ";
            if (string.IsNullOrWhiteSpace(category)) missingFields += "Category, ";
            if (string.IsNullOrWhiteSpace(description)) missingFields += "Description, ";
            
            if (!string.IsNullOrWhiteSpace(missingFields))
            {
                // Remove trailing comma and space
                missingFields = missingFields.TrimEnd(',', ' ');
                TempData["Error"] = $"Missing required information: {missingFields}. Please start over.";
                return RedirectToAction("ReportIssueStep1");
            }

            try
            {
                
                var report = new ReportModel
                {
                    UserId = userId.Value,
                    Location = location!,
                    ReportType = category!,
                    Description = description!,
                    Status = ReportStatus.Pending,
                    ReportDate = DateTime.Now
                };

                _reportRepository.AddReport(report, uploadedFile);
                
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
        //===============================================================================================================

        //===============================================================================================================
        // Confirmation Page where the user's first name is displays which is part of the user engagement feature
        //===============================================================================================================
        public IActionResult Confirmation()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            // Get user's first name
            var user = _userRepository.GetUserById(userId.Value);
            if (user != null)
            {
                ViewData["Name"] = user.FirstName;
            }
            
            return View();
        }
        //===============================================================================================================

        //===============================================================================================================
        // Ensure User is logged in before accessing report features
        //===============================================================================================================
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }
        //===============================================================================================================

        //===============================================================================================================
        // Alternative Create Report method for direct report creation (not part of multi-step process)
        //===============================================================================================================
        [HttpPost]
        public IActionResult CreateReport(ReportModel report, IFormFile? uploadedFile)
        {
            _reportRepository.AddReport(report, uploadedFile);
            return RedirectToAction("UserDashboard", "User");
        }
        //===============================================================================================================
    }
}
//==============================================End==of==File=============================================================
