using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Onlinevotingsystem.Models;
using System.Diagnostics;

namespace Onlinevotingsystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            
            if (x != "admin" && y != "admin")
            {
                return RedirectToAction("Login", "Users", new { area = "" });
            }else if(x == "admin" && y == "admin")
            {
                return View();
            }
            return RedirectToAction("Homepage", "Users", new { area = "" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}