using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Data;
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
            string[] nombre = Session.nombre_usuario.Split(' ');
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
                           join e2 in _TechResContext.estados
                           on t.id_estado_progreso equals e2.id_estado
                           where t.id_cliente == id && t.tipo_cliente == "Externo"
                           select new
                           {
                               id = t.id_ticket,
                               nombre = t.nombre_problema,
                               prioridad = e.nombre,
                               progreso = e2.nombre,
                               categoria = t.id_categoria,
                               fecha = t.fecha_creacion
                           }).ToList();
            ViewData["tickets"] = tickets;

            ViewBag.tipo_usuario = Session.tipo_usuario;

            return View();
        }

        public IActionResult CrearTicket()
        {
            var estadosPrioridad = (from e in _TechResContext.estados
                                    select e).ToList();
            ViewData["estadosPrioridad"] = estadosPrioridad;

            string[] nombre = Session.nombre_usuario.Split(' ');
            string nombres = nombre[0] + " " + nombre[1];

            var cliente = (from c in _TechResContext.externos
                           where c.nombres_e == nombres
                           select new
                           {
                               id = c.id_externo,
                               nombres = c.nombres_e,
                               apellidos = c.apellidos_e,
                               direccion = c.direccion,
                               telefono = c.telefono
                           }).ToList();

            ViewBag.id_cliente = cliente.First().id;
            ViewBag.nombre_cliente = cliente.First().nombres + " " + cliente.First().apellidos;
            ViewBag.direccion_cliente = cliente.First().direccion;
            ViewBag.telefono_cliente = cliente.First().telefono;

            return View();
        }

        public IActionResult CreateTicket(int id_usuario, string nombre_problema, string descripcion, int prioridad)
        {
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
