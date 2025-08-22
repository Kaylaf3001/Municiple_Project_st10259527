using Microsoft.AspNetCore.Mvc;
using Municiple_Project_st10259527.Services;

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

        public IActionResult SignUp()
        {
            return View();
        }
    }
}
