using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Numerics;
using technicalResoultion.Data;
using technicalResoultion.Models;
using System.Text.Json;
using technicalResoultion.Services;

namespace technicalResoultion.Controllers
{
    public class SegAsigTareasController : Controller
    {
        public static int id_ticket;
        private readonly TechResContext _TechResContext;
        private IConfiguration _configuration;

        public SegAsigTareasController(TechResContext context, IConfiguration configuration)
        {
            _TechResContext = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var tickets = (from t in _TechResContext.tickets
                           join e in _TechResContext.estados
                           on t.id_estado_prioridad equals e.id_estado
                           join e2 in _TechResContext.estados
                           on t.id_estado_progreso equals e2.id_estado
                           select new
                           {
                               id = t.id_ticket,
                               nombre = t.nombre_problema,
                               prioridad = e.nombre,
                               progreso = e2.nombre
                           }).ToList();

            ViewBag.Tickets = tickets;

            return View();
        }

        public IActionResult AsignarTarea()
        {
            var tickets = (from t in _TechResContext.tickets
                           join e in _TechResContext.estados
                           on t.id_estado_prioridad equals e.id_estado
                           join e2 in _TechResContext.estados
                           on t.id_estado_progreso equals e2.id_estado
                           select new
                           {
                               id = t.id_ticket,
                               nombre = t.nombre_problema,
                               prioridad = e.nombre,
                               progreso = e2.nombre
                           }).ToList();

            ViewBag.Tickets = tickets;

            var internos = (from i in _TechResContext.internos
                            join r in _TechResContext.roles
                            on i.id_role equals r.id_role
                            select new
                            {
                                id = i.id_interno,
                                nombres = i.nombres_i,
                                apellidos = i.apellidos_i,
                                rol = r.nombre_role
                            }).ToList();

            ViewBag.Internos = internos;


            correo enviarCorreo = new correo(_configuration);

            enviarCorreo.enviar("soymariohdez@gmail.com", "Prueba Asunto", "Esta es una NUEVA prueba de correo");

            return View();
        }

        public IActionResult CreateTarea(int id_ticket, string tare_area)
        {
            string[] tareasAsignadas = tare_area.TrimEnd(';').Split(';');
            string[][] tareaEncargado = new string[tareasAsignadas.Length][];

            for (int i = 0; i < tareasAsignadas.Length; i++)
            {
                tareaEncargado[i] = tareasAsignadas[i].Split(",");
            }

            for (int i = 0; i < tareaEncargado.Length; i++)
            {
                asignar_tareas tareas = new asignar_tareas();
                tareas.id_ticket = id_ticket;
                tareas.tarea = tareaEncargado[i][0];
                tareas.id_interno = Int32.Parse(tareaEncargado[i][1]);

                _TechResContext.asignar_tareas.Add(tareas);
                _TechResContext.SaveChanges();
            }

            return RedirectToAction("AsignarTarea");
        }

        public IActionResult TareasYComentarios(int id)
        {
            if(id > 0)
            {
                id_ticket = id;
            }

            var detalles = (from t in _TechResContext.tickets
                            join e in _TechResContext.estados
                            on t.id_estado_prioridad equals e.id_estado
                            where t.id_ticket == id_ticket
                            select new
                            {
                                id = t.id_ticket,
                                nombre = t.nombre_problema,
                                archivo = t.archivos,
                                descripcion = t.descripcion,
                                prioridad = e.nombre
                            }).ToList();
            ViewBag.Tickets = detalles;

            var tareas = (from at in _TechResContext.asignar_tareas
                          join e in _TechResContext.estados
                          on at.id_estado_progreso equals e.id_estado
                          join i in _TechResContext.internos
                          on at.id_interno equals i.id_interno
                          where at.id_ticket == id_ticket
                          select new
                          {
                              id = at.id_tarea,
                              nombre = at.tarea,
                              progreso = e.nombre,
                              nombres_i = i.nombres_i,
                              apellidos_i = i.apellidos_i
                          }).ToList();
            ViewBag.Tareas = tareas;

            var comentarios = (from c in _TechResContext.comentarios
                               join i in _TechResContext.internos
                               on c.id_interno equals i.id_interno
                               join r in _TechResContext.roles
                               on i.id_role equals r.id_role
                               where c.id_ticket == id_ticket
                               select new
                               {
                                   nombres = i.nombres_i,
                                   apellidos = i.apellidos_i,
                                   rol = r.nombre_role,
                                   fecha = c.fecha,
                                   comentario = c.comentario
                               }).ToList().OrderByDescending(x => x.fecha);
            ViewBag.Comentarios = comentarios;

            return View();
        }

        public IActionResult CreateComentario(int id_ticket, string comentario)
        {
            var datosUsuario = JsonSerializer.Deserialize<internos>(HttpContext.Session.GetString("usuario"));

            comentarios com = new comentarios();
            com.comentario = comentario;
            com.id_ticket = id_ticket;
            com.id_interno = datosUsuario.id_interno;

            _TechResContext.comentarios.Add(com);
            _TechResContext.SaveChanges();

            return RedirectToAction("TareasYComentarios");
        }
    }
}
