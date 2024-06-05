using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using technicalResoultion.Models;

namespace technicalResoultion.Controllers
{
    public class internosController : Controller
    {
        private readonly TechResContext _context;

        public internosController(TechResContext context)
        {
            _context = context;
        }
        public IActionResult Index1()
        {
            var internos = (from i in _context.internos
                            join r in _context.roles
                            on i.id_role equals r.id_role
                            select new
                            {
                                i.id_interno,
                                Nombre = i.nombres_i + " " + i.apellidos_i,
                                Rol = r.nombre_role
                            }).ToList();

            ViewData["internos"] = internos;

            return View();
        }

        public IActionResult Detalles(int id)
        {
            var interno = (from i in _context.internos
                           join r in _context.roles
                           on i.id_role equals r.id_role
                           where i.id_interno == id
                           select new
                           {
                               i.id_interno,
                               i.nombres_i,
                               i.apellidos_i,
                               i.tipo_usuario,
                               i.correo_i,
                               i.id_role,
                               Rol = r.nombre_role,
                               r.permisos_descripcion
                           }).FirstOrDefault();

            if (interno == null)
            {
                return NotFound();
            }

            ViewData["interno"] = interno;

            return View();
        }


        public IActionResult CrearInterno()
        {
            var roles = _context.roles.ToList();
            ViewData["roles"] = roles;

            return View();
        }

        [HttpPost]
        public IActionResult CrearInterno(internos interno)
        {
            if (ModelState.IsValid)
            {
                interno.tipo_usuario = "Interno"; //tipo de usuario sea siempre "Interno"
                _context.internos.Add(interno);
                _context.SaveChanges();
                return RedirectToAction("Index1");
            }

            var roles = _context.roles.ToList();
            ViewData["roles"] = roles;

            return View(interno);
        }



        // GET: internos
        public async Task<IActionResult> Index()
        {
            return View(await _context.internos.ToListAsync());
        }

        // GET: internos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internos = await _context.internos
                .FirstOrDefaultAsync(m => m.id_interno == id);
            if (internos == null)
            {
                return NotFound();
            }

            return View(internos);
        }

        // GET: internos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: internos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_interno,nombres_i,apellidos_i,tipo_usuario,correo_i,contrasena_i,id_role")] internos internos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(internos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(internos);
        }

        // GET: internos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internos = await _context.internos.FindAsync(id);
            if (internos == null)
            {
                return NotFound();
            }
            return View(internos);
        }

        // POST: internos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_interno,nombres_i,apellidos_i,tipo_usuario,correo_i,contrasena_i,id_role")] internos internos)
        {
            if (id != internos.id_interno)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(internos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!internosExists(internos.id_interno))
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
            return View(internos);
        }

        // GET: internos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internos = await _context.internos
                .FirstOrDefaultAsync(m => m.id_interno == id);
            if (internos == null)
            {
                return NotFound();
            }

            return View(internos);
        }

        // POST: internos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var internos = await _context.internos.FindAsync(id);
            if (internos != null)
            {
                _context.internos.Remove(internos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool internosExists(int id)
        {
            return _context.internos.Any(e => e.id_interno == id);
        }
    }
}
