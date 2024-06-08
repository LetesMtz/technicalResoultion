using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using technicalResoultion.Models;
using technicalResoultion.Services;

namespace technicalResoultion.Controllers
{
    public class externosController : Controller
    {
        private readonly TechResContext _context;
        private IConfiguration _configuration;

        private bool estaRegistrado = false;

        public externosController(TechResContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var externos = _context.externos.ToList();
            ViewData["externos"] = externos;
            return View();

        }

        // Acción para mostrar la vista de registro
        public IActionResult Register()
        {
            return View();
        }

        // Acción para manejar el registro de un nuevo usuario externo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(externos externo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(externo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tu información ha sido registrada. En unos momentos nos comunicaremos contigo.";
                return RedirectToAction("Index", "Login");
            }
            return View(externo);
        }
        public IActionResult Details(int id)
        {
            var externo = _context.externos.Find(id);
            if (externo == null)
            {
                return NotFound();
            }
            ViewData["externo"] = externo; // Asignar el objeto "externo" al ViewData
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(externos externo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(externo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(externo);
        }

        public IActionResult Edit(int id)
        {

            var externo = _context.externos.Find(id);
            if (externo == null)
            {
                return NotFound();
            }

            //Primero revisamos que efectivamente no haya sido registrado
            estaRegistrado = !string.IsNullOrEmpty(externo.correo_login);


            return View(externo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, externos externo)
        {
            if (id != externo.id_externo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    



                    _context.Update(externo);
                    await _context.SaveChangesAsync();

                    //Correo
                    if (!estaRegistrado)
                    {
                        correo enviarCorreo = new correo(_configuration);

                        string correoParaEnviar = string.IsNullOrEmpty(externo.correo_login) ? externo.correo_e : externo.correo_login;

                        enviarCorreo.enviar(correoParaEnviar, "TECHNICAL RESOLUTION: USUARIO REGISTRADO",
                            "Estimado, " + externo.nombres_e + ", ¡Bienvenido a nuestro sistema de tickets! \n \n" +
                            "Estas son sus credenciales para ingresar a nuestro sitio. \n \nCorreo asignado: " + externo.correo_login +
                            "\nContraseña: " + externo.contrasena_e + "\n \nSerá un gusto atenderle.");
                    }



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!externosExists(externo.id_externo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(externo);
        }

        private bool externosExists(int id)
        {
            return _context.externos.Any(e => e.id_externo == id);
        }
    }
}

