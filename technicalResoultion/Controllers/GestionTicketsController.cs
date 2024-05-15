using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Models;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace technicalResoultion.Controllers
{
    public class GestionTicketsController : Controller
    {
        private readonly TechResContext _TechResContext;

        public GestionTicketsController(TechResContext context)
        {
            _TechResContext = context;
        }

        public IActionResult Index()
        {
            string[] nombre = externos.nombre_externo_static.Split(' ');
            string nombres = nombre[0] + " " + nombre[1];

            var cliente = (from c in _TechResContext.externos
                           where c.nombres_e == nombres
                           select new
                           {
                               id = c.id_externo,
                           }).ToList();

            foreach (var item in cliente)
            {
                ViewBag.cliente = item.id;
            }

            int id = ViewBag.cliente;

            var tickets = (from t in _TechResContext.tickets
                           join e in _TechResContext.estados
                           on t.id_estado_prioridad equals e.id_estado
                           where t.id_cliente == id && t.tipo_cliente == "Externo"
                           select new
                           {
                               id = t.id_ticket,
                               nombre = t.nombre_problema,
                               prioridad = e.nombre,
                               categoria = t.id_categoria,
                               fecha = t.fecha_creacion
                           }).ToList();
            ViewData["tickets"] = tickets;

            return View();
        }

        public IActionResult CrearTicket()
        {
            var estadosPrioridad = (from e in _TechResContext.estados
                                    select e).ToList();
            ViewData["estadosPrioridad"] = estadosPrioridad;

            string[] nombre = externos.nombre_externo_static.Split(' ');
            string nombres = nombre[0] + " " + nombre[1];

            var cliente = (from c in _TechResContext.externos
                           where c.nombres_e == nombres
                           select new
                           {
                               id = c.id_externo
                           }).ToList();
            
            foreach (var item in cliente)
            {
                ViewBag.cliente = item.id;
            }

            return View();
        }

        public IActionResult CreateTicket(int id_usuario, string nombre_problema, string descripcion, int prioridad)
        {
            //string[] nombre = ViewBag.usuario.split(' ');
            //string nombres = nombre[0] + " " + nombre[1];

            //var cliente = (from c in _TechResContext.externos
            //               where c.nombres_e == nombres
            //               select c).ToList();

            tickets ticket = new tickets();

            ticket.id_cliente = id_usuario;
            ticket.tipo_cliente = "Externo";
            ticket.nombre_problema = nombre_problema;
            ticket.descripcion = descripcion;
            ticket.id_estado_prioridad = prioridad;
            ticket.id_estado_progreso = 4;

            _TechResContext.tickets.Add(ticket);
            _TechResContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
