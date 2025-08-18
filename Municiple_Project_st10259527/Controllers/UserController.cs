using Microsoft.AspNetCore.Mvc;

namespace Municiple_Project_st10259527.Controllers
{
    public class UserController : Controller
    {
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
