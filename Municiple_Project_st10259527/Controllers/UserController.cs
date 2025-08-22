using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.Models; // ✅ add this

namespace Municiple_Project_st10259527.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
                return Unauthorized("Invalid login");

            HttpContext.Session.SetInt32("UserId", user.UserId);
            return Ok("Login successful!");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                return BadRequest("User already exists");

            var user = new UserModel { Email = email, Password = password };
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered!");
        }
    }
}
