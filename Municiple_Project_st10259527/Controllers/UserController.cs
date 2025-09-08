using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repositories;

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
                return Unauthorized("Invalid login");

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
                return BadRequest("User already exists");

            var user = new UserModel 
            { 
                FirstName = firstName,
                LastName = lastName,
                Email = email, 
                Password = password,
                IsAdmin = false // Explicitly set to false
            };
            _userRepository.AddUser(user);

            return Ok("User registered successfully!");
        }
    }
}
