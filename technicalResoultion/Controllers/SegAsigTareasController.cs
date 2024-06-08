using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Numerics;
using technicalResoultion.Data;
using technicalResoultion.Models;
using System.Text.Json;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using technicalResoultion.Services;
using System.Linq;

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

            return View();
        }

        public IActionResult CreateTarea(int id_ticket, string tare_area)
        {
            string[] tareasAsignadas = tare_area.TrimEnd(';').Split(';');
            string[][] tareaEncargado = new string[tareasAsignadas.Length][];

            correo enviarCorreoEmpleado = new correo(_configuration);

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

                // Enviar correo al empleado
                var correoEmpleado = (from e in _TechResContext.internos
                                      join t in _TechResContext.asignar_tareas on e.id_interno equals t.id_interno
                                      where t.id_tarea == tareas.id_tarea
                                      select e.correo_i).FirstOrDefault();  // Selecciona el primer correo encontrado

                if (correoEmpleado != null)
                {
                    enviarCorreoEmpleado.enviar(correoEmpleado, "TECHNICAL RESOLUTION: TAREA ASIGNADA", "Se te ha asignado una tarea. \nTicket: " + id_ticket + " \nTarea: " + tareas.tarea);
                }
            }

            var usuarioExterno = (from t in _TechResContext.tickets
                                  join e in _TechResContext.externos on t.id_cliente equals e.id_externo
                                  join es in _TechResContext.estados on t.id_estado_progreso equals es.id_estado
                                  where t.id_ticket == id_ticket
                                  select new
                                  {
                                      id = t.id_ticket,
                                      correo = e.correo_login,
                                      estado = es.nombre
                                  }).FirstOrDefault();

            if (usuarioExterno != null)
            {
                correo enviarCorreo = new correo(_configuration);
                string correoParaEnviar = usuarioExterno.correo;

                enviarCorreo.enviar(correoParaEnviar, "TECHNICAL RESOLUTION: INFORMES", "Estimado usuario, ha habido una actualización en su ticket.\n \nNo. de seguimiento: " + id_ticket + " \nEstado de su ticket: " + usuarioExterno.estado);
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
                                categoria = t.id_categoria,
                                nombre = t.nombre_problema,
                                descripcion = t.descripcion,
                                archivo = t.archivos,
                                fecha_creacion = t.fecha_creacion,
                                fecha_modificacion = t.fecha_ult_mod,
                                prioridad = e.nombre,
                                id_prioridad = t.id_estado_prioridad,
                                id_progreso = t.id_estado_progreso,
                                id_cliente = t.id_cliente,
                                tipo_cliente = t.tipo_cliente
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

        public async Task<IActionResult> CerrarTicket(int? id, [Bind("id_ticket, id_categoria, nombre_problema, descripcion, archivos, fecha_creacion," +
    "fecha_ult_mod, id_estado_prioridad, id_estado_progreso, id_cliente, tipo_cliente")] tickets ticket)
        {
            _TechResContext.Update(ticket);
            await _TechResContext.SaveChangesAsync();

            //AQUÍ VA LA PARTE DE ENVÍO DE CORREO
            correo enviarCorreo = new correo(_configuration);

            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));
            string correoParaEnviar = datosUsuario.correo_e; //GUARDAR CORREO DEL QUE INICIÓ SESIÓN

            enviarCorreo.enviar(correoParaEnviar, "TECHNICAL RESOLUTION: INFORMES", "Estimado usuario, ha habido una actualización en su ticket.\n \nNo. de seguimiento:  " + ticket.id_ticket + " \nEstado de su ticket: " +
                "Cerrado \n \nEl caso ha sido cerrado exitosamente, favor crear un nuevo ticket en caso de presentar más inconvenientes.");

            return RedirectToAction("Index");
        }


        public IActionResult DetalleTarea(int id)
        {
            var tarea = (from at in _TechResContext.asignar_tareas
                         join i in _TechResContext.internos
                         on at.id_interno equals i.id_interno
                         join e in _TechResContext.estados
                         on at.id_estado_progreso equals e.id_estado
                         where at.id_tarea == id
                         select new
                         {
                             id_tarea = at.id_tarea,
                             id_ticket = at.id_ticket,
                             tarea = at.tarea,
                             id_progreso = at.id_estado_progreso,
                             progreso = e.nombre,
                             nombres_i = i.nombres_i,
                             apellidos_i = i.apellidos_i,
                             id_interno = at.id_interno
                         }).ToList();

            ViewBag.tarea = tarea;

            var progreso = (from e in _TechResContext.estados
                            where e.tipo_estado == "Progreso"
                            select e).ToList();

            ViewBag.id_progreso = progreso;

            return View();
        }

        public IActionResult ActualizarTarea(int id_tarea, int id_ticket, string tarea, int id_estado_progreso, int id_interno)
        {
                var tareaActualizar = _TechResContext.asignar_tareas.FirstOrDefault(at => at.id_tarea == id_tarea);

                    tareaActualizar.id_ticket = id_ticket;
                    tareaActualizar.tarea = tarea;
                    tareaActualizar.id_estado_progreso = id_estado_progreso;
                    tareaActualizar.id_interno = id_interno;

                    // Guardar los cambios en la base de datos
                    _TechResContext.SaveChanges();

            //AQUÍ VA LA PARTE DE ENVÍO DE CORREO
            correo enviarCorreo = new correo(_configuration);

            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));
            string correoParaEnviar = datosUsuario.correo_e; //GUARDAR CORREO DEL QUE INICIÓ SESIÓN

            var estadoTarea = from t in _TechResContext.asignar_tareas
                              join e in _TechResContext.estados on t.id_estado_progreso equals e.id_estado
                              where t.id_tarea == tareaActualizar.id_tarea
                              select e.nombre;


            enviarCorreo.enviar(correoParaEnviar, "TECHNICAL RESOLUTION: INFORMES", "Estimado usuario, ha habido una actualización en su ticket.\n \nNo. de seguimiento:  " + tareaActualizar.id_ticket + " \nEstado de su ticket: " +
                "En progreso \n \nTarea actual: " + tareaActualizar.tarea + "\nEstado de tarea: " + estadoTarea + ".");





            // Redireccionar a la acción DetalleTarea con el ID de la tarea
            return RedirectToAction("Index", new { id = id_tarea });

        }


    }
}
