using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Models;
using System.Text.Json;
using technicalResoultion.Data;

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
            //if (HttpContext.User.Identity.IsAuthenticated) 
            //{
            //    var cliente = (from c in _TechResContext.externos
            //                   select c).ToList();

            //    bool confirm = false;

            //    foreach (var item in cliente)
            //    {
            //        string nombre_completo = item.nombres_e + " " + item.apellidos_e;
            //        if (nombre_completo.Equals(HttpContext.User.Identity.Name))
            //        {
            //            confirm = true;
            //            break;
            //        }
            //    }

            //    if (confirm)
            //    {
            //        Session.nombre_usuario = HttpContext.User.Identity.Name;
            //        Session.tipo_usuario = "Externo";
            //        //ViewBag.usuario = result.Principal.Identity.Name;
            //        return RedirectToAction("Index", "GestionTickets", new { area = "" });
            //    }
            //    else
            //    {
            //        Session.nombre_usuario = "";
            //        Session.tipo_usuario = "";
            //        return View();

            //    }
            //}

            //var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));

            //if(datosUsuario != null)
            //{
            //    return RedirectToAction("Index", "GestionTickets", new { area = "" });
            //}

            return View();
        }

        public IActionResult Login(string? correo, string? clave)
        {
            //await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            //    new AuthenticationProperties
            //    {
            //        RedirectUri = Url.Action("GoogleResponse")
            //    });

            if(!string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(clave))
            {
                externos? externos = (from ex in _TechResContext.externos
                                      where ex.correo_login == correo && ex.contrasena_e == clave
                                      select ex).FirstOrDefault();

                if(externos != null)
                {
                    string datosUsuario = JsonSerializer.Serialize(externos);

                    HttpContext.Session.SetString("usuario", datosUsuario);

                    return RedirectToAction("Index", "GestionTickets", new { area = "" });
                }
                else
                {
                    internos? internos = (from ex in _TechResContext.internos
                                          where ex.correo_i == correo && ex.contrasena_i == clave
                                          select ex).FirstOrDefault();

                    if(internos != null)
                    {
                        string datosUsuario = JsonSerializer.Serialize(internos);

                        HttpContext.Session.SetString("usuario", datosUsuario);

                        return RedirectToAction("Index", "SegAsigTareas", new { area = "" });
                    }
                }
            }

            return View("Index");
        }

        //public async Task<IActionResult> GoogleResponse()
        //{
        //    //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    //var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        //    //{
        //    //    claim.Issuer,
        //    //    claim.OriginalIssuer,
        //    //    claim.Type,
        //    //    claim.Value
        //    //});

        //    //var cliente = (from c in _TechResContext.externos
        //    //               select c).ToList();

        //    //bool confirm = false;

        //    //foreach (var item in cliente) 
        //    //{
        //    //    string nombre_completo = item.nombres_e + " " + item.apellidos_e;
        //    //    if (nombre_completo.Equals(result.Principal.Identity.Name))
        //    //    {
        //    //        confirm = true;
        //    //        break;
        //    //    }
        //    //}

        //    //if(confirm)
        //    //{
        //    //    Session.nombre_usuario = "";
        //    //    Session.tipo_usuario = "";
        //    //    //ViewBag.usuario = result.Principal.Identity.Name;
        //    //    return RedirectToAction("Index", "GestionTickets", new { area = "" });
        //    //}
        //    //else
        //    //{
        //    //    return View("Index");

        //    //}

        //}

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync();

            return View("Index");
        }
    }
}
