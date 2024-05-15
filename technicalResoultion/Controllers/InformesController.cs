using Microsoft.AspNetCore.Mvc;

namespace technicalResoultion.Controllers
{
    public class InformesController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "INFORMES";
            return View();
        }
    }
}
