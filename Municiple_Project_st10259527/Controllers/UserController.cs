using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;

namespace Municiple_Project_st10259527.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

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

            return RedirectToAction("Index", "Home");
        }


        public IActionResult SignUp()
        {
            return View();
        }

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


        //Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}
