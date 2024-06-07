using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Models;
using System.Text.Json;
using technicalResoultion.Data;
using System.Security.Claims;


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


        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            if (!string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(clave))
            {
                var externo = _TechResContext.externos.FirstOrDefault(ex => ex.correo_login == correo && ex.contrasena_e == clave);

                if (externo != null)
                {
                    string datosUsuario = JsonSerializer.Serialize(externo);
                    HttpContext.Session.SetString("usuario", datosUsuario);
                    await SignInUser(externo.correo_login, "Externo", "Externo");
                    return RedirectToAction("Index1", "Home");
                }
                else
                {
                    var interno = _TechResContext.internos.FirstOrDefault(ex => ex.correo_i == correo && ex.contrasena_i == clave);

                    if (interno != null)
                    {
                        string datosUsuario = JsonSerializer.Serialize(interno);
                        HttpContext.Session.SetString("usuario", datosUsuario);

                        var role = _TechResContext.roles.FirstOrDefault(r => r.id_role == interno.id_role)?.nombre_role;
                        await SignInUser(interno.correo_i, role, "Interno");
                        return RedirectToAction("Index1", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View("Index");
        }

        private async Task SignInUser(string correo, string role, string tipoUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, correo),
                new Claim(ClaimTypes.Role, role),
                new Claim("TipoUsuario", tipoUsuario)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }



        public IActionResult Login1(string? correo, string? clave)
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

                    return RedirectToAction("Index1", "Home", new { area = "" });
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

                        return RedirectToAction("Index1", "Home", new { area = "" });
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

        public async Task<IActionResult> Logout1()
        {
            //await HttpContext.SignOutAsync();

            return View("Index");
        }
    }
}
