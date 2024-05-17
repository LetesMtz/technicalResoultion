using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class internos
    {
        [Key]
        public int id_interno {  get; set; }
        public string? nombres_i {  get; set; }
        public string? apellidos_i { get; set; }
        public string? tipo_usuario { get; set; }
        public string? correo_i { get; set; }
        public string? contrasena_i { get; set; }
        public int? id_role { get; set; }
    }
}
