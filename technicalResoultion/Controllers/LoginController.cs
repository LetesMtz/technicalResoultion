using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Models;

namespace technicalResoultion.Controllers
{
    public class LoginController : Controller
    {
        private readonly TechResContext _TechResContext;

        public LoginController(TechResContext context)
        {
            _TechResContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var cliente = (from c in _TechResContext.externos
                           select c).ToList();

            bool confirm = false;

            foreach (var item in cliente) 
            {
                string nombre_completo = item.nombres_e + " " + item.apellidos_e;
                if (nombre_completo.Equals(result.Principal.Identity.Name))
                {
                    confirm = true;
                    break;
                }
            }

            if(confirm)
            {
                externos.nombre_externo_static = result.Principal.Identity.Name;
                //ViewBag.usuario = result.Principal.Identity.Name;
                return RedirectToAction("Index", "GestionTickets", new { area = "" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });

            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return View("Index");
        }
    }
}
