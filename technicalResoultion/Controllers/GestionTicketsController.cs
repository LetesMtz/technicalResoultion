using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Data;
using technicalResoultion.Models;
using Firebase.Auth;
using Firebase.Storage;
using System.Text.Json;
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
            int id = 0;
            string tipo_usuario = "";

            var datosExternos = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));

            if(datosExternos.nombres_e != null)
            {
                var cliente = (from c in _TechResContext.externos
                               where c.nombres_e == datosExternos.nombres_e
                               select new
                               {
                                   id = c.id_externo,
                                   tipo_usuario = c.tipo_usuario
                               }).ToList();

                foreach (var item in cliente)
                {
                    ViewBag.usuario = item.id;
                    ViewBag.tipo_usuario = item.tipo_usuario;
                }

                id = ViewBag.usuario;
                tipo_usuario = ViewBag.tipo_usuario;
            }
            else
            {
                var datosInternos = JsonSerializer.Deserialize<internos>(HttpContext.Session.GetString("usuario"));

                var cliente = (from c in _TechResContext.internos
                               where c.nombres_i == datosInternos.nombres_i
                               select new
                               {
                                   id = c.id_interno,
                                   tipo_usuario = c.tipo_usuario
                               }).ToList();

                foreach (var item in cliente)
                {
                    ViewBag.usuario = item.id;
                    ViewBag.tipo_usuario = item.tipo_usuario;
                }

                id = ViewBag.usuario;
                tipo_usuario = ViewBag.tipo_usuario;
            }

            var tickets = (from t in _TechResContext.tickets
                           join e in _TechResContext.estados
                           on t.id_estado_prioridad equals e.id_estado
                           join e2 in _TechResContext.estados
                           on t.id_estado_progreso equals e2.id_estado
                           where t.id_cliente == id && t.tipo_cliente == tipo_usuario
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

            ViewBag.tipo_usuario = tipo_usuario;

            return View();
        }

        public IActionResult CrearTicket()
        {
            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));

            if (datosUsuario.nombres_e != null )
            {
                var estadosPrioridad = (from e in _TechResContext.estados
                                        select e).ToList();
                ViewData["estadosPrioridad"] = estadosPrioridad;

                var cliente = (from c in _TechResContext.externos
                               where c.nombres_e == datosUsuario.nombres_e
                               select new
                               {
                                   id = c.id_externo,
                                   nombres = c.nombres_e,
                                   apellidos = c.apellidos_e,
                                   direccion = c.direccion,
                                   telefono = c.telefono,
                                   tipo_usuario = c.tipo_usuario,
                               }).ToList();

                ViewBag.id_cliente = cliente.First().id;
                ViewBag.nombre_cliente = cliente.First().nombres + " " + cliente.First().apellidos;
                ViewBag.direccion_cliente = cliente.First().direccion;
                ViewBag.telefono_cliente = cliente.First().telefono;
                ViewBag.tipo_usuario = cliente.First().tipo_usuario;
            }
            else
            {
                var datosInternos = JsonSerializer.Deserialize<internos>(HttpContext.Session.GetString("usuario"));

                var estadosPrioridad = (from e in _TechResContext.estados
                                        select e).ToList();
                ViewData["estadosPrioridad"] = estadosPrioridad;

                var cliente = (from c in _TechResContext.internos
                               where c.nombres_i == datosInternos.nombres_i
                               select new
                               {
                                   id = c.id_interno,
                                   nombres = c.nombres_i,
                                   apellidos = c.apellidos_i,
                                   tipo_usuario = c.tipo_usuario,
                                   //direccion = c.direccion,
                                   //telefono = c.telefono
                               }).ToList();

                ViewBag.id_cliente = cliente.First().id;
                ViewBag.nombre_cliente = cliente.First().nombres + " " + cliente.First().apellidos;
                ViewBag.tipo_usuario = cliente.First().tipo_usuario;
                //ViewBag.direccion_cliente = cliente.First().direccion;
                //ViewBag.telefono_cliente = cliente.First().telefono;
            }

            return View();
        }

        public async Task<IActionResult> CreateTicket(IFormFile archivo, int id_usuario, string nombre_problema, string descripcion, int prioridad, string tipo_usuario)
        {
            var urlArchivoCargado = "";

            if (archivo != null)
            {
                //Leemos el archivo subido
                Stream archivoASubir = archivo.OpenReadStream();

                //Configuramos la conexion hacia FireBase
                string email = "carlos.murga1@catolica.edu.sv";
                string clave = "h1n12002";
                string ruta = "dulcesabor-imagenes.appspot.com";
                string api_key = "AIzaSyCUmhGjhkkuvkE5S5bnPXjTHIYn9qW5pl4";

                var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
                var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);

                var cancellation = new CancellationTokenSource();
                var tokenUser = autenticarFireBase.FirebaseToken;

                var tareaCargarArchivo = new FirebaseStorage(ruta,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                        ThrowOnCancel = true
                    }).Child("ImgPrueba").Child(archivo.FileName).PutAsync(archivoASubir, cancellation.Token);

                urlArchivoCargado = await tareaCargarArchivo;
            }

            tickets ticket = new tickets();

            ticket.id_cliente = id_usuario;
            ticket.tipo_cliente = tipo_usuario;
            ticket.nombre_problema = nombre_problema;
            ticket.descripcion = descripcion;
            ticket.archivos = urlArchivoCargado;
            ticket.id_estado_prioridad = prioridad;
            ticket.id_estado_progreso = 4;

            _TechResContext.tickets.Add(ticket);
            _TechResContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
