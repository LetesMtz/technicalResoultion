using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using technicalResoultion.Models;

namespace technicalResoultion.Controllers
{
    public class DashboardController : Controller
    {
        private readonly TechResContext _context;

        public DashboardController(TechResContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "DASHBOARD";

            // Obtener estadísticas
            var tickets = _context.tickets.ToList();
            var ticketsEnEspera = tickets.Count(t => t.id_estado_progreso == 4 || t.id_estado_progreso == 12);
            var ticketsEnProgreso = tickets.Count(t => t.id_estado_progreso == 5 || t.id_estado_progreso == 13);
            var ticketsResueltos = tickets.Count(t => t.id_estado_progreso == 6 || t.id_estado_progreso == 14);
            var ticketsAbiertosHoy = tickets.Count(t => t.fecha_creacion?.Date == DateTime.Today);
            var ticketsResueltosHoy = tickets.Count(t => t.fecha_ult_mod?.Date == DateTime.Today && (t.id_estado_progreso == 6 || t.id_estado_progreso == 14));
            var ticketsConResolucion = tickets
                .Where(t => (t.id_estado_progreso == 6 || t.id_estado_progreso == 14) && t.fecha_ult_mod != null && t.fecha_creacion != null);

            var tiempoPromedioResolucionHoras = ticketsConResolucion.Any() ?
                Math.Round(ticketsConResolucion.Average(t => (t.fecha_ult_mod.Value - t.fecha_creacion.Value).TotalHours), 1) :
                0;

            var empleadosConTicket = _context.asignar_tareas
                .Select(t => t.id_interno) // Seleccionar los IDs de empleados
                .Distinct() // Obtener IDs de empleados únicos
                .Count(); // Contar el número de IDs únicos de empleados

            var empleadosSinTicket = _context.internos.Count() - empleadosConTicket;

            // Obtener estadísticas
            var listaTickets = _context.tickets.ToList();
            var totalTickets = listaTickets.Count();

            var categoriasConTickets = _context.tickets
                .GroupBy(t => t.id_categoria)
                .Select(g => new { CategoriaId = g.Key, Cantidad = g.Count() })
                .OrderByDescending(c => c.Cantidad)
                .Take(3)
                .ToList();

            var tendencias = new Dictionary<string, int>();

            foreach (var categoria in categoriasConTickets)
            {
                var categoriaId = categoria.CategoriaId;
                var nombreCategoria = _context.categorias.FirstOrDefault(c => c.id_categoria == categoriaId)?.categoria;
                if (nombreCategoria != null)
                {
                    double porcentaje = (categoria.Cantidad / (double)totalTickets) * 100;
                    tendencias[nombreCategoria] = (int)Math.Round(porcentaje, 0);
                }
            }

            ViewBag.Tendencias = tendencias;

            ViewBag.TicketsEnEspera = ticketsEnEspera;
            ViewBag.TicketsEnProgreso = ticketsEnProgreso;
            ViewBag.TicketsResueltos = ticketsResueltos;
            ViewBag.TicketsAbiertosHoy = ticketsAbiertosHoy;
            ViewBag.TicketsResueltosHoy = ticketsResueltosHoy;
            ViewBag.TiempoPromedioResolucionHoras = tiempoPromedioResolucionHoras;
            ViewBag.EmpleadosConTicket = empleadosConTicket;
            ViewBag.EmpleadosSinTicket = empleadosSinTicket;


            return View();
        }
    }
}
