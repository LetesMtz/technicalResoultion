using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using technicalResoultion.Models;
using technicalResolution.Attributes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace technicalResoultion.Controllers
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
            return View();
        }


        [Authorize(Roles = "Administrador,Desarrollador,Funcionalidad,Auditor,Externo")]
        public IActionResult Index1()
        {
            return View();
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
