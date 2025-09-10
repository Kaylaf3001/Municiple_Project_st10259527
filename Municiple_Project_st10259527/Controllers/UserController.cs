using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;

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

        public UserController(IUserRepository userRepository, IReportRepository reportRepository)
        {
            _userRepository = userRepository;
            _reportRepository = reportRepository;
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
            if (_userRepository.UserExists(email))
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
        public IActionResult UserDashBoard()
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
        //==============================================================================================

        //==============================================================================================
        //Logout action
        //==============================================================================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        // ==============================================================================================
    }
}
//================================================End=Of=File================================================
