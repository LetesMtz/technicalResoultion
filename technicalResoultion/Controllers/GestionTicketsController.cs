using Microsoft.AspNetCore.Mvc;
using technicalResoultion.Data;
using technicalResoultion.Models;
using Firebase.Auth;
using Firebase.Storage;
using System.Text.Json;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using technicalResoultion.Services;

namespace technicalResoultion.Controllers
{
    public class GestionTicketsController : Controller
    {
        private readonly TechResContext _TechResContext;
        private IConfiguration _configuration;

        public GestionTicketsController(TechResContext context, IConfiguration configuration)
        {
            _TechResContext = context;
            _configuration = configuration; 
        }

        public IActionResult Index()
        {
            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));

            var cliente = (from c in _TechResContext.externos
                           where c.nombres_e == datosUsuario.nombres_e
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

            ViewBag.tipo_usuario = datosUsuario.tipo_usuario;

            return View();
        }

        public IActionResult CrearTicket()
        {
            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));

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
                               telefono = c.telefono
                           }).ToList();

            ViewBag.id_cliente = cliente.First().id;
            ViewBag.nombre_cliente = cliente.First().nombres + " " + cliente.First().apellidos;
            ViewBag.direccion_cliente = cliente.First().direccion;
            ViewBag.telefono_cliente = cliente.First().telefono;

            return View();
        }


        public async Task<IActionResult> CreateTicket(IFormFile archivo, int id_usuario, string nombre_problema, string descripcion, int prioridad)
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
            ticket.tipo_cliente = "Externo";
            ticket.nombre_problema = nombre_problema;
            ticket.descripcion = descripcion;
            ticket.archivos = urlArchivoCargado;
            ticket.id_estado_prioridad = prioridad;
            ticket.id_estado_progreso = 4;


            _TechResContext.tickets.Add(ticket);
            _TechResContext.SaveChanges();

            //AQUÍ VA LA PARTE DE ENVÍO DE CORREO
            correo enviarCorreo = new correo(_configuration);

            var datosUsuario = JsonSerializer.Deserialize<externos>(HttpContext.Session.GetString("usuario"));
            string correoParaEnviar = datosUsuario.correo_e; //GUARDAR CORREO DEL QUE INICIÓ SESIÓN

            enviarCorreo.enviar(correoParaEnviar, "TECHNICAL RESOLUTION: INFORMES", "Estimado usuario, se ha creado su ticket. \nNo. de seguimiento: " + ticket.id_ticket +
                "\nProblema: " + ticket.nombre_problema + "\nDescripción: " + ticket.descripcion + ". \n \nEspere nuevos informes.");




            return RedirectToAction("Index");
        }
    }
}
