using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using System.Diagnostics;
using Municiple_Project_st10259527.ViewModels;

namespace Municiple_Project_st10259527.Controllers
{
    public class HomeController : Controller
    {
        //==============================================================================================
        // GET: Home/Index
        //==============================================================================================
        public IActionResult Index()
        {
           return View();
        }
        //=============================================================================================
        //==============================================================================================
        // GET: Home/Privacy
        //==============================================================================================
        public IActionResult Privacy()
        {
            return View();
        }
        //==============================================================================================

        //==============================================================================================
        // POST: Home/Issue
        //==============================================================================================
        [HttpPost]
        public IActionResult Issue()
        {
            return RedirectToAction("ReportIssueStep1", "Report");
        }
        //==============================================================================================

        //==============================================================================================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //==============================================================================================

    }
}
//==============================================================================================
