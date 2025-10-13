using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{
    public class UserController : Controller
    {
        //==============================================================================================
        // Dependency Injection
        //==============================================================================================
        #region
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IEventsRepository _eventsRepository;

        public UserController(IUserRepository userRepository, IReportRepository reportRepository, IEventsRepository eventsRepository)
        {
            _userRepository = userRepository;
            _reportRepository = reportRepository;
            _eventsRepository = eventsRepository;
        }
        #endregion
        //==============================================================================================

        //==============================================================================================
        // User Authentication (Login and Signup)
        //==============================================================================================
        public IActionResult Login()
        {
            return View();
        }
        // ==============================================================================================

        //==============================================================================================
        // POST: User/Login
        //==============================================================================================
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _userRepository.GetUserByEmailAndPassword(email, password);
            if (user == null)
            {
                TempData["LoginError"] = "Invalid email or password!";
                return RedirectToAction("Login"); // Stay on the same page
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");

            if (user.IsAdmin)
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("UserDashBoard", "User");
        }
        // ==============================================================================================

        //==============================================================================================
        // GET: User/SignUp
        //==============================================================================================
        public IActionResult SignUp()
        {
            return View();
        }
        // ==============================================================================================

        //==============================================================================================
        // POST: User/SignUp
        //==============================================================================================
        [HttpPost]
        public IActionResult SignUp(string firstName, string lastName, string email, string password)
        {
            HashSet<string> existingEmails = _userRepository.GetAllUserEmails();
            if (existingEmails.Contains(email))
            {
                TempData["SignUpError"] = "A user with this email already exists!";
                return RedirectToAction("SignUp");
            }

            var user = new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                IsAdmin = false
            };
            _userRepository.AddUser(user);

            TempData["SignUpSuccess"] = "User registered successfully!";
            return RedirectToAction("Login", "User"); // Redirect to login page after successful signup
        }
        // ==============================================================================================

        //==============================================================================================
        // User Dashboard
        //==============================================================================================
        


        public async Task<IActionResult> UserDashBoard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            IEnumerable<ReportModel> recentUserReports = null;
            var viewModel = new UserDashboardViewModel();

            try
            {
                // Get user details
                var user = _userRepository.GetUserById(userId.Value);
                if (user != null)
                {
                    viewModel.UserName = user.FirstName;
                }

                // Get reports data
                viewModel.TotalReports = _reportRepository.GetReportsCountByUserId(userId.Value);
                recentUserReports = _reportRepository.GetReportsByUserId(userId.Value, 5);
                viewModel.RecentReports = recentUserReports.ToList();

                // Get upcoming events
                var upcomingEventsQueue = await _eventsRepository.GetUpcomingEventsQueueAsync();
                viewModel.NextUpcomingEvent = await _eventsRepository.GetNextUpcomingEventAsync();
                viewModel.QueueCount = upcomingEventsQueue.Count;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in User/UserDashBoard: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Initialize empty collections in case of error to prevent null reference
                viewModel.RecentReports = new List<ReportModel>();
            }

            return View(viewModel);
        }
        //==============================================================================================

        //==============================================================================================
        // GET: User/MyReports
        //==============================================================================================
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
                ApprovedCount = reports.Count(r => r.Status == ReportStatus.Approved),
                InReviewCount = reports.Count(r => r.Status == ReportStatus.InReview),
                DeniedCount = reports.Count(r => r.Status == ReportStatus.Rejected),
                PendingCount = reports.Count(r => r.Status == ReportStatus.Pending)
            };

            return View(viewModel);
        }
        //==============================================================================================

        //==============================================================================================
        // GET: User/ViewReport/id
        //==============================================================================================
        public async Task<IActionResult> ViewReport(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var report = await _reportRepository.GetReportByIdAsync(id);

            // Ensure the report exists and belongs to the current user
            if (report == null || report.UserId != userId)
            {
                return NotFound();
            }

            return View(report);
        }
        //==============================================================================================

        //==============================================================================================
        //Logout action
        //==============================================================================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        // ==============================================================================================

    }
}
//================================================End=Of=File================================================
