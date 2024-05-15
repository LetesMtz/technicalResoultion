using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class externos
    {
        [Key]
        public int id_externo { get; set; }
        public string? nombres_e { get; set; }
        public string? apellidos_e { get; set; }
        public string? tipo_usuario { get; set; }
        public string? correo_e { get; set; }
        public string? correo_login { get; set; }
        public string? contrasena_e { get; set; }
        public string? nombre_empresa { get; set; }
        public string? contacto_principal { get; set; }
        public string? direccion { get; set; }
        public int? telefono { get; set; }
        public int? id_estado { get; set; }

        public static string? nombre_externo_static { get; set; }
    }
}
