﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Numerics;
using technicalResoultion.Data;
using technicalResoultion.Models;
using System.Text.Json;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;

namespace technicalResoultion.Controllers
{
    public class SegAsigTareasController : Controller
    {
        public static int id_ticket;
        private readonly TechResContext _TechResContext;

        public SegAsigTareasController(TechResContext context)
        {
            _TechResContext = context;
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

            return RedirectToAction("Index");
        }
    }
}
