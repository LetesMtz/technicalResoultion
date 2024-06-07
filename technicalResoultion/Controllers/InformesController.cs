using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Models;

namespace technicalResoultion.Controllers
{
    public class InformesController : Controller
    {

        private readonly TechResContext _TechResContext;

        public InformesController(TechResContext context)
        {
            _TechResContext = context;
        }

        public IActionResult Index(string estadoFiltro, string prioridadFiltro)
        {
            var ticketsQuery = from t in _TechResContext.tickets
                               join c in _TechResContext.categorias on t.id_categoria equals c.id_categoria
                               join e in _TechResContext.estados on t.id_estado_prioridad equals e.id_estado
                               join es in _TechResContext.estados on t.id_estado_progreso equals es.id_estado
                               join i in _TechResContext.internos on t.id_cliente equals i.id_interno
                               select new
                               {
                                   N_ticket = t.id_ticket,
                                   nombre = t.nombre_problema,
                                   atiende = i.nombres_i + " " + i.apellidos_i,
                                   categoria = c.categoria,
                                   prioridad = e.nombre,
                                   fecha = t.fecha_creacion,
                                   progreso = es.nombre
                               };

            if (!string.IsNullOrEmpty(estadoFiltro))
            {
                ticketsQuery = ticketsQuery.Where(t => t.progreso == estadoFiltro);
            }

            if (!string.IsNullOrEmpty(prioridadFiltro))
            {
                ticketsQuery = ticketsQuery.Where(t => t.prioridad == prioridadFiltro);
            }

            var listaDeTickets = ticketsQuery.ToList();
            ViewBag.listaDeTickets = listaDeTickets;

            // Cargar opciones de estados para el filtro de estado
            ViewBag.estados = _TechResContext.estados
                                          .Where(e => e.tipo_estado == "Progreso")
                                          .Select(e => e.nombre)
                                          .Distinct()
                                          .ToList();

            // Cargar opciones de prioridades para el filtro de prioridad
            ViewBag.prioridades = _TechResContext.estados
                                              .Where(e => e.tipo_estado == "Prioridad")
                                              .Select(e => e.nombre)
                                              .Distinct()
                                              .ToList();

            ViewBag.estadoFiltro = estadoFiltro;
            ViewBag.prioridadFiltro = prioridadFiltro;

            return View();
        }




    }
}
