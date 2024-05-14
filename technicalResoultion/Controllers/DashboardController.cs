using Microsoft.AspNetCore.Mvc;

namespace technicalResoultion.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "DASHBOARD";
            return View();
        }
    }
}
